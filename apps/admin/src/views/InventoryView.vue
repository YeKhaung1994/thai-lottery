<template>
  <div class="inventory">
    <div class="head">
      <h1>Inventory</h1>
      <label class="filter">
        <span class="sr-only">Filter by status</span>
        <select v-model="status" @change="load">
          <option value="">All statuses</option>
          <option>Available</option>
          <option>Reserved</option>
          <option>Sold</option>
        </select>
      </label>
    </div>

    <p v-if="error" class="status error">
      {{ error }}
      <BaseButton variant="secondary" @click="load">Retry</BaseButton>
    </p>
    <div v-else-if="loading" class="skeletons">
      <SkeletonBlock height="220px" rounded />
    </div>
    <EmptyState
      v-else-if="!tickets.length"
      icon="hash"
      :title="status ? `No ${status.toLowerCase()} tickets` : 'No tickets uploaded yet'"
      hint="Upload tickets for the next draw from the Upload page."
    />
    <DataTable v-else :columns="columns" :rows="tickets" noun="ticket">
      <template #cell-number="{ row }">
        <TicketNumber :value="row.number" size="sm" />
      </template>
      <template #cell-price="{ row }">฿{{ row.price.toLocaleString() }}</template>
      <template #cell-status="{ row }">
        <StatusPill :value="row.status" />
      </template>
      <template #cell-actions="{ row }">
        <BaseButton v-if="row.status === 'Available'" variant="danger" @click="remove(row)">Delete</BaseButton>
      </template>
    </DataTable>
  </div>
</template>

<script>
import {
  BaseButton, DataTable, EmptyState, SkeletonBlock, StatusPill, TicketNumber, useToasts
} from '@htawpyi/shared-ui'
import { deleteTicket, listTickets } from '@/services/adminApi'

export default {
  name: 'InventoryView',
  components: { BaseButton, DataTable, EmptyState, SkeletonBlock, StatusPill, TicketNumber },
  setup() {
    const { push } = useToasts()
    return { toast: push }
  },
  data() {
    return {
      tickets: [],
      status: '',
      loading: true,
      error: null,
      columns: [
        { key: 'drawDate', label: 'Draw', sortable: true },
        { key: 'number', label: 'Number', sortable: true },
        { key: 'price', label: 'Price', align: 'right', sortable: true },
        { key: 'status', label: 'Status', sortable: true },
        { key: 'actions', label: '' }
      ]
    }
  },
  created() {
    this.load()
  },
  methods: {
    async load() {
      this.loading = true
      this.error = null
      try {
        this.tickets = await listTickets(null, this.status)
      } catch (err) {
        this.error = err.message
      } finally {
        this.loading = false
      }
    },
    async remove(ticket) {
      if (!window.confirm(`Delete ticket ${ticket.number} (${ticket.drawDate})?`)) return
      try {
        await deleteTicket(ticket.id)
        this.tickets = this.tickets.filter((t) => t.id !== ticket.id)
        this.toast(`Ticket ${ticket.number} deleted.`, 'success')
      } catch (err) {
        this.toast(err.message, 'danger')
      }
    }
  }
}
</script>

<style scoped>
.inventory {
  display: flex;
  flex-direction: column;
  gap: 16px;
  text-align: left;
}

.head {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

h1 {
  margin: 0;
  font-size: 26px;
}

select {
  min-height: 44px;
  padding: 0 12px;
  border: 1px solid var(--line);
  border-radius: 8px;
  font: inherit;
  font-size: 15px;
  background: #ffffff;
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

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  overflow: hidden;
  clip: rect(0 0 0 0);
  white-space: nowrap;
}
</style>
