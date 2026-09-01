<template>
  <div class="home">
    <div class="hero">
      <section class="latest-card">
        <template v-if="draw">
          <p class="eyebrow"><AppIcon name="calendar" :size="15" /> Latest draw · {{ dateLabel }}</p>
          <h1>First Prize Winning Number</h1>
          <DigitTiles :number="draw.firstPrize" accent />
          <p class="reward">Prize: {{ formatBaht(draw.firstReward) }}</p>
        </template>
        <template v-else-if="loading">
          <p class="eyebrow">Latest draw</p>
          <h1>Loading the latest results…</h1>
        </template>
        <template v-else>
          <p class="eyebrow">Latest draw</p>
          <h1>Couldn't load results</h1>
          <p class="reward">{{ error }}</p>
          <button type="button" class="retry" @click="retry">Try again</button>
        </template>
      </section>
      <TicketChecker :draw="draw" />
    </div>

    <MyTickets :draw="draw" />

    <section v-if="draw" class="glance">
      <h2>Latest draw at a glance</h2>
      <div class="glance-grid">
        <div class="glance-card tint-red">
          <span class="glance-label"><AppIcon name="hash" :size="16" /> 2-Digit</span>
          <span class="glance-value">{{ draw.last2 }}</span>
          <span class="glance-sub">{{ formatBaht(draw.last2Reward) }}</span>
        </div>
        <div class="glance-card tint-gold">
          <span class="glance-label"><AppIcon name="ticket" :size="16" /> 3-Digit Front</span>
          <span class="glance-value">{{ draw.front3.join(' · ') }}</span>
          <span class="glance-sub">{{ formatBaht(draw.front3Reward) }}</span>
        </div>
        <div class="glance-card tint-teal">
          <span class="glance-label"><AppIcon name="ticket" :size="16" /> 3-Digit Back</span>
          <span class="glance-value">{{ draw.back3.join(' · ') }}</span>
          <span class="glance-sub">{{ formatBaht(draw.back3Reward) }}</span>
        </div>
        <div class="glance-card">
          <span class="glance-label"><AppIcon name="clock" :size="16" /> Next draw</span>
          <span class="glance-value">{{ nextDrawLabel }}</span>
          <span class="glance-sub">{{ nextDrawCountdown }}</span>
        </div>
      </div>
    </section>

    <section class="recent">
      <div class="recent-header">
        <h2>Recent draws</h2>
        <router-link to="/results">View all results →</router-link>
      </div>
      <p v-if="recentError" class="recent-note">{{ recentError }}</p>
      <div v-else class="recent-list">
        <router-link v-for="item in recentDraws" :key="item.date" class="recent-row" :to="`/winners/${item.date}`">
          <span>{{ formatDrawDate(item.date) }}</span>
          <span class="recent-number">First prize: {{ item.firstPrize || '…' }}</span>
          <span class="recent-more">details ›</span>
        </router-link>
      </div>
    </section>
  </div>
</template>

<script>
import AppIcon from '@/components/AppIcon.vue'
import DigitTiles from '@/components/DigitTiles.vue'
import TicketChecker from '@/components/TicketChecker.vue'
import MyTickets from '@/components/MyTickets.vue'
import { useLatestDraw } from '@/composables/useLatestDraw'
import { formatBaht, formatDrawDate, getDrawByDate, getDrawDates } from '@/services/lotteryApi'

