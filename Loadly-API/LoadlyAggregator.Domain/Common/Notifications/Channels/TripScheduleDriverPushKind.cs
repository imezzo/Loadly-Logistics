namespace LoadlyAggregator.Domain.Common.Notifications.Channels;

public enum TripScheduleDriverPushKind
{
    NewAssignment,
    DriverChangedOnTripUpdate,
    ScheduleUpdatedOnTripUpdate,
    VehicleCascadeReassignment
}
