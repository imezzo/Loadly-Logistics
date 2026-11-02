import 'dart:io';

/// Max file size: 10MB (API requirement)
const int kMaxImageSizeBytes = 10 * 1024 * 1024;

/// Allowed image extensions (API: jpeg/jpg/png/webp)
const Set<String> kAllowedImageExtensions = {'jpg', 'jpeg', 'png', 'webp'};

/// Result of image validation.
class ImageValidationResult {
  const ImageValidationResult({this.error});

  final String? error;

  bool get isValid => error == null;
}

/// Validates a single image file for trip uploads.
ImageValidationResult validateTripImage(File file) {
  if (!file.existsSync()) {
    return const ImageValidationResult(error: 'File not found');
  }
  final size = file.lengthSync();
  if (size > kMaxImageSizeBytes) {
    return const ImageValidationResult(error: 'trips.image_too_large');
  }
  final ext = file.path.split('.').last.toLowerCase();
  if (!kAllowedImageExtensions.contains(ext)) {
    return const ImageValidationResult(error: 'trips.invalid_image_format');
  }
  return const ImageValidationResult();
}
