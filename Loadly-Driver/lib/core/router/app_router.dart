import 'dart:async';

import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';
import '../../features/trips/presentation/widgets/trip_customs.dart';

import '../../features/auth/presentation/views/login_screen.dart';
import '../../features/auth/presentation/views/mfa_channel_screen.dart';
import '../../features/auth/presentation/views/otp_screen.dart';
import '../../features/auth/presentation/views/password_login_screen.dart';
import '../../features/auth/presentation/views/verification_screen.dart';
import '../../features/auth/presentation/views/change_password_screen.dart';
import '../../features/auth/presentation/views/verify_email_screen.dart';
import '../../features/home/presentation/views/home_screen.dart';
import '../../features/notifications/presentation/views/notifications_screen.dart';
import '../../features/profile/presentation/views/profile_screen.dart';
import '../../features/settings/presentation/views/settings_screen.dart';
import '../../features/trips/domain/entities/trip.dart';
import '../../features/trips/presentation/views/trip_details_screen.dart';
import '../../features/trips/presentation/views/trip_success_screen.dart';
import '../../features/trips/presentation/views/trips_list_screen.dart';
import '../services/app_update_service.dart';
import '../services/notification_launch_handler.dart';
import '../storage/auth_storage.dart';
import '../utils/fcm_notification_logger.dart';
import '../theme/app_colors.dart';
import '../widgets/app_update_presenter.dart';

/// Route observer for detecting when routes become visible again (e.g. after pop).
/// Used by TripsListScreen to reload data when returning from trip details.
final RouteObserver<ModalRoute<void>> routeObserver =
    RouteObserver<ModalRoute<void>>();

/// Global router instance holder for accessing router from interceptors
class AppRouterHolder {
  AppRouterHolder._();
  static final AppRouterHolder instance = AppRouterHolder._();

  GoRouter? _router;

  void setRouter(GoRouter router) {
    _router = router;
  }

  void clearRouter() {
    _router = null;
  }

  GoRouter? get router => _router;
}

/// Set by auth screens (OTP, login, etc.) so 401 interceptor does not redirect
/// to login when user is in the middle of auth (e.g. on OTP screen when
/// opening notification center triggers a request that returns 401).
class AuthScreenGuard {
  AuthScreenGuard._();
  static bool isOnAuthScreen = false;
}

const _logoAsset = 'assets/images/loadly_logo.png';

class _SplashScreen extends StatefulWidget {
  const _SplashScreen();

