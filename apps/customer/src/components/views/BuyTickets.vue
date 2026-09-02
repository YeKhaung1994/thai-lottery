<template>
  <div class="shop">
    <div class="shop-header">
      <h1>Buy Tickets</h1>
      <p class="hint">Real tickets for the upcoming draw — pick your lucky numbers.</p>
    </div>

    <fieldset class="lucky-picker">
      <legend class="picker-label">Lucky number — leave boxes blank as wildcards</legend>
      <div class="digit-boxes">
        <input
          v-for="(slot, i) in pattern"
          :key="i"
          :ref="`digit${i}`"
          :value="slot"
          :aria-label="`Digit ${i + 1}`"
          type="text"
          inputmode="numeric"
          maxlength="1"
          class="digit-box"
          @input="setDigit(i, $event)"
          @keydown.backspace="backOn(i, $event)"
        />
      </div>
      <div class="quick-chips">
        <button type="button" class="chip" :class="{ on: onlyDoubles }" @click="onlyDoubles = !onlyDoubles; onlyTriples = false">Doubles</button>
        <button type="button" class="chip" :class="{ on: onlyTriples }" @click="onlyTriples = !onlyTriples; onlyDoubles = false">Triples</button>
        <button v-if="hasFilters" type="button" class="chip clear" @click="clearFilters">Clear</button>
      </div>
    </fieldset>

    <p v-if="error" class="status error">
      {{ error }}
      <BaseButton variant="secondary" @click="load">Retry</BaseButton>
    </p>
    <div v-else-if="loading" class="ticket-grid">
      <SkeletonBlock v-for="n in 8" :key="n" height="96px" rounded />
    </div>
    <EmptyState
      v-else-if="!filtered.length"
      icon="ticket"
      :title="hasFilters ? 'No tickets match your lucky number' : 'No tickets on sale right now'"
      :hint="hasFilters ? 'Try fewer digits or clear the filters.' : 'New tickets are uploaded before each draw — check back soon.'"
    />
    <div v-else class="ticket-grid">
      <button
        v-for="ticket in filtered"
        :key="ticket.id"
        type="button"
        class="ticket-card"
        :class="{ selected: has(ticket.id) }"
        :aria-pressed="has(ticket.id) ? 'true' : 'false'"
        @click="pick(ticket)"
      >
        <TicketNumber :value="ticket.number" size="lg" />
        <span class="ticket-meta">{{ formatDrawDate(ticket.drawDate) }}</span>
        <span class="ticket-price">{{ formatBaht(ticket.price) }}</span>
      </button>
    </div>

    <div v-if="count" class="cart-bar">
      <span class="cart-info">
        {{ count }} ticket{{ count === 1 ? '' : 's' }} · {{ formatBaht(total) }}
      </span>
      <BaseButton size="lg" @click="review">
        {{ isLoggedIn ? 'Review order' : 'Log in to buy' }}
      </BaseButton>
    </div>
  </div>
</template>

<script>
import {
  BaseButton, EmptyState, SkeletonBlock, TicketNumber,
  formatBaht, formatDrawDate, hasDouble, hasTriple, matchesDigitPattern, useToasts
} from '@htawpyi/shared-ui'
import { useAuth } from '@/composables/useAuth'
import { useCart } from '@/composables/useCart'
import { searchTickets } from '@/services/platformApi'

