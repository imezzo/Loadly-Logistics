import 'package:equatable/equatable.dart';
import '../../domain/entities/collector_trips.dart';
import '../../domain/entities/document.dart';

/// States for Home BLoC
abstract class HomeState extends Equatable {
  const HomeState();

  @override
  List<Object?> get props => [];
}

/// Initial state
class HomeInitial extends HomeState {
  const HomeInitial();
}

class TripsLoading extends HomeState {
  const TripsLoading();
}

class TripsLoaded extends HomeState {
  const TripsLoaded({required this.tripsList, this.error});

  final CollectorTripsList tripsList;
  final String? error;

  @override
  List<Object?> get props => [tripsList, error];
}

class DocumentsLoading extends HomeState {
  const DocumentsLoading();
}

class DocumentsLoaded extends HomeState {
  const DocumentsLoaded({required this.documentsList, this.error});

  final List<Document> documentsList;
  final String? error;

  @override
  List<Object?> get props => [documentsList, error];
}
