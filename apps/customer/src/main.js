import { createApp } from 'vue'
import '@htawpyi/shared-ui/tokens.css'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'
import LotteryHome from './components/views/LotteryHome.vue'
import LotteryResults from './components/views/LotteryResults.vue'
import LotteryWinners from './components/views/LotteryWinners.vue'
import AboutUs from './components/views/AboutUs.vue'
import BuyTickets from './components/views/BuyTickets.vue'
import LoginView from './components/views/LoginView.vue'
import MyPurchases from './components/views/MyPurchases.vue'
import MockPay from './components/views/MockPay.vue'
import CheckoutView from './components/views/CheckoutView.vue'
import { getAuth } from './services/platformApi'

const routes = [
  { path: '/', component: LotteryHome, meta: { title: 'HtiMart — Thai Lottery Results' } },
  { path: '/buy', component: BuyTickets, meta: { title: 'HtiMart — Buy Tickets' } },
  { path: '/login', component: LoginView, meta: { title: 'HtiMart — Log In' } },
  {
    path: '/checkout',
    component: CheckoutView,
    meta: { title: 'HtiMart — Checkout', requiresAuth: true }
  },
  {
    path: '/purchases',
    component: MyPurchases,
    meta: { title: 'HtiMart — My Purchases', requiresAuth: true }
  },
  {
    path: '/pay/mock/:paymentId',
    component: MockPay,
    meta: { title: 'HtiMart — Payment', requiresAuth: true }
  },
  { path: '/history', component: LotteryResults, meta: { title: 'HtiMart — Draw History' } },
  { path: '/draws/:date?', component: LotteryWinners, meta: { title: 'HtiMart — Draw Details' } },
  { path: '/how-it-works', component: AboutUs, meta: { title: 'HtiMart — How It Works' } },
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

router.beforeEach((to) => {
  if (to.meta.requiresAuth && !getAuth()) {
    return { path: '/login', query: { next: to.fullPath } }
  }
})

router.afterEach((to) => {
  document.title = to.meta.title || 'HtiMart'
})

createApp(App).use(router).mount('#app')
