<template>
  <div class="results">
    <h1>Draw History</h1>

    <div class="filter-bar">
      <label class="search-field">
        <span class="sr-only">Search a number</span>
        <input
          v-model.trim="searchQuery"
          type="text"
          inputmode="numeric"
          placeholder="Search a number (full or partial, e.g. 417212 or 04)"
        />
      </label>
      <label class="date-field">
        <span class="sr-only">Filter by draw date</span>
        <select v-model="selectedDate">
          <option :value="null">All draw dates</option>
          <option v-for="date in dates" :key="date" :value="date">{{ formatDrawDate(date) }}</option>
        </select>
      </label>
      <button v-if="searchQuery || selectedDate" type="button" class="clear" @click="clearFilters">Clear filters</button>
    </div>

    <p v-if="listError" class="status error">
      {{ listError }}
      <button type="button" class="retry" @click="loadDates">Retry</button>
    </p>
    <p v-else-if="!dates.length" class="status">Loading draw history…</p>
    <template v-else>
      <p v-if="searchQuery" class="status">
        Showing loaded draws where <strong>{{ searchQuery }}</strong> appears among the winning numbers.
      </p>
      <section v-for="group in visibleGroups" :key="group.key" class="month-group">
        <h2 class="month-heading">{{ group.label }}</h2>
        <div class="draw-list">
          <DrawCard
            v-for="date in datesToShow(group)"
            :key="date"
            :date="date"
            :draw="draws[date] || null"
            :loading="loadingDates.includes(date)"
            :error="drawErrors[date] || null"
            :expanded="expandedDate === date"
            :details-to="`/draws/${date}`"
            @toggle="toggleDraw(date)"
            @retry="fetchDraw(date)"
          />
          <p v-if="!datesToShow(group).length" class="status">No draws in {{ group.label }} match the search.</p>
        </div>
      </section>
      <p v-if="!visibleGroups.length" class="status">No draws match the current filters.</p>
      <button
        v-if="!selectedDate && visibleGroups.length < groupedMonths.length"
        type="button"
        class="load-more"
        @click="monthsShown += 3"
      >
        Show earlier months ▾
      </button>
    </template>
  </div>
</template>

<script>
import { DrawCard } from '@htawpyi/shared-ui'
import { formatDrawDate, getDrawByDate, getDrawDates } from '@/services/lotteryApi'

export default {
  name: 'LotteryResults',
  components: { DrawCard },
  data() {
    return {
      dates: [],
      listError: null,
      draws: {},
      drawErrors: {},
      loadingDates: [],
      expandedDate: null,
      searchQuery: '',
      selectedDate: null,
      monthsShown: 3
    }
  },
  computed: {
    filteredDates() {
      if (this.selectedDate) {
        return this.dates.filter((date) => date === this.selectedDate)
      }
      return this.dates
    },
    groupedMonths() {
      const groups = []
      for (const date of this.filteredDates) {
        const key = date.slice(0, 7)
        let group = groups[groups.length - 1]
        if (!group || group.key !== key) {
          group = { key, label: this.monthLabel(date), dates: [] }
          groups.push(group)
        }
        group.dates.push(date)
      }
      return groups
    },
    visibleGroups() {
      if (this.selectedDate) return this.groupedMonths
      return this.groupedMonths.slice(0, this.monthsShown)
    },
    visibleDates() {
      return this.visibleGroups.flatMap((group) => group.dates)
    }
  },
  watch: {
    visibleDates(dates) {
      dates.forEach((date) => this.fetchDraw(date))
    }
  },
  created() {
    this.loadDates()
  },
  methods: {
    formatDrawDate,
    monthLabel(isoDate) {
      const d = new Date(`${isoDate}T00:00:00`)
      return d.toLocaleDateString('en-GB', { month: 'long', year: 'numeric' })
    },
    datesToShow(group) {
      if (!this.searchQuery) return group.dates
      return group.dates.filter((date) => {
        const draw = this.draws[date]
        return draw ? this.drawMatches(draw, this.searchQuery) : false
      })
    },
    async loadDates() {
      this.listError = null
      try {
        this.dates = await getDrawDates()
        this.visibleDates.forEach((date) => this.fetchDraw(date))
      } catch (err) {
        this.listError = err.message || 'Could not load draw history'
      }
    },
    async fetchDraw(date) {
      if (this.draws[date] || this.loadingDates.includes(date)) return
      this.loadingDates.push(date)
      delete this.drawErrors[date]
      try {
        this.draws[date] = await getDrawByDate(date)
      } catch (err) {
        this.drawErrors[date] = err.message || 'Failed to load this draw'
      } finally {
        this.loadingDates = this.loadingDates.filter((d) => d !== date)
      }
    },
    toggleDraw(date) {
      this.expandedDate = this.expandedDate === date ? null : date
      if (this.expandedDate) this.fetchDraw(date)
    },
    drawMatches(draw, query) {
      const all = [
        ...draw.prizes.flatMap((p) => p.numbers),
        ...draw.front3,
        ...draw.back3,
        draw.last2 || ''
      ]
      return all.some((number) => number.includes(query))
    },
    clearFilters() {
      this.searchQuery = ''
      this.selectedDate = null
    }
  }
}
</script>

<style scoped>
.results {
  display: flex;
  flex-direction: column;
  gap: 20px;
  text-align: left;
}

.results h1 {
  margin: 0;
  font-size: 28px;
}

.filter-bar {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}

.search-field {
  flex: 1;
  min-width: 240px;
}

.search-field input,
.date-field select {
  width: 100%;
  min-height: 48px;
  padding: 0 14px;
  border: 1px solid var(--line);
  border-radius: 8px;
  font: inherit;
  font-size: 16px;
  box-sizing: border-box;
  background: #ffffff;
}

.search-field input:focus,
.date-field select:focus {
  outline: 2px solid #d97706;
  outline-offset: 1px;
}

.clear {
  min-height: 44px;
  padding: 0 12px;
  border: none;
  background: none;
  font: inherit;
  font-weight: 600;
  color: #b45309;
  cursor: pointer;
}

.clear:hover {
  color: #92400e;
}

.status {
  margin: 0;
  color: var(--muted);
}

.status.error {
  color: #b3261e;
}

.retry {
  min-height: 44px;
  padding: 0 12px;
  margin-left: 8px;
  border: 1px solid #2b2b2b;
  border-radius: 8px;
  background: none;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.month-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.month-heading {
  margin: 8px 0 0 0;
  font-size: 18px;
  font-weight: 700;
  color: var(--muted);
  text-transform: uppercase;
  letter-spacing: 1px;
}

.draw-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.load-more {
  align-self: center;
  min-height: 48px;
  padding: 0 24px;
  border: 1px solid #2b2b2b;
  border-radius: 8px;
  background: #ffffff;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.load-more:hover {
  border-color: #d97706;
  color: #b45309;
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
