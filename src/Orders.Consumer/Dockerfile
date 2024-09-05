FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# install OpenTelemetry .NET Automatic Instrumentation
RUN apt-get update && apt-get install -y curl unzip bash
ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/latest/download/otel-dotnet-auto-install.sh otel-dotnet-auto-install.sh
ENV OTEL_DOTNET_AUTO_HOME="/otel-dotnet-auto"
RUN sh otel-dotnet-auto-install.sh

WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Orders.Consumer.dll"]
