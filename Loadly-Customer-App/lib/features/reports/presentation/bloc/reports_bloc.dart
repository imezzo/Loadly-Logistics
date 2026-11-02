import 'dart:async';

import 'package:flutter_bloc/flutter_bloc.dart';

import '../../domain/entities/get_invoice_statistics_result.dart';
import '../../domain/entities/get_trip_cost_period_result.dart';
import '../../domain/entities/invoice.dart';
import '../../domain/entities/invoice_payment_request.dart';
import '../../domain/failures/reports_failure.dart';
import '../../domain/use_cases/get_invoice_payment_requests_usecase.dart';
import '../../domain/use_cases/get_invoice_statistics_usecase.dart';
import '../../domain/use_cases/get_invoices_usecase.dart';
import '../../domain/use_cases/get_trip_cost_period_usecase.dart';
import 'reports_event.dart';
import 'reports_state.dart';

class ReportsBloc extends Bloc<ReportsEvent, ReportsState> {
  ReportsBloc({
    required this.getTripCostPeriodUseCase,
    required this.getInvoiceStatisticsUseCase,
    required this.getInvoicesUseCase,
    required this.getInvoicePaymentRequestsUseCase,
  }) : super(const ReportsInitial()) {
    on<ReportsGetTripCostPeriod>(_onGetTripCostPeriod);
    on<ReportsGetInvoiceStatistics>(_onGetInvoiceStatistics);
    on<ReportsGetInvoices>(_onGetInvoices);
    on<ReportsInvoicePaymentRequests>(_onGetInvoicePaymentRequests);
  }

  final GetTripCostPeriodUseCase getTripCostPeriodUseCase;
  final GetInvoiceStatisticsUseCase getInvoiceStatisticsUseCase;
  final GetInvoicesUseCase getInvoicesUseCase;
  final GetInvoicePaymentRequestsUseCase getInvoicePaymentRequestsUseCase;

  Future<void> _onGetTripCostPeriod(
    ReportsGetTripCostPeriod event,
    Emitter<ReportsState> emit,
  ) async {
    emit(const TripCostPeriodLoading());

    final getTripCostPeriodResult = await getTripCostPeriodUseCase(
      GetTripCostPeriodParams(),
    );

    getTripCostPeriodResult.fold(
      (failure) {
        final errorMessage = failure is ReportsFailureServer
            ? failure.message ?? 'Failed to load Trip Cost Period'
            : failure is ReportsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(
          TripCostPeriodLoaded(
            tripCostPeriod: const GetTripCostPeriodResult(
              totalCostLastSevenDays: 0,
              totalCostLastThirtyDays: 0,
              totalCostLastThreeMonths: 0,
            ),
            error: errorMessage,
          ),
        );
      },
      (tripCostPeriod) {
        emit(TripCostPeriodLoaded(tripCostPeriod: tripCostPeriod, error: null));
      },
    );
  }

  Future<void> _onGetInvoiceStatistics(
    ReportsGetInvoiceStatistics event,
    Emitter<ReportsState> emit,
  ) async {
    emit(const InvoiceStatisticsLoading());

    final getInvoiceStatisticsResult = await getInvoiceStatisticsUseCase(
      GetInvoiceStatisticsParams(),
    );

    getInvoiceStatisticsResult.fold(
      (failure) {
        final errorMessage = failure is ReportsFailureServer
            ? failure.message ?? 'Failed to load Trip Cost Period'
            : failure is ReportsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(
          InvoiceStatisticsLoaded(
            invoiceStatistics: const GetInvoiceStatisticsResult(
              paidInvoicesCount: 0,
              unpaidInvoicesCount: 0,
              partiallyPaidInvoicesCount: 0,
              totalInvoicesAmount: 0,
              totalPaidAmount: 0,
              totalRemainingAmount: 0,
            ),
            error: errorMessage,
          ),
        );
      },
      (invoiceStatistics) {
        emit(
          InvoiceStatisticsLoaded(
            invoiceStatistics: invoiceStatistics,
            error: null,
          ),
        );
      },
    );
  }

  Future<void> _onGetInvoices(
    ReportsGetInvoices event,
    Emitter<ReportsState> emit,
  ) async {
    emit(const InvoicesLoading());

    final result = await getInvoicesUseCase(
      GetInvoicesParams(
        page: event.page,
        pageSize: event.pageSize,
        search: event.search,
        contractId: event.contractId,
        status: event.status,
        invoiceType: event.invoiceType,
        startDate: event.startDate,
        endDate: event.endDate,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is ReportsFailureServer
            ? failure.message ?? 'Failed to load Invoices'
            : failure is ReportsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(
          InvoicesLoaded(invoiceList: const InvoiceList(), error: errorMessage),
        );
      },
      (invoices) {
        emit(InvoicesLoaded(invoiceList: invoices, error: null));
      },
    );
  }

  Future<void> _onGetInvoicePaymentRequests(
    ReportsInvoicePaymentRequests event,
    Emitter<ReportsState> emit,
  ) async {
    emit(const InvoicePaymentRequestsLoading());

    final result = await getInvoicePaymentRequestsUseCase(
      GetInvoicePaymentRequestsParams(
        page: event.page,
        pageSize: event.pageSize,
        search: event.search,
        invoiceId: event.invoiceId,
        status: event.status,
        startDate: event.startDate,
        endDate: event.endDate,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is ReportsFailureServer
            ? failure.message ?? 'Failed to load invoice Payment Requests'
            : failure is ReportsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(
          InvoicePaymentRequestsLoaded(
            invoicePaymentRequestList: const InvoicePaymentRequestList(),
            error: errorMessage,
          ),
        );
      },
      (invoices) {
        emit(
          InvoicePaymentRequestsLoaded(
            invoicePaymentRequestList: invoices,
            error: null,
          ),
        );
      },
    );
  }
}
