import 'dart:async';

import 'package:chucker_flutter/chucker_flutter.dart';
import '../../features/notifications/presentation/views/notifications_screen.dart';
import '../../features/profile/presentation/views/profile_screen.dart';
import '../../features/requests/presentation/views/requests_screen.dart';
import '../../features/requests/presentation/views/transfers_list_type.dart';
import '../../features/settings/presentation/views/settings_screen.dart';
import '../../features/trips/domain/entities/booking_request/booking_request.dart';
import '../../features/trips/presentation/views/completed_trips_due_screen.dart';
import '../../features/trips/presentation/views/trips_financial_due_screen.dart';
import 'route_constants.dart';
import '../../features/auth/presentation/views/register_screen.dart';
import '../../features/home/presentation/views/home_screen.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/views/login_screen.dart';
import '../../features/auth/presentation/views/mfa_channel_screen.dart';
import '../../features/auth/presentation/views/onboarding_screen.dart';
import '../../features/auth/presentation/views/otp_screen.dart';
import '../../features/auth/presentation/views/password_login_screen.dart';
import '../../features/auth/presentation/views/change_password_screen.dart';
import '../permissions/prominent_user_data_disclosure.dart';
import '../services/app_update_service.dart';
import '../services/fcm_service.dart';
import '../storage/auth_storage.dart';
import '../widgets/app_update_presenter.dart';
import 'app_router_holder.dart';
import '../../features/auth/presentation/views/first_choose_screen.dart';
import '../../features/drivers/presentation/views/add_driver_screen.dart';
import '../../features/trips/presentation/views/edit_booking_request_screen.dart';
import '../../features/trips/presentation/views/my_trips_screen.dart';
import '../../features/trips/presentation/views/trip_details_screen.dart';
import '../../features/marketplce/presentation/views/marketplace.dart';
import '../../l10n/gen/app_localizations.dart';

class _SplashScreen extends StatefulWidget {
  const _SplashScreen();

  @override
  State<_SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<_SplashScreen>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _logoScale;
  late Animation<double> _textOpacity;
  late Animation<Offset> _textSlide;
  late Animation<Color?> _backgroundColor;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      unawaited(_startSplashFlow());
    });
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 2500),
    );

    // Background Animation
    _backgroundColor =
        ColorTween(begin: Colors.white, end: const Color(0xff0F766E)).animate(
          CurvedAnimation(
            parent: _controller,
            curve: const Interval(.15, .85, curve: Curves.easeInOut),
          ),
        );

    // Logo Scale
    _logoScale = Tween<double>(begin: 1.5, end: .65).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(.45, 1, curve: Curves.easeInOutBack),
      ),
    );

    // Name Fade
    _textOpacity = Tween<double>(begin: 0, end: 1).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(.6, .9, curve: Curves.easeIn),
      ),
    );

    // Name Slide
    _textSlide =
        Tween<Offset>(
          begin: const Offset(0, .8),
          end: const Offset(0, -1),
        ).animate(
          CurvedAnimation(
            parent: _controller,
            curve: const Interval(.7, 1, curve: Curves.easeOut),
          ),
        );

    _controller.forward();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _startSplashFlow() async {
    final updateFuture = AppUpdateService.instance?.checkForUpdate();
    final permissionFuture = FCMService.instance.resolvePermission(
      canPrompt: () => mounted,
      showDisclosure: () async {
        if (!mounted) return false;
        final l10n = AppLocalizations.of(context);
        if (l10n == null) return false;
        return showProminentUserDataDisclosure(
          context,
          title: l10n.notificationPermissionDisclosureTitle,
          body: l10n.notificationPermissionDisclosureBody,
        );
      },
    );
    await Future.wait<void>([
      Future<void>.delayed(const Duration(milliseconds: 3500)),
      permissionFuture,
      if (updateFuture != null) updateFuture.then((_) {}),
    ]);

    if (!mounted) return;

    final updateInfo = updateFuture != null ? await updateFuture : null;
    if (!mounted) return;

    if (updateInfo?.isForced == true) {
      await showAppUpdate(context: context, info: updateInfo!);
      return;
    }

    final destination = AuthStorage.instance.isLoggedIn()
        ? AppRoutePaths.home
        : AppRoutePaths.onboarding;
    AppRouterHolder.instance.router?.go(destination);
    // Session check can emit before this screen subscribes, so read the same
    // storage flag the router redirect uses before draining a pending tap.
    FCMService.instance.setAuthenticated(AuthStorage.instance.isLoggedIn());
    FCMService.instance.markNavigationReady();

    if (updateInfo != null) {
      WidgetsBinding.instance.addPostFrameCallback((_) async {
        final rootContext = ChuckerFlutter.navigatorKey.currentContext;
        if (rootContext != null && rootContext.mounted) {
          await showAppUpdate(context: rootContext, info: updateInfo);
        }
      });
    }
  }

  Widget ripple(double delay) {
    return AnimatedBuilder(
      animation: _controller,
      builder: (_, _) {
        double value = (_controller.value - delay).clamp(0.0, 1.0);

        return IgnorePointer(
          child: Container(
            width: 700 * value,
            height: 700 * value,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: Colors.white.withValues(alpha: 0.12 * (1 - value)),
            ),
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: _controller,
      builder: (context, child) {
        return Scaffold(
          backgroundColor: _backgroundColor.value,
          body: Stack(
            alignment: Alignment.center,
            children: [ripple(0.0), ripple(0.18), ripple(0.36), child!],
          ),
        );
      },
      child: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            AnimatedBuilder(
              animation: _controller,
              builder: (context, child) {
                return Container(
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    boxShadow: [
                      BoxShadow(
                        color: Colors.white.withValues(
                          alpha: 0.6 * (1 - _controller.value),
                        ),
                        blurRadius: 60,
                        spreadRadius: 15,
                      ),
                      BoxShadow(
                        color: Colors.blue.withValues(
                          alpha: 0.45 * (1 - _controller.value),
                        ),
                        blurRadius: 120,
                        spreadRadius: 35,
                      ),
                    ],
                  ),
                  child: child,
                );
              },
              child: ScaleTransition(
                scale: _logoScale,
                child: Image.asset("assets/images/logo.png", width: 180),
              ),
            ),

            FadeTransition(
              opacity: _textOpacity,
              child: SlideTransition(
                position: _textSlide,
                child: Image.asset("assets/images/loadly_img.png", width: 90),
              ),
            ),
            const SizedBox(height: 30),
          ],
        ),
      ),
    );
  }
}