  @override
  State<_SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<_SplashScreen> {
  @override
  void initState() {
    super.initState();
    AuthScreenGuard.isOnAuthScreen = true;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      unawaited(_startSplashFlow());
    });
  }

  @override
  void dispose() {
    AuthScreenGuard.isOnAuthScreen = false;
    super.dispose();
  }

  Future<void> _startSplashFlow() async {
    final updateFuture = AppUpdateService.instance?.checkForUpdate();
    await Future.wait<void>([
      Future<void>.delayed(const Duration(seconds: 2)),
      if (updateFuture != null) updateFuture.then((_) {}),
    ]);

    if (!mounted) return;

    final updateInfo = updateFuture != null ? await updateFuture : null;
    if (!mounted) return;

    if (updateInfo?.isForced == true) {
      await showAppUpdate(context: context, info: updateInfo!);
      return;
    }

    final destination = AuthStorage.instance.isLoggedIn() ? '/home' : '/login';
    FcmNotificationLogger.logStep(
      step: 'splash_navigate',
      source: 'splashScreen',
      route: destination,
      isLoggedIn: AuthStorage.instance.isLoggedIn(),
      detail: 'pendingRoute=${NotificationLaunchHandler.pendingRoute}',
    );
    AppRouterHolder.instance.router?.go(destination);
    NotificationLaunchHandler.navigateIfPending(
      isLoggedIn: AuthStorage.instance.isLoggedIn(),
    );

    if (updateInfo != null) {
      WidgetsBinding.instance.addPostFrameCallback((_) async {
        final rootContext = ChuckerFlutter.navigatorKey.currentContext;
        if (rootContext != null && rootContext.mounted) {
          await showAppUpdate(context: rootContext, info: updateInfo);
        }
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return CupertinoPageScaffold(
      backgroundColor: AppColors.white,
      child: SafeArea(
        child: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Image.asset(
                _logoAsset,
                width: 140,
                height: 168,
                fit: BoxFit.contain,
              ),
              const SizedBox(height: 24),
              const CupertinoActivityIndicator(
                color: AppColors.mainBlue,
                radius: 14,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

GoRouter createAppRouter() {
  final router = GoRouter(
    initialLocation: '/',
    navigatorKey: ChuckerFlutter.navigatorKey,
    observers: [routeObserver],
    redirect: (context, state) {
      final isLoggedIn = AuthStorage.instance.isLoggedIn();
      final isAuthRoute =
          state.uri.path == '/login' ||
          state.uri.path == '/auth/login' ||
          state.uri.path == '/verification' ||
          state.uri.path == '/otp' ||
          state.uri.path == '/mfa-channel' ||
          state.uri.path == '/';

      // If logged in and trying to access auth routes, redirect to home
      if (isLoggedIn && isAuthRoute && state.uri.path != '/') {
        return '/home';
      }

      // If not logged in and trying to access protected routes, redirect to login
      if (!isLoggedIn && !isAuthRoute) {
        return '/login';
      }

      return null; // No redirect needed
    },
    routes: [
      GoRoute(path: '/', builder: (context, state) => const _SplashScreen()),
      GoRoute(path: '/login', builder: (context, state) => const LoginScreen()),
      GoRoute(
        path: '/auth/login',
        builder: (context, state) => const PasswordLoginScreen(),
      ),
      GoRoute(
        path: '/verification',
        builder: (context, state) {
          final email = state.uri.queryParameters['email'];
          final phone = state.uri.queryParameters['phone'];
          return VerificationScreen(email: email, phone: phone);
        },
      ),
      GoRoute(
        path: '/mfa-channel',
        builder: (context, state) {
          final email = state.uri.queryParameters['email'] ?? '';
          final phone = state.uri.queryParameters['phone'] ?? '';
          return MfaChannelScreen(email: email, phone: phone);
        },
      ),
      GoRoute(
        path: '/otp',
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
      GoRoute(path: '/home', builder: (context, state) => const HomeScreen()),
      GoRoute(
        path: '/profile',
        builder: (context, state) => const ProfileScreen(),
      ),
      GoRoute(
        path: '/notifications',
        builder: (context, state) => const NotificationsScreen(),
      ),
      GoRoute(
        path: '/settings',
        builder: (context, state) => const SettingsScreen(),
      ),
      GoRoute(
        path: '/change-password',
        builder: (context, state) => const ChangePasswordScreen(),
      ),
      GoRoute(
        path: '/verify-email',
        builder: (context, state) => const VerifyEmailScreen(),
      ),
      GoRoute(
        path: '/trips',
        builder: (context, state) => const TripsListScreen(),
      ),
      GoRoute(
        path: '/trips/success',
        builder: (context, state) {
          final extra = state.extra is Map<String, dynamic>
              ? state.extra as Map<String, dynamic>
              : null;
          final trip = extra?['trip'];
          return TripSuccessScreen(trip: trip is Trip ? trip : null);
        },
      ),
      GoRoute(
        path: '/trips/:tripId',
        builder: (context, state) {
          final tripId = int.parse(state.pathParameters['tripId']!);
          final extra = state.extra is Map<String, dynamic>
              ? state.extra as Map<String, dynamic>
              : null;
          final returnRoute = extra?['returnTo'] as String?;
          return TripDetailsScreen(tripId: tripId, returnRoute: returnRoute);
        },
      ),
      GoRoute(
        path: '/trips/:tripId/customs',
        builder: (context, state) {
          return TripCustoms(trip: state.extra as Trip);
        },
      ),
    ],
  );

  // Store router instance globally for access from interceptors
  AppRouterHolder.instance.setRouter(router);

  return router;
}
