import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/usecase/usecase.dart';
import '../../data/models/paged_list.dart';
import '../entities/booking_request/booking_request.dart';
import '../repositories/trip_repository.dart';

class GetBookingRequestsParams {
  GetBookingRequestsParams({
    this.page = 1,
    this.pageSize = 10,
    this.status,
    this.search,
    this.vehicleTypeId,
    this.startDateFrom,
    this.startDateTo,
  });

  final int page;
  final int pageSize;
  final int? status;
  final String? search;
  final int? vehicleTypeId;
  final String? startDateFrom;
  final String? startDateTo;
}

class GetBookingRequestsUseCase
    implements UseCase<PagedList<BookingRequest>, GetBookingRequestsParams> {
  GetBookingRequestsUseCase(this._repository);

  final TripRepository _repository;

  @override
  Future<Either<Failure, PagedList<BookingRequest>>> call(
    GetBookingRequestsParams params,
  ) {
    return _repository.getBookingRequests(
      page: params.page,
      pageSize: params.pageSize,
      status: params.status,
      search: params.search,
      vehicleTypeId: params.vehicleTypeId,
      startDateFrom: params.startDateFrom,
      startDateTo: params.startDateTo,
    );
  }
}
