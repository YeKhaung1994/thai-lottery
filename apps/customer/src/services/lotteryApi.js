import axios from 'axios'

// GLO has no CORS headers; in dev, /glo is proxied to https://www.glo.or.th
// (see vue.config.js). Production deployments set VUE_APP_LOTTERY_API_BASE
// to their own reverse proxy.
const BASE = process.env.VUE_APP_LOTTERY_API_BASE || '/glo'

const client = axios.create({
  baseURL: `${BASE}/api/lottery`,
  headers: { 'Content-Type': 'application/json' },
  timeout: 15000
})

const PRIZE_META = [
  { key: 'first', name: '1st Prize' },
  { key: 'near1', name: 'Adjacent to 1st' },
  { key: 'second', name: '2nd Prize' },
  { key: 'third', name: '3rd Prize' },
  { key: 'fourth', name: '4th Prize' },
  { key: 'fifth', name: '5th Prize' }
]

// GLO shape: response.data.<group> = { price: "6000000.00", number: [{round, value}] }
export function normalizeDraw(response) {
  const data = response.data || {}
  const numbersOf = (key) => ((data[key] && data[key].number) || []).map((n) => n.value).sort()
  const rewardOf = (key) => (data[key] ? Math.round(parseFloat(data[key].price)) : 0)

  return {
    date: response.date,
    pdfUrl: response.pdf_url || null,
    prizes: PRIZE_META.filter((p) => data[p.key]).map((p) => ({
      id: p.key,
      name: p.name,
      reward: rewardOf(p.key),
      numbers: numbersOf(p.key)
    })),
    firstPrize: numbersOf('first')[0] || null,
    firstReward: rewardOf('first'),
    adjacent: numbersOf('near1'),
    adjacentReward: rewardOf('near1'),
    front3: numbersOf('last3f'),
    front3Reward: rewardOf('last3f'),
    back3: numbersOf('last3b'),
    back3Reward: rewardOf('last3b'),
    last2: numbersOf('last2')[0] || null,
    last2Reward: rewardOf('last2')
  }
}

const drawCache = new Map()

export async function getLatestDraw() {
  const { data } = await client.post('/getLatestLottery', {})
  if (!data || !data.response) throw new Error('Empty response from lottery API')
  const draw = normalizeDraw(data.response)
  drawCache.set(draw.date, draw)
  return draw
}

// isoDate: "2026-08-16"
export async function getDrawByDate(isoDate) {
  if (drawCache.has(isoDate)) return drawCache.get(isoDate)
  const [year, month, day] = isoDate.split('-')
  const { data } = await client.post('/getLotteryResult', { date: day, month, year })
  if (!data || !data.response) throw new Error(`No result for draw ${isoDate}`)
  const draw = normalizeDraw(data.response)
  drawCache.set(isoDate, draw)
  return draw
}

// → ISO date strings, newest first
export async function getDrawDates() {
  const { data } = await client.post('/getPeriodList', {})
  if (!data || !data.response || !Array.isArray(data.response.list)) {
    throw new Error('Empty period list from lottery API')
  }
  return data.response.list
}

// Every prize a 6-digit ticket wins in a draw. A ticket can win more than
// one running-number prize at once.
export function checkTicket(draw, ticket) {
  if (!/^\d{6}$/.test(ticket)) return null
  const wins = []
  for (const prize of draw.prizes) {
    if (prize.numbers.includes(ticket)) {
      wins.push({ name: prize.name, reward: prize.reward })
    }
  }
  if (draw.front3.includes(ticket.slice(0, 3))) {
    wins.push({ name: '3-Digit Front', reward: draw.front3Reward })
  }
  if (draw.back3.includes(ticket.slice(-3))) {
    wins.push({ name: '3-Digit Back', reward: draw.back3Reward })
  }
  if (draw.last2 && ticket.slice(-2) === draw.last2) {
    wins.push({ name: '2-Digit', reward: draw.last2Reward })
  }
  return wins
}



// Draws are on the 1st and 16th of each month. Given the last draw's ISO
// date, returns the next draw as a Date (local midnight).
export function nextDrawDateFrom(lastDrawIso) {
  const last = new Date(`${lastDrawIso}T00:00:00`)
  if (Number.isNaN(last.getTime())) return null
  const next = new Date(last)
  if (last.getDate() < 16) {
    next.setDate(16)
  } else {
    next.setMonth(next.getMonth() + 1, 1)
  }
  return next
}

// Formatters live in the shared UI library; re-exported for convenience.
// (Imported from the utils module directly so pure-JS consumers — and Jest,
// which has no .vue transformer — never touch the component barrel.)
export { formatBaht, formatDrawDate } from '@htawpyi/shared-ui/utils/format'
