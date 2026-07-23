# Cultivation RPG Bot (MVP) - Implementation Plan

Based on the core documentation (`game_design_document.md`, `system_architecture.md`, `user_flow.md`, `database_schema.md`), this document outlines the phased implementation plan for the MVP.

## Current State Summary
- **Backend (.NET Core 10):** Core entities created (`Character`, `DiscordUser`, `Inventory`, `ServerConfig`, `AuditLog`). A basic `CharactersController` with a `/characters/profile` endpoint exists.
- **Frontend (Discord Bot - Node.js 24):** Command handler and event handler bootstrapped. A placeholder `/cultivate` command exists.

---

## [x] Phase 1: Foundation & The Interactive Menu
**Goal:** Connect the Discord Bot to the Backend API and establish the core UI loop.

1. **Discord Bot - API Client:** Implement a secure HTTP client service in the bot to communicate with the .NET Backend API. (Using native Node.js `fetch`).
2. **Discord Bot - `/cultivate` Main Menu:** Update the `/cultivate` command to call the backend `/characters/profile` endpoint, ensuring the user's character exists. Render a **Main Menu Embed** containing three interactive buttons: `[Profile]`, `[Exploration]`, `[Secret Domain]`.
3. **Discord Bot - Interaction Handler:** Implement `interactionCreate` handler specifically for button clicks.
4. **Discord Bot - Profile View:** Handle the `Profile` button click. Fetch the latest `CharacterProfileDto` from the API and display a detailed Profile Embed showing Level, Stats, Realm, and Resources.

## [x] Phase 2: The Core Loop - Passive Chat-to-Earn
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

## [x] Phase 3: Active Grind - Exploration (Mocked Narrative)
**Goal:** Implement the exploration loop. (Note: OpenAI integration is deferred. Responses will be mocked/static for now).

1. **Backend - Exploration Engine:** Create the core logic for Exploration. 
   - Roll a random event (Story vs Combat).
   - If Story: Return a mocked narrative text instead of calling an LLM.
   - If Combat: Execute basic PvE combat calculation.
2. **Backend - Exploration API:** Create endpoints for initiating exploration and submitting choices (e.g., `POST /api/v1/activities/explore`).
3. **Discord Bot - Exploration UI:** Handle the `Exploration` button from the Main Menu. Provide users with location choices, display the mocked narrative in an Embed, and attach buttons for their reaction choices.

## [x] Phase 4: Progression & Secret Domains
**Goal:** Enable players to ascend realms and farm specific materials.

### Architecture & Data Models
- **`IGameConfigProvider` & `InMemoryGameConfigProvider`**: Implemented an interface-driven approach for game configurations. All requirements for levels, domains, boss stats, and rewards are defined centrally and can be easily swapped for a Database-backed provider in the future.
- **DTOs**: Added `AscendRequestDto`, `AscendResultDto`, `SecretDomainRequestDto`, and `SecretDomainResultDto` for robust data transfer.

### 1. Backend - Inventory Management & Security
- **`InventoryService`**: Created secure methods to manage the character's JSONB inventory list dynamically while letting Entity Framework Core track the changes.
- **Security**: Utilized EF Core Optimistic Concurrency (`[Timestamp] Version`) on `Character` and `Inventory` entities to prevent race conditions and double-spending vulnerabilities during rapid button clicks.

### 2. Backend - Ascension Logic (`POST /api/v1/characters/ascend`)
- Validates if the character meets the Qi and Item requirements for their next level.
- Safely deducts required items (e.g., Breakthrough Pills) and Qi.
- Dynamically calculates and applies new Base Stats (Health, Strength, Agility, Luck, Mana) based on the `GameConfigProvider`.

### 3. Backend - Secret Domains (`POST /api/v1/activities/secret-domain`)
- Simulates PvE combat using player attributes vs. boss attributes with an RNG variance.
- Grants Spirit Stones and handles variable drop rates for specific items (e.g., 50% chance for a breakthrough pill).
- Records an `AuditLog` entry for domain victories.

### 4. Discord Bot - Domain & Ascension UI
- **Ascension Flow**: Integrated an `[Ascend (Breakthrough)]` button directly into the `[Profile]` Embed. Executing it safely calls the backend and displays a dynamic success/failure modal with the new stats.
- **Secret Domain UI**: Added a `[Secret Domain]` button to the main menu `(/tutien)` which opens a selection menu of available domains (e.g., Goblin Cave, Azure Cloud Mountain).
- **Embed Builders**: Updated `embedBuilder.ts` to output formatted embeds for Domain Combat Logs and Ascension outcomes.

## [x] Phase 5: System Polish, Dynamic Configuration & Core Loop Integration
**Goal:** Transition the MVP into a production-ready state with deeper mechanics, proper onboarding, and scalable configurations.

## Execution Status
> [!NOTE]
> **Phase 5 Implementation Complete.**
> All backend API additions, configuration strategies, documentation updates, and Bot presentation layers have been implemented successfully.

### Architecture Notes (Premium & Scale)
- **Override/Fallback Pattern:** To support Premium servers having custom configs later, the `IGameConfigProvider` interface methods will accept `ServerId`. For this MVP, it will ignore `ServerId` and return the Global JSON config. In the future, it will check the Database first for a Premium override, then fallback to JSON.
- **Ecosystem CMS:** As Items, Skills, and Gears multiply, a single JSON will become unmanageable. The interface-driven design (`IGameConfigProvider`) ensures that when we eventually migrate the catalog to a Database with an Admin Web Dashboard (CMS), the core game logic will remain entirely untouched.

