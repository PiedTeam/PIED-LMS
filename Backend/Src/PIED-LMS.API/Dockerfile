# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy các file cấu hình tập trung để tối ưu Docker Cache
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]

# Copy các file .csproj dựa theo đúng cấu trúc thư mục của Kyros
COPY ["Src/PIED-LMS.API/PIED-LMS.API.csproj", "Src/PIED-LMS.API/"]
COPY ["Src/PIED-LMS.Application/PIED-LMS.Application.csproj", "Src/PIED-LMS.Application/"]
COPY ["Src/PIED-LMS.Domain/PIED-LMS.Domain.csproj", "Src/PIED-LMS.Domain/"]
COPY ["Src/PIED-LMS.Infrastructure/PIED-LMS.Infrastructure.csproj", "Src/PIED-LMS.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Src/PIED-LMS.API/PIED-LMS.API.csproj"

# Copy toàn bộ mã nguồn
COPY ["Src/", "Src/"]

# Build và Publish ứng dụng
WORKDIR "/src/Src/PIED-LMS.API"
RUN dotnet build "PIED-LMS.API.csproj" -c Release -o /app/build
RUN dotnet publish "PIED-LMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# .NET 8+ đã có sẵn user 'app' (UID 1654), không cần tự tạo user/group nữa
# Copy dữ liệu đã publish từ build stage
COPY --from=build /app/publish .

# Đổi quyền sở hữu cho user 'app'
RUN chown -R app:app /app

# Chuyển sang dùng user 'app' để bảo mật (Non-root)
USER app

# Port mặc định của user 'app' là 8080
EXPOSE 8080

# Health check bằng wget (Ubuntu/Alpine bản gọn thường có sẵn wget hơn curl)
# Giả sử endpoint của Kyros là /health
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "PIED-LMS.API.dll"]
