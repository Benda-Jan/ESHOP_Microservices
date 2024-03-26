#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY ["src/gateways/ApiGateway/ApiGateway.csproj", "src/gateways/ApiGateway/"]
COPY ["src/middlewares/AuthorizationMiddleware/AuthorizationMiddleware.csproj", "src/middlewares/AuthorizationMiddleware/"]
COPY ["src/libraries/JwtLibrary/JwtLibrary.csproj", "src/libraries/JwtLibrary/"]
RUN dotnet restore "src/gateways/ApiGateway/ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/gateways/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
