FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore IngestionService/IngestionService.csproj && dotnet publish IngestionService/IngestionService.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet","IngestionService.dll"]