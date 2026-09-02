<template>
  <div class="login-wrap">
    <form class="login-card" @submit.prevent="submit">
      <div class="login-brand">
        <img src="~@htawpyi/shared-ui/assets/brand-ticket.svg" alt="HtiMart" class="login-logo" />
        <h1>HtiMart Admin</h1>
      </div>
      <label>
        <span>Email</span>
        <input v-model.trim="email" type="email" autocomplete="username" required />
      </label>
      <label>
        <span>Password</span>
        <input v-model="password" type="password" autocomplete="current-password" required />
      </label>
      <p v-if="error" class="error" role="alert">{{ error }}</p>
      <button type="submit" :disabled="busy">{{ busy ? 'Signing in…' : 'Log in' }}</button>
    </form>
  </div>
</template>

<script>
import { login } from '@/services/adminApi'
import { authState } from '@/App.vue'

export default {
  name: 'AdminLogin',
  data() {
    return { email: '', password: '', error: null, busy: false }
  },
  methods: {
    async submit() {
      this.busy = true
      this.error = null
      try {
        authState.value = await login(this.email, this.password)
        this.$router.push('/')
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
.login-wrap {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  box-sizing: border-box;
}

.login-card {
  width: 100%;
  max-width: 380px;
  display: flex;
  flex-direction: column;
  gap: 14px;
  padding: 28px 32px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.login-brand {
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 6px;
}

.login-logo {
  width: 52px;
  height: 52px;
  border-radius: 12px;
  box-shadow: var(--shadow);
}

.login-card h1 {
  margin: 0;
  font-size: 24px;
}

label {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 14px;
  font-weight: 600;
  color: var(--muted);
}

input {
  min-height: 48px;
  padding: 0 14px;
  border: 1px solid var(--line);
  border-radius: 8px;
  font: inherit;
  font-size: 16px;
}

input:focus {
  outline: 2px solid var(--amber);
  outline-offset: 1px;
}

.error {
  margin: 0;
  font-size: 14px;
  color: #b3261e;
}

button {
  min-height: 48px;
  border: none;
  border-radius: 8px;
  background: var(--amber);
  color: #ffffff;
  font: inherit;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
}

button:hover {
  background: var(--amber-dark);
}

button:disabled {
  background: #c9c9c9;
  cursor: not-allowed;
}
</style>
