import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/features/notifications/presentation/widgets/notification_attachment_pdf_loader.dart';

void main() {
  tearDown(NotificationAttachmentPdfLoader.clearCacheForTesting);

  test('loadBytes caches by url', () async {
    var callCount = 0;
    final dio = Dio();
    dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          callCount++;
          handler.resolve(
            Response<List<int>>(
              requestOptions: options,
              data: [0x25, 0x50, 0x44, 0x46],
            ),
          );
        },
      ),
    );

    const url = 'https://example.com/a.pdf';
    await NotificationAttachmentPdfLoader.loadBytes(url, dio: dio);
    await NotificationAttachmentPdfLoader.loadBytes(url, dio: dio);

    expect(callCount, 1);
  });
}
