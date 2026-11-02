import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import '../../domain/entities/monthly_trips_kilometers_result.dart';
import '../../domain/entities/trips_statistics_result.dart';
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
        : (e.response?.data is Map
              ? (e.response?.data as Map)['message']?.toString()
              : null);
    return HomeFailureServer(message: message, statusCode: statusCode);
  }

  @override
  Future<Either<HomeFailure, TripsStatisticsResult>> getTripsStatistics({
    int? year,
    required bool lastMonth,
    required bool lastWeek,
  }) async {
    try {
      final dto = await _remote.getTripsStatistics(
        year: year,
        lastMonth: lastMonth,
        lastWeek: lastWeek,
      );
      final tripsStatistics = TripsStatisticsResult(
        totalTrips: dto.totalTrips,
        scheduledTrips: dto.scheduledTrips,
        inProgressTrips: dto.inProgressTrips,
        canceledTrips: dto.canceledTrips,
        completedTrips: dto.completedTrips,
        otherTrips: dto.otherTrips,
      );
      return Right(tripsStatistics);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(HomeFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(HomeFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<HomeFailure, List<MonthlyTripsKilometersResult>>>
  getMonthlyTripsKilometers({int? year, int? month}) async {
    try {
      final dto = await _remote.getMonthlyTripsKilometers(
        year: year,
        month: month,
      );
      final monthlyTripsKilometers = dto
          .map(
            (e) => MonthlyTripsKilometersResult(
              month: e.month,
              monthName: e.monthName,
              totalTrips: e.totalTrips,
              totalKilometers: e.totalKilometers,
            ),
          )
          .toList();
      return Right(monthlyTripsKilometers);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(HomeFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(HomeFailureUnknown(message: e.toString()));
    }
  }
}
