import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../features/notifications/presentation/bloc/notifications_bloc.dart';
import '../../features/notifications/presentation/bloc/notifications_event.dart';
import '../../features/notifications/presentation/bloc/notifications_state.dart';
import '../router/route_constants.dart';
import '../theme/app_colors.dart';
import 'app_blue_pattern_background.dart';

// class AppNavBar extends StatelessWidget
//     implements ObstructingPreferredSizeWidget {
//   final Widget? trailng;
//   const AppNavBar({super.key, this.trailng});

//   @override
//   Widget build(BuildContext context) {
//     return CupertinoNavigationBar(
//       backgroundColor: AppColors.mainBlue,
//       enableBackgroundFilterBlur: false,
//       leading: SvgPicture.asset('assets/images/logo.svg'),
//       trailing: Row(
//         mainAxisAlignment: MainAxisAlignment.end,
//         children: [
//           ?trailng,
//           CupertinoButton(
//             padding: EdgeInsets.zero,
//             onPressed: () {},
//             child: SvgPicture.asset('assets/images/notification.svg'),
//           ),
//         ],
//       ),
//     );
//   }

//   @override
//   Size get preferredSize => const Size.fromHeight(50);

//   @override
//   bool shouldFullyObstruct(BuildContext context) {
//     return true;
//   }
// }

class AppNavBar extends StatelessWidget
    implements ObstructingPreferredSizeWidget {
  const AppNavBar({super.key});

  static const _contentHeight = 50.0;

  @override
  Widget build(BuildContext context) {
    final topPadding = MediaQuery.paddingOf(context).top;
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

        return SizedBox(
          height: topPadding + _contentHeight,
          child: AppBluePatternBackground(
            child: Padding(
              padding: EdgeInsets.fromLTRB(10, topPadding, 10, 0),
              child: SizedBox(
                height: _contentHeight,
                child: Row(
                  children: [
                    SvgPicture.asset('assets/images/logo.svg'),
                    const Spacer(),
                    Stack(
                      clipBehavior: Clip.none,
                      children: [
                        CupertinoButton(
                          padding: EdgeInsets.zero,
                          onPressed: () =>
                              context.go(AppRoutePaths.notifications),
                          child: SvgPicture.asset(
                            'assets/images/notification.svg',
                          ),
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
                ),
              ),
            ),
          ),
        );
      },
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(_contentHeight);

  @override
  bool shouldFullyObstruct(BuildContext context) => true;

  static int _unreadCount(NotificationsState state) {
    if (state is! NotificationsLoaded) return 0;
    return state.notifications.where((n) => !n.isRead).length;
  }
}
