<template>
  <section class="ticket-checker">
    <h2>Check your ticket</h2>
    <p class="hint">
      Enter your 6-digit number and see instantly if it won any prize in
      {{ draw ? `the draw of ${dateLabel}` : 'the latest draw' }}.
    </p>
    <form class="check-form" @submit.prevent="check">
      <label class="sr-only" for="ticket-number">Ticket number</label>
      <input
        id="ticket-number"
        v-model="ticket"
        type="text"
        inputmode="numeric"
        maxlength="6"
        pattern="\d{6}"
        placeholder="e.g. 417212"
        autocomplete="off"
        @input="result = null"
      />
      <button type="submit" :disabled="!draw">Check</button>
    </form>
    <p v-if="invalid" class="result miss" role="status">Please enter exactly 6 digits.</p>
    <template v-else-if="result !== null">
      <p v-if="result.length" class="result win" role="status">
        You won{{ result.length > 1 ? ` ${result.length} prizes` : '' }}:
        <span v-for="(win, index) in result" :key="win.name">{{ index ? ' + ' : '' }}{{ win.name }} ({{ formatBaht(win.reward) }})</span>
      </p>
      <p v-else class="result miss" role="status">No prize in this draw ({{ dateLabel }}). Better luck next time!</p>
      <button v-if="!alreadySaved" type="button" class="save" @click="saveTicket">
        Save to My Tickets
      </button>
      <p v-else class="saved-note">Saved to My Tickets — it will be checked against every new draw.</p>
    </template>
  </section>
</template>

<script>
import { checkTicket, formatBaht, formatDrawDate } from '@/services/lotteryApi'
import { useMyTickets } from '@/composables/useMyTickets'

export default {
  name: 'TicketChecker',
  props: {
    draw: {
      type: Object,
      default: null
    }
  },
  setup() {
    const { tickets, add } = useMyTickets()
    return { savedTickets: tickets, addTicket: add }
  },
  data() {
    return {
      ticket: '',
      result: null,
      invalid: false
    }
  },
  computed: {
    dateLabel() {
      return this.draw ? formatDrawDate(this.draw.date) : ''
    },
    alreadySaved() {
      return this.savedTickets.includes(this.ticket.trim())
    }
  },
  methods: {
    formatBaht,
    check() {
      if (!this.draw) return
      const wins = checkTicket(this.draw, this.ticket.trim())
      this.invalid = wins === null
      this.result = wins
    },
    saveTicket() {
      this.addTicket(this.ticket.trim())
    }
  }
}
</script>

<style scoped>
.ticket-checker {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 24px 28px;
  border: 2px dashed #c9c9c9;
  border-radius: 8px;
  background: #ffffff;
  text-align: left;
}

.ticket-checker h2 {
  margin: 0;
  font-size: 22px;
}

.hint {
  margin: 0;
  font-size: 15px;
  color: #6b6b6b;
}

.check-form {
  display: flex;
  gap: 10px;
}

.check-form input {
  flex: 1;
  min-width: 0;
  min-height: 48px;
  padding: 0 16px;
  border: 2px solid #2b2b2b;
  border-radius: 6px;
  font-size: 20px;
  font-variant-numeric: tabular-nums;
  letter-spacing: 4px;
}

.check-form input:focus {
  outline: 2px solid #d97706;
  outline-offset: 1px;
}

.check-form button {
  min-height: 48px;
  padding: 0 28px;
  border: none;
  border-radius: 6px;
  background: #d97706;
  color: #ffffff;
  font: inherit;
  font-size: 18px;
  font-weight: 700;
  cursor: pointer;
}

.check-form button:hover {
  background: #b45309;
}

.check-form button:disabled {
  background: #c9c9c9;
  cursor: not-allowed;
}

.result {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

.result.win {
  color: #d2232a;
  font-size: 17px;
  animation: win-pop 0.35s ease;
}

@keyframes win-pop {
  0% {
    transform: scale(0.9);
    opacity: 0;
  }
  60% {
    transform: scale(1.04);
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

.result.miss {
  color: #6b6b6b;
}

.save {
  align-self: flex-start;
  min-height: 44px;
  padding: 0 16px;
  border: 1px solid #2b2b2b;
  border-radius: 6px;
  background: none;
  font: inherit;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
}

.save:hover {
  border-color: #d97706;
  color: #b45309;
}

.saved-note {
  margin: 0;
  font-size: 14px;
  color: #6b6b6b;
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