GoRouter createAppRouter() {
  final router = GoRouter(
    initialLocation: '/',
    navigatorKey: ChuckerFlutter.navigatorKey,
    redirect: (context, state) {
      final isLoggedIn = AuthStorage.instance.isLoggedIn();
      final isAuthRoute =
          state.uri.path == AppRoutePaths.onboarding ||
          state.uri.path == AppRoutePaths.firstChoose ||
          state.uri.path == AppRoutePaths.register ||
          state.uri.path == AppRoutePaths.login ||
          state.uri.path == AppRoutePaths.passwordLogin ||
          state.uri.path == AppRoutePaths.otp ||
          state.uri.path == AppRoutePaths.mfa ||
          state.uri.path == AppRoutePaths.splash;

      // If logged in and trying to access auth routes, redirect to home
      if (isLoggedIn && isAuthRoute && state.uri.path != AppRoutePaths.splash) {
        return AppRoutePaths.home;
      }

      // If not logged in and trying to access protected routes, redirect to login
      if (!isLoggedIn && !isAuthRoute) {
        return AppRoutePaths.login;
      }

      return null; // No redirect needed
    },
    errorBuilder: (context, state) => _ErrorScreen(state.error?.toString()),
    routes: [
      GoRoute(
        path: AppRoutePaths.splash,
        builder: (context, state) => const _SplashScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.login,
        builder: (context, state) => const LoginScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.passwordLogin,
        builder: (context, state) => const PasswordLoginScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.firstChoose,
        builder: (context, state) => const FirstChooseScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.register,
        builder: (context, state) => const RegisterScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.mfa,
        builder: (context, state) {
          final email = state.uri.queryParameters['email'] ?? '';
          final phone = state.uri.queryParameters['phone'] ?? '';
          return MfaChannelScreen(email: email, phone: phone);
        },
      ),
      GoRoute(
        path: AppRoutePaths.addDriver,
        builder: (context, state) => const AddDriverScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.editBookingRequest,
        builder: (context, state) {
          final booking = state.extra as BookingRequest?;
          if (booking == null) {
            return const SizedBox.shrink();
          }
          return EditBookingRequestScreen(booking: booking);
        },
      ),
      GoRoute(
        path: AppRoutePaths.otp,
        builder: (context, state) {
          final contact = state.uri.queryParameters['contact'] ?? '';
          final viaSms = state.uri.queryParameters['viaSms'] != 'false';
          final forgetPassword =
              state.uri.queryParameters['forgetPassword'] == 'true';
          final isMfa = state.uri.queryParameters['isMfa'] == 'true';
          return OtpScreen(
            contact: contact,
            viaSms: viaSms,
            isForgetPassword: forgetPassword,
            isMfa: isMfa,
          );
        },
      ),
      // GoRoute(
      //   path: AppRoutePaths.onboarding,
      //   builder: (context, state) => const OnBoardingScreen(),
      // ),
      GoRoute(
        path: AppRoutePaths.onboarding,
        pageBuilder: (context, state) {
          return CustomTransitionPage(
            key: state.pageKey,
            child: const OnBoardingScreen(),
            transitionsBuilder:
                (context, animation, secondaryAnimation, child) {
                  return FadeTransition(opacity: animation, child: child);
                },
            transitionDuration: const Duration(milliseconds: 1200),
          );
        },
      ),

      GoRoute(
        path: AppRoutePaths.changePassword,
        builder: (context, state) => const ChangePasswordScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.home,
        builder: (context, state) => const HomeScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.trips,
        builder: (context, state) => const MyTripsScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.market,
        builder: (context, state) => const MarketplaceScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.tripDetails,
        builder: (context, state) {
          final tripId = int.parse(state.pathParameters['tripId']!);
          final returnRoute = state.extra as String?;
          return TripDetailsScreen(tripId: tripId, returnTo: returnRoute);
        },
      ),
      GoRoute(
        path: AppRoutePaths.settings,
        builder: (context, state) => const SettingsScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.profile,
        builder: (context, state) => const ProfileScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.notifications,
        builder: (context, state) => const NotificationsScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.completedTripsDue,
        builder: (context, state) => const CompletedTripsDueScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.tripsFinancialDue,
        builder: (context, state) => const TripsFinancialDueScreen(),
      ),
      GoRoute(
        path: AppRoutePaths.requests,
        builder: (context, state) {
          final listType = TransfersListType.fromQuery(
            state.uri.queryParameters[AppRoutePaths.requestsTypeParam],
          );
          return RequestsScreen(key: ValueKey(listType), listType: listType);
        },
      ),
    ],
  );

  // Store router instance globally for access from interceptors
  AppRouterHolder.instance.setRouter(router);

  return router;
}

class _ErrorScreen extends StatelessWidget {
  const _ErrorScreen(this.message);

  final String? message;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Scaffold(
      appBar: AppBar(title: Text(l10n.routeError)),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Text(message ?? l10n.unknownError),
        ),
      ),
    );
  }
}
