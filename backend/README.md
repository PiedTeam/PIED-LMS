# Backend Overview

This backend is a .NET 10 solution for PIED-LMS. It includes the API layer plus supporting application, domain, and infrastructure projects. SDK version is pinned via [backend/global.json](global.json).

## Prerequisites

- .NET 10 SDK (required)
- Docker (optional, for container runs)
- Doppler CLI (optional, if you use Doppler-managed secrets)
- go-task (optional, to run the Taskfile shortcuts)

## Project layout

- [backend/backend.slnx](backend.slnx) — solution file
- [backend/Src/PIED-LMS.API](Src/PIED-LMS.API) — ASP.NET Core API
- [backend/Src/PIED-LMS.Application](Src/PIED-LMS.Application) — application services
- [backend/Src/PIED-LMS.Domain](Src/PIED-LMS.Domain) — domain model
- [backend/Src/PIED-LMS.Infrastructure](Src/PIED-LMS.Infrastructure) — infrastructure and data access

## Quick start

1) Restore and build

```bash
cd backend
dotnet restore
dotnet build
```

1) Run the API (hot reload)

```bash
cd Backend
doppler run -- dotnet watch run --project Src/PIED-LMS.API/PIED-LMS.API.csproj
```

If you are not using Doppler, drop the `doppler run --` prefix.

1) Run tests

```bash
cd Backend
dotnet test
```

## Task shortcuts

If you have go-task installed, the Taskfile provides helpful aliases:

- `task dev` — run API with hot reload
- `task test` — run all tests
- `task migrate` — apply database migrations
- `task clean` — remove all bin/obj folders
- `task status` — show the current Doppler project/config
- `task list` — list available tasks

## Database migrations

From `Backend/`:

```bash
doppler run -- dotnet ef database update --project Src/PIED-LMS.API/PIED-LMS.API.csproj
```

Adjust the command if you manage configuration another way.

## Docker

Build and run locally from `Backend/`:

```bash
docker build -f Src/PIED-LMS.API/Dockerfile -t pied-lms-api:dev .
docker run --rm -p 8080:8080 pied-lms-api:dev
```

The Dockerfile exposes port 8080 and includes a health check.

## CI

The GitHub workflow [Backend/.github/workflows/backend-ci.yaml](../.github/workflows/backend-ci.yaml) verifies pull requests by restoring, building, and performing a Docker syntax check (no push).
