// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'trips_statistics_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TripsStatisticsResponseDto _$TripsStatisticsResponseDtoFromJson(
  Map<String, dynamic> json,
) => TripsStatisticsResponseDto(
  totalTrips: (json['totalTrips'] as num).toInt(),
  scheduledTrips: (json['scheduledTrips'] as num).toInt(),
  inProgressTrips: (json['inProgressTrips'] as num).toInt(),
  canceledTrips: (json['canceledTrips'] as num).toInt(),
  completedTrips: (json['completedTrips'] as num).toInt(),
  otherTrips: (json['otherTrips'] as num).toInt(),
);

Map<String, dynamic> _$TripsStatisticsResponseDtoToJson(
  TripsStatisticsResponseDto instance,
) => <String, dynamic>{
  'totalTrips': instance.totalTrips,
  'scheduledTrips': instance.scheduledTrips,
  'inProgressTrips': instance.inProgressTrips,
  'canceledTrips': instance.canceledTrips,
  'completedTrips': instance.completedTrips,
  'otherTrips': instance.otherTrips,
};
