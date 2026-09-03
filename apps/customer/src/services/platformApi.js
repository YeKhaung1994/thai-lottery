import axios from 'axios'

// Platform API (apps/api). '/api' is proxied to the .NET API by the dev
// server (vue.config.js) and by the production reverse proxy.
const client = axios.create({ baseURL: '/api', timeout: 15000 })

const AUTH_KEY = 'htawpyi-auth'

export function loadAuth() {
  try {
    const stored = JSON.parse(localStorage.getItem(AUTH_KEY))
    return stored && stored.accessToken ? stored : null
  } catch {
    return null
  }
}

export function saveAuth(auth) {
  try {
    if (auth) localStorage.setItem(AUTH_KEY, JSON.stringify(auth))
    else localStorage.removeItem(AUTH_KEY)
  } catch {
    // Storage unavailable — session lives in memory only.
  }
}

let currentAuth = loadAuth()

export function setAuth(auth) {
  currentAuth = auth
  saveAuth(auth)
}

export function getAuth() {
  return currentAuth
}

client.interceptors.request.use((config) => {
  if (currentAuth?.accessToken) {
    config.headers.Authorization = `Bearer ${currentAuth.accessToken}`
  }
  return config
})

// Listeners fired when the session ends on token expiry (toast, redirect,
// ref sync). Multiple parts of the app can subscribe.
const expiredHandlers = []
export function onSessionExpired(fn) {
  expiredHandlers.push(fn)
}

function expireSession() {
  const hadSession = !!currentAuth
  setAuth(null)
  if (hadSession) expiredHandlers.forEach((fn) => fn())
}

let refreshing = null

client.interceptors.response.use(undefined, async (error) => {
  const { response, config } = error
  if (response?.status !== 401 || config._retried) throw error

  // 401 on an authenticated request: try to refresh, else the session is over.
  if (!currentAuth?.refreshToken) {
    expireSession()
    throw error
  }
  config._retried = true
  refreshing = refreshing || axios
    .post('/api/auth/refresh', { refreshToken: currentAuth.refreshToken })
    .then(({ data }) => setAuth(data))
    .catch(() => expireSession())
    .finally(() => {
      refreshing = null
    })
  await refreshing
  if (!currentAuth) throw error
  return client(config)
})

function message(error) {
  return error.response?.data?.title || error.message || 'Something went wrong'
}

async function call(promise) {
  try {
    return (await promise).data
  } catch (error) {
    throw new Error(message(error))
  }
}

export const register = (email, password) =>
  call(client.post('/auth/register', { email, password })).then((auth) => {
    setAuth(auth)
    return auth
  })

export const login = (email, password) =>
  call(client.post('/auth/login', { email, password })).then((auth) => {
    setAuth(auth)
    return auth
  })

export const logout = () => setAuth(null)

export const searchTickets = (query) =>
  call(client.get('/tickets', { params: { q: query || undefined } }))

export const createOrder = (ticketIds) =>
  call(client.post('/orders', { ticketIds }))

export const myOrders = () => call(client.get('/orders/mine'))

export const mockConfirm = (paymentId, success) =>
  call(client.post(`/payments/${paymentId}/mock-confirm`, null, { params: { success } }))
