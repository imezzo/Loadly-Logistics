#!/bin/bash

# Database Seeding Script for Loadly Aggregator
# This script seeds the database with initial data

echo "🌱 Starting database seeding..."
echo "================================"

cd "$(dirname "$0")/LoadlyAggregator.API"

# Run migrations first
echo "📊 Applying database migrations..."
dotnet ef database update

if [ $? -ne 0 ]; then
    echo "❌ Migration failed. Please check your database connection."
    exit 1
fi

# Seed the database
echo "🌱 Seeding database..."
dotnet run --seed

if [ $? -eq 0 ]; then
    echo "✅ Database seeding completed successfully!"
else
    echo "❌ Database seeding failed. Check the error messages above."
    exit 1
fi
