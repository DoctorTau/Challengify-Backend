# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080 5432

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Backend.sln", "./"]
COPY ["Controllers/Controllers.csproj", "Controllers/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Services/Services.csproj", "Services/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/Controllers"
RUN dotnet build "Controllers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Controllers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Controllers.dll"]