import 'package:drift/native.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/database/app_database.dart';
import 'package:loadly_driver/core/location/trip_gps_trace_writer.dart';

void main() {
  late AppDatabase db;

  setUp(() {
    db = AppDatabase.forTesting(NativeDatabase.memory());
  });

  tearDown(() async {
    await db.close();
  });

  test('background packets use the same distance rules as live packets', () async {
    final writer = TripGpsTraceWriter();
    final start = DateTime.utc(2026, 6, 1, 8);

    expect(
      await writer.record(
        db: db,
        tripId: 7,
        latitude: 24,
        longitude: 46,
        timestamp: start,
        accuracyMeters: 8,
        speedMps: 0,
        source: 'background',
      ),
      isTrue,
    );

    expect(
      await writer.record(
        db: db,
        tripId: 7,
        latitude: 24.00001,
        longitude: 46,
        timestamp: start.add(const Duration(seconds: 2)),
        accuracyMeters: 8,
        speedMps: 1,
        source: 'background',
      ),
      isFalse,
    );

    expect(
      await writer.record(
        db: db,
        tripId: 7,
        latitude: 24 + (200 / 111320),
        longitude: 46,
        timestamp: start.add(const Duration(seconds: 45)),
        accuracyMeters: 8,
        speedMps: 5,
        source: 'background',
      ),
      isTrue,
    );

    expect(
      await writer.record(
        db: db,
        tripId: 7,
        latitude: 24.01,
        longitude: 46,
        timestamp: start.add(const Duration(seconds: 60)),
        accuracyMeters: 250,
        speedMps: 5,
        source: 'background',
      ),
      isFalse,
    );

    final packets = await db.packetsForTrip(7);
    expect(packets, hasLength(2));
    expect(packets.last.source, 'background');
    expect(writer.totalMeters, closeTo(200, 5));

    final resumed = TripGpsTraceWriter()..seedFromPackets(packets);
    expect(resumed.totalMeters, closeTo(writer.totalMeters, 0.01));
    expect(resumed.lastPersistedLat, packets.last.latitude);
  });
}
