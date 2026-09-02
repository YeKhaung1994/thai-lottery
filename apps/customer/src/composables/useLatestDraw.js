import { ref, readonly } from 'vue'
import { getLatestDraw } from '@/services/lotteryApi'

// Module-level state: every component shares one fetch of the latest draw.
const draw = ref(null)
const loading = ref(false)
const error = ref(null)
let inflight = null

export function useLatestDraw() {
  function load() {
    if (draw.value || inflight) return inflight
    loading.value = true
    error.value = null
    inflight = getLatestDraw()
      .then((result) => {
        draw.value = result
      })
      .catch((err) => {
        error.value = err.message || 'Could not load the latest draw'
      })
      .finally(() => {
        loading.value = false
        inflight = null
      })
    return inflight
  }

  function retry() {
    draw.value = null
    error.value = null
    return load()
  }

  // Re-fetch without clearing the current draw — no loading flash. Used by
  // draw-day polling.
  function refresh() {
    if (inflight) return inflight
    inflight = getLatestDraw()
      .then((result) => {
        draw.value = result
        error.value = null
      })
      .catch(() => {
        // Keep showing the last known draw; the next poll retries.
      })
      .finally(() => {
        inflight = null
      })
    return inflight
  }

  load()

  return { draw: readonly(draw), loading: readonly(loading), error: readonly(error), retry, refresh }
}
