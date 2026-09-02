<template>
  <svg
    :width="size"
    :height="size"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    stroke-width="2"
    stroke-linecap="round"
    stroke-linejoin="round"
    aria-hidden="true"
  >
    <template v-if="name === 'ticket'">
      <path d="M3 9V7a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2v2a3 3 0 0 0 0 6v2a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-2a3 3 0 0 0 0-6Z"></path>
      <line x1="14" y1="5" x2="14" y2="19" stroke-dasharray="2 3"></line>
    </template>
    <template v-else-if="name === 'trophy'">
      <path d="M7 4h10v5a5 5 0 0 1-10 0Z"></path>
      <path d="M7 6H4.5a2.5 2.5 0 0 0 2.8 4"></path>
      <path d="M17 6h2.5a2.5 2.5 0 0 1-2.8 4"></path>
      <line x1="12" y1="14" x2="12" y2="18"></line>
      <path d="M8 21h8"></path>
    </template>
    <template v-else-if="name === 'calendar'">
      <rect x="3" y="5" width="18" height="16" rx="2"></rect>
      <line x1="3" y1="10" x2="21" y2="10"></line>
      <line x1="8" y1="3" x2="8" y2="6"></line>
      <line x1="16" y1="3" x2="16" y2="6"></line>
    </template>
    <template v-else-if="name === 'clock'">
      <circle cx="12" cy="12" r="9"></circle>
      <path d="M12 7v5l3 2"></path>
    </template>
    <template v-else-if="name === 'hash'">
      <line x1="4" y1="9" x2="20" y2="9"></line>
      <line x1="4" y1="15" x2="20" y2="15"></line>
      <line x1="10" y1="4" x2="8" y2="20"></line>
      <line x1="16" y1="4" x2="14" y2="20"></line>
    </template>
    <template v-else-if="name === 'search'">
      <circle cx="11" cy="11" r="7"></circle>
      <line x1="16.5" y1="16.5" x2="21" y2="21"></line>
    </template>
    <template v-else-if="name === 'sparkle'">
      <path d="M12 3l1.9 5.1L19 10l-5.1 1.9L12 17l-1.9-5.1L5 10l5.1-1.9Z"></path>
      <path d="M18.5 15.5l.8 2.2 2.2.8-2.2.8-.8 2.2-.8-2.2-2.2-.8 2.2-.8Z"></path>
    </template>
  </svg>
</template>

<script>
export default {
  name: 'AppIcon',
  props: {
    name: {
      type: String,
      required: true
    },
    size: {
      type: Number,
      default: 20
    }
  }
}
</script>
