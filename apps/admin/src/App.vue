<template>
  <template v-if="isLoggedIn">
    <div class="layout">
      <aside class="sidebar">
        <div class="brand">
          <img src="~@htawpyi/shared-ui/assets/brand-ticket.svg" alt="" class="brand-logo" />
          <span>HtiMart Admin</span>
        </div>
        <nav class="side-nav" aria-label="Admin navigation">
          <router-link to="/"><AppIcon name="sparkle" :size="18" /> Dashboard</router-link>
          <router-link to="/upload"><AppIcon name="ticket" :size="18" /> Upload Tickets</router-link>
          <router-link to="/inventory"><AppIcon name="hash" :size="18" /> Inventory</router-link>
          <router-link to="/sales"><AppIcon name="trophy" :size="18" /> Sales</router-link>
        </nav>
        <div class="session">
          <span class="who">{{ email }}</span>
          <button type="button" class="logout" @click="signOut">Log out</button>
        </div>
      </aside>
      <main class="content">
        <router-view />
      </main>
    </div>
  </template>
  <router-view v-else />
  <ToastHost />
</template>

<script>
import { computed, ref } from 'vue'
import { AppIcon, ToastHost } from '@htawpyi/shared-ui'
import { getAuth, logout } from '@/services/adminApi'

export const authState = ref(getAuth())

export default {
  name: 'App',
  components: { AppIcon, ToastHost },
  setup() {
    return {
      isLoggedIn: computed(() => !!authState.value),
      email: computed(() => authState.value?.email || '')
    }
  },
  methods: {
    signOut() {
      logout()
      authState.value = null
      this.$router.push('/login')
    }
  }
}
</script>

<style>
body {
  margin: 0;
  background: var(--cream);
}

#app {
  font-family: Avenir, Helvetica, Arial, 'Noto Sans Myanmar', sans-serif;
  -webkit-font-smoothing: antialiased;
  color: var(--ink);
}

.layout {
  display: flex;
  min-height: 100vh;
}

.sidebar {
  display: flex;
  flex-direction: column;
  gap: 24px;
  width: 220px;
  flex-shrink: 0;
  padding: 20px 16px;
  background: #ffffff;
  border-right: 1px solid var(--line);
  box-sizing: border-box;
}

.brand {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 17px;
  font-weight: 700;
}

.brand-logo {
  width: 36px;
  height: 36px;
  border-radius: 8px;
}

.side-nav {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.side-nav a {
  display: flex;
  align-items: center;
  gap: 10px;
  min-height: 44px;
  padding: 0 12px;
  border-left: 3px solid transparent;
  border-radius: 8px;
  font-size: 15px;
  font-weight: 600;
  color: var(--ink);
  text-decoration: none;
}

.side-nav a:hover {
  background: var(--cream);
}

.side-nav a.router-link-exact-active,
.side-nav a.router-link-active:not([href="/"]) {
  background: var(--amber-tint);
  border-left-color: var(--amber);
  color: var(--amber-dark);
}

.session {
  margin-top: auto;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.who {
  font-size: 13px;
  color: var(--muted);
  word-break: break-all;
}

.logout {
  align-self: flex-start;
  min-height: 44px;
  padding: 0 12px;
  border: 1px solid var(--line);
  border-radius: 8px;
  background: none;
  font: inherit;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
}

.logout:hover {
  border-color: var(--red);
  color: var(--red);
}

.content {
  flex: 1;
  padding: 28px 36px;
  min-width: 0;
}

h1,
h2,
h3 {
  font-family: var(--font-display);
  letter-spacing: -0.01em;
}

:focus-visible {
  outline: 2px solid var(--amber);
  outline-offset: 2px;
}

@media (max-width: 767px) {
  .layout {
    flex-direction: column;
  }

  .sidebar {
    width: 100%;
    flex-direction: row;
    align-items: center;
    gap: 12px;
    padding: 10px 12px;
    border-right: none;
    border-bottom: 1px solid var(--line);
    overflow-x: auto;
  }

  .side-nav {
    flex-direction: row;
    gap: 2px;
  }

  .side-nav a {
    border-left: none;
    border-bottom: 3px solid transparent;
    border-radius: 8px 8px 0 0;
    white-space: nowrap;
  }

  .side-nav a.router-link-exact-active,
  .side-nav a.router-link-active:not([href="/"]) {
    border-left: none;
    border-bottom-color: var(--amber);
  }

  .session {
    margin-top: 0;
    margin-left: auto;
    flex-direction: row;
    align-items: center;
  }

  .who {
    display: none;
  }

  .content {
    padding: 18px 16px;
  }
}
</style>
