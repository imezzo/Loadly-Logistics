import 'package:json_annotation/json_annotation.dart';

part 'send_mfa_code_request_dto.g.dart';

@JsonSerializable()
class SendMfaCodeRequestDto {
  const SendMfaCodeRequestDto({required this.channel});

  final int channel;

  factory SendMfaCodeRequestDto.fromJson(Map<String, dynamic> json) =>
      _$SendMfaCodeRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$SendMfaCodeRequestDtoToJson(this);
}
