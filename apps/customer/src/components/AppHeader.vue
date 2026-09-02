<template>
  <header class="app-header">
    <router-link to="/" class="brand">
      <img class="brand-logo" src="~@htawpyi/shared-ui/assets/brand-ticket.svg" alt="" />
      <span class="brand-name">HtiMart</span>
    </router-link>
    <nav class="top-nav" aria-label="Main navigation">
      <router-link to="/">Home</router-link>
      <router-link to="/buy">Buy Tickets</router-link>
      <router-link to="/history">History</router-link>
      <router-link to="/how-it-works">How It Works</router-link>
    </nav>
    <div class="account">
      <template v-if="isLoggedIn">
        <router-link to="/purchases" class="account-link">{{ email }}</router-link>
        <button type="button" class="logout" @click="signOut">Log out</button>
      </template>
      <router-link v-else to="/login" class="account-link">Log in</router-link>
    </div>
  </header>
</template>

<script>
import { useAuth } from '@/composables/useAuth'

export default {
  name: 'AppHeader',
  setup() {
    const { isLoggedIn, email, logout } = useAuth()
    return { isLoggedIn, email, logout }
  },
  methods: {
    signOut() {
      this.logout()
      if (this.$route.meta.requiresAuth) this.$router.push('/')
    }
  }
}
</script>

<style scoped>
.app-header {
  position: sticky;
  top: 0;
  z-index: 20;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding: 8px 48px;
  background: rgba(255, 255, 255, 0.92);
  backdrop-filter: blur(8px);
  border-bottom: 1px solid var(--line);
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
  text-decoration: none;
  color: #2b2b2b;
  min-height: 44px;
}

.brand-logo {
  width: 46px;
  height: 46px;
  object-fit: contain;
  border-radius: 10px;
  display: block;
}

.brand-name {
  font-family: var(--font-display);
  font-size: 21px;
  font-weight: 800;
  letter-spacing: -0.01em;
  color: var(--ink);
}

.top-nav {
  display: flex;
  gap: 8px;
}

.top-nav a {
  display: inline-flex;
  align-items: center;
  min-height: 44px;
  padding: 0 16px;
  font-size: 16px;
  font-weight: 600;
  color: #2b2b2b;
  text-decoration: none;
  border-bottom: 3px solid transparent;
}

.top-nav a:hover {
  color: #d97706;
}

.top-nav a.router-link-active {
  color: #d97706;
  border-bottom-color: #d97706;
}

.account {
  display: flex;
  align-items: center;
  gap: 10px;
}

.account-link {
  display: inline-flex;
  align-items: center;
  min-height: 44px;
  padding: 0 12px;
  font-size: 15px;
  font-weight: 600;
  color: #b45309;
  text-decoration: none;
}

.account-link:hover {
  color: #92400e;
}

.logout {
  min-height: 44px;
  padding: 0 12px;
  border: none;
  background: none;
  font: inherit;
  font-size: 14px;
  font-weight: 600;
  color: var(--muted);
  cursor: pointer;
}

.logout:hover {
  color: #b3261e;
}

@media (max-width: 767px) {
  .app-header {
    padding: 10px 16px;
  }

  /* On mobile the bottom tab bar takes over navigation. */
  .top-nav {
    display: none;
  }
}
</style>
