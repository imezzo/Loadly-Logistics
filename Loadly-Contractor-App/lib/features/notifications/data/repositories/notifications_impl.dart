import 'package:dartz/dartz.dart';

import '../../../../core/errors/api_fallback_keys.dart';
import '../../../../core/network/api_failure_mapper.dart';
import '../../../../core/network/safe_api_call.dart';
import '../../domain/entities/contractor_notification.dart';
import '../../domain/entities/notifications_list_result.dart';
import '../../domain/failures/notifications_failure.dart';
import '../../domain/repositories/notifications_repository.dart';
import '../models/notification_dto.dart';
import '../sources/remote/notifications_remote_data_source.dart';

class NotificationsImpl implements NotificationsRepository {
  NotificationsImpl(this._remote);

  final NotificationsRemoteDataSource _remote;

  Future<Either<NotificationsFailure, T>> _run<T>(
    Future<T> Function() request, {
    String? unknownMessage,
  }) {
    return safeApiCall<T, NotificationsFailure>(
      request: request,
      mapFailure: (details) {
        return switch (details.kind) {
          ApiFailureKind.network => const NotificationsFailureNetwork(
            message: ApiFallbackKeys.networkError,
          ),
          ApiFailureKind.server ||
          ApiFailureKind.unauthorized => NotificationsFailureServer(
            message: details.message,
            statusCode: details.statusCode,
          ),
          ApiFailureKind.unknown => NotificationsFailureServer(
            message: details.message ?? unknownMessage,
          ),
        };
      },
      mapUnknownFailure: (_) => NotificationsFailureServer(
        message: unknownMessage ?? ApiFallbackKeys.unexpectedError,
      ),
    );
  }

  ContractorNotification _mapDtoToEntity(NotificationDto dto) {
    return ContractorNotification(
      id: dto.id,
      title: dto.title,
      body: dto.body,
      eventType: dto.eventType,
      resourceType: dto.resourceType,
      resourceId: dto.resourceId,
      clickAction: dto.clickAction,
      isRead: dto.isRead ?? false,
      readAt: dto.readAt != null
          ? DateTime.tryParse(dto.readAt!)?.toLocal()
          : null,
      createdAt: dto.createdAt != null
          ? DateTime.tryParse(dto.createdAt!)?.toLocal() ?? DateTime.now()
          : DateTime.now(),
      updatedAt: dto.updatedAt != null
          ? DateTime.tryParse(dto.updatedAt!)?.toLocal()
          : null,
    );
  }

  @override
  Future<Either<NotificationsFailure, NotificationsListResult>>
  getNotifications({
    int? page,
    int? pageSize,
    String? startDateFrom,
    String? startDateTo,
  }) async {
    return _run(() async {
      final response = await _remote.getNotifications(
        page: page,
        pageSize: pageSize,
        startDateFrom: startDateFrom,
        startDateTo: startDateTo,
      );
      final notifications = response.notifications
          .map(_mapDtoToEntity)
          .toList();
      return NotificationsListResult(
        notifications: notifications,
        total: response.total,
        page: response.page,
        pageSize: response.pageSize,
      );
    }, unknownMessage: ApiFallbackKeys.unexpectedError);
  }

  @override
  Future<Either<NotificationsFailure, void>> markAsRead(
    int notificationId,
  ) async {
    return _run(() async {
      await _remote.markAsRead(notificationId);
    }, unknownMessage: ApiFallbackKeys.markAsRead);
  }
}
