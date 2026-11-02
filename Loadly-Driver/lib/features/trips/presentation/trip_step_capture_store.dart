import 'dart:convert';
import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

/// Capture slots for the current trip step. Kept stable so a reload can put
/// the photo back on the same field.
abstract final class TripCaptureSlots {
  static const receiptNote = 'receiptNote';
  static const shipment = 'shipment';
  static const unloadingOdometer = 'unloadingOdometer';
  static const deliveryNote = 'deliveryNote';
  static const unloading = 'unloading';
  static const departOdometer = 'departOdometer';
  static const completeOdometer = 'completeOdometer';
  static const tollgate = 'tollgate';
  static const tollgateWeight = 'tollgateWeight';
}

class TripStepCaptureSnapshot {
  const TripStepCaptureSnapshot({
    this.files = const {},
    this.pendingSlot,
    this.completeLoadingNote,
    this.completeUnloadingNote,
    this.completeTripNote,
    this.departOdometer,
    this.completeUnloadingOdometer,
    this.completeTripOdometer,
  });

  final Map<String, List<String>> files;
  final String? pendingSlot;
  final String? completeLoadingNote;
  final String? completeUnloadingNote;
  final String? completeTripNote;
  final int? departOdometer;
  final int? completeUnloadingOdometer;
  final int? completeTripOdometer;

  int get fileCount =>
      files.values.fold<int>(0, (sum, paths) => sum + paths.length);

  bool get hasFiles => fileCount > 0;

  List<File> existingFiles(String slot) {
    return (files[slot] ?? const [])
        .map(File.new)
        .where((file) => file.existsSync())
        .toList();
  }

  Map<String, dynamic> toJson() => {
    'files': files,
    'pendingSlot': pendingSlot,
    'completeLoadingNote': completeLoadingNote,
    'completeUnloadingNote': completeUnloadingNote,
    'completeTripNote': completeTripNote,
    'departOdometer': departOdometer,
    'completeUnloadingOdometer': completeUnloadingOdometer,
    'completeTripOdometer': completeTripOdometer,
  };

  factory TripStepCaptureSnapshot.fromJson(Map<String, dynamic> json) {
    final rawFiles = json['files'];
    final files = <String, List<String>>{};
    if (rawFiles is Map) {
      for (final entry in rawFiles.entries) {
        final value = entry.value;
        if (value is List) {
          files[entry.key.toString()] = value.map((e) => e.toString()).toList();
        }
      }
    }
    return TripStepCaptureSnapshot(
      files: files,
      pendingSlot: json['pendingSlot'] as String?,
      completeLoadingNote: json['completeLoadingNote'] as String?,
      completeUnloadingNote: json['completeUnloadingNote'] as String?,
      completeTripNote: json['completeTripNote'] as String?,
      departOdometer: (json['departOdometer'] as num?)?.toInt(),
      completeUnloadingOdometer: (json['completeUnloadingOdometer'] as num?)
          ?.toInt(),
      completeTripOdometer: (json['completeTripOdometer'] as num?)?.toInt(),
    );
  }
}

/// Remembers in-progress trip-step photos across a screen reload.
///
/// Opening the camera can recreate the trip page. The widget state is lost,
/// but this store keeps the files so the same step can show them again.
class TripStepCaptureStore {
  TripStepCaptureStore._();

  static const _prefix = 'trip_step_capture_';
  static final Map<int, TripStepCaptureSnapshot> _memory = {};

  /// Fires after each [save] or [clear] so a newly created step widget can
  /// pick up a photo that finished after it was built.
  static final ValueNotifier<int> changes = ValueNotifier(0);

  static final Map<int, int> _generations = {};
  static final Map<int, Future<void>> _writeChains = {};

  static TripStepCaptureSnapshot? peek(int tripId) => _memory[tripId];

  static Future<void> save(int tripId, TripStepCaptureSnapshot snapshot) {
    _memory[tripId] = snapshot;
    changes.value++;
    final generation = (_generations[tripId] ?? 0) + 1;
    _generations[tripId] = generation;
    final encoded = jsonEncode(snapshot.toJson());
    final previous = _writeChains[tripId] ?? Future<void>.value();
    final next = previous.catchError((_) {}).then((_) async {
      if (_generations[tripId] != generation) return;
      final prefs = await SharedPreferences.getInstance();
      if (_generations[tripId] != generation) return;
      await prefs.setString('$_prefix$tripId', encoded);
    });
    _writeChains[tripId] = next;
    return next;
  }

  static Future<void> hydrate(int tripId) async {
    if (_memory.containsKey(tripId)) return;
    final prefs = await SharedPreferences.getInstance();
    final raw = prefs.getString('$_prefix$tripId');
    if (raw == null || raw.isEmpty) return;
    try {
      final decoded = jsonDecode(raw);
      if (decoded is! Map) return;
      _memory[tripId] = TripStepCaptureSnapshot.fromJson(
        Map<String, dynamic>.from(decoded),
      );
      changes.value++;
    } catch (_) {}
  }

  static Future<void> clear(int tripId) {
    _memory.remove(tripId);
    final generation = (_generations[tripId] ?? 0) + 1;
    _generations[tripId] = generation;
    changes.value++;
    final previous = _writeChains[tripId] ?? Future<void>.value();
    final next = previous.catchError((_) {}).then((_) async {
      if (_generations[tripId] != generation) return;
      final prefs = await SharedPreferences.getInstance();
      if (_generations[tripId] != generation) return;
      await prefs.remove('$_prefix$tripId');
    });
    _writeChains[tripId] = next;
    return next;
  }

  @visibleForTesting
  static void resetForTesting() {
    _memory.clear();
    _generations.clear();
    _writeChains.clear();
    changes.value = 0;
  }
}
