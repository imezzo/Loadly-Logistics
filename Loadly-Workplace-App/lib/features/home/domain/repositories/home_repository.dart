import 'package:dartz/dartz.dart';
import '../entities/collector_trips.dart';
import '../entities/document.dart';

import '../failures/home_failure.dart';

abstract interface class HomeRepository {
  Future<Either<HomeFailure, CollectorTripsList>> getCollectorTrips({
    int? page,
    int? pageSize,
    String? search,
    int? contractId,
    String? contractReferenceNumber,
    int? companyId,
    int? vehicleId,
    int? vehicleTypeId,
    int? driverId,
    int? status,
    List<int>? tripDocumentStatuses,
    String? startDate,
    String? endDate,
    String? companyName,
    String? driverName,
    String? plate,
  });
  Future<Either<HomeFailure, List<Document>>> getDocuments();
}
