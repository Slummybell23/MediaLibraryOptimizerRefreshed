# Media Library Optimizer

A self-hosted tool for homelabbers that walks your media library and reclaims disk space — without sacrificing quality. Runs as a single Docker container with a web UI.

## What it does

Media Library Optimizer scans the media files you point it at and runs optimization tasks against them:

- **Near-lossless AV1 encoding** — re-encodes video to AV1 at visually transparent quality to significantly reduce file sizes.
- **Dolby Vision profile 7 → 8 remuxing** — converts DV profile 7 (dual-layer, disc-sourced) files to profile 8 for broad player compatibility, without re-encoding.

> **Status:** early development. The scaffolding (API, database, web UI, containerization) is in place; the scanning and encoding pipelines are being built.

## Tech stack

| Component | Technology |
|-----------|------------|
| Backend   | ASP.NET Core (.NET 10) Web API |
| Database  | SQLite via Entity Framework Core (auto-migrates on startup) |
| Frontend  | React 19 + Vite, Tailwind CSS v4 (JavaScript) |
| Media tooling | ffmpeg (bundled in the Docker image) |
| Deployment | Single Docker image serving both API and UI |

## Quick start (Docker Compose)

```yaml
services:
  media-library-optimizer:
    image: media-library-optimizer:latest
    container_name: media-library-optimizer
    ports:
      - "8080:8080"
    volumes:
      - appdata:/appdata
      - /path/to/your/media:/media
    restart: unless-stopped

volumes:
  appdata:
```

```bash
git clone https://github.com/<you>/MediaLibraryOptimizer.git
cd MediaLibraryOptimizer
# Edit docker-compose.yml and set the media path to your library
docker compose up -d --build
```

Then open <http://localhost:8080>.

### Or with `docker run`

```bash
docker build -t media-library-optimizer .

docker run -d \
  --name media-library-optimizer \
  -p 8080:8080 \
  -v appdata:/appdata \
  -v /path/to/your/media:/media \
  --restart unless-stopped \
  media-library-optimizer
```

### Volumes

| Container path | Purpose |
|----------------|---------|
| `/appdata` | SQLite database and application state. Use a named volume or a bind mount you back up. |
| `/media`   | Your media library. Mount read-write so optimized files can replace the originals. |

### Configuration

Paths are configurable through environment variables (defaults shown are what the Docker image ships with):

| Variable | Default | Description |
|----------|---------|-------------|
| `Storage__AppDataPath` | `/appdata` | Where the SQLite database lives |
| `Storage__MediaPath`   | `/media`   | Root of the media library to scan |

No authentication is built in — run it on a trusted network or behind your own reverse-proxy auth (Authelia, Authentik, etc.).

## Local development

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download), [Node.js 22+](https://nodejs.org/).

**Backend** (runs on `http://localhost:5250`):

```bash
cd Server
dotnet tool restore   # installs dotnet-ef (only needed for migrations)
dotnet run
```

The SQLite database is created at `Server/appdata/medialibraryoptimizer.db` and migrations apply automatically on startup.

**Frontend** (runs on `http://localhost:5173`, proxies `/api` to the backend):

```bash
cd webapp
npm install
npm run dev
```

**Adding a database migration** after changing the entity models:

```bash
cd Server
dotnet ef migrations add <MigrationName>
```

## Project structure

```
├── Server/            ASP.NET Core Web API
│   ├── Controllers/   API endpoints (media files, jobs, system status)
│   ├── Data/          EF Core DbContext
│   ├── Models/        MediaFile, OptimizationJob entities
│   └── Migrations/    EF Core migrations
├── webapp/            React + Vite + Tailwind frontend
├── Dockerfile         Multi-stage build → single runtime image
└── docker-compose.yml Example deployment
```

## API

The backend exposes a small REST API under `/api`:

- `GET /api/system/status` — health/status info (paths, file and job counts)
- `GET /api/media-files` — discovered media files
- `GET /api/media-files/{id}` — a single file including its job history
- `GET /api/jobs?status=Pending` — optimization jobs, optionally filtered by status
- `POST /api/jobs` — queue a job: `{ "mediaFileId": 1, "type": "Av1Encode" }`

An OpenAPI document is available at `/openapi/v1.json` when running in development.

## License

[MIT](LICENSE)
