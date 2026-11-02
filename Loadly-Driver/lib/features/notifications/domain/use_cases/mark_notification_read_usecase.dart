import 'package:dartz/dartz.dart';

import '../failures/notifications_failure.dart';
import '../repositories/notifications_repository.dart';

class MarkNotificationReadUseCase {
  MarkNotificationReadUseCase(this._repository);

  final NotificationsRepository _repository;

  Future<Either<NotificationsFailure, void>> call(int notificationId) {
    return _repository.markAsRead(notificationId);
  }
}
