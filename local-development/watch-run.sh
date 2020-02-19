#!/bin/bash
ASPNETCORE_ENVIRONMENT="Development" \
TIKA_API_ENDPOINT="http://localhost:3000" \
dotnet watch --project ../src/Infrastructure/KafkaJanitor.RestApi/KafkaJanitor.RestApi.csproj \
run --urls "http://*:5000" 
