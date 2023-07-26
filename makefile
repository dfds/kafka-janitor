IMAGE_NAME=ded/kafka-janitor
BUILD_NUMBER=n/a
OUTPUTDIR=${PWD}/.output
CONFIGURATION=Debug
MAIN_APP_PROJECT_FILE=Infrastructure/KafkaJanitor.RestApi/KafkaJanitor.RestApi.csproj
registry_image_name="579478677147.dkr.ecr.eu-central-1.amazonaws.com/${IMAGE_NAME}:${BUILD_NUMBER}"

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

docker-buildx-setup: 
	docker run --privileged --rm tonistiigi/binfmt --install all
	docker buildx create --name mutiarchbuilder --use
	docker buildx inspect --bootstrap

container: docker-buildx-setup 
	aws ecr get-login-password --region=eu-central-1 | docker login --username AWS --password-stdin 579478677147.dkr.ecr.eu-central-1.amazonaws.com
	docker buildx build --platform linux/amd64,linux/arm64 -t $(registry_image_name) --push .

release: container
	chmod +x ./scripts/push_container_image.sh && ./scripts/push_container_image.sh $(IMAGE_NAME) $(BUILD_NUMBER)