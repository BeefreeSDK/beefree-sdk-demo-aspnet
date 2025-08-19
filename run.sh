#!/bin/bash

echo "Building and running BeeFree SDK Demo..."

# Build and run using Docker Compose
docker-compose up --build

echo "Application should be available at: http://localhost:54067"
echo "API endpoint: http://localhost:54067/api/auth"
