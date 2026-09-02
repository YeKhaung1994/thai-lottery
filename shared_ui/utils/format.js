export function formatDrawDate(isoDate) {
  const d = new Date(`${isoDate}T00:00:00`)
  if (Number.isNaN(d.getTime())) return isoDate
  return d.toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })
}

export function formatBaht(amount) {
  return `฿${Number(amount).toLocaleString('en-US')}`
}
