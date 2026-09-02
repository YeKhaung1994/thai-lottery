<template>
  <AppHeader />
  <main class="page">
    <router-view />
  </main>
  <LotteryFooter />
  <BottomNav />
  <ToastHost />
</template>

<script>
import { ToastHost } from '@htawpyi/shared-ui'
import AppHeader from './components/AppHeader.vue'
import BottomNav from './components/BottomNav.vue'
import LotteryFooter from './components/views/LotteryFooter.vue'

export default {
  name: 'App',
  components: { ToastHost, AppHeader, BottomNav, LotteryFooter }
}
</script>

<style>
body {
  margin: 0;
  background: var(--cream);
}

#app {
  /* Noto Sans Myanmar renders the Burmese brand name; Latin text falls back to Avenir. */
  font-family: Avenir, Helvetica, Arial, 'Noto Sans Myanmar', sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: var(--ink);
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.page {
  flex: 1;
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 32px 48px;
  box-sizing: border-box;
}

@media (max-width: 767px) {
  .page {
    padding: 20px 16px;
  }
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
</style>
