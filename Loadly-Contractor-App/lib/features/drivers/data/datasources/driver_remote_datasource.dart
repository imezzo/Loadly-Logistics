import 'package:dio/dio.dart';
import 'package:path/path.dart' as p;

import '../../../../../core/errors/api_fallback_keys.dart';
import '../../../../../core/utils/string_utils.dart';
import '../../../../../core/network/api_endpoints.dart';
import '../../../../../core/network/api_response_parser.dart';
import '../models/change_driver_dto.dart';
import '../models/driver_document_input.dart';
import '../models/driver_document_type_dto.dart';
import '../models/driver_dto.dart';
import '../models/driver_model.dart';

abstract interface class DriverRemoteDataSource {
  Future<List<DriverDto>> getDrivers({int page = 1, int pageSize = 10});

  Future<List<DriverDocumentTypeDto>> getDocumentTypes({
    int page = 1,
    int pageSize = 15,
  });

  Future<DriverModel?> createDriver({
    required String fullNameEn,
    required String fullNameAr,
    required String phone,
    required String nationalId,
    required List<DriverDocumentInput> documents,
  });

  Future<void> updateTripDriver({
    required int tripId,
    required ChangeDriverDto request,
  });
}

class DriverRemoteDataSourceImpl implements DriverRemoteDataSource {
  DriverRemoteDataSourceImpl(this._dio);

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

  Future<Map<String, dynamic>> _postEnvelope(
    String path, {
    Object? data,
    String fallbackMessage = ApiFallbackKeys.requestFailed,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(path, data: data);
    throwIfEnvelopeSuccessFalse(response, fallbackMessage: fallbackMessage);
    return requireEnvelope(response);
  }

  Future<void> _putSuccess(
    String path, {
    Object? data,
    Options? options,
    String fallbackMessage = ApiFallbackKeys.requestFailed,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      path,
      data: data,
      options: options,
    );
    throwIfEnvelopeSuccessFalse(response, fallbackMessage: fallbackMessage);
  }

  Map<String, dynamic>? _asMap(dynamic data) {
    if (data is Map<String, dynamic>) return data;
    if (data is Map) return Map<String, dynamic>.from(data);
    if (data is List && data.isNotEmpty) {
      final first = data.first;
      if (first is Map) return Map<String, dynamic>.from(first);
    }
    return null;
  }

  @override
  Future<List<DriverDto>> getDrivers({int page = 1, int pageSize = 10}) async {
    final response = await _get(
      ApiEndpoints.drivers,
      queryParameters: {'page': page, 'pageSize': pageSize},
    );

    final data = requireEnvelopeList(
      response,
      fallbackMessage: ApiFallbackKeys.loadDrivers,
      invalidListMessage: ApiFallbackKeys.invalidResponse,
    );

    return _mapListItems(data, DriverDto.fromJson);
  }

  @override
  Future<List<DriverDocumentTypeDto>> getDocumentTypes({
    int page = 1,
    int pageSize = 15,
  }) async {
    final response = await _get(
      ApiEndpoints.documents,
      queryParameters: {
        'Page': page,
        'PageSize': pageSize,
        'Search': '',
        'Status': 1,
        'entityId': ApiEndpoints.driverDocumentsEntityId,
      },
    );

    final data = requireEnvelopeList(
      response,
      fallbackMessage: ApiFallbackKeys.loadDocumentTypes,
      invalidListMessage: ApiFallbackKeys.invalidResponse,
    );

    return _mapListItems(
      data,
      DriverDocumentTypeDto.fromJson,
    ).where((item) => item.id != 0).toList();
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
  Future<DriverModel?> createDriver({
    required String fullNameEn,
    required String fullNameAr,
    required String phone,
    required String nationalId,
    required List<DriverDocumentInput> documents,
  }) async {
    final formData = FormData();

    formData.fields.add(MapEntry('FullNameEn', fullNameEn));
    formData.fields.add(MapEntry('FullNameAr', fullNameAr));
    formData.fields.add(MapEntry('Phone', formatEgyptPhone(phone)));
    formData.fields.add(MapEntry('NationalId', nationalId));

    for (var i = 0; i < documents.length; i++) {
      final document = documents[i];
      formData.fields.add(
        MapEntry('Documents[$i].DocumentId', document.documentId.toString()),
      );
      formData.fields.add(
        MapEntry(
          'Documents[$i].ExpiryDate',
          document.expiryDate.toIso8601String().split('T').first,
        ),
      );
      formData.fields.add(
        MapEntry('Documents[$i].Status', document.status.toString()),
      );
      formData.files.add(
        MapEntry(
          'Documents[$i].File',
          await MultipartFile.fromFile(
            document.filePath,
            filename: p.basename(document.filePath),
          ),
        ),
      );
    }

    final envelope = await _postEnvelope(
      ApiEndpoints.drivers,
      data: formData,
      fallbackMessage: ApiFallbackKeys.createDriver,
    );

    final raw = envelope['data'] ?? envelope['Data'] ?? envelope;
    if (raw is num) {
      return DriverModel(
        id: raw.toInt(),
        fullNameEn: fullNameEn,
        fullNameAr: fullNameAr,
        nationalId: nationalId,
        phone: formatEgyptPhone(phone),
        status: 0,
      );
    }

    final map = _asMap(raw);
    if (map == null) return null;

    final dto = DriverDto.fromJson(map);
    if (dto.id == null) return null;

    return DriverModel(
      id: dto.id!,
      fullNameEn: dto.fullNameEn ?? fullNameEn,
      fullNameAr: dto.fullNameAr ?? fullNameAr,
      nationalId: dto.nationalId ?? nationalId,
      userName: dto.userName,
      email: dto.email,
      phone: dto.phone ?? formatEgyptPhone(phone),
      status: dto.status ?? 0,
      workingStatus: dto.workingStatus,
    );
  }

  @override
  Future<void> updateTripDriver({
    required int tripId,
    required ChangeDriverDto request,
  }) async {
    final formData = await request.toFormData();
    await _putSuccess(
      ApiEndpoints.updateTripDriver(tripId),
      data: formData,
      options: Options(
        sendTimeout: _uploadTimeout,
        receiveTimeout: _uploadTimeout,
      ),
      fallbackMessage: ApiFallbackKeys.updateTripDriver,
    );
  }
}
