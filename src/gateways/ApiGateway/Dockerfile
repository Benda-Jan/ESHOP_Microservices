FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY src/gateways/ApiGateway/*.csproj .
COPY src/libraries/JwtLibrary/*.csproj .
RUN dotnet restore ApiGateway.csproj
COPY . .
WORKDIR /src
RUN dotnet publish "src/gateways/ApiGateway/ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
