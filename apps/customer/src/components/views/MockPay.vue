<template>
  <div class="mock-pay">
    <h1>Confirm payment</h1>
    <p class="hint">
      Development payment (mock provider) — no real money moves. Confirm to
      complete your order or cancel to release the tickets.
    </p>
    <p class="reserve-note">
      Tickets reserved for <CountdownTimer :target="reservedUntil" format="clock" /> — complete payment before it runs out.
    </p>
    <p v-if="error" class="status error">{{ error }}</p>
    <div class="actions">
      <button type="button" class="confirm" :disabled="busy" @click="finish(true)">
        Confirm payment
      </button>
      <button type="button" class="cancel" :disabled="busy" @click="finish(false)">
        Cancel
      </button>
    </div>
  </div>
</template>

<script>
import { CountdownTimer } from '@htawpyi/shared-ui'
import { mockConfirm } from '@/services/platformApi'

export default {
  name: 'MockPay',
  components: { CountdownTimer },
  data() {
    return {
      busy: false,
      error: null,
      reservedUntil: new Date(Date.now() + 15 * 60 * 1000)
    }
  },
  methods: {
    async finish(success) {
      this.busy = true
      this.error = null
      try {
        await mockConfirm(this.$route.params.paymentId, success)
        this.$router.push('/purchases')
      } catch (err) {
        this.error = err.message
        this.busy = false
      }
    }
  }
}
</script>

<style scoped>
.mock-pay {
  max-width: 480px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 24px 28px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  text-align: left;
}

.mock-pay h1 {
  margin: 0;
  font-size: 26px;
}

.hint {
  margin: 0;
  font-size: 15px;
  color: var(--muted);
  line-height: 1.5;
}

.reserve-note {
  margin: 0;
  padding: 10px 14px;
  background: var(--amber-tint);
  border: 1px solid #f0ddb6;
  border-radius: 8px;
  font-size: 14px;
  color: var(--amber-dark);
}

.status.error {
  margin: 0;
  color: #b3261e;
}

.actions {
  display: flex;
  gap: 12px;
}

.confirm {
  flex: 1;
  min-height: 48px;
  border: none;
  border-radius: 8px;
  background: #d97706;
  color: #ffffff;
  font: inherit;
  font-size: 17px;
  font-weight: 700;
  cursor: pointer;
}

.confirm:hover {
  background: #b45309;
}

.cancel {
  min-height: 48px;
  padding: 0 20px;
  border: 1px solid #2b2b2b;
  border-radius: 8px;
  background: #ffffff;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.confirm:disabled,
.cancel:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
