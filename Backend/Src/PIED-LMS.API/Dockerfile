# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy cấu hình và restore
COPY ["Directory.Build.props", "Directory.Packages.props", "global.json", "./"]
COPY ["Src/PIED-LMS.API/PIED-LMS.API.csproj", "Src/PIED-LMS.API/"]
# ... (Copy các project khác tương tự)
RUN dotnet restore "Src/PIED-LMS.API/PIED-LMS.API.csproj"

# Copy source và Publish
COPY ["Src/", "Src/"]
WORKDIR "/src/Src/PIED-LMS.API"
RUN dotnet publish "PIED-LMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Tối ưu chown ngay lúc copy
COPY --from=build --chown=app:app /app/publish .

USER app
EXPOSE 8080

# Healthcheck: Chấp nhận cài curl nếu bạn cần kiểm tra Endpoint thực tế
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*
USER app

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "PIED-LMS.API.dll"]
