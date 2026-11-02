class NotificationAttachmentDto {
  const NotificationAttachmentDto({
    required this.id,
    this.url,
    this.contentType,
  });

  final int id;
  final String? url;
  final String? contentType;

  static String? _readContentType(Map<String, dynamic> json) {
    return json['contentType'] as String? ??
        json['content_type'] as String? ??
        json['mimeType'] as String? ??
        json['mime_type'] as String?;
  }

  factory NotificationAttachmentDto.fromJson(Map<String, dynamic> json) =>
      NotificationAttachmentDto(
        id: json['id'] as int? ?? 0,
        url: json['url'] as String?,
        contentType: _readContentType(json),
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'url': url,
    if (contentType != null) 'contentType': contentType,
  };
}

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
    this.attachments = const [],
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
  final List<NotificationAttachmentDto> attachments;

  static List<NotificationAttachmentDto> _parseAttachments(dynamic raw) {
    if (raw == null || raw is! List) return const [];
    return raw
        .whereType<Map>()
        .map(
          (e) =>
              NotificationAttachmentDto.fromJson(Map<String, dynamic>.from(e)),
        )
        .toList();
  }

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
        attachments: _parseAttachments(json['attachments']),
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
    'attachments': attachments.map((e) => e.toJson()).toList(),
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
