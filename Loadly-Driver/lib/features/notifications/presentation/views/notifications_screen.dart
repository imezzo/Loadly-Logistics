import 'dart:ui' as ui;

import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/utils/fcm_notification_logger.dart';
import '../../../../core/utils/notification_click_action.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/driver_notification.dart';
import '../bloc/notifications_bloc.dart';
import '../bloc/notifications_event.dart';
import '../bloc/notifications_state.dart';
import '../widgets/notification_attachment_dialog.dart';
import '../widgets/notification_attachment_preview.dart';

/// Notifications screen displaying a list of notifications from the API,
/// grouped by date, with pull-to-refresh and load more.
class NotificationsScreen extends StatefulWidget {
  const NotificationsScreen({super.key});

  @override
  State<NotificationsScreen> createState() => _NotificationsScreenState();
}

class _NotificationsScreenState extends State<NotificationsScreen> {
  String? _startDateFrom;
  String? _startDateTo;
  bool _attachmentDialogVisible = false;

  void _handleBackNavigation() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go('/home');
    }
  }

  void _loadNotifications() {
    context.read<NotificationsBloc>().add(
      NotificationsLoad(
        page: 1,
        pageSize: 15,
        startDateFrom: _startDateFrom,
        startDateTo: _startDateTo,
      ),
    );
  }

  @override
  void initState() {
    super.initState();
    _loadNotifications();
  }

  String _formatTimeAgo(DateTime timestamp, AppLocalizations l10n) {
    final now = DateTime.now();
    final difference = now.difference(timestamp);

    if (difference.inDays > 0) {
      return DateFormat('dd/MM/yyyy').format(timestamp);
    } else if (difference.inHours > 0) {
      return l10n.hoursAgo(difference.inHours);
    } else if (difference.inMinutes > 0) {
      return l10n.minsAgo(difference.inMinutes);
    } else {
      return 'الآن';
    }
  }

  bool _isToday(DateTime date) {
    final now = DateTime.now();
    return date.year == now.year &&
        date.month == now.month &&
        date.day == now.day;
  }

  bool _isSameDate(DateTime date1, DateTime date2) {
    return date1.year == date2.year &&
        date1.month == date2.month &&
        date1.day == date2.day;
  }

  Future<void> _onNotificationTap(DriverNotification notification) async {
    FcmNotificationLogger.logStep(
      step: 'in_app_notification_tap',
      source: 'notificationsScreen',
      data: {
        'id': notification.id,
        'clickAction': notification.clickAction,
        'resourceId': notification.resourceId,
        'hasAttachment': notification.hasSupportedAttachment,
      },
    );
    if (!notification.isRead) {
      context.read<NotificationsBloc>().add(
        NotificationsMarkAsRead(notificationId: notification.id),
      );
    }

    final tripRoute = NotificationClickAction.tripRouteFromNotification(
      clickAction: notification.clickAction,
      resourceId: notification.resourceId,
      resourceType: notification.resourceType,
      source: 'notificationsScreen',
    );
    if (tripRoute != null) {
      FcmNotificationLogger.logStep(
        step: 'in_app_navigate_start',
        source: 'notificationsScreen',
        route: tripRoute,
      );
      context.go(tripRoute);
    }
  }

  Future<void> _onAttachmentPreviewTap(DriverNotification notification) async {
    final supported = notification.firstSupportedAttachment;
    if (supported == null) return;

    if (!notification.isRead) {
      context.read<NotificationsBloc>().add(
        NotificationsMarkAsRead(notificationId: notification.id),
      );
    }

    if (_attachmentDialogVisible || !mounted) return;
    _attachmentDialogVisible = true;
    await showNotificationAttachmentDialog(
      context: context,
      title: notification.title ?? notification.body,
      body: notification.body,
      attachmentUrl: supported.url,
      mediaType: supported.type,
    );
    if (mounted) {
      _attachmentDialogVisible = false;
    }
  }

  List<_NotificationGroup> _groupNotifications(
    List<DriverNotification> notifications,
  ) {
    final groups = <_NotificationGroup>[];
    final today = DateTime.now();

    final todayNotifications = notifications
        .where((n) => _isToday(n.createdAt))
        .toList();

    if (todayNotifications.isNotEmpty) {
      groups.add(
        _NotificationGroup(
          date: today,
          isToday: true,
          notifications: todayNotifications,
        ),
      );
    }

    final otherNotifications =
        notifications.where((n) => !_isToday(n.createdAt)).toList()
          ..sort((a, b) => b.createdAt.compareTo(a.createdAt));

    DateTime? currentDate;
    List<DriverNotification>? currentGroup;

    for (final notification in otherNotifications) {
      if (currentDate == null ||
          !_isSameDate(currentDate, notification.createdAt)) {
        if (currentGroup != null && currentGroup.isNotEmpty) {
          groups.add(
            _NotificationGroup(
              date: currentDate!,
              isToday: false,
              notifications: currentGroup,
            ),
          );
        }
        currentDate = notification.createdAt;
        currentGroup = [notification];
      } else {
        currentGroup!.add(notification);
      }
    }

    if (currentGroup != null && currentGroup.isNotEmpty) {
      groups.add(
        _NotificationGroup(
          date: currentDate!,
          isToday: false,
          notifications: currentGroup,
        ),
      );
    }

    return groups;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) _handleBackNavigation();
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.mainBlue,
        navigationBar: CupertinoNavigationBar(
          backgroundColor: AppColors.mainBlue,
          brightness: Brightness.dark,
          leading: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              AppBackButton(onPressed: _handleBackNavigation),
              const SizedBox(width: 8),
              Text(
                l10n.notifications,
                style: const TextStyle(
                  color: AppColors.white,
                  fontWeight: FontWeight.w600,
                  fontSize: 17,
                ),
              ),
            ],
          ),
          // trailing: CupertinoButton(
          //   padding: EdgeInsets.zero,
          //   minimumSize: Size.zero,
          //   onPressed: () => _showDateFilter(context, l10n),
          //   child: const Icon(
          //     CupertinoIcons.calendar,
          //     color: AppColors.white,
          //     size: 24,
          //   ),
          // ),
        ),
        child: MainScaffold(
          selectedIndex: 1,
          child: ClipRRect(
            borderRadius: AppLayout.authContentBorderRadius,
            child: Container(
              color: AppColors.white,
              child: BlocBuilder<NotificationsBloc, NotificationsState>(
                builder: (context, state) {
                  if (state is NotificationsLoading) {
                    return const Center(
                      child: CupertinoActivityIndicator(
                        color: AppColors.mainBlue,
                        radius: 16,
                      ),
                    );
                  }

                  if (state is NotificationsLoaded) {
                    if (state.error != null && state.notifications.isEmpty) {
                      return Center(
                        child: Padding(
                          padding: const EdgeInsets.all(24.0),
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              const Icon(
                                CupertinoIcons.exclamationmark_triangle,
                                size: 64,
                                color: AppColors.red,
                              ),
                              const SizedBox(height: 16),
                              Text(
                                state.error!,
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: AppColors.darkGray,
                                ),
                                textAlign: TextAlign.center,
                              ),
                              const SizedBox(height: 24),
                              AppButton(
                                label: l10n.retry,
                                onPressed: _loadNotifications,
                              ),
                            ],
                          ),
                        ),
                      );
                    }

                    final notificationGroups = _groupNotifications(
                      state.notifications,
                    );

                    if (notificationGroups.isEmpty) {
                      return Center(
                        child: Text(
                          l10n.noNotifications,
                          style: const TextStyle(
                            color: AppColors.darkGray,
                            fontSize: 16,
                          ),
                        ),
                      );
                    }

                    return CustomScrollView(
                      slivers: [
                        CupertinoSliverRefreshControl(
                          onRefresh: () async {
                            context.read<NotificationsBloc>().add(
                              const NotificationsRefresh(),
                            );
                            await Future.delayed(
                              const Duration(milliseconds: 500),
                            );
                          },
                        ),
                        SliverPadding(
                          padding: const EdgeInsets.symmetric(vertical: 16),
                          sliver: SliverList(
                            delegate: SliverChildBuilderDelegate((
                              context,
                              groupIndex,
                            ) {
                              final group = notificationGroups[groupIndex];
                              return Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Padding(
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 16,
                                      vertical: 12,
                                    ),
                                    child: Text(
                                      group.isToday
                                          ? l10n.todayNotifications
                                          : DateFormat(
                                              'dd/MM/yyyy',
                                            ).format(group.date),
                                      style: const TextStyle(
                                        fontSize: 16,
                                        fontWeight: FontWeight.w600,
                                        color: AppColors.darkGray,
                                      ),
                                      textAlign: TextAlign.center,
                                    ),
                                  ),
                                  ...group.notifications.map((notification) {
                                    return Padding(
                                      padding: const EdgeInsets.symmetric(
                                        horizontal: 16,
                                        vertical: 6,
                                      ),
                                      child: _NotificationCard(
                                        notification: notification,
                                        formatTimeAgo: (timestamp) =>
                                            _formatTimeAgo(timestamp, l10n),
                                        onTap: () =>
                                            _onNotificationTap(notification),
                                        onAttachmentPreviewTap: () =>
                                            _onAttachmentPreviewTap(
                                              notification,
                                            ),
                                      ),
                                    );
                                  }),
                                ],
                              );
                            }, childCount: notificationGroups.length),
                          ),
                        ),
                        if (state.hasMore)
                          SliverToBoxAdapter(
                            child: Padding(
                              padding: const EdgeInsets.all(16),
                              child: CupertinoButton(
                                onPressed: () {
                                  context.read<NotificationsBloc>().add(
                                    const NotificationsLoadMore(),
                                  );
                                },
                                child: Text(l10n.loadMore),
                              ),
                            ),
                          ),
                      ],
                    );
                  }

                  return const Center(
                    child: CupertinoActivityIndicator(
                      color: AppColors.mainBlue,
                      radius: 16,
                    ),
                  );
                },
              ),
            ),
          ),
        ),
      ),
    );
  }

  // ignore: unused_element
  Future<void> _showDateFilter(
    BuildContext context,
    AppLocalizations l10n,
  ) async {
    DateTime? from = _startDateFrom != null
        ? DateTime.tryParse(_startDateFrom!)
        : null;
    DateTime? to = _startDateTo != null
        ? DateTime.tryParse(_startDateTo!)
        : null;

    final picked =
        await showCupertinoModalPopup<({DateTime from, DateTime to})>(
          context: context,
          builder: (ctx) => _DateFilterSheet(from: from, to: to, l10n: l10n),
        );
    if (picked != null && mounted) {
      setState(() {
        _startDateFrom = picked.from.toIso8601String().substring(0, 10);
        _startDateTo = picked.to.toIso8601String().substring(0, 10);
      });
      _loadNotifications();
    }
  }
}

