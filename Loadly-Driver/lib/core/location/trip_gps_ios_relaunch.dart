import 'dart:io' show Platform;

import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';

/// Arms iOS significant-location monitoring for the open trip.
///
/// Continuous GPS already runs while the app is suspended. After iOS itself
/// terminates the process, significant-location delivery relaunches it so the
/// trip trace can attach again. A force-quit from the app switcher is not
/// relaunched until the driver opens the app; that is an iOS restriction.
class TripGpsIosRelaunch {
  static const _channel = MethodChannel('com.loadly.loadly_driver/trip_gps');

  static Future<void> arm() async {
    if (!Platform.isIOS) return;
    try {
      await _channel.invokeMethod<void>('arm');
    } catch (e) {
      debugPrint('Trip GPS iOS relaunch arm failed: $e');
    }
  }

  static Future<void> disarm() async {
    if (!Platform.isIOS) return;
    try {
      await _channel.invokeMethod<void>('disarm');
    } catch (e) {
      debugPrint('Trip GPS iOS relaunch disarm failed: $e');
    }
  }

  /// Fix saved by iOS when it relaunched the process for a location event.
  static Future<TripGpsRelaunchFix?> takePendingFix() async {
    if (!Platform.isIOS) return null;
    try {
      final raw = await _channel.invokeMethod<Object?>('takePendingFix');
      if (raw is! Map) return null;
      final latitude = raw['latitude'];
      final longitude = raw['longitude'];
      final accuracy = raw['accuracy'];
      final timestamp = raw['timestamp'];
      if (latitude is! num ||
          longitude is! num ||
          accuracy is! num ||
          timestamp is! num) {
        return null;
      }
      final speed = raw['speed'];
      return TripGpsRelaunchFix(
        latitude: latitude.toDouble(),
        longitude: longitude.toDouble(),
        accuracyMeters: accuracy.toDouble(),
        timestamp: DateTime.fromMillisecondsSinceEpoch(
          (timestamp.toDouble() * 1000).round(),
          isUtc: true,
        ),
        speedMps: speed is num ? speed.toDouble() : null,
      );
    } catch (e) {
      debugPrint('Trip GPS iOS pending fix failed: $e');
      return null;
    }
  }
}

class TripGpsRelaunchFix {
  const TripGpsRelaunchFix({
    required this.latitude,
    required this.longitude,
    required this.accuracyMeters,
    required this.timestamp,
    this.speedMps,
  });

  final double latitude;
  final double longitude;
  final double accuracyMeters;
  final DateTime timestamp;
  final double? speedMps;
}
