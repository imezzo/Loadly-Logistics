import 'package:dartz/dartz.dart';

import '../../../../core/errors/api_fallback_keys.dart';
import '../../../../core/errors/failure.dart';
import '../../../../core/network/repository_api_runner.dart';
import '../../domain/entities/driver/driver.dart';
import '../../domain/entities/driver_document_type/driver_document_type.dart';
import '../../domain/repositories/driver_repository.dart';
import '../../domain/entities/update_trip_driver/update_trip_driver.dart';
import '../datasources/driver_remote_datasource.dart';
import '../mappers/driver_document_type_mapper.dart';
import '../mappers/driver_mapper.dart';
import '../mappers/update_trip_driver_mapper.dart';
import '../models/driver_document_input.dart';
import '../models/driver_model.dart';

class DriverRepositoryImpl implements DriverRepository {
  DriverRepositoryImpl(this._remoteDataSource);

  final DriverRemoteDataSource _remoteDataSource;

  @override
  Future<Either<Failure, List<Driver>>> getDrivers({
    int page = 1,
    int pageSize = 10,
  }) {
    return runApiCall(
      () async {
        final result = await _remoteDataSource.getDrivers(
          page: page,
          pageSize: pageSize,
        );
        return result.map((e) => e.toEntity()).toList();
      },
      fallbackMessage: ApiFallbackKeys.loadDrivers,
    );
  }

  @override
  Future<Either<Failure, List<DriverDocumentType>>> getDocumentTypes({
    int page = 1,
    int pageSize = 15,
  }) {
    return runApiCall(
      () async {
        final result = await _remoteDataSource.getDocumentTypes(
          page: page,
          pageSize: pageSize,
        );
        return result.map((e) => e.toEntity()).toList();
      },
      fallbackMessage: ApiFallbackKeys.loadDocumentTypes,
    );
  }

  @override
  Future<Either<Failure, DriverModel>> createDriver({
    required String fullNameEn,
    required String fullNameAr,
    required String phone,
    required String nationalId,
    required List<DriverDocumentInput> documents,
  }) {
    return runApiCall(
      () async {
        final driver = await _remoteDataSource.createDriver(
          fullNameEn: fullNameEn,
          fullNameAr: fullNameAr,
          phone: phone,
          nationalId: nationalId,
          documents: documents,
        );
        if (driver == null) {
          throw Exception(ApiFallbackKeys.createDriver);
        }
        return driver;
      },
      fallbackMessage: ApiFallbackKeys.createDriver,
    );
  }

  @override
  Future<Either<Failure, void>> updateTripDriver(UpdateTripDriver request) {
    return runApiCall(
      () => _remoteDataSource.updateTripDriver(
        tripId: request.tripId,
        request: request.toDto(),
      ),
      fallbackMessage: ApiFallbackKeys.updateTripDriver,
    );
  }
}
