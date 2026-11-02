// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'get_trip_cost_period_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

GetTripCostPeriodResponseDto _$GetTripCostPeriodResponseDtoFromJson(
  Map<String, dynamic> json,
) => GetTripCostPeriodResponseDto(
  totalCostLastSevenDays: (json['totalCostLastSevenDays'] as num).toDouble(),
  totalCostLastThirtyDays: (json['totalCostLastThirtyDays'] as num).toDouble(),
  totalCostLastThreeMonths: (json['totalCostLastThreeMonths'] as num)
      .toDouble(),
);

Map<String, dynamic> _$GetTripCostPeriodResponseDtoToJson(
  GetTripCostPeriodResponseDto instance,
) => <String, dynamic>{
  'totalCostLastSevenDays': instance.totalCostLastSevenDays,
  'totalCostLastThirtyDays': instance.totalCostLastThirtyDays,
  'totalCostLastThreeMonths': instance.totalCostLastThreeMonths,
};
