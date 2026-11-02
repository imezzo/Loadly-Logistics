import 'package:flutter/cupertino.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/features/notifications/domain/entities/notification_attachment_media_type.dart';
import 'package:loadly_driver/features/notifications/presentation/widgets/notification_attachment_dialog.dart';

void main() {
  Future<void> pumpDialog(
    WidgetTester tester, {
    required NotificationAttachmentMediaType mediaType,
  }) async {
    await tester.pumpWidget(
      const CupertinoApp(home: CupertinoPageScaffold(child: SizedBox.shrink())),
    );
    final context = tester.element(find.byType(CupertinoPageScaffold));
    showNotificationAttachmentDialog(
      context: context,
      title: 'مخالفة مرورية',
      body: 'تم تسجيل مخالفة',
      attachmentUrl: mediaType == NotificationAttachmentMediaType.image
          ? 'https://example.com/image.png'
          : 'https://invalid.test/not-a-real-pdf',
      mediaType: mediaType,
    );
    await tester.pump();
  }

  testWidgets('image and pdf use the same dialog shell and close button', (
    tester,
  ) async {
    for (final type in NotificationAttachmentMediaType.values) {
      await pumpDialog(tester, mediaType: type);
      expect(find.text('مخالفة مرورية'), findsOneWidget);
      expect(find.text('إغلاق'), findsOneWidget);
      await tester.tap(find.text('إغلاق'));
      await tester.pumpAndSettle();
    }
  });

  testWidgets('pdf load failure shows Arabic error in the same dialog', (
    tester,
  ) async {
    await pumpDialog(tester, mediaType: NotificationAttachmentMediaType.pdf);
    await tester.pumpAndSettle(const Duration(seconds: 5));
    expect(find.text('تعذر تحميل ملف PDF'), findsOneWidget);
    expect(find.text('إغلاق'), findsOneWidget);
  });
}
