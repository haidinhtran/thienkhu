# User Flow Document - Cultivation RPG Bot

This document details the user interaction flow for the single-command (`/cultivate`) execution model. All actions branch dynamically via Discord Embed interfaces and Interactive Buttons.

## In-Place Embed Updating & Navigation

To keep the Discord channel clean, the bot utilizes **In-Place Embed Updating**. The initial `/cultivate` command generates an Ephemeral message (only visible to the executing user). All subsequent button clicks and menu selections update this exact same message frame using `interaction.deferUpdate()`.

A `[🔙 Back to Main]` button is present on all sub-views to seamlessly return the user to the Main Menu without generating new messages.

### Note on State Locking & UX Caching
Because the bot uses in-place navigation, users might click `[🔙 Back to Main]` while in the middle of a state-locked action (e.g. an active Exploration event).
- **Backend Lock:** The backend intentionally locks the character in `IN_EXPLORATION` so they cannot exploit other game mechanics (like Secret Domains) while an event is pending.
- **Frontend Cache:** The bot mitigates soft-locks by caching active events in memory. If a user navigates away and clicks `[Exploration]` again, the bot immediately redisplays the pending event, honoring a "continue where we left off" seamless UX.

## Single-Command Interactive Flowchart

```mermaid
flowchart TD
    classDef start finish fill:#2d3436,stroke:#dfe6e9,stroke-width:2px,color:#fff;
    classDef action fill:#0984e3,stroke:#74b9ff,stroke-width:2px,color:#fff;
    classDef condition fill:#fdcb6e,stroke:#ffeaa7,stroke-width:2px,color:#2d3436;
    classDef ai fill:#6c5ce7,stroke:#a29bfe,stroke-width:2px,color:#fff;
    classDef back fill:#636e72,stroke:#b2bec3,stroke-width:2px,color:#fff;

    A([User types /cultivate (Ephemeral)]):::start --> B{Character Exists?}:::condition

    %% Account Registration
    B -- No --> C[Display Welcome Embed]:::action
    C --> C1([Button: Begin Journey]):::action
    C1 --> D[Create Default Character Profile]:::action
    D --> E

    %% Main Menu Navigation
    B -- Yes --> E[Display Main Menu Embed]:::action
    E --> F([Button: Profile]):::action
    E --> G([Button: Exploration]):::action
    E --> H([Button: Secret Domain]):::action
    E --> I([Button: Inventory]):::action

    %% Back Button Routing
    BackBtn([Button: 🔙 Back to Main]):::back --> E

    %% Profile Inspection & Ascension
    F --> F1[Display Profile & Base Stats Embed]:::action
    F1 --> F2([Button: Ascend]):::action
    F1 -.-> BackBtn
    F2 --> F3{Meets Qi & Item Reqs?}:::condition
    F3 -- Yes --> F4[Consume Items, Reset Qi, Increase Level & Stats]:::action
    F3 -- No --> F5[Display Failure Embed]:::action
    F4 -.-> BackBtn
    F5 -.-> BackBtn

    %% Exploration Activity
    G --> G1[Select Location Menu]:::action
    G1 -.-> BackBtn
    G1 --> G2{Trigger Random Event}:::condition

    G2 -- Story Event --> G3[AI Generates Dynamic Narrative]:::ai
    G3 --> G4[User Selects Choice]:::action
    G4 --> G5{Luck / Attribute Check}:::condition
    G5 -- Success --> G6[Grant Rewards: Qi / Items / Spirit Stones]:::action
    G5 -- Failure --> G7[Apply Penalty: Stat Loss / Injury]:::action
    G6 -.-> BackBtn
    G7 -.-> BackBtn

    G2 -- Combat Event --> G8[Initiate PvE Battle]:::action
    G8 --> G9{Battle Outcome}:::condition
    G9 -- Victory --> G6
    G9 -- Defeat --> G7

    %% Secret Domain Activity
    H --> H1[Select Domain Challenge Menu]:::action
    H1 -.-> BackBtn
    H1 --> H2[Execute Combat Encounters]:::action
    H2 --> H3[Display Combat Log & Loot]:::action
    H3 -.-> BackBtn
    
    %% Inventory View
    I --> I1[Display Inventory Embed]:::action
    I1 -.-> BackBtn
```
