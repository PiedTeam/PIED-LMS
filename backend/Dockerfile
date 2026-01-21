# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /src

# Copy project files
COPY ["Src/PIED_LMS.API/PIED_LMS.API.csproj", "Src/PIED_LMS.API/"]
COPY ["Src/PIED_LMS.Application/PIED_LMS.Application.csproj", "Src/PIED_LMS.Application/"]
COPY ["Src/PIED_LMS.Contract/PIED_LMS.Contract.csproj", "Src/PIED_LMS.Contract/"]
COPY ["Src/PIED_LMS.Domain/PIED_LMS.Domain.csproj", "Src/PIED_LMS.Domain/"]
COPY ["Src/PIED_LMS.Infrastructure/PIED_LMS.Infrastructure.csproj", "Src/PIED_LMS.Infrastructure/"]
COPY ["Src/PIED_LMS.Persistence/PIED_LMS.Persistence.csproj", "Src/PIED_LMS.Persistence/"]
COPY ["Src/PIED_LMS.Presentation/PIED_LMS.Presentation.csproj", "Src/PIED_LMS.Presentation/"]

# Copy build props
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]

# Restore dependencies
RUN dotnet restore "Src/PIED_LMS.API/PIED_LMS.API.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "Src/PIED_LMS.API/PIED_LMS.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Src/PIED_LMS.API/PIED_LMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PIED_LMS.API.dll"]
