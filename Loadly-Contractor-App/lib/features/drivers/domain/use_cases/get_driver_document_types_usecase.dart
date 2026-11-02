import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/usecase/usecase.dart';
import '../entities/driver_document_type/driver_document_type.dart';
import '../repositories/driver_repository.dart';

class GetDriverDocumentTypesParams {
  const GetDriverDocumentTypesParams({this.page = 1, this.pageSize = 15});

  final int page;
  final int pageSize;
}

class GetDriverDocumentTypesUseCase
    implements
        UseCase<List<DriverDocumentType>, GetDriverDocumentTypesParams> {
  GetDriverDocumentTypesUseCase(this._repository);

  final DriverRepository _repository;

  @override
  Future<Either<Failure, List<DriverDocumentType>>> call(
    GetDriverDocumentTypesParams params,
  ) {
    return _repository.getDocumentTypes(
      page: params.page,
      pageSize: params.pageSize,
    );
  }
}
