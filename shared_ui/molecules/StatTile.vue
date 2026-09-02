<template>
  <div class="stat-tile" :class="tone ? `tint-${tone}` : ''">
    <span class="stat-label"><slot name="icon" /> {{ label }}</span>
    <span class="stat-value">{{ value }}</span>
    <span v-if="sub" class="stat-sub">{{ sub }}</span>
  </div>
</template>

<script>
export default {
  name: 'StatTile',
  props: {
    label: {
      type: String,
      required: true
    },
    value: {
      type: [String, Number],
      required: true
    },
    sub: {
      type: String,
      default: null
    },
    tone: {
      type: String,
      default: null,
      validator: (v) => ['red', 'gold', 'teal', 'success'].includes(v)
    }
  }
}
</script>

<style scoped>
.stat-tile {
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding: 16px 18px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.stat-label {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 1px;
  text-transform: uppercase;
  color: var(--muted);
}

.stat-value {
  font-family: var(--font-display);
  font-size: 26px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
}

.stat-sub {
  font-size: 14px;
  color: var(--muted);
}

.tint-red {
  background: var(--red-tint);
  border-color: #f5d4d5;
}

.tint-red .stat-label {
  color: var(--red);
}

.tint-gold {
  background: var(--amber-tint);
  border-color: #f0ddb6;
}

.tint-gold .stat-label {
  color: var(--amber-dark);
}

.tint-teal {
  background: var(--teal-tint);
  border-color: #cde7ed;
}

.tint-teal .stat-label {
  color: #23808f;
}

.tint-success {
  background: var(--success-tint);
  border-color: #cfe8cf;
}

.tint-success .stat-label {
  color: var(--success);
}
</style>
