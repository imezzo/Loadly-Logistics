import 'package:equatable/equatable.dart';

sealed class TripEvent {}

class GetTrip extends TripEvent {
  GetTrip(this.tripId);

  final int tripId;
}

class GetTrips extends TripEvent {
  GetTrips({
    this.page = 1,
    this.pageSize = 10,
    this.status,
    this.statuses,
    this.openInviteOnly,
    this.loadMore = false,
    this.search,
    this.vehicleTypeId,
    this.startDateFrom,
    this.startDateTo,
    this.isContractorPaid,
    this.hasContractor,
  });

  final int page;
  final int pageSize;
  final int? status;
  final List<int>? statuses;
  final bool? openInviteOnly;
  final bool loadMore;
  final String? search;
  final int? vehicleTypeId;
  final String? startDateFrom;
  final String? startDateTo;
  final bool? isContractorPaid;
  final bool? hasContractor;
}

class BookTrip extends TripEvent {
  BookTrip(this.tripId);

  final int tripId;
}

class WithdrawTrip extends TripEvent {
  WithdrawTrip(this.tripId);

  final int tripId;
}

/// Identifies which trips list API request produced a [TripLoaded] state.
class TripsListQuery extends Equatable {
  const TripsListQuery({
    required this.page,
    required this.pageSize,
    this.status,
    this.statuses,
    this.openInviteOnly,
    this.search,
    this.vehicleTypeId,
    this.startDateFrom,
    this.startDateTo,
    this.isContractorPaid,
    this.hasContractor,
  });

  factory TripsListQuery.fromGetTrips(GetTrips event) {
    final trimmedSearch = event.search?.trim();
    final search = (trimmedSearch == null || trimmedSearch.isEmpty)
        ? null
        : trimmedSearch;

    return TripsListQuery(
      page: event.page,
      pageSize: event.pageSize,
      status: event.status,
      statuses: event.statuses == null || event.statuses!.isEmpty
          ? null
          : (List<int>.from(event.statuses!)..sort()),
      openInviteOnly: event.openInviteOnly,
      search: search,
      vehicleTypeId: event.vehicleTypeId,
      startDateFrom: _normalizeOptionalString(event.startDateFrom),
      startDateTo: _normalizeOptionalString(event.startDateTo),
      isContractorPaid: event.isContractorPaid,
      hasContractor: event.hasContractor,
    );
  }

  static String? _normalizeOptionalString(String? value) {
    final trimmed = value?.trim();
    if (trimmed == null || trimmed.isEmpty) return null;
    return trimmed;
  }

  final int page;
  final int pageSize;
  final int? status;
  final List<int>? statuses;
  final bool? openInviteOnly;
  final String? search;
  final int? vehicleTypeId;
  final String? startDateFrom;
  final String? startDateTo;
  final bool? isContractorPaid;
  final bool? hasContractor;

  @override
  List<Object?> get props => [
    page,
    pageSize,
    status,
    statuses,
    openInviteOnly,
    search,
    vehicleTypeId,
    startDateFrom,
    startDateTo,
    isContractorPaid,
    hasContractor,
  ];
}
