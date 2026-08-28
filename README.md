# AssetPortal

Internal IT asset-management portal for tracking inventory, support tickets, and
integration webhooks across the organization.

## Features

- Asset inventory with search, sorting, and CSV export
- Support ticket queue with comments and status workflow
- Webhook subscriptions and event delivery
- Employee directory and profiles
- Data import/restore for asset snapshots
- Session-based authentication for staff accounts

## Tech stack

- ASP.NET Core 8 (MVC + Razor views)
- Entity Framework Core with SQLite
- Newtonsoft.Json
- Docker (multi-stage build)

## Getting started

### Run directly

```bash
dotnet run --project src/AssetPortal.Web
```

The app listens on `http://localhost:5000` by default. A SQLite database file is
created automatically on first launch and seeded with starter sample records:
an administrator account, support and finance users, a few assets, one open ticket,
and a webhook subscription.

### Run with Docker

```bash
docker compose up --build
```

The app is then available at `http://localhost:8080`. The database is persisted on the
`assetportal-data` volume.

## Default credentials

| Username | Password   | Role  |
|----------|------------|-------|
| admin    | admin1234  | Admin |

## Project layout

```
src/AssetPortal.Web/
  Controllers/            Request handling and API endpoints
  Data/                    Database context and seed data
  Helpers/                 Shared utilities (hashing, export, errors)
  Middleware/              Request pipeline components
  Models/                  Entities and enums
  Repositories/            Data access layer
  Services/                Business logic (tokens, webhooks, import)
  Views/                   Razor templates
  wwwroot/                 Static assets
```

## Configuration

Settings live in `appsettings.json`. The SQLite connection string can be overridden
with the `ConnectionStrings__Default` environment variable. Asset uploads are stored
under `wwwroot/uploads`.

## API endpoints

| Method | Path        | Description                        |
|--------|-------------|------------------------------------|
| POST   | `/api/token`   | Issue a signed integration token |
| POST   | `/api/notify`  | Submit an event notification    |

## License

Internal use.
