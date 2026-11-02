import 'notification_attachment_media_type.dart';

class NotificationSupportedAttachment {
  const NotificationSupportedAttachment({
    required this.url,
    required this.type,
  });

  final String url;
  final NotificationAttachmentMediaType type;
}
