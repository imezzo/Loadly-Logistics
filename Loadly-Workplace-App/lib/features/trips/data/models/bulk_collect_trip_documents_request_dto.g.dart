// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bulk_collect_trip_documents_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BulkCollectTripDocumentsRequestDto _$BulkCollectTripDocumentsRequestDtoFromJson(
  Map<String, dynamic> json,
) => BulkCollectTripDocumentsRequestDto(
  tripIds: (json['tripIds'] as List<dynamic>)
      .map((e) => (e as num).toInt())
      .toList(),
);

Map<String, dynamic> _$BulkCollectTripDocumentsRequestDtoToJson(
  BulkCollectTripDocumentsRequestDto instance,
) => <String, dynamic>{'tripIds': instance.tripIds};
