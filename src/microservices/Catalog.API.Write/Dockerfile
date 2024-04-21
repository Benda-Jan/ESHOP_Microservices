FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5140

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY src/microservices/Catalog.API.Write/*.csproj .
RUN dotnet restore Catalog.API.Write.csproj
COPY . .
WORKDIR /src
RUN dotnet publish "src/microservices/Catalog.API.Write/Catalog.API.Write.csproj" -c $BUILD_CONFIGURATION -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Catalog.API.Write.dll"]