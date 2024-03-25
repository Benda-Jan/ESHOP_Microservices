FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5261

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY Identity.API/*.csproj .
COPY Identity.Entities/*.csproj .
COPY Identity.Infrastructure/*.csproj .
RUN dotnet restore Identity.API.csproj
COPY . .
WORKDIR /src/Identity.API
RUN dotnet publish "Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Identity.API.dll"]