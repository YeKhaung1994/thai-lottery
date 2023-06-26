import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'
import LotteryHome from './components/views/LotteryHome.vue'
import LotteryResults from './components/views/LotteryResults.vue'
import LotteryWinners from './components/views/LotteryWinners.vue'
import AboutUs from './components/views/AboutUs.vue'

const routes = [
  { path: '/', component: LotteryHome },
  { path: '/results', component: LotteryResults },
  { path: '/winners', component: LotteryWinners },
  { path: '/about', component: AboutUs }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

createApp(App).use(router).mount('#app')
