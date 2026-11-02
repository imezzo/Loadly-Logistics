import 'package:flutter/cupertino.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';

import '../../features/notifications/presentation/bloc/notifications_bloc.dart';
import '../../features/notifications/presentation/bloc/notifications_event.dart';
import '../../features/notifications/presentation/bloc/notifications_state.dart';
import '../router/route_constants.dart';
import '../theme/app_colors.dart';
import 'app_blue_pattern_background.dart';

/// Unified main-tab header: status bar, logo, notifications, and patterned blue body.
class AppMainTabHeader extends StatelessWidget {
  const AppMainTabHeader({
    super.key,
    required this.bodyHeight,
    this.color = AppColors.mainBlue,
    this.alignLogoRowWithTitleBand = false,
    this.centerTitle,
    this.titleBandRowLift = 0,
  });

  /// Patterned blue area below the logo row (same as former [AppHeaderBackground] height).
  final double bodyHeight;
  final Color color;

  /// Places logo and notifications in [bodyHeight] band (aligned with [centerTitle]).
  final bool alignLogoRowWithTitleBand;

  /// Shown centered between logo and notifications when [alignLogoRowWithTitleBand] is true.
  final Widget? centerTitle;

  /// Moves the unified title row upward (px) when [alignLogoRowWithTitleBand] is true.
  final double titleBandRowLift;

  static const logoRowHeight = 50.0;

  static double chromeHeight(BuildContext context) {
    return MediaQuery.paddingOf(context).top + logoRowHeight;
  }

  static double totalHeight(BuildContext context, double bodyHeight) {
    return chromeHeight(context) + bodyHeight;
  }

  /// Padding for content stacked on the header (e.g. cards on Home).
  static EdgeInsets overlayContentPadding(
    BuildContext context, {
    double inset = 15,
  }) {
    return EdgeInsets.fromLTRB(
      inset,
      chromeHeight(context) + inset,
      inset,
      inset,
    );
  }

  @override
  Widget build(BuildContext context) {
    final topPadding = MediaQuery.paddingOf(context).top;
    final height = topPadding + logoRowHeight + bodyHeight;

    return AnnotatedRegion<SystemUiOverlayStyle>(
      value: SystemUiOverlayStyle.light.copyWith(
        statusBarColor: CupertinoColors.transparent,
      ),
      child: SizedBox(
        height: height,
        width: double.infinity,
        child: Stack(
          fit: StackFit.expand,
          children: [
            AppBluePatternFill(color: color),
            if (alignLogoRowWithTitleBand)
              Positioned(
                left: 10,
                right: 10,
                top: topPadding + logoRowHeight - titleBandRowLift,
                height: bodyHeight,
                child: _HeaderLogoRow(center: centerTitle),
              )
            else
              Padding(
                padding: EdgeInsets.fromLTRB(10, topPadding, 10, bodyHeight),
                child: const SizedBox(
                  height: logoRowHeight,
                  child: _HeaderLogoRow(),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

class _HeaderLogoRow extends StatelessWidget {
  const _HeaderLogoRow({this.center});

  final Widget? center;

  @override
  Widget build(BuildContext context) {
    final notificationsBloc = context.read<NotificationsBloc>();

    return BlocBuilder<NotificationsBloc, NotificationsState>(
      bloc: notificationsBloc,
      buildWhen: (prev, curr) =>
          prev is! NotificationsLoaded ||
          curr is! NotificationsLoaded ||
          _unreadCount(prev) != _unreadCount(curr),
      builder: (context, notificationsState) {
        if (notificationsState is NotificationsInitial) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            if (context.mounted) {
              context.read<NotificationsBloc>().add(
                const NotificationsLoad(page: 1, pageSize: 15),
              );
            }
          });
        }
        final unreadCount = _unreadCount(notificationsState);

        return Row(
          children: [
            SvgPicture.asset('assets/images/logo.svg'),
            if (center != null)
              Expanded(child: Center(child: center))
            else
              const Spacer(),
            Stack(
              clipBehavior: Clip.none,
              children: [
                CupertinoButton(
                  padding: EdgeInsets.zero,
                  onPressed: () => context.go(AppRoutePaths.notifications),
                  child: SvgPicture.asset('assets/images/notification.svg'),
                ),
                if (unreadCount > 0)
                  Positioned(
                    top: 0,
                    right: 0,
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 5,
                        vertical: 2,
                      ),
                      constraints: const BoxConstraints(minWidth: 16),
                      decoration: BoxDecoration(
                        color: AppColors.red,
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: Text(
                        unreadCount > 99 ? '99+' : '$unreadCount',
                        style: const TextStyle(
                          color: AppColors.white,
                          fontSize: 10,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ),
              ],
            ),
          ],
        );
      },
    );
  }

  static int _unreadCount(NotificationsState state) {
    if (state is! NotificationsLoaded) return 0;
    return state.notifications.where((n) => !n.isRead).length;
  }
}
