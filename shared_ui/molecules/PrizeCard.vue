<template>
  <section class="prize-card">
    <header class="prize-header">
      <h3>{{ prize.name }}</h3>
      <span class="prize-meta">{{ rewardLabel }} · {{ prize.numbers.length }} number{{ prize.numbers.length === 1 ? '' : 's' }}</span>
    </header>
    <div class="chip-list">
      <NumberChip v-for="number in visibleNumbers" :key="number" :value="number" />
    </div>
    <button v-if="prize.numbers.length > collapseAt" type="button" class="toggle" @click="expanded = !expanded">
      {{ expanded ? 'Show fewer ▴' : `Show all ${prize.numbers.length} ▾` }}
    </button>
  </section>
</template>

<script>
import NumberChip from '../atoms/NumberChip.vue'
import { formatBaht } from '../utils/format'

export default {
  name: 'PrizeCard',
  components: { NumberChip },
  props: {
    prize: {
      type: Object,
      required: true
    },
    collapseAt: {
      type: Number,
      default: 10
    }
  },
  data() {
    return {
      expanded: false
    }
  },
  computed: {
    rewardLabel() {
      return formatBaht(this.prize.reward)
    },
    visibleNumbers() {
      if (this.expanded || this.prize.numbers.length <= this.collapseAt) {
        return this.prize.numbers
      }
      return this.prize.numbers.slice(0, this.collapseAt)
    }
  }
}
</script>

<style scoped>
.prize-card {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 12px;
  padding: 18px 20px;
  border: 1px solid var(--line);
  border-radius: var(--radius);
  background: var(--card);
  box-shadow: var(--shadow);
  text-align: left;
}

.prize-header {
  display: flex;
  flex-wrap: wrap;
  align-items: baseline;
  gap: 8px 12px;
  width: 100%;
}

.prize-header h3 {
  margin: 0;
  font-size: 19px;
}

.prize-meta {
  font-size: 14px;
  color: #6b6b6b;
}

.chip-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.toggle {
  min-height: 44px;
  padding: 0 12px;
  border: none;
  background: none;
  font: inherit;
  font-size: 15px;
  font-weight: 600;
  color: #b45309;
  cursor: pointer;
}

.toggle:hover {
  color: #92400e;
}
</style>
