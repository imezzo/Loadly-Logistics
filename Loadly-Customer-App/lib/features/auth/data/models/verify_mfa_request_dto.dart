import 'package:json_annotation/json_annotation.dart';

part 'verify_mfa_request_dto.g.dart';

@JsonSerializable()
class VerifyMfaRequestDto {
  const VerifyMfaRequestDto({required this.code});

  final String code;

  factory VerifyMfaRequestDto.fromJson(Map<String, dynamic> json) =>
      _$VerifyMfaRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$VerifyMfaRequestDtoToJson(this);
}
