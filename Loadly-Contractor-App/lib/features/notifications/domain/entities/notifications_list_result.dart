import 'contractor_notification.dart';

/// Result of a paginated notifications list request.
class NotificationsListResult {
  const NotificationsListResult({
    required this.notifications,
    this.total,
    this.page = 1,
    this.pageSize = 15,
  });

  final List<ContractorNotification> notifications;
  final int? total;
  final int page;
  final int pageSize;

  bool get hasMore => total != null && notifications.length < total!;
}
