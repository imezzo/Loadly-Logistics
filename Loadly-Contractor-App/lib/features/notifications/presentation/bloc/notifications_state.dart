import 'package:equatable/equatable.dart';

import '../../domain/entities/contractor_notification.dart';

abstract class NotificationsState extends Equatable {
  const NotificationsState();

  @override
  List<Object?> get props => [];
}

class NotificationsInitial extends NotificationsState {
  const NotificationsInitial();
}

class NotificationsLoading extends NotificationsState {
  const NotificationsLoading();
}

class NotificationsLoaded extends NotificationsState {
  const NotificationsLoaded({
    required this.notifications,
    this.error,
    this.page = 1,
    this.total,
    this.pageSize = 15,
    this.startDateFrom,
    this.startDateTo,
  });

  final List<ContractorNotification> notifications;
  final String? error;
  final int page;
  final int? total;
  final int pageSize;
  final String? startDateFrom;
  final String? startDateTo;

  bool get hasMore => total != null && notifications.length < total!;

  NotificationsLoaded copyWith({
    List<ContractorNotification>? notifications,
    String? error,
    int? page,
    int? total,
    int? pageSize,
    String? startDateFrom,
    String? startDateTo,
  }) {
    return NotificationsLoaded(
      notifications: notifications ?? this.notifications,
      error: error,
      page: page ?? this.page,
      total: total ?? this.total,
      pageSize: pageSize ?? this.pageSize,
      startDateFrom: startDateFrom ?? this.startDateFrom,
      startDateTo: startDateTo ?? this.startDateTo,
    );
  }

  @override
  List<Object?> get props => [
    notifications,
    error,
    page,
    total,
    pageSize,
    startDateFrom,
    startDateTo,
  ];
}
