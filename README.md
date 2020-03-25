<!-- ABOUT THE PROJECT -->
# Kafka-janitor

The purpose of this project is to help faciliate Kafka functionality for Capabilities. Whether this be creation of Topics or Service accounts, Kafka-janitor will take care of those actions on behalf of services like [Capability Service](https://github.com/dfds/capability-service).

### How?

Confluent Cloud is a bit of a mess in terms of accessing it's functionality, some functionality is only available using their proprietary binary tool *ccloud* (which doesn't support outputting in a reasonable data schema), with their web interface(which uses a unsupported HTTP API with JSON as its data schema) and *confluent-kafka-dotnet* SDK partially supporting some of the functionality. Therefore we happen to use all 3 mentioned ways to access Confluent Cloud, in order to automate tedious manual tasks.

## Dependencies

The Kafka janitor has the following dependencies:

### Tika

A restful API on top of the ccloud cli  
[Repository](https://github.com/dfds/tika)
A Tika server docker image can be build by running the command `docker build -t tika .` in the `tika/server` folder.

### AWS Systems Manager Parameter Store

Kafka janitor uses AWS parameter store to save the key secrets generated for the users, this dependency can be replaced by an in memory vault by setting the following environment variable `KAFKAJANITOR_VAULT="INMEMORY"`

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple steps.

## Installation
 
1. Clone the repo
```sh
git clone git@github.com:dfds/kafka-janitor.git
```
2. Restore dependencies
```sh
cd kafka-janitor/src
dotnet restore .
```

## Running

### With short feedback loop development

You can run the project with a hot reloader file watcher by completing the following steps:

Start a local instance of Tika in not connected to ccloud mode:

```bash
cd tika/server/
npm install
cd ../local-development
./run-not-connected.sh
```

Start Kafka janitor

```bash
cd kafka-janitor/local-development/
./watch-run.sh
```

You should now be able to make a get request against the services health endpoint and get a `Healthy` response.

```bash
curl --request GET \
  --url http://localhost:5000/Healthz
```

## Interacting with the REST endpoint

We have provided some sample interactions for the REST API endpoint in the folder `kafka-janitor/local-development/`
The interactions ends with `.rest`

The samples require [Visual studio code](https://code.visualstudio.com/#alt-downloads) and the plugin [humao.rest-client](https://github.com/Huachao/vscode-restclient) to run in Visual studio code.

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.