import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/location/trip_gps_session_reconciler.dart';
import 'package:loadly_driver/core/location/trip_odometer_service.dart';
import 'package:loadly_driver/core/location/unified_driver_location_hub.dart';
import 'package:loadly_driver/core/database/app_database.dart';
import 'package:loadly_driver/features/trips/domain/entities/trip.dart';
import 'package:loadly_driver/features/trips/domain/failures/trips_failure.dart';
import 'package:loadly_driver/features/trips/domain/repositories/trips_repository.dart';
import 'package:loadly_driver/features/trips/data/sources/remote/trips_remote_data_source.dart';
import 'package:loadly_driver/core/database/trip_gps_sync_service.dart';

import 'package:drift/native.dart';

class _StubTripsRemote implements TripsRemoteDataSource {
  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

Trip _minimalTrip({
  required int id,
  int? statusCode,
  int? lifecycleActor,
  String? lifecycleActorName,
}) {
  return Trip(
    cutoffDuration: 0,
    id: id,
    title: 'Test',
    startDateTime: '2026-01-01',
    startDate: '2026-01-01',
    startTime: '12:00',
    pickupLocation: const Location(address: 'A'),
    deliveryLocation: const Location(address: 'B'),
    statusCode: statusCode,
    lifecycleActor: lifecycleActor,
    lifecycleActorName: lifecycleActorName,
  );
}

class _FakeTripsRepository implements TripsRepository {
  _FakeTripsRepository(this.trip);

  Trip trip;

  @override
  Future<Either<TripsFailure, Trip>> getTripDetails(int tripId) async {
    return Right(trip);
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

void main() {
  late AppDatabase db;
  late TripOdometerService odometer;
  late _FakeTripsRepository repository;
  late TripGpsSessionReconciler reconciler;

  setUp(() async {
    db = AppDatabase.forTesting(NativeDatabase.memory());
    odometer = TripOdometerService(
      () async => db,
      UnifiedDriverLocationHub(),
      () async => TripGpsSyncService(db, _StubTripsRemote()),
    );
    repository = _FakeTripsRepository(
      _minimalTrip(id: 42, statusCode: kTripStatusCodeCompleted),
    );
    reconciler = TripGpsSessionReconciler(odometer, repository);
    await db.beginTripSession(42);
    await odometer.init();
  });

  tearDown(() async {
    await odometer.dispose();
    await db.close();
  });

  test('reconcileOpenSessionFromServer finalizes when server trip is terminal',
      () async {
    await reconciler.reconcileOpenSessionFromServer();

    final session = await db.getSession(42);
    expect(session?.endedAt, isNotNull);
    expect(odometer.isTrackingTrip(42), isFalse);
  });

  test('reconcileFromTrips uses list trip without extra fetch', () async {
    var fetchCount = 0;
    final countingRepo = _CountingTripsRepository(
      _minimalTrip(id: 42, statusCode: kTripStatusCodeCompleted),
      onFetch: () => fetchCount++,
    );
    final localReconciler = TripGpsSessionReconciler(odometer, countingRepo);

    await localReconciler.reconcileFromTrips([
      _minimalTrip(id: 99, statusCode: kTripStatusCodeInProgress),
      _minimalTrip(id: 42, statusCode: kTripStatusCodeCompleted),
    ]);

    expect(fetchCount, 0);
    final session = await db.getSession(42);
    expect(session?.endedAt, isNotNull);
  });

  test('admin completion discards local packets without route sync', () async {
    await db.insertPacket(
      tripId: 42,
      recordedAt: DateTime.utc(2026, 1, 1),
      latitude: 24,
      longitude: 46,
      accuracyM: 5,
    );
    repository = _FakeTripsRepository(
      _minimalTrip(
        id: 42,
        statusCode: kTripStatusCodeCompleted,
        lifecycleActor: 2,
        lifecycleActorName: 'Admin',
      ),
    );
    reconciler = TripGpsSessionReconciler(odometer, repository);

    await reconciler.reconcileOpenSessionFromServer();

    expect(await db.packetsForTrip(42), isEmpty);
    final session = await db.getSession(42);
    expect(session?.syncStatus, 'discarded');
  });
}

class _CountingTripsRepository implements TripsRepository {
  _CountingTripsRepository(this.trip, {required this.onFetch});

  final Trip trip;
  final void Function() onFetch;

  @override
  Future<Either<TripsFailure, Trip>> getTripDetails(int tripId) async {
    onFetch();
    return Right(trip);
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}
