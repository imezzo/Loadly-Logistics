import 'package:dartz/dartz.dart';

import '../../data/models/collect_trip_documents_request_dto.dart';
import '../failures/trips_failure.dart';

abstract interface class TripsRepository {
  Future<Either<TripsFailure, void>> collectTripDocuments({
    required int tripId,
    required List<CollectTripDocumentsRequestDto> collectorDocuments,
    required bool submit,
  });
  Future<Either<TripsFailure, void>> confirmTripDocuments({
    required int tripId,
    required List<CollectTripDocumentsRequestDto> collectorDocuments,
    required bool submit,
  });
  Future<Either<TripsFailure, void>> bulkCollectTripDocuments({
    required List<int> tripIds,
  });
  Future<Either<TripsFailure, void>> bulkConfirmTripDocuments({
    required List<int> tripIds,
  });
}
