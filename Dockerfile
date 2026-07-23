# ---- Stage 1: build the React frontend ----
FROM node:22-alpine AS webapp-build
WORKDIR /src/webapp
COPY webapp/package.json webapp/package-lock.json ./
RUN npm ci
COPY webapp/ ./
RUN npm run build

# ---- Stage 2: build the ASP.NET Core backend ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS server-build
WORKDIR /src/Server
COPY Server/Server.csproj ./
RUN dotnet restore
COPY Server/ ./
RUN dotnet publish -c Release -o /app/publish

# ---- Stage 3: runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# ffmpeg powers the AV1 encoding and remuxing features.
RUN apt-get update \
    && apt-get install -y --no-install-recommends ffmpeg \
    && rm -rf /var/lib/apt/lists/*

COPY --from=server-build /app/publish ./
# The SPA is served by ASP.NET Core as static files.
COPY --from=webapp-build /src/webapp/dist ./wwwroot

# /appdata holds the SQLite database; /media is the user's media library.
ENV ASPNETCORE_URLS=http://+:8080 \
    Storage__AppDataPath=/appdata \
    Storage__MediaPath=/media

EXPOSE 8080

ENTRYPOINT ["dotnet", "Server.dll"]
