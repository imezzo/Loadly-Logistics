class NotificationDto {
  const NotificationDto({
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
  final String createdAt;
  final String updatedAt;

  factory NotificationDto.fromJson(Map<String, dynamic> json) =>
      NotificationDto(
        id: json['id'] as int? ?? 0,
        title: json['title'] as String? ?? '',
        body: json['body'] as String? ?? '',
        eventType: json['eventType'] as int? ?? 0,
        resourceType: json['resourceType'] as String? ?? '',
        resourceId: json['resourceId'] as int? ?? 0,
        clickAction: json['clickAction'] as String? ?? '',
        isRead: json['isRead'] as bool? ?? false,
        readAt: json['readAt'] as String? ?? '',
        createdAt: json['createdAt'] as String? ?? '',
        updatedAt: json['updatedAt'] as String? ?? '',
      );
}

/// Paginated response for notifications list.
class NotificationsListResponseDto {
  const NotificationsListResponseDto({
    required this.notifications,
    this.total,
    this.page = 1,
    this.pageSize = 15,
  });

  final List<NotificationDto> notifications;
  final int? total;
  final int page;
  final int pageSize;

  bool get hasMore => total != null && notifications.length < total!;
}
