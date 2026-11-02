import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/get_invoice_statistics_result.dart';
import '../failures/reports_failure.dart';
import '../repositories/reports_repository.dart';

class GetInvoiceStatisticsUseCase
    implements UseCase<GetInvoiceStatisticsResult, GetInvoiceStatisticsParams> {
  GetInvoiceStatisticsUseCase(this._repository);

  final ReportsRepository _repository;

  @override
  Future<Either<ReportsFailure, GetInvoiceStatisticsResult>> call(
    GetInvoiceStatisticsParams params,
  ) {
    return _repository.getInvoiceStatistics();
  }
}

class GetInvoiceStatisticsParams {}
