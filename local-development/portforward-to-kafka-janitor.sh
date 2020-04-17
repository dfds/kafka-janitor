#!/bin/bash

if [ "$1" == "stop" ]; then
    kill -9 $(lsof -i:a
 -t)
    echo "Forwarding stopped"
else
    kubectl -n selfservice port-forward service/kafka-janitor 5000:80 &
fi

