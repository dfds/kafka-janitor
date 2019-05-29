IMAGE_NAME=ded/kafka-janitor
BUILD_NUMBER=n/a
OUTPUTDIR=${PWD}/.output
CONFIGURATION=Debug
MAIN_APP_PROJECT_FILE=KafkaJanitor.WebApp/KafkaJanitor.WebApp.csproj

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

container: CONFIGURATION=Release
container: package
	docker build -t $(IMAGE_NAME) .

release: container
	chmod +x ./scripts/push_container_image.sh && ./scripts/push_container_image.sh $(IMAGE_NAME) $(BUILD_NUMBER)