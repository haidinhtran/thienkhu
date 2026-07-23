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

## 3. API Contracts

The core REST API exposes endpoints for the Discord Bot to consume. 

### Characters API

**GET `/api/v1/characters/profile`**
Retrieves the character profile for a user. If the character, server config, or discord user does not exist, it will be automatically created.

- **Query Parameters:**
  - `discordId` (string, required): The Discord Snowflake ID of the user.
  - `serverId` (string, required): The Discord Snowflake ID of the server.
  - `username` (string, required): The current Discord username.
- **Response (`200 OK`):**
  ```json
  {
    "discordId": "string",
    "serverId": "string",
    "username": "string",
    "level": "int",
    "realmName": "string",
    "currentQi": "int",
    "dailyQiLimit": "int",
    "spiritStones": "int",
    "currentState": "string"
  }
  ```

**POST `/api/v1/characters/create`**
Creates a new character profile.

- **Request Body:**
  ```json
  {
    "discordId": "string",
    "serverId": "string",
    "username": "string"
  }
  ```
- **Response (`200 OK`):**
  Same as `GET /api/v1/characters/profile`

**POST `/api/v1/characters/gain-qi`**
Grants passive Qi to a character based on server configuration limits and cooldowns.

- **Request Body:**
  ```json
  {
    "discordId": "string",
    "serverId": "string",
    "username": "string"
  }
  ```
- **Response (`200 OK` or `400 Bad Request` if cooldown/limit hit):**
  ```json
  {
    "success": true,
    "message": "string",
    "currentQi": "long",
    "gainedQi": "long"
  }
  ```

**POST `/api/v1/characters/ascend`**
Attempts to ascend a character to the next level by consuming required Qi and Items.

- **Request Body:**
  ```json
  {
    "discordId": "string",
    "serverId": "string"
  }
  ```
- **Response (`200 OK` or `400 Bad Request` if requirements not met):**
  ```json
  {
    "success": true,
    "message": "string",
    "oldLevel": "int",
    "newLevel": "int",
    "newBaseStats": {
      "strength": "int",
      "agility": "int",
      "luck": "int",
      "health": "int",
      "mana": "int"
    }
  }
  ```

### Servers API

**GET `/api/v1/servers/{serverId}/config`**
Retrieves the server's configuration, including chat-to-earn settings. Auto-creates a default config if one doesn't exist.

- **Response (`200 OK`):**
  ```json
  {
    "serverId": "string",
    "chatToEarnChannels": ["string"],
    "isActive": true
  }
  ```

### Inventory API

**GET `/api/v1/inventory`**
Retrieves the user's inventory and equipped gear.

- **Query Parameters:**
  - `discordId` (string, required)
  - `serverId` (string, required)
- **Response (`200 OK`):**
  ```json
  {
    "id": "string",
    "characterId": "string",
    "items": [
      {
        "itemId": "string",
        "quantity": "int",
        "itemType": "string"
      }
    ],
    "equippedGear": {
      "weaponId": "string",
      "armorId": "string",
      "accessoryId": "string"
    }
  }
  ```

**POST `/api/v1/inventory/equip`**
Equips an item to a specific slot (Weapon, Armor, Accessory).

- **Query Parameters:**
  - `discordId` (string, required)
  - `serverId` (string, required)
  - `itemId` (string, required)
  - `slot` (string, required)
- **Response (`200 OK`):**
  ```json
  {
    "success": true
  }
  ```

### Activities API

**POST `/api/v1/activities/explore`**
Initiates an exploration event for a user in a specific location. Updates the character's state to `IN_EXPLORATION`.

- **Request Body:**
  ```json
  {
    "discordId": "string",
    "serverId": "string",
    "locationId": "string"
  }
  ```
- **Response (`200 OK`):**
  ```json
  {
    "eventId": "string",
    "eventType": "string",
    "title": "string",
    "description": "string",
    "imageUrl": "string",
    "choices": [
      {
        "choiceId": "string",
        "label": "string",
        "style": "string"
      }
    ]
  }
  ```

**POST `/api/v1/activities/explore/choice`**
Submits the user's choice for an ongoing exploration event. Resolves the event, distributes rewards/penalties, and resets character state to `IDLE`.

- **Request Body:**
  ```json
  {
    "discordId": "string",
    "serverId": "string",
    "eventId": "string",
    "choiceId": "string"
  }
  ```
- **Response (`200 OK`):**
  ```json
  {
    "success": true,
    "title": "string",
    "narrative": "string",
    "qiReward": "long",
    "spiritStonesReward": "int"
  }
  ```

**POST `/api/v1/activities/secret-domain`**
Initiates a combat challenge in a specific Secret Domain to farm items and Spirit Stones.

- **Request Body:**
  ```json
  {
    "discordId": "string",
    "serverId": "string",
    "domainId": "string"
  }
  ```
- **Response (`200 OK` or `400 Bad Request`):**
  ```json
  {
    "success": true,
    "message": "string",
    "isVictory": true,
    "battleLog": ["string"],
    "rewardSpiritStones": "int",
    "rewardItems": [
      {
        "itemId": "string",
        "quantity": "int",
        "itemType": "string"
      }
    ]
  }
  ```

## 4. State Management & Error Handling

### 4.1 Exploration State Soft-Lock Prevention (MVP)
The MVP backend does not persist generated `ExplorationEvent` instances in the database (they are returned on the fly). This introduces a risk of soft-locking: if a user navigates away from the event embed, their character remains locked in the `IN_EXPLORATION` state with no way to retrieve the lost event.
- **Frontend Mitigation:** The Discord Bot maintains an in-memory cache (`activeExplorations`) mapping `discordId` to their active `ExplorationEvent`. If the user clicks `[Exploration]` while still in the `IN_EXPLORATION` state, the bot seamlessly resumes the cached event.
- **Backend Mitigation:** `StartExplorationAsync` is permitted to overwrite an existing `IN_EXPLORATION` state. If the bot restarts and the cache is lost, the user can safely generate a new event to clear the lock.

### 4.2 API Error Mapping & Global Exception Handling
The Discord bot delegates all business rules (stat requirements, state machine validations) to the backend API.
1. **Backend Propagation:** When a rule is violated (e.g., trying to enter a Secret Domain while exploring), the backend throws a `DomainException`, which the Global Exception Handler (`GlobalExceptionHandler.cs`) translates into an HTTP 400 Bad Request with a `ProblemDetails` JSON body.
2. **Frontend Translation:** The `CultivationApiClient` explicitly intercepts 400 Bad Request responses. If it detects a `Domain Rule Violation`, it throws a custom `ApiBusinessError`.
3. **Global UX Handler:** The bot's global `interactionCreate` event listener catches `ApiBusinessError` and renders the specific business violation text directly to the user as an Ephemeral Embed, preventing generic "Unexpected error" masks while avoiding redundant `try-catch` blocks in controllers.
