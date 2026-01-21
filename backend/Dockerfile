FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080
# Install ICU libs before switching to non-root user
RUN apk add --no-cache icu-libs
USER app

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Src/*/*.csproj", "Directory.Build.props", "Directory.Packages.props", "global.json", "./"]
RUN for file in $(ls *.csproj); do mkdir -p Src/${file%.*}/ && mv $file Src/${file%.*}/; done

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "Src/PIED_LMS.API/PIED_LMS.API.csproj"

# Copy toàn bộ code
COPY . .

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "Src/PIED_LMS.API/PIED_LMS.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "PIED_LMS.API.dll"]
