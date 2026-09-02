<template>
  <div class="dashboard">
    <h1>Dashboard</h1>

    <p v-if="error" class="status error">{{ error }} <button type="button" class="retry" @click="load">Retry</button></p>
    <div v-else-if="loading" class="tiles">
      <SkeletonBlock v-for="n in 4" :key="n" height="104px" rounded />
    </div>
    <template v-else>
      <div class="tiles">
        <StatTile label="Available" :value="counts.Available" tone="success" />
        <StatTile label="Reserved" :value="counts.Reserved" tone="gold" />
        <StatTile label="Sold" :value="counts.Sold" tone="red" />
        <StatTile label="Revenue (paid)" :value="formatBaht(revenue)" tone="teal" :sub="`${paidOrders.length} paid orders`" />
      </div>

      <section class="latest">
        <div class="latest-head">
          <h2>Latest orders</h2>
          <router-link to="/sales">All sales →</router-link>
        </div>
        <EmptyState v-if="!latestOrders.length" icon="clock" title="No orders yet" />
        <ul v-else class="latest-list">
          <li v-for="o in latestOrders" :key="o.id" class="latest-row">
            <span class="row-when">{{ formatDateTime(o.createdAt) }}</span>
            <span class="row-who">{{ o.customerEmail }}</span>
            <span class="row-tickets"><TicketNumber v-for="n in o.ticketNumbers" :key="n" :value="n" size="sm" /></span>
            <span class="row-total">{{ formatBaht(o.total) }}</span>
            <StatusPill :value="o.status" />
          </li>
        </ul>
      </section>
    </template>
  </div>
</template>

<script>
import {
  EmptyState, SkeletonBlock, StatTile, StatusPill, TicketNumber, formatBaht
} from '@htawpyi/shared-ui'
import { listOrders, listTickets } from '@/services/adminApi'

export default {
  name: 'DashboardView',
  components: { EmptyState, SkeletonBlock, StatTile, StatusPill, TicketNumber },
  data() {
    return {
      tickets: [],
      orders: [],
      loading: true,
      error: null
    }
  },
  computed: {
    counts() {
      const counts = { Available: 0, Reserved: 0, Sold: 0 }
      for (const t of this.tickets) {
        if (counts[t.status] !== undefined) counts[t.status] += 1
      }
      return counts
    },
    paidOrders() {
      return this.orders.filter((o) => o.status === 'Paid')
    },
    revenue() {
      return this.paidOrders.reduce((sum, o) => sum + o.total, 0)
    },
    latestOrders() {
      return this.orders.slice(0, 5)
    }
  },
  created() {
    this.load()
  },
  methods: {
    formatBaht,
    formatDateTime(value) {
      const d = new Date(value)
      return Number.isNaN(d.getTime())
        ? value
        : d.toLocaleString('en-GB', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })
    },
    async load() {
      this.loading = true
      this.error = null
      try {
        const [tickets, orders] = await Promise.all([listTickets(), listOrders()])
        this.tickets = tickets
        this.orders = orders
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
.dashboard {
  display: flex;
  flex-direction: column;
  gap: 20px;
  text-align: left;
}

h1 {
  margin: 0;
  font-size: 26px;
}

.tiles {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 16px;
}

.status {
  margin: 0;
  color: var(--muted);
}

.status.error {
  color: var(--danger);
}

.retry {
  min-height: 44px;
  padding: 0 12px;
  border: 1px solid var(--ink);
  border-radius: 8px;
  background: none;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.latest-head {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  gap: 12px;
}

.latest-head h2 {
  margin: 0 0 10px 0;
  font-size: 19px;
}

.latest-head a {
  color: var(--amber-dark);
  font-weight: 600;
  text-decoration: none;
}

.latest-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.latest-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px 16px;
  min-height: 48px;
  padding: 8px 16px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  font-size: 14px;
}

.row-when {
  color: var(--muted);
}

.row-who {
  font-weight: 600;
}

.row-tickets {
  flex: 1;
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.row-total {
  font-weight: 700;
}

@media (max-width: 1023px) {
  .tiles {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}
</style>
