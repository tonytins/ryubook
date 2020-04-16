#!/usr/bin/env bash

if [ "$(uname)" == "Darwin" ]; then
    echo "Publishing macOS release."
    dotnet publish -c Release -r osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true       
elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then
    echo "Publishing Linux release."
    dotnet publish -c Release -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
fi