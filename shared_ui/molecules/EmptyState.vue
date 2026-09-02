<template>
  <div class="empty-state">
    <span v-if="icon" class="empty-icon"><AppIcon :name="icon" :size="28" /></span>
    <p class="empty-title">{{ title }}</p>
    <p v-if="hint" class="empty-hint">{{ hint }}</p>
    <slot />
  </div>
</template>

<script>
import AppIcon from '../atoms/AppIcon.vue'

export default {
  name: 'EmptyState',
  components: { AppIcon },
  props: {
    icon: {
      type: String,
      default: null
    },
    title: {
      type: String,
      required: true
    },
    hint: {
      type: String,
      default: null
    }
  }
}
</script>

<style scoped>
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 36px 24px;
  text-align: center;
  background: var(--card);
  border: 1px dashed var(--line-strong);
  border-radius: var(--radius);
}

.empty-icon {
  color: var(--muted);
}

.empty-title {
  margin: 0;
  font-size: 17px;
  font-weight: 700;
}

.empty-hint {
  margin: 0;
  font-size: 14px;
  color: var(--muted);
  max-width: 420px;
}
</style>
