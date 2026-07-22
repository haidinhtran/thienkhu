# Cultivation RPG Bot (MVP) - Implementation Plan

Based on the core documentation (`game_design_document.md`, `system_architecture.md`, `user_flow.md`, `database_schema.md`), this document outlines the phased implementation plan for the MVP.

## Current State Summary
- **Backend (.NET Core 10):** Core entities created (`Character`, `DiscordUser`, `Inventory`, `ServerConfig`, `AuditLog`). A basic `CharactersController` with a `/characters/profile` endpoint exists.
- **Frontend (Discord Bot - Node.js 24):** Command handler and event handler bootstrapped. A placeholder `/cultivate` command exists.

---

## Phase 1: Foundation & The Interactive Menu
**Goal:** Connect the Discord Bot to the Backend API and establish the core UI loop.

1. **Discord Bot - API Client:** Implement a secure HTTP client service in the bot to communicate with the .NET Backend API. (Using native Node.js `fetch`).
2. **Discord Bot - `/cultivate` Main Menu:** Update the `/cultivate` command to call the backend `/characters/profile` endpoint, ensuring the user's character exists. Render a **Main Menu Embed** containing three interactive buttons: `[Profile]`, `[Exploration]`, `[Secret Domain]`.
3. **Discord Bot - Interaction Handler:** Implement `interactionCreate` handler specifically for button clicks.
4. **Discord Bot - Profile View:** Handle the `Profile` button click. Fetch the latest `CharacterProfileDto` from the API and display a detailed Profile Embed showing Level, Stats, Realm, and Resources.

## Phase 2: The Core Loop - Passive Chat-to-Earn
**Goal:** Implement the primary mechanism for gaining Qi passively through Discord activity.

1. **Backend - Server Config Integration:** Ensure `ServerConfig` is properly utilized in the database to govern the `daily_qi_limit` per server.
2. **Backend - Gain Qi Endpoint:** Create a new REST endpoint (e.g., `POST /api/v1/characters/gain-qi`) and a corresponding DTO.
3. **Backend - Domain Logic:** Implement `AddPassiveQiAsync` in `CharacterService`. This must handle:
   - Cooldown tracking against `message_cooldown_seconds` (e.g., `last_meditated`).
   - Optimistic concurrency control (handling DB row versions).
   - Dynamic `qi_per_message` addition and Daily limit capping.
   - Recording an `AuditLog` entry for `EXP_GAIN`.
4. **Discord Bot - Message Listener:** Implement the `messageCreate` event handler in the bot.
   - Fetch allowed `chat_to_earn_channels` for the server.
   - Fallback to the first accessible text channel if none are configured. Disable feature if no fallback is found.
   - Check if message is in an allowed channel.
   - When valid, asynchronously ping the backend `gain-qi` endpoint without blocking the chat flow.

## Phase 3: Active Grind - Exploration (Mocked Narrative)
**Goal:** Implement the exploration loop. (Note: OpenAI integration is deferred. Responses will be mocked/static for now).

1. **Backend - Exploration Engine:** Create the core logic for Exploration. 
   - Roll a random event (Story vs Combat).
   - If Story: Return a mocked narrative text instead of calling an LLM.
   - If Combat: Execute basic PvE combat calculation.
2. **Backend - Exploration API:** Create endpoints for initiating exploration and submitting choices (e.g., `POST /api/v1/activities/explore`).
3. **Discord Bot - Exploration UI:** Handle the `Exploration` button from the Main Menu. Provide users with location choices, display the mocked narrative in an Embed, and attach buttons for their reaction choices.

## Phase 4: Progression & Secret Domains
**Goal:** Enable players to ascend realms and farm specific materials.

1. **Backend - Inventory Management:** Implement logic to parse and update the `JSONB` inventory structure.
2. **Backend - Ascension Logic:** Create an endpoint `POST /api/v1/characters/ascend`. Deduct required Qi, check for breakthrough materials in the inventory, update the `numeric_level`, and increase `base_stats`.
3. **Backend - Secret Domains:** Create fixed, challenging combat instances that grant specific loot (Breakthrough Pills, Spirit Stones, Gear).
4. **Discord Bot - Domain & Ascension UI:** Complete the button flows for `Secret Domain` and add an `Ascend` button to the Profile view when Qi is maxed out.
