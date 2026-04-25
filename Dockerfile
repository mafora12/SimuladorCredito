# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out
RUN dotnet restore SimuladorCreditoWeb/SimuladorCreditoWeb.csproj
RUN dotnet publish SimuladorCreditoWeb/SimuladorCreditoWeb.csproj -c Release -o out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out ./

EXPOSE 10000

ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "SimuladorCreditoWeb.dll"]
