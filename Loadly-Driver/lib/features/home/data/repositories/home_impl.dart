import 'package:dartz/dartz.dart';

import '../../../../core/network/api_failure_mapper.dart';
import '../../../../core/network/safe_api_call.dart';
import '../../domain/failures/home_failure.dart';
import '../../domain/repositories/home_repository.dart';
import '../models/go_online_request_dto.dart';
import '../sources/remote/home_remote_data_source.dart';

class HomeImpl implements HomeRepository {
  HomeImpl(this._remote);

  final HomeRemoteDataSource _remote;

  Future<Either<HomeFailure, T>> _run<T>(Future<T> Function() request) {
    return safeApiCall<T, HomeFailure>(
      request: request,
      mapFailure: (details) {
        return switch (details.kind) {
          ApiFailureKind.network => const HomeFailureNetwork(
            message: 'Network error',
          ),
          ApiFailureKind.server ||
          ApiFailureKind.unauthorized => HomeFailureServer(
            message: details.message,
            statusCode: details.statusCode,
          ),
          ApiFailureKind.unknown => HomeFailureUnknown(
            message: details.message,
          ),
        };
      },
      mapUnknownFailure: (error) =>
          HomeFailureUnknown(message: error.toString()),
    );
  }

  @override
  Future<Either<HomeFailure, void>> goOnline({
    required double latitude,
    required double longitude,
  }) async {
    return _run(() async {
      await _remote.goOnline(
        GoOnlineRequestDto(latitude: latitude, longitude: longitude),
      );
    });
  }

  @override
  Future<Either<HomeFailure, void>> goOffline({
    required double latitude,
    required double longitude,
  }) async {
    return _run(() async {
      await _remote.goOffline(
        GoOnlineRequestDto(latitude: latitude, longitude: longitude),
      );
    });
  }

  @override
  Future<Either<HomeFailure, void>> updateLocation({
    required double latitude,
    required double longitude,
  }) async {
    return _run(() async {
      await _remote.pingLocation(latitude: latitude, longitude: longitude);
    });
  }
}
