<template>
  <span class="status-pill" :class="toneClass">{{ value }}</span>
</template>

<script>
// Auto-maps common domain statuses to a tone; override with the tone prop.
const TONES = {
  paid: 'success',
  available: 'success',
  succeeded: 'success',
  pending: 'warn',
  reserved: 'warn',
  initiated: 'warn',
  failed: 'danger',
  expired: 'danger',
  sold: 'danger'
}

export default {
  name: 'StatusPill',
  props: {
    value: {
      type: String,
      required: true
    },
    tone: {
      type: String,
      default: null,
      validator: (v) => ['success', 'warn', 'danger', 'neutral'].includes(v)
    }
  },
  computed: {
    toneClass() {
      return this.tone || TONES[this.value.toLowerCase()] || 'neutral'
    }
  }
}
</script>

<style scoped>
.status-pill {
  display: inline-flex;
  align-items: center;
  padding: 3px 12px;
  border-radius: 999px;
  font-size: 13px;
  font-weight: 700;
}

.status-pill.success {
  background: #e6f4e6;
  color: #1d7a2e;
}

.status-pill.warn {
  background: var(--amber-tint);
  color: var(--amber-dark);
}

.status-pill.danger {
  background: var(--red-tint);
  color: #b3261e;
}

.status-pill.neutral {
  background: var(--cream);
  color: var(--muted);
}
</style>
