import 'package:equatable/equatable.dart';

import 'notification_attachment_media_type.dart';

class NotificationAttachment extends Equatable {
  const NotificationAttachment({required this.id, this.url, this.contentType});

  final int id;
  final String? url;
  final String? contentType;

  /// Valid http(s) URL, or null if missing/invalid.
  String? get validHttpUrl {
    final trimmed = url?.trim();
    if (trimmed == null || trimmed.isEmpty) return null;
    final uri = Uri.tryParse(trimmed);
    if (uri == null || !uri.hasScheme) return null;
    if (uri.scheme != 'http' && uri.scheme != 'https') return null;
    return trimmed;
  }

  NotificationAttachmentMediaType? get mediaType {
    return NotificationAttachmentTypeResolver.resolve(
      contentType: contentType,
      url: validHttpUrl,
    );
  }

  /// Valid http(s) image URL when the attachment is an image, otherwise null.
  String? get validImageUrl {
    final httpUrl = validHttpUrl;
    if (httpUrl == null) return null;
    if (mediaType == NotificationAttachmentMediaType.image) return httpUrl;
    return null;
  }

  @override
  List<Object?> get props => [id, url, contentType];
}
