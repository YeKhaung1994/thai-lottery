import axios from 'axios'

const client = axios.create({ baseURL: '/api', timeout: 15000 })

const AUTH_KEY = 'htawpyi-admin-auth'

function load() {
  try {
    const stored = JSON.parse(localStorage.getItem(AUTH_KEY))
    return stored && stored.accessToken ? stored : null
  } catch {
    return null
  }
}

let currentAuth = load()

export function setAuth(auth) {
  currentAuth = auth
  try {
    if (auth) localStorage.setItem(AUTH_KEY, JSON.stringify(auth))
    else localStorage.removeItem(AUTH_KEY)
  } catch {
    // Storage unavailable — session lives in memory only.
  }
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

function message(error) {
  return error.response?.data?.title || error.message || 'Something went wrong'
}

// Listeners fired when the session ends on token expiry.
const expiredHandlers = []
export function onSessionExpired(fn) {
  expiredHandlers.push(fn)
}

// Admin has no refresh token, so a 401 on an authenticated call means the
// session is over. (Login itself runs with no auth, so it is exempt.)
client.interceptors.response.use(undefined, (error) => {
  if (error.response?.status === 401 && currentAuth) {
    setAuth(null)
    expiredHandlers.forEach((fn) => fn())
  }
  throw error
})

async function call(promise) {
  try {
    return (await promise).data
  } catch (error) {
    const err = new Error(message(error))
    err.status = error.response?.status
    throw err
  }
}

export async function login(email, password) {
  const auth = await call(client.post('/auth/login', { email, password }))
  if (auth.role !== 'Admin') {
    throw new Error('This account is not an admin.')
  }
  setAuth(auth)
  return auth
}

export const logout = () => setAuth(null)

export const uploadTickets = (rows) => call(client.post('/admin/tickets', rows))

export const listTickets = (drawDate, status) =>
  call(client.get('/admin/tickets', {
    params: { drawDate: drawDate || undefined, status: status || undefined }
  }))

export const deleteTicket = (id) => call(client.delete(`/admin/tickets/${id}`))

export const listOrders = () => call(client.get('/admin/orders'))
