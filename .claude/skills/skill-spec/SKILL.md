---
name: skill-spec
description: Write a feature specification for the ထောပြီ app from a feature request or idea, saved to docs/specs/. Use when asked to spec, design, or plan a new feature before implementation.
---

# Write a feature spec

Turn the feature request in the arguments (or ask for one if missing) into a
written spec at `docs/specs/<kebab-case-feature-name>.md`. The spec is the
contract that `/skill-implement` will later build from, so it must be
implementable without further questions.

## Before writing

1. Read `README.md` and skim `docs/phase-1-review.md` for current
   product decisions (module names, roadmap, design decisions).
2. Check `docs/specs/` for an existing spec covering the same feature —
   update it instead of duplicating.
3. Ground every data claim in the real GLO API contract documented in
   `docs/redesign-prompt.md` (endpoints, response shapes). Never invent
   fields — if unsure, verify with a `curl` against the live API first.

## Spec structure

```markdown
# <Feature name>

**Status:** draft | approved | implemented
**Module(s):** Home | History | Draw Details | How It Works | new

## Purpose
One paragraph: the user problem and why this feature solves it.

## User stories
As a <user>, I want <capability> so that <outcome>. (2-5 stories)

## Functional requirements
Numbered, testable statements (FR1, FR2, ...). Each must be verifiable.

## Data & API
Which GLO endpoints / composables / localStorage keys are involved; new
normalizer fields if any.

## UI behavior
Per screen: layout placement, states (loading / error / empty / success),
mobile behavior. Reference existing components (DigitTiles, NumberChip,
PrizeCard, DrawCard, TicketChecker, MyTickets, AppIcon) and the design
tokens in App.vue (--red, --gold, --teal, --cream, --amber accent).

## Acceptance criteria
Checklist the implementation must pass, including: lint clean, jest tests
for new pure logic, production build succeeds, verified in a real browser.

## Out of scope
What this spec deliberately excludes.
```

## Rules

- Keep it under ~120 lines; a spec nobody reads is worthless.
- English UI copy for now (Burmese localization is a separate roadmap item).
- Respect standing design decisions: amber is the only interactive accent,
  red is brand/win, no emoji icons, 44px minimum hit targets.
- Finish by listing the spec path and a 3-line summary in chat.
