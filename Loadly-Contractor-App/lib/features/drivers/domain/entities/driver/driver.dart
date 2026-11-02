import 'package:equatable/equatable.dart';

class Driver extends Equatable {
  const Driver({
    this.id,
    this.fullNameEn,
    this.fullNameAr,
    this.phone,
    this.nationalId,
    this.email,
    this.userName,
    this.status,
    this.workingStatus,
    this.documents = const [],
  });

  final int? id;
  final String? fullNameEn;
  final String? fullNameAr;
  final String? phone;
  final String? nationalId;
  final String? email;
  final String? userName;
  final int? status;
  final int? workingStatus;
  final List<DriverDocument> documents;

  bool get isNew => id == null || id == 0;

  @override
  List<Object?> get props => [
    id,
    fullNameEn,
    fullNameAr,
    phone,
    nationalId,
    email,
    userName,
    status,
    workingStatus,
    documents,
  ];
}

class DriverDocument extends Equatable {
  const DriverDocument({
    required this.documentId,
    required this.filePath,
    required this.expiryDate,
  });

  final int documentId;
  final String filePath;
  final DateTime expiryDate;

  @override
  List<Object?> get props => [documentId, filePath, expiryDate];
}
