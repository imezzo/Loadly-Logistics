// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'login_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

LoginResponseDto _$LoginResponseDtoFromJson(Map<String, dynamic> json) =>
    LoginResponseDto(
      accessToken: json['accessToken'] as String?,
      refreshToken: json['refreshToken'] as String?,
      expiresIn: (json['expiresIn'] as num?)?.toInt(),
      user: json['user'] == null
          ? null
          : LoginUserDto.fromJson(json['user'] as Map<String, dynamic>),
      redirect: json['redirect'] as String?,
      isOnline: json['isOnline'] as bool?,
    );

Map<String, dynamic> _$LoginResponseDtoToJson(LoginResponseDto instance) =>
    <String, dynamic>{
      'accessToken': instance.accessToken,
      'refreshToken': instance.refreshToken,
      'expiresIn': instance.expiresIn,
      'user': instance.user?.toJson(),
      'redirect': instance.redirect,
      'isOnline': instance.isOnline,
    };

LoginUserDto _$LoginUserDtoFromJson(Map<String, dynamic> json) => LoginUserDto(
  id: (json['id'] as num).toInt(),
  fullName: json['fullName'] as String?,
  userName: json['userName'] as String?,
  email: json['email'] as String?,
  userType: json['userType'] as String?,
  mfaChannel: (json['mfaChannel'] as num?)?.toInt(),
  permissions: json['permissions'] as Map<String, dynamic>?,
);

Map<String, dynamic> _$LoginUserDtoToJson(LoginUserDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'fullName': instance.fullName,
      'userName': instance.userName,
      'email': instance.email,
      'userType': instance.userType,
      'mfaChannel': instance.mfaChannel,
      'permissions': instance.permissions,
    };

CustomerProfileDto _$CustomerProfileDtoFromJson(Map<String, dynamic> json) =>
    CustomerProfileDto(
      id: (json['id'] as num).toInt(),
      email: json['email'] as String,
      fullName: json['fullName'] as String?,
      fullNameAr: json['fullNameAr'] as String?,
      nationalId: json['nationalId'] as String?,
      userName: json['userName'] as String?,
      phone: json['phone'] as String?,
      userType: json['userType'] as String?,
      mfaChannel: (json['mfaChannel'] as num?)?.toInt(),
      emailVerified: json['emailVerified'] as bool?,
      phoneVerified: json['phoneVerified'] as bool?,
      status: (json['status'] as num?)?.toInt(),
      profilePicture: json['profilePicture'] as String?,
      customerRating: (json['customerRating'] as num?)?.toDouble(),
      tripsCount: (json['tripsCount'] as num?)?.toInt(),
      biometricFingerprint: json['biometricFingerprint'] as bool?,
      permissions: json['permissions'] as Map<String, dynamic>?,
      vehicles: json['vehicles'] as List<dynamic>?,
      createdAt: json['createdAt'] as String?,
      updatedAt: json['updatedAt'] as String?,
      isOnline: json['isOnline'] as bool?,
    );

Map<String, dynamic> _$CustomerProfileDtoToJson(CustomerProfileDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'fullName': instance.fullName,
      'fullNameAr': instance.fullNameAr,
      'nationalId': instance.nationalId,
      'userName': instance.userName,
      'email': instance.email,
      'phone': instance.phone,
      'userType': instance.userType,
      'mfaChannel': instance.mfaChannel,
      'emailVerified': instance.emailVerified,
      'phoneVerified': instance.phoneVerified,
      'status': instance.status,
      'profilePicture': instance.profilePicture,
      'customerRating': instance.customerRating,
      'tripsCount': instance.tripsCount,
      'biometricFingerprint': instance.biometricFingerprint,
      'permissions': instance.permissions,
      'vehicles': instance.vehicles,
      'createdAt': instance.createdAt,
      'updatedAt': instance.updatedAt,
      'isOnline': instance.isOnline,
    };
