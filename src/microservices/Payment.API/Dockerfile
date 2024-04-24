FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5059

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY src/microservices/Payment.API/*.csproj .
RUN dotnet restore Payment.API.csproj
COPY . .
WORKDIR /src
RUN dotnet publish "src/microservices/Payment.API/Payment.API.csproj" -c $BUILD_CONFIGURATION -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Payment.API.dll"]