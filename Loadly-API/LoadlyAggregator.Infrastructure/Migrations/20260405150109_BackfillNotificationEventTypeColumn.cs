using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadlyAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillNotificationEventTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE notifications
                SET notification_event_type = CASE event_type
                    WHEN 'trip_requested'      THEN 0
                    WHEN 'trip_scheduled'      THEN 1
                    WHEN 'trip_cancelled'      THEN 2
                    WHEN 'trip_completed'      THEN 3
                    WHEN 'trip_request_admins' THEN 4
                    WHEN 'trip_started'        THEN 5
                    WHEN 'waypoint_arrived'    THEN 6
                    WHEN 'loading_started'     THEN 7
                    WHEN 'loading_completed'   THEN 8
                    WHEN 'unloading_started'   THEN 9
                    WHEN 'unloading_completed' THEN 10
                    WHEN 'waypoint_departed'   THEN 11
                END
                WHERE notification_event_type IS NULL
                  AND event_type IN (
                    'trip_requested', 'trip_scheduled', 'trip_cancelled', 'trip_completed',
                    'trip_request_admins', 'trip_started', 'waypoint_arrived',
                    'loading_started', 'loading_completed', 'unloading_started',
                    'unloading_completed', 'waypoint_departed'
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE notifications
                SET notification_event_type = NULL
                WHERE notification_event_type IS NOT NULL;
            ");
        }
    }
}
