import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';

import '../../../../../core/errors/api_fallback_keys.dart';
import '../../../../../core/network/api_endpoints.dart';
import '../../../../../core/network/api_response_parser.dart';
import '../models/booking_request_dto.dart';
import '../models/paged_list.dart';
import '../models/trip_dto.dart';
import '../models/trip_statistics_dto.dart';

abstract interface class TripRemoteDataSource {
  Future<TripDto> getTrip(int tripId);

  Future<TripsListResponseDto> getTrips({
    int page = 1,
    int pageSize = 10,
    int? status,
    List<int>? statuses,
    bool? openInviteOnly,
    String? search,
    int? vehicleTypeId,
    String? startDateFrom,
    String? startDateTo,
    bool? isContractorPaid,
    bool? hasContractor,
  });

  Future<PagedList<BookingRequestDto>> getBookingRequests({
    int page = 1,
    int pageSize = 10,
    int? status,
    String? search,
    int? vehicleTypeId,
    String? startDateFrom,
    String? startDateTo,
  });

  Future<void> bookTrip({required int tripId});

  Future<void> withdrawTrip({required int tripId});

  Future<InvitationStatisticsDto> getInvitationStatistics();

  Future<TripStatusStatisticsDto> getStatusStatistics();

  Future<PayoutStatisticsDto> getPayoutStatistics();
}

class TripRemoteDataSourceImpl implements TripRemoteDataSource {
  TripRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  static const _uploadTimeout = Duration(seconds: 60);

  Future<Response<Map<String, dynamic>>> _get(
    String path, {
    Map<String, dynamic>? queryParameters,
  }) {
    return _dio.get<Map<String, dynamic>>(
      path,
      queryParameters: queryParameters,
    );
  }

  Future<void> _postSuccess(
    String path, {
    Object? data,
    Options? options,
    String fallbackMessage = ApiFallbackKeys.requestFailed,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      path,
      data: data,
      options: options,
    );
    throwIfEnvelopeSuccessFalse(response, fallbackMessage: fallbackMessage);
  }

  List<T> _mapListItems<T>(
    List<dynamic> data,
    T Function(Map<String, dynamic> json) fromJson,
  ) {
    final items = <T>[];
    for (final item in data) {
      if (item is Map) {
        items.add(fromJson(Map<String, dynamic>.from(item)));
      }
    }
    return items;
  }

  @override
  Future<TripDto> getTrip(int tripId) async {
    final response = await _get(ApiEndpoints.tripById(tripId));
    throwIfEnvelopeSuccessFalse(
      response,
      fallbackMessage: ApiFallbackKeys.loadTrip,
    );
    final envelope = requireEnvelope(response);
    final data = envelope['data'] ?? envelope['Data'];
    if (data is Map<String, dynamic>) {
      return TripDto.fromJson(data);
    }
    if (data is Map) {
      return TripDto.fromJson(Map<String, dynamic>.from(data));
    }
    throw DioException(
      requestOptions: response.requestOptions,
      response: response,
      type: DioExceptionType.badResponse,
      error: ApiFallbackKeys.invalidResponse,
    );
  }

  @override
  Future<TripsListResponseDto> getTrips({
    int page = 1,
    int pageSize = 10,
    int? status,
    List<int>? statuses,
    bool? openInviteOnly,
    String? search,
    int? vehicleTypeId,
    String? startDateFrom,
    String? startDateTo,
    bool? isContractorPaid,
    bool? hasContractor,
  }) async {
    final response = await _get(
      ApiEndpoints.trips,
      queryParameters: {
        'page': page,
        'pageSize': pageSize,
        'status': ?status,
        'statuses': ?statuses,
        'openInviteOnly': ?openInviteOnly,
        'search': ?search,
        'vehicleTypeId': ?vehicleTypeId,
        'StartDateFrom': ?startDateFrom,
        'StartDateTo': ?startDateTo,
        'IsContractorPaid': ?isContractorPaid,
        'HasContractor': ?hasContractor,
      },
    );

    throwIfEnvelopeSuccessFalse(
      response,
      fallbackMessage: ApiFallbackKeys.loadTrips,
    );
    final envelope = requireEnvelope(response);
    final listResponse = TripsListResponseDto.fromJson(envelope);

    if (kDebugMode && listResponse.data.isNotEmpty) {
      debugPrint('TRIP_RAW ${jsonEncode(listResponse.data.first.toJson())}');
    }

    return listResponse;
  }

  @override
  Future<PagedList<BookingRequestDto>> getBookingRequests({
    int page = 1,
    int pageSize = 10,
    int? status,
    String? search,
    int? vehicleTypeId,
    String? startDateFrom,
    String? startDateTo,
  }) async {
    final response = await _get(
      ApiEndpoints.bookingRequests,
      queryParameters: {
        'page': page,
        'pageSize': pageSize,
        'Status': ?status,
        'search': ?search,
        'vehicleTypeId': ?vehicleTypeId,
        'StartDateFrom': ?startDateFrom,
        'StartDateTo': ?startDateTo,
      },
    );

    final data = requireEnvelopeList(
      response,
      fallbackMessage: ApiFallbackKeys.loadBookingRequests,
      invalidListMessage: ApiFallbackKeys.invalidResponse,
    );

    if (kDebugMode && data.isNotEmpty) {
      debugPrint('BOOKING_RAW ${jsonEncode(data.first)}');
    }

    final items = _mapListItems(data, BookingRequestDto.fromJson);
    final envelope = requireEnvelope(response);

    return PagedList(
      items: items,
      totalCount: totalCountFromEnvelope(envelope, items.length),
    );
  }

  @override
  Future<void> bookTrip({required int tripId}) async {
    await _postSuccess(
      ApiEndpoints.bookTrip(tripId),
      options: Options(
        sendTimeout: _uploadTimeout,
        receiveTimeout: _uploadTimeout,
      ),
      fallbackMessage: ApiFallbackKeys.bookTrip,
    );
  }

  @override
  Future<void> withdrawTrip({required int tripId}) async {
    await _postSuccess(
      ApiEndpoints.withdrawTrip(tripId),
      fallbackMessage: ApiFallbackKeys.withdrawBooking,
    );
  }

  @override
  Future<InvitationStatisticsDto> getInvitationStatistics() async {
    final data = await _statisticsData(ApiEndpoints.invitationStatistics);
    return InvitationStatisticsDto.fromJson(data);
  }

  @override
  Future<TripStatusStatisticsDto> getStatusStatistics() async {
    final data = await _statisticsData(ApiEndpoints.statusStatistics);
    return TripStatusStatisticsDto.fromJson(data);
  }

  @override
  Future<PayoutStatisticsDto> getPayoutStatistics() async {
    final data = await _statisticsData(ApiEndpoints.payoutStatistics);
    return PayoutStatisticsDto.fromJson(data);
  }

  Future<Map<String, dynamic>> _statisticsData(String path) async {
    final response = await _get(path);
    throwIfEnvelopeSuccessFalse(
      response,
      fallbackMessage: ApiFallbackKeys.loadTrips,
    );
    final envelope = requireEnvelope(response);
    final data = envelope['data'] ?? envelope['Data'];
    if (data is Map<String, dynamic>) return data;
    if (data is Map) return Map<String, dynamic>.from(data);
    throw DioException(
      requestOptions: response.requestOptions,
      response: response,
      type: DioExceptionType.badResponse,
      error: ApiFallbackKeys.invalidResponse,
    );
  }
}
