import 'package:json_annotation/json_annotation.dart';

part 'login_request_dto.g.dart';

@JsonSerializable()
class LoginRequestDto {
  const LoginRequestDto({required this.login, this.password});

  /// Phone or identifier for login, e.g. +201000665931 or 01000665931
  final String login;

  /// Password for login (optional, used for password-based login)
  final String? password;

  factory LoginRequestDto.fromJson(Map<String, dynamic> json) =>
      _$LoginRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$LoginRequestDtoToJson(this);
}
