<template>
  <article class="draw-card">
    <button type="button" class="draw-summary" :aria-expanded="expanded ? 'true' : 'false'" @click="$emit('toggle')">
      <span class="draw-date">Draw · {{ dateLabel }}</span>
      <span v-if="draw" class="draw-first">{{ draw.firstPrize }}</span>
      <span v-else-if="loading" class="draw-note">loading…</span>
      <span v-else-if="error" class="draw-note error">failed to load</span>
      <span class="draw-chevron">{{ expanded ? '▴' : '▾' }}</span>
    </button>
    <div v-if="expanded" class="draw-detail">
      <p v-if="loading" class="draw-note">Loading draw…</p>
      <p v-else-if="error" class="draw-note error">
        {{ error }}
        <button type="button" class="retry" @click="$emit('retry')">Retry</button>
      </p>
      <template v-else-if="draw">
        <div class="running-row">
          <NumberChip :value="draw.firstPrize" highlight />
          <span class="running-label">1st Prize · {{ firstRewardLabel }}</span>
        </div>
        <div class="running-row">
          <span class="running-label">Front 3: {{ draw.front3.join(', ') }}</span>
          <span class="running-label">Back 3: {{ draw.back3.join(', ') }}</span>
          <span class="running-label">Last 2: {{ draw.last2 }}</span>
        </div>
        <div class="prize-grid">
          <PrizeCard v-for="prize in secondaryPrizes" :key="prize.id" :prize="prize" :collapse-at="5" />
        </div>
        <router-link class="open-draw" :to="`/winners/${date}`">Open draw page →</router-link>
      </template>
    </div>
  </article>
</template>

<script>
import NumberChip from './NumberChip.vue'
import PrizeCard from './PrizeCard.vue'
import { formatBaht, formatDrawDate } from '@/services/lotteryApi'

export default {
  name: 'DrawCard',
  components: { NumberChip, PrizeCard },
  props: {
    date: {
      type: String,
      required: true
    },
    draw: {
      type: Object,
      default: null
    },
    loading: {
      type: Boolean,
      default: false
    },
    error: {
      type: String,
      default: null
    },
    expanded: {
      type: Boolean,
      default: false
    }
  },
  emits: ['toggle', 'retry'],
  computed: {
    dateLabel() {
      return formatDrawDate(this.date)
    },
    firstRewardLabel() {
      return this.draw ? formatBaht(this.draw.firstReward) : ''
    },
    secondaryPrizes() {
      return this.draw ? this.draw.prizes.filter((p) => p.id !== 'first') : []
    }
  }
}
</script>

<style scoped>
.draw-card {
  border: 1px solid #d9d9d9;
  border-radius: 8px;
  background: #ffffff;
}

.draw-summary {
  display: flex;
  align-items: center;
  gap: 16px;
  width: 100%;
  min-height: 56px;
  padding: 10px 20px;
  border: none;
  background: none;
  font: inherit;
  cursor: pointer;
  text-align: left;
}

.draw-date {
  font-size: 18px;
  font-weight: 600;
}

.draw-first {
  font-size: 20px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  color: #d97706;
}

.draw-chevron {
  margin-left: auto;
  color: #6b6b6b;
}

.draw-note {
  font-size: 15px;
  color: #6b6b6b;
}

.draw-note.error {
  color: #b3261e;
}

.retry {
  min-height: 44px;
  padding: 0 12px;
  border: 1px solid #2b2b2b;
  border-radius: 6px;
  background: none;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.draw-detail {
  display: flex;
  flex-direction: column;
  gap: 14px;
  padding: 0 20px 18px 20px;
  border-top: 1px solid #eeeeee;
  padding-top: 14px;
  text-align: left;
}

.running-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 10px 20px;
}

.running-label {
  font-size: 15px;
  color: #2b2b2b;
}

.prize-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;
}

.open-draw {
  align-self: flex-start;
  display: inline-flex;
  align-items: center;
  min-height: 44px;
  font-size: 15px;
  font-weight: 600;
  color: #b45309;
  text-decoration: none;
}

.open-draw:hover {
  color: #92400e;
}

@media (max-width: 767px) {
  .prize-grid {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
