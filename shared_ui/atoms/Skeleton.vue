<template>
  <span class="skeleton" :style="{ width, height, borderRadius: rounded ? 'var(--radius)' : '6px' }" aria-hidden="true"></span>
</template>

<script>
export default {
  name: 'SkeletonBlock',
  props: {
    width: {
      type: String,
      default: '100%'
    },
    height: {
      type: String,
      default: '16px'
    },
    rounded: {
      type: Boolean,
      default: false
    }
  }
}
</script>

<style scoped>
.skeleton {
  display: block;
  background: linear-gradient(90deg, var(--line) 25%, var(--cream) 50%, var(--line) 75%);
  background-size: 200% 100%;
  animation: skeleton-shimmer 1.4s ease infinite;
}

@media (prefers-reduced-motion: reduce) {
  .skeleton {
    animation: none;
  }
}

@keyframes skeleton-shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}
</style>
