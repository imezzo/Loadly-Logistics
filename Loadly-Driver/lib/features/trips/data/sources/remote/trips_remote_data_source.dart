import 'dart:io';

import 'package:dio/dio.dart';

import '../../../../../core/network/api_endpoints.dart';
import '../../models/trip_action_params.dart';
import '../../models/trip_dto.dart';

abstract interface class TripsRemoteDataSource {
  Future<TripsListResponseDto> getTrips({
    int? page,
    int? limit,
    int? status,
    String? startDateFrom,
    String? startDateTo,
  });
  Future<TripDto> getTripDetails(int tripId);
  Future<String> getDocumentDownloadUrl(int tripId, String path);
  Future<TripDto> startLoading(
    int tripId,
    int waypointId,
    TripLocation? location,
  );
  Future<TripDto> completeLoading(
    int tripId,
    int waypointId,
    CompleteLoadingParams params,
  );
  Future<TripDto> startUnloading(
    int tripId,
    int waypointId,
    TripLocation? location,
  );
  Future<TripDto> completeUnloading(
    int tripId,
    int waypointId,
    CompleteUnloadingParams params,
  );
  Future<TripDto> startTrip(int tripId, StartTripParams params);
  Future<TripDto> arriveAtWaypoint(
    int tripId,
    int waypointId,
    TripLocation? location,
  );
  Future<TripDto> departFromWaypoint(
    int tripId,
    int waypointId,
    DepartWaypointParams params,
  );
  Future<TripDto> enterCustoms(int tripId, EnterCustomsParams params);
  Future<TripDto> exitCustoms(int tripId, ExitCustomsParams params);
  Future<TripDto> completeTrip(int tripId, CompleteTripParams params);
  Future<ValidateOdometerResponseDto> validateOdometerImage(
    int tripId,
    File image,
  );
  Future<void> syncTripRoute(int tripId, Map<String, dynamic> body);
}

class TripsRemoteDataSourceImpl implements TripsRemoteDataSource {
  TripsRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  Map<String, dynamic> _headersWithLocation(TripLocation? location) {
    if (location == null) return {};
    return location.toHeaders();
  }

  @override
  Future<TripsListResponseDto> getTrips({
    int? page,
    int? limit,
    int? status,
    String? startDateFrom,
    String? startDateTo,
  }) async {
    final queryParams = <String, dynamic>{
      'page': page ?? 1,
      'pageSize': limit ?? 10,
      'status': ?status,
      'startDateFrom': ?startDateFrom,
      'startDateTo': ?startDateTo,
    };

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.trips,
      queryParameters: queryParams.isEmpty ? null : queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    final dataList = data['data'];
    final tripsList = dataList is List ? dataList : <dynamic>[];
    final pagination = data['pagination'] as Map<String, dynamic>?;

    final trips = tripsList
        .map(
          (e) =>
              TripDto.fromJson(_normalizeTripJson(e as Map<String, dynamic>)),
        )
        .toList();

    return TripsListResponseDto(
      trips: trips,
      total: (pagination?['totalCount'] as num?)?.toInt(),
      page: (pagination?['page'] as num?)?.toInt(),
      limit: (pagination?['pageSize'] as num?)?.toInt(),
    );
  }

