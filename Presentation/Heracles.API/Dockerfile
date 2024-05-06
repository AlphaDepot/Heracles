#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/Heracles.API/Heracles.API.csproj", "Presentation/Heracles.API/"]
COPY ["Core/Heracles.Application/Heracles.Application.csproj", "Core/Heracles.Application/"]
COPY ["Core/Heracles.Domain/Heracles.Domain.csproj", "Core/Heracles.Domain/"]
COPY ["Infrastructure/Heracles.Infrastructure/Heracles.Infrastructure.csproj", "Infrastructure/Heracles.Infrastructure/"]
COPY ["Infrastructure/Heracles.Persistence/Heracles.Persistence.csproj", "Infrastructure/Heracles.Persistence/"]
RUN dotnet restore "./Presentation/Heracles.API/Heracles.API.csproj"
COPY . .
WORKDIR "/src/Presentation/Heracles.API"
RUN dotnet build "./Heracles.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Heracles.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Heracles.API.dll"]