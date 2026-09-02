<template>
  <header class="app-header">
    <router-link to="/" class="brand">
      <img class="brand-logo" src="../assets/brand-ticket.svg" alt="ထောပြီ" />
    </router-link>
    <nav class="top-nav" aria-label="Main navigation">
      <router-link to="/">Home</router-link>
      <router-link to="/history">History</router-link>
      <router-link to="/how-it-works">How It Works</router-link>
    </nav>
  </header>
</template>

<script>
export default {
  name: 'AppHeader'
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
  width: 100px;
  height: 70px;
  object-fit: contain;
  border-radius: 10px;
  display: block;
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
