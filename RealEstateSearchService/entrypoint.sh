#!/bin/bash

host="db"  # The hostname of your PostgreSQL container/service
port="5432"  # The port PostgreSQL is running on (default is 5432)

# Wait for PostgreSQL to be ready
echo "Waiting for PostgreSQL to be ready..."
until nc -z $host $port
do
  echo "PostgreSQL is unavailable - sleeping"
  sleep 1
done

echo "PostgreSQL is up - executing command"


dotnet RealEstateSearchService.dll
