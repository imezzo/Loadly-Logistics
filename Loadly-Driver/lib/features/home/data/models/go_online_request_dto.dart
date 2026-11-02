import 'package:json_annotation/json_annotation.dart';

part 'go_online_request_dto.g.dart';

@JsonSerializable()
class GoOnlineRequestDto {
  const GoOnlineRequestDto({required this.latitude, required this.longitude});

  final double latitude;
  final double longitude;

  factory GoOnlineRequestDto.fromJson(Map<String, dynamic> json) =>
      _$GoOnlineRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$GoOnlineRequestDtoToJson(this);
}
