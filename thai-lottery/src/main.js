import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'
import LotteryHome from './components/views/LotteryHome.vue'
import LotteryResults from './components/views/LotteryResults.vue'
import LotteryWinners from './components/views/LotteryWinners.vue'
import AboutUs from './components/views/AboutUs.vue'

const routes = [
  { path: '/', component: LotteryHome, meta: { title: 'ထောပြီ — Thai Lottery Results' } },
  { path: '/results', component: LotteryResults, meta: { title: 'ထောပြီ — Draw History' } },
  { path: '/winners/:date?', component: LotteryWinners, meta: { title: 'ထောပြီ — Winning Numbers' } },
  { path: '/about', component: AboutUs, meta: { title: 'ထောပြီ — About' } }
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
