<template>
  <AppHeader />
  <main class="page">
    <router-view />
  </main>
  <LotteryFooter />
  <BottomNav />
</template>

<script>
import AppHeader from './components/AppHeader.vue'
import BottomNav from './components/BottomNav.vue'
import LotteryFooter from './components/views/LotteryFooter.vue'

export default {
  name: 'App',
  components: { AppHeader, BottomNav, LotteryFooter }
}
</script>

<style>
:root {
  /* Palette derived from the htawpyi ticket logo. */
  --ink: #2b2b2b;
  --muted: #6b6455;
  --red: #d2232a;
  --red-tint: #fdeeee;
  --amber: #d97706;
  --amber-dark: #b45309;
  --amber-tint: #fdf3e0;
  --gold: #f2b01e;
  --teal: #3aa6b9;
  --teal-tint: #e9f5f8;
  --cream: #faf6ec;
  --card: #ffffff;
  --line: #eee6d6;
  --radius: 12px;
  --shadow: 0 1px 2px rgba(64, 48, 15, 0.06), 0 6px 16px rgba(64, 48, 15, 0.06);
}

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
</style>
