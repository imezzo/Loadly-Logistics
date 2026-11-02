import 'package:json_annotation/json_annotation.dart';

part 'verify_email_request_dto.g.dart';

@JsonSerializable()
class VerifyEmailRequestDto {
  const VerifyEmailRequestDto({required this.code});

  final String code;

  factory VerifyEmailRequestDto.fromJson(Map<String, dynamic> json) =>
      _$VerifyEmailRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$VerifyEmailRequestDtoToJson(this);
}
