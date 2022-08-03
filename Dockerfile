FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app-build
COPY ./src ./

RUN dotnet restore && dotnet build -c Release
RUN dotnet publish --no-build -c Release -o /app Infrastructure/KafkaJanitor.RestApi/KafkaJanitor.RestApi.csproj

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS run

WORKDIR /app
COPY --from=build /app /app/
RUN curl -sS -o /app/cert.pem https://curl.se/ca/cacert.pem
ENV KAFKA_JANITOR_SSL_CA_LOCATION=/app/cert.pem

#Non-root user settings
ARG USERNAME=kafkaJanitor
ARG USER_UID=1000
ARG USER_GID=$USER_UID

RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME 


USER $USERNAME

ENTRYPOINT [ "dotnet", "KafkaJanitor.RestApi.dll" ]
