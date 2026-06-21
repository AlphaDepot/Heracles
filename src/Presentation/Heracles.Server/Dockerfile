FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/Heracles.Server/Heracles.Server.csproj", "src/Presentation/Heracles.Server/"]
RUN dotnet restore "src/Presentation/Heracles.Server/Heracles.Server.csproj"
COPY . .
WORKDIR "/src/src/Presentation/Heracles.Server"
RUN dotnet build "./Heracles.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Heracles.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Heracles.Server.dll"]
