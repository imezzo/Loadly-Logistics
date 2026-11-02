// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

import '../../data/models/collect_trip_documents_request_dto.dart';

abstract class TripsEvent extends Equatable {
  const TripsEvent();

  @override
  List<Object?> get props => [];
}

class TripsCollectDocuments extends TripsEvent {
  const TripsCollectDocuments({
    required this.tripId,
    required this.collectorDocuments,
    required this.submit,
  });

  final int tripId;
  final List<CollectTripDocumentsRequestDto> collectorDocuments;
  final bool submit;

  @override
  List<Object?> get props => [tripId, collectorDocuments, submit];
}

class TripsConfirmDocuments extends TripsEvent {
  const TripsConfirmDocuments({
    required this.tripId,
    required this.collectorDocuments,
    required this.submit,
  });

  final int tripId;
  final List<CollectTripDocumentsRequestDto> collectorDocuments;
  final bool submit;

  @override
  List<Object?> get props => [tripId, collectorDocuments, submit];
}

class TripsBulkCollectDocuments extends TripsEvent {
  const TripsBulkCollectDocuments({required this.tripIds});

  final List<int> tripIds;

  @override
  List<Object?> get props => [tripIds];
}

class TripsBulkConfirmDocuments extends TripsEvent {
  const TripsBulkConfirmDocuments({required this.tripIds});

  final List<int> tripIds;

  @override
  List<Object?> get props => [tripIds];
}
