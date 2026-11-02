import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../data/models/paged_list.dart';
import '../entities/booking_request/booking_request.dart';
import '../entities/trip/trip.dart';
import '../entities/trip/trip_statistics.dart';
import '../entities/trip/trips_list_response.dart';

abstract interface class TripRepository {
  Future<Either<Failure, Trip>> getTrip(int tripId);

  Future<Either<Failure, TripsListResponse>> getTrips({
    int page = 1,
    int pageSize = 10,
    int? status,
    List<int>? statuses,
    bool? openInviteOnly,
    String? search,
    int? vehicleTypeId,
    String? startDateFrom,
    String? startDateTo,
    bool? isContractorPaid,
    bool? hasContractor,
  });

  Future<Either<Failure, PagedList<BookingRequest>>> getBookingRequests({
    int page = 1,
    int pageSize = 10,
    int? status,
    String? search,
    int? vehicleTypeId,
    String? startDateFrom,
    String? startDateTo,
  });

  Future<Either<Failure, void>> bookTrip(int tripId);

  Future<Either<Failure, void>> withdrawTrip(int tripId);

  Future<Either<Failure, InvitationStatistics>> getInvitationStatistics();

  Future<Either<Failure, TripStatusStatistics>> getStatusStatistics();

  Future<Either<Failure, PayoutStatistics>> getPayoutStatistics();
}
