// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

/// Events for Home BLoC
abstract class ReportsEvent extends Equatable {
  const ReportsEvent();

  @override
  List<Object?> get props => [];
}

class ReportsGetTripCostPeriod extends ReportsEvent {
  const ReportsGetTripCostPeriod();

  @override
  List<Object?> get props => [];
}

class ReportsGetInvoiceStatistics extends ReportsEvent {
  const ReportsGetInvoiceStatistics();

  @override
  List<Object?> get props => [];
}

class ReportsGetInvoices extends ReportsEvent {
  const ReportsGetInvoices({
    this.page,
    this.pageSize,
    this.search,
    this.contractId,
    this.status,
    this.invoiceType,
    this.startDate,
    this.endDate,
  });
  final int? page;
  final int? pageSize;
  final String? search;
  final int? contractId;
  final int? status;
  final int? invoiceType;
  final String? startDate;
  final String? endDate;

  @override
  List<Object?> get props => [
    page,
    pageSize,
    search,
    contractId,
    status,
    invoiceType,
    startDate,
    endDate,
  ];
}

class ReportsInvoicePaymentRequests extends ReportsEvent {
  const ReportsInvoicePaymentRequests({
    this.page,
    this.pageSize,
    this.search,
    this.invoiceId,
    this.status,
    this.startDate,
    this.endDate,
  });
  final int? page;
  final int? pageSize;
  final String? search;
  final int? invoiceId;
  final int? status;
  final String? startDate;
  final String? endDate;

  @override
  List<Object?> get props => [
    page,
    pageSize,
    search,
    invoiceId,
    status,
    startDate,
    endDate,
  ];
}
