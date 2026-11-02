import 'package:flutter/cupertino.dart';

/// Loadly Truck UI design system colors.
abstract final class AppColors {
  /// Light gray (0) - backgrounds, borders.
  static const Color lightGray = Color(0xFFD1E7E3);

  /// Medium blue-gray (25).
  static const Color mediumBlueGray = Color(0xFF99C2BC);

  /// Vibrant teal - accents.
  static const Color vibrantBlue = Color(0xFF14B8A6);

  /// Primary teal - actions, headers.
  static const Color mainBlue = Color(0xFF0F766E);

  /// White (for text on main blue).
  static const Color white = Color(0xFFFFFFFF);

  /// Dark gray for body text on light backgrounds.
  static const Color darkGray = Color(0xFF333333);

  /// Soft mint for selected/highlighted cards.
  static const Color lightMint = Color(0xFFCCFBF1);

  /// Green color for success/online status.
  static const Color green = Color(0xFF4CAF50);

  /// New green color for new design.
  static const Color newgreen = Color(0xFF2EA154);

  /// Red color for error/offline status.
  static const Color red = Color(0xFFE53935);

  static const Color orange = Color(0xFFE8A317);
}
