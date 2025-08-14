# music-data-api-2471-2480

Music Data API backend (ASP.NET Core 8 + EF Core) that serves songs, artists, and genres with pagination and filtering.

## Features
- RESTful JSON endpoints for:
  - GET /api/songs (search, filter by artistId/genreId, paginate)
  - GET /api/songs/{id}
  - GET /api/artists (paginate)
  - GET /api/artists/{id}
  - GET /api/artists/{id}/songs (paginate)
  - GET /api/genres (paginate)
  - GET /api/genres/{id}
  - GET /api/genres/{id}/songs (paginate)
- OpenAPI docs via NSwag at /docs
- EF Core database integration:
  - SQLite by default (auto-creates Data/music.db and seeds demo data)
  - Optional SQL Server or PostgreSQL via environment variables

## Run locally
From the songs_api_backend folder:
```
dotnet restore
dotnet run
```
Open http://localhost:3001/docs for the interactive API docs.

## Configuration
Set environment variables via a .env file (populated by the orchestrator). See `.env.example` for supported values:
- DATABASE_PROVIDER: sqlite | sqlserver | postgres (default: sqlite)
- SQLITE_PATH: path for SQLite DB file (optional; default Data/music.db inside app)
- DATABASE_CONNECTION_STRING: required for sqlserver/postgres

Note: For production, prefer EF Core migrations instead of EnsureCreated/seed.

## Health
- GET / returns `{ "message": "Healthy" }`.