/**
 * Digit-pattern matching for the lucky-number picker.
 * A pattern is 6 slots; empty string / null = wildcard.
 * matchesDigitPattern('417212', ['4','','7','','','2']) → true
 */
export function matchesDigitPattern(number, pattern) {
  if (typeof number !== 'string' || number.length !== 6) return false
  if (!Array.isArray(pattern) || pattern.length !== 6) return false
  return pattern.every((slot, i) => !slot || number[i] === slot)
}

/** Any digit appearing twice in a row (e.g. 411212 → true via "11"). */
export function hasDouble(number) {
  return /(\d)\1/.test(number)
}

/** Any digit appearing three times in a row. */
export function hasTriple(number) {
  return /(\d)\1\1/.test(number)
}

export function endsWith(number, suffix) {
  return typeof number === 'string' && number.endsWith(suffix)
}
