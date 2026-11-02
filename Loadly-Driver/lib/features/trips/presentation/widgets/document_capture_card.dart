import 'dart:io';

import 'package:flutter/cupertino.dart';
import 'package:flutter/services.dart';
import 'package:image_picker/image_picker.dart';
import 'package:permission_handler/permission_handler.dart';

import '../../../../core/permissions/prominent_user_data_disclosure.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../core/image_validation.dart';

/// A card showing a document type with add area (dashed border, camera) and captured thumbnails.
class DocumentCaptureCard extends StatefulWidget {
  const DocumentCaptureCard({
    super.key,
    required this.title,
    required this.capturedFiles,
    required this.onFilesChanged,
    this.minCount = 1,
    this.singleFile = false,
    this.onValidationError,
    this.onImageAccepted,
    this.onPrepareForCamera,
    this.onCameraClosed,
    this.openCameraHint,
    this.openCameraButtonLabel,
  });

  final String title;
  final String? openCameraHint;
  final String? openCameraButtonLabel;
  final List<File> capturedFiles;
  final ValueChanged<List<File>> onFilesChanged;

  /// Minimum required (for multi-file). Ignored if singleFile.
  final int minCount;

  /// If true, only one file allowed (e.g. receipt note).
  final bool singleFile;
  final ValueChanged<String>? onValidationError;
  final Future<String?> Function(File file)? onImageAccepted;

  /// Runs before the camera activity opens so the page can release heavy views
  /// (the map) and remember which slot this photo belongs to.
  final Future<void> Function()? onPrepareForCamera;
  final VoidCallback? onCameraClosed;

  @override
  State<DocumentCaptureCard> createState() => _DocumentCaptureCardState();
}

class _DocumentCaptureCardState extends State<DocumentCaptureCard> {
  final ImagePicker _picker = ImagePicker();
  bool _isProcessingCapture = false;

  Future<void> _openCamera() async {
    if (_isProcessingCapture) return;
    setState(() => _isProcessingCapture = true);
    final l10n = AppLocalizations.of(context)!;
    try {
      final cameraStatus = await Permission.camera.status;
      if (!mounted) return;
      if (cameraStatus.isDenied) {
        final ok = await showProminentUserDataDisclosure(
          context,
          title: l10n.cameraPermissionDisclosureTitle,
          body: l10n.cameraPermissionDisclosureBody,
        );
        if (!ok || !mounted) return;
        final result = await Permission.camera.request();
        if (!result.isGranted) {
          if (mounted) {
            widget.onValidationError?.call(l10n.cameraPermissionRequired);
          }
          return;
        }
      } else if (cameraStatus.isPermanentlyDenied) {
        widget.onValidationError?.call(l10n.cameraPermissionRequired);
        return;
      }

      await widget.onPrepareForCamera?.call();
      if (!mounted) return;

      final img = await _picker.pickImage(
        source: ImageSource.camera,
        imageQuality: 70,
        maxWidth: 1280,
        maxHeight: 720,
      );
      // The page may have been recreated while the camera was open. Still
      // report the file so it can be stored for the new page.
      if (img == null) return;
      final file = File(img.path);
      final err = validateTripImage(file).error;
      if (err != null) {
        if (mounted) widget.onValidationError?.call(err);
        return;
      }
      final customErr = await widget.onImageAccepted?.call(file);
      if (customErr != null) {
        if (mounted) widget.onValidationError?.call(customErr);
        return;
      }
      final list = List<File>.from(widget.capturedFiles);
      if (widget.singleFile) {
        list.clear();
      }
      list.add(file);
      widget.onFilesChanged(list);
      if (mounted) setState(() {});
    } on PlatformException {
      if (widget.capturedFiles.length >= widget.minCount) return;
      if (mounted) widget.onValidationError?.call(l10n.cameraCancelled);
    } finally {
      widget.onCameraClosed?.call();
      if (mounted) setState(() => _isProcessingCapture = false);
    }
  }

  void _removeAt(int index) {
    setState(() {
      final list = List<File>.from(widget.capturedFiles)..removeAt(index);
      widget.onFilesChanged(list);
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          widget.title,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: AppColors.darkGray,
          ),
        ),
        const SizedBox(height: 12),
        // Add area - dashed border, upload icon, Open camera button
        GestureDetector(
          onTap: _isProcessingCapture ? null : _openCamera,
          child: Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 16),
            decoration: BoxDecoration(
              color: AppColors.lightGray.withValues(alpha: 0.3),
              borderRadius: BorderRadius.circular(12),
              border: Border.all(
                color: AppColors.mainBlue.withValues(alpha: 0.5),
                width: 2,
                strokeAlign: BorderSide.strokeAlignInside,
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Icon(
                  CupertinoIcons.cloud_upload,
                  size: 40,
                  color: AppColors.mainBlue.withValues(alpha: 0.8),
                ),
                const SizedBox(height: 8),
                Text(
                  widget.openCameraHint ?? 'Open camera for taking photos',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontSize: 13,
                    color: AppColors.darkGray.withValues(alpha: 0.8),
                  ),
                ),
                const SizedBox(height: 12),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 20,
                    vertical: 10,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.white,
                    borderRadius: BorderRadius.circular(8),
                    border: Border.all(color: AppColors.mainBlue),
                  ),
                  child: Text(
                    _isProcessingCapture
                        ? l10n.processing
                        : widget.openCameraButtonLabel ?? 'Open camera',
                    style: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                      color: AppColors.mainBlue,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
        if (widget.capturedFiles.isNotEmpty) ...[
          const SizedBox(height: 16),
          Text(
            '${widget.title} (${widget.capturedFiles.length})',
            style: const TextStyle(
              fontSize: 13,
              fontWeight: FontWeight.w600,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 8),
          SizedBox(
            height: 80,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              itemCount: widget.capturedFiles.length,
              separatorBuilder: (context, index) => const SizedBox(width: 12),
              itemBuilder: (context, index) {
                final file = widget.capturedFiles[index];
                return Stack(
                  children: [
                    ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: Image.file(
                        file,
                        width: 80,
                        height: 80,
                        fit: BoxFit.cover,
                      ),
                    ),
                    Positioned(
                      top: 4,
                      right: 4,
                      child: GestureDetector(
                        onTap: () => _removeAt(index),
                        child: Container(
                          padding: const EdgeInsets.all(4),
                          decoration: const BoxDecoration(
                            color: AppColors.red,
                            shape: BoxShape.circle,
                          ),
                          child: const Icon(
                            CupertinoIcons.trash,
                            size: 16,
                            color: AppColors.white,
                          ),
                        ),
                      ),
                    ),
                  ],
                );
              },
            ),
          ),
        ],
      ],
    );
  }
}
