<template>
  <div class="auth-page">
    <div class="auth-brand">
      <img src="~@htawpyi/shared-ui/assets/brand-ticket.svg" alt="HtiMart" class="auth-logo" />
      <h1>{{ mode === 'login' ? 'Log in' : 'Create account' }}</h1>
    </div>
    <form class="auth-form" @submit.prevent="submit">
      <label>
        <span>Email</span>
        <input v-model.trim="email" type="email" autocomplete="email" required />
      </label>
      <label>
        <span>Password</span>
        <input
          v-model="password"
          type="password"
          :autocomplete="mode === 'login' ? 'current-password' : 'new-password'"
          minlength="8"
          required
        />
      </label>
      <p v-if="error" class="error" role="alert">{{ error }}</p>
      <button type="submit" class="primary" :disabled="busy">
        {{ busy ? 'Please wait…' : mode === 'login' ? 'Log in' : 'Register' }}
      </button>
    </form>
    <button type="button" class="switch" @click="toggleMode">
      {{ mode === 'login' ? "No account yet? Register" : 'Already registered? Log in' }}
    </button>
  </div>
</template>

<script>
import { useAuth } from '@/composables/useAuth'

export default {
  name: 'LoginView',
  setup() {
    const { login, register } = useAuth()
    return { doLogin: login, doRegister: register }
  },
  data() {
    return {
      mode: 'login',
      email: '',
      password: '',
      error: null,
      busy: false
    }
  },
  methods: {
    toggleMode() {
      this.mode = this.mode === 'login' ? 'register' : 'login'
      this.error = null
    },
    async submit() {
      this.busy = true
      this.error = null
      try {
        if (this.mode === 'login') await this.doLogin(this.email, this.password)
        else await this.doRegister(this.email, this.password)
        this.$router.push(this.$route.query.next || '/buy')
      } catch (err) {
        this.error = err.message
      } finally {
        this.busy = false
      }
    }
  }
}
</script>

<style scoped>
.auth-page {
  max-width: 420px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 18px;
  text-align: left;
}

.auth-brand {
  display: flex;
  align-items: center;
  gap: 14px;
}

.auth-logo {
  width: 56px;
  height: 56px;
  border-radius: 12px;
  box-shadow: var(--shadow);
}

.auth-page h1 {
  margin: 0;
  font-size: 28px;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 14px;
  padding: 24px 28px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.auth-form label {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 14px;
  font-weight: 600;
  color: var(--muted);
}

.auth-form input {
  min-height: 48px;
  padding: 0 14px;
  border: 1px solid var(--line);
  border-radius: 8px;
  font: inherit;
  font-size: 16px;
}

.auth-form input:focus {
  outline: 2px solid #d97706;
  outline-offset: 1px;
}

.error {
  margin: 0;
  font-size: 14px;
  color: #b3261e;
}

.primary {
  min-height: 48px;
  border: none;
  border-radius: 8px;
  background: #d97706;
  color: #ffffff;
  font: inherit;
  font-size: 17px;
  font-weight: 700;
  cursor: pointer;
}

.primary:hover {
  background: #b45309;
}

.primary:disabled {
  background: #c9c9c9;
  cursor: not-allowed;
}

.switch {
  align-self: flex-start;
  min-height: 44px;
  padding: 0;
  border: none;
  background: none;
  font: inherit;
  font-size: 15px;
  font-weight: 600;
  color: #b45309;
  cursor: pointer;
}

.switch:hover {
  color: #92400e;
}
</style>
