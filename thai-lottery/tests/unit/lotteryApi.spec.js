import { checkTicket, formatBaht, formatDrawDate, normalizeDraw } from '@/services/lotteryApi'
import gloLatest from './fixtures/glo-latest.json'

// Fixture is a real GLO getLatestLottery response (draw of 2026-09-01).
const draw = normalizeDraw(gloLatest.response)

describe('normalizeDraw', () => {
  it('extracts the draw date and first prize', () => {
    expect(draw.date).toBe('2026-09-01')
    expect(draw.firstPrize).toBe('417212')
    expect(draw.firstReward).toBe(6000000)
  })

  it('maps all six prize groups with names and numbers', () => {
    expect(draw.prizes.map((p) => p.id)).toEqual(['first', 'near1', 'second', 'third', 'fourth', 'fifth'])
    const byId = Object.fromEntries(draw.prizes.map((p) => [p.id, p]))
    expect(byId.second.numbers).toHaveLength(5)
    expect(byId.third.numbers).toHaveLength(10)
    expect(byId.fourth.numbers).toHaveLength(50)
    expect(byId.fifth.numbers).toHaveLength(100)
    expect(byId.near1.numbers).toEqual(['417211', '417213'])
  })

  it('extracts running numbers and their rewards', () => {
    expect(draw.front3).toEqual(['257', '346'])
    expect(draw.back3).toEqual(['136', '740'])
    expect(draw.last2).toBe('04')
    expect(draw.front3Reward).toBe(4000)
    expect(draw.last2Reward).toBe(2000)
  })

  it('handles a draw with missing groups without crashing', () => {
    const sparse = normalizeDraw({ date: '2026-01-01', data: { first: { price: '100.00', number: [{ round: 1, value: '123456' }] } } })
    expect(sparse.firstPrize).toBe('123456')
    expect(sparse.prizes).toHaveLength(1)
    expect(sparse.front3).toEqual([])
    expect(sparse.last2).toBeNull()
  })
})

describe('checkTicket', () => {
  it('finds a first-prize win', () => {
    expect(checkTicket(draw, '417212')).toEqual([{ name: '1st Prize', reward: 6000000 }])
  })

  it('finds an adjacent-to-first win', () => {
    expect(checkTicket(draw, '417211')).toEqual([{ name: 'Adjacent to 1st', reward: 100000 }])
  })

  it('finds multiple running-number wins on one ticket', () => {
    expect(checkTicket(draw, '257136')).toEqual([
      { name: '3-Digit Front', reward: 4000 },
      { name: '3-Digit Back', reward: 4000 }
    ])
  })

  it('finds a 2-digit win', () => {
    expect(checkTicket(draw, '999904')).toEqual([{ name: '2-Digit', reward: 2000 }])
  })

  it('returns an empty list for a losing ticket', () => {
    expect(checkTicket(draw, '123456')).toEqual([])
  })

  it('rejects malformed tickets with null', () => {
    expect(checkTicket(draw, '12345')).toBeNull()
    expect(checkTicket(draw, '12345a')).toBeNull()
    expect(checkTicket(draw, '')).toBeNull()
  })
})

describe('formatters', () => {
  it('formats baht amounts with a currency sign and separators', () => {
    expect(formatBaht(6000000)).toBe('฿6,000,000')
    expect(formatBaht(2000)).toBe('฿2,000')
  })

  it('formats ISO draw dates for display', () => {
    // ICU renders September as "Sep" or "Sept" depending on version.
    expect(formatDrawDate('2026-09-01')).toMatch(/^1 Sept? 2026$/)
    expect(formatDrawDate('not-a-date')).toBe('not-a-date')
  })
})
