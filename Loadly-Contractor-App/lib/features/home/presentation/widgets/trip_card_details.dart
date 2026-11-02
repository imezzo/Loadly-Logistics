import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../trips/domain/entities/booking_request/booking_request.dart';
import '../../../trips/domain/entities/trip/trip.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/presentation/bloc/trip/trip_bloc.dart';
import '../../../trips/presentation/bloc/trip/trip_state.dart';

class TripCardDetails {
  const TripCardDetails({
    required this.trip,
    required this.tripId,
    required this.companyName,
    required this.dateStr,
    required this.from,
    required this.to,
    required this.vehicleName,
    required this.priceText,
    required this.statusCode,
    required this.statusName,
  });

  final Trip? trip;
  final int? tripId;
  final String companyName;
  final String dateStr;
  final String from;
  final String to;
  final String vehicleName;
  final String priceText;
  final int? statusCode;
  final String? statusName;
}

TripCardDetails resolveTripCardDetails(
  BuildContext context, {
  BookingRequest? booking,
  Trip? trip,
}) {
  final tripEntity = _tripWithLocations(
    context,
    trip ?? booking?.trip,
    booking?.tripId ?? trip?.id ?? booking?.trip?.id,
  );
  final isRtl = Directionality.of(context) == TextDirection.rtl;
  final isArabic = Localizations.localeOf(context).languageCode == 'ar';
  final l10n = AppLocalizations.of(context)!;
  final cost = booking?.contractorPrice ?? tripEntity?.contractorPrice;

  return TripCardDetails(
    trip: tripEntity,
    tripId: booking?.tripId ?? tripEntity?.id,
    companyName: booking != null
        ? booking.companyDisplayNameOrDash(preferArabic: isRtl)
        : tripEntity?.companyDisplayNameOrDash(preferArabic: isRtl) ?? '—',
    dateStr: _dateText(
      l10n: l10n,
      startDate: tripEntity?.startDate,
      fallback: booking?.createdAt ?? tripEntity?.createdAt,
      isArabic: isArabic,
    ),
    from: _firstLocation([
      _tripFrom(tripEntity),
      booking?.fromLocation,
      _tripFrom(booking?.trip),
      _tripFrom(trip),
    ]),
    to: _firstLocation([
      _tripTo(tripEntity),
      booking?.toLocation,
      _tripTo(booking?.trip),
      _tripTo(trip),
    ]),
    vehicleName: tripEntity?.vehicleTypeDisplayName(preferArabic: isRtl) ?? '—',
    priceText: cost == null
        ? '—'
        : NumberFormat('#,###').format(cost.round()).replaceAll(',', '.'),
    statusCode: tripEntity?.status ?? booking?.status,
    statusName: tripEntity?.tripStatusName ?? booking?.statusName,
  );
}

Trip? _tripWithLocations(BuildContext context, Trip? trip, int? tripId) {
  if (_hasWaypointNames(trip)) return trip;
  if (tripId == null) return trip;
  try {
    final bloc = context.read<TripBloc>();
    final cached = bloc.cachedTrips[tripId];
    if (_hasWaypointNames(cached)) return cached;
    final state = bloc.state;
    if (state is TripLoaded) {
      for (final item in state.trips) {
        if (item.id == tripId && _hasWaypointNames(item)) return item;
      }
    }
  } catch (_) {}
  return trip;
}

bool _hasWaypointNames(Trip? trip) {
  final waypoints = trip?.waypoints;
  if (waypoints == null || waypoints.isEmpty) return false;
  return _waypointName(waypoints.first) != null ||
      _waypointName(waypoints.last) != null;
}

String formatTripCardDateTime(
  DateTime date, {
  required AppLocalizations l10n,
  required bool isArabic,
}) {
  final local = date.toLocal();
  final time = DateFormat('hh:mm', 'en').format(local);
  final meridiem = DateFormat('a', 'en').format(local);
  final period = isArabic
      ? (meridiem == 'AM' ? l10n.meridiemAm : l10n.meridiemPm)
      : meridiem;
  final datePart = DateFormat('dd-MM-yyyy').format(local);
  return '$time $period • $datePart';
}

String _dateText({
  required AppLocalizations l10n,
  String? startDate,
  DateTime? fallback,
  required bool isArabic,
}) {
  final parsed = startDate == null ? fallback : DateTime.tryParse(startDate);
  final date = parsed ?? fallback;
  if (date == null) return l10n.statusUnknown;
  return formatTripCardDateTime(date, l10n: l10n, isArabic: isArabic);
}

String _firstLocation(List<String?> values) {
  for (final value in values) {
    final text = value?.trim();
    if (text != null && text.isNotEmpty && text != '—') return text;
  }
  return '—';
}

String _tripFrom(Trip? trip) {
  final waypoints = trip?.waypoints;
  if (waypoints != null && waypoints.isNotEmpty) {
    final name = _waypointName(waypoints.first);
    if (name != null) return name;
  }
  final start = _locationText(trip?.contractDestination?.start);
  if (start != '—') return start;
  return _locationText(trip?.contractDestination?.nameAr) == '—'
      ? _locationText(trip?.contractDestination?.nameEn)
      : _locationText(trip?.contractDestination?.nameAr);
}

String _tripTo(Trip? trip) {
  final waypoints = trip?.waypoints;
  if (waypoints != null && waypoints.isNotEmpty) {
    final name = _waypointName(waypoints.last);
    if (name != null) return name;
  }
  return _locationText(trip?.contractDestination?.end);
}

String? _waypointName(Waypoint waypoint) {
  final name = waypoint.addressName?.trim();
  if (name != null && name.isNotEmpty) return name;
  return _locationText(waypoint.taskDetails) == '—'
      ? null
      : _locationText(waypoint.taskDetails);
}

String _locationText(dynamic value) {
  if (value is String && value.trim().isNotEmpty) return value.trim();
  if (value is Map) {
    for (final key in [
      'addressName',
      'AddressName',
      'nameAr',
      'NameAr',
      'nameEn',
      'NameEn',
      'name',
      'Name',
      'address',
      'Address',
      'city',
      'City',
      'governorate',
      'Governorate',
    ]) {
      final text = _locationText(value[key]);
      if (text != '—') return text;
    }
  }
  return '—';
}
