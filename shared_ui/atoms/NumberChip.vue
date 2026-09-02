<template>
  <span class="number-chip" :class="{ highlight }">{{ value }}</span>
</template>

<script>
export default {
  name: 'NumberChip',
  props: {
    value: {
      type: String,
      required: true
    },
    highlight: {
      type: Boolean,
      default: false
    }
  }
}
</script>

<style scoped>
.number-chip {
  display: inline-flex;
  align-items: center;
  padding: 6px 14px;
  border: 1px solid var(--line);
  border-radius: 999px;
  font-size: 16px;
  font-weight: 500;
  font-family: var(--font-mono);
  font-variant-numeric: tabular-nums;
  color: var(--ink);
  background: var(--cream);
}

.number-chip.highlight {
  border-color: var(--gold);
  background: var(--amber-tint);
  color: var(--red);
}
</style>
