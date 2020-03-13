#!/bin/bash

case "$OSTYPE" in
  darwin)  platform=darwin ;; 
  linux)   platform=linux ;;
  #msys*)    platform=WINDOWS ;;
  *)        (exit 1) ;;
esac

manifest=$1 # First parameter is kubernetes manifest to scan
strict=0

if [ ! -z "$2" ]
then
    strict=$2
fi

RC=1

curl -o kube-advisor https://alcide.blob.core.windows.net/generic/DCV-2.1/$platform/advisor
chmod +x ./kube-advisor

RC=$(./kube-advisor validate file $manifest --eula-sign --outfile output.html | awk '/Critical/{print $4}')

if [ $strict -gt 0 ]
then
RC2=$(./kube-advisor validate file $manifest --eula-sign --outfile output.html | awk '/High/{print $4}')
RC=$(($RC + $RC2))
fi

./kube-advisor validate file $manifest --eula-sign --output yaml

if [ $RC -gt 0 ]
then
    >&2 echo "Scan failed see output"
    (exit 1)
fi