import 'package:dio/dio.dart';

import '../../../../../core/network/api_endpoints.dart';
import '../../../../../core/network/api_response_parser.dart';
import '../../models/go_online_request_dto.dart';

abstract interface class HomeRemoteDataSource {
  Future<void> goOnline(GoOnlineRequestDto request);
  Future<void> goOffline(GoOnlineRequestDto request);
  Future<void> pingLocation({
    required double latitude,
    required double longitude,
  });
}

class HomeRemoteDataSourceImpl implements HomeRemoteDataSource {
  HomeRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<void> goOnline(GoOnlineRequestDto request) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.goOnline,
      data: request.toJson(),
    );
    throwIfEnvelopeFailed(response, fallbackMessage: 'Failed to go online');
  }

  @override
  Future<void> goOffline(GoOnlineRequestDto request) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.goOffline,
      data: request.toJson(),
    );
    throwIfEnvelopeFailed(response, fallbackMessage: 'Failed to go offline');
  }

  @override
  Future<void> pingLocation({
    required double latitude,
    required double longitude,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.locationPing,
      options: Options(
        headers: {
          'X-Latitude': latitude.toString(),
          'X-Longitude': longitude.toString(),
        },
      ),
    );
    throwIfEnvelopeFailed(response, fallbackMessage: 'Failed to ping location');
  }
}
