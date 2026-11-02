import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/invoice_payment_request.dart';
import '../failures/reports_failure.dart';
import '../repositories/reports_repository.dart';

class GetInvoicePaymentRequestsUseCase
    implements
        UseCase<InvoicePaymentRequestList, GetInvoicePaymentRequestsParams> {
  GetInvoicePaymentRequestsUseCase(this._repository);

  final ReportsRepository _repository;

  @override
  Future<Either<ReportsFailure, InvoicePaymentRequestList>> call(
    GetInvoicePaymentRequestsParams params,
  ) {
    return _repository.getInvoicesPaymentRequests(
      page: params.page,
      pageSize: params.pageSize,
      search: params.search,
      invoiceId: params.invoiceId,
      status: params.status,
      startDate: params.startDate,
      endDate: params.endDate,
    );
  }
}

class GetInvoicePaymentRequestsParams {
  final int? page;
  final int? pageSize;
  final String? search;
  final int? invoiceId;
  final int? status;
  final String? startDate;
  final String? endDate;

  GetInvoicePaymentRequestsParams({
    this.page,
    this.pageSize,
    this.search,
    this.invoiceId,
    this.status,
    this.startDate,
    this.endDate,
  });
}
