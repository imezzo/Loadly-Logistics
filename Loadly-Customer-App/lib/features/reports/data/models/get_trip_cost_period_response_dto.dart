// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:json_annotation/json_annotation.dart';

part 'get_trip_cost_period_response_dto.g.dart';

@JsonSerializable()
class GetTripCostPeriodResponseDto {
  final double totalCostLastSevenDays;
  final double totalCostLastThirtyDays;
  final double totalCostLastThreeMonths;

  const GetTripCostPeriodResponseDto({
    required this.totalCostLastSevenDays,
    required this.totalCostLastThirtyDays,
    required this.totalCostLastThreeMonths,
  });

  factory GetTripCostPeriodResponseDto.fromJson(Map<String, dynamic> json) =>
      _$GetTripCostPeriodResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$GetTripCostPeriodResponseDtoToJson(this);
}
