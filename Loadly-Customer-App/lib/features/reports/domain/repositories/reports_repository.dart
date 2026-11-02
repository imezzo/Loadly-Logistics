import 'package:dartz/dartz.dart';

import '../entities/get_invoice_statistics_result.dart';
import '../entities/get_trip_cost_period_result.dart';
import '../entities/invoice.dart';
import '../entities/invoice_payment_request.dart';
import '../failures/reports_failure.dart';

abstract interface class ReportsRepository {
  Future<Either<ReportsFailure, GetTripCostPeriodResult>> getTripCostPeriod();
  Future<Either<ReportsFailure, GetInvoiceStatisticsResult>>
  getInvoiceStatistics();
  Future<Either<ReportsFailure, InvoiceList>> getInvoices({
    int? page,
    int? pageSize,
    String? search,
    int? contractId,
    int? status,
    int? invoiceType,
    String? startDate,
    String? endDate,
  });
  Future<Either<ReportsFailure, InvoicePaymentRequestList>>
  getInvoicesPaymentRequests({
    int? page,
    int? pageSize,
    int? invoiceId,
    int? status,
    String? startDate,
    String? endDate,
    String? search,
  });
}
