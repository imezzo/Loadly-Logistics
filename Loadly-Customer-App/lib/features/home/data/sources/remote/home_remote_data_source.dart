import 'package:dio/dio.dart';

import '../../../../../core/network/api_endpoints.dart';
import '../../models/monthly_trips_kilometers_response_dto.dart';
import '../../models/trips_statistics_response_dto.dart';

abstract interface class HomeRemoteDataSource {
  Future<TripsStatisticsResponseDto> getTripsStatistics({
    int? year,
    required bool lastMonth,
    required bool lastWeek,
  });
  Future<List<MonthlyTripsKilometersResponseDto>> getMonthlyTripsKilometers({
    int? year,
    int? month,
  });
}

class HomeRemoteDataSourceImpl implements HomeRemoteDataSource {
  HomeRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<TripsStatisticsResponseDto> getTripsStatistics({
    int? year,
    required bool lastMonth,
    required bool lastWeek,
  }) async {
    final queryParams = <String, dynamic>{};
    if (year != null) queryParams['year'] = year;
    queryParams['lastMonth'] = lastMonth;
    queryParams['lastWeek'] = lastWeek;

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.tripsStatistics,
      queryParameters: queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    final tripsStatistics = data['data'];
    return TripsStatisticsResponseDto.fromJson(tripsStatistics);
  }

  @override
  Future<List<MonthlyTripsKilometersResponseDto>> getMonthlyTripsKilometers({
    int? year,
    int? month,
  }) async {
    final queryParams = <String, dynamic>{};
    if (year != null) queryParams['year'] = year;
    if (month != null) queryParams['month'] = month;

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.monthlyTripsKilometers,
      queryParameters: queryParams.isEmpty ? null : queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    final dataList = data['data'];
    final monthlyTripsKilometers = dataList is List ? dataList : <dynamic>[];
    return monthlyTripsKilometers
        .map(
          (e) => MonthlyTripsKilometersResponseDto.fromJson(
            e as Map<String, dynamic>,
          ),
        )
        .toList();
  }
}
