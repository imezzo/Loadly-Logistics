import '../../features/auth/domain/failures/auth_failure.dart';
import '../../features/home/domain/failures/home_failure.dart';
import '../../features/trips/domain/failures/trips_failure.dart';
import '../locale/user_facing_message.dart';
import 'user_message_codes.dart';

class FailureMessageMapper {
  static String forAuth(
    AuthFailure failure, {
    String invalidCredentialsMessage = UserMessageCodes.invalidLogin,
    String networkMessage = UserMessageCodes.network,
    String unknownMessage = UserMessageCodes.unexpected,
    String serverFallback = UserMessageCodes.unexpected,
  }) {
    return switch (failure) {
      AuthFailureInvalidCredentials() => invalidCredentialsMessage,
      AuthFailureNetwork() => networkMessage,
      AuthFailureServer(message: final message, statusCode: final statusCode) =>
        resolveUserMessageCode(
          message,
          statusCode: statusCode,
          fallback: serverFallback,
        ),
      AuthFailureUnknown(message: final message) => resolveUserMessageCode(
        message,
        fallback: unknownMessage,
      ),
    };
  }

  static String forHome(
    HomeFailure failure, {
    String serverFallback = UserMessageCodes.requestFailed,
    String networkFallback = UserMessageCodes.network,
    String unknownFallback = UserMessageCodes.unexpected,
  }) {
    return switch (failure) {
      HomeFailureServer(message: final message, statusCode: final statusCode) =>
        resolveUserMessageCode(
          message,
          statusCode: statusCode,
          fallback: serverFallback,
        ),
      HomeFailureNetwork() => networkFallback,
      HomeFailureUnknown(message: final message) => resolveUserMessageCode(
        message,
        fallback: unknownFallback,
      ),
      _ => unknownFallback,
    };
  }

  static String forTrips(
    TripsFailure failure, {
    String serverFallback = UserMessageCodes.requestFailed,
    String networkFallback = UserMessageCodes.network,
    String unknownFallback = UserMessageCodes.unexpected,
  }) {
    return switch (failure) {
      TripsFailureServer(message: final message, statusCode: final statusCode) =>
        resolveUserMessageCode(
          message,
          statusCode: statusCode,
          fallback: serverFallback,
        ),
      TripsFailureNetwork() => networkFallback,
      TripsFailureUnknown(message: final message) => resolveUserMessageCode(
        message,
        fallback: unknownFallback,
      ),
      _ => unknownFallback,
    };
  }

  static String defaultForStatusCode(int? code) => codeForStatus(code);
}
