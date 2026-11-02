import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/usecase/usecase.dart';
import '../entities/driver/driver.dart';
import '../repositories/driver_repository.dart';

class GetDriversParams {
  const GetDriversParams({this.page = 1, this.pageSize = 10});

  final int page;
  final int pageSize;
}

class GetDriversUseCase implements UseCase<List<Driver>, GetDriversParams> {
  GetDriversUseCase(this._repository);

  final DriverRepository _repository;

  @override
  Future<Either<Failure, List<Driver>>> call(GetDriversParams params) {
    return _repository.getDrivers(
      page: params.page,
      pageSize: params.pageSize,
    );
  }
}