class _NotificationGroup {
  _NotificationGroup({
    required this.date,
    required this.isToday,
    required this.notifications,
  });

  final DateTime date;
  final bool isToday;
  final List<DriverNotification> notifications;
}

class _NotificationCard extends StatelessWidget {
  const _NotificationCard({
    required this.notification,
    required this.formatTimeAgo,
    this.onTap,
    this.onAttachmentPreviewTap,
  });

  final DriverNotification notification;
  final String Function(DateTime) formatTimeAgo;
  final VoidCallback? onTap;
  final VoidCallback? onAttachmentPreviewTap;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == ui.TextDirection.rtl;

    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: notification.isRead
              ? AppColors.white
              : AppColors.lightMint.withValues(alpha: 0.3),
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: AppColors.lightGray, width: 1),
          boxShadow: [
            BoxShadow(
              color: AppColors.lightGray.withValues(alpha: 0.2),
              blurRadius: 4,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              textDirection: isRTL
                  ? ui.TextDirection.rtl
                  : ui.TextDirection.ltr,
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  formatTimeAgo(notification.createdAt),
                  style: const TextStyle(
                    fontSize: 12,
                    color: AppColors.mediumBlueGray,
                  ),
                ),
                Row(
                  textDirection: isRTL
                      ? ui.TextDirection.rtl
                      : ui.TextDirection.ltr,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(
                      CupertinoIcons.bell_circle_fill,
                      size: 18,
                      color: AppColors.mainBlue,
                    ),
                    const SizedBox(width: 6),
                    Text(
                      l10n.companyNotification,
                      style: const TextStyle(
                        fontSize: 12,
                        color: AppColors.darkGray,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: 12),
            Builder(
              builder: (context) {
                final attachment = notification.firstSupportedAttachment;
                final showPreview =
                    attachment != null && onAttachmentPreviewTap != null;
                final textBlock = Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        notification.title ?? notification.body,
                        style: const TextStyle(
                          fontSize: 14,
                          color: AppColors.darkGray,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      if (notification.title != null &&
                          notification.title != notification.body) ...[
                        const SizedBox(height: 4),
                        Text(
                          notification.body,
                          style: const TextStyle(
                            fontSize: 14,
                            color: AppColors.darkGray,
                            fontWeight: FontWeight.w400,
                          ),
                        ),
                      ],
                    ],
                  ),
                );
                final preview = showPreview
                    ? NotificationAttachmentPreview(
                        attachment: attachment,
                        onTap: onAttachmentPreviewTap!,
                      )
                    : null;

                return Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    if (!isRTL && preview != null) ...[
                      preview,
                      const SizedBox(width: 10),
                    ],
                    textBlock,
                    if (isRTL && preview != null) ...[
                      const SizedBox(width: 10),
                      preview,
                    ],
                  ],
                );
              },
            ),
          ],
        ),
      ),
    );
  }
}

