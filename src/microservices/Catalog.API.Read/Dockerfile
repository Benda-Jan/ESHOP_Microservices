FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5136

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY src/microservices/Catalog.API.Read/*.csproj .
COPY src/microservices/Catalog.Entities/*.csproj .
COPY src/microservices/Catalog.Infrastructure/*.csproj .
COPY src/microservices/EventBus/*.csproj .
COPY src/libraries/JwtLibrary/*.csproj .
RUN dotnet restore Catalog.API.Read.csproj
COPY . .
WORKDIR /src
RUN dotnet publish "src/microservices/Catalog.API.Read/Catalog.API.Read.csproj" -c $BUILD_CONFIGURATION -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Catalog.API.Read.dll"]