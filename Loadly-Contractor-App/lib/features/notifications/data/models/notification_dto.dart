class NotificationDto {
  const NotificationDto({
    required this.id,
    this.title,
    required this.body,
    this.eventType,
    this.resourceType,
    this.resourceId,
    this.clickAction,
    this.isRead,
    this.readAt,
    this.createdAt,
    this.updatedAt,
  });

  final int id;
  final String? title;
  final String body;
  final int? eventType;
  final String? resourceType;
  final int? resourceId;
  final String? clickAction;
  final bool? isRead;
  final String? readAt;
  final String? createdAt;
  final String? updatedAt;

  factory NotificationDto.fromJson(Map<String, dynamic> json) =>
      NotificationDto(
        id: json['id'] as int? ?? 0,
        title: json['title'] as String?,
        body: json['body'] as String? ?? '',
        eventType: json['eventType'] as int?,
        resourceType: json['resourceType'] as String?,
        resourceId: json['resourceId'] as int?,
        clickAction: json['clickAction'] as String?,
        isRead: json['isRead'] as bool?,
        readAt: json['readAt'] as String?,
        createdAt: json['createdAt'] as String?,
        updatedAt: json['updatedAt'] as String?,
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'title': title,
    'body': body,
    'eventType': eventType,
    'resourceType': resourceType,
    'resourceId': resourceId,
    'clickAction': clickAction,
    'isRead': isRead,
    'readAt': readAt,
    'createdAt': createdAt,
    'updatedAt': updatedAt,
  };
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
