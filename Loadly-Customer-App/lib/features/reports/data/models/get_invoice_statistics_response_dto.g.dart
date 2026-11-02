// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'get_invoice_statistics_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

GetInvoiceStatisticsResponseDto _$GetInvoiceStatisticsResponseDtoFromJson(
  Map<String, dynamic> json,
) => GetInvoiceStatisticsResponseDto(
  paidInvoicesCount: (json['paidInvoicesCount'] as num).toInt(),
  unpaidInvoicesCount: (json['unpaidInvoicesCount'] as num).toInt(),
  partiallyPaidInvoicesCount: (json['partiallyPaidInvoicesCount'] as num)
      .toInt(),
  totalInvoicesAmount: (json['totalInvoicesAmount'] as num).toDouble(),
  totalPaidAmount: (json['totalPaidAmount'] as num).toDouble(),
  totalRemainingAmount: (json['totalRemainingAmount'] as num).toDouble(),
);

Map<String, dynamic> _$GetInvoiceStatisticsResponseDtoToJson(
  GetInvoiceStatisticsResponseDto instance,
) => <String, dynamic>{
  'paidInvoicesCount': instance.paidInvoicesCount,
  'unpaidInvoicesCount': instance.unpaidInvoicesCount,
  'partiallyPaidInvoicesCount': instance.partiallyPaidInvoicesCount,
  'totalInvoicesAmount': instance.totalInvoicesAmount,
  'totalPaidAmount': instance.totalPaidAmount,
  'totalRemainingAmount': instance.totalRemainingAmount,
};