export default {
  name: 'BuyTickets',
  components: { BaseButton, EmptyState, SkeletonBlock, TicketNumber },
  setup() {
    const { isLoggedIn } = useAuth()
    const { count, total, has, toggle } = useCart()
    const { push } = useToasts()
    return { isLoggedIn, count, total, has, toggle, toast: push }
  },
  data() {
    return {
      tickets: [],
      pattern: ['', '', '', '', '', ''],
      onlyDoubles: false,
      onlyTriples: false,
      loading: true,
      error: null
    }
  },
  computed: {
    hasFilters() {
      return this.onlyDoubles || this.onlyTriples || this.pattern.some(Boolean)
    },
    filtered() {
      return this.tickets.filter((t) =>
        matchesDigitPattern(t.number, this.pattern) &&
        (!this.onlyDoubles || hasDouble(t.number)) &&
        (!this.onlyTriples || hasTriple(t.number)))
    }
  },
  created() {
    this.load()
  },
  methods: {
    formatBaht,
    formatDrawDate,
    async load() {
      this.loading = true
      this.error = null
      try {
        this.tickets = await searchTickets('')
      } catch (err) {
        this.error = err.message
      } finally {
        this.loading = false
      }
    },
    setDigit(i, event) {
      const digit = event.target.value.replace(/\D/g, '').slice(-1)
      event.target.value = digit
      this.pattern.splice(i, 1, digit)
      if (digit && i < 5) this.$refs[`digit${i + 1}`][0].focus()
    },
    backOn(i, event) {
      if (!event.target.value && i > 0) this.$refs[`digit${i - 1}`][0].focus()
    },
    clearFilters() {
      this.pattern = ['', '', '', '', '', '']
      this.onlyDoubles = false
      this.onlyTriples = false
    },
    pick(ticket) {
      if (!this.toggle(ticket)) {
        this.toast('An order can contain at most 10 tickets.', 'danger')
      }
    },
    review() {
      if (!this.isLoggedIn) {
        this.$router.push({ path: '/login', query: { next: '/checkout' } })
        return
      }
      this.$router.push('/checkout')
    }
  }
}
</script>

<style scoped>
.shop {
  display: flex;
  flex-direction: column;
  gap: 18px;
  text-align: left;
  padding-bottom: 80px;
}

.shop-header h1 {
  margin: 0;
  font-size: 28px;
}

.hint {
  margin: 4px 0 0 0;
  font-size: 15px;
  color: var(--muted);
}

.lucky-picker {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin: 0;
  padding: 18px 22px;
  border: 1px solid var(--line);
  border-radius: var(--radius);
  background: var(--card);
  box-shadow: var(--shadow);
}

.picker-label {
  font-size: 14px;
  font-weight: 700;
  color: var(--muted);
  padding: 0 6px;
}

.digit-boxes {
  display: flex;
  gap: 10px;
}

.digit-box {
  width: 52px;
  height: 60px;
  text-align: center;
  font-family: var(--font-mono);
  font-size: 26px;
  font-weight: 700;
  border: 1px solid var(--line-strong);
  border-radius: 10px;
  background: var(--cream);
  box-sizing: border-box;
}

.digit-box:focus {
  outline: 2px solid var(--amber);
  outline-offset: 1px;
  background: #ffffff;
}

.quick-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.chip {
  min-height: 40px;
  padding: 0 16px;
  border: 1px solid var(--line-strong);
  border-radius: var(--radius-pill);
  background: #ffffff;
  font: inherit;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
}

.chip:hover {
  border-color: var(--amber);
}

.chip.on {
  background: var(--amber-tint);
  border-color: var(--amber);
  color: var(--amber-dark);
}

.chip.clear {
  color: var(--muted);
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

.ticket-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: 14px;
}

.ticket-card {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 4px;
  min-height: 96px;
  padding: 14px 16px 14px 22px;
  background: var(--card);
  border: 1px solid var(--line);
  border-left: 4px solid var(--red);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  font: inherit;
  text-align: left;
  cursor: pointer;
  transition: border-color 0.15s ease, transform 0.15s ease;
  /* Perforation edge */
  background-image: radial-gradient(circle at 0 50%, transparent 0, transparent 100%);
}

.ticket-card::before {
  content: '';
  position: absolute;
  left: 8px;
  top: 8px;
  bottom: 8px;
  border-left: 2px dashed var(--line-strong);
}

.ticket-card:hover {
  border-color: var(--amber);
  border-left-color: var(--red);
  transform: translateY(-1px);
}

.ticket-card.selected {
  border-color: var(--amber);
  background: var(--amber-tint);
}

.ticket-meta {
  font-size: 13px;
  color: var(--muted);
}

.ticket-price {
  font-size: 15px;
  font-weight: 700;
  color: var(--amber-dark);
}

.cart-bar {
  position: fixed;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 15;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: center;
  gap: 10px 20px;
  padding: 12px 16px calc(12px + env(safe-area-inset-bottom, 0px)) 16px;
  background: #ffffff;
  border-top: 1px solid var(--line);
  box-shadow: 0 -4px 16px rgba(64, 48, 15, 0.1);
}

.cart-info {
  font-family: var(--font-display);
  font-size: 17px;
  font-weight: 700;
}

@media (max-width: 767px) {
  .cart-bar {
    bottom: 57px;
  }

  .digit-box {
    width: 44px;
    height: 54px;
    font-size: 22px;
  }
}
</style>
