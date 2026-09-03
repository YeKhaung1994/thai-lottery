import { createApp } from 'vue'
import '@htawpyi/shared-ui/tokens.css'
import { useToasts } from '@htawpyi/shared-ui'
import { createRouter, createWebHistory } from 'vue-router'
import App, { authState } from './App.vue'
import { getAuth, onSessionExpired } from './services/adminApi'
import AdminLogin from './views/AdminLogin.vue'
import DashboardView from './views/DashboardView.vue'
import UploadTickets from './views/UploadTickets.vue'
import InventoryView from './views/InventoryView.vue'
import SalesView from './views/SalesView.vue'

const routes = [
  { path: '/', component: DashboardView, meta: { title: 'HtiMart Admin — Dashboard' } },
  { path: '/login', component: AdminLogin, meta: { title: 'HtiMart Admin — Log In' } },
  { path: '/upload', component: UploadTickets, meta: { title: 'HtiMart Admin — Upload' } },
  { path: '/inventory', component: InventoryView, meta: { title: 'HtiMart Admin — Inventory' } },
  { path: '/sales', component: SalesView, meta: { title: 'HtiMart Admin — Sales' } }
]

const router = createRouter({ history: createWebHistory(), routes })

router.beforeEach((to) => {
  if (to.path !== '/login' && !getAuth()) return '/login'
  if (to.path === '/login' && getAuth()) return '/'
})

router.afterEach((to) => {
  document.title = to.meta.title || 'HtiMart Admin'
})

// Session expired: clear the shared auth ref, alert, and go to login.
onSessionExpired(() => {
  authState.value = null
  useToasts().push('Your session has expired — please log in again.', 'danger')
  if (router.currentRoute.value.path !== '/login') router.push('/login')
})

createApp(App).use(router).mount('#app')
