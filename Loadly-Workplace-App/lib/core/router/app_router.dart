import 'dart:async';

import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/views/otp_screen.dart';
import '../../features/auth/presentation/views/password_login_screen.dart';
import '../../features/home/domain/entities/collector_trips.dart';
import '../../features/home/presentation/views/filter_screen.dart';
import '../../features/home/presentation/views/home_screen.dart';
import '../../features/home/presentation/views/trip_details_screen.dart';
import '../../features/profile/presentation/views/profile_screen.dart';
import '../../features/trips/presentation/views/trips_list_screen.dart';
import '../services/app_update_service.dart';
import '../storage/auth_storage.dart';
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
    WidgetsBinding.instance.addPostFrameCallback((_) {
      unawaited(_startSplashFlow());
    });
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

    final destination = AuthStorage.instance.isLoggedIn()
        ? '/home'
        : '/auth/login';
    AppRouterHolder.instance.router?.go(destination);

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
          state.uri.path == '/auth/login' ||
          state.uri.path == '/otp' ||
          state.uri.path == '/';

      // If logged in and trying to access auth routes, redirect to home
      if (isLoggedIn && isAuthRoute && state.uri.path != '/') {
        return '/home';
      }

      // If not logged in and trying to access protected routes, redirect to login
      if (!isLoggedIn && !isAuthRoute) {
        return '/auth/login';
      }

      return null; // No redirect needed
    },
    routes: [
      GoRoute(path: '/', builder: (context, state) => const _SplashScreen()),
      GoRoute(
        path: '/auth/login',
        builder: (context, state) => const PasswordLoginScreen(),
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
      GoRoute(
        path: '/home',
        builder: (context, state) => HomeScreen(
          isFinancialCollector:
              AuthStorage.instance.getisFinancialCollector() ?? false,
        ),
      ),
      GoRoute(
        path: '/profile',
        builder: (context, state) => const ProfileScreen(),
      ),
      GoRoute(
        path: '/trips',
        builder: (context, state) => const TripsListScreen(),
      ),
      GoRoute(
        path: '/filter',
        builder: (context, state) => FilterScreen(
          filterData: state.extra as FilterData,
          isFinancialCollector:
              AuthStorage.instance.getisFinancialCollector() ?? false,
        ),
      ),
      GoRoute(
        path: '/trips/:tripId',
        builder: (context, state) =>
            TripDetailsScreen(trip: state.extra as CollectorTrips),
      ),
    ],
  );

  // Store router instance globally for access from interceptors
  AppRouterHolder.instance.setRouter(router);

  return router;
}
