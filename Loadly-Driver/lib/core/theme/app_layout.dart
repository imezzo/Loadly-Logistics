import 'package:flutter/cupertino.dart';

/// Global layout dimensions for auth screens (login, verification, OTP).
/// Control header and content card from this single place.
abstract final class AppLayout {
  /// Height of the blue header area on auth screens.
  static const double authHeaderHeight = 200;

  /// How much the white content container overlaps the header (negative top offset).
  /// Same as login screen for consistent look across auth screens.
  static const double authContentOverlap = 100;

  /// Border radius for the top corners of the white content container.
  static const double authContentRadius = 24;

  /// [Positioned] top value for the white content: -[authContentOverlap].
  static const double authContentTop = -authContentOverlap;

  /// [BorderRadius] for the white content card.
  static BorderRadius get authContentBorderRadius =>
      const BorderRadius.vertical(top: Radius.circular(authContentRadius));
}
