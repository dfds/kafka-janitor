IMAGE_NAME=ded/kafka-janitor
BUILD_NUMBER=n/a
OUTPUTDIR=${PWD}/.output
CONFIGURATION=Debug
MAIN_APP_PROJECT_FILE=Infrastructure/KafkaJanitor.RestApi/KafkaJanitor.RestApi.csproj

NEW_MAIN_APP_PROJECT_FILE=KafkaJanitor.App/KafkaJanitor.App.csproj

init: restore build

clean:
	@rm -Rf $(OUTPUTDIR)
	@mkdir $(OUTPUTDIR)
	@cd src && dotnet clean -v d --configuration Debug
	@cd src && dotnet clean -v d --configuration Release

restore:
	@cd src && dotnet restore -v q

build:
	@cd src && dotnet build -c $(CONFIGURATION)

run:
	cd src && dotnet run --project $(NEW_MAIN_APP_PROJECT_FILE)

dev:
	cd src && dotnet watch --no-hot-reload --project $(NEW_MAIN_APP_PROJECT_FILE) run

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