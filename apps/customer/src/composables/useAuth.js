import { computed, ref } from 'vue'
import * as api from '@/services/platformApi'

const auth = ref(api.getAuth())

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
