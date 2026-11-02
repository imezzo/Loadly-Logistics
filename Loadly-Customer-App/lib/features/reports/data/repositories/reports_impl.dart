import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import '../../domain/entities/get_invoice_statistics_result.dart';
import '../../domain/entities/get_trip_cost_period_result.dart';
import '../../domain/entities/invoice.dart';
import '../../domain/entities/invoice_payment_request.dart';
import '../../domain/failures/reports_failure.dart';
import '../../domain/repositories/reports_repository.dart';
import '../mappers/invoice_mapper.dart';
import '../mappers/invoice_payment_request_mapper.dart';
import '../sources/remote/reports_remote_data_source.dart';

class ReportsImpl implements ReportsRepository {
  ReportsImpl(this._remote);

  final ReportsRemoteDataSource _remote;

  ReportsFailure _serverFailure(DioException e) {
    final statusCode = e.response?.statusCode;
    final message = e.error is String
        ? e.error as String
        : (e.response?.data is Map
              ? (e.response?.data as Map)['message']?.toString()
              : null);
    return ReportsFailureServer(message: message, statusCode: statusCode);
  }

  @override
  Future<Either<ReportsFailure, GetTripCostPeriodResult>>
  getTripCostPeriod() async {
    try {
      final dto = await _remote.getTripCostPeriod();
      return Right(
        GetTripCostPeriodResult(
          totalCostLastSevenDays: dto.totalCostLastSevenDays,
          totalCostLastThirtyDays: dto.totalCostLastThirtyDays,
          totalCostLastThreeMonths: dto.totalCostLastThreeMonths,
        ),
      );
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(ReportsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(ReportsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<ReportsFailure, GetInvoiceStatisticsResult>>
  getInvoiceStatistics() async {
    try {
      final dto = await _remote.getInvoiceStatistics();
      return Right(
        GetInvoiceStatisticsResult(
          paidInvoicesCount: dto.paidInvoicesCount,
          unpaidInvoicesCount: dto.unpaidInvoicesCount,
          partiallyPaidInvoicesCount: dto.partiallyPaidInvoicesCount,
          totalInvoicesAmount: dto.totalInvoicesAmount,
          totalPaidAmount: dto.totalPaidAmount,
          totalRemainingAmount: dto.totalRemainingAmount,
        ),
      );
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(ReportsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(ReportsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<ReportsFailure, InvoiceList>> getInvoices({
    int? page,
    int? pageSize,
    String? search,
    int? contractId,
    int? status,
    int? invoiceType,
    String? startDate,
    String? endDate,
  }) async {
    try {
      final response = await _remote.getInvoices(
        page: page,
        pageSize: pageSize,
        search: search,
        status: status,
        contractId: contractId,
        invoiceType: invoiceType,
        startDate: startDate,
        endDate: endDate,
      );
      return Right(response.toEntity());
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(ReportsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(ReportsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<ReportsFailure, InvoicePaymentRequestList>>
  getInvoicesPaymentRequests({
    int? page,
    int? pageSize,
    int? invoiceId,
    int? status,
    String? startDate,
    String? endDate,
    String? search,
  }) async {
    try {
      final response = await _remote.getInvoicesPaymentRequests(
        page: page,
        pageSize: pageSize,
        search: search,
        status: status,
        invoiceId: invoiceId,
        startDate: startDate,
        endDate: endDate,
      );
      return Right(response.toEntity());
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(ReportsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(ReportsFailureUnknown(message: e.toString()));
    }
  }
}
