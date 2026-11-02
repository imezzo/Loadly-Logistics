import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';

import '../models/bulk_collect_trip_documents_request_dto.dart';
import '../models/collect_trip_documents_request_dto.dart';
import '../../domain/failures/trips_failure.dart';
import '../../domain/repositories/trips_repository.dart';
import '../sources/remote/trips_remote_data_source.dart';

class TripsImpl implements TripsRepository {
  TripsImpl(this._remote);

  final TripsRemoteDataSource _remote;

  TripsFailure _serverFailure(DioException e) {
    final statusCode = e.response?.statusCode;
    final message = e.error is String
        ? e.error as String
        : (e.response?.data is Map
              ? (e.response?.data as Map)['message']?.toString()
              : null);
    return TripsFailureServer(message: message, statusCode: statusCode);
  }

  @override
  Future<Either<TripsFailure, void>> collectTripDocuments({
    required int tripId,
    required List<CollectTripDocumentsRequestDto> collectorDocuments,
    required bool submit,
  }) async {
    try {
      await _remote.collectTripDocuments(
        tripId: tripId,
        documents: CollectorDocuments(collectorDocuments, submit),
      );

      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork());
      }
      return Left(_serverFailure(e));
    } catch (e, stackTrace) {
      debugPrint('collectTripDocuments failed: $e\n$stackTrace');
      return const Left(TripsFailureUnknown());
    }
  }

  @override
  Future<Either<TripsFailure, void>> confirmTripDocuments({
    required int tripId,
    required List<CollectTripDocumentsRequestDto> collectorDocuments,
    required bool submit,
  }) async {
    try {
      await _remote.confirmTripDocuments(
        tripId: tripId,
        documents: CollectorDocuments(collectorDocuments, submit),
      );

      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork());
      }
      return Left(_serverFailure(e));
    } catch (e, stackTrace) {
      debugPrint('confirmTripDocuments failed: $e\n$stackTrace');
      return const Left(TripsFailureUnknown());
    }
  }

  @override
  Future<Either<TripsFailure, void>> bulkCollectTripDocuments({
    required List<int> tripIds,
  }) async {
    try {
      await _remote.bulkCollectTripDocuments(
        BulkCollectTripDocumentsRequestDto(tripIds: tripIds),
      );

      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork());
      }
      return Left(_serverFailure(e));
    } catch (e, stackTrace) {
      debugPrint('bulkCollectTripDocuments failed: $e\n$stackTrace');
      return const Left(TripsFailureUnknown());
    }
  }

  @override
  Future<Either<TripsFailure, void>> bulkConfirmTripDocuments({
    required List<int> tripIds,
  }) async {
    try {
      await _remote.bulkConfirmTripDocuments(
        BulkCollectTripDocumentsRequestDto(tripIds: tripIds),
      );

      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork());
      }
      return Left(_serverFailure(e));
    } catch (e, stackTrace) {
      debugPrint('bulkConfirmTripDocuments failed: $e\n$stackTrace');
      return const Left(TripsFailureUnknown());
    }
  }
}
