import { readonly, ref } from 'vue'

const STORAGE_KEY = 'htawpyi-tickets'

function loadStored() {
  try {
    const stored = JSON.parse(localStorage.getItem(STORAGE_KEY))
    return Array.isArray(stored) ? stored.filter((t) => /^\d{6}$/.test(t)) : []
  } catch {
    return []
  }
}

function persist(tickets) {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(tickets))
  } catch {
    // Storage unavailable (private mode, etc.) — tickets stay in memory only.
  }
}

// Module-level so the checker and the My Tickets list share one source of truth.
const tickets = ref(loadStored())

export function useMyTickets() {
  function add(ticket) {
    if (!/^\d{6}$/.test(ticket) || tickets.value.includes(ticket)) return false
    tickets.value = [...tickets.value, ticket]
    persist(tickets.value)
    return true
  }

  function remove(ticket) {
    tickets.value = tickets.value.filter((t) => t !== ticket)
    persist(tickets.value)
  }

  return { tickets: readonly(tickets), add, remove }
}
