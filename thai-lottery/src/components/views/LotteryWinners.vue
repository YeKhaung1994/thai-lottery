<template>
  <div class="winners">
    <div class="winners-header">
      <h1>Winning Numbers</h1>
      <div class="draw-picker">
        <button type="button" aria-label="Newer draw" :disabled="selectedIndex <= 0" @click="step(-1)">‹</button>
        <label>
          <span class="sr-only">Select draw</span>
          <select v-model="selectedDate">
            <option v-for="date in dates" :key="date" :value="date">{{ formatDrawDate(date) }}</option>
          </select>
        </label>
        <button type="button" aria-label="Older draw" :disabled="selectedIndex >= dates.length - 1" @click="step(1)">›</button>
      </div>
    </div>

    <p v-if="error" class="status error">
      {{ error }}
      <button type="button" class="retry" @click="init">Retry</button>
    </p>
    <p v-else-if="!draw" class="status">Loading winning numbers…</p>
    <template v-else>
      <section class="first-prize">
        <div>
          <p class="eyebrow">First Prize · {{ formatBaht(draw.firstReward) }}</p>
          <p class="first-number">{{ draw.firstPrize }}</p>
        </div>
        <div v-if="draw.adjacent.length" class="adjacent">
          <p class="eyebrow">Adjacent numbers · {{ formatBaht(draw.adjacentReward) }}</p>
          <div class="chip-row">
            <NumberChip v-for="number in draw.adjacent" :key="number" :value="number" />
          </div>
        </div>
      </section>

      <TicketChecker :draw="draw" />

      <div class="prize-grid">
        <PrizeCard v-for="prize in gridPrizes" :key="prize.id" :prize="prize" />
        <PrizeCard :prize="front3Prize" />
        <PrizeCard :prize="back3Prize" />
        <PrizeCard :prize="last2Prize" />
      </div>
    </template>
  </div>
</template>

<script>
import NumberChip from '@/components/NumberChip.vue'
import PrizeCard from '@/components/PrizeCard.vue'
import TicketChecker from '@/components/TicketChecker.vue'
import { formatBaht, formatDrawDate, getDrawByDate, getDrawDates, getLatestDraw } from '@/services/lotteryApi'

export default {
  name: 'LotteryWinners',
  components: { NumberChip, PrizeCard, TicketChecker },
  data() {
    return {
      dates: [],
      selectedDate: null,
      draw: null,
      error: null,
      ready: false
    }
  },
  computed: {
    selectedIndex() {
      return this.dates.indexOf(this.selectedDate)
    },
    gridPrizes() {
      return this.draw.prizes.filter((p) => p.id !== 'first' && p.id !== 'near1')
    },
    front3Prize() {
      return { id: 'front3', name: '3-Digit Front', reward: this.draw.front3Reward, numbers: this.draw.front3 }
    },
    back3Prize() {
      return { id: 'back3', name: '3-Digit Back', reward: this.draw.back3Reward, numbers: this.draw.back3 }
    },
    last2Prize() {
      return { id: 'last2', name: '2-Digit', reward: this.draw.last2Reward, numbers: this.draw.last2 ? [this.draw.last2] : [] }
    }
  },
  watch: {
    selectedDate(date) {
      if (!date || !this.ready) return
      this.syncRoute()
      this.loadDraw()
    }
  },
  created() {
    this.init()
  },
  methods: {
    formatBaht,
    formatDrawDate,
    step(offset) {
      const next = this.dates[this.selectedIndex + offset]
      if (next) this.selectedDate = next
    },
    syncRoute() {
      const target = `/winners/${this.selectedDate}`
      if (this.$route.path !== target) this.$router.replace(target)
    },
    async init() {
      this.error = null
      this.ready = false
      try {
        this.dates = await getDrawDates()
        const fromRoute = this.$route.params.date
        this.selectedDate = this.dates.includes(fromRoute) ? fromRoute : this.dates[0]
        this.syncRoute()
        this.ready = true
        await this.loadDraw()
      } catch (err) {
        this.error = err.message || 'Could not load draw dates'
      }
    },
    async loadDraw() {
      this.error = null
      this.draw = null
      try {
        // The newest date uses the latest endpoint, which is available the
        // moment results are announced.
        this.draw =
          this.selectedDate === this.dates[0]
            ? await getLatestDraw()
            : await getDrawByDate(this.selectedDate)
      } catch (err) {
        this.error = err.message || 'Could not load this draw'
      }
    }
  }
}
</script>

<style scoped>
.winners {
  display: flex;
  flex-direction: column;
  gap: 22px;
  text-align: left;
}

.winners-header {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
}

.winners-header h1 {
  margin: 0;
  font-size: 28px;
}

.draw-picker {
  display: flex;
  align-items: center;
  gap: 8px;
}

.draw-picker button {
  width: 44px;
  height: 44px;
  border: 1px solid #2b2b2b;
  border-radius: 6px;
  background: none;
  font-size: 18px;
  cursor: pointer;
}

.draw-picker button:disabled {
  border-color: #c9c9c9;
  color: #c9c9c9;
  cursor: not-allowed;
}

.draw-picker select {
  min-height: 44px;
  padding: 0 12px;
  border: 1px solid #c9c9c9;
  border-radius: 6px;
  font: inherit;
  font-size: 16px;
}

.status {
  margin: 0;
  color: #6b6b6b;
}

.status.error {
  color: #b3261e;
}

.retry {
  min-height: 44px;
  padding: 0 12px;
  margin-left: 8px;
  border: 1px solid #2b2b2b;
  border-radius: 6px;
  background: none;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.first-prize {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 18px;
  padding: 22px 28px;
  border: 2px solid #d97706;
  border-radius: 8px;
}

.eyebrow {
  margin: 0 0 6px 0;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 1px;
  text-transform: uppercase;
  color: #6b6b6b;
}

.first-number {
  margin: 0;
  font-size: 44px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  letter-spacing: 6px;
  color: #d2232a;
}

.chip-row {
  display: flex;
  gap: 10px;
}

.prize-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 18px;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  overflow: hidden;
  clip: rect(0 0 0 0);
  white-space: nowrap;
}

@media (max-width: 1023px) {
  .prize-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 767px) {
  .prize-grid {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
