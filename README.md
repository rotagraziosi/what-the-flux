# WhatTheFlux

A menstrual flow tracking app to log and monitor periods over time.

Built with an Angular 21 frontend and a .NET 10 REST API, backed by SQLite.

## Stack

| Layer    | Technology          |
|----------|---------------------|
| Frontend | Angular 21 (PWA)    |
| Backend  | ASP.NET Core 10     |
| Database | SQLite (via EF Core)|
| Runtime  | Docker              |

## Running with Docker

```bash
docker compose up -d
```

The app will be available at <http://localhost:4321>.

> Port `4321` on the host maps to port `8080` inside the container.
> The SQLite database is persisted in the `app-data` Docker volume.

## Development

### Prerequisites

- .NET 10 SDK
- Node.js 22 + npm

### Backend

```bash
cd src/WhatTheFlux.Api
dotnet run
```

API listens on `http://localhost:5000` (or as configured in `launchSettings.json`).

### Frontend

```bash
cd src/WhatTheFlux.Client
npm install
npm start
```

Angular dev server runs on `http://localhost:4200` and proxies API calls to the backend.

## API Endpoints

| Method | Path                                              | Description              |
|--------|---------------------------------------------------|--------------------------|
| GET    | `/api/criteria`                                   | List tracking criteria   |
| GET    | `/api/periods`                                    | List all periods         |
| POST   | `/api/periods`                                    | Create a period          |
| GET    | `/api/periods/{id}`                               | Get a period             |
| DELETE | `/api/periods/{id}`                               | Delete a period          |
| POST   | `/api/periods/{id}/days`                          | Add a day to a period    |
| DELETE | `/api/periods/{id}/days/{dayNumber}`              | Remove a day             |
| PUT    | `/api/periods/{id}/days/{dayNumber}/counts/{criterionId}` | Log a criterion count |

## Tracking Criteria

| ID | Label                        | Multiplier |
|----|------------------------------|------------|
| 1  | Slightly soaked protection   | ×1         |
| 2  | Moderately soaked protection | ×5         |
| 3  | Saturated protection         | ×20        |
| 4  | Small blood clot (<1 cm)     | ×1         |
| 5  | Large blood clot (>1 cm)     | ×5         |
