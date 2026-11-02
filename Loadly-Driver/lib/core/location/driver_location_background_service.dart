import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:geolocator/geolocator.dart';

import '../../features/home/domain/use_cases/update_location_usecase.dart';
import 'unified_driver_location_hub.dart';

/// Sends driver coordinates to the backend while online via [UnifiedDriverLocationHub].
class DriverLocationBackgroundService {
  DriverLocationBackgroundService(this._updateLocation, this._hub);

  final UpdateLocationUseCase _updateLocation;
  final UnifiedDriverLocationHub _hub;

  static const _uploadInterval = Duration(minutes: 1);

  DateTime? _lastUploadedAt;
  bool _uploadInProgress = false;
  var _started = false;
  PositionListener? _listener;

  Future<void> start() async {
    if (_started) return;

    final permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied ||
        permission == LocationPermission.deniedForever) {
      return;
    }
    if (!await Geolocator.isLocationServiceEnabled()) {
      return;
    }

    _started = true;
    _listener = _onPosition;
    _hub.addListener(_listener!);
    _hub.setOnlinePingActive(true);

    await _uploadCurrentPosition();
  }

  Future<void> stop() async {
    if (!_started) return;
    _started = false;
    _hub.setOnlinePingActive(false);
    if (_listener != null) {
      _hub.removeListener(_listener!);
      _listener = null;
    }
    _lastUploadedAt = null;
    _uploadInProgress = false;
  }

  void _onPosition(Position position) {
    final now = DateTime.now();
    if (_lastUploadedAt != null &&
        now.difference(_lastUploadedAt!) < _uploadInterval) {
      return;
    }
    unawaited(_upload(position.latitude, position.longitude));
  }

  Future<void> _uploadCurrentPosition() async {
    try {
      final position = await Geolocator.getCurrentPosition(
        locationSettings: const LocationSettings(
          accuracy: LocationAccuracy.medium,
        ),
      );
      await _upload(position.latitude, position.longitude);
    } catch (e) {
      debugPrint('Background location initial read failed: $e');
    }
  }

  Future<void> _upload(double latitude, double longitude) async {
    if (_uploadInProgress) return;
    _uploadInProgress = true;
    try {
      final result = await _updateLocation(
        UpdateLocationParams(latitude: latitude, longitude: longitude),
      );
      result.fold(
        (failure) => debugPrint('Background location upload failed: $failure'),
        (_) => _lastUploadedAt = DateTime.now(),
      );
    } finally {
      _uploadInProgress = false;
    }
  }
}
