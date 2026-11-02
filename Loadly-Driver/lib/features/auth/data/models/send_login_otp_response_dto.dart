import 'package:json_annotation/json_annotation.dart';

part 'send_login_otp_response_dto.g.dart';

@JsonSerializable()
class SendLoginOtpResponseDto {
  const SendLoginOtpResponseDto({required this.token});

  final String token;

  factory SendLoginOtpResponseDto.fromJson(Map<String, dynamic> json) =>
      _$SendLoginOtpResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$SendLoginOtpResponseDtoToJson(this);
}
