# Database Schema Outline Document - Cultivation RPG Bot (MVP)

This document outlines the relational database schema designed for PostgreSQL. It leverages `JSONB` data types for flexible attributes and inventory management, keeping the database schema lean for the MVP while supporting dynamic server configuration.

## Entity-Relationship Diagram

```mermaid
erDiagram
    DISCORD_USER ||--o| CHARACTER : owns
    CHARACTER ||--o| INVENTORY : maintains
    CHARACTER }|--|| SERVER_CONFIG : belongs_to
    CHARACTER ||--o{ AUDIT_LOG : generates

    DISCORD_USER {
        string discord_id PK "Discord Snowflake ID"
        string username "Current Discord Username"
        timestamp created_at
    }

    CHARACTER {
        uuid id PK "Unique Character ID"
        string discord_id FK "References DISCORD_USER(discord_id)"
        string server_id FK "References SERVER_CONFIG(server_id)"
        string current_state "IDLE, IN_EXPLORATION, IN_COMBAT"
        int numeric_level "Internal absolute level"
        bigint current_qi "Current experience points"
        bigint daily_qi_accumulated "Reset daily to enforce Cap"
        int spirit_stones "Primary game currency"
        jsonb base_stats "JSONB: {strength, agility, luck, health, mana, insight}"
        timestamp last_meditated "Cooldown tracking"
        int version "Row version for Concurrency Control"
    }

    INVENTORY {
        uuid id PK
        uuid character_id FK
        jsonb equipped_gear "JSONB: {head, chest, weapon, artifact}"
        jsonb items "JSONB: [{item_id, quantity, item_type}]"
    }

    SERVER_CONFIG {
        string server_id PK "Discord Server Snowflake ID"
        jsonb realm_names "JSONB: Level to Title Mapping"
        int daily_qi_limit "Customizable daily cap for Chat-to-Earn"
        jsonb chat_to_earn_channels "JSONB: List of allowed channel IDs"
        int min_qi_per_message "Minimum Qi granted per message"
        int max_qi_per_message "Maximum Qi cap granted per message"
        double insight_multiplier "Multiplier for Insight influence on Qi gain"
        int message_cooldown_seconds "Cooldown between eligible messages"
        boolean is_active
    }

    AUDIT_LOG {
        uuid id PK
        uuid character_id FK
        string action_type "EXP_GAIN, EXPLORATION_REWARD, ITEM_CONSUME, BREAKTHROUGH"
        jsonb details "Context data"
        timestamp created_at
    }
```
