<template>
  <div class="toast-host" aria-live="polite">
    <div v-for="toast in toasts" :key="toast.id" class="toast" :class="toast.tone">
      <span class="toast-message">{{ toast.message }}</span>
      <button type="button" class="toast-close" :aria-label="`Dismiss: ${toast.message}`" @click="dismiss(toast.id)">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" aria-hidden="true">
          <line x1="5" y1="5" x2="19" y2="19"></line>
          <line x1="19" y1="5" x2="5" y2="19"></line>
        </svg>
      </button>
    </div>
  </div>
</template>

<script>
import { useToasts } from '../toast'

export default {
  name: 'ToastHost',
  setup() {
    const { toasts, dismiss } = useToasts()
    return { toasts, dismiss }
  }
}
</script>

<style scoped>
.toast-host {
  position: fixed;
  left: 50%;
  bottom: 24px;
  transform: translateX(-50%);
  z-index: 100;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  pointer-events: none;
}

.toast {
  display: flex;
  align-items: center;
  gap: 8px;
  max-width: min(92vw, 480px);
  padding: 10px 8px 10px 18px;
  background: var(--ink);
  color: #ffffff;
  border-radius: var(--radius-pill);
  box-shadow: var(--shadow-lg);
  pointer-events: auto;
  animation: toast-in 0.2s ease-out;
}

@media (prefers-reduced-motion: reduce) {
  .toast {
    animation: none;
  }
}

@keyframes toast-in {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.toast.success {
  background: var(--success);
}

.toast.danger {
  background: var(--danger);
}

.toast-message {
  font-size: 15px;
  font-weight: 600;
}

.toast-close {
  min-width: 36px;
  min-height: 36px;
  border: none;
  background: none;
  color: inherit;
  font-size: 13px;
  cursor: pointer;
  opacity: 0.7;
}

.toast-close:hover {
  opacity: 1;
}
</style>
