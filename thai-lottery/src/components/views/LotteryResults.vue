<template>
  <div class="results">
    <h1>Lottery Results</h1>

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
        Showing draws on this page where <strong>{{ searchQuery }}</strong> appears among the winning numbers.
      </p>
      <div class="draw-list">
        <DrawCard
          v-for="date in visibleDates"
          :key="date"
          :date="date"
          :draw="draws[date] || null"
          :loading="loadingDates.includes(date)"
          :error="drawErrors[date] || null"
          :expanded="expandedDate === date"
          @toggle="toggleDraw(date)"
          @retry="fetchDraw(date)"
        />
      </div>
      <p v-if="!visibleDates.length" class="status">No draws match the current filters.</p>
      <div v-if="!selectedDate && totalPages > 1" class="pagination">
        <button type="button" :disabled="page === 1" @click="page -= 1">‹ Prev</button>
        <span>Page {{ page }} of {{ totalPages }}</span>
        <button type="button" :disabled="page === totalPages" @click="page += 1">Next ›</button>
      </div>
    </template>
  </div>
</template>

<script>
import DrawCard from '@/components/DrawCard.vue'
import { formatDrawDate, getDrawByDate, getDrawDates } from '@/services/lotteryApi'

const PAGE_SIZE = 6

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
      page: 1
    }
  },
  computed: {
    filteredDates() {
      if (this.selectedDate) {
        return this.dates.filter((date) => date === this.selectedDate)
      }
      return this.dates
    },
    totalPages() {
      return Math.max(1, Math.ceil(this.filteredDates.length / PAGE_SIZE))
    },
    pageDates() {
      const start = (this.page - 1) * PAGE_SIZE
      return this.filteredDates.slice(start, start + PAGE_SIZE)
    },
    visibleDates() {
      if (!this.searchQuery) return this.pageDates
      return this.pageDates.filter((date) => {
        const draw = this.draws[date]
        return draw ? this.drawMatches(draw, this.searchQuery) : false
      })
    }
  },
  watch: {
    page() {
      this.fetchPage()
    },
    selectedDate() {
      this.page = 1
      this.fetchPage()
    }
  },
  created() {
    this.loadDates()
  },
  methods: {
    formatDrawDate,
    async loadDates() {
      this.listError = null
      try {
        this.dates = await getDrawDates()
        this.fetchPage()
      } catch (err) {
        this.listError = err.message || 'Could not load draw history'
      }
    },
    fetchPage() {
      this.pageDates.forEach((date) => this.fetchDraw(date))
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
      this.page = 1
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
  border: 1px solid #c9c9c9;
  border-radius: 6px;
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

.draw-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 18px;
}

.pagination button {
  min-height: 44px;
  padding: 0 16px;
  border: 1px solid #2b2b2b;
  border-radius: 6px;
  background: none;
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.pagination button:disabled {
  border-color: #c9c9c9;
  color: #c9c9c9;
  cursor: not-allowed;
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
