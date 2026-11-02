import 'package:dartz/dartz.dart';

import '../errors/failure.dart';
import '../errors/api_fallback_keys.dart';
import 'api_failure_mapper.dart';
import 'exception_to_failure.dart';
import 'safe_api_call.dart';

Failure mapApiDetailsToFailure(
  ApiFailureDetails details, {
  String fallbackMessage = ApiFallbackKeys.requestFailed,
}) {
  return switch (details.kind) {
    ApiFailureKind.network => const Failure('network_error'),
    ApiFailureKind.unauthorized =>
      Failure(details.message ?? 'unauthorized'),
    ApiFailureKind.server => Failure(
      _serverFailureMessage(details.message, fallbackMessage),
    ),
    ApiFailureKind.unknown => Failure(details.message ?? fallbackMessage),
  };
}

String _serverFailureMessage(String? message, String fallbackMessage) {
  if (message == null || message.trim().isEmpty) return fallbackMessage;
  if (!isUserFacingApiMessage(message)) return fallbackMessage;
  return message;
}

Future<Either<Failure, T>> runApiCall<T>(
  Future<T> Function() request, {
  String fallbackMessage = ApiFallbackKeys.requestFailed,
}) {
  return safeApiCall<T, Failure>(
    request: request,
    mapFailure: (details) =>
        mapApiDetailsToFailure(details, fallbackMessage: fallbackMessage),
    mapUnknownFailure: (error) =>
        mapExceptionToFailure(error, fallback: fallbackMessage),
  );
}
