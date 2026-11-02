import 'package:equatable/equatable.dart';

class ContractorProfile extends Equatable {
  const ContractorProfile({
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
    this.isOnline,
    this.documents = const <ContractorProfileDocument>[],
  });

  final int id;
  final String email;
  final String? fullName;
  final String? fullNameAr;
  final String? nationalId;
  final String? userName;
  final String? phone;
  final String? userType;
  final int? mfaChannel;
  final bool? emailVerified;
  final bool? phoneVerified;
  final int? status;
  final String? profilePicture;
  final double? driverRating;
  final int? tripsCount;
  final bool? biometricFingerprint;
  final bool? isOnline;
  final List<ContractorProfileDocument> documents;

  @override
  List<Object?> get props => [
    id,
    email,
    fullName,
    fullNameAr,
    nationalId,
    userName,
    phone,
    userType,
    mfaChannel,
    emailVerified,
    phoneVerified,
    status,
    profilePicture,
    driverRating,
    tripsCount,
    biometricFingerprint,
    isOnline,
    documents,
  ];
}

class ContractorProfileDocument extends Equatable {
  const ContractorProfileDocument({
    required this.id,
    this.documentId,
    this.fileUrl,
    this.expiryDate,
    this.status,
    this.document,
  });

  final int id;
  final int? documentId;
  final String? fileUrl;
  final String? expiryDate;
  final int? status;
  final ContractorProfileDocumentType? document;

  @override
  List<Object?> get props => [
    id,
    documentId,
    fileUrl,
    expiryDate,
    status,
    document,
  ];
}

class ContractorProfileDocumentType extends Equatable {
  const ContractorProfileDocumentType({
    required this.id,
    this.nameEn,
    this.nameAr,
  });

  final int id;
  final String? nameEn;
  final String? nameAr;

  @override
  List<Object?> get props => [id, nameEn, nameAr];
}
