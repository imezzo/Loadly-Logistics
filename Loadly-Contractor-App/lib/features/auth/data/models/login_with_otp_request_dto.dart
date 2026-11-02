import 'package:json_annotation/json_annotation.dart';

part 'login_with_otp_request_dto.g.dart';

@JsonSerializable()
class LoginWithOtpRequestDto {
  const LoginWithOtpRequestDto({
    required this.token,
    required this.login,
    required this.code,
  });

  final String token;
  final String login;
  final String code;

  factory LoginWithOtpRequestDto.fromJson(Map<String, dynamic> json) =>
      _$LoginWithOtpRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$LoginWithOtpRequestDtoToJson(this);
}
