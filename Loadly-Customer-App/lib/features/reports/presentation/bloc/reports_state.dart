// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

import '../../domain/entities/get_invoice_statistics_result.dart';
import '../../domain/entities/get_trip_cost_period_result.dart';
import '../../domain/entities/invoice.dart';
import '../../domain/entities/invoice_payment_request.dart';

abstract class ReportsState extends Equatable {
  const ReportsState();

  @override
  List<Object?> get props => [];
}

/// Initial state
class ReportsInitial extends ReportsState {
  const ReportsInitial();
}

class TripCostPeriodLoading extends ReportsState {
  const TripCostPeriodLoading();
}

class TripCostPeriodLoaded extends ReportsState {
  const TripCostPeriodLoaded({required this.tripCostPeriod, this.error});

  final GetTripCostPeriodResult tripCostPeriod;
  final String? error;

  @override
  List<Object?> get props => [tripCostPeriod, error];
}

class InvoiceStatisticsLoading extends ReportsState {
  const InvoiceStatisticsLoading();
}

class InvoiceStatisticsLoaded extends ReportsState {
  const InvoiceStatisticsLoaded({required this.invoiceStatistics, this.error});

  final GetInvoiceStatisticsResult invoiceStatistics;
  final String? error;

  @override
  List<Object?> get props => [invoiceStatistics, error];
}

class InvoicesLoading extends ReportsState {
  const InvoicesLoading();
}

class InvoicesLoaded extends ReportsState {
  const InvoicesLoaded({required this.invoiceList, this.error});

  final InvoiceList invoiceList;
  final String? error;

  @override
  List<Object?> get props => [invoiceList, error];
}

class InvoicePaymentRequestsLoading extends ReportsState {
  const InvoicePaymentRequestsLoading();
}

class InvoicePaymentRequestsLoaded extends ReportsState {
  const InvoicePaymentRequestsLoaded({
    required this.invoicePaymentRequestList,
    this.error,
  });

  final InvoicePaymentRequestList invoicePaymentRequestList;
  final String? error;

  @override
  List<Object?> get props => [invoicePaymentRequestList, error];
}
