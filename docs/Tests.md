## Intro

The tests expects certain environment variables to order to run properly.

* KAFKA_JANITOR_BOOTSTRAP_SERVERS (e.g. '127.0.0.1:9092')
* KAFKA_JANITOR_GROUP_ID (e.g. 'build.selfservice.kafka-janitor-consumer')
* KAFKA_JANITOR_BROKER_VERSION_FALLBACK (e.g. '0.10.0.0')
* KAFKA_JANITOR_API_VERSION_FALLBACK_MS (e.g. '0')
* KAFKA_JANITOR_SASL_MECHANISMS (e.g. 'PLAIN')
* KAFKA_JANITOR_SECURITY_PROTOCOL (e.g. 'SASL_SSL')
* KAFKA_JANITOR_SASL_USERNAME (e.g. 'root')
* KAFKA_JANITOR_SASL_PASSWORD (e.g. 'hunter2')