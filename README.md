<!-- ABOUT THE PROJECT -->
## Kafka-janitor

The purpose of this project is to help faciliate Kafka functionality for Capabilities. Whether this be creation of Topics or Service accounts, Kafka-janitor will take care of those actions on behalf of services like [Capability Service](https://github.com/dfds/capability-service).

### How?

Confluent Cloud is a bit of a mess in terms of accessing it's functionality, some functionality is only available using their proprietary binary tool *ccloud* (which doesn't support outputting in a reasonable data schema), with their web interface(which uses a unsupported HTTP API with JSON as its data schema) and *confluent-kafka-dotnet* SDK partially supporting some of the functionality. Therefore we happen to use all 3 mentioned ways to access Confluent Cloud, in order to automate tedious manual tasks.

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple steps.

### Installation
 
1. Clone the repo
```sh
git clone git@github.com:dfds/kafka-janitor.git
```
2. Restore dependencies
```sh
cd kafka-janitor/src
dotnet restore .
```


<!-- USAGE EXAMPLES -->
## Usage

Kafka-janitor expects certain environment variables to be configured before running

* KAFKA_JANITOR_BOOTSTRAP_SERVERS -> '127.0.0.1:9092'
* KAFKA_JANITOR_GROUP_ID -> 'kafka-janitor-consumer'
* KAFKA_JANITOR_BROKER_VERSION_FALLBACK -> '0.10.0.0'
* KAFKA_JANITOR_API_VERSION_FALLBACK_MS -> '0'
* KAFKA_JANITOR_SASL_MECHANISMS -> 'PLAIN'
* KAFKA_JANITOR_SECURITY_PROTOCOL -> 'SASL_SSL'
* KAFKA_JANITOR_SASL_USERNAME -> 'key'
* KAFKA_JANITOR_SASL_PASSWORD -> 'secret


These environment variables will be mapped to the default Kafka configuration keys, so if you're already familiar with using Kafka, you may have seen e.g. 'bootstrap.servers' and 'group.id' before, they're exactly as shown above, so setting *KAFKA_JANITOR_BOOTSTRAP_SERVERS* will make Kafka janitor read it as *bootstrap.servers* and pass it along to the *confluent-kafka-dotnet* SDK. With that in mind, the above example may not be enough, or may be too much for your given Kafka setup, please take a look at [Producer configs](https://kafka.apache.org/documentation/#producerconfigs), [Consumer config](https://kafka.apache.org/documentation/#consumerconfigs) and [Connect config](https://kafka.apache.org/documentation/#connectconfigs) for possible ways to configure the application.

The integration tests for Kafka-janitor also expects certain environment variables. If one intends to use the included Docker Compose setup for spinning up a local Kafka setup, the following can be used for tests:

* KAFKA_JANITOR_BOOTSTRAP_SERVERS -> '127.0.0.1:29092'
* KAFKA_JANITOR_GROUP_ID -> build.selfservice.kafka-janitor-consumer
* KAFKA_JANITOR_BROKER_VERSION_FALLBACK -> '0.10.0.0'
* KAFKA_JANITOR_API_VERSION_FALLBACK_MS -> '0'
* KAFKA_JANITOR_SASL_MECHANISMS -> 'PLAIN'



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