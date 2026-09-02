<template>
  <div class="purchases">
    <h1>My Purchases</h1>

    <p v-if="error" class="status error">
      {{ error }}
      <button type="button" class="retry" @click="load">Retry</button>
    </p>
    <div v-else-if="loading" class="order-list">
      <SkeletonBlock v-for="n in 3" :key="n" height="110px" rounded />
    </div>
    <EmptyState
      v-else-if="!orders.length"
      icon="ticket"
      title="No purchases yet"
      hint="Your bought tickets and their results will appear here."
    >
      <router-link to="/buy" class="empty-link">Buy your first ticket →</router-link>
    </EmptyState>
    <div v-else class="order-list">
      <article v-for="order in orders" :key="order.id" class="order-card" :class="{ winner: totalWon(order) > 0 }">
        <div v-if="totalWon(order) > 0" class="win-banner">
          <AppIcon name="trophy" :size="18" />
          Winning order — {{ formatBaht(totalWon(order)) }} in prizes
        </div>
        <header class="order-header">
          <span class="order-date">{{ formatDateTime(order.createdAt) }}</span>
          <StatusPill :value="order.status" />
          <span class="order-total">{{ formatBaht(order.total) }}</span>
        </header>
        <ul class="item-list">
          <li v-for="item in order.items" :key="item.number + item.drawDate" class="item-row">
            <TicketNumber :value="item.number" size="md" />
            <span class="item-draw">Draw {{ formatDrawDate(item.drawDate) }}</span>
            <span v-if="item.wins.length" class="item-win">
              Won {{ item.wins.map((w) => `${w.name} (${formatBaht(w.reward)})`).join(' + ') }}
            </span>
            <span v-else-if="order.status === 'Paid' && isPast(item.drawDate)" class="item-miss">
              No prize
            </span>
            <span v-else-if="order.status === 'Paid'" class="item-pending">
              Draw pending
            </span>
          </li>
        </ul>
        <button type="button" class="receipt-toggle" @click="toggleReceipt(order.id)">
          {{ openReceipts.includes(order.id) ? 'Hide details ▴' : 'Details ▾' }}
        </button>
        <dl v-if="openReceipts.includes(order.id)" class="receipt">
          <div><dt>Order</dt><dd>{{ order.id }}</dd></div>
          <div><dt>Placed</dt><dd>{{ formatDateTime(order.createdAt) }}</dd></div>
          <div><dt>Status</dt><dd>{{ order.status }}</dd></div>
          <div><dt>Total</dt><dd>{{ formatBaht(order.total) }}</dd></div>
        </dl>
      </article>
    </div>
  </div>
</template>

<script>
import { AppIcon, EmptyState, SkeletonBlock, StatusPill, TicketNumber, formatBaht, formatDrawDate } from '@htawpyi/shared-ui'
import { myOrders } from '@/services/platformApi'

export default {
  name: 'MyPurchases',
  components: { AppIcon, EmptyState, SkeletonBlock, StatusPill, TicketNumber },
  data() {
    return {
      orders: [],
      loading: true,
      error: null,
      openReceipts: []
    }
  },
  created() {
    this.load()
  },
  methods: {
    formatBaht,
    formatDrawDate,
    totalWon(order) {
      return order.items.flatMap((i) => i.wins).reduce((sum, w) => sum + w.reward, 0)
    },
    toggleReceipt(id) {
      this.openReceipts = this.openReceipts.includes(id)
        ? this.openReceipts.filter((r) => r !== id)
        : [...this.openReceipts, id]
    },
    isPast(isoDate) {
      return isoDate <= new Date().toISOString().slice(0, 10)
    },
    formatDateTime(value) {
      const d = new Date(value)
      return Number.isNaN(d.getTime())
        ? value
        : d.toLocaleString('en-GB', {
            day: 'numeric', month: 'short', year: 'numeric',
            hour: '2-digit', minute: '2-digit'
          })
    },
    async load() {
      this.loading = true
      this.error = null
      try {
        this.orders = await myOrders()
      } catch (err) {
        this.error = err.message
      } finally {
        this.loading = false
      }
    }
  }
}
</script>

<style scoped>
.purchases {
  display: flex;
  flex-direction: column;
  gap: 18px;
  text-align: left;
}

.purchases h1 {
  margin: 0;
  font-size: 28px;
}

.status {
  margin: 0;
  color: var(--muted);
}

.status.error {
  color: #b3261e;
}

.status a {
  color: #b45309;
}

.retry {
  min-height: 44px;
  padding: 0 12px;
  margin-left: 8px;
  border: 1px solid #2b2b2b;
  border-radius: 8px;
  background: none;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.order-list {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.order-card {
  overflow: hidden;
  padding: 16px 20px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.order-header {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px 16px;
  padding-bottom: 10px;
  border-bottom: 1px solid var(--line);
}

.order-date {
  font-size: 15px;
  color: var(--muted);
}

.order-total {
  margin-left: auto;
  font-size: 16px;
  font-weight: 700;
}

.item-list {
  list-style: none;
  margin: 0;
  padding: 10px 0 0 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.item-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px 16px;
}

.item-number {
  font-size: 18px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  letter-spacing: 2px;
}

.item-draw {
  font-size: 14px;
  color: var(--muted);
}

.item-win {
  font-size: 15px;
  font-weight: 700;
  color: var(--red);
}

.item-miss,
.item-pending {
  font-size: 14px;
  color: var(--muted);
}

.order-card.winner {
  border-color: var(--gold);
}

.win-banner {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: -16px -20px 12px -20px;
  padding: 10px 20px;
  background: linear-gradient(100deg, var(--gold-tint) 0%, #fff8e3 60%, var(--gold-tint) 100%);
  color: var(--amber-dark);
  font-family: var(--font-display);
  font-size: 16px;
  font-weight: 700;
  animation: win-shine 0.6s ease-out;
}

@media (prefers-reduced-motion: reduce) {
  .win-banner {
    animation: none;
  }
}

@keyframes win-shine {
  from {
    opacity: 0;
    transform: translateY(-6px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.empty-link {
  color: var(--amber-dark);
  font-weight: 700;
  text-decoration: none;
  min-height: 44px;
  display: inline-flex;
  align-items: center;
}

.receipt-toggle {
  min-height: 44px;
  padding: 0;
  margin-top: 4px;
  border: none;
  background: none;
  font: inherit;
  font-size: 14px;
  font-weight: 600;
  color: var(--amber-dark);
  cursor: pointer;
}

.receipt {
  margin: 0;
  padding: 10px 14px;
  background: var(--cream);
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 13px;
}

.receipt div {
  display: flex;
  gap: 10px;
}

.receipt dt {
  width: 64px;
  color: var(--muted);
  font-weight: 600;
}

.receipt dd {
  margin: 0;
  font-variant-numeric: tabular-nums;
  word-break: break-all;
}
</style>
