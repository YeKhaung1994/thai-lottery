<template>
  <div class="checkout">
    <h1>Review your order</h1>

    <EmptyState
      v-if="!items.length"
      icon="ticket"
      title="Your cart is empty"
      hint="Pick some lucky numbers first."
    >
      <BaseButton @click="$router.push('/buy')">Browse tickets</BaseButton>
    </EmptyState>

    <template v-else>
      <ul class="summary-list">
        <li v-for="ticket in items" :key="ticket.id" class="summary-row">
          <TicketNumber :value="ticket.number" size="md" />
          <span class="row-meta">Draw {{ formatDrawDate(ticket.drawDate) }}</span>
          <span class="row-price">{{ formatBaht(ticket.price) }}</span>
          <button type="button" class="row-remove" :aria-label="`Remove ticket ${ticket.number}`" @click="remove(ticket.id)">
            Remove
          </button>
        </li>
      </ul>

      <div class="totals">
        <span>Total</span>
        <span class="total-amount">{{ formatBaht(total) }}</span>
      </div>

      <p class="pay-note">
        Tickets are reserved for 15 minutes once you continue to payment.
      </p>
      <p v-if="error" class="status error" role="alert">{{ error }}</p>

      <div class="actions">
        <BaseButton size="lg" :disabled="paying" @click="pay">
          {{ paying ? 'Reserving tickets…' : `Pay ${formatBaht(total)}` }}
        </BaseButton>
        <BaseButton variant="ghost" @click="$router.push('/buy')">← Keep shopping</BaseButton>
      </div>
    </template>
  </div>
</template>

<script>
import {
  BaseButton, EmptyState, TicketNumber, formatBaht, formatDrawDate, useToasts
} from '@htawpyi/shared-ui'
import { useCart } from '@/composables/useCart'
import { createOrder } from '@/services/platformApi'

export default {
  name: 'CheckoutView',
  components: { BaseButton, EmptyState, TicketNumber },
  setup() {
    const { items, ids, total, remove, clear } = useCart()
    const { push } = useToasts()
    return { items, ids, total, remove, clearCart: clear, toast: push }
  },
  data() {
    return { paying: false, error: null }
  },
  methods: {
    formatBaht,
    formatDrawDate,
    async pay() {
      this.paying = true
      this.error = null
      try {
        const order = await createOrder([...this.ids])
        this.clearCart()
        if (order.redirectUrl.startsWith(window.location.origin) || order.redirectUrl.startsWith('/')) {
          this.$router.push(order.redirectUrl.replace(window.location.origin, ''))
        } else {
          window.location.href = order.redirectUrl
        }
      } catch (err) {
        this.error = err.message
        this.toast('Could not reserve those tickets — someone may have beaten you to one.', 'danger')
      } finally {
        this.paying = false
      }
    }
  }
}
</script>

<style scoped>
.checkout {
  max-width: 640px;
  display: flex;
  flex-direction: column;
  gap: 18px;
  text-align: left;
}

.checkout h1 {
  margin: 0;
  font-size: 28px;
}

.summary-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.summary-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px 16px;
  min-height: 56px;
  padding: 10px 18px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.row-meta {
  flex: 1;
  font-size: 14px;
  color: var(--muted);
}

.row-price {
  font-weight: 700;
}

.row-remove {
  min-height: 44px;
  padding: 0 10px;
  border: none;
  background: none;
  font: inherit;
  font-size: 14px;
  font-weight: 600;
  color: var(--muted);
  cursor: pointer;
}

.row-remove:hover {
  color: var(--danger);
}

.totals {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  padding: 14px 18px;
  background: var(--amber-tint);
  border: 1px solid #f0ddb6;
  border-radius: var(--radius);
  font-size: 17px;
  font-weight: 700;
}

.total-amount {
  font-family: var(--font-display);
  font-size: 24px;
}

.pay-note {
  margin: 0;
  font-size: 14px;
  color: var(--muted);
}

.status.error {
  margin: 0;
  color: var(--danger);
  font-weight: 600;
}

.actions {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}
</style>
