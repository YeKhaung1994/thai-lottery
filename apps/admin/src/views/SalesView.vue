<template>
  <div class="sales">
    <h1>Sales</h1>

    <p v-if="error" class="status error">
      {{ error }}
      <BaseButton variant="secondary" @click="load">Retry</BaseButton>
    </p>
    <div v-else-if="loading">
      <SkeletonBlock height="220px" rounded />
    </div>
    <EmptyState v-else-if="!orders.length" icon="trophy" title="No orders yet" hint="Sales appear here as customers buy tickets." />
    <template v-else>
      <p class="summary">
        {{ paidCount }} paid order{{ paidCount === 1 ? '' : 's' }} ·
        ฿{{ paidTotal.toLocaleString() }} revenue
      </p>
      <DataTable :columns="columns" :rows="tableRows" noun="order">
        <template #cell-tickets="{ row }">
          <span class="numbers">
            <TicketNumber v-for="n in row.tickets" :key="n" :value="n" size="sm" />
          </span>
        </template>
        <template #cell-total="{ row }">฿{{ row.total.toLocaleString() }}</template>
        <template #cell-status="{ row }">
          <StatusPill :value="row.status" />
        </template>
      </DataTable>
    </template>
  </div>
</template>

<script>
import {
  BaseButton, DataTable, EmptyState, SkeletonBlock, StatusPill, TicketNumber
} from '@htawpyi/shared-ui'
import { listOrders } from '@/services/adminApi'

export default {
  name: 'SalesView',
  components: { BaseButton, DataTable, EmptyState, SkeletonBlock, StatusPill, TicketNumber },
  data() {
    return {
      orders: [],
      loading: true,
      error: null,
      columns: [
        { key: 'when', label: 'Date', sortable: true },
        { key: 'customerEmail', label: 'Customer', sortable: true },
        { key: 'tickets', label: 'Tickets' },
        { key: 'total', label: 'Total', align: 'right', sortable: true },
        { key: 'status', label: 'Status', sortable: true }
      ]
    }
  },
  computed: {
    paidCount() {
      return this.orders.filter((o) => o.status === 'Paid').length
    },
    paidTotal() {
      return this.orders
        .filter((o) => o.status === 'Paid')
        .reduce((sum, o) => sum + o.total, 0)
    },
    tableRows() {
      return this.orders.map((o) => ({
        id: o.id,
        when: this.formatDateTime(o.createdAt),
        customerEmail: o.customerEmail,
        tickets: o.ticketNumbers,
        total: o.total,
        status: o.status
      }))
    }
  },
  created() {
    this.load()
  },
  methods: {
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
        this.orders = await listOrders()
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
.sales {
  display: flex;
  flex-direction: column;
  gap: 16px;
  text-align: left;
}

h1 {
  margin: 0;
  font-size: 26px;
}

.summary {
  margin: 0;
  font-family: var(--font-display);
  font-size: 16px;
  font-weight: 700;
  color: var(--amber-dark);
}

.status {
  margin: 0;
  display: flex;
  align-items: center;
  gap: 12px;
  color: var(--muted);
}

.status.error {
  color: var(--danger);
}

.numbers {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}
</style>
