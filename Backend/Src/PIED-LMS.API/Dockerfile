# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]
COPY ["Src/PIED-LMS.API/PIED-LMS.API.csproj", "Src/PIED-LMS.API/"]
COPY ["Src/PIED-LMS.Application/PIED-LMS.Application.csproj", "Src/PIED-LMS.Application/"]
COPY ["Src/PIED-LMS.Domain/PIED-LMS.Domain.csproj", "Src/PIED-LMS.Domain/"]
COPY ["Src/PIED-LMS.Infrastructure/PIED-LMS.Infrastructure.csproj", "Src/PIED-LMS.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Src/PIED-LMS.API/PIED-LMS.API.csproj"

# Copy source code
COPY ["Src/", "Src/"]

# Build and publish
WORKDIR "/src/Src/PIED-LMS.API"
RUN dotnet build "PIED-LMS.API.csproj" -c Release -o /app/build
RUN dotnet publish "PIED-LMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1000 appuser && \
    adduser --system --uid 1000 --ingroup appuser --shell /bin/false appuser

# Copy published app from build stage
COPY --from=build /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "PIED-LMS.API.dll"]
