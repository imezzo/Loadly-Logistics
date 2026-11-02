import 'package:equatable/equatable.dart';

import 'notification_attachment.dart';
import 'notification_supported_attachment.dart';

class DriverNotification extends Equatable {
  final int id;
  final String? title;
  final String body;
  final int? eventType;
  final String? resourceType;
  final int? resourceId;
  final String? clickAction;
  final bool isRead;
  final DateTime? readAt;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final List<NotificationAttachment> attachments;

  const DriverNotification({
    required this.id,
    this.title,
    required this.body,
    this.eventType,
    this.resourceType,
    this.resourceId,
    this.clickAction,
    this.isRead = false,
    this.readAt,
    required this.createdAt,
    this.updatedAt,
    this.attachments = const [],
  });

  /// First attachment that is a supported image or PDF, in list order.
  NotificationSupportedAttachment? get firstSupportedAttachment {
    for (final attachment in attachments) {
      final url = attachment.validHttpUrl;
      final type = attachment.mediaType;
      if (url != null && type != null) {
        return NotificationSupportedAttachment(url: url, type: type);
      }
    }
    return null;
  }

  /// URL of the first attachment with a valid http(s) image link, if any.
  String? get firstValidImageAttachmentUrl {
    for (final attachment in attachments) {
      final url = attachment.validImageUrl;
      if (url != null) return url;
    }
    return null;
  }

  bool get hasSupportedAttachment => firstSupportedAttachment != null;

  @override
  List<Object?> get props {
    return [
      id,
      title,
      body,
      eventType,
      resourceType,
      resourceId,
      clickAction,
      isRead,
      readAt,
      createdAt,
      updatedAt,
      attachments,
    ];
  }
}
