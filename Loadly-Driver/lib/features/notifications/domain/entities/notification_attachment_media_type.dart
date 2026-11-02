/// Supported notification attachment media kinds.
enum NotificationAttachmentMediaType { image, pdf }

/// Resolves attachment media type from MIME (preferred) or URL path extension.
abstract final class NotificationAttachmentTypeResolver {
  static const _imageExtensions = {
    'jpg',
    'jpeg',
    'png',
    'webp',
    'gif',
    'bmp',
    'heic',
    'heif',
  };

  static NotificationAttachmentMediaType? resolve({
    String? contentType,
    required String? url,
  }) {
    if (url == null || url.trim().isEmpty) return null;

    final mime = _normalizeMime(contentType);
    if (mime != null) {
      if (mime == 'application/pdf') {
        return NotificationAttachmentMediaType.pdf;
      }
      if (mime.startsWith('image/')) {
        return NotificationAttachmentMediaType.image;
      }
      return null;
    }

    final extension = fileExtensionFromHttpUrl(url);
    if (extension == null) return null;
    if (extension == 'pdf') return NotificationAttachmentMediaType.pdf;
    if (_imageExtensions.contains(extension)) {
      return NotificationAttachmentMediaType.image;
    }
    return null;
  }

  static String? _normalizeMime(String? raw) {
    if (raw == null) return null;
    final trimmed = raw.trim();
    if (trimmed.isEmpty) return null;
    return trimmed.split(';').first.trim().toLowerCase();
  }

  /// Extension from URL path only (query/fragment ignored).
  static String? fileExtensionFromHttpUrl(String url) {
    final uri = Uri.tryParse(url.trim());
    if (uri == null) return null;
    final path = uri.path;
    if (path.isEmpty) return null;
    final dot = path.lastIndexOf('.');
    if (dot <= 0 || dot >= path.length - 1) return null;
    return path.substring(dot + 1).toLowerCase();
  }
}
