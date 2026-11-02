import 'package:flutter/cupertino.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/features/notifications/domain/entities/notification_attachment_media_type.dart';
import 'package:loadly_driver/features/notifications/domain/entities/notification_supported_attachment.dart';
import 'package:loadly_driver/features/notifications/presentation/widgets/notification_attachment_preview.dart';

void main() {
  testWidgets('image preview renders and forwards tap', (tester) async {
    var tapped = false;
    await tester.pumpWidget(
      CupertinoApp(
        home: CupertinoPageScaffold(
          child: NotificationAttachmentPreview(
            attachment: const NotificationSupportedAttachment(
              url: 'https://example.com/photo.jpg',
              type: NotificationAttachmentMediaType.image,
            ),
            onTap: () => tapped = true,
          ),
        ),
      ),
    );

    expect(find.byType(NotificationAttachmentPreview), findsOneWidget);
    await tester.tap(find.byType(NotificationAttachmentPreview));
    await tester.pump();
    expect(tapped, isTrue);
  });

  testWidgets('pdf preview handles load failure without crashing', (
    tester,
  ) async {
    await tester.pumpWidget(
      CupertinoApp(
        home: CupertinoPageScaffold(
          child: NotificationAttachmentPreview(
            attachment: const NotificationSupportedAttachment(
              url: 'https://invalid.test/not-a-real-pdf',
              type: NotificationAttachmentMediaType.pdf,
            ),
            onTap: () {},
          ),
        ),
      ),
    );

    await tester.pump();
    expect(find.byType(CupertinoActivityIndicator), findsOneWidget);
    await tester.pumpAndSettle(const Duration(seconds: 5));
    expect(
      find.byIcon(CupertinoIcons.exclamationmark_triangle),
      findsOneWidget,
    );
  });
}
