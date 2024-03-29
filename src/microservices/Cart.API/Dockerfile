FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5264

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY src/microservices/Cart.API/*.csproj .
COPY src/microservices/Cart.Entities/*.csproj .
COPY src/microservices/Cart.Infrastructure/*.csproj .
COPY src/libraries/JwtLibrary/*.csproj .
RUN dotnet restore Cart.API.csproj
COPY . .
WORKDIR /src
RUN dotnet publish "src/microservices/Cart.API/Cart.API.csproj" -c $BUILD_CONFIGURATION -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Cart.API.dll"]