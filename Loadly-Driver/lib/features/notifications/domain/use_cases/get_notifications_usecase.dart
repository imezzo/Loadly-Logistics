import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/notifications_list_result.dart';
import '../failures/notifications_failure.dart';
import '../repositories/notifications_repository.dart';

class GetNotificationsUseCase
    implements UseCase<NotificationsListResult, GetNotificationsParams> {
  GetNotificationsUseCase(this._repository);

  final NotificationsRepository _repository;

  @override
  Future<Either<NotificationsFailure, NotificationsListResult>> call(
    GetNotificationsParams params,
  ) {
    return _repository.getNotifications(
      page: params.page,
      pageSize: params.pageSize,
      startDateFrom: params.startDateFrom,
      startDateTo: params.startDateTo,
    );
  }
}

class GetNotificationsParams {
  const GetNotificationsParams({
    this.page,
    this.pageSize,
    this.startDateFrom,
    this.startDateTo,
  });

  final int? page;
  final int? pageSize;
  final String? startDateFrom;
  final String? startDateTo;
}
