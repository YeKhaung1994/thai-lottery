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
body {
  margin: 0;
  background: #ffffff;
}

#app {
  /* Noto Sans Myanmar renders the Burmese brand name; Latin text falls back to Avenir. */
  font-family: Avenir, Helvetica, Arial, 'Noto Sans Myanmar', sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2b2b2b;
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
