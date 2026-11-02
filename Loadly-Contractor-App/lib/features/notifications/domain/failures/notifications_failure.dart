import 'package:equatable/equatable.dart';

abstract class NotificationsFailure extends Equatable {
  const NotificationsFailure();

  @override
  List<Object?> get props => [];
}

class NotificationsFailureNetwork extends NotificationsFailure {
  const NotificationsFailureNetwork({this.message});

  final String? message;

  @override
  List<Object?> get props => [message];
}

class NotificationsFailureServer extends NotificationsFailure {
  const NotificationsFailureServer({this.message, this.statusCode});

  final String? message;
  final int? statusCode;

  @override
  List<Object?> get props => [message, statusCode];
}
