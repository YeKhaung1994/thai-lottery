<template>
  <label class="text-field">
    <span class="field-label" :class="{ 'sr-only': hideLabel }">{{ label }}</span>
    <input
      v-bind="$attrs"
      :value="modelValue"
      :type="type"
      @input="$emit('update:modelValue', $event.target.value)"
    />
    <span v-if="error" class="field-error" role="alert">{{ error }}</span>
    <span v-else-if="hint" class="field-hint">{{ hint }}</span>
  </label>
</template>

<script>
export default {
  name: 'TextField',
  inheritAttrs: false,
  props: {
    modelValue: {
      type: [String, Number],
      default: ''
    },
    label: {
      type: String,
      required: true
    },
    hideLabel: {
      type: Boolean,
      default: false
    },
    type: {
      type: String,
      default: 'text'
    },
    error: {
      type: String,
      default: null
    },
    hint: {
      type: String,
      default: null
    }
  },
  emits: ['update:modelValue']
}
</script>

<style scoped>
.text-field {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.field-label {
  font-size: 14px;
  font-weight: 600;
  color: var(--muted);
}

input {
  min-height: 48px;
  padding: 0 14px;
  border: 1px solid var(--line);
  border-radius: 8px;
  font: inherit;
  font-size: 16px;
  background: var(--card);
  box-sizing: border-box;
  width: 100%;
}

input:focus {
  outline: 2px solid var(--amber);
  outline-offset: 1px;
}

.field-error {
  font-size: 14px;
  color: var(--danger);
}

.field-hint {
  font-size: 13px;
  color: var(--muted);
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
