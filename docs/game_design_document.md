# Game Design Document (GDD) - Cultivation RPG Bot (MVP)

This document outlines the core loop and mechanics for the Minimum Viable Product (MVP) phase of the text-based RPG cultivation game.

## 1. Core Vision & Progression

- **Theme:** Eastern Fantasy (Xianxia/Wuxia) Cultivation.
- **Progression System:** Internally, the system uses numeric levels to ensure easy mathematical scaling and management. Externally, server administrators can customize the names of the cultivation realms to fit their community context.
- **Interaction:** A streamlined UI/UX using a single primary command `/tutien`. All subsequent actions are performed via Discord interactive buttons attached to Embed messages.

## 2. The Core Loop

The primary gameplay loop consists of gathering resources and spending them to ascend:

1. **Passive Grind (Chat-to-Earn):** Players earn Qi (Experience Points) passively by chatting on the Discord server. Experience is granted based on configurable server settings, including specific allowed channels (with fallback logic), dynamic Qi gains per message, message cooldowns (e.g., every 60 seconds), and a daily Qi accumulation cap.
2. **Active Grind (Activities):** Players use the `/tutien` menu to access activities like Exploration and Secret Domains to earn Spirit Stones, gear, and breakthrough materials.
3. **Ascension:** Players consume accrued Qi and specific items to break through to the next level or realm.

## 3. Core Activities

- **Exploration:** Players select a specific location (e.g., Novice Village, Azure Cloud Mountain). Each location has specific level requirements.
  - _Event 1 (Story):_ AI-generated "Choice Matters" events influenced by the player's Luck stat. The AI handles narrative variety to prevent repetitive outcomes, providing rewards or penalties based on the player's decisions.
  - _Event 2 (Combat):_ PvE encounters against NPCs, monsters, or elite entities.
- **Secret Domains:** Challenging instances that players enter to farm specific resources, including skills, equipment, Spirit Stones, and pills required for advancement.