export default {
  name: 'LotteryHome',
  components: { AppIcon, DigitTiles, TicketChecker, MyTickets },
  setup() {
    const { draw, loading, error, retry } = useLatestDraw()
    return { draw, loading, error, retry }
  },
  data() {
    return {
      recentDraws: [],
      recentError: null
    }
  },
  computed: {
    dateLabel() {
      return this.draw ? formatDrawDate(this.draw.date) : ''
    },
    nextDrawDate() {
      // Draws are on the 1st and 16th of each month.
      if (!this.draw) return null
      const last = new Date(`${this.draw.date}T00:00:00`)
      const next = new Date(last)
      if (last.getDate() < 16) {
        next.setDate(16)
      } else {
        next.setMonth(next.getMonth() + 1, 1)
      }
      return next
    },
    nextDrawLabel() {
      return this.nextDrawDate
        ? this.nextDrawDate.toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })
        : ''
    },
    nextDrawCountdown() {
      if (!this.nextDrawDate) return ''
      const days = Math.ceil((this.nextDrawDate - new Date()) / 86400000)
      if (days <= 0) return 'today'
      return `in ${days} day${days === 1 ? '' : 's'}`
    }
  },
  created() {
    this.loadRecent()
  },
  methods: {
    formatBaht,
    formatDrawDate,
    async loadRecent() {
      try {
        const dates = await getDrawDates()
        this.recentDraws = dates.slice(1, 4).map((date) => ({ date, firstPrize: null }))
        // Map over the reactive array so mutations trigger re-renders.
        await Promise.all(
          this.recentDraws.map(async (item) => {
            try {
              const result = await getDrawByDate(item.date)
              item.firstPrize = result.firstPrize
            } catch {
              item.firstPrize = '—'
            }
          })
        )
      } catch (err) {
        this.recentError = err.message || 'Could not load recent draws'
      }
    }
  }
}
</script>

<style scoped>
.home {
  display: flex;
  flex-direction: column;
  gap: 36px;
  text-align: left;
}

.hero {
  display: grid;
  grid-template-columns: 1.4fr 1fr;
  gap: 28px;
  align-items: stretch;
}

.latest-card {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 24px 28px;
  background: linear-gradient(160deg, #fffdf7 0%, var(--card) 55%);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.eyebrow {
  display: flex;
  align-items: center;
  gap: 6px;
  margin: 0;
  font-size: 14px;
  font-weight: 700;
  letter-spacing: 1px;
  text-transform: uppercase;
  color: var(--muted);
}

.latest-card h1 {
  margin: 0;
  font-size: 26px;
}

.reward {
  margin: 0;
  font-size: 18px;
}

.retry {
  align-self: flex-start;
  min-height: 44px;
  padding: 0 20px;
  border: 1px solid #2b2b2b;
  border-radius: 6px;
  background: none;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.glance h2,
.recent h2 {
  margin: 0 0 14px 0;
  font-size: 22px;
}

.glance-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 18px;
}

.glance-card {
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding: 16px 18px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.glance-card.tint-red {
  background: var(--red-tint);
  border-color: #f5d4d5;
}

.glance-card.tint-red .glance-label {
  color: var(--red);
}

.glance-card.tint-gold {
  background: var(--amber-tint);
  border-color: #f0ddb6;
}

.glance-card.tint-gold .glance-label {
  color: var(--amber-dark);
}

.glance-card.tint-teal {
  background: var(--teal-tint);
  border-color: #cde7ed;
}

.glance-card.tint-teal .glance-label {
  color: #23808f;
}

.glance-label {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 1px;
  text-transform: uppercase;
  color: var(--muted);
}

.glance-value {
  font-size: 26px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
}

.glance-sub {
  font-size: 14px;
  color: #6b6b6b;
}

.recent-header {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  gap: 16px;
}

.recent-header a {
  color: #b45309;
  font-weight: 600;
  text-decoration: none;
}

.recent-header a:hover {
  color: #92400e;
}

.recent-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.recent-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  min-height: 52px;
  padding: 8px 20px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  color: var(--ink);
  text-decoration: none;
  transition: border-color 0.15s ease, transform 0.15s ease;
}

.recent-row:hover {
  border-color: var(--amber);
  transform: translateY(-1px);
}

.recent-number {
  font-weight: 600;
  font-variant-numeric: tabular-nums;
}

.recent-more {
  color: #6b6b6b;
  font-size: 14px;
}

.recent-note {
  color: #6b6b6b;
}

@media (max-width: 767px) {
  .hero {
    grid-template-columns: minmax(0, 1fr);
  }

  .glance-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .recent-row {
    flex-wrap: wrap;
  }
}
</style>
