<template>
  <span class="ticket-number" :class="size">{{ value }}</span>
</template>

<script>
export default {
  name: 'TicketNumber',
  props: {
    value: {
      type: String,
      required: true
    },
    size: {
      type: String,
      default: 'md',
      validator: (v) => ['sm', 'md', 'lg', 'xl'].includes(v)
    }
  }
}
</script>

<style scoped>
.ticket-number {
  font-family: var(--font-mono);
  font-variant-numeric: tabular-nums;
  letter-spacing: 0.08em;
  font-weight: 700;
}

.ticket-number.sm {
  font-size: 15px;
  font-weight: 500;
}

.ticket-number.md {
  font-size: 18px;
}

.ticket-number.lg {
  font-size: 24px;
}

.ticket-number.xl {
  font-size: 40px;
}
</style>
