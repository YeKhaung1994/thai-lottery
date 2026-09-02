import { computed, readonly, ref } from 'vue'

// Module-level so BuyTickets, Checkout, and the header share one cart.
const items = ref([]) // full ticket objects {id, number, drawDate, price}

export function useCart() {
  function toggle(ticket) {
    const next = items.value.filter((t) => t.id !== ticket.id)
    if (next.length === items.value.length) {
      if (next.length >= 10) return false
      next.push(ticket)
    }
    items.value = next
    return true
  }

  function remove(id) {
    items.value = items.value.filter((t) => t.id !== id)
  }

  function clear() {
    items.value = []
  }

  return {
    items: readonly(items),
    ids: computed(() => items.value.map((t) => t.id)),
    total: computed(() => items.value.reduce((sum, t) => sum + t.price, 0)),
    count: computed(() => items.value.length),
    has: (id) => items.value.some((t) => t.id === id),
    toggle,
    remove,
    clear
  }
}
