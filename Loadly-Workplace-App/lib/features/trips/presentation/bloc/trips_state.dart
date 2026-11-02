import 'package:equatable/equatable.dart';

abstract class TripsState extends Equatable {
  const TripsState();

  @override
  List<Object?> get props => [];
}

class TripsInitial extends TripsState {
  const TripsInitial();
}

class CollectDocumentsLoading extends TripsState {
  const CollectDocumentsLoading();
}

class CollectDocumentsDone extends TripsState {
  const CollectDocumentsDone({this.error});

  final String? error;

  @override
  List<Object?> get props => [error];
}

class ConfirmDocumentsLoading extends TripsState {
  const ConfirmDocumentsLoading();
}

class ConfirmDocumentsDone extends TripsState {
  const ConfirmDocumentsDone({this.error});

  final String? error;

  @override
  List<Object?> get props => [error];
}

class BulkCollectDocumentsLoading extends TripsState {
  const BulkCollectDocumentsLoading();
}

class BulkCollectDocumentsDone extends TripsState {
  const BulkCollectDocumentsDone({this.error});

  final String? error;

  @override
  List<Object?> get props => [error];
}

class BulkConfirmDocumentsLoading extends TripsState {
  const BulkConfirmDocumentsLoading();
}

class BulkConfirmDocumentsDone extends TripsState {
  const BulkConfirmDocumentsDone({this.error});

  final String? error;

  @override
  List<Object?> get props => [error];
}
