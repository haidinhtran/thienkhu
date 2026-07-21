# User Flow Document - Cultivation RPG Bot (MVP)

This document details the user interaction flow for the single-command (`/tutien`) execution model. All actions branch dynamically via Discord Embed interfaces and Interactive Buttons.

## Single-Command Interactive Flowchart

```mermaid
flowchart TD
    classDef start finish fill:#2d3436,stroke:#dfe6e9,stroke-width:2px,color:#fff;
    classDef action fill:#0984e3,stroke:#74b9ff,stroke-width:2px,color:#fff;
    classDef condition fill:#fdcb6e,stroke:#ffeaa7,stroke-width:2px,color:#2d3436;
    classDef ai fill:#6c5ce7,stroke:#a29bfe,stroke-width:2px,color:#fff;

    A([User types /cultivate]):::start --> B{Character Exists?}:::condition

    %% Account Registration
    B -- No --> C[Create Character Profile]:::action
    C --> D[Assign Base Stats & Numeric Level 1]:::action
    D --> E

    %% Main Menu Navigation
    B -- Yes --> E[Display Main Menu Embed]:::action
    E --> F([Button: Profile]):::action
    E --> G([Button: Exploration]):::action
    E --> H([Button: Secret Domain]):::action

    %% Profile Inspection
    F --> F1[Display Level, Realm Name, Stats & Inventory Embed]:::action

    %% Exploration Activity
    G --> G1[Select Location]:::action
    G1 --> G2{Trigger Random Event}:::condition

    G2 -- Story Event --> G3[AI Generates Dynamic Narrative]:::ai
    G3 --> G4[User Selects Choice]:::action
    G4 --> G5{Luck / Attribute Check}:::condition
    G5 -- Success --> G6[Grant Rewards: Qi / Items / Spirit Stones]:::action
    G5 -- Failure --> G7[Apply Penalty: Stat Loss / Injury]:::action

    G2 -- Combat Event --> G8[Initiate PvE Battle]:::action
    G8 --> G9{Battle Outcome}:::condition
    G9 -- Victory --> G6
    G9 -- Defeat --> G7

    %% Secret Domain Activity
    H --> H1[Select Domain Challenge]:::action
    H1 --> H2[Execute Combat Encounters]:::action
    H2 --> H3[Grant Domain Loot: Breakthrough Materials & Gear]:::action
```
