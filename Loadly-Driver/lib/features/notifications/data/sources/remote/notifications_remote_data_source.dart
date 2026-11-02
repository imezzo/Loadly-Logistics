import 'package:dio/dio.dart';

import '../../../../../core/network/api_endpoints.dart';
import '../../models/notification_dto.dart';

abstract interface class NotificationsRemoteDataSource {
  Future<NotificationsListResponseDto> getNotifications({
    int? page,
    int? pageSize,
    String? startDateFrom,
    String? startDateTo,
  });
  Future<void> markAsRead(int notificationId);
}

class NotificationsRemoteDataSourceImpl
    implements NotificationsRemoteDataSource {
  NotificationsRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<NotificationsListResponseDto> getNotifications({
    int? page,
    int? pageSize,
    String? startDateFrom,
    String? startDateTo,
  }) async {
    final queryParams = <String, dynamic>{
      'Page': page ?? 1,
      'PageSize': pageSize ?? 15,
      'StartDateFrom': ?startDateFrom,
      'StartDateTo': ?startDateTo,
    };

    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.notifications,
      queryParameters: queryParams.isEmpty ? null : queryParams,
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }

    final dataList = data['data'];
    final notificationsList = dataList is List ? dataList : <dynamic>[];
    final pagination = data['pagination'] as Map<String, dynamic>?;

    final notifications = notificationsList
        .map((e) => NotificationDto.fromJson(e as Map<String, dynamic>))
        .toList();

    final total = pagination?['totalCount'] ?? pagination?['TotalCount'];
    final pageVal = pagination?['page'] ?? pagination?['Page'];
    final pageSizeVal = pagination?['pageSize'] ?? pagination?['PageSize'];

    return NotificationsListResponseDto(
      notifications: notifications,
      total: (total as num?)?.toInt(),
      page: (pageVal as num?)?.toInt() ?? 1,
      pageSize: (pageSizeVal as num?)?.toInt() ?? 15,
    );
  }

  @override
  Future<void> markAsRead(int notificationId) async {
    await _dio.patch<void>(ApiEndpoints.notificationRead(notificationId));
  }
}
