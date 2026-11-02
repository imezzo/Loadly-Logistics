import 'package:json_annotation/json_annotation.dart';

part 'resend_verification_request_dto.g.dart';

/// Channel: 0 = Email, 1 = SMS. Omit body for default (Email).
@JsonSerializable(includeIfNull: false)
class ResendVerificationRequestDto {
  const ResendVerificationRequestDto({this.channel});

  final int? channel;

  factory ResendVerificationRequestDto.fromJson(Map<String, dynamic> json) =>
      _$ResendVerificationRequestDtoFromJson(json);
  Map<String, dynamic> toJson() {
    final map = <String, dynamic>{};
    if (channel != null) map['channel'] = channel!;
    return map;
  }
}