class _DateFilterSheet extends StatefulWidget {
  const _DateFilterSheet({this.from, this.to, required this.l10n});

  final DateTime? from;
  final DateTime? to;
  final AppLocalizations l10n;

  @override
  State<_DateFilterSheet> createState() => _DateFilterSheetState();
}

class _DateFilterSheetState extends State<_DateFilterSheet> {
  late DateTime _from;
  late DateTime _to;

  @override
  void initState() {
    super.initState();
    final now = DateTime.now();
    _from = widget.from ?? now.subtract(const Duration(days: 30));
    _to = widget.to ?? now;
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 320,
      padding: const EdgeInsets.all(16),
      decoration: const BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            widget.l10n.filterByDate,
            style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(widget.l10n.fromDate),
                    SizedBox(
                      height: 120,
                      child: CupertinoDatePicker(
                        mode: CupertinoDatePickerMode.date,
                        initialDateTime: _from,
                        minimumDate: DateTime(2020),
                        maximumDate: _to,
                        onDateTimeChanged: (d) => setState(() {
                          _from = d;
                          if (_to.isBefore(_from)) _to = _from;
                        }),
                      ),
                    ),
                  ],
                ),
              ),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(widget.l10n.toDate),
                    SizedBox(
                      height: 120,
                      child: CupertinoDatePicker(
                        mode: CupertinoDatePickerMode.date,
                        initialDateTime: _to,
                        minimumDate: _from,
                        maximumDate: DateTime.now(),
                        onDateTimeChanged: (d) => setState(() {
                          _to = d;
                          if (_from.isAfter(_to)) _from = _to;
                        }),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const Spacer(),
          AppButton(
            label: widget.l10n.applyFilter,
            onPressed: () => Navigator.of(context).pop((from: _from, to: _to)),
          ),
        ],
      ),
    );
  }
}
