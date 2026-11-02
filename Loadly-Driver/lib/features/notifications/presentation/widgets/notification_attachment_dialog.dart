import 'package:dio/dio.dart';
import 'package:flutter/cupertino.dart';
import 'package:pdfx/pdfx.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/utils/service_locator.dart';
import '../../domain/entities/notification_attachment_media_type.dart';
import 'notification_attachment_pdf_loader.dart';

/// Modal popup for a notification attachment (image or PDF).
Future<void> showNotificationAttachmentDialog({
  required BuildContext context,
  required String title,
  required String body,
  required String attachmentUrl,
  required NotificationAttachmentMediaType mediaType,
}) {
  return showCupertinoDialog<void>(
    context: context,
    barrierDismissible: true,
    builder: (dialogContext) {
      return _NotificationAttachmentDialog(
        title: title,
        body: body,
        attachmentUrl: attachmentUrl,
        mediaType: mediaType,
      );
    },
  );
}

class _NotificationAttachmentDialog extends StatelessWidget {
  const _NotificationAttachmentDialog({
    required this.title,
    required this.body,
    required this.attachmentUrl,
    required this.mediaType,
  });

  final String title;
  final String body;
  final String attachmentUrl;
  final NotificationAttachmentMediaType mediaType;

  static const _closeLabel = 'إغلاق';

  void _close(BuildContext context) {
    Navigator.of(context, rootNavigator: true).pop();
  }

  @override
  Widget build(BuildContext context) {
    final maxHeight = MediaQuery.sizeOf(context).height * 0.82;
    final maxWidth = MediaQuery.sizeOf(context).width - 48;

    return Center(
      child: Container(
        constraints: BoxConstraints(maxWidth: maxWidth, maxHeight: maxHeight),
        margin: const EdgeInsets.symmetric(horizontal: 24),
        padding: const EdgeInsets.fromLTRB(20, 24, 20, 20),
        decoration: BoxDecoration(
          color: AppColors.white,
          borderRadius: BorderRadius.circular(20),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              title,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w700,
                color: AppColors.mainBlue,
                height: 1.35,
              ),
            ),
            if (body.trim() != title.trim()) ...[
              const SizedBox(height: 8),
              Text(
                body,
                textAlign: TextAlign.center,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w400,
                  color: AppColors.mediumBlueGray,
                  height: 1.4,
                ),
              ),
            ],
            const SizedBox(height: 16),
            Flexible(
              child: SingleChildScrollView(
                physics: const BouncingScrollPhysics(),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: mediaType == NotificationAttachmentMediaType.image
                      ? _NotificationAttachmentImageContent(
                          imageUrl: attachmentUrl,
                        )
                      : _NotificationAttachmentPdfContent(
                          pdfUrl: attachmentUrl,
                        ),
                ),
              ),
            ),
            const SizedBox(height: 20),
            CupertinoButton(
              padding: EdgeInsets.zero,
              onPressed: () => _close(context),
              child: Container(
                width: double.infinity,
                padding: const EdgeInsets.symmetric(vertical: 14),
                decoration: BoxDecoration(
                  color: AppColors.white,
                  borderRadius: BorderRadius.circular(28),
                  border: Border.all(color: AppColors.mainBlue, width: 1.5),
                ),
                child: const Text(
                  _closeLabel,
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: AppColors.mainBlue,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Image branch — unchanged from the original image-only popup.
class _NotificationAttachmentImageContent extends StatelessWidget {
  const _NotificationAttachmentImageContent({required this.imageUrl});

  final String imageUrl;

  @override
  Widget build(BuildContext context) {
    return Image.network(
      imageUrl,
      width: double.infinity,
      fit: BoxFit.contain,
      loadingBuilder: (context, child, loadingProgress) {
        if (loadingProgress == null) return child;
        return const SizedBox(
          height: 200,
          child: Center(
            child: CupertinoActivityIndicator(
              color: AppColors.mainBlue,
              radius: 16,
            ),
          ),
        );
      },
      errorBuilder: (context, error, stackTrace) {
        return Container(
          width: double.infinity,
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 32),
          decoration: BoxDecoration(
            color: AppColors.lightMint.withValues(alpha: 0.5),
            borderRadius: BorderRadius.circular(12),
          ),
          child: const Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(
                CupertinoIcons.exclamationmark_triangle,
                size: 40,
                color: AppColors.red,
              ),
              SizedBox(height: 12),
              Text(
                'تعذر تحميل الصورة',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 14,
                  color: AppColors.darkGray,
                  fontWeight: FontWeight.w500,
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}

class _NotificationAttachmentPdfContent extends StatefulWidget {
  const _NotificationAttachmentPdfContent({required this.pdfUrl});

  final String pdfUrl;

  @override
  State<_NotificationAttachmentPdfContent> createState() =>
      _NotificationAttachmentPdfContentState();
}

class _NotificationAttachmentPdfContentState
    extends State<_NotificationAttachmentPdfContent> {
  PdfControllerPinch? _controller;
  bool _loading = true;
  bool _error = false;

  @override
  void initState() {
    super.initState();
    _loadPdf();
  }

  Future<void> _loadPdf() async {
    try {
      final dio = getIt.isRegistered<Dio>() ? getIt<Dio>() : null;
      final bytes = await NotificationAttachmentPdfLoader.loadBytes(
        widget.pdfUrl,
        dio: dio,
      );
      if (!mounted) return;
      _controller = PdfControllerPinch(document: PdfDocument.openData(bytes));
      setState(() {
        _loading = false;
        _error = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _loading = false;
        _error = true;
      });
    }
  }

  @override
  void dispose() {
    _controller?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_loading) {
      return const SizedBox(
        height: 200,
        width: double.infinity,
        child: Center(
          child: CupertinoActivityIndicator(
            color: AppColors.mainBlue,
            radius: 16,
          ),
        ),
      );
    }
    if (_error || _controller == null) {
      return Container(
        width: double.infinity,
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 32),
        decoration: BoxDecoration(
          color: AppColors.lightMint.withValues(alpha: 0.5),
          borderRadius: BorderRadius.circular(12),
        ),
        child: const Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              CupertinoIcons.exclamationmark_triangle,
              size: 40,
              color: AppColors.red,
            ),
            SizedBox(height: 12),
            Text(
              'تعذر تحميل ملف PDF',
              textAlign: TextAlign.center,
              style: TextStyle(
                fontSize: 14,
                color: AppColors.darkGray,
                fontWeight: FontWeight.w500,
              ),
            ),
          ],
        ),
      );
    }

    return SizedBox(
      height: 280,
      width: double.infinity,
      child: PdfViewPinch(controller: _controller!),
    );
  }
}
