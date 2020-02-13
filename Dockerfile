FROM mcr.microsoft.com/dotnet/core/sdk:3.1

WORKDIR /app
COPY ./.output/ ./

RUN curl -sS -o /app/cert.pem https://curl.haxx.se/ca/cacert.pem
ENV KAFKA_JANITOR_SSL_CA_LOCATION=/app/cert.pem

ENTRYPOINT [ "dotnet", "KafkaJanitor.RestApi.dll" ]