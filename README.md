# Cursed Knight

A dark fantasy deck-building roguelike built solo in **Unity and C#**.

Build your deck, collect Relics, and navigate branching encounters while balancing limited Energy against the power—and consequences—of Corruption.

**Current version:** v0.7.0 · **Milestone 7 — Presentation complete**  
**Currently working on:** Milestone 8 — Release Prep

## Gameplay

- **Strategic card combat:** Attack, Defense, Utility, and Power cards with targeting, status effects, and resource management.
- **Corruption risk and reward:** Powerful cards generate persistent Corruption, triggering Curse Surge and obscuring enemy intent values.
- **Persistent build progression:** Card rewards, removal, individual card upgrades, and 15 Relics distributed across three Acts.
- **Branching encounters:** Battles, Elites, Rest Sites, and Bosses connected through a node-based map.
- **Varied enemy behavior:** Fixed-pattern and weighted-random AI, summoning, support actions, counterattacks, and multi-phase Bosses.
- **Checkpoint-based continuation:** Save between encounters and resume with your deck, upgrades, Relics, player state, and map progression intact.

## Engineering Highlights

Designed and implemented the gameplay systems, persistence, UI, and presentation as a solo developer.

| System | Implementation |
| --- | --- |
| **Card architecture** | ScriptableObjects define card data; persistent runtime instances track mutable state and independent upgrades for each card copy. |
| **Combat sequencing** | Coroutine-based action resolution coordinates enemy animations, damage timing, status triggers, death handling, and input restrictions. |
| **Enemy AI** | Fixed-pattern and weighted-random action selection with validation, dynamic intents, and phase-based action replacement. |
| **Run persistence** | JSON serialization reconstructs runtime state using stable Card, Relic, and Map Node IDs. Temporary combat state remains separate from saved progression. |
| **Reusable UI** | Shared card viewers support deck inspection, combat piles, removal, upgrades, confirmation, and side-by-side previews. |
| **Player preferences** | Independent settings persistence covers audio, graphics, Reduce Motion, and optional End Turn confirmation. |

## Latest Update — v0.7.0

Milestone 7 completes the presentation layer:

- Card and UI sound effects, turn cues, battle music, Boss music, and Victory music.
- Sequential enemy actions with coordinated impact timing and death fades.
- Animated Card reward reveals, Map selection feedback, and screen fades.
- Separate Main Menu, Loading, and Gameplay scenes with New Run and Continue flows.
- Pause menu with Resume, Settings, Give Up confirmation, and checkpoint-based Save and Quit.
- Audio, Graphics, Controls, and Accessibility tabs with persistent preferences.

The Controls screen documents planned keyboard shortcuts; implementation is scheduled for Milestone 8.

## Development Roadmap

| Version | Milestone | Status |
| --- | --- | --- |
| v0.1.0 | Playable Prototype | Complete |
| v0.2.0 | Combat Foundation & Enemy Systems | Complete |
| v0.3.0 | Combat Polish | Complete |
| v0.4.0 | Run Progression | Complete |
| v0.5.0 | Relics & Card Upgrades | Complete |
| v0.6.0 | Run Data & Persistence | Complete |
| **v0.7.0** | **Presentation** | **Complete** |
| v1.0.0 | Release Prep | In progress |

Release preparation focuses on gameplay balance, final artwork, keyboard shortcuts, external playtesting, save/load regression testing, display testing, and Windows build validation.

## Technology

**Unity 6.6 · C# · uGUI / TextMeshPro · ScriptableObjects · Coroutines · JSON Serialization · Git / GitHub**

## Project Status

Cursed Knight is in active development toward its first release. The core run loop and presentation systems are implemented; final artwork, balance, and release validation are ongoing.

Run saves use between-encounter checkpoints. Leaving during combat resumes from the last completed checkpoint rather than restoring an unfinished Battle.
