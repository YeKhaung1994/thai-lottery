<template>
  <div class="digit-tiles" :class="{ small: size === 'small' }">
    <span v-for="(digit, index) in digits" :key="index" class="tile">{{ digit }}</span>
  </div>
</template>

<script>
export default {
  name: 'DigitTiles',
  props: {
    number: {
      type: String,
      required: true
    },
    size: {
      type: String,
      default: 'large'
    }
  },
  computed: {
    digits() {
      return this.number.split('')
    }
  }
}
</script>

<style scoped>
.digit-tiles {
  display: flex;
  gap: 10px;
}

.tile {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 56px;
  height: 68px;
  border: 2px solid #2b2b2b;
  border-radius: 6px;
  font-size: 38px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  background: #ffffff;
}

.small .tile {
  width: 36px;
  height: 44px;
  font-size: 24px;
  border-width: 1px;
}

@media (max-width: 767px) {
  .digit-tiles {
    gap: 6px;
  }

  .tile {
    flex: 1;
    width: auto;
    min-width: 0;
    height: 56px;
    font-size: 28px;
  }
}
</style>
