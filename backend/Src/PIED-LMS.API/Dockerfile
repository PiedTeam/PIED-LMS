# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy configuration and restore
COPY ["Directory.Build.props", "Directory.Packages.props", "global.json", "./"]
COPY ["Src/PIED-LMS.API/PIED-LMS.API.csproj", "Src/PIED-LMS.API/"]
COPY ["Src/PIED-LMS.Application/PIED-LMS.Application.csproj", "Src/PIED-LMS.Application/"]
COPY ["Src/PIED-LMS.Infrastructure/PIED-LMS.Infrastructure.csproj", "Src/PIED-LMS.Infrastructure/"]
COPY ["Src/PIED-LMS.Domain/PIED-LMS.Domain.csproj", "Src/PIED-LMS.Domain/"]
RUN dotnet restore "Src/PIED-LMS.API/PIED-LMS.API.csproj"

# Copy source and publish
COPY ["Src/", "Src/"]
WORKDIR "/src/Src/PIED-LMS.API"
RUN dotnet publish "PIED-LMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime


WORKDIR /app

# Optimize chown during copy
COPY --from=build --chown=app:app /app/publish .

USER app
EXPOSE 8080

# Healthcheck: Install curl if you need to check actual endpoint
USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*
USER app

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "PIED-LMS.API.dll"]
