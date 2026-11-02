import 'package:dartz/dartz.dart';

import '../../../../core/errors/api_fallback_keys.dart';
import '../../../../core/errors/failure.dart';
import '../../../../core/network/repository_api_runner.dart';
import '../../domain/entities/booking_request/booking_request.dart';
import '../../domain/entities/trip/trip.dart';
import '../../domain/entities/trip/trip_statistics.dart';
import '../../domain/entities/trip/trips_list_response.dart';
import '../../domain/repositories/trip_repository.dart';
import '../datasources/trip_remote_datasource.dart';
import '../mappers/booking_request_mapper.dart';
import '../mappers/trip_mapper.dart';
import '../models/paged_list.dart';

class TripRepositoryImpl implements TripRepository {
  TripRepositoryImpl(this._remoteDataSource);

  final TripRemoteDataSource _remoteDataSource;

  @override
  Future<Either<Failure, Trip>> getTrip(int tripId) {
    return runApiCall(() async {
      final result = await _remoteDataSource.getTrip(tripId);
      return result.toEntity();
    }, fallbackMessage: ApiFallbackKeys.loadTrip);
  }

  @override
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
  }) {
    return runApiCall(() async {
      final result = await _remoteDataSource.getTrips(
        page: page,
        pageSize: pageSize,
        status: status,
        statuses: statuses,
        openInviteOnly: openInviteOnly,
        search: search,
        vehicleTypeId: vehicleTypeId,
        startDateFrom: startDateFrom,
        startDateTo: startDateTo,
        isContractorPaid: isContractorPaid,
        hasContractor: hasContractor,
      );
      return result.toEntity();
    }, fallbackMessage: ApiFallbackKeys.loadTrips);
  }

  @override
  Future<Either<Failure, PagedList<BookingRequest>>> getBookingRequests({
    int page = 1,
    int pageSize = 10,
    int? status,
    String? search,
    int? vehicleTypeId,
    String? startDateFrom,
    String? startDateTo,
  }) {
    return runApiCall(() async {
      final result = await _remoteDataSource.getBookingRequests(
        page: page,
        pageSize: pageSize,
        status: status,
        search: search,
        vehicleTypeId: vehicleTypeId,
        startDateFrom: startDateFrom,
        startDateTo: startDateTo,
      );
      return PagedList(
        items: result.items.map((e) => e.toEntity()).toList(),
        totalCount: result.totalCount,
      );
    }, fallbackMessage: ApiFallbackKeys.loadBookingRequests);
  }

  @override
  Future<Either<Failure, void>> bookTrip(int tripId) {
    return runApiCall(
      () => _remoteDataSource.bookTrip(tripId: tripId),
      fallbackMessage: ApiFallbackKeys.bookTrip,
    );
  }

  @override
  Future<Either<Failure, void>> withdrawTrip(int tripId) {
    return runApiCall(
      () => _remoteDataSource.withdrawTrip(tripId: tripId),
      fallbackMessage: ApiFallbackKeys.withdrawBooking,
    );
  }

  @override
  Future<Either<Failure, InvitationStatistics>> getInvitationStatistics() {
    return runApiCall(() async {
      final result = await _remoteDataSource.getInvitationStatistics();
      return result.toEntity();
    }, fallbackMessage: ApiFallbackKeys.loadTrips);
  }

  @override
  Future<Either<Failure, TripStatusStatistics>> getStatusStatistics() {
    return runApiCall(() async {
      final result = await _remoteDataSource.getStatusStatistics();
      return result.toEntity();
    }, fallbackMessage: ApiFallbackKeys.loadTrips);
  }

  @override
  Future<Either<Failure, PayoutStatistics>> getPayoutStatistics() {
    return runApiCall(() async {
      final result = await _remoteDataSource.getPayoutStatistics();
      return result.toEntity();
    }, fallbackMessage: ApiFallbackKeys.loadTrips);
  }
}
