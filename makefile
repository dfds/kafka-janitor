CONFIGURATIOBN=Debug

init: restore build

restore:
	cd src && dotnet restore

build:
	cd src && dotnet build -c $(CONFIGURATIOBN)

run:
	cd src && dotnet run --project KafkaJanitor.WebApp/