#!/bin/bash

if [ "$1" == "stop" ]; then
    kill -9 $(lsof -i:a
 -t)
    echo "Forwarding stopped"
else
    kubectl -n selfservice port-forward deployment/kafka-janitor 3000
fi

