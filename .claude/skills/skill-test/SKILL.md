---
name: skill-test
description: Write and run tests for the ထောပြီ app — Jest unit tests for logic plus headless-browser verification for UI — and report results honestly. Use when asked to test, add tests, or verify a feature.
---

# Test the app

Test the target named in the arguments (a feature, module, or "everything").
Two layers: Jest for logic, headless Chrome for rendered behavior.

## Jest unit tests (logic)

- Location: `thai-lottery/tests/unit/*.spec.js`; run with `npm test`
  (config in `jest.config.js`, `@/` maps to `src/`, jest env is scoped to
  test files in `package.json` eslintConfig overrides).
- Follow the existing pattern in `lotteryApi.spec.js`: test pure functions
  against the **real GLO fixture** (`tests/unit/fixtures/glo-latest.json`),
  not hand-invented shapes. If a new endpoint's shape is needed, capture a
  real response with curl into a new fixture first.
- What must be unit-tested: everything in `src/services/` and any pure
  logic in composables (ticket matching, normalizing, grouping, formatting,
  validation). Components are verified in the browser instead — do not
  bolt on a component-testing framework without being asked.
- Cover the unhappy paths: malformed input, missing API fields, empty
  lists, storage unavailable.

## Browser verification (UI)

1. Dev server on http://localhost:8080 (start if needed).
2. Screenshot each affected route with headless Chrome:
   `"/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
   --headless=new --disable-gpu --user-data-dir=<fresh tmp dir>
   --window-size=1440,1200 --screenshot=<out.png>
   --virtual-time-budget=15000 <url>` — background it, sleep ~16s, then
   `pkill -f headless=new`.
3. READ every screenshot and compare against the spec/acceptance criteria.
   Known harness quirk: Chrome clamps windows to 500px minimum width, so
   true phone-width rendering can't be captured — check mobile CSS by
   reading it, and say so in the report.
4. Check deep links and redirects return 200
   (`curl -s -o /dev/null -w '%{http_code}'`).

## Report

- State plainly what passed and what failed, with the failing output —
  never soften a red result.
- A found bug is a finding to report (file:line + failure scenario), and
  only fix it when the user asked for fixes.
- Finish with the full gate: `npm run lint && npm test && npm run build`.
