// HtiMart shared UI — atomic design library.
// Atoms: smallest building blocks. Molecules: compositions of atoms.
// Consumers must import '@htawpyi/shared-ui/tokens.css' once (main.js).

export { default as AppIcon } from './atoms/AppIcon.vue'
export { default as NumberChip } from './atoms/NumberChip.vue'
export { default as DigitTiles } from './atoms/DigitTiles.vue'
export { default as StatusPill } from './atoms/StatusPill.vue'
export { default as TicketNumber } from './atoms/TicketNumber.vue'
export { default as BaseButton } from './atoms/BaseButton.vue'
export { default as TextField } from './atoms/TextField.vue'
export { default as SkeletonBlock } from './atoms/Skeleton.vue'
export { default as CountdownTimer } from './atoms/CountdownTimer.vue'

export { default as PrizeCard } from './molecules/PrizeCard.vue'
export { default as DrawCard } from './molecules/DrawCard.vue'
export { default as StatTile } from './molecules/StatTile.vue'
export { default as EmptyState } from './molecules/EmptyState.vue'
export { default as ToastHost } from './molecules/ToastHost.vue'

export { default as DataTable } from './organisms/DataTable.vue'

export { useToasts } from './toast'
export { formatBaht, formatDrawDate } from './utils/format'
export { matchesDigitPattern, hasDouble, hasTriple, endsWith } from './utils/tickets'
