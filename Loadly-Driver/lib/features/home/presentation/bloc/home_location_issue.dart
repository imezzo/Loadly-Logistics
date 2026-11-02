import 'package:equatable/equatable.dart';

/// Geolocation / permission failure on the home screen (localized in the UI).
enum HomeLocationIssueKind {
  servicesDisabled,
  permissionDenied,
  permissionDeniedForever,
  getPositionFailed,
  getInitialPositionFailed,
  streamError,
  unavailable,
}

class HomeLocationIssue extends Equatable {
  const HomeLocationIssue({required this.kind, this.detail});

  final HomeLocationIssueKind kind;
  final String? detail;

  @override
  List<Object?> get props => [kind, detail];
}
