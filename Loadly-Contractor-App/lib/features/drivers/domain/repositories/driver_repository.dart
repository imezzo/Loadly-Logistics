import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../data/models/driver_document_input.dart';
import '../../data/models/driver_model.dart';
import '../entities/driver/driver.dart';
import '../entities/driver_document_type/driver_document_type.dart';
import '../entities/update_trip_driver/update_trip_driver.dart';

abstract interface class DriverRepository {
  Future<Either<Failure, List<Driver>>> getDrivers({
    int page = 1,
    int pageSize = 10,
  });

  Future<Either<Failure, List<DriverDocumentType>>> getDocumentTypes({
    int page = 1,
    int pageSize = 15,
  });

  Future<Either<Failure, DriverModel>> createDriver({
    required String fullNameEn,
    required String fullNameAr,
    required String phone,
    required String nationalId,
    required List<DriverDocumentInput> documents,
  });

  Future<Either<Failure, void>> updateTripDriver(UpdateTripDriver request);
}
