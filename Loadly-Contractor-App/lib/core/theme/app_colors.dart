import 'package:flutter/cupertino.dart';

/// Loadly Truck UI design system colors.
abstract final class AppColors {
  /// Light gray (0) - backgrounds, borders.
  static const Color lightGray = Color(0xFFD1E7E3);

  /// Medium blue-gray (25).
  static const Color mediumBlueGray = Color(0xFF99C2BC);

  /// Vibrant teal (100) - accents.
  static const Color vibrantBlue = Color(0xFF14B8A6);

  /// Main / teal - primary actions, headers.
  static const Color mainBlue = Color(0xFF0F766E);

  /// Main gray - backgrounds (25).
  static const Color mainGray = Color(0xF6F6F6F6);

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

  /// New-trip card border and outer glow.
  static const Color newTripHighlightBorder = Color(0xFFFF6300);

  /// New-trip badge background.
  static const Color newTripHighlightBadge = Color(0xFFF27C0D);
}
