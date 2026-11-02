import 'package:equatable/equatable.dart';

class GetInvoiceStatisticsResult extends Equatable {
  final int paidInvoicesCount;
  final int unpaidInvoicesCount;
  final int partiallyPaidInvoicesCount;
  final double totalInvoicesAmount;
  final double totalPaidAmount;
  final double totalRemainingAmount;

  const GetInvoiceStatisticsResult({
    required this.paidInvoicesCount,
    required this.unpaidInvoicesCount,
    required this.partiallyPaidInvoicesCount,
    required this.totalInvoicesAmount,
    required this.totalPaidAmount,
    required this.totalRemainingAmount,
  });

  @override
  List<Object?> get props {
    return [
      paidInvoicesCount,
      unpaidInvoicesCount,
      partiallyPaidInvoicesCount,
      totalInvoicesAmount,
      totalPaidAmount,
      totalRemainingAmount,
    ];
  }
}
