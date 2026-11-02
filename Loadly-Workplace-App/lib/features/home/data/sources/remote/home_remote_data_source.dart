import 'package:dio/dio.dart';
import '../../../../../core/network/api_endpoints.dart';
import '../../models/collector_trips_dto.dart';
import '../../models/document_dto.dart';

abstract interface class HomeRemoteDataSource {
  Future<CollectorTripsListDto> getCollectorTrips({
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
  });
  Future<List<DocumentDto>> getDocuments();
}

class HomeRemoteDataSourceImpl implements HomeRemoteDataSource {
  HomeRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<CollectorTripsListDto> getCollectorTrips({
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
    final queryParams = <String, dynamic>{};
    if (page != null) queryParams['page'] = page;
    if (pageSize != null) queryParams['pageSize'] = pageSize;
    if (search != null) queryParams['search'] = search;
    if (contractId != null) queryParams['contractId'] = contractId;
    if (contractReferenceNumber != null) {
      queryParams['contractReferenceNumber'] = contractReferenceNumber;
    }
    if (companyId != null) queryParams['companyId'] = companyId;
    if (vehicleId != null) queryParams['vehicleId'] = vehicleId;
    if (vehicleTypeId != null) queryParams['vehicleTypeId'] = vehicleTypeId;
    if (driverId != null) queryParams['driverId'] = driverId;
    if (status != null) queryParams['status'] = status;
    if (tripDocumentStatuses != null) {
      queryParams['tripDocumentStatuses'] = tripDocumentStatuses;
    }
    if (startDate != null) queryParams['startDateFrom'] = startDate;
    if (endDate != null) queryParams['startDateTo'] = endDate;
    if (companyName != null) queryParams['companyName'] = companyName;
    if (driverName != null) queryParams['driverName'] = driverName;
    if (plate != null) queryParams['plate'] = plate;

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.collectorTrips,
      queryParameters: queryParams.isEmpty ? null : queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    final collectorTrips = dataList is List ? dataList : <dynamic>[];
    return CollectorTripsListDto(
      collectorTrips: collectorTrips
          .map((e) => CollectorTripsDto.fromJson(e as Map<String, dynamic>))
          .toList(),
      pagination: PaginationDto.fromJson(data['pagination']),
    );
  }

  @override
  Future<List<DocumentDto>> getDocuments() async {
    final queryParams = <String, dynamic>{};
    queryParams['pageSize'] = 100;
    queryParams['entityId'] = 5;

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.documents,
      queryParameters: queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    final documentsList = dataList is List ? dataList : <dynamic>[];
    return documentsList.map((e) => DocumentDto.fromJson(e as Map<String, dynamic>)).toList();
  }
}
