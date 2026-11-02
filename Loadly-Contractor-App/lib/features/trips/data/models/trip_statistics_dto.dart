import '../../domain/entities/trip/trip_statistics.dart';

class InvitationStatisticsDto {
  const InvitationStatisticsDto({
    required this.openInvitationTrips,
    required this.pendingBookingRequestTrips,
  });

  final int openInvitationTrips;
  final int pendingBookingRequestTrips;

  factory InvitationStatisticsDto.fromJson(Map<String, dynamic> json) {
    return InvitationStatisticsDto(
      openInvitationTrips: _asInt(
        json['openInvitationTrips'] ?? json['OpenInvitationTrips'],
      ),
      pendingBookingRequestTrips: _asInt(
        json['pendingBookingRequestTrips'] ??
            json['PendingBookingRequestTrips'],
      ),
    );
  }

  InvitationStatistics toEntity() {
    return InvitationStatistics(
      openInvitationTrips: openInvitationTrips,
      pendingBookingRequestTrips: pendingBookingRequestTrips,
    );
  }
}

class TripStatusStatisticsDto {
  const TripStatusStatisticsDto({
    required this.completedTrips,
    required this.scheduledTrips,
    required this.inProgressTrips,
    required this.pendingTrips,
  });

  final int completedTrips;
  final int scheduledTrips;
  final int inProgressTrips;
  final int pendingTrips;

  factory TripStatusStatisticsDto.fromJson(Map<String, dynamic> json) {
    return TripStatusStatisticsDto(
      completedTrips: _asInt(json['completedTrips'] ?? json['CompletedTrips']),
      scheduledTrips: _asInt(json['scheduledTrips'] ?? json['ScheduledTrips']),
      inProgressTrips: _asInt(
        json['inProgressTrips'] ?? json['InProgressTrips'],
      ),
      pendingTrips: _asInt(json['pendingTrips'] ?? json['PendingTrips']),
    );
  }

  TripStatusStatistics toEntity() {
    return TripStatusStatistics(
      completedTrips: completedTrips,
      scheduledTrips: scheduledTrips,
      inProgressTrips: inProgressTrips,
      pendingTrips: pendingTrips,
    );
  }
}

class PayoutStatisticsDto {
  const PayoutStatisticsDto({
    required this.completedPaidTrips,
    required this.completedUnpaidTrips,
    required this.completedPaidTripsTotalAmount,
    required this.completedUnpaidTripsTotalAmount,
  });

  final int completedPaidTrips;
  final int completedUnpaidTrips;
  final double completedPaidTripsTotalAmount;
  final double completedUnpaidTripsTotalAmount;

  factory PayoutStatisticsDto.fromJson(Map<String, dynamic> json) {
    return PayoutStatisticsDto(
      completedPaidTrips: _asInt(
        json['completedPaidTrips'] ?? json['CompletedPaidTrips'],
      ),
      completedUnpaidTrips: _asInt(
        json['completedUnpaidTrips'] ?? json['CompletedUnpaidTrips'],
      ),
      completedPaidTripsTotalAmount: _asDouble(
        json['completedPaidTripsTotalAmount'] ??
            json['CompletedPaidTripsTotalAmount'],
      ),
      completedUnpaidTripsTotalAmount: _asDouble(
        json['completedUnpaidTripsTotalAmount'] ??
            json['CompletedUnpaidTripsTotalAmount'],
      ),
    );
  }

  PayoutStatistics toEntity() {
    return PayoutStatistics(
      completedPaidTrips: completedPaidTrips,
      completedUnpaidTrips: completedUnpaidTrips,
      completedPaidTripsTotalAmount: completedPaidTripsTotalAmount,
      completedUnpaidTripsTotalAmount: completedUnpaidTripsTotalAmount,
    );
  }
}

int _asInt(dynamic value) {
  if (value is num) return value.toInt();
  if (value is String) return int.tryParse(value.trim()) ?? 0;
  return 0;
}

double _asDouble(dynamic value) {
  if (value is num) return value.toDouble();
  if (value is String) return double.tryParse(value.trim()) ?? 0;
  return 0;
}
