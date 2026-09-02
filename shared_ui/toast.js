import { readonly, ref } from 'vue'

// Module-level so any component (and ToastHost) share one queue.
const toasts = ref([])
let nextId = 1

export function useToasts() {
  function push(message, tone = 'info', timeoutMs = 4000) {
    const id = nextId++
    toasts.value = [...toasts.value, { id, message, tone }]
    if (timeoutMs > 0) setTimeout(() => dismiss(id), timeoutMs)
    return id
  }

  function dismiss(id) {
    toasts.value = toasts.value.filter((t) => t.id !== id)
  }

  return { toasts: readonly(toasts), push, dismiss }
}
