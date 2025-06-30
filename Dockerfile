# =============================================================================
# Dockerfile para AppCreditosBackEnd API
# Versión simplificada para producción
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1: Build environment
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["AppCreditosBackEnd.sln", "./"]
COPY ["AppCreditosBackEnd.Api/AppCreditosBackEnd.Api.csproj", "AppCreditosBackEnd.Api/"]
COPY ["AppCreditosBackEnd.Application/AppCreditosBackEnd.Application.csproj", "AppCreditosBackEnd.Application/"]
COPY ["AppCreditosBackEnd.Domain/AppCreditosBackEnd.Domain.csproj", "AppCreditosBackEnd.Domain/"]
COPY ["AppCredtiosBackEnd.Infrastructure/AppCreditosBackEnd.Infrastructure.csproj", "AppCredtiosBackEnd.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "AppCreditosBackEnd.Api/AppCreditosBackEnd.Api.csproj"

# Copy source code and build
COPY . .
RUN dotnet publish "AppCreditosBackEnd.Api/AppCreditosBackEnd.Api.csproj" -c Release -o /app/publish

# -----------------------------------------------------------------------------
# Stage 2: Runtime image
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN addgroup --group appuser --gid 1001 \
    && adduser --uid 1001 --gid 1001 --system appuser

# Copy published application
COPY --from=build /app/publish .

# Set ownership and switch to non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Configure environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD dotnet --version || exit 1

# Entry point
ENTRYPOINT ["dotnet", "AppCreditosBackEnd.Api.dll"]
