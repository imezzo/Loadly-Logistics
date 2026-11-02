import 'package:equatable/equatable.dart';

abstract class TripsFailure extends Equatable {
  const TripsFailure();

  @override
  List<Object?> get props => [];
}

@pragma('vm:keep')
class TripsFailureNetwork extends TripsFailure {
  const TripsFailureNetwork({this.message});

  final String? message;

  @override
  List<Object?> get props => [message];
}

@pragma('vm:keep')
class TripsFailureServer extends TripsFailure {
  const TripsFailureServer({this.message, this.statusCode});

  final String? message;
  final int? statusCode;

  @override
  List<Object?> get props => [message, statusCode];
}

@pragma('vm:keep')
class TripsFailureUnknown extends TripsFailure {
  const TripsFailureUnknown({this.message});

  final String? message;

  @override
  List<Object?> get props => [message];
}
