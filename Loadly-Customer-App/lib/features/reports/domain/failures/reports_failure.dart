import 'package:equatable/equatable.dart';

abstract class ReportsFailure extends Equatable {
  const ReportsFailure();

  @override
  List<Object?> get props => [];
}

class ReportsFailureNetwork extends ReportsFailure {
  const ReportsFailureNetwork({this.message});

  final String? message;

  @override
  List<Object?> get props => [message];
}

class ReportsFailureServer extends ReportsFailure {
  const ReportsFailureServer({this.message, this.statusCode});

  final String? message;
  final int? statusCode;

  @override
  List<Object?> get props => [message, statusCode];
}

class ReportsFailureUnknown extends ReportsFailure {
  const ReportsFailureUnknown({this.message});

  final String? message;

  @override
  List<Object?> get props => [message];
}
