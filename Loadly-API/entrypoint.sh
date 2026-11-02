#!/bin/sh
set -e

# Determine startup mode based on environment variable
if [ "$FAST_STARTUP" = "true" ]; then
    echo "========================================="
    echo "Loadly Aggregator - FAST Startup Mode"
    echo "========================================="
    STARTUP_MODE="FAST"
else
    echo "========================================="
    echo "Loadly Aggregator - Standard Startup"
    echo "========================================="
    STARTUP_MODE="STANDARD"
fi

apply_context_migrations() {
    context_name=$1
    bundle_path=$2
    connection_string=$3

    if [ ! -f "$bundle_path" ]; then
        echo "MIGRATION_FAILURE: ${context_name} bundle not found at ${bundle_path}"
        return 1
    fi

    echo "Applying ${context_name} migrations..."
    if [ -n "$connection_string" ]; then
        if "$bundle_path" --connection "$connection_string" --verbose; then
            echo "✓ ${context_name} migrations completed successfully!"
            return 0
        fi
    elif "$bundle_path" --verbose; then
        echo "✓ ${context_name} migrations completed successfully!"
        return 0
    fi

    echo "MIGRATION_FAILURE: ${context_name} migration failed (see output above)"
    return 1
}

# Function to run migrations with optimizations
run_migrations() {
    echo "Running database migrations..."

    mkdir -p /app/tmp
    export DOTNET_BUNDLE_EXTRACT_BASE_DIR=/app/tmp

    cd /app

    failed=0
    apply_context_migrations "ApplicationDbContext" "/app/efbundle" "$ConnectionStrings__DefaultConnection" || failed=1
    apply_context_migrations "LogsDbContext" "/app/efbundle-logs" "$ConnectionStrings__LogsConnection" || failed=1

    if [ "$failed" -ne 0 ]; then
        echo "MIGRATION_FAILURE: one or more contexts failed to apply (see output above)"
        return 1
    fi

    echo "MIGRATION_SUCCESS: all contexts applied"
    return 0
}

# Function to seed lookup + demo users (idempotent)
run_seed() {
    echo ""
    echo "========================================="
    echo "Seeding database..."
    echo "========================================="
    cd /app
    if dotnet LoadlyAggregator.API.dll --seed; then
        echo "✓ Database seed completed!"
    else
        echo "⚠ Database seed failed, continuing..."
    fi
}

# Function to load permissions
load_permissions() {
    echo ""
    echo "========================================="  
    echo "Loading Permissions..."
    echo "========================================="
    
    cd /app
    
    if [ "$FAST_STARTUP" = "true" ]; then
        # Ultra-fast mode: parallel loading, output captured per-guard so failures are visible
        echo "Loading all permissions (parallel mode)..."

        mkdir -p /tmp/perm-logs

        dotnet LoadlyAggregator.API.dll --load permission admin >/tmp/perm-logs/admin.log 2>&1 &
        ADMIN_PID=$!
        dotnet LoadlyAggregator.API.dll --load permission company >/tmp/perm-logs/company.log 2>&1 &
        COMPANY_PID=$!
        dotnet LoadlyAggregator.API.dll --load permission driver >/tmp/perm-logs/driver.log 2>&1 &
        DRIVER_PID=$!
        dotnet LoadlyAggregator.API.dll --load permission contractor >/tmp/perm-logs/contractor.log 2>&1 &
        CONTRACTOR_PID=$!
        dotnet LoadlyAggregator.API.dll --load permission collector >/tmp/perm-logs/collector.log 2>&1 &
        COLLECTOR_PID=$!

        # Wait for all background processes to complete. A single guard failing
        # must never kill the entrypoint (and thus the container) under `set -e`.
        for guard_pid in "admin:$ADMIN_PID" "company:$COMPANY_PID" "driver:$DRIVER_PID" "contractor:$CONTRACTOR_PID" "collector:$COLLECTOR_PID"; do
            guard=${guard_pid%%:*}
            pid=${guard_pid##*:}
            if wait "$pid"; then
                echo "✓ $guard permissions loaded successfully!"
            else
                echo "⚠ $guard permission load failed, continuing... (log below)"
                cat "/tmp/perm-logs/$guard.log"
            fi
        done
        echo "✓ Permission loading step completed!"
    else
        # Standard mode: sequential loading with verbose output
        for guard in admin company driver contractor collector; do
            echo "Loading $guard permissions..."
            if dotnet LoadlyAggregator.API.dll --load permission $guard; then
                echo "✓ $guard permissions loaded successfully!"
            else
                echo "⚠ $guard permissions completed with warnings"
            fi
        done
        echo "✓ Permission loading completed!"
    fi
}

# Execute startup sequence
# Seed must run AFTER migrations and BEFORE permissions (admin ACL assignment).
if [ "$FAST_STARTUP" = "true" ]; then
    echo "🚀 FAST STARTUP MODE ENABLED"

    if [ "$SKIP_MIGRATION" != "true" ]; then
        run_migrations || echo "⚠ migration step reported a failure, continuing..."
    fi

    if [ "$SEED_DATABASE" = "true" ]; then
        run_seed
    fi

    if [ "$SKIP_PERMISSIONS" != "true" ]; then
        load_permissions || echo "⚠ permission loading step reported a failure, continuing..."
    fi
else
    # Standard startup - sequential execution
    if [ "$SKIP_MIGRATION" != "true" ]; then
        run_migrations || echo "⚠ migration step reported a failure, continuing..."
    fi

    if [ "$SEED_DATABASE" = "true" ]; then
        run_seed
    fi

    if [ "$SKIP_PERMISSIONS" != "true" ]; then
        load_permissions
    fi
fi

echo ""
echo "========================================="
echo "Starting API Application..."
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "URLs: $ASPNETCORE_URLS"
echo "Startup Mode: $STARTUP_MODE"
echo "========================================="

# Start the application (exec ensures proper signal handling)
exec dotnet LoadlyAggregator.API.dll
