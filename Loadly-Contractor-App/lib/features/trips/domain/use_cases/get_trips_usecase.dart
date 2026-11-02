import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/usecase/usecase.dart';
import '../entities/trip/trips_list_response.dart';
import '../repositories/trip_repository.dart';

class GetTripsParams {
  const GetTripsParams({
    this.page = 1,
    this.pageSize = 10,
    this.status,
    this.statuses,
    this.openInviteOnly,
    this.search,
    this.vehicleTypeId,
    this.startDateFrom,
    this.startDateTo,
    this.isContractorPaid,
    this.hasContractor,
  });

  final int page;
  final int pageSize;
  final int? status;
  final List<int>? statuses;
  final bool? openInviteOnly;
  final String? search;
  final int? vehicleTypeId;
  final String? startDateFrom;
  final String? startDateTo;
  final bool? isContractorPaid;
  final bool? hasContractor;
}

class GetTripsUseCase implements UseCase<TripsListResponse, GetTripsParams> {
  GetTripsUseCase(this._repository);

  final TripRepository _repository;

  @override
  Future<Either<Failure, TripsListResponse>> call(GetTripsParams params) {
    return _repository.getTrips(
      page: params.page,
      pageSize: params.pageSize,
      status: params.status,
      statuses: params.statuses,
      openInviteOnly: params.openInviteOnly,
      search: params.search,
      vehicleTypeId: params.vehicleTypeId,
      startDateFrom: params.startDateFrom,
      startDateTo: params.startDateTo,
      isContractorPaid: params.isContractorPaid,
      hasContractor: params.hasContractor,
    );
  }
}
