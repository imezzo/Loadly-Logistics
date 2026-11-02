import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/errors/api_fallback_keys.dart';
import '../../../../core/errors/failure_message_mapper.dart';
import '../../domain/entities/contractor_notification.dart';
import '../../domain/use_cases/get_notifications_usecase.dart';
import '../../domain/use_cases/mark_notification_read_usecase.dart';
import 'notifications_event.dart';
import 'notifications_state.dart';

class NotificationsBloc extends Bloc<NotificationsEvent, NotificationsState> {
  NotificationsBloc({
    required this.getNotificationsUseCase,
    required this.markNotificationReadUseCase,
  }) : super(const NotificationsInitial()) {
    on<NotificationsLoad>(_onLoad);
    on<NotificationsRefresh>(_onRefresh);
    on<NotificationsLoadMore>(_onLoadMore);
    on<NotificationsMarkAsRead>(_onMarkAsRead);
  }

  final GetNotificationsUseCase getNotificationsUseCase;
  final MarkNotificationReadUseCase markNotificationReadUseCase;

  Future<void> _onLoad(
    NotificationsLoad event,
    Emitter<NotificationsState> emit,
  ) async {
    emit(const NotificationsLoading());

    final result = await getNotificationsUseCase(
      GetNotificationsParams(
        page: event.page ?? 1,
        pageSize: event.pageSize ?? 15,
        startDateFrom: event.startDateFrom,
        startDateTo: event.startDateTo,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = FailureMessageMapper.forNotifications(
          failure,
          serverFallback: ApiFallbackKeys.loadNotifications,
        );
        emit(
          NotificationsLoaded(
            notifications: const [],
            error: errorMessage,
            startDateFrom: event.startDateFrom,
            startDateTo: event.startDateTo,
          ),
        );
      },
      (listResult) {
        emit(
          NotificationsLoaded(
            notifications: listResult.notifications,
            error: null,
            page: listResult.page,
            total: listResult.total,
            pageSize: listResult.pageSize,
            startDateFrom: event.startDateFrom,
            startDateTo: event.startDateTo,
          ),
        );
      },
    );
  }

  Future<void> _onRefresh(
    NotificationsRefresh event,
    Emitter<NotificationsState> emit,
  ) async {
    final currentState = state;
    final pageSize = currentState is NotificationsLoaded
        ? currentState.pageSize
        : 15;
    final startDateFrom = currentState is NotificationsLoaded
        ? currentState.startDateFrom
        : null;
    final startDateTo = currentState is NotificationsLoaded
        ? currentState.startDateTo
        : null;

    add(
      NotificationsLoad(
        page: 1,
        pageSize: pageSize,
        startDateFrom: startDateFrom,
        startDateTo: startDateTo,
      ),
    );
  }

  Future<void> _onLoadMore(
    NotificationsLoadMore event,
    Emitter<NotificationsState> emit,
  ) async {
    final current = state;
    if (current is! NotificationsLoaded ||
        !current.hasMore ||
        current.error != null) {
      return;
    }

    final result = await getNotificationsUseCase(
      GetNotificationsParams(
        page: current.page + 1,
        pageSize: current.pageSize,
        startDateFrom: current.startDateFrom,
        startDateTo: current.startDateTo,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = FailureMessageMapper.forNotifications(
          failure,
          serverFallback: ApiFallbackKeys.loadMore,
          networkFallback: ApiFallbackKeys.networkError,
        );
        emit(current.copyWith(error: errorMessage));
      },
      (listResult) {
        emit(
          current.copyWith(
            notifications: [
              ...current.notifications,
              ...listResult.notifications,
            ],
            page: listResult.page,
            total: listResult.total,
            error: null,
          ),
        );
      },
    );
  }

  Future<void> _onMarkAsRead(
    NotificationsMarkAsRead event,
    Emitter<NotificationsState> emit,
  ) async {
    final current = state;
    if (current is! NotificationsLoaded) return;

    final result = await markNotificationReadUseCase(event.notificationId);

    result.fold(
      (failure) {
        // Optionally show error - for now we keep the list as is
        final errorMessage = FailureMessageMapper.forNotifications(
          failure,
          serverFallback: ApiFallbackKeys.markAsRead,
          networkFallback: ApiFallbackKeys.markAsRead,
        );
        emit(current.copyWith(error: errorMessage));
      },
      (_) {
        // Update the notification in the list to show as read
        final updated = current.notifications.map((n) {
          if (n.id == event.notificationId) {
            return ContractorNotification(
              id: n.id,
              title: n.title,
              body: n.body,
              eventType: n.eventType,
              resourceType: n.resourceType,
              resourceId: n.resourceId,
              clickAction: n.clickAction,
              isRead: true,
              readAt: n.readAt,
              createdAt: n.createdAt,
              updatedAt: n.updatedAt,
            );
          }
          return n;
        }).toList();
        emit(current.copyWith(notifications: updated, error: null));
      },
    );
  }
}
