FROM microsoft/dotnet:2.2.1-aspnetcore-runtime-stretch-slim

WORKDIR /app
COPY ./.output/ ./

RUN curl -sS -o /app/cert.pem https://curl.haxx.se/ca/cacert.pem
ENV KAFKA_JANITOR_SSL_CA_LOCATION=/app/cert.pem

ENTRYPOINT [ "dotnet", "KafkaJanitor.WebApp.dll" ]