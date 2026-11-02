import 'package:json_annotation/json_annotation.dart';

part 'login_response_dto.g.dart';

/// Parsed from API envelope: { data: LoginResponseDataDto, message, success, errors }
@JsonSerializable()
class LoginResponseDto {
  const LoginResponseDto({
    this.accessToken,
    this.refreshToken,
    this.expiresIn,
    this.user,
    this.redirect,
    this.isOnline,
  });

  @JsonKey(name: 'accessToken')
  final String? accessToken;

  @JsonKey(name: 'refreshToken')
  final String? refreshToken;

  @JsonKey(name: 'expiresIn')
  final int? expiresIn;

  final LoginUserDto? user;

  final String? redirect;

  @JsonKey(name: 'isOnline')
  final bool? isOnline;

  factory LoginResponseDto.fromJson(Map<String, dynamic> json) =>
      _$LoginResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$LoginResponseDtoToJson(this);
}

@JsonSerializable()
class LoginUserDto {
  const LoginUserDto({
    required this.id,
    this.fullName,
    this.userName,
    this.email,
    this.userType,
    this.mfaChannel,
    this.permissions,
  });

  final int id;
  @JsonKey(name: 'fullName')
  final String? fullName;
  @JsonKey(name: 'userName')
  final String? userName;
  final String? email;
  @JsonKey(name: 'userType')
  final String? userType;
  @JsonKey(name: 'mfaChannel')
  final int? mfaChannel;
  final Map<String, dynamic>? permissions;

  factory LoginUserDto.fromJson(Map<String, dynamic> json) =>
      _$LoginUserDtoFromJson(json);
  Map<String, dynamic> toJson() => _$LoginUserDtoToJson(this);
}

/// Used by getProfile API (may have different structure than LoginUserDto)
@JsonSerializable()
class ContractorProfileDto {
  const ContractorProfileDto({
    required this.id,
    required this.email,
    this.fullName,
    this.fullNameAr,
    this.nationalId,
    this.userName,
    this.phone,
    this.userType,
    this.mfaChannel,
    this.emailVerified,
    this.phoneVerified,
    this.status,
    this.profilePicture,
    this.driverRating,
    this.tripsCount,
    this.biometricFingerprint,
    this.permissions,
    this.vehicles,
    this.documents,
    this.createdAt,
    this.updatedAt,
    this.isOnline,
  });

  final int id;
  @JsonKey(name: 'fullName')
  final String? fullName;
  @JsonKey(name: 'fullNameAr')
  final String? fullNameAr;
  @JsonKey(name: 'nationalId')
  final String? nationalId;
  @JsonKey(name: 'userName')
  final String? userName;
  final String email;
  final String? phone;
  @JsonKey(name: 'userType')
  final String? userType;
  @JsonKey(name: 'mfaChannel')
  final int? mfaChannel;
  @JsonKey(name: 'emailVerified')
  final bool? emailVerified;
  @JsonKey(name: 'phoneVerified')
  final bool? phoneVerified;
  final int? status;
  @JsonKey(name: 'profilePicture')
  final String? profilePicture;
  @JsonKey(name: 'driverRating')
  final double? driverRating;
  @JsonKey(name: 'tripsCount')
  final int? tripsCount;
  @JsonKey(name: 'biometricFingerprint')
  final bool? biometricFingerprint;
  final Map<String, dynamic>? permissions;
  final List<dynamic>? vehicles;
  final List<ContractorProfileDocumentDto>? documents;
  @JsonKey(name: 'createdAt')
  final String? createdAt;
  @JsonKey(name: 'updatedAt')
  final String? updatedAt;
  @JsonKey(name: 'isOnline')
  final bool? isOnline;

  factory ContractorProfileDto.fromJson(Map<String, dynamic> json) =>
      _$ContractorProfileDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ContractorProfileDtoToJson(this);
}

@JsonSerializable()
class ContractorProfileDocumentDto {
  const ContractorProfileDocumentDto({
    required this.id,
    this.documentId,
    this.fileUrl,
    this.expiryDate,
    this.status,
    this.document,
  });

  final int id;
  @JsonKey(name: 'documentId')
  final int? documentId;
  @JsonKey(name: 'fileUrl')
  final String? fileUrl;
  @JsonKey(name: 'expiryDate')
  final String? expiryDate;
  final int? status;
  final ContractorDocumentDto? document;

  factory ContractorProfileDocumentDto.fromJson(Map<String, dynamic> json) =>
      _$ContractorProfileDocumentDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ContractorProfileDocumentDtoToJson(this);
}

@JsonSerializable()
class ContractorDocumentDto {
  const ContractorDocumentDto({required this.id, this.nameEn, this.nameAr});

  final int id;
  @JsonKey(name: 'nameEn')
  final String? nameEn;
  @JsonKey(name: 'nameAr')
  final String? nameAr;

  factory ContractorDocumentDto.fromJson(Map<String, dynamic> json) =>
      _$ContractorDocumentDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ContractorDocumentDtoToJson(this);
}
