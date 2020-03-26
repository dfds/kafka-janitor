FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app-build
COPY ./src ./

RUN dotnet restore && dotnet build -c Release
RUN dotnet publish --no-build -c Release -o /app Infrastructure/KafkaJanitor.RestApi/KafkaJanitor.RestApi.csproj

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS run

WORKDIR /app
COPY --from=build /app /app/
RUN curl -sS -o /app/cert.pem https://curl.haxx.se/ca/cacert.pem
ENV KAFKA_JANITOR_SSL_CA_LOCATION=/app/cert.pem

ENTRYPOINT [ "dotnet", "KafkaJanitor.RestApi.dll" ]