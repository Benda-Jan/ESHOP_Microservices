FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5136

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY Catalog.API/*.csproj .
COPY Catalog.Entities/*.csproj .
COPY Catalog.Infrastructure/*.csproj .
COPY EventBus/*.csproj .
RUN dotnet restore Catalog.API.csproj
COPY . .
WORKDIR /src/Catalog.API
RUN dotnet publish "Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]