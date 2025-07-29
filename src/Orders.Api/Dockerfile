FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar arquivos de projeto e dependências compartilhadas
COPY src/Orders.Api/*.csproj ./src/Orders.Api/
COPY src/Shared.Correlation/*.csproj ./src/Shared.Correlation/
COPY src/Shared.Correlation.OpenTelemetry/*.csproj ./src/Shared.Correlation.OpenTelemetry/

# Restaurar dependências
WORKDIR /app/src/Orders.Api
RUN dotnet restore

# Copiar código fonte
WORKDIR /app
COPY src/ ./src/

# Publicar aplicação
WORKDIR /app/src/Orders.Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Orders.Api.dll"]
