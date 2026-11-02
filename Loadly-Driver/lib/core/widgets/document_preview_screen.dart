import 'package:flutter/cupertino.dart';
import 'package:url_launcher/url_launcher.dart';

import '../theme/app_colors.dart';

/// Full-screen in-app image preview. Shows the image from [imageUrl]
/// with zoom/pan support. Use [Navigator.push] to present.
/// Falls back to external browser when image fails to load (e.g. PDF).
class DocumentPreviewScreen extends StatelessWidget {
  const DocumentPreviewScreen({super.key, required this.imageUrl});

  final String imageUrl;

  static const String _openInBrowserLabel = 'Open in browser';

  @override
  Widget build(BuildContext context) {
    return CupertinoPageScaffold(
      backgroundColor: AppColors.darkGray,
      child: SafeArea(
        child: Stack(
          children: [
            InteractiveViewer(
              minScale: 0.5,
              maxScale: 4.0,
              child: Center(
                child: Image.network(
                  imageUrl,
                  fit: BoxFit.contain,
                  loadingBuilder: (context, child, loadingProgress) {
                    if (loadingProgress == null) return child;
                    return const Center(
                      child: CupertinoActivityIndicator(
                        color: AppColors.white,
                        radius: 16,
                      ),
                    );
                  },
                  errorBuilder: (context, error, stackTrace) {
                    return Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(
                            CupertinoIcons.exclamationmark_triangle,
                            size: 48,
                            color: AppColors.white.withValues(alpha: 0.8),
                          ),
                          const SizedBox(height: 16),
                          Text(
                            'Could not load image',
                            style: TextStyle(
                              color: AppColors.white.withValues(alpha: 0.8),
                              fontSize: 16,
                            ),
                          ),
                          const SizedBox(height: 24),
                          CupertinoButton(
                            color: AppColors.mainBlue,
                            onPressed: () async {
                              try {
                                await launchUrl(
                                  Uri.parse(imageUrl),
                                  mode: LaunchMode.externalApplication,
                                );
                              } catch (_) {}
                            },
                            child: const Text(_openInBrowserLabel),
                          ),
                        ],
                      ),
                    );
                  },
                ),
              ),
            ),
            Positioned(
              top: 8,
              left: 8,
              child: CupertinoButton(
                padding: const EdgeInsets.all(8),
                color: AppColors.darkGray.withValues(alpha: 0.6),
                borderRadius: BorderRadius.circular(8),
                onPressed: () => Navigator.of(context).pop(),
                child: const Icon(
                  CupertinoIcons.xmark,
                  color: AppColors.white,
                  size: 24,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
