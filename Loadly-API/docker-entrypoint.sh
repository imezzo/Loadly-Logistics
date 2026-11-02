#!/bin/sh
set -e

echo "========================================="
echo "Loadly Aggregator - Starting Up"
echo "========================================="

# Function to run migrations using EF Core bundle
run_migrations() {
    echo "Running database migrations..."
    
    # Ensure bundle extraction directory exists and is writable
    mkdir -p /app/tmp
    export DOTNET_BUNDLE_EXTRACT_BASE_DIR=/app/tmp
    
    local success=0
    
    # Run ApplicationDbContext migrations
    if [ -f /app/efbundle ]; then
        echo "Applying ApplicationDbContext migrations..."
        if [ -n "$ConnectionStrings__DefaultConnection" ]; then
            # Use environment variable if provided
            if /app/efbundle --connection "$ConnectionStrings__DefaultConnection" --verbose; then
                echo "✓ ApplicationDbContext migrations completed successfully!"
            else
                echo "✗ ApplicationDbContext migration failed!"
                success=1
            fi
        else
            # Read from appsettings.json (bundle will use the configuration)
            if /app/efbundle --verbose; then
                echo "✓ ApplicationDbContext migrations completed successfully!"
            else
                echo "✗ ApplicationDbContext migration failed!"
                success=1
            fi
        fi
    else
        echo "⚠ ApplicationDbContext migration bundle not found, skipping..."
    fi
    
    # Run LogsDbContext migrations
    if [ -f /app/efbundle-logs ]; then
        echo "Applying LogsDbContext migrations..."
        if [ -n "$ConnectionStrings__LogsConnection" ]; then
            # Use environment variable if provided
            if /app/efbundle-logs --connection "$ConnectionStrings__LogsConnection" --verbose; then
                echo "✓ LogsDbContext migrations completed successfully!"
            else
                echo "✗ LogsDbContext migration failed!"
                success=1
            fi
        else
            # Read from appsettings.json (bundle will use the configuration)
            if /app/efbundle-logs --verbose; then
                echo "✓ LogsDbContext migrations completed successfully!"
            else
                echo "✗ LogsDbContext migration failed!"
                success=1
            fi
        fi
    else
        echo "⚠ LogsDbContext migration bundle not found, skipping..."
    fi
    
    return $success
}

# Function to load permissions
# NOTE: This requires the application to support --load permission commands
# Consider moving this to an init container or separate job
load_permissions() {
    echo ""
    echo "========================================="
    echo "Loading Permissions..."
    echo "========================================="
    
    # Load admin permissions
    echo "Loading admin permissions..."
    if dotnet /app/LoadlyAggregator.API.dll --load permission admin; then
        echo "✓ Admin permissions loaded successfully!"
    else
        echo "✗ Failed to load admin permissions"
        return 1
    fi
    
    # Load company permissions
    echo "Loading company permissions..."
    if dotnet /app/LoadlyAggregator.API.dll --load permission company; then
        echo "✓ Company permissions loaded successfully!"
    else
        echo "✗ Failed to load company permissions"
        return 1
    fi
    
    # Load driver permissions
    echo "Loading driver permissions..."
    if dotnet /app/LoadlyAggregator.API.dll --load permission driver; then
        echo "✓ Driver permissions loaded successfully!"
    else
        echo "✗ Failed to load driver permissions"
        return 1
    fi
    
    echo "✓ Permission loading completed!"
    return 0
}

# Check if migrations should be skipped
if [ "$SKIP_MIGRATION" = "true" ]; then
    echo "⚠ Skipping migrations (SKIP_MIGRATION=true)"
else
    # Run migrations (continue even if they fail, but log the error)
    run_migrations || echo "⚠ Migrations completed with errors, continuing..."
fi

# Check if permission loading should be skipped
if [ "$SKIP_PERMISSIONS" = "true" ]; then
    echo "⚠ Skipping permission loading (SKIP_PERMISSIONS=true)"
else
    # Load permissions (continue even if they fail, but log the error)
    load_permissions || echo "⚠ Permission loading completed with errors, continuing..."
fi

echo ""
echo "========================================="
echo "Starting API Application..."
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "URLs: $ASPNETCORE_URLS"
echo "========================================="

# Start the application (exec ensures proper signal handling)
exec dotnet LoadlyAggregator.API.dll
