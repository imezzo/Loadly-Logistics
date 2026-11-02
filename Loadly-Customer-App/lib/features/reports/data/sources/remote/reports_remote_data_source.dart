import 'package:dio/dio.dart';

import '../../../../../core/network/api_endpoints.dart';
import '../../models/get_invoice_statistics_response_dto.dart';
import '../../models/get_trip_cost_period_response_dto.dart';
import '../../models/invoice_dto.dart';
import '../../models/invoice_payment_request_dto.dart' as req;

abstract interface class ReportsRemoteDataSource {
  Future<GetTripCostPeriodResponseDto> getTripCostPeriod();
  Future<GetInvoiceStatisticsResponseDto> getInvoiceStatistics();
  Future<InvoiceListDto> getInvoices({
    int? page,
    int? pageSize,
    String? search,
    int? contractId,
    int? status,
    int? invoiceType,
    String? startDate,
    String? endDate,
  });
  Future<req.InvoicePaymentRequestListDto> getInvoicesPaymentRequests({
    int? page,
    int? pageSize,
    int? invoiceId,
    int? status,
    String? startDate,
    String? endDate,
    String? search,
  });
}

class ReportsRemoteDataSourceImpl implements ReportsRemoteDataSource {
  ReportsRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<GetTripCostPeriodResponseDto> getTripCostPeriod() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.tripCostPeriod,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    return GetTripCostPeriodResponseDto.fromJson(data['data']);
  }

  @override
  Future<GetInvoiceStatisticsResponseDto> getInvoiceStatistics() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.invoiceStatistics,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    return GetInvoiceStatisticsResponseDto.fromJson(data['data']);
  }

  @override
  Future<InvoiceListDto> getInvoices({
    int? page,
    int? pageSize,
    String? search,
    int? contractId,
    int? status,
    int? invoiceType,
    String? startDate,
    String? endDate,
  }) async {
    final queryParams = <String, dynamic>{};
    if (page != null) queryParams['page'] = page;
    if (pageSize != null) queryParams['pageSize'] = pageSize;
    if (search != null) queryParams['search'] = search;
    if (contractId != null) queryParams['contractId'] = contractId;
    if (status != null) queryParams['status'] = status;
    if (invoiceType != null) queryParams['invoiceType'] = invoiceType;
    if (startDate != null) queryParams['startDateFrom'] = startDate;
    if (endDate != null) queryParams['startDateTo'] = endDate;

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.invoices,
      queryParameters: queryParams.isEmpty ? null : queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    final invoicesList = dataList is List ? dataList : <dynamic>[];
    return InvoiceListDto(
      invoices: invoicesList
          .map((e) => InvoiceDto.fromJson(e as Map<String, dynamic>))
          .toList(),
      pagination: PaginationDto.fromJson(data['pagination']),
    );
  }

  @override
  Future<req.InvoicePaymentRequestListDto> getInvoicesPaymentRequests({
    int? page,
    int? pageSize,
    int? invoiceId,
    int? status,
    String? startDate,
    String? endDate,
    String? search,
  }) async {
    final queryParams = <String, dynamic>{};
    if (page != null) queryParams['page'] = page;
    if (pageSize != null) queryParams['pageSize'] = pageSize;
    if (invoiceId != null) queryParams['invoiceId'] = invoiceId;
    if (status != null) queryParams['status'] = status;
    if (startDate != null) queryParams['requestDateFrom'] = startDate;
    if (endDate != null) queryParams['requestDateTo'] = endDate;
    if (search != null) queryParams['search'] = search;

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.invoicesPaymentRequest,
      queryParameters: queryParams.isEmpty ? null : queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    // API returns: { data: [...], pagination: { totalCount, page, pageSize, totalPages } }

    final dataList = data['data'];
    final invoicesList = dataList is List ? dataList : <dynamic>[];
    return req.InvoicePaymentRequestListDto(
      invoicePaymentRequests: invoicesList
          .map(
            (e) => req.InvoicePaymentRequestDto.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
      pagination: req.PaginationDto.fromJson(data['pagination']),
    );
  }
}
