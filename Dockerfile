FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim

WORKDIR /app
COPY ./.output/ ./

RUN curl -sS -o /app/cert.pem https://curl.haxx.se/ca/cacert.pem
ENV KAFKA_JANITOR_SSL_CA_LOCATION=/app/cert.pem

ENTRYPOINT [ "dotnet", "KafkaJanitor.WebApp.dll" ]