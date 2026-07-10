FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore ConsensusService/ConsensusService.csproj && dotnet publish ConsensusService/ConsensusService.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet","ConsensusService.dll"]