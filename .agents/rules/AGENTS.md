---
trigger: always_on
---

---
trigger: always_on
---

# AGENTS.md - General AI Operating Protocol & Global Rules

This document is the mandatory instruction manual for all AI Coding Agents. All Agents must adhere strictly to these execution protocols and verification steps.

---

## 1. General AI Operating Protocol (Every Task)

AI Agents must follow a strict lifecycle for every task:
Step 1: Context & Search -> Step 2: Planning & Approval -> Step 3: Incremental Execution -> Step 4: Self-Review & Verification -> Step 5: Update Documentation Maintenance

### Phase 1: Context & Search Gathering
- Always respond to user in Vietnamese but use English in documentation and implementation.
- Read existing design, workflow, and roadmap documents in `docs/`:
  - [docs/developer_guide.md](docs/developer_guide.md) (Developer Playbook, CLI commands, and Workflows)
  - [docs/implementation_plan.md](docs/implementation_plan.md) (Project Roadmap & Feature tracking)
  - [docs/game_design_document.md](docs/game_design_document.md)
  - [docs/user_flow.md](docs/user_flow.md)
  - [docs/system_architecture.md](docs/system_architecture.md)
  - [docs/database_schema.md](docs/database_schema.md)
- Locate and understand existing code interfaces, models, and execution flows before generating new files.
- **No Assumptions & Clarification Rule:** NEVER make arbitrary assumptions when requirements or technical specs are incomplete. If information is insufficient, ask the user for clarification before implementation.

### Phase 2: Planning & Approval
- Formulate a clear, step-by-step implementation plan. Refer to `docs/implementation_plan.md` for context.
- Highlight potential breaking changes, API contract updates, or database schema migrations.

### Phase 3: Incremental Execution
- Follow the exact development workflows defined in `docs/developer_guide.md` (Section 3).
- Implement changes in small, testable, and modular commits/units.
- Never refactor unrelated files or reformat unchanged codebases unless explicitly instructed.

### Phase 4: Self-Review & Verification
- Run build/compilation checks locally using commands in `docs/developer_guide.md`.
- Verify no syntax, typing, nullability, or linting errors were introduced.
- Inspect edge cases, concurrency risks, resource timeouts, and error handling paths.

### Phase 5: Automated Documentation Maintenance (MANDATORY)
- **Rule:** Code and documentation MUST stay 100% synchronized at all times.
- If any DB entity, API contract, state machine, command flow, or roadmap item is added/modified, immediately update the corresponding Markdown file inside `docs/`.

---

## 2. Security, Resilience & Logging Rules

### A. Information Leakage & Public Response Masking
- Never expose raw exceptions, stack traces, DB queries, or internal IP addresses to Discord Embeds or API clients.
- Always return user-friendly, ephemeral error messages containing a unique CorrelationId / TraceId.

### B. Resilience & Crash Prevention
- All HTTP API endpoints must be guarded by a Global Exception Handling Middleware returning RFC 7807 ProblemDetails.
- Discord interaction handlers must be wrapped in top-level try/catch blocks to prevent unhandled rejections.
- Handle Discord's 3-second limit using dynamic deferReply calls for async operations.

### C. Structured Logging & Sensitive Data Sanitization
- Log detailed error stack traces strictly to internal server log sinks (Serilog / Winston).
- Automatically sanitize and mask sensitive data (Tokens, DB Strings, JWTs, API Keys) before logging.

---

## 3. Definition of Done (DoD) Checklist

An AI Agent must consider a task complete ONLY when:
- [ ] Code compiles with zero errors (`dotnet build` and `npx tsc --noEmit`).
- [ ] Code adheres strictly to the project architecture (`src/CoreAPI/` vs `src/DiscordBot/`).
- [ ] Exception paths, null checks, and edge cases are handled gracefully.
- [ ] Public outputs are sanitized and detailed logs include trace IDs.
- [ ] Sensitive data (tokens, secrets) are masked from log outputs.
- [ ] Corresponding Markdown documents inside `docs/` have been updated if code/schema/API/roadmap changed.
- [ ] A concise summary of changes, build verification results, and updated documentation files is provided to the human reviewer.