# =============================================================================
# Dockerfile para AppCreditosBackEnd API
# Optimizado para producción con multi-stage build
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1: Base runtime image
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user for security
RUN addgroup --group appuser --gid 1001 \
    && adduser --uid 1001 --gid 1001 --system appuser

# -----------------------------------------------------------------------------
# Stage 2: Build environment
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file first for better layer caching
COPY ["AppCreditosBackEnd.sln", "./"]

# Copy project files for dependency restore
COPY ["AppCreditosBackEnd.Api/AppCreditosBackEnd.Api.csproj", "AppCreditosBackEnd.Api/"]
COPY ["AppCreditosBackEnd.Application/AppCreditosBackEnd.Application.csproj", "AppCreditosBackEnd.Application/"]
COPY ["AppCreditosBackEnd.Domain/AppCreditosBackEnd.Domain.csproj", "AppCreditosBackEnd.Domain/"]
COPY ["AppCredtiosBackEnd.Infrastructure/AppCreditosBackEnd.Infrastructure.csproj", "AppCredtiosBackEnd.Infrastructure/"]

# Restore dependencies (cached layer if project files haven't changed)
RUN dotnet restore "AppCreditosBackEnd.Api/AppCreditosBackEnd.Api.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/AppCreditosBackEnd.Api"
RUN dotnet build "AppCreditosBackEnd.Api.csproj" -c Release -o /app/build --no-restore

# -----------------------------------------------------------------------------
# Stage 3: Publish
# -----------------------------------------------------------------------------
FROM build AS publish
RUN dotnet publish "AppCreditosBackEnd.Api.csproj" -c Release -o /app/publish --no-restore \
    --no-build \
    /p:PublishReadyToRun=true \
    /p:PublishTrimmed=false

# -----------------------------------------------------------------------------
# Stage 4: Final runtime image
# -----------------------------------------------------------------------------
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_EnableDiagnostics=0

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "AppCreditosBackEnd.Api.dll"]
