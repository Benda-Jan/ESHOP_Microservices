FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5261

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY src/microservices/Identity.API/*.csproj .
COPY src/microservices/Identity.Entities/*.csproj .
COPY src/microservices/Identity.Infrastructure/*.csproj .
COPY src/libraries/JwtLibrary/*.csproj .
RUN dotnet restore Identity.API.csproj
COPY . .
WORKDIR /src
RUN dotnet publish "src/microservices/Identity.API/Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Identity.API.dll"]