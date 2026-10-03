# Cursed Knight — Agent Rules

## Existing Code Is the Source of Truth
Before writing or modifying code, inspect nearby scripts and follow the project's existing naming, formatting, architecture, and patterns. Do not modernize working code or reformat unrelated code. New code should look like it belongs in this repository.

## Coding Style
- Keep code simple, explicit, and readable.
- Match existing formatting.
- Keep method calls on one line when reasonably readable.
- Use `[SerializeField] private` fields for Inspector references.
- Use private fields for internal runtime state.
- Existing `GetSomething()` methods are valid; do not replace them with properties just for style.
- Use PascalCase for classes and methods.
- Follow surrounding field/local naming conventions.
- Do not rename existing members without a functional reason.
- Avoid unnecessary abstractions, interfaces, wrappers, factories, services, or dependency injection.
- Do not create architecture for hypothetical future requirements.
- Do not extract trivial one-line wrapper methods.
- Use early returns where they improve readability.
- Add null guards where failure is realistically possible, not everywhere.
- Comments should explain WHY, not narrate obvious code.
- Do not add unnecessary XML documentation.

## Architecture
- ScriptableObjects contain immutable/static definitions.
- Runtime objects contain mutable gameplay/run state.
- Managers own gameplay rules and state transitions.
- Display/UI classes own presentation only.
- UI must not own gameplay state.
- Avoid duplicate ownership.

Current ownership examples:
- `DeckManager` owns the persistent player deck.
- `RelicManager` owns the player's Relic collection.
- `RunManager` owns run progression.
- `SaveManager` coordinates persistence but does not own gameplay state.
- `Map` owns the authored map graph and MapNode lookup.
- `Player` owns player runtime state.
- `CardViewDisplay` owns card presentation, not card state.

## Communication
Prefer direct method calls when there is a clear owner/callee relationship. Use events when a system needs to announce that something happened without knowing its listeners. Do not use events for everything.

Whenever adding a method, establish:
1. Who calls it?
2. When is it called?
3. Why does this class own it?

Do not add orphan methods with no integration path.

## Unity Lifecycle
Use `Awake`, `Start`, `OnEnable`, and `OnDisable` intentionally. Do not move initialization between lifecycle methods without a concrete reason. Always consider whether the GameObject is active when lifecycle initialization is required.

`Map` must remain on an always-active GameObject because persistence requires its node lookup to initialize even while the Map UI is hidden.

## Refactoring
Do not perform unrelated refactors while implementing a feature. If existing code works and does not block the current requirement, leave it alone. If existing architecture genuinely blocks the requested feature, explain why before making a substantial structural change.

Prefer the smallest complete change that satisfies the current requirement.

## Reuse Existing Systems
Before creating a new manager, abstraction, database, runtime type, or UI system, inspect whether an existing system already owns that responsibility. Do not duplicate existing systems.

In particular:
- Do not introduce a `PersistentCard` type. `RuntimeCard` is the persistent per-card run instance.
- Do not redesign the completed persistence architecture unless an actual requirement or bug demands it.

## Packages and Project Configuration
Ask before:
- installing/removing Unity packages
- modifying package versions
- changing major Project Settings
- changing build configuration
- introducing third-party dependencies

Do not modify `Packages/manifest.json` or package dependencies without approval.

## Changes
When explicitly asked to implement something:
1. Inspect the relevant existing code first.
2. Reuse existing systems.
3. Match local coding style.
4. Make the smallest complete change.
5. Include required caller/callee wiring.
6. Avoid unrelated modifications.
7. Check for compile errors.
8. Report which files changed and why.
9. Explain how the change should be tested.

## Verification
Never claim something is tested merely because it was implemented.

Clearly distinguish:
- Implemented
- Compiles
- Tested in Editor
- Playtested
- Fully verified

If you cannot perform a level of testing, say so.

## Scope
Cursed Knight is a small solo indie game approaching release.

Do not introduce scope creep.

Current roadmap:
- Milestone 7 — Presentation
- Milestone 8 — Release Prep
- Cursed Knight 1.0

Do not add unrelated gameplay systems or speculative features.
