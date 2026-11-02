import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/invoice.dart';
import '../failures/reports_failure.dart';
import '../repositories/reports_repository.dart';

class GetInvoicesUseCase implements UseCase<InvoiceList, GetInvoicesParams> {
  GetInvoicesUseCase(this._repository);

  final ReportsRepository _repository;

  @override
  Future<Either<ReportsFailure, InvoiceList>> call(GetInvoicesParams params) {
    return _repository.getInvoices(
      page: params.page,
      pageSize: params.pageSize,
      search: params.search,
      contractId: params.contractId,
      status: params.status,
      invoiceType: params.invoiceType,
      startDate: params.startDate,
      endDate: params.endDate,
    );
  }
}

class GetInvoicesParams {
  final int? page;
  final int? pageSize;
  final String? search;
  final int? contractId;
  final int? status;
  final int? invoiceType;
  final String? startDate;
  final String? endDate;

  GetInvoicesParams({
    this.page,
    this.pageSize,
    this.search,
    this.contractId,
    this.status,
    this.invoiceType,
    this.startDate,
    this.endDate,
  });
}
