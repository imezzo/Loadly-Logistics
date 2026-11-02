// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:json_annotation/json_annotation.dart';

part 'bulk_collect_trip_documents_request_dto.g.dart';

@JsonSerializable()
class BulkCollectTripDocumentsRequestDto {
  final List<int> tripIds;
  const BulkCollectTripDocumentsRequestDto({required this.tripIds});

  factory BulkCollectTripDocumentsRequestDto.fromJson(
    Map<String, dynamic> json,
  ) => _$BulkCollectTripDocumentsRequestDtoFromJson(json);
  Map<String, dynamic> toJson() =>
      _$BulkCollectTripDocumentsRequestDtoToJson(this);
}
