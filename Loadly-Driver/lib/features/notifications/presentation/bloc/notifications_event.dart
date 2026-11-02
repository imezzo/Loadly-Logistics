import 'package:equatable/equatable.dart';

abstract class NotificationsEvent extends Equatable {
  const NotificationsEvent();

  @override
  List<Object?> get props => [];
}

class NotificationsLoad extends NotificationsEvent {
  const NotificationsLoad({
    this.page,
    this.pageSize,
    this.startDateFrom,
    this.startDateTo,
  });

  final int? page;
  final int? pageSize;
  final String? startDateFrom;
  final String? startDateTo;

  @override
  List<Object?> get props => [page, pageSize, startDateFrom, startDateTo];
}

class NotificationsRefresh extends NotificationsEvent {
  const NotificationsRefresh();
}

class NotificationsLoadMore extends NotificationsEvent {
  const NotificationsLoadMore();
}

class NotificationsMarkAsRead extends NotificationsEvent {
  const NotificationsMarkAsRead({required this.notificationId});

  final int notificationId;

  @override
  List<Object?> get props => [notificationId];
}
