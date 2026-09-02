<template>
  <button :type="type" class="base-button" :class="[variant, size]" :disabled="disabled">
    <slot />
  </button>
</template>

<script>
export default {
  name: 'BaseButton',
  props: {
    variant: {
      type: String,
      default: 'primary',
      validator: (v) => ['primary', 'secondary', 'ghost', 'danger'].includes(v)
    },
    size: {
      type: String,
      default: 'md',
      validator: (v) => ['md', 'lg'].includes(v)
    },
    type: {
      type: String,
      default: 'button'
    },
    disabled: {
      type: Boolean,
      default: false
    }
  }
}
</script>

<style scoped>
.base-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  min-height: 44px;
  padding: 0 20px;
  border-radius: 8px;
  border: 1px solid transparent;
  font: inherit;
  font-size: 15px;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.15s ease, border-color 0.15s ease, color 0.15s ease;
}

.base-button.lg {
  min-height: 48px;
  padding: 0 28px;
  font-size: 17px;
}

.base-button.primary {
  background: var(--amber);
  color: #ffffff;
}

.base-button.primary:hover:not(:disabled) {
  background: var(--amber-dark);
}

.base-button.secondary {
  background: var(--card);
  border-color: var(--ink);
  color: var(--ink);
}

.base-button.secondary:hover:not(:disabled) {
  border-color: var(--amber);
  color: var(--amber-dark);
}

.base-button.ghost {
  background: none;
  color: var(--amber-dark);
  padding: 0 12px;
}

.base-button.ghost:hover:not(:disabled) {
  color: var(--amber);
}

.base-button.danger {
  background: var(--card);
  border-color: var(--line);
  color: var(--muted);
}

.base-button.danger:hover:not(:disabled) {
  border-color: var(--danger);
  color: var(--danger);
}

.base-button:disabled {
  background: #c9c9c9;
  border-color: transparent;
  color: #ffffff;
  cursor: not-allowed;
}

.base-button.ghost:disabled,
.base-button.danger:disabled {
  background: none;
  color: #c9c9c9;
}
</style>
