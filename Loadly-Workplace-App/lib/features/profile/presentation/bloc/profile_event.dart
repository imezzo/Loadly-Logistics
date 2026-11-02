import 'package:equatable/equatable.dart';

abstract class ProfileEvent extends Equatable {
  const ProfileEvent();

  @override
  List<Object?> get props => [];
}

class ProfileLoadRequested extends ProfileEvent {
  const ProfileLoadRequested();
}

class ProfileUpdateRequested extends ProfileEvent {
  const ProfileUpdateRequested({
    this.fullName,
    this.fullNameAr,
    this.userName,
    this.email,
    this.phone,
    this.profilePicturePath,
    this.biometricFingerprint,
  });

  final String? fullName;
  final String? fullNameAr;
  final String? userName;
  final String? email;
  final String? phone;
  final String? profilePicturePath;
  final bool? biometricFingerprint;

  @override
  List<Object?> get props => [
    fullName,
    fullNameAr,
    userName,
    email,
    phone,
    profilePicturePath,
    biometricFingerprint,
  ];
}

class ProfileDocumentSaveRequested extends ProfileEvent {
  const ProfileDocumentSaveRequested({
    required this.documentRecordId,
    required this.documentTypeId,
    required this.filePath,
    this.expiryDate,
  });

  final int? documentRecordId;
  final int documentTypeId;
  final String filePath;
  final DateTime? expiryDate;

  @override
  List<Object?> get props => [
    documentRecordId,
    documentTypeId,
    filePath,
    expiryDate,
  ];
}
