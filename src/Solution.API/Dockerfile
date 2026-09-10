FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .

COPY Solution.API/Solution.API.csproj Solution.API/
COPY BuildingBlocks/Solution.BuildingBlocks.Core/Solution.BuildingBlocks.Core.csproj BuildingBlocks/Solution.BuildingBlocks.Core/
COPY BuildingBlocks/Solution.BuildingBlocks.Infrastructure/Solution.BuildingBlocks.Infrastructure.csproj BuildingBlocks/Solution.BuildingBlocks.Infrastructure/
COPY Modules/UniLeague/Solution.UniLeague.API/Solution.UniLeague.API.csproj Modules/UniLeague/Solution.UniLeague.API/
COPY Modules/UniLeague/Solution.UniLeague.Core/Solution.UniLeague.Core.csproj Modules/UniLeague/Solution.UniLeague.Core/
COPY Modules/UniLeague/Solution.UniLeague.Infrastructure/Solution.UniLeague.Infrastructure.csproj Modules/UniLeague/Solution.UniLeague.Infrastructure/
COPY Modules/Identity/Solution.Identity.API/Solution.Identity.API.csproj Modules/Identity/Solution.Identity.API/
COPY Modules/Identity/Solution.Identity.Core/Solution.Identity.Core.csproj Modules/Identity/Solution.Identity.Core/
COPY Modules/Identity/Solution.Identity.Infrastructure/Solution.Identity.Infrastructure.csproj Modules/Identity/Solution.Identity.Infrastructure/

RUN dotnet restore Solution.API/Solution.API.csproj

COPY . .

RUN dotnet publish Solution.API/Solution.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Solution.API.dll"]