#!/bin/bash

# Start the View.Webapi project
echo "Starting View.Webapi project..."
dotnet run --project src/View.Webapi/View.Webapi.csproj --launch-profile https &

# Wait for a moment (adjust the sleep duration as needed)
sleep 5

# Start the View.React project
echo "Starting View.React project..."
dotnet run --project src/View.React/View.React.csproj
