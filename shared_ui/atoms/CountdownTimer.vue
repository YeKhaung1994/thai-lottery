<template>
  <span class="countdown" aria-live="polite">{{ display }}</span>
</template>

<script>
export default {
  name: 'CountdownTimer',
  props: {
    /** ISO date-time or Date the countdown targets. */
    target: {
      type: [String, Date],
      required: true
    },
    /** 'long' → "13d 21h" / "21h 4m"; 'clock' → "14:59" (mm:ss). */
    format: {
      type: String,
      default: 'long',
      validator: (v) => ['long', 'clock'].includes(v)
    }
  },
  emits: ['done'],
  data() {
    return { now: Date.now(), timer: null }
  },
  computed: {
    remainingMs() {
      const target = this.target instanceof Date ? this.target : new Date(this.target)
      return Math.max(0, target.getTime() - this.now)
    },
    display() {
      const total = Math.floor(this.remainingMs / 1000)
      if (this.format === 'clock') {
        const m = Math.floor(total / 60)
        const s = total % 60
        return `${m}:${String(s).padStart(2, '0')}`
      }
      const days = Math.floor(total / 86400)
      const hours = Math.floor((total % 86400) / 3600)
      const minutes = Math.floor((total % 3600) / 60)
      if (days > 0) return `${days}d ${hours}h`
      if (hours > 0) return `${hours}h ${minutes}m`
      return `${minutes}m`
    }
  },
  mounted() {
    this.timer = setInterval(() => {
      this.now = Date.now()
      if (this.remainingMs === 0) {
        clearInterval(this.timer)
        this.$emit('done')
      }
    }, 1000)
  },
  beforeUnmount() {
    clearInterval(this.timer)
  }
}
</script>

<style scoped>
.countdown {
  font-variant-numeric: tabular-nums;
  font-weight: 700;
}
</style>
