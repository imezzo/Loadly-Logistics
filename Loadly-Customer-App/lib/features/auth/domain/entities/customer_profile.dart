import 'package:equatable/equatable.dart';

class CustomerProfile extends Equatable {
  const CustomerProfile({
    required this.id,
    required this.email,
    this.fullName,
    this.userName,
    this.phone,
    this.userType,
    this.mfaChannel,
    this.emailVerified,
    this.phoneVerified,
    this.status,
    this.profilePicture,
    this.biometricFingerprint,
  });

  final int id;
  final String email;
  final String? fullName;
  final String? userName;
  final String? phone;
  final String? userType;
  final int? mfaChannel;
  final bool? emailVerified;
  final bool? phoneVerified;
  final int? status;
  final String? profilePicture;
  final bool? biometricFingerprint;

  @override
  List<Object?> get props => [
    id,
    email,
    fullName,
    userName,
    phone,
    userType,
    mfaChannel,
    emailVerified,
    phoneVerified,
    status,
    profilePicture,
    biometricFingerprint,
  ];
}
