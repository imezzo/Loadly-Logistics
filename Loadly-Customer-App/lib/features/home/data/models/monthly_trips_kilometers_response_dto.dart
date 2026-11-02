import 'package:json_annotation/json_annotation.dart';

part 'monthly_trips_kilometers_response_dto.g.dart';

@JsonSerializable()
class MonthlyTripsKilometersResponseDto {
  final int month;
  final String monthName;
  final int totalTrips;
  final double totalKilometers;

  const MonthlyTripsKilometersResponseDto({
    required this.month,
    required this.monthName,
    required this.totalTrips,
    required this.totalKilometers,
  });

  factory MonthlyTripsKilometersResponseDto.fromJson(
    Map<String, dynamic> json,
  ) => _$MonthlyTripsKilometersResponseDtoFromJson(json);
  Map<String, dynamic> toJson() =>
      _$MonthlyTripsKilometersResponseDtoToJson(this);
}
