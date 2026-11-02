// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'login_with_otp_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

LoginWithOtpRequestDto _$LoginWithOtpRequestDtoFromJson(
  Map<String, dynamic> json,
) => LoginWithOtpRequestDto(
  token: json['token'] as String,
  login: json['login'] as String,
  code: json['code'] as String,
);

Map<String, dynamic> _$LoginWithOtpRequestDtoToJson(
  LoginWithOtpRequestDto instance,
) => <String, dynamic>{
  'token': instance.token,
  'login': instance.login,
  'code': instance.code,
};
