# syntax=docker/dockerfile:1.7

# Loadly Aggregator API - Optimized Dockerfile
# Designed for standalone usage with docker run commands
# Supports fast startup mode and flexible environment configuration

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

# Copy dependency manifests first so source-only changes reuse NuGet restore.
# Directory.Build.props contains a PackageReference and must be in the restore
# cache key; BannedSymbols.txt is referenced by that props file.
COPY ["Directory.Build.props", "BannedSymbols.txt", "./"]
COPY ["LoadlyAggregator.API/LoadlyAggregator.API.csproj", "LoadlyAggregator.API/"]
COPY ["LoadlyAggregator.Domain/LoadlyAggregator.Domain.csproj", "LoadlyAggregator.Domain/"]
COPY ["LoadlyAggregator.Infrastructure/LoadlyAggregator.Infrastructure.csproj", "LoadlyAggregator.Infrastructure/"]
COPY ["LoadlyAggregator.Shared/LoadlyAggregator.Shared.csproj", "LoadlyAggregator.Shared/"]
RUN dotnet restore "LoadlyAggregator.API/LoadlyAggregator.API.csproj"

# EF Core tools (used by the bundle stages below).
RUN dotnet tool install --global dotnet-ef --version 10.0.*
ENV PATH="${PATH}:/root/.dotnet/tools"

# Build migration bundles from only the projects that define the DbContexts and
# migrations. API-only changes therefore do not invalidate this stage.
FROM restore AS migrations
COPY ["LoadlyAggregator.Shared/", "LoadlyAggregator.Shared/"]
COPY ["LoadlyAggregator.Domain/", "LoadlyAggregator.Domain/"]
COPY ["LoadlyAggregator.Infrastructure/", "LoadlyAggregator.Infrastructure/"]
WORKDIR "/src/LoadlyAggregator.Infrastructure"

# Design-time factories require connection strings while generating bundles,
# but they do not connect to these databases during the image build.
ENV ConnectionStrings__DefaultConnection="Host=localhost;Database=build;Username=build;Password=build" \
    ConnectionStrings__LogsConnection="Host=localhost;Database=build_logs;Username=build;Password=build"

RUN dotnet build "LoadlyAggregator.Infrastructure.csproj" \
      -c Release \
      --no-restore

# Framework-dependent bundles reuse the ASP.NET runtime in the final image.
# Keep these commands sequential to avoid two concurrent compiler processes.
RUN mkdir -p /app/migrations \
    && dotnet ef migrations bundle \
      --context ApplicationDbContext \
      --project LoadlyAggregator.Infrastructure.csproj \
      --startup-project LoadlyAggregator.Infrastructure.csproj \
      --configuration Release \
      --output /app/migrations/efbundle \
      --no-build \
    && chmod +x /app/migrations/efbundle

RUN dotnet ef migrations bundle \
      --context LogsDbContext \
      --project LoadlyAggregator.Infrastructure.csproj \
      --startup-project LoadlyAggregator.Infrastructure.csproj \
      --configuration Release \
      --output /app/migrations/efbundle-logs \
      --no-build \
    && chmod +x /app/migrations/efbundle-logs

FROM restore AS publish
# Carry the cached bundles forward and make cold compilation sequential, which
# avoids the runner OOM seen when publish and bundle builds run concurrently.
COPY --from=migrations /app/migrations /app/migrations

# Copy application source only after the independently cacheable bundle stage.
COPY . .
WORKDIR "/src/LoadlyAggregator.API"
RUN dotnet publish "LoadlyAggregator.API.csproj" \
      -c Release \
      -o /app/publish \
      --no-restore \
      /p:UseAppHost=false \
      /p:ExcludeMigrationsFromBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# tzdata: Africa/Cairo DST rules (Law 24/2023); curl for HEALTHCHECK.
# Cache package indexes and archives between BuildKit builds. The mounted
# directories are not committed to the final image layer.
RUN --mount=type=cache,id=apt-lists,target=/var/lib/apt/lists,sharing=locked \
    --mount=type=cache,id=apt-cache,target=/var/cache/apt,sharing=locked \
    apt-get update \
    && apt-get install -y --no-install-recommends tzdata curl

# Copy published application and independently cached migration bundles.
COPY --from=publish /app/publish .
COPY --from=publish /app/migrations/efbundle /app/efbundle
COPY --from=publish /app/migrations/efbundle-logs /app/efbundle-logs

# Copy entrypoint script.
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

# Set default environment variables (can be overridden with docker run -e).
ENV ASPNETCORE_URLS=http://+:8080
ENV SKIP_MIGRATION=false
ENV SKIP_PERMISSIONS=false
ENV FAST_STARTUP=true

ENV TZ=Africa/Cairo

# Performance optimizations.
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# HEALTHCHECK --interval=30s --timeout=5s --start-period=120s --retries=3 \
# CMD curl -sf http://localhost:8080/health || exit 1

ENTRYPOINT ["/app/entrypoint.sh"]