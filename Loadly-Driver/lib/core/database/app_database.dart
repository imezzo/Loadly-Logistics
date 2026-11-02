import 'dart:io';

import 'package:drift/drift.dart';
import 'package:drift/native.dart';
import 'package:path/path.dart' as p;
import 'package:path_provider/path_provider.dart';
import 'package:sqlite3/sqlite3.dart';

import 'tables/gps_trace_tables.dart';

part 'app_database.g.dart';

@DriftDatabase(tables: [TripGpsSessions, GpsPackets])
class AppDatabase extends _$AppDatabase {
  AppDatabase(super.e);

  AppDatabase.forTesting(super.e);

  @override
  int get schemaVersion => 3;

  @override
  MigrationStrategy get migration => MigrationStrategy(
    onUpgrade: (m, from, to) async {
      if (from < 2) {
        await m.createIndex(
          Index(
            'idx_gps_packets_trip_recorded',
            'CREATE INDEX IF NOT EXISTS idx_gps_packets_trip_recorded ON gps_packets (trip_id, recorded_at)',
          ),
        );
      }
      if (from < 3) {
        await m.addColumn(gpsPackets, gpsPackets.headingDeg);
        await m.addColumn(gpsPackets, gpsPackets.distanceFromPreviousM);
      }
    },
  );

  static AppDatabase? _instance;

  static Future<AppDatabase> open() async {
    if (_instance != null) return _instance!;
    _instance = AppDatabase(
      NativeDatabase.createInBackground(
        await _databaseFile(),
        setup: configureTripGpsSqlite,
      ),
    );
    return _instance!;
  }

  /// Second connection for the Android trip-GPS foreground isolate.
  ///
  /// Not cached. The caller closes it when that isolate's task ends.
  static Future<AppDatabase> openDedicated() async {
    return AppDatabase(
      NativeDatabase(await _databaseFile(), setup: configureTripGpsSqlite),
    );
  }

  static Future<File> _databaseFile() async {
    final dir = await getApplicationDocumentsDirectory();
    return File(p.join(dir.path, 'loadly_driver_gps.sqlite'));
  }

  Future<TripGpsSession?> getActiveSession() {
    return (select(tripGpsSessions)
          ..where((t) => t.endedAt.isNull()))
        .getSingleOrNull();
  }

  Future<TripGpsSession?> getSession(int tripId) {
    return (select(tripGpsSessions)..where((t) => t.tripId.equals(tripId)))
        .getSingleOrNull();
  }

  Future<void> beginTripSession(int tripId) async {
    await (delete(gpsPackets)..where((t) => t.tripId.equals(tripId))).go();
    await into(tripGpsSessions).insertOnConflictUpdate(
      TripGpsSessionsCompanion(
        tripId: Value(tripId),
        startedAt: Value(DateTime.now().toUtc()),
        endedAt: const Value(null),
        totalMeters: const Value(0),
        syncStatus: const Value('pending'),
      ),
    );
  }

  Future<void> endSession({
    required int tripId,
    required double totalMeters,
  }) async {
    await (update(tripGpsSessions)..where((t) => t.tripId.equals(tripId))).write(
      TripGpsSessionsCompanion(
        endedAt: Value(DateTime.now().toUtc()),
        totalMeters: Value(totalMeters),
      ),
    );
  }

  Future<void> updateSessionTotalMeters(int tripId, double totalMeters) async {
    await (update(tripGpsSessions)..where((t) => t.tripId.equals(tripId))).write(
      TripGpsSessionsCompanion(totalMeters: Value(totalMeters)),
    );
  }

  Future<int> insertPacket({
    required int tripId,
    required DateTime recordedAt,
    required double latitude,
    required double longitude,
    double? accuracyM,
    double? speedMps,
    double? headingDeg,
    double? distanceFromPreviousM,
    String source = 'live',
  }) {
    return into(gpsPackets).insert(
      GpsPacketsCompanion.insert(
        tripId: tripId,
        recordedAt: recordedAt.toUtc(),
        latitude: latitude,
        longitude: longitude,
        accuracyM: Value(accuracyM),
        speedMps: Value(speedMps),
        headingDeg: Value(headingDeg),
        distanceFromPreviousM: Value(distanceFromPreviousM),
        source: Value(source),
      ),
    );
  }

  Future<List<GpsPacket>> packetsForTrip(int tripId) {
    return (select(gpsPackets)
          ..where((t) => t.tripId.equals(tripId))
          ..orderBy([(t) => OrderingTerm.asc(t.recordedAt)]))
        .get();
  }

  Future<void> markSessionSynced(int tripId) async {
    await (update(tripGpsSessions)..where((t) => t.tripId.equals(tripId))).write(
      const TripGpsSessionsCompanion(syncStatus: Value('synced')),
    );
    await _deletePacketsForTrip(tripId);
  }

  /// Drops local GPS packets without uploading (e.g. admin completed the trip).
  Future<void> discardLocalGpsTrace(int tripId) async {
    await (update(tripGpsSessions)..where((t) => t.tripId.equals(tripId))).write(
      const TripGpsSessionsCompanion(syncStatus: Value('discarded')),
    );
    await _deletePacketsForTrip(tripId);
  }

  Future<void> _deletePacketsForTrip(int tripId) async {
    await (delete(gpsPackets)..where((t) => t.tripId.equals(tripId))).go();
  }

  /// Payload for future end-of-trip API (not wired yet).
  Future<TripGpsUploadPayload?> buildUploadPayload(int tripId) async {
    final session = await getSession(tripId);
    if (session == null) return null;
    final packets = await packetsForTrip(tripId);
    return TripGpsUploadPayload(
      tripId: tripId,
      totalMeters: session.totalMeters,
      startedAt: session.startedAt,
      endedAt: session.endedAt,
      packets: packets,
    );
  }
}

class TripGpsUploadPayload {
  const TripGpsUploadPayload({
    required this.tripId,
    required this.totalMeters,
    required this.startedAt,
    required this.endedAt,
    required this.packets,
  });

  final int tripId;
  final double totalMeters;
  final DateTime startedAt;
  final DateTime? endedAt;
  final List<GpsPacket> packets;

  double get totalKm => totalMeters / 1000;
}

/// WAL lets the UI isolate read packets while the foreground isolate writes them.
void configureTripGpsSqlite(Database database) {
  database.execute('PRAGMA journal_mode=WAL');
  database.execute('PRAGMA busy_timeout=5000');
}
