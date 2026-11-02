import 'package:json_annotation/json_annotation.dart';

part 'extract_place_response_dto.g.dart';

@JsonSerializable()
class ExtractPlaceResponseDto {
  final double? lat;
  final double? lng;
  final String? address;
  final String? placeId;

  ExtractPlaceResponseDto({this.lat, this.lng, this.address, this.placeId});
  factory ExtractPlaceResponseDto.fromJson(Map<String, dynamic> json) =>
      _$ExtractPlaceResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ExtractPlaceResponseDtoToJson(this);
}
