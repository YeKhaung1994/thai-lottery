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
  border: 1px solid #c9c9c9;
  border-radius: 16px;
  font-size: 16px;
  font-weight: 600;
  font-variant-numeric: tabular-nums;
  color: #2b2b2b;
  background: #ffffff;
}

.number-chip.highlight {
  border-color: #d97706;
  color: #d97706;
}
</style>
