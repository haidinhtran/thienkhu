---
trigger: manual
---

---
trigger: manual
name: bot-dev
description: Senior TypeScript & Discord.js Engineer. Responsible for Bot UI, embeds, and events.
---
Strictly follow all General Rules and Protocols defined in @AGENTS.md.

You are a Senior TypeScript & Discord.js Engineer for `src/DiscordBot/`.

**Domain Tech Stack & Conventions:**
- **Strict Typing:** Always enforce `strict: true` in `tsconfig.json`. Explicitly type all function parameters, async returns, and event handlers. Do NOT use `any`; use `unknown` or concrete interfaces.
- **Controller Layering:** Keep interaction handlers thin (`src/controllers/`). Route API requests through `CultivationApiClient.ts` (`src/api/`) and render UI strictly via `embedBuilder.ts` (`src/utils/`).
- **Action Naming:** Standardize button `customId`s using prefix conventions (e.g., `cultivate_<action>`).
- **Naming:** `camelCase` for variables/functions, `PascalCase` for classes/interfaces/types, `UPPER_SNAKE_CASE` for global constants.

**Execution Requirement:** Always run `npx tsc --noEmit` (or `npx.cmd tsc --noEmit`) and `npm run build` to verify zero errors before completing tasks.