import 'trip_dto.dart';
import 'json_location.dart';

class BookingRequestDto {
  const BookingRequestDto({
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
  final TripDto? trip;
  final DriverDto? driver;
  final CompanyDto? company;
  final String? contactPersonName;
  final String? fromLocation;
  final String? toLocation;
  final double? contractorPrice;

  factory BookingRequestDto.fromJson(Map<String, dynamic> json) {
    final tripMap =
        jsonObject(json['trip']) ??
        jsonObject(json['Trip']) ??
        jsonObject(json['tripDetails']) ??
        jsonObject(json['TripDetails']);
    TripDto? trip = _tryTrip(tripMap);
    if (!_tripHasLocations(trip)) {
      trip = _tryTrip(json) ?? trip;
    }

    final fromLocation = locationFromTripJson(json, from: true);
    final toLocation = locationFromTripJson(json, from: false);
    final driverMap =
        jsonObject(json['driver']) ??
        jsonObject(json['Driver']) ??
        jsonObject(tripMap?['driver']) ??
        jsonObject(tripMap?['Driver']);
    DriverDto? driver;
    if (driverMap != null) {
      try {
        driver = DriverDto.fromJson(driverMap);
      } catch (_) {
        driver = null;
      }
    }

    return BookingRequestDto(
      id: (json['id'] as num?)?.toInt() ?? (json['Id'] as num?)?.toInt(),
      status:
          (json['status'] as num?)?.toInt() ??
          (json['Status'] as num?)?.toInt(),
      statusName:
          json['statusName'] as String? ?? json['StatusName'] as String?,
      note: json['note'] as String? ?? json['Note'] as String?,
      createdAt: _parseDate(json['createdAt'] ?? json['CreatedAt']),
      reviewedAt: _parseDate(json['reviewedAt'] ?? json['ReviewedAt']),
      reviewedByAdminId:
          (json['reviewedByAdminId'] as num?)?.toInt() ??
          (json['ReviewedByAdminId'] as num?)?.toInt(),
      tripId:
          (json['tripId'] as num?)?.toInt() ??
          (json['TripId'] as num?)?.toInt() ??
          trip?.id,
      trip: trip,
      driver: driver ?? trip?.driver,
      company: () {
        try {
          final map =
              jsonObject(json['company']) ??
              jsonObject(json['Company']) ??
              jsonObject(tripMap?['company']) ??
              jsonObject(tripMap?['Company']);
          if (map == null) return null;
          return CompanyDto.fromJson(map);
        } catch (_) {
          return null;
        }
      }(),
      contactPersonName: _parseContactPersonName(json, tripMap, trip),
      fromLocation: fromLocation,
      toLocation: toLocation,
      contractorPrice: _parseDouble(
        json['contractorPrice'] ?? json['ContractorPrice'],
      ),
    );
  }
}

TripDto? _tryTrip(Map<String, dynamic>? json) {
  if (json == null) return null;
  try {
    return TripDto.fromJson(json);
  } catch (_) {
    return null;
  }
}

bool _tripHasLocations(TripDto? trip) {
  final waypoints = trip?.waypoints;
  if (waypoints != null) {
    for (final point in waypoints) {
      final name = point.addressName?.trim();
      if (name != null && name.isNotEmpty) return true;
    }
  }
  final destination = trip?.contractDestination;
  return jsonText(destination?.start) != null ||
      jsonText(destination?.end) != null;
}

DateTime? _parseDate(dynamic value) {
  if (value is String && value.isNotEmpty) return DateTime.tryParse(value);
  return null;
}

double? _parseDouble(dynamic value) {
  if (value is num) return value.toDouble();
  return null;
}

String? _parseContactPersonName(
  Map<String, dynamic> json,
  Map<String, dynamic>? tripMap,
  TripDto? trip,
) {
  final fromTrip = trip?.contactPersonName;
  if (fromTrip != null && fromTrip.trim().isNotEmpty) return fromTrip.trim();
  for (final source in [json, tripMap]) {
    if (source == null) continue;
    final value =
        source['contactPersonName'] ?? source['ContactPersonName'];
    if (value is String && value.trim().isNotEmpty) return value.trim();
  }
  return null;
}
