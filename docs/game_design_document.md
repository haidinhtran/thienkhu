# Game Design Document (GDD) - Cultivation RPG Bot (MVP)

This document outlines the core loop and mechanics for the Minimum Viable Product (MVP) phase of the text-based RPG cultivation game.

## 1. Core Vision & Progression

- **Theme:** Eastern Fantasy (Xianxia/Wuxia) Cultivation.
- **Progression System:** Internally, the system uses numeric levels to ensure easy mathematical scaling and management. Externally, server administrators can customize the names of the cultivation realms to fit their community context.
- **Interaction:** A streamlined UI/UX using a single primary command `/tutien`. All subsequent actions are performed via Discord interactive buttons attached to Embed messages.

## 2. The Core Loop

The primary gameplay loop consists of gathering resources and spending them to ascend:

1. **Passive Grind (Chat-to-Earn):** Players earn Qi (Experience Points) passively by chatting on the Discord server. Experience is granted dynamically based on the player's **Insight (Ngộ tính)** attribute and configurable server settings (`MinQiPerMessage`, `MaxQiPerMessage`, `InsightMultiplier`), guarded by message cooldowns (e.g., every 60 seconds) and a daily Qi accumulation cap. Gain details are kept subtle/hidden to encourage player discovery and stat progression strategy.
   - **Qi Calculation Formula:**
     $$\text{UpperLimit} = \text{MinQiPerMessage} + \lfloor \text{Insight} \times \text{InsightMultiplier} \rfloor$$
     $$\text{MaxAllowed} = \min(\text{UpperLimit}, \text{MaxQiPerMessage})$$
     $$\text{QiEarned} = \text{Random}(\text{MinQiPerMessage}, \text{MaxAllowed})$$
     $$\text{ActualQiEarned} = \min(\text{QiEarned}, \text{DailyQiLimit} - \text{DailyQiAccumulated})$$
2. **Active Grind (Activities):** Players use the `/tutien` menu to access activities like Exploration and Secret Domains to earn Spirit Stones, gear, and breakthrough materials.
3. **Ascension:** Players consume accrued Qi and specific items to break through to the next level or realm.

## 3. Core Activities

- **Exploration:** Players select a specific location (e.g., Novice Village, Azure Cloud Mountain). Each location has specific level requirements.
  - _Event 1 (Story):_ AI-generated "Choice Matters" events influenced by the player's Luck stat. The AI handles narrative variety to prevent repetitive outcomes, providing rewards or penalties based on the player's decisions.
  - _Event 2 (Combat):_ PvE encounters against NPCs, monsters, or elite entities.
- **Secret Domains:** Challenging instances that players enter to farm specific resources, including skills, equipment, Spirit Stones, and pills required for advancement.

## 4. Hardcoded Data (MVP Phase 5)

All game configuration is currently managed via `game_data.json` inside the `.NET API`.

**Realms:**
- Level 1-3: Qi Condensation
- Level 4-6: Foundation Establishment
- Level 7-9: Golden Core
- Level 10: Nascent Soul

**Secret Domains:**
1. **Goblin Cave:** Required Level 1. Drops `breakthrough_pill_1`.
2. **Azure Cloud Mountain:** Required Level 4. Drops `breakthrough_pill_1` and `spirit_herb`.

**Items:**
- `breakthrough_pill_1` (Consumable)
- `breakthrough_pill_2` (Consumable)
- `breakthrough_pill_3` (Consumable)
- `spirit_herb` (Material)
