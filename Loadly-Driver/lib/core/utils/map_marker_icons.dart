import 'dart:ui' as ui;

import 'package:flutter/material.dart'
    show Color, Icons, IconData, TextPainter, TextSpan, TextStyle;
import 'package:flutter/painting.dart' show TextDirection;
import 'package:google_maps_flutter/google_maps_flutter.dart';

import '../theme/app_colors.dart';

/// Logical size of markers in display pixels (small on map).
const double _markerLogicalSize = 28.0;

/// Render at 3x for crisp display on high-DPI screens.
const double _pixelRatio = 3.0;

/// Creates custom map marker icons for trip waypoints.
/// Uses distinct icons: Start (flag), Pickup/Cargo (box), Drop off (location).
class MapMarkerIcons {
  MapMarkerIcons._();

  static BitmapDescriptor? _startIcon;
  static BitmapDescriptor? _pickupCargoIcon;
  static BitmapDescriptor? _additionalPickupIcon;
  static BitmapDescriptor? _dropOffIcon;

  static bool _initialized = false;

  /// Call once (e.g. in initState) to preload marker icons.
  static Future<void> initialize() async {
    if (_initialized) return;
    _startIcon = await _createMarkerFromIcon(
      Icons.flag_rounded,
      AppColors.mainBlue,
      AppColors.white,
    );
    _pickupCargoIcon = await _createMarkerFromIcon(
      Icons.inventory_2_rounded,
      AppColors.mainBlue,
      AppColors.white,
    );
    _additionalPickupIcon = await _createMarkerFromIcon(
      Icons.inventory_2_rounded,
      AppColors.orange,
      AppColors.white,
    );
    _dropOffIcon = await _createMarkerFromIcon(
      Icons.location_on_rounded,
      AppColors.green,
      AppColors.white,
    );
    _initialized = true;
  }

  static BitmapDescriptor get start =>
      _startIcon ??
      BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueAzure);

  /// First pickup on the trip. Blue.
  static BitmapDescriptor get pickupCargo =>
      _pickupCargoIcon ??
      BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueAzure);

  /// Pickup points after the first. App orange.
  static BitmapDescriptor get additionalPickup =>
      _additionalPickupIcon ??
      BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueOrange);

  /// Drop-off points. App green.
  static BitmapDescriptor get dropOff =>
      _dropOffIcon ??
      BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueGreen);

  static Future<BitmapDescriptor> _createMarkerFromIcon(
    IconData iconData,
    Color backgroundColor,
    Color iconColor,
  ) async {
    // Render at high resolution for crisp display on retina
    final size = _markerLogicalSize * _pixelRatio;
    final iconSize = size / 2;

    final pictureRecorder = ui.PictureRecorder();
    final canvas = ui.Canvas(pictureRecorder);

    // Circle background
    final bgPaint = ui.Paint()
      ..color = backgroundColor
      ..style = ui.PaintingStyle.fill;
    canvas.drawCircle(ui.Offset(size / 2, size / 2), size / 2 - 2, bgPaint);

    // Border
    final borderPaint = ui.Paint()
      ..color = backgroundColor.withValues(alpha: 0.8)
      ..style = ui.PaintingStyle.stroke
      ..strokeWidth = 1;
    canvas.drawCircle(ui.Offset(size / 2, size / 2), size / 2 - 2, borderPaint);

    // Icon
    final iconStr = String.fromCharCode(iconData.codePoint);
    final textPainter = TextPainter(
      textDirection: TextDirection.ltr,
      text: TextSpan(
        text: iconStr,
        style: TextStyle(
          fontSize: iconSize,
          fontFamily: iconData.fontFamily ?? 'MaterialIcons',
          fontFamilyFallback: iconData.fontPackage != null
              ? <String>['MaterialIcons']
              : null,
          color: iconColor,
        ),
      ),
    );
    textPainter.layout();
    textPainter.paint(
      canvas,
      ui.Offset(
        (size - textPainter.width) / 2,
        (size - textPainter.height) / 2,
      ),
    );

    final picture = pictureRecorder.endRecording();
    final image = await picture.toImage(size.toInt(), size.toInt());
    final byteData = await image.toByteData(format: ui.ImageByteFormat.png);
    final bytes = byteData!.buffer.asUint8List();

    return BytesMapBitmap(
      bytes,
      imagePixelRatio: _pixelRatio,
      width: _markerLogicalSize,
      height: _markerLogicalSize,
      bitmapScaling: MapBitmapScaling.auto,
    );
  }
}
