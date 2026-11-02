import 'package:json_annotation/json_annotation.dart';

part 'trips_statistics_response_dto.g.dart';

@JsonSerializable()
class TripsStatisticsResponseDto {
  final int totalTrips;
  final int scheduledTrips;
  final int inProgressTrips;
  final int canceledTrips;
  final int completedTrips;
  final int otherTrips;

  const TripsStatisticsResponseDto({
    required this.totalTrips,
    required this.scheduledTrips,
    required this.inProgressTrips,
    required this.canceledTrips,
    required this.completedTrips,
    required this.otherTrips,
  });

  factory TripsStatisticsResponseDto.fromJson(Map<String, dynamic> json) =>
      _$TripsStatisticsResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripsStatisticsResponseDtoToJson(this);
}
