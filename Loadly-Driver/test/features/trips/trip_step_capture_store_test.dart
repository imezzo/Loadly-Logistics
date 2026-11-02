import 'dart:io';

import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:loadly_driver/features/trips/presentation/trip_step_capture_store.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  setUp(() {
    SharedPreferences.setMockInitialValues({});
    TripStepCaptureStore.resetForTesting();
  });

  tearDown(TripStepCaptureStore.resetForTesting);

  test('keeps captured photos in memory for a reloaded step', () async {
    const tripId = 12;
    final file = File('${Directory.systemTemp.path}/loadly_capture_test.jpg');
    await file.writeAsString('photo');
    addTearDown(() async {
      if (file.existsSync()) await file.delete();
    });

    await TripStepCaptureStore.save(
      tripId,
      TripStepCaptureSnapshot(
        files: {
          TripCaptureSlots.receiptNote: [file.path],
        },
        pendingSlot: TripCaptureSlots.receiptNote,
      ),
    );

    final restored = TripStepCaptureStore.peek(tripId);
    expect(restored?.pendingSlot, TripCaptureSlots.receiptNote);
    expect(restored?.existingFiles(TripCaptureSlots.receiptNote), hasLength(1));
    expect(
      restored?.existingFiles(TripCaptureSlots.receiptNote).single.path,
      file.path,
    );
  });

  test(
    'reloads the photo from disk after the in-memory copy is cleared',
    () async {
      const tripId = 12;
      await TripStepCaptureStore.save(
        tripId,
        const TripStepCaptureSnapshot(
          files: {
            TripCaptureSlots.shipment: ['/missing/photo.jpg'],
          },
        ),
      );

      TripStepCaptureStore.resetForTesting();
      await TripStepCaptureStore.hydrate(tripId);

      final restored = TripStepCaptureStore.peek(tripId);
      expect(restored?.files[TripCaptureSlots.shipment], [
        '/missing/photo.jpg',
      ]);
      expect(restored?.existingFiles(TripCaptureSlots.shipment), isEmpty);
    },
  );

  test('clear removes the saved step photos', () async {
    const tripId = 4;
    await TripStepCaptureStore.save(
      tripId,
      const TripStepCaptureSnapshot(pendingSlot: TripCaptureSlots.tollgate),
    );

    await TripStepCaptureStore.clear(tripId);
    TripStepCaptureStore.resetForTesting();
    await TripStepCaptureStore.hydrate(tripId);

    expect(TripStepCaptureStore.peek(tripId), isNull);
  });
}
