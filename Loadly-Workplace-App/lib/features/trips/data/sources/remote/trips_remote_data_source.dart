import 'package:dio/dio.dart';

import '../../../../../core/errors/user_message_codes.dart';
import '../../../../../core/network/api_endpoints.dart';
import '../../../../../core/network/api_failure_mapper.dart';
import '../../models/bulk_collect_trip_documents_request_dto.dart';
import '../../models/collect_trip_documents_request_dto.dart';

abstract interface class TripsRemoteDataSource {
  Future<void> collectTripDocuments({
    required int tripId,
    required CollectorDocuments documents,
  });
  Future<void> confirmTripDocuments({
    required int tripId,
    required CollectorDocuments documents,
  });
  Future<void> bulkCollectTripDocuments(
    BulkCollectTripDocumentsRequestDto request,
  );
  Future<void> bulkConfirmTripDocuments(
    BulkCollectTripDocumentsRequestDto request,
  );
}

class TripsRemoteDataSourceImpl implements TripsRemoteDataSource {
  TripsRemoteDataSourceImpl(this._dio);

  final Dio _dio;
  @override
  Future<void> collectTripDocuments({
    required int tripId,
    required CollectorDocuments documents,
  }) async {
    final formData = await documents.toFormData();
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.collectTripDocuments(tripId),
      data: formData,
      options: Options(contentType: 'multipart/form-data'),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final success = data['success'] as bool? ?? false;
    if (!success) {
      final errorMessage =
          extractApiMessage(data) ?? UserMessageCodes.requestFailed;
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> confirmTripDocuments({
    required int tripId,
    required CollectorDocuments documents,
  }) async {
    final formData = await documents.toFormData();
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.confirmTripDocuments(tripId),
      data: formData,
      options: Options(contentType: 'multipart/form-data'),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final success = data['success'] as bool? ?? false;
    if (!success) {
      final errorMessage =
          extractApiMessage(data) ?? UserMessageCodes.requestFailed;
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> bulkCollectTripDocuments(
    BulkCollectTripDocumentsRequestDto request,
  ) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.bulkCollectTripDocuments,
      data: request.toJson(),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final success = data['success'] as bool? ?? false;
    if (!success) {
      final errorMessage =
          extractApiMessage(data) ?? UserMessageCodes.requestFailed;
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> bulkConfirmTripDocuments(
    BulkCollectTripDocumentsRequestDto request,
  ) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.bulkConfirmTripDocuments,
      data: request.toJson(),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final success = data['success'] as bool? ?? false;
    if (!success) {
      final errorMessage =
          extractApiMessage(data) ?? UserMessageCodes.requestFailed;
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }
}
