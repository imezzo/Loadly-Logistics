import 'package:equatable/equatable.dart';

import '../book_trip/book_trip_request.dart';

class UpdateBookingRequest extends Equatable {
  const UpdateBookingRequest({
    required this.bookingRequestId,
    this.driverId,
    this.driver,
    this.note,
  });

  final int bookingRequestId;
  final int? driverId;
  final BookTripDriver? driver;
  final String? note;

  @override
  List<Object?> get props => [bookingRequestId, driverId, driver, note];
}
