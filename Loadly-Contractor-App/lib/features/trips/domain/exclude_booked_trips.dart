import 'entities/booking_request/booking_request.dart';
import 'entities/trip/trip.dart';

Set<int> bookedTripIds(Iterable<BookingRequest> bookings) {
  return {
    for (final booking in bookings) ...[
      if (booking.tripId != null) booking.tripId!,
      if (booking.trip?.id != null) booking.trip!.id!,
    ],
  };
}

List<Trip> excludeBookedTrips(
  List<Trip> trips,
  Iterable<BookingRequest> bookings,
) {
  final bookedIds = bookedTripIds(bookings);
  if (bookedIds.isEmpty) return trips;
  return [
    for (final trip in trips)
      if (trip.id == null || !bookedIds.contains(trip.id)) trip,
  ];
}

int requestedCountExcludingBooked({
  required List<Trip> trips,
  required int totalCount,
  required Iterable<BookingRequest> bookings,
}) {
  final requestedTrips = trips.where((trip) => trip.status == 1).toList();
  final hidden =
      requestedTrips.length - excludeBookedTrips(requestedTrips, bookings).length;
  final count = requestedTrips.length - hidden;
  return count < 0 ? 0 : count;
}

List<Trip> requestedTrips(List<Trip> trips) {
  return trips.where((trip) => trip.status == 1).toList();
}
