import 'package:dio/dio.dart';

import '../../../../../core/network/api_endpoints.dart';
import '../../models/add_trip_request_dto.dart';
import '../../models/destination_dto.dart';
import '../../models/extract_place_response_dto.dart';
import '../../models/lookup_dto.dart';
import '../../models/track_trips_response_dto.dart';
import '../../models/trip_dto.dart' hide ContractDto;

abstract interface class TripsRemoteDataSource {
  Future<TripsListDto> getTrips({
    int? page,
    int? limit,
    int? status,
    String? search,
    int? vehicleTypeId,
    int? contractId,
    String? startDate,
    String? endDate,
  });
  Future<TripDto> getTripDetails(int tripId);
  Future<List<LookupDto>> getContractReferenceNumbers();
  Future<List<LookupDto>> getVehicleTypes({int? contractId});
  Future<int> getConsumptionType({required int contractId});
  Future<List<LookupDto>> getContractDestinations({required int contractId});
  Future<DestinationDto> getContractDestination({required int destinationId});
  Future<ExtractPlaceResponseDto> extractPlace(String url);
  Future<void> addTrip(AddTripRequestDto request);
  Future<void> updateTrip({
    required int tripId,
    required AddTripRequestDto request,
  });
  Future<void> deleteTrip(int tripId);
  Future<List<TrackTripsResponseDto>> trackTrips();
}

class TripsRemoteDataSourceImpl implements TripsRemoteDataSource {
  TripsRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<TripsListDto> getTrips({
    int? page,
    int? limit,
    int? status,
    String? search,
    int? vehicleTypeId,
    int? contractId,
    String? startDate,
    String? endDate,
  }) async {
    final queryParams = <String, dynamic>{};
    if (page != null) queryParams['page'] = page;
    if (limit != null) queryParams['pageSize'] = limit;
    if (status != null) queryParams['status'] = status;
    if (search != null) queryParams['search'] = search;
    if (vehicleTypeId != null) queryParams['vehicleTypeId'] = vehicleTypeId;
    if (contractId != null) queryParams['contractId'] = contractId;
    if (startDate != null) queryParams['startDateFrom'] = startDate;
    if (endDate != null) queryParams['startDateTo'] = endDate;

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.trips,
      queryParameters: queryParams.isEmpty ? null : queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    final tripsList = dataList is List ? dataList : <dynamic>[];
    return TripsListDto(
      trips: tripsList
          .map((e) => TripDto.fromJson(e as Map<String, dynamic>))
          .toList(),
      pagination: PaginationDto.fromJson(data['pagination']),
    );
  }

  @override
  Future<TripDto> getTripDetails(int tripId) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '${ApiEndpoints.trips}/$tripId',
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // Handle API response envelope
    final responseData = data['data'];
    if (responseData == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Invalid response format',
      );
    }

    return TripDto.fromJson(responseData as Map<String, dynamic>);
  }

  @override
  Future<List<LookupDto>> getVehicleTypes({int? contractId}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.vehicleTypes,
      queryParameters: {'pageSize': 100, 'contractId': ?contractId},
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    final vehicleTypesList = dataList is List ? dataList : <dynamic>[];
    return vehicleTypesList
        .map((e) => LookupDto.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  @override
  Future<List<LookupDto>> getContractReferenceNumbers() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.contractReferenceNumbers,
      queryParameters: {'pageSize': 100},
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    final contractsList = dataList is List ? dataList : <dynamic>[];
    return contractsList
        .map((e) => LookupDto.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  @override
  Future<int> getConsumptionType({required int contractId}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.consumptionType(contractId),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    return dataList['consumptionType'] as int? ?? 0;
  }

  @override
  Future<List<LookupDto>> getContractDestinations({
    required int contractId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.contractDestinations,
      queryParameters: {'pageSize': 100, 'contractId': contractId},
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    final destinationsList = dataList is List ? dataList : <dynamic>[];
    return destinationsList
        .map((e) => LookupDto.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  @override
  Future<DestinationDto> getContractDestination({
    required int destinationId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.contractDestination(destinationId),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    return DestinationDto.fromJson(dataList);
  }

  @override
  Future<ExtractPlaceResponseDto> extractPlace(String url) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.extractPlace,
      queryParameters: {'url': url},
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // Handle API response envelope
    final responseData = data['data'];
    if (responseData == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Invalid response format',
      );
    }

    return ExtractPlaceResponseDto.fromJson(
      responseData as Map<String, dynamic>,
    );
  }

  @override
  Future<void> addTrip(AddTripRequestDto request) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.trips,
      data: request.toJson(),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final success = data['success'] as bool? ?? false;
    final message = data['message'] as String? ?? '';
    if (!success) {
      final errors = data['errors'];
      final errorMessage =
          message.isNotEmpty ||
              !message.startsWith('One or more') ||
              !message.startsWith('حدث خطأ أو')
          ? message
          : (errors is Map
                        ? (errors['message'] ?? errors.toString())
                        : data['message'])
                    ?.toString() ??
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> updateTrip({
    required int tripId,
    required AddTripRequestDto request,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      '${ApiEndpoints.trips}/$tripId',
      data: request.toJson(),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final success = data['success'] as bool? ?? false;
    final message = data['message'] as String? ?? '';
    if (!success) {
      final errors = data['errors'];
      final errorMessage =
          message.isNotEmpty ||
              !message.startsWith('One or more') ||
              !message.startsWith('حدث خطأ أو')
          ? message
          : (errors is Map
                        ? (errors['message'] ?? errors.toString())
                        : data['message'])
                    ?.toString() ??
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> deleteTrip(int tripId) async {
    final response = await _dio.delete<Map<String, dynamic>>(
      '${ApiEndpoints.trips}/$tripId',
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final success = data['success'] as bool? ?? false;
    final message = data['message'] as String? ?? '';
    if (!success) {
      final errors = data['errors'];
      final errorMessage = message.isNotEmpty ? message : errors.toString();
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<List<TrackTripsResponseDto>> trackTrips() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.trackTrips,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    final dataList = data['data'];
    final trackedTrips = dataList is List ? dataList : <dynamic>[];
    return trackedTrips
        .map((e) => TrackTripsResponseDto.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
