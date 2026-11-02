import 'dart:async';

import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/views/auth_success_screen.dart';
import '../../features/auth/presentation/views/register_screen.dart';
import '../../features/auth/presentation/views/login_screen.dart';
import '../../features/auth/presentation/views/mfa_channel_screen.dart';
import '../../features/auth/presentation/views/onboarding_screen.dart';
import '../../features/auth/presentation/views/otp_screen.dart';
import '../../features/auth/presentation/views/password_login_screen.dart';
import '../../features/auth/presentation/views/verification_screen.dart';
import '../../features/auth/presentation/views/change_password_screen.dart';
import '../../features/auth/presentation/views/verify_email_screen.dart';
import '../../features/home/presentation/views/home_screen.dart';
import '../../features/notifications/presentation/views/notifications_screen.dart';
import '../../features/profile/presentation/views/profile_screen.dart';
import '../../features/reports/domain/entities/invoice.dart' hide Trip;
import '../../features/reports/domain/entities/invoice_payment_request.dart';
import '../../features/reports/presentation/views/cost_tracking_details_screen.dart';
import '../../features/reports/presentation/views/cost_tracking_screen.dart';
import '../../features/reports/presentation/views/invoice_details_screen.dart';
import '../../features/reports/presentation/views/invoice_payment_request_details_screen.dart';
import '../../features/reports/presentation/views/invoice_payment_requests_listing_screen.dart';
import '../../features/reports/presentation/views/invoices_listing_screen.dart';
import '../../features/reports/presentation/views/reports_screen.dart';
import '../../features/trips/domain/entities/trip.dart';
import '../../features/trips/presentation/views/track_trips_screen.dart';
import '../../features/settings/presentation/views/settings_screen.dart';
import '../../features/trips/presentation/views/add_trip_screen.dart';
import '../../features/trips/presentation/views/add_trip_success_screen.dart';
import '../../features/trips/presentation/views/trip_details_screen.dart';
import '../../features/trips/presentation/views/trips_list_screen.dart';
import '../../l10n/gen/app_localizations.dart';
import '../services/app_update_service.dart';
import '../storage/auth_storage.dart';
import '../theme/app_colors.dart';
import '../widgets/app_update_presenter.dart';
import 'navigation_state.dart';

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

