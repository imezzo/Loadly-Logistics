import 'package:equatable/equatable.dart';

import '../../../../trips/domain/entities/book_trip/book_trip_request.dart';

class UpdateTripDriver extends Equatable {
  const UpdateTripDriver({
    required this.tripId,
    this.driverId,
    this.driver,
    this.note,
  });

  final int tripId;
  final int? driverId;
  final BookTripDriver? driver;
  final String? note;

  @override
  List<Object?> get props => [tripId, driverId, driver, note];
}
