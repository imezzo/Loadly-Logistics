import 'package:dartz/dartz.dart';

import '../../../../core/errors/api_fallback_keys.dart';
import '../../../../core/errors/failure.dart';
import '../../../../core/network/repository_api_runner.dart';
import '../../data/datasources/lookups_remote_datasource.dart';
import '../../data/models/lookup_item_dto.dart';

class GetDefaultVehicleTypesUseCase {
  GetDefaultVehicleTypesUseCase(this._lookups);

  final LookupsRemoteDataSource _lookups;

  Future<Either<Failure, List<LookupItemDto>>> call({
    int page = 1,
    int limit = 20,
  }) {
    return runApiCall(
      () => _lookups.getDefaultVehicleTypes(page: page, limit: limit),
      fallbackMessage: ApiFallbackKeys.loadVehicleTypes,
    );
  }
}