import { endsWith, hasDouble, hasTriple, matchesDigitPattern } from '@htawpyi/shared-ui/utils/tickets'
import { nextDrawDateFrom } from '@/services/lotteryApi'

describe('matchesDigitPattern (lucky-number picker)', () => {
  const blank = ['', '', '', '', '', '']

  it('matches everything with an all-wildcard pattern', () => {
    expect(matchesDigitPattern('417212', blank)).toBe(true)
  })

  it('matches fixed digits at their positions', () => {
    expect(matchesDigitPattern('417212', ['4', '', '7', '', '', '2'])).toBe(true)
    expect(matchesDigitPattern('417212', ['4', '', '8', '', '', '2'])).toBe(false)
  })

  it('rejects malformed input', () => {
    expect(matchesDigitPattern('12345', blank)).toBe(false)
    expect(matchesDigitPattern('417212', ['4'])).toBe(false)
    expect(matchesDigitPattern(417212, blank)).toBe(false)
  })
})

describe('doubles / triples / suffix', () => {
  it('detects consecutive doubles', () => {
    expect(hasDouble('411212')).toBe(true)
    expect(hasDouble('123456')).toBe(false)
  })

  it('detects consecutive triples', () => {
    expect(hasTriple('888123')).toBe(true)
    expect(hasTriple('881238')).toBe(false)
  })

  it('matches suffixes', () => {
    expect(endsWith('417288', '88')).toBe(true)
    expect(endsWith('417288', '89')).toBe(false)
  })
})

describe('nextDrawDateFrom', () => {
  // The util returns LOCAL midnight, so compare in local time (en-CA
  // formats as yyyy-MM-dd), not via toISOString (UTC).
  const localIso = (d) => d.toLocaleDateString('en-CA')

  it('advances the 1st to the 16th of the same month', () => {
    expect(localIso(nextDrawDateFrom('2026-09-01'))).toBe('2026-09-16')
  })

  it('advances the 16th to the 1st of the next month', () => {
    expect(localIso(nextDrawDateFrom('2026-09-16'))).toBe('2026-10-01')
  })

  it('rolls over the year end', () => {
    expect(localIso(nextDrawDateFrom('2026-12-16'))).toBe('2027-01-01')
  })

  it('returns null for garbage', () => {
    expect(nextDrawDateFrom('not-a-date')).toBeNull()
  })
})
