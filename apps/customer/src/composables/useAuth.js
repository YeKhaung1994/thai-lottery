import { computed, ref } from 'vue'
import * as api from '@/services/platformApi'

const auth = ref(api.getAuth())

// Keep this ref in sync when the API clears the session on token expiry,
// so the header reflects the logged-out state immediately.
api.onSessionExpired(() => {
  auth.value = null
})

export function useAuth() {
  async function login(email, password) {
    auth.value = await api.login(email, password)
  }

  async function register(email, password) {
    auth.value = await api.register(email, password)
  }

  function logout() {
    api.logout()
    auth.value = null
  }

  return {
    auth,
    isLoggedIn: computed(() => !!auth.value),
    email: computed(() => auth.value?.email || null),
    login,
    register,
    logout
  }
}
