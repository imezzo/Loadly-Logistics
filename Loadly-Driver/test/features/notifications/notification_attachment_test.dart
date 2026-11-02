import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/features/notifications/data/models/notification_dto.dart';
import 'package:loadly_driver/features/notifications/domain/entities/driver_notification.dart';
import 'package:loadly_driver/features/notifications/domain/entities/notification_attachment.dart';
import 'package:loadly_driver/features/notifications/domain/entities/notification_attachment_media_type.dart';

void main() {
  group('NotificationDto attachments', () {
    test('parses attachments list', () {
      final dto = NotificationDto.fromJson({
        'id': 1,
        'body': 'body',
        'attachments': [
          {'id': 14, 'url': 'https://example.com/image.jpg'},
        ],
      });

      expect(dto.attachments, hasLength(1));
      expect(dto.attachments.first.id, 14);
      expect(dto.attachments.first.url, 'https://example.com/image.jpg');
    });

    test('parses content type from alternate json keys', () {
      final dto = NotificationDto.fromJson({
        'id': 1,
        'body': 'body',
        'attachments': [
          {
            'id': 1,
            'url': 'https://example.com/file',
            'mime_type': 'application/pdf',
          },
        ],
      });

      expect(dto.attachments.first.contentType, 'application/pdf');
    });

    test('missing or empty attachments defaults to empty list', () {
      expect(
        NotificationDto.fromJson({'id': 1, 'body': 'b'}).attachments,
        isEmpty,
      );
      expect(
        NotificationDto.fromJson({
          'id': 1,
          'body': 'b',
          'attachments': null,
        }).attachments,
        isEmpty,
      );
      expect(
        NotificationDto.fromJson({
          'id': 1,
          'body': 'b',
          'attachments': [],
        }).attachments,
        isEmpty,
      );
    });
  });

  group('NotificationAttachmentTypeResolver', () {
    test('recognizes pdf from extension', () {
      expect(
        NotificationAttachmentTypeResolver.resolve(
          url: 'https://cdn.example.com/doc/attachment.pdf',
        ),
        NotificationAttachmentMediaType.pdf,
      );
    });

    test('ignores query and fragment for extension', () {
      expect(
        NotificationAttachmentTypeResolver.resolve(
          url: 'https://cdn.example.com/a/file.pdf?token=abc#page=2',
        ),
        NotificationAttachmentMediaType.pdf,
      );
    });

    test('prefers mime type over extension', () {
      expect(
        NotificationAttachmentTypeResolver.resolve(
          contentType: 'application/pdf',
          url: 'https://example.com/no-extension',
        ),
        NotificationAttachmentMediaType.pdf,
      );
    });
  });

  group('DriverNotification.firstSupportedAttachment', () {
    test('returns first valid image url', () {
      final notification = DriverNotification(
        id: 1,
        body: 'test',
        createdAt: _fixedDate,
        attachments: const [
          NotificationAttachment(id: 1, url: ''),
          NotificationAttachment(id: 2, url: 'ftp://bad'),
          NotificationAttachment(id: 3, url: 'https://cdn.example/a.png'),
          NotificationAttachment(id: 4, url: 'http://other/b.jpg'),
        ],
      );

      final supported = notification.firstSupportedAttachment!;
      expect(supported.url, 'https://cdn.example/a.png');
      expect(supported.type, NotificationAttachmentMediaType.image);
      expect(
        notification.firstValidImageAttachmentUrl,
        'https://cdn.example/a.png',
      );
    });

    test('returns first pdf when no prior valid attachment', () {
      final notification = DriverNotification(
        id: 1,
        body: 'test',
        createdAt: _fixedDate,
        attachments: const [
          NotificationAttachment(id: 1, url: 'not-a-url'),
          NotificationAttachment(id: 2, url: 'https://example.com/report.pdf'),
        ],
      );

      final supported = notification.firstSupportedAttachment!;
      expect(supported.type, NotificationAttachmentMediaType.pdf);
    });

    test('skips unsupported and picks later valid attachment', () {
      final notification = DriverNotification(
        id: 1,
        body: 'test',
        createdAt: _fixedDate,
        attachments: const [
          NotificationAttachment(id: 1, url: 'https://example.com/unknown.bin'),
          NotificationAttachment(id: 2, url: 'https://example.com/doc.pdf'),
        ],
      );

      expect(
        notification.firstSupportedAttachment!.type,
        NotificationAttachmentMediaType.pdf,
      );
    });

    test('returns null when no valid supported attachment', () {
      final notification = DriverNotification(
        id: 1,
        body: 'test',
        createdAt: _fixedDate,
        attachments: const [
          NotificationAttachment(id: 1),
          NotificationAttachment(id: 2, url: 'not-a-url'),
        ],
      );

      expect(notification.firstSupportedAttachment, isNull);
      expect(notification.hasSupportedAttachment, isFalse);
    });

    test('validImageUrl excludes pdf urls', () {
      const attachment = NotificationAttachment(
        id: 1,
        url: 'https://example.com/violation.pdf',
      );
      expect(attachment.validImageUrl, isNull);
      expect(attachment.mediaType, NotificationAttachmentMediaType.pdf);
    });
  });
}

final _fixedDate = DateTime(2024, 7, 23);
