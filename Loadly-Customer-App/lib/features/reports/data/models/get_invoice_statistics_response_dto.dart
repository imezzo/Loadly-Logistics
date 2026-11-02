import 'package:json_annotation/json_annotation.dart';

part 'get_invoice_statistics_response_dto.g.dart';

@JsonSerializable()
class GetInvoiceStatisticsResponseDto {
  final int paidInvoicesCount;
  final int unpaidInvoicesCount;
  final int partiallyPaidInvoicesCount;
  final double totalInvoicesAmount;
  final double totalPaidAmount;
  final double totalRemainingAmount;

  const GetInvoiceStatisticsResponseDto({
    required this.paidInvoicesCount,
    required this.unpaidInvoicesCount,
    required this.partiallyPaidInvoicesCount,
    required this.totalInvoicesAmount,
    required this.totalPaidAmount,
    required this.totalRemainingAmount,
  });

  factory GetInvoiceStatisticsResponseDto.fromJson(Map<String, dynamic> json) =>
      _$GetInvoiceStatisticsResponseDtoFromJson(json);
  Map<String, dynamic> toJson() =>
      _$GetInvoiceStatisticsResponseDtoToJson(this);
}
