// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'dart:io';

import 'package:equatable/equatable.dart';

abstract class TripsEvent extends Equatable {
  const TripsEvent();

  @override
  List<Object?> get props => [];
}

class TripsLoadTrips extends TripsEvent {
  const TripsLoadTrips({
    this.page,
    this.limit,
    this.status,
    this.startDateFrom,
    this.startDateTo,
  });

  final int? page;
  final int? limit;
  final int? status;
  final String? startDateFrom;
  final String? startDateTo;

  @override
  List<Object?> get props => [page, limit, status, startDateFrom, startDateTo];
}

class TripsLoadTripDetails extends TripsEvent {
  const TripsLoadTripDetails({required this.tripId});

  final int tripId;

  @override
  List<Object?> get props => [tripId];
}

class TripsRefresh extends TripsEvent {
  const TripsRefresh();
}

class TripsLoadMore extends TripsEvent {
  const TripsLoadMore();
}

class TripsStartLoading extends TripsEvent {
  const TripsStartLoading({
    required this.tripId,
    required this.waypointId,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final int waypointId;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [tripId, waypointId, latitude, longitude];
}

class TripsCompleteLoading extends TripsEvent {
  const TripsCompleteLoading({
    required this.tripId,
    required this.waypointId,
    required this.receiptNoteImages,
    required this.shipmentImages,
    this.note,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final int waypointId;
  final List<File> receiptNoteImages;
  final List<File> shipmentImages;
  final String? note;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [
    tripId,
    waypointId,
    receiptNoteImages,
    shipmentImages,
    note,
    latitude,
    longitude,
  ];
}

class TripsStartUnloading extends TripsEvent {
  const TripsStartUnloading({
    required this.tripId,
    required this.waypointId,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final int waypointId;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [tripId, waypointId, latitude, longitude];
}

class TripsCompleteUnloading extends TripsEvent {
  const TripsCompleteUnloading({
    required this.tripId,
    required this.waypointId,
    this.odometer,
    this.odometerImages,
    required this.unloadingImages,
    required this.deliveryNoteImages,
    this.note,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final int waypointId;
  final int? odometer;
  final List<File>? odometerImages;
  final List<File> unloadingImages;
  final List<File> deliveryNoteImages;
  final String? note;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [
    tripId,
    waypointId,
    odometer,
    odometerImages,
    unloadingImages,
    deliveryNoteImages,
    note,
    latitude,
    longitude,
  ];
}

class TripsStartTrip extends TripsEvent {
  const TripsStartTrip({
    required this.tripId,
    this.note,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final String? note;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [tripId, note, latitude, longitude];
}

class TripsArriveAtWaypoint extends TripsEvent {
  const TripsArriveAtWaypoint({
    required this.tripId,
    required this.waypointId,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final int waypointId;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [tripId, waypointId, latitude, longitude];
}

class TripsDepartFromWaypoint extends TripsEvent {
  const TripsDepartFromWaypoint({
    required this.tripId,
    required this.waypointId,
    this.deliveryNoteImages,
    this.note,
    this.latitude,
    this.longitude,
    this.odometer,
    this.odometerImages,
  });

  final int tripId;
  final int waypointId;
  final List<File>? deliveryNoteImages;
  final String? note;
  final double? latitude;
  final double? longitude;

  /// Required when departing from first pickup (start-of-trip odometer).
  final int? odometer;
  final List<File>? odometerImages;

  @override
  List<Object?> get props => [
    tripId,
    waypointId,
    deliveryNoteImages,
    note,
    latitude,
    longitude,
    odometer,
    odometerImages,
  ];
}

class TripsEnterCustoms extends TripsEvent {
  const TripsEnterCustoms({
    required this.tripId,
    required this.customDocuments,
    this.note,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final List<File> customDocuments;
  final String? note;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [
    tripId,
    customDocuments,
    note,
    latitude,
    longitude,
  ];
}

class TripsExitCustoms extends TripsEvent {
  const TripsExitCustoms({
    required this.tripId,
    this.customsTotalAmount,
    required this.customsReceipts,
    this.note,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final double? customsTotalAmount;
  final List<File> customsReceipts;
  final String? note;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [
    tripId,
    customsTotalAmount,
    customsReceipts,
    note,
    latitude,
    longitude,
  ];
}

class TripsCompleteTrip extends TripsEvent {
  const TripsCompleteTrip({
    required this.tripId,
    required this.odometer,
    required this.odometerImages,
    required this.tollgateImages,
    required this.tollgateWeightImages,
    required this.note,
    this.latitude,
    this.longitude,
  });

  final int tripId;
  final int odometer;
  final List<File> odometerImages;
  final List<File> tollgateImages;
  final List<File> tollgateWeightImages;
  final String note;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [
    tripId,
    odometer,
    odometerImages,
    tollgateImages,
    tollgateWeightImages,
    note,
    latitude,
    longitude,
  ];
}

class TripsOpenDocument extends TripsEvent {
  const TripsOpenDocument({required this.tripId, required this.documentPath});

  final int tripId;
  final String documentPath;

  @override
  List<Object?> get props => [tripId, documentPath];
}

class TripsClearDocumentUrl extends TripsEvent {
  const TripsClearDocumentUrl();
}
