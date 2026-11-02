import 'package:equatable/equatable.dart';

import '../trip/trip.dart';

class BookingRequest extends Equatable {
  static const int pendingStatus = 1;

  /// Trip status that allows editing the booking request.
  static const int editableTripStatus = 0;

  const BookingRequest({
    this.id,
    this.status,
    this.statusName,
    this.note,
    this.createdAt,
    this.reviewedAt,
    this.reviewedByAdminId,
    this.tripId,
    this.trip,
    this.driver,
    this.company,
    this.contactPersonName,
    this.fromLocation,
    this.toLocation,
    this.contractorPrice,
  });

  final int? id;
  final int? status;
  final String? statusName;
  final String? note;
  final DateTime? createdAt;
  final DateTime? reviewedAt;
  final int? reviewedByAdminId;
  final int? tripId;
  final Trip? trip;
  final Driver? driver;
  final Company? company;
  final String? contactPersonName;
  final String? fromLocation;
  final String? toLocation;
  final double? contractorPrice;

  bool get canEdit => trip?.status == editableTripStatus;

  String? companyDisplayName({required bool preferArabic}) {
    return resolveCompanyDisplayName(
      preferArabic: preferArabic,
      company: company ?? trip?.company,
      contactPersonName: contactPersonName ?? trip?.contactPersonName,
    );
  }

  String companyDisplayNameOrDash({required bool preferArabic}) {
    return companyDisplayName(preferArabic: preferArabic) ?? '—';
  }

  @override
  List<Object?> get props => [
    id,
    status,
    statusName,
    note,
    createdAt,
    reviewedAt,
    reviewedByAdminId,
    tripId,
    trip,
    driver,
    company,
    contactPersonName,
    fromLocation,
    toLocation,
    contractorPrice,
  ];
}
