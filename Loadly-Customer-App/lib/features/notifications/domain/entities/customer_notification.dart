/// Domain entity for a Customer notification.
/// Named CustomerNotification to avoid conflict with Flutter's Notification class.
class CustomerNotification {
  const CustomerNotification({
    required this.id,
    required this.title,
    required this.body,
    required this.eventType,
    required this.resourceType,
    required this.resourceId,
    required this.clickAction,
    required this.isRead,
    required this.readAt,
    required this.createdAt,
    required this.updatedAt,
  });

  final int id;
  final String title;
  final String body;
  final int eventType;
  final String resourceType;
  final int resourceId;
  final String clickAction;
  final bool isRead;
  final String readAt;
  final DateTime createdAt;
  final String updatedAt;
}
