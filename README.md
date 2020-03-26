<!-- ABOUT THE PROJECT -->
# Kafka-janitor

The purpose of this project is to help facilitate Kafka functionality for Capabilities. Whether this be creation of Topics or Service accounts, Kafka-janitor will take care of those actions on behalf of services like [Capability Service](https://github.com/dfds/capability-service).

## How

Confluent Cloud is a bit of a mess in terms of accessing it's functionality, some functionality is only available using their proprietary binary tool *ccloud* (which doesn't support outputting in a reasonable data schema), with their web interface(which uses a unsupported HTTP API with JSON as its data schema) and *confluent-kafka-dotnet* SDK partially supporting some of the functionality.

[Kafka janitor](https://github.com/dfds/kafka-janitor) knows what relations we desire between Kafka topics, access control lists, service accounts and key secrets. It also understand the domain language used by the rest of our self service platform.

[Tika](https://github.com/dfds/tika) wraps the ccloud command line interface and exposes a REST HTTP interface that the Kafka janitor uses to instruct Confluent Cloud about what artifacts to create or delete.

[ccloud CLI](https://docs.confluent.io/current/cloud/cli/index.html) is a command lind tool that interacts with Confluent cloud. It can do more [interactions with Confluent Cloud](https://docs.confluent.io/current/cloud/cli/command-reference/index.html) than the current SDK


[Confluent Cloud](https://docs.confluent.io/current/cloud/cloud-start.html) a managed Apache Kafka cluster

```
+-----------------+                 
|  Kafka janitor  |                 
+-----------------+                 
         |                          
         v                          
+-----------------+                 
|       Tika      |                 
+-----------------+                 
         |                          
         v                          
+-----------------+                 
|    ccloud CLI   |                 
+-----------------+                 
         |                          
         v                          
+-----------------+                 
| Confluent Cloud |                 
+-----------------+
```

## Dependencies

The Kafka janitor has the following dependencies:

### Tika

A restful HTTP API on top of the ccloud cli  
[Repository](https://github.com/dfds/tika)  
A Tika server docker image can be build by running the command `docker build -t tika .` in the `tika/server` folder.

### AWS Systems Manager Parameter Store

Kafka janitor uses AWS parameter store to save the key secrets generated for the users, this dependency can be replaced by an in memory vault by setting the following environment variable `KAFKAJANITOR_VAULT="INMEMORY"`

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple steps.

## Installation

### Prerequisites

* [Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)

Clone the repository

```shell
git clone git@github.com:dfds/kafka-janitor.git
```

Restore dependencies

```shell
cd kafka-janitor/src
dotnet restore .
```

## Running

### With short feedback loop development

#### Prerequisites

* [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) a set of libraries and tools that allow developers to create . NET Core applications and libraries.
* [curl](https://curl.haxx.se/download.html) a command line tool for transferring data with URLs.
* [Unix shell](https://en.wikipedia.org/wiki/Unix_shell) a command-line interpreter that provides a command line user interface for Unix-like operating systems.

You can run the project with a hot reloader file watcher by completing the following steps:

Start a local instance of Tika in not connected to ccloud mode:

```shell
cd tika/server/
npm install
cd ../local-development
./run-not-connected.sh
```

Start Kafka janitor

```shell
kafka-janitor/local-development/watch-run.sh
```

You should now be able to make a get request against the services health endpoint and get a `Healthy` response.

```shell
curl --request GET \
  --url http://localhost:5000/Healthz
```

### In a local cluster

#### Prerequisites

* [Docker](https://docs.docker.com/install/) a tool designed to make it easier to create, deploy, and run applications by using containers.
* a local Kubernetes cluster, this could run in: [Minikube](https://kubernetes.io/docs/setup/learning-environment/minikube/), [Microk8s](https://microk8s.io/), [Kind](https://kind.sigs.k8s.io/) or [K3S](https://k3s.io/)
* [kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/) a command line tool for controlling Kubernetes clusters.
* [Kustomize](https://github.com/kubernetes-sigs/kustomize/blob/master/docs/INSTALL.md) a standalone tool to customize Kubernetes objects through a kustomization file.  
* [curl](https://curl.haxx.se/download.html) a command line tool for transferring data with URLs.
* [Unix shell](https://en.wikipedia.org/wiki/Unix_shell) a command-line interpreter that provides a command line user interface for Unix-like operating systems.

#### Running in the local cluster

Build the docker images for `kafka-janitor` and `tika`:

```shell
cd kafka-janitor/
docker build -t ded/kafka-janitor .
```

```shell
cd tika/server/
docker build -t ded/tika .
```

Make sure your images are in your local clusters registry, otherwise you will get a `repository does not exist or may require 'docker login'` error when starting the pods in Kubernetes.

Point your `kubectl` to your local cluster. via the command: `kubectl config use-context [your-local-cluster-context]`

Deploy the services to your cluster via kubectl:

```shell
kafka-janitor/local-development/deploy-to-local-cluster.sh
```

You should now be able to access the Kafka janitor running in kubernetes by port forwarding into it, and checking its health:

```shell
kubectl -n selfservice port-forward service/kafka-janitor 5000:80
curl --request GET --url http://localhost:5000/Healthz
```

## Interacting with the REST endpoint

#### Prerequisites

* [Visual studio code](https://code.visualstudio.com/#alt-downloads) a extendable code editor with support for: debugging, version control and much more.
* [humao.rest-client](https://github.com/Huachao/vscode-restclient) a extension that allows you to send HTTP request from Visual studio code.

We have provided some sample interactions for the REST API endpoint in the folder `kafka-janitor/local-development/`
The interactions ends with `.rest`

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

Distributed under the MIT License.
