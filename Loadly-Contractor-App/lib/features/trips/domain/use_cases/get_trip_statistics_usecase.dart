import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/usecase/usecase.dart';
import '../entities/trip/trip_statistics.dart';
import '../repositories/trip_repository.dart';

class GetInvitationStatisticsUseCase
    implements UseCaseNoParams<InvitationStatistics> {
  GetInvitationStatisticsUseCase(this._repository);

  final TripRepository _repository;

  @override
  Future<Either<Failure, InvitationStatistics>> call() {
    return _repository.getInvitationStatistics();
  }
}

class GetStatusStatisticsUseCase
    implements UseCaseNoParams<TripStatusStatistics> {
  GetStatusStatisticsUseCase(this._repository);

  final TripRepository _repository;

  @override
  Future<Either<Failure, TripStatusStatistics>> call() {
    return _repository.getStatusStatistics();
  }
}

class GetPayoutStatisticsUseCase implements UseCaseNoParams<PayoutStatistics> {
  GetPayoutStatisticsUseCase(this._repository);

  final TripRepository _repository;

  @override
  Future<Either<Failure, PayoutStatistics>> call() {
    return _repository.getPayoutStatistics();
  }
}
