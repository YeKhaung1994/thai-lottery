<template>
  <div v-if="draw" class="draw-strip" :class="{ 'draw-day': awaitingResults }">
    <AppIcon :name="awaitingResults ? 'sparkle' : 'clock'" :size="16" />
    <template v-if="awaitingResults">
      <span>Draw day! Results usually appear from 14:30 (Thailand time) — refreshing automatically.</span>
    </template>
    <template v-else-if="nextDraw">
      <span>Next draw {{ nextDrawLabel }} · in <CountdownTimer :target="nextDraw" /></span>
      <router-link to="/buy" class="strip-link">Buy tickets →</router-link>
    </template>
  </div>
</template>

<script>
import { AppIcon, CountdownTimer } from '@htawpyi/shared-ui'
import { nextDrawDateFrom } from '@/services/lotteryApi'

export default {
  name: 'DrawStrip',
  components: { AppIcon, CountdownTimer },
  props: {
    draw: {
      type: Object,
      default: null
    },
    awaitingResults: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    nextDraw() {
      return this.draw ? nextDrawDateFrom(this.draw.date) : null
    },
    nextDrawLabel() {
      return this.nextDraw
        ? this.nextDraw.toLocaleDateString('en-GB', { day: 'numeric', month: 'short' })
        : ''
    }
  }
}
</script>

<style scoped>
.draw-strip {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: center;
  gap: 8px 14px;
  padding: 10px 16px;
  background: var(--teal-tint);
  border: 1px solid #cde7ed;
  border-radius: var(--radius);
  color: #23808f;
  font-size: 15px;
  font-weight: 600;
}

.draw-strip.draw-day {
  background: var(--gold-tint);
  border-color: #f0ddb6;
  color: var(--amber-dark);
}

.strip-link {
  color: var(--amber-dark);
  font-weight: 700;
  text-decoration: none;
  min-height: 24px;
}

.strip-link:hover {
  color: var(--amber);
}
</style>
