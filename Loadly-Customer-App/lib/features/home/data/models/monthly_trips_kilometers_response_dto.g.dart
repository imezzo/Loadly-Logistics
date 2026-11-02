// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'monthly_trips_kilometers_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MonthlyTripsKilometersResponseDto _$MonthlyTripsKilometersResponseDtoFromJson(
  Map<String, dynamic> json,
) => MonthlyTripsKilometersResponseDto(
  month: (json['month'] as num).toInt(),
  monthName: json['monthName'] as String,
  totalTrips: (json['totalTrips'] as num).toInt(),
  totalKilometers: (json['totalKilometers'] as num).toDouble(),
);

Map<String, dynamic> _$MonthlyTripsKilometersResponseDtoToJson(
  MonthlyTripsKilometersResponseDto instance,
) => <String, dynamic>{
  'month': instance.month,
  'monthName': instance.monthName,
  'totalTrips': instance.totalTrips,
  'totalKilometers': instance.totalKilometers,
};
