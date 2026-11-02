import 'package:dio/dio.dart';

import '../../../../../core/errors/api_fallback_keys.dart';
import '../../../../../core/network/api_client.dart';
import '../../../../../core/network/api_endpoints.dart';
import '../../../../../core/network/api_response_parser.dart';
import '../models/lookup_item_dto.dart';

abstract interface class LookupsRemoteDataSource {
  Future<List<LookupItemDto>> getDefaultVehicleTypes({
    int page = 1,
    int limit = 20,
  });
}

class LookupsRemoteDataSourceImpl implements LookupsRemoteDataSource {
  LookupsRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<List<LookupItemDto>> getDefaultVehicleTypes({
    int page = 1,
    int limit = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '$kApiBaseUrl${ApiEndpoints.vehicleTypesDefault}',
      queryParameters: {
        'page': page,
        'limit': limit,
      },
    );

    final data = requireEnvelopeList(
      response,
      fallbackMessage: ApiFallbackKeys.loadVehicleTypes,
      invalidListMessage: ApiFallbackKeys.invalidResponse,
    );

    final items = <LookupItemDto>[];
    for (final item in data) {
      if (item is Map) {
        items.add(
          LookupItemDto.fromJson(Map<String, dynamic>.from(item)),
        );
      }
    }
    return items;
  }
}