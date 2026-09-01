import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'
import LotteryHome from './components/views/LotteryHome.vue'
import LotteryResults from './components/views/LotteryResults.vue'
import LotteryWinners from './components/views/LotteryWinners.vue'
import AboutUs from './components/views/AboutUs.vue'

const routes = [
  { path: '/', component: LotteryHome, meta: { title: 'ထောပြီ — Thai Lottery Results' } },
  { path: '/history', component: LotteryResults, meta: { title: 'ထောပြီ — Draw History' } },
  { path: '/draws/:date?', component: LotteryWinners, meta: { title: 'ထောပြီ — Draw Details' } },
  { path: '/how-it-works', component: AboutUs, meta: { title: 'ထောပြီ — How It Works' } },
  // Old paths keep working after the module renames.
  { path: '/results', redirect: '/history' },
  { path: '/about', redirect: '/how-it-works' },
  { path: '/winners/:date?', redirect: (to) => (to.params.date ? `/draws/${to.params.date}` : '/draws') }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 }
  }
})

router.afterEach((to) => {
  document.title = to.meta.title || 'ထောပြီ'
})

createApp(App).use(router).mount('#app')
