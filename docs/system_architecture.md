# System Architecture Document - Cultivation RPG Bot (MVP)

This document outlines the technical architecture, layer responsibilities, and data communication flow for the Cultivation RPG Discord Bot. The architecture decouples the presentation layer from backend logic to ensure future web client expansion.

## 1. Technologies

- Frontend: Node.js 24 LTS + TypeScript 5.x + Discord.js v14
- Backend: .NET 10 LTS (C# 13 + EF Core 10)
- Database: PostgreSQL 17 LTS
- Cache: Redis 7.4+
- AI: OpenAI API / Local LLM (JSON mode)

## 2. System Architecture Diagram

```mermaid
flowchart TD
    classDef client fill:#0984e3,stroke:#74b9ff,stroke-width:2px,color:#fff;
    classDef bot fill:#00b894,stroke:#55efc4,stroke-width:2px,color:#fff;
    classDef api fill:#6c5ce7,stroke:#a29bfe,stroke-width:2px,color:#fff;
    classDef db fill:#e17055,stroke:#fab1a0,stroke-width:2px,color:#fff;
    classDef external fill:#fdcb6e,stroke:#ffeaa7,stroke-width:2px,color:#2d3436;

    subgraph ClientLayer [Discord Platform]
        User([Discord User]):::client
    end

    subgraph InterfaceLayer [Frontend Bot Service]
        DiscordBot[TypeScript Discord.js Bot]:::bot
    end

    subgraph LogicLayer [Backend Core Service]
        DotNetAPI[.NET Core REST API]:::api
        Engine[Game Logic & Combat Engine]:::api
    end

    subgraph DataLayer [Storage & Caching]
        Postgres[(PostgreSQL Database)]:::db
        RedisCache[(Redis Cache - Future)]:::db
    end

    subgraph IntegrationLayer [External Services]
        LLM[OpenAI API / Local LLM]:::external
    end

    %% Communication Flows
    User -- Slash Command / Interaction --> DiscordBot
    DiscordBot -- HTTP / REST Requests --> DotNetAPI
    DotNetAPI --> Engine
    Engine -- EF Core Queries --> Postgres
    Engine -- Read/Write Cache --> RedisCache
    Engine -- Prompt / Narrative Request --> LLM
    LLM -- JSON Narrative Response --> Engine
    DotNetAPI -- JSON Response --> DiscordBot
    DiscordBot -- Render Embeds & Buttons --> User
```
