import 'package:json_annotation/json_annotation.dart';

part 'enable_mfa_request_dto.g.dart';

@JsonSerializable()
class EnableMfaRequestDto {
  const EnableMfaRequestDto({required this.channel});

  final int channel;

  factory EnableMfaRequestDto.fromJson(Map<String, dynamic> json) =>
      _$EnableMfaRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$EnableMfaRequestDtoToJson(this);
}
