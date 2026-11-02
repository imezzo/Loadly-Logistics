import 'package:dartz/dartz.dart';

import '../entities/notifications_list_result.dart';
import '../failures/notifications_failure.dart';

abstract interface class NotificationsRepository {
  Future<Either<NotificationsFailure, NotificationsListResult>>
  getNotifications({
    int? page,
    int? pageSize,
    String? startDateFrom,
    String? startDateTo,
  });
  Future<Either<NotificationsFailure, void>> markAsRead(int notificationId);
}