  @override
  Future<TripDto> getTripDetails(int tripId) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.trip(tripId),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    final responseData = data['data'];
    if (responseData == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Invalid response format',
      );
    }

    return TripDto.fromJson(
      _normalizeTripJson(responseData as Map<String, dynamic>),
    );
  }

  @override
  Future<String> getDocumentDownloadUrl(int tripId, String documentPath) async {
    final apiPath =
        '${ApiEndpoints.trips}/$tripId/documents/download?path=${Uri.encodeComponent(documentPath)}';
    final response = await _dio.get(
      apiPath,
      options: Options(
        followRedirects: false,
        validateStatus: (s) => s != null && s < 400,
      ),
    );
    final location = response.headers.value('location');
    if (location != null && location.isNotEmpty) {
      return location.startsWith('http')
          ? location
          : '${_dio.options.baseUrl}$location';
    }
    return '${_dio.options.baseUrl}$apiPath';
  }

  @override
  Future<TripDto> startLoading(
    int tripId,
    int waypointId,
    TripLocation? location,
  ) async {
    final headers = _headersWithLocation(location);
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripWaypointLoadingStart(tripId, waypointId),
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<TripDto> completeLoading(
    int tripId,
    int waypointId,
    CompleteLoadingParams params,
  ) async {
    final headers = _headersWithLocation(params.location);
    final receiptNoteImages = await Future.wait(
      params.receiptNoteImages.map(
        (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
      ),
    );
    final formData = FormData.fromMap({
      'receiptNoteImages': receiptNoteImages,
      // Keep legacy field for backward compatibility with older backend payloads.
      if (params.receiptNoteImages.isNotEmpty)
        'receiptNoteImage': await MultipartFile.fromFile(
          params.receiptNoteImages.first.path,
          filename: _fileName(params.receiptNoteImages.first.path),
        ),
      'shipmentImages': await Future.wait(
        params.shipmentImages.map(
          (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
        ),
      ),
      if (params.note != null) 'note': params.note,
    });
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripWaypointLoadingComplete(tripId, waypointId),
      data: formData,
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<TripDto> startUnloading(
    int tripId,
    int waypointId,
    TripLocation? location,
  ) async {
    final headers = _headersWithLocation(location);
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripWaypointUnloadingStart(tripId, waypointId),
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<TripDto> completeUnloading(
    int tripId,
    int waypointId,
    CompleteUnloadingParams params,
  ) async {
    final headers = _headersWithLocation(params.location);
    final deliveryNoteImages = await Future.wait(
      params.deliveryNoteImages.map(
        (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
      ),
    );
    final map = <String, dynamic>{
      'unloadingImages': await Future.wait(
        params.unloadingImages.map(
          (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
        ),
      ),
      'deliveryNoteImages': deliveryNoteImages,
      // Keep legacy field for backward compatibility with older backend payloads.
      if (params.deliveryNoteImages.isNotEmpty)
        'deliveryNoteImage': await MultipartFile.fromFile(
          params.deliveryNoteImages.first.path,
          filename: _fileName(params.deliveryNoteImages.first.path),
        ),
      if (params.note != null) 'note': params.note,
    };
    if (params.odometer != null) map['odometer'] = params.odometer!;
    if (params.odometerImages != null && params.odometerImages!.isNotEmpty) {
      final odometerImages = await Future.wait(
        params.odometerImages!.map(
          (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
        ),
      );
      map['odometerImages'] = odometerImages;
      final firstOdometerImage = params.odometerImages!.first;
      map['odometerImage'] = await MultipartFile.fromFile(
        firstOdometerImage.path,
        filename: _fileName(firstOdometerImage.path),
      );
    }
    final formData = FormData.fromMap(map);
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripWaypointUnloadingComplete(tripId, waypointId),
      data: formData,
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<TripDto> startTrip(int tripId, StartTripParams params) async {
    final headers = <String, dynamic>{
      'Content-Type': 'application/json',
      ..._headersWithLocation(params.location),
    };
    final body = <String, dynamic>{};
    if (params.note != null) body['note'] = params.note;
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripStart(tripId),
      data: body,
      options: Options(headers: headers),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final responseData = data['data'] ?? data;
    if (responseData is Map<String, dynamic> &&
        responseData.containsKey('id') &&
        responseData.containsKey('contractId')) {
      return TripDto.fromJson(_normalizeTripJson(responseData));
    }
    // API returns { message, nextWaypoint } on success - fetch updated trip
    return getTripDetails(tripId);
  }

  @override
  Future<TripDto> arriveAtWaypoint(
    int tripId,
    int waypointId,
    TripLocation? location,
  ) async {
    final headers = _headersWithLocation(location);
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripWaypointArrive(tripId, waypointId),
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<TripDto> departFromWaypoint(
    int tripId,
    int waypointId,
    DepartWaypointParams params,
  ) async {
    final headers = _headersWithLocation(params.location);
    final map = <String, dynamic>{};
    if (params.deliveryNoteImages != null &&
        params.deliveryNoteImages!.isNotEmpty) {
      final deliveryNoteImages = await Future.wait(
        params.deliveryNoteImages!.map(
          (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
        ),
      );
      map['deliveryNoteImages'] = deliveryNoteImages;
      // Keep legacy field for backward compatibility with older backend payloads.
      final firstDeliveryNote = params.deliveryNoteImages!.first;
      map['deliveryNoteImage'] = await MultipartFile.fromFile(
        firstDeliveryNote.path,
        filename: _fileName(firstDeliveryNote.path),
      );
    }
    if (params.odometer != null) map['odometer'] = params.odometer!;
    if (params.odometerImages != null && params.odometerImages!.isNotEmpty) {
      final odometerImages = await Future.wait(
        params.odometerImages!.map(
          (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
        ),
      );
      map['odometerImages'] = odometerImages;
      // Keep legacy field for backward compatibility with older backend payloads.
      final firstOdometerImage = params.odometerImages!.first;
      map['odometerImage'] = await MultipartFile.fromFile(
        firstOdometerImage.path,
        filename: _fileName(firstOdometerImage.path),
      );
    }
    if (params.note != null) map['note'] = params.note;

    final formData = FormData.fromMap(map);
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripWaypointDepart(tripId, waypointId),
      data: formData,
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<TripDto> enterCustoms(int tripId, EnterCustomsParams params) async {
    final headers = _headersWithLocation(params.location);
    final formData = FormData.fromMap({
      if (params.customDocuments.isNotEmpty)
        'customDocuments': await Future.wait(
          params.customDocuments.map(
            (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
          ),
        ),
      if (params.note != null) 'note': params.note,
    });
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripEnterCustoms(tripId),
      data: formData,
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<TripDto> exitCustoms(int tripId, ExitCustomsParams params) async {
    final headers = _headersWithLocation(params.location);
    final formData = FormData.fromMap({
      if (params.customsReceipts.isNotEmpty)
        'customsReceipts': await Future.wait(
          params.customsReceipts.map(
            (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
          ),
        ),
      if (params.note != null) 'note': params.note,
      'customsTotalAmount': params.customsTotalAmount,
    });
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripExitCustoms(tripId),
      data: formData,
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<TripDto> completeTrip(int tripId, CompleteTripParams params) async {
    final headers = _headersWithLocation(params.location);
    final odometerImages = await Future.wait(
      params.odometerImages.map(
        (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
      ),
    );
    final formData = FormData.fromMap({
      'odometer': params.odometer,
      'odometerImages': odometerImages,
      // Keep legacy field for backward compatibility with older backend payloads.
      if (params.odometerImages.isNotEmpty)
        'odometerImage': await MultipartFile.fromFile(
          params.odometerImages.first.path,
          filename: _fileName(params.odometerImages.first.path),
        ),
      'tollgateImages': await Future.wait(
        params.tollgateImages.map(
          (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
        ),
      ),
      'tollgateWeightImages': await Future.wait(
        params.tollgateWeightImages.map(
          (f) => MultipartFile.fromFile(f.path, filename: _fileName(f.path)),
        ),
      ),
      'note': params.note,
    });
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.tripComplete(tripId),
      data: formData,
      options: Options(headers: headers.isNotEmpty ? headers : null),
    );
    return await _parseTripResponseOrFetch(response, tripId);
  }

  @override
  Future<ValidateOdometerResponseDto> validateOdometerImage(
    int tripId,
    File image,
  ) async {
    final formData = FormData.fromMap({
      'odometerImage': await MultipartFile.fromFile(
        image.path,
        filename: _fileName(image.path),
      ),
    });
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.validateOdometerImage(tripId),
      data: formData,
      options: Options(headers: {'Content-Type': 'multipart/form-data'}),
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final responseData = data['data'] ?? data;
    return ValidateOdometerResponseDto.fromJson(responseData);
  }

  @override
  Future<void> syncTripRoute(int tripId, Map<String, dynamic> body) async {
    await _dio.post<void>(
      ApiEndpoints.tripRouteSync(tripId),
      data: body,
      options: Options(headers: {'Content-Type': 'application/json'}),
    );
  }

  /// Parses response as trip if it has full trip shape; otherwise fetches trip details.
  Future<TripDto> _parseTripResponseOrFetch(
    Response<Map<String, dynamic>> response,
    int tripId,
  ) async {
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final responseData = data['data'] ?? data;
    if (responseData is Map<String, dynamic> &&
        responseData.containsKey('id') &&
        responseData.containsKey('contractId')) {
      return TripDto.fromJson(_normalizeTripJson(responseData));
    }
    return getTripDetails(tripId);
  }

  String _fileName(String path) {
    final parts = path.replaceAll('\\', '/').split('/');
    return parts.isNotEmpty ? parts.last : 'image.jpg';
  }

  /// Normalizes API response to match DTO expectations.
  /// API returns vehicle.id, plateNo, plateLetters, vehicleType.nameEn/nameAr
  /// while DTOs expect vehicleId, contractId, plateNumber, vehicleTypeName, vehicleTypeNameAr.
  Map<String, dynamic> _normalizeTripJson(Map<String, dynamic> json) {
    final normalized = Map<String, dynamic>.from(json);

    // contractId: API may omit it; derive from contract.id or use 0
    if (normalized['contractId'] == null) {
      final contract = normalized['contract'];
      if (contract is Map<String, dynamic>) {
        final id = contract['id'];
        if (id != null) normalized['contractId'] = id;
      }
      normalized['contractId'] ??= 0;
    }

    // vehicleId: API may omit it; derive from vehicle.id
    if (normalized['vehicleId'] == null) {
      final vehicle = normalized['vehicle'];
      if (vehicle is Map<String, dynamic>) {
        final id = vehicle['id'];
        if (id != null) normalized['vehicleId'] = id;
      }
    }

    // vehicle: flatten plateNo+plateLetters -> plateNumber, vehicleType -> vehicleTypeName/Ar
    final vehicle = normalized['vehicle'];
    if (vehicle is Map<String, dynamic>) {
      final vehicleNorm = Map<String, dynamic>.from(vehicle);
      if (vehicleNorm['plateNumber'] == null) {
        final plateNo = vehicleNorm['plateNo']?.toString() ?? '';
        final plateLetters = vehicleNorm['plateLetters']?.toString() ?? '';
        vehicleNorm['plateNumber'] = '$plateNo $plateLetters'.trim();
      }
      if (vehicleNorm['vehicleTypeName'] == null) {
        final vt = vehicleNorm['vehicleType'];
        if (vt is Map<String, dynamic>) {
          vehicleNorm['vehicleTypeName'] = vt['nameEn'];
          vehicleNorm['vehicleTypeNameAr'] = vt['nameAr'];
        }
      }
      normalized['vehicle'] = vehicleNorm;
    }

    return normalized;
  }
}
