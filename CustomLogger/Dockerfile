FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Publish
WORKDIR /src
COPY /CustomLogger/*.csproj .
COPY /EventBus/*.csproj .
RUN dotnet restore CustomLogger.csproj
COPY . .
WORKDIR /src/CustomLogger
RUN dotnet publish "CustomLogger.csproj" -c $BUILD_CONFIGURATION -o /app

FROM build AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CustomLogger.dll"]