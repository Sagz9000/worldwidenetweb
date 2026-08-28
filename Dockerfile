# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/AssetPortal.Web/AssetPortal.Web.csproj .
RUN dotnet restore

COPY src/AssetPortal.Web/ .
RUN dotnet publish -c Release -o /out --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "AssetPortal.Web.dll"]
