<template>
  <section class="my-tickets">
    <div class="tickets-header">
      <h2><AppIcon name="ticket" :size="20" class="heading-icon" /> My tickets</h2>
      <p class="hint">Saved on this device and checked against every new draw.</p>
    </div>

    <form class="add-form" @submit.prevent="addNew">
      <label class="sr-only" for="new-ticket">Add a ticket number</label>
      <input
        id="new-ticket"
        v-model="newTicket"
        type="text"
        inputmode="numeric"
        maxlength="6"
        pattern="\d{6}"
        placeholder="Add a 6-digit ticket"
        autocomplete="off"
        @input="addError = null"
      />
      <button type="submit">Add</button>
    </form>
    <p v-if="addError" class="add-error" role="status">{{ addError }}</p>

    <p v-if="!tickets.length" class="empty">No saved tickets yet — check a number above and save it, or add one here.</p>
    <ul v-else class="ticket-list">
      <li v-for="ticket in tickets" :key="ticket" class="ticket-row">
        <span class="ticket-number">{{ ticket }}</span>
        <span v-if="!draw" class="ticket-status">—</span>
        <span v-else-if="winsFor(ticket).length" class="ticket-status win">
          Won {{ winsFor(ticket).map((w) => `${w.name} (${formatBaht(w.reward)})`).join(' + ') }}
        </span>
        <span v-else class="ticket-status">No prize in the latest draw</span>
        <button type="button" class="remove" :aria-label="`Remove ticket ${ticket}`" @click="remove(ticket)">Remove</button>
      </li>
    </ul>
  </section>
</template>

<script>
import { AppIcon } from '@htawpyi/shared-ui'
import { checkTicket, formatBaht } from '@/services/lotteryApi'
import { useMyTickets } from '@/composables/useMyTickets'

export default {
  name: 'MyTickets',
  components: { AppIcon },
  props: {
    draw: {
      type: Object,
      default: null
    }
  },
  setup() {
    const { tickets, add, remove } = useMyTickets()
    return { tickets, add, remove }
  },
  data() {
    return {
      newTicket: '',
      addError: null
    }
  },
  methods: {
    formatBaht,
    winsFor(ticket) {
      return checkTicket(this.draw, ticket) || []
    },
    addNew() {
      const ticket = this.newTicket.trim()
      if (!/^\d{6}$/.test(ticket)) {
        this.addError = 'Please enter exactly 6 digits.'
        return
      }
      if (!this.add(ticket)) {
        this.addError = 'That ticket is already saved.'
        return
      }
      this.newTicket = ''
      this.addError = null
    }
  }
}
</script>

<style scoped>
.my-tickets {
  display: flex;
  flex-direction: column;
  gap: 14px;
  padding: 24px 28px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  text-align: left;
}

.my-tickets h2 {
  display: flex;
  align-items: center;
  gap: 8px;
}

.heading-icon {
  color: var(--red);
  flex-shrink: 0;
}

.tickets-header {
  display: flex;
  flex-wrap: wrap;
  align-items: baseline;
  gap: 6px 16px;
}

.my-tickets h2 {
  margin: 0;
  font-size: 22px;
}

.hint {
  margin: 0;
  font-size: 14px;
  color: #6b6b6b;
}

.add-form {
  display: flex;
  gap: 10px;
  max-width: 360px;
}

.add-form input {
  flex: 1;
  min-width: 0;
  min-height: 44px;
  padding: 0 14px;
  border: 1px solid #c9c9c9;
  border-radius: 6px;
  font-size: 16px;
  font-variant-numeric: tabular-nums;
}

.add-form input:focus {
  outline: 2px solid #d97706;
  outline-offset: 1px;
}

.add-form button {
  min-height: 44px;
  padding: 0 20px;
  border: 1px solid #2b2b2b;
  border-radius: 6px;
  background: none;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.add-form button:hover {
  border-color: #d97706;
  color: #b45309;
}

.add-error {
  margin: 0;
  font-size: 14px;
  color: #b3261e;
}

.empty {
  margin: 0;
  font-size: 15px;
  color: #6b6b6b;
}

.ticket-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.ticket-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px 16px;
  min-height: 48px;
  padding: 6px 14px;
  background: var(--cream);
  border: 1px solid var(--line);
  border-radius: 8px;
}

.ticket-row:has(.ticket-status.win) {
  background: var(--red-tint);
  border-color: #f5d4d5;
}

.ticket-number {
  font-size: 18px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
}

.ticket-status {
  flex: 1;
  font-size: 15px;
  color: #6b6b6b;
}

.ticket-status.win {
  color: #d2232a;
  font-weight: 600;
}

.remove {
  min-height: 44px;
  padding: 0 12px;
  border: none;
  background: none;
  font: inherit;
  font-size: 14px;
  color: #6b6b6b;
  cursor: pointer;
}

.remove:hover {
  color: #b3261e;
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
