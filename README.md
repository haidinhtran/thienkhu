# Cultivation RPG Discord Bot

A text-based Eastern Fantasy (Xianxia/Wuxia) RPG Discord Bot that allows users to cultivate, explore, and battle directly within Discord using a modern, decoupled architecture.

## 🏗 Architecture Overview

The system is built with a decoupled architecture to ensure scalability and future expansion (e.g., adding a Web Client):
- **Frontend / Bot Layer:** Node.js + TypeScript (Discord.js v14) with Native ESM (`tsx`).
- **Backend / Core Logic:** .NET 10 Web API using Clean Architecture and CQRS (MediatR + FluentValidation).
- **Database:** PostgreSQL 17 (Entity Framework Core).
- **Caching:** Redis 7.4.

*(Refer to the `docs/` folder for detailed system architecture, database schema, user flows, and game design documents).*

## 🚀 Prerequisites

To run this project locally, you will need:
- [Docker & Docker Compose](https://www.docker.com/) (For PostgreSQL & Redis)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 24 LTS](https://nodejs.org/en/download/) (or Node 20+)

## 🛠 Getting Started

### 1. Start Infrastructure (Database & Cache)
Start the PostgreSQL and Redis containers in the background:
```bash
docker-compose up -d
```
> **Note:** The `docker-compose.yml` also includes a `pgadmin` container accessible at `http://localhost:5050`.

### 2. Setup and Run Backend (.NET API)
Navigate to the API folder, restore dependencies, and run the server:
```bash
cd src/CoreAPI
dotnet build CultivationApi.slnx
dotnet ef database update --project CultivationApi.Infrastructure --startup-project CultivationApi.WebApi
dotnet run --project CultivationApi.WebApi/CultivationApi.WebApi.csproj
```

### 3. Setup and Run Discord Bot
Open a new terminal, navigate to the Bot folder, install dependencies, and start the development server:
```bash
cd src/DiscordBot
npm install
npm run dev
```

> **Note:** You will need to create a `.env` file inside `src/DiscordBot` and provide your `DISCORD_TOKEN`.

## 📜 Documentation

- `docs/system_architecture.md`: System components and data flow.
- `docs/user_flow.md`: How players interact with the `/tutien` command.
- `docs/database_schema.md`: PostgreSQL tables and JSONB columns.
- `docs/game_design_document.md`: Core loop, activities, and gameplay mechanics.
- `AGENTS.md`: Mandatory developer guidelines and AI operating protocols for this repository.
