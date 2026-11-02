import 'package:dio/dio.dart';
import 'package:flutter/cupertino.dart';
import 'package:pdfx/pdfx.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/utils/service_locator.dart';
import '../../domain/entities/notification_attachment_media_type.dart';
import '../../domain/entities/notification_supported_attachment.dart';
import 'notification_attachment_pdf_loader.dart';

/// Inline attachment preview for notification list cards (below company header).
class NotificationAttachmentPreview extends StatelessWidget {
  const NotificationAttachmentPreview({
    super.key,
    required this.attachment,
    required this.onTap,
  });

  final NotificationSupportedAttachment attachment;
  final VoidCallback onTap;

  static const previewWidth = 72.0;
  static const previewHeight = 72.0;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: previewWidth,
        height: previewHeight,
        decoration: BoxDecoration(
          color: AppColors.white,
          borderRadius: BorderRadius.circular(8),
          border: Border.all(color: AppColors.lightGray, width: 1),
        ),
        clipBehavior: Clip.antiAlias,
        child: attachment.type == NotificationAttachmentMediaType.image
            ? _ImagePreviewContent(imageUrl: attachment.url)
            : _PdfPreviewContent(pdfUrl: attachment.url),
      ),
    );
  }
}

class _ImagePreviewContent extends StatelessWidget {
  const _ImagePreviewContent({required this.imageUrl});

  final String imageUrl;

  @override
  Widget build(BuildContext context) {
    return Image.network(
      imageUrl,
      width: NotificationAttachmentPreview.previewWidth,
      height: NotificationAttachmentPreview.previewHeight,
      fit: BoxFit.cover,
      loadingBuilder: (context, child, loadingProgress) {
        if (loadingProgress == null) return child;
        return const Center(
          child: CupertinoActivityIndicator(
            color: AppColors.mainBlue,
            radius: 10,
          ),
        );
      },
      errorBuilder: (context, error, stackTrace) =>
          const _PreviewError(message: 'تعذر تحميل المعاينة'),
    );
  }
}

class _PdfPreviewContent extends StatefulWidget {
  const _PdfPreviewContent({required this.pdfUrl});

  final String pdfUrl;

  @override
  State<_PdfPreviewContent> createState() => _PdfPreviewContentState();
}

class _PdfPreviewContentState extends State<_PdfPreviewContent> {
  PdfControllerPinch? _controller;
  bool _loading = true;
  bool _error = false;

  @override
  void initState() {
    super.initState();
    _loadPreview();
  }

  Future<void> _loadPreview() async {
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
      return const Center(
        child: CupertinoActivityIndicator(
          color: AppColors.mainBlue,
          radius: 10,
        ),
      );
    }
    if (_error || _controller == null) {
      return const _PreviewError(message: 'تعذر تحميل المعاينة');
    }

    return Stack(
      fit: StackFit.expand,
      children: [
        IgnorePointer(child: PdfViewPinch(controller: _controller!)),
        Positioned(
          left: 2,
          bottom: 2,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 2),
            decoration: BoxDecoration(
              color: AppColors.darkGray.withValues(alpha: 0.65),
              borderRadius: BorderRadius.circular(4),
            ),
            child: const Icon(
              CupertinoIcons.doc_text_fill,
              size: 10,
              color: AppColors.white,
            ),
          ),
        ),
      ],
    );
  }
}

class _PreviewError extends StatelessWidget {
  const _PreviewError({required this.message});

  final String message;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Icon(
        CupertinoIcons.exclamationmark_triangle,
        size: 22,
        color: AppColors.mediumBlueGray.withValues(alpha: 0.9),
      ),
    );
  }
}
