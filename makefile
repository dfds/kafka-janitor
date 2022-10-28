IMAGE_NAME=ded/kafka-janitor
BUILD_NUMBER=n/a
OUTPUTDIR=${PWD}/.output
CONFIGURATION=Debug
MAIN_APP_PROJECT_FILE=Infrastructure/KafkaJanitor.RestApi/KafkaJanitor.RestApi.csproj

init: restore build

clean:
	rm -Rf $(OUTPUTDIR)
	mkdir $(OUTPUTDIR)
	cd src && dotnet clean -v q

restore:
	cd src && dotnet restore

build:
	cd src && dotnet build -c $(CONFIGURATION)

run:
	cd src && dotnet run --project $(MAIN_APP_PROJECT_FILE)

package: clean restore build
	cd src && dotnet publish --no-build -o $(OUTPUTDIR) -c $(CONFIGURATION) $(MAIN_APP_PROJECT_FILE)

container: 
	docker build -t $(IMAGE_NAME) .

release: container
	chmod +x ./scripts/push_container_image.sh && ./scripts/push_container_image.sh $(IMAGE_NAME) $(BUILD_NUMBER)

newmigration:
	@echo "Enter name of migration (e.g. add-table-for-xxx):" && read && \
		echo "-- $(shell date +'%4Y-%m-%d %H:%M:%S') : $$REPLY" > db/migrations/$(shell date +'%4Y%m%d%H%M%S')_$$REPLY.sql

nm: newmigration