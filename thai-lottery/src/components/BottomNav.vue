<template>
  <nav class="bottom-nav" aria-label="Mobile navigation">
    <router-link to="/">
      <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
        <path d="M3 11 L12 3 L21 11"></path>
        <path d="M5 10 V21 H19 V10"></path>
      </svg>
      <span>Home</span>
    </router-link>
    <router-link to="/history">
      <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
        <rect x="4" y="3" width="16" height="18" rx="2"></rect>
        <line x1="8" y1="8" x2="16" y2="8"></line>
        <line x1="8" y1="12" x2="16" y2="12"></line>
        <line x1="8" y1="16" x2="13" y2="16"></line>
      </svg>
      <span>History</span>
    </router-link>
    <router-link to="/how-it-works">
      <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
        <circle cx="12" cy="12" r="9"></circle>
        <line x1="12" y1="11" x2="12" y2="17"></line>
        <line x1="12" y1="7" x2="12" y2="7.01"></line>
      </svg>
      <span>How It Works</span>
    </router-link>
  </nav>
</template>

<script>
export default {
  name: 'BottomNav'
}
</script>

<style scoped>
.bottom-nav {
  display: none;
}

@media (max-width: 767px) {
  .bottom-nav {
    position: fixed;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: 20;
    display: flex;
    background: #ffffff;
    border-top: 1px solid var(--line);
    box-shadow: 0 -4px 16px rgba(64, 48, 15, 0.08);
    padding: 4px 4px calc(4px + env(safe-area-inset-bottom, 0px)) 4px;
  }

  .bottom-nav a {
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 2px;
    min-height: 48px;
    font-size: 12px;
    font-weight: 600;
    color: #2b2b2b;
    text-decoration: none;
  }

  .bottom-nav a.router-link-exact-active,
  .bottom-nav a.router-link-active:not([href="/"]) {
    color: #d97706;
  }
}
</style>