### 1. Game Configuration (JSON File Strategy)
- `[x]` **Backend:** Remove `InMemoryGameConfigProvider`.
- `[x]` **Backend:** Create `game_data.json` in `CultivationApi.WebApi` containing level thresholds, stats, and domains.
- `[x]` **Backend:** Refactor `IGameConfigProvider` signatures to accept `ServerId` (for future-proofing).
- `[x]` **Backend:** Implement `JsonGameConfigProvider` to load configuration from `game_data.json` at startup.

### 2. Character Onboarding (/cultivate First Time)
- `[x]` **Backend:** Modify `CharactersController.cs` and `CharacterService.cs` to stop silent auto-creation on GET `/profile`. Return 404/null.
- `[x]` **Backend:** Add POST `/create` endpoint for creating a default "Mortal" character. (Affinity gacha is deferred to late-game items).
- `[x]` **Bot:** Update `cultivateController.ts` `/cultivate` flow: Check if character exists. If not, send `buildWelcomeEmbed` with `[Begin Journey]` button.
- `[x]` **Bot:** Handle `[Begin Journey]` button to call `createCharacter` API.

### 3. Ascension Progress & Clarity UI
- `[x]` **Backend:** Update `CharacterProfileDto` to include `TargetQi` (required Qi for next level) and `RequiredBreakthroughItems`.
- `[x]` **Bot:** Update `embedBuilder.ts` (`buildProfileEmbed`) to display a visual Qi progress bar (e.g., `[▓▓▓▓░░░░░░] 45%`) using `TargetQi`.
- `[x]` **Bot:** List any required breakthrough materials in the Profile embed.

### 4. Ecosystem Synergy (Inventory & Items)
- `[x]` **Backend:** Ensure `ActivitiesService.cs` (Secret Domain) rewards are correctly inserted into the `INVENTORY` JSONB column.
- `[x]` **Backend:** Add endpoints to view inventory and equip items: `GET /api/v1/characters/inventory`, `POST /api/v1/characters/equip`.
- `[x]` **Backend:** Update combat logic in Secret Domain to use effective stats: `BaseStats + EquippedGearStats`.
- `[x]` **Bot:** Add `[Inventory]` button to the main menu.
- `[x]` **Bot:** Handle inventory display and equipment selection UI.

### 5. Server Owner DM Onboarding Guide
- `[x]` **Bot:** Create `src/events/guildCreate.ts` to listen for the bot joining a server.
- `[x]` **Bot:** Fetch the `guild.ownerId` and send a static DM Welcome Guide.
- `[x]` **Bot:** The guide will explain Chat-to-earn and note that custom configurations will be available in future updates.

### 6. Documentation Updates
- `[x]` **Docs:** Update `user_flow.md` to reflect the new Onboarding flow and Inventory.
- `[x]` **Docs:** Update `system_architecture.md` to document the JSON GameConfig provider and new API endpoints.
- `[x]` **Docs:** Update `game_design_document.md` to explicitly list all game content and data (Realms, Domains, Items, Story mocks) to serve as the absolute single source of truth.

---

## Phase 6: Dynamic QiPerMessage & Insight (Ngộ tính) Stat System

**Goal:** Transform fixed `QiPerMessage` into a dynamic random value influenced by player **Insight (Ngộ tính)** stat while preventing Power Creep via configurable `ServerConfig` limits.

### 1. Domain & Entities
- `[x]` **Backend:** Add `Insight` (default 10) to `BaseStats` class on `Character.cs` and `BaseStatsConfig` on `GameConfigs.cs`.
- `[x]` **Backend:** Update `ServerConfig.cs` to remove old `QiPerMessage` and add `MinQiPerMessage` (default 10), `MaxQiPerMessage` (default 100), `InsightMultiplier` (default 1.0).

### 2. Application & Progression Logic
- `[x]` **Backend:** Refactor `GainQiFromMessageAsync` in `CharacterService.cs` to calculate dynamic Qi using the clean formula:
  $$\text{UpperLimit} = \text{MinQiPerMessage} + \lfloor \text{Insight} \times \text{InsightMultiplier} \rfloor$$
  $$\text{MaxAllowed} = \min(\text{UpperLimit}, \text{MaxQiPerMessage})$$
  $$\text{QiEarned} = \text{Random}(\text{MinQiPerMessage}, \text{MaxAllowed})$$
  $$\text{QiEarned} = \min(\text{QiEarned}, \text{DailyQiLimit} - \text{DailyQiAccumulated})$$
- `[x]` **Backend:** Update `game_data.json` to assign scaling `Insight` stat values per Realm Level (Level 1: 10, Level 2: 12, etc.).
- `[x]` **Backend:** Update `AscendRealmAsync` and character creation in `CharacterService.cs` to handle `Insight` stat properly.

### 3. Database Migration
- `[x]` **Backend:** Generate EF Core Migration `AddInsightAndServerConfigQiFormula` (removes `QiPerMessage` column, adds `MinQiPerMessage`, `MaxQiPerMessage`, `InsightMultiplier`).

### 4. Presentation & Documentation
- `[x]` **Discord Bot:** Keep Qi gain notifications subtle/standard on Discord UI while logging detailed Qi calculation in server logs (hidden mechanic for player discovery).
- `[x]` **Docs:** Update `database_schema.md`, `game_design_document.md`, and `implementation_plan.md`.

