import 'package:drift/drift.dart';

class TripGpsSessions extends Table {
  IntColumn get tripId => integer()();

  DateTimeColumn get startedAt => dateTime()();

  DateTimeColumn get endedAt => dateTime().nullable()();

  RealColumn get totalMeters => real().withDefault(const Constant(0))();

  /// pending | synced — for future end-of-trip API upload.
  TextColumn get syncStatus => text().withDefault(const Constant('pending'))();

  @override
  Set<Column<Object>> get primaryKey => {tripId};
}

@TableIndex(name: 'idx_gps_packets_trip_recorded', columns: {#tripId, #recordedAt})
class GpsPackets extends Table {
  IntColumn get id => integer().autoIncrement()();

  IntColumn get tripId => integer()();

  DateTimeColumn get recordedAt => dateTime()();

  RealColumn get latitude => real()();

  RealColumn get longitude => real()();

  RealColumn get accuracyM => real().nullable()();

  RealColumn get speedMps => real().nullable()();

  /// Clockwise degrees from north [0, 360); null when unknown (e.g. first point).
  RealColumn get headingDeg => real().nullable()();

  /// Great-circle distance from the previous stored packet (0 for first point).
  RealColumn get distanceFromPreviousM => real().nullable()();

  /// live | background | debug_simulate
  TextColumn get source => text().withDefault(const Constant('live'))();
}