const _logoAsset = 'assets/images/loadly_logo.svg';

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
        : '/onboarding';
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
    final l10n = AppLocalizations.of(context)!;
    return CupertinoPageScaffold(
      backgroundColor: AppColors.white,
      child: SafeArea(
        child: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              SvgPicture.asset(
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
              const SizedBox(height: 16),
              Text(
                l10n.appTitle,
                style: CupertinoTheme.of(context)
                    .textTheme
                    .navLargeTitleTextStyle
                    .copyWith(color: AppColors.darkGray),
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
    redirect: (context, state) {
      final isLoggedIn = AuthStorage.instance.isLoggedIn();
      final isAuthRoute =
          state.uri.path == '/onboarding' ||
          state.uri.path == '/login' ||
          state.uri.path == '/auth/login' ||
          state.uri.path == '/verification' ||
          state.uri.path == '/otp' ||
          state.uri.path == '/register' ||
          state.uri.path == '/auth-success' ||
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
      GoRoute(
        path: '/onboarding',
        builder: (context, state) => const OnboardingScreen(),
      ),
      GoRoute(
        path: '/register',
        builder: (context, state) => const RegisterScreen(),
      ),
      GoRoute(
        path: '/auth-success',
        builder: (context, state) => const AuthSuccessScreen(),
      ),
      GoRoute(
        path: '/change-password',
        builder: (context, state) => const ChangePasswordScreen(),
      ),
      GoRoute(
        path: '/settings',
        builder: (context, state) => const SettingsScreen(),
      ),
      GoRoute(
        path: '/profile',
        builder: (context, state) => const ProfileScreen(),
      ),
      GoRoute(
        path: '/verify-email',
        builder: (context, state) => const VerifyEmailScreen(),
      ),
      GoRoute(
        path: '/home',
        pageBuilder: (context, state) => CustomTransitionPage(
          key: state.pageKey,
          child: const HomeScreen(),
          transitionsBuilder: (context, animation, secondaryAnimation, child) {
            final begin = NavigationState.slideFromRight
                ? const Offset(1.0, 0.0)
                : const Offset(-1.0, 0.0);
            return SlideTransition(
              position: Tween(begin: begin, end: Offset.zero).animate(
                CurvedAnimation(parent: animation, curve: Curves.easeInOut),
              ),
              child: child,
            );
          },
        ),
      ),
      GoRoute(
        path: '/trips',
        pageBuilder: (context, state) => CustomTransitionPage(
          key: state.pageKey,
          child: const TripsListScreen(),
          transitionsBuilder: (context, animation, secondaryAnimation, child) {
            final begin = NavigationState.slideFromRight
                ? const Offset(1.0, 0.0)
                : const Offset(-1.0, 0.0);

            return SlideTransition(
              position: Tween(begin: begin, end: Offset.zero).animate(
                CurvedAnimation(parent: animation, curve: Curves.easeInOut),
              ),
              child: child,
            );
          },
        ),
      ),
      GoRoute(
        path: '/track-trips',
        pageBuilder: (context, state) => CustomTransitionPage(
          key: state.pageKey,
          child: const TrackTripsScreen(),
          transitionsBuilder: (context, animation, secondaryAnimation, child) {
            final begin = NavigationState.slideFromRight
                ? const Offset(1.0, 0.0)
                : const Offset(-1.0, 0.0);

            return SlideTransition(
              position: Tween(begin: begin, end: Offset.zero).animate(
                CurvedAnimation(parent: animation, curve: Curves.easeInOut),
              ),
              child: child,
            );
          },
        ),
      ),
      GoRoute(
        path: '/reports',
        pageBuilder: (context, state) => CustomTransitionPage(
          key: state.pageKey,
          child: const ReportsScreen(),
          transitionsBuilder: (context, animation, secondaryAnimation, child) {
            final begin = NavigationState.slideFromRight
                ? const Offset(1.0, 0.0)
                : const Offset(-1.0, 0.0);

            return SlideTransition(
              position: Tween(begin: begin, end: Offset.zero).animate(
                CurvedAnimation(parent: animation, curve: Curves.easeInOut),
              ),
              child: child,
            );
          },
        ),
      ),
      GoRoute(
        path: '/notifications',
        pageBuilder: (context, state) => CustomTransitionPage(
          key: state.pageKey,
          child: const NotificationsScreen(),
          transitionsBuilder: (context, animation, secondaryAnimation, child) {
            final begin = NavigationState.slideFromRight
                ? const Offset(1.0, 0.0)
                : const Offset(-1.0, 0.0);

            return SlideTransition(
              position: Tween(begin: begin, end: Offset.zero).animate(
                CurvedAnimation(parent: animation, curve: Curves.easeInOut),
              ),
              child: child,
            );
          },
        ),
      ),
      GoRoute(
        path: '/trips/:tripId',
        builder: (context, state) {
          final tripId = int.parse(state.pathParameters['tripId']!);
          return TripDetailsScreen(
            tripId: tripId,
            backRoute: state.extra as String,
          );
        },
      ),
      GoRoute(
        path: '/add-trip',
        builder: (context, state) => AddTripScreen(
          tripId: state.uri.queryParameters['tripId'] == null
              ? null
              : int.parse(state.uri.queryParameters['tripId']!),
          formData: state.extra as FormData,
        ),
      ),
      GoRoute(
        path: '/add-trip-success',
        builder: (context, state) => const AddTripSuccessScreen(),
      ),
      GoRoute(
        path: '/cost-tracking',
        builder: (context, state) => const CostTrackingScreen(),
      ),
      GoRoute(
        path: '/cost-tracking/:tripId',
        builder: (context, state) =>
            CostTrackingDetailsScreen(trip: state.extra as Trip),
      ),
      GoRoute(
        path: '/invoices',
        builder: (context, state) => const InvoicesListingScreen(),
      ),
      GoRoute(
        path: '/invoices/:invoiceId',
        builder: (context, state) =>
            InvoiceDetailsScreen(invoice: state.extra as Invoice),
      ),
      GoRoute(
        path: '/invoice-payment-requests',
        builder: (context, state) =>
            const InvoicePaymentRequestsListingScreen(),
      ),
      GoRoute(
        path: '/invoice-payment-requests/:invoicePaymentRequestId',
        builder: (context, state) => InvoicePaymentRequestDetailsScreen(
          invoicePaymentRequest: state.extra as InvoicePaymentRequest,
        ),
      ),
    ],
  );

  // Store router instance globally for access from interceptors
  AppRouterHolder.instance.setRouter(router);

  return router;
}
