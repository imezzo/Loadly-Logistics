import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import '../mappers/collector_trips_mapper.dart';
import '../mappers/document_mapper.dart';

import '../../domain/entities/collector_trips.dart' as trip;

import '../../domain/entities/document.dart';

import '../../domain/failures/home_failure.dart';

import '../../domain/repositories/home_repository.dart';
import '../sources/remote/home_remote_data_source.dart';

class HomeImpl implements HomeRepository {
  HomeImpl(this._remote);

  final HomeRemoteDataSource _remote;
  HomeFailure _serverFailure(DioException e) {
    final statusCode = e.response?.statusCode;
    final message = e.error is String
        ? e.error as String
        : (e.response?.data is Map ? (e.response?.data as Map)['message']?.toString() : null);
    return HomeFailureServer(message: message, statusCode: statusCode);
  }

  @override
  Future<Either<HomeFailure, trip.CollectorTripsList>> getCollectorTrips({
    int? page,
    int? pageSize,
    String? search,
    int? contractId,
    String? contractReferenceNumber,
    int? companyId,
    int? vehicleId,
    int? vehicleTypeId,
    int? driverId,
    int? status,
    List<int>? tripDocumentStatuses,
    String? startDate,
    String? endDate,
    String? companyName,
    String? driverName,
    String? plate,
  }) async {
    try {
      final response = await _remote.getCollectorTrips(
        page: page,
        pageSize: pageSize,
        search: search,
        contractId: contractId,
        contractReferenceNumber: contractReferenceNumber,
        companyId: companyId,
        vehicleId: vehicleId,
        driverId: driverId,
        vehicleTypeId: vehicleTypeId,
        status: status,
        tripDocumentStatuses: tripDocumentStatuses,
        startDate: startDate,
        endDate: endDate,
        companyName: companyName,
        driverName: driverName,
        plate: plate,
      );
      final trips = response.toEntity();
      return Right(trips);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(HomeFailureNetwork());
      }
      return Left(_serverFailure(e));
    } catch (e, stackTrace) {
      debugPrint('getCollectorTrips failed: $e\n$stackTrace');
      return const Left(HomeFailureUnknown());
    }
  }

  @override
  Future<Either<HomeFailure, List<Document>>> getDocuments() async {
    try {
      final response = await _remote.getDocuments();
      return Right(response.toEntityList());
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(HomeFailureNetwork());
      }
      return Left(_serverFailure(e));
    } catch (e, stackTrace) {
      debugPrint('getDocuments failed: $e\n$stackTrace');
      return const Left(HomeFailureUnknown());
    }
  }
}
