import 'dart:async';
import 'dart:io' show Platform;

import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart' show ScaffoldMessenger;
import 'package:flutter_bloc/flutter_bloc.dart';
// ignore: depend_on_referenced_packages - transitive from google_maps_flutter
import 'package:google_maps_flutter_android/google_maps_flutter_android.dart';
// ignore: depend_on_referenced_packages - transitive from google_maps_flutter
import 'package:google_maps_flutter_platform_interface/google_maps_flutter_platform_interface.dart';
import 'package:flutter_foreground_task/flutter_foreground_task.dart';
import 'package:sentry_flutter/sentry_flutter.dart';
import 'core/location/trip_gps_foreground_task.dart';
import 'core/location/trip_gps_session_reconciler.dart';
import 'core/utils/service_locator.dart' show getIt, setup;

import 'features/auth/domain/use_cases/validate_password_usecase.dart';
import 'features/trips/presentation/bloc/trips_bloc.dart';
import 'features/trips/presentation/bloc/trips_event.dart';
import 'features/trips/presentation/bloc/trips_state.dart';

import 'core/locale/bloc/locale_bloc.dart';
import 'core/services/app_update_service.dart';
import 'core/services/fcm_background_handler.dart';
import 'core/services/fcm_service.dart';
import 'core/services/local_notification_service.dart';
import 'core/services/notification_deep_link_store.dart';
import 'core/services/notification_launch_handler.dart';
import 'core/utils/fcm_notification_logger.dart';
import 'core/locale/bloc/locale_state.dart';
import 'core/network/api_client.dart';
import 'core/router/app_router.dart';
import 'core/storage/auth_storage.dart';
import 'core/theme/app_theme.dart';
import 'features/auth/data/sources/remote/auth_remote_data_source.dart';
import 'features/auth/data/repositories/auth_impl.dart';
import 'features/auth/domain/use_cases/forget_password_usecase.dart';
import 'features/auth/domain/use_cases/get_profile_usecase.dart';
import 'features/auth/domain/use_cases/login_usecase.dart';
import 'features/auth/domain/use_cases/login_with_otp_usecase.dart';
import 'features/auth/domain/use_cases/logout_usecase.dart';
import 'features/auth/domain/use_cases/reset_password_usecase.dart';
import 'features/auth/domain/use_cases/send_login_otp_usecase.dart';
import 'features/auth/domain/use_cases/send_mfa_code_usecase.dart';
import 'features/auth/domain/use_cases/verify_mfa_usecase.dart';
import 'features/auth/domain/use_cases/enable_mfa_usecase.dart';
import 'features/auth/domain/use_cases/disable_mfa_usecase.dart';
import 'features/auth/domain/use_cases/change_password_usecase.dart';
import 'features/auth/domain/use_cases/update_profile_usecase.dart';
import 'features/auth/domain/use_cases/upload_driver_document_usecase.dart';
import 'features/auth/domain/use_cases/update_driver_document_usecase.dart';
import 'features/auth/domain/use_cases/verify_email_usecase.dart';
import 'features/auth/domain/use_cases/resend_verification_usecase.dart';
import 'features/auth/presentation/bloc/auth_bloc.dart';
import 'features/auth/presentation/bloc/auth_event.dart';
import 'features/home/data/repositories/home_impl.dart';
import 'features/home/data/sources/remote/home_remote_data_source.dart';
import 'core/location/driver_location_background_service.dart';
import 'core/location/trip_odometer_service.dart';
import 'core/location/unified_driver_location_hub.dart';
import 'features/home/domain/use_cases/go_offline_usecase.dart';
import 'features/home/domain/use_cases/go_online_usecase.dart';
import 'features/home/domain/use_cases/update_location_usecase.dart';
import 'features/home/presentation/bloc/home_bloc.dart';
import 'features/profile/presentation/bloc/profile_bloc.dart';
import 'features/notifications/data/repositories/notifications_impl.dart';
import 'features/notifications/data/sources/remote/notifications_remote_data_source.dart';
import 'features/notifications/domain/use_cases/get_notifications_usecase.dart';
import 'features/notifications/domain/use_cases/mark_notification_read_usecase.dart';
import 'features/notifications/presentation/bloc/notifications_bloc.dart';
import 'features/trips/data/repositories/trips_impl.dart';
import 'features/trips/data/sources/remote/trips_remote_data_source.dart';
import 'features/trips/domain/use_cases/get_trip_details_usecase.dart';
import 'features/trips/domain/use_cases/get_trips_usecase.dart';
import 'features/trips/domain/use_cases/trip_actions_usecase.dart';
import 'l10n/gen/app_localizations.dart';

/// Override with --dart-define=SENTRY_DSN=... for other environments.
const _sentryDsn = String.fromEnvironment(
  'SENTRY_DSN',
  defaultValue:
      'https://ae6e1918a34413f3d4428687887d0e05@o4508886465970176.ingest.de.sentry.io/4510835397296208',
);

bool get _isValidSentryDsn =>
    _sentryDsn.isNotEmpty &&
    _sentryDsn.contains('sentry.io') &&
    !_sentryDsn.contains('your-other-dsn');

Future<void> _runApp({bool useSentry = false}) async {
  // Initialize Android map renderer before any GoogleMap is created.
  // Fixes blank/blue screen when returning from camera or background on some devices.
  if (Platform.isAndroid) {
    try {
      final mapsImplementation = GoogleMapsFlutterPlatform.instance;
      if (mapsImplementation is GoogleMapsFlutterAndroid) {
        await mapsImplementation.initializeWithRenderer(
          AndroidMapRenderer.latest,
        );
      }
    } catch (e) {
      debugPrint('Map renderer init: $e');
    }
  }

  setup(); // getIt setup
  await getIt.allReady();
  await getIt<TripOdometerService>().init();

  // Initialize Firebase
  try {
    await Firebase.initializeApp();
    debugPrint('Firebase initialized successfully');
  } catch (e) {
    debugPrint('Error initializing Firebase: $e');
  }

  // Initialize storage first
  await AuthStorage.instance.init();
  await NotificationDeepLinkStore.init();

  final dio = createDriverApiClient();
  AppUpdateService.init(dio);

  // Auth dependencies
  final authRemoteDataSource = AuthRemoteDataSourceImpl(dio);
  final authRepository = AuthImpl(authRemoteDataSource);

  // Home dependencies
  final homeRemoteDataSource = HomeRemoteDataSourceImpl(dio);
  final homeRepository = HomeImpl(homeRemoteDataSource);

  // Trips dependencies
  final tripsRemoteDataSource = TripsRemoteDataSourceImpl(dio);
  final tripsRepository = TripsImpl(tripsRemoteDataSource);

  // Notifications dependencies
  final notificationsRemoteDataSource = NotificationsRemoteDataSourceImpl(dio);
  final notificationsRepository = NotificationsImpl(
    notificationsRemoteDataSource,
  );

  // Initialize use cases
  final loginUseCase = LoginUseCase(authRepository);
  final sendLoginOtpUseCase = SendLoginOtpUseCase(authRepository);
  final loginWithOtpUseCase = LoginWithOtpUseCase(authRepository);
  final forgetPasswordUseCase = ForgetPasswordUseCase(authRepository);
  final resetPasswordUseCase = ResetPasswordUseCase(authRepository);
  final resetPasswordWithCodeUseCase = ResetPasswordWithCodeUseCase(
    authRepository,
  );
  final logoutUseCase = LogoutUseCase(authRepository);
  final getProfileUseCase = GetProfileUseCase(authRepository);
  final sendMfaCodeUseCase = SendMfaCodeUseCase(authRepository);
  final verifyMfaUseCase = VerifyMfaUseCase(authRepository);
  final enableMfaUseCase = EnableMfaUseCase(authRepository);
  final disableMfaUseCase = DisableMfaUseCase(authRepository);
  final changePasswordUseCase = ChangePasswordUseCase(authRepository);
  final updateProfileUseCase = UpdateProfileUseCase(authRepository);
  final uploadDriverDocumentUseCase = UploadDriverDocumentUseCase(
    authRepository,
  );
  final updateDriverDocumentUseCase = UpdateDriverDocumentUseCase(
    authRepository,
  );
  final verifyEmailUseCase = VerifyEmailUseCase(authRepository);
  final resendVerificationUseCase = ResendVerificationUseCase(authRepository);
  final validatePasswordUseCase = ValidatePasswordUseCase(authRepository);

  // Initialize Local Notifications (for foreground FCM display)
  try {
    await LocalNotificationService.instance.initialize();
    LocalNotificationService.instance.setOnScheduleUpdateTap(() {
      FcmNotificationLogger.logStep(
        step: 'schedule_update_tap_navigate',
        source: 'main',
        route: '/trips',
      );
      final router = AppRouterHolder.instance.router;
      if (router != null) {
        router.go('/trips');
      }
    });
    LocalNotificationService.instance.setOnGenericNotificationTap(() {
      FcmNotificationLogger.logStep(
        step: 'generic_tap_navigate',
        source: 'main',
        route: '/trips',
      );
      final router = AppRouterHolder.instance.router;
      if (router != null) {
        router.go('/trips');
      }
    });
  } catch (e) {
    debugPrint('Error initializing LocalNotificationService: $e');
  }

  // Initialize FCM Service (tripsBloc set below after creation)
  TripsBloc? tripsBlocRef;
  try {
    final fcmService = FCMService();
    await fcmService.initialize(
      requestRuntimeNotificationPermission: Platform.isIOS,
    );

    void handleNotificationTap(RemoteMessage message) {
      FcmNotificationLogger.logStep(
        step: 'main_handle_notification_tap',
        source: 'main',
        message: message,
      );
      NotificationLaunchHandler.handleTap(
        message,
        onRefreshTrips: () => tripsBlocRef?.add(
          const TripsLoadTrips(page: 1, limit: 10, status: null),
        ),
      );
    }

    // Handle notification tap when app is opened from background/terminated state
    fcmService.setupNotificationTapHandler(handleNotificationTap);

    // Check if app was opened from a terminated state via notification
    final initialMessage = await fcmService.getInitialMessage();
    if (initialMessage != null) {
      FcmNotificationLogger.logStep(
        step: 'main_set_pending_from_initial_message',
        source: 'main',
        message: initialMessage,
      );
      NotificationLaunchHandler.setPendingFromMessage(initialMessage);
    } else {
      await NotificationLaunchHandler.handlePlatformNotificationLaunch(
        isLoggedIn: AuthStorage.instance.isLoggedIn(),
        onRefreshTrips: () => tripsBlocRef?.add(
          const TripsLoadTrips(page: 1, limit: 10, status: null),
        ),
      );
    }
  } catch (e) {
    debugPrint('Error initializing FCM Service: $e');
  }

  // Home use cases
  final goOnlineUseCase = GoOnlineUseCase(homeRepository);
  final goOfflineUseCase = GoOfflineUseCase(homeRepository);
  final updateLocationUseCase = UpdateLocationUseCase(homeRepository);
  final backgroundLocationService = DriverLocationBackgroundService(
    updateLocationUseCase,
    getIt<UnifiedDriverLocationHub>(),
  );

  // Trips use cases
  final getTripsUseCase = GetTripsUseCase(tripsRepository);
  final getTripDetailsUseCase = GetTripDetailsUseCase(tripsRepository);
  final getDocumentDownloadUrlUseCase = GetDocumentDownloadUrlUseCase(
    tripsRepository,
  );
  final startLoadingUseCase = StartLoadingUseCase(tripsRepository);
  final completeLoadingUseCase = CompleteLoadingUseCase(tripsRepository);
  final startUnloadingUseCase = StartUnloadingUseCase(tripsRepository);
  final completeUnloadingUseCase = CompleteUnloadingUseCase(tripsRepository);
  final startTripUseCase = StartTripUseCase(tripsRepository);
  final arriveAtWaypointUseCase = ArriveAtWaypointUseCase(tripsRepository);
  final departFromWaypointUseCase = DepartFromWaypointUseCase(tripsRepository);
  final enterCustomsUseCase = EnterCustomsUseCase(tripsRepository);
  final exitCustomsUseCase = ExitCustomsUseCase(tripsRepository);
  final completeTripUseCase = CompleteTripUseCase(tripsRepository);

  // Notifications use cases
  final getNotificationsUseCase = GetNotificationsUseCase(
    notificationsRepository,
  );
  final markNotificationReadUseCase = MarkNotificationReadUseCase(
    notificationsRepository,
  );

  // Initialize BloC with use cases
  final authBloc = AuthBloc(
    loginUseCase: loginUseCase,
    sendLoginOtpUseCase: sendLoginOtpUseCase,
    loginWithOtpUseCase: loginWithOtpUseCase,
    forgetPasswordUseCase: forgetPasswordUseCase,
    resetPasswordUseCase: resetPasswordUseCase,
    resetPasswordWithCodeUseCase: resetPasswordWithCodeUseCase,
    logoutUseCase: logoutUseCase,
    sendMfaCodeUseCase: sendMfaCodeUseCase,
    verifyMfaUseCase: verifyMfaUseCase,
    enableMfaUseCase: enableMfaUseCase,
    disableMfaUseCase: disableMfaUseCase,
    changePasswordUseCase: changePasswordUseCase,
    verifyEmailUseCase: verifyEmailUseCase,
    resendVerificationUseCase: resendVerificationUseCase,
    validatePasswordUseCase: validatePasswordUseCase,
  );
  final profileBloc = ProfileBloc(
    getProfileUseCase: getProfileUseCase,
    updateProfileUseCase: updateProfileUseCase,
    uploadDriverDocumentUseCase: uploadDriverDocumentUseCase,
    updateDriverDocumentUseCase: updateDriverDocumentUseCase,
  );
  final localeBloc = LocaleBloc();
  final homeBloc = HomeBloc(
    goOnlineUseCase: goOnlineUseCase,
    goOfflineUseCase: goOfflineUseCase,
    backgroundLocationService: backgroundLocationService,
  );

  if (AuthStorage.instance.getIsOnline() == true) {
    unawaited(backgroundLocationService.start());
  }
  final notificationsBloc = NotificationsBloc(
    getNotificationsUseCase: getNotificationsUseCase,
    markNotificationReadUseCase: markNotificationReadUseCase,
  );
  final tripsBloc = TripsBloc(
    getTripsUseCase: getTripsUseCase,
    getTripDetailsUseCase: getTripDetailsUseCase,
    getDocumentDownloadUrlUseCase: getDocumentDownloadUrlUseCase,
    startLoadingUseCase: startLoadingUseCase,
    completeLoadingUseCase: completeLoadingUseCase,
    startUnloadingUseCase: startUnloadingUseCase,
    completeUnloadingUseCase: completeUnloadingUseCase,
    startTripUseCase: startTripUseCase,
    arriveAtWaypointUseCase: arriveAtWaypointUseCase,
    departFromWaypointUseCase: departFromWaypointUseCase,
    enterCustomsUseCase: enterCustomsUseCase,
    exitCustomsUseCase: exitCustomsUseCase,
    completeTripUseCase: completeTripUseCase,
    tripOdometerService: getIt<TripOdometerService>(),
    tripGpsSessionReconciler: getIt<TripGpsSessionReconciler>(),
  );

  tripsBlocRef = tripsBloc;

  final gpsSessionReconciler = getIt<TripGpsSessionReconciler>();
  if (AuthStorage.instance.isLoggedIn()) {
    unawaited(_resumeOpenTripGps(gpsSessionReconciler));
  }

  // When any FCM notification arrives (foreground), refresh trips
  FCMService().setOnNotificationReceived(() {
    FcmNotificationLogger.logStep(
      step: 'trips_refresh_triggered',
      source: 'main_onNotificationReceived',
    );
    unawaited(gpsSessionReconciler.reconcileOpenSessionFromServer());
    tripsBloc.add(const TripsLoadTrips(page: 1, limit: 10, status: null));
  });

  // Check session on startup
  authBloc.add(const AuthCheckSessionRequested());
  // Don't request location here - let HomeScreen request it when it's actually shown

  runApp(
    MultiBlocProvider(
      providers: [
        BlocProvider.value(value: authBloc),
        BlocProvider.value(value: profileBloc),
        BlocProvider.value(value: localeBloc),
        BlocProvider.value(value: homeBloc),
        BlocProvider.value(value: notificationsBloc),
        BlocProvider.value(value: tripsBloc),
      ],
      child: _AppLifecycleRefresher(
        tripsBloc: tripsBloc,
        onNotificationResume: () =>
            NotificationLaunchHandler.handlePlatformNotificationLaunch(
              isLoggedIn: AuthStorage.instance.isLoggedIn(),
              onRefreshTrips: () => tripsBloc.add(
                const TripsLoadTrips(page: 1, limit: 10, status: null),
              ),
            ),
        child: useSentry
            ? SentryWidget(child: const LoadlyDriverApp())
            : const LoadlyDriverApp(),
      ),
    ),
  );
}

bool _sentryInitialized = false;

Future<void> _resumeOpenTripGps(TripGpsSessionReconciler reconciler) async {
  await getIt<TripOdometerService>().resumeTrackingForOpenSession();
  await reconciler.reconcileOpenSessionFromServer();
}

Future<void> main() async {
  // Ensure Flutter binding is initialized before accessing platform channels
  WidgetsFlutterBinding.ensureInitialized();
  FlutterForegroundTask.initCommunicationPort();
  TripGpsForegroundController.ensureInitialized();
  ChuckerFlutter.showOnRelease = true;

  // Must be registered before any other Firebase/FCM setup (required for killed-state delivery).
  FirebaseMessaging.onBackgroundMessage(firebaseMessagingBackgroundHandler);

  if (_isValidSentryDsn && !_sentryInitialized && kReleaseMode) {
    try {
      await SentryFlutter.init(
        (options) {
          options.dsn = _sentryDsn;
          options.tracesSampleRate = 1.0;
          options.sendDefaultPii = true;
        },
        appRunner: () async {
          _sentryInitialized = true;
          await _runApp(useSentry: true);
        },
      );
      return;
    } catch (_) {
      await _runApp(useSentry: false);
    }
  } else {
    await _runApp(useSentry: _sentryInitialized);
  }
}

/// Refreshes trips when app resumes from background (e.g. after receiving
/// a notification while app was in background).
class _AppLifecycleRefresher extends StatefulWidget {
  const _AppLifecycleRefresher({
    required this.tripsBloc,
    required this.onNotificationResume,
    required this.child,
  });

  final TripsBloc tripsBloc;
  final Future<void> Function() onNotificationResume;
  final Widget child;

  @override
  State<_AppLifecycleRefresher> createState() => _AppLifecycleRefresherState();
}

class _AppLifecycleRefresherState extends State<_AppLifecycleRefresher>
    with WidgetsBindingObserver {
  /// True after the app leaves the foreground (not merely [inactive], e.g. notification shade).
  bool _leftForegroundSinceLastResume = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    super.dispose();
  }

  bool _isOnHomeRoute() {
    final router = AppRouterHolder.instance.router;
    if (router == null) return false;
    try {
      return router.state.uri.path == '/home';
    } catch (_) {
      return false;
    }
  }

  /// Home keeps refresh on every resume (including notification shade). Other routes
  /// only refresh after a true background transition ([paused]/[hidden]), not shade-only [inactive].
  bool _shouldRunResumeRefresh() {
    if (_isOnHomeRoute()) return true;
    return _leftForegroundSinceLastResume;
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.paused ||
        state == AppLifecycleState.hidden) {
      _leftForegroundSinceLastResume = true;
    }

    if (state == AppLifecycleState.resumed) {
      unawaited(_resumeOpenTripGps(getIt<TripGpsSessionReconciler>()));
      if (!_shouldRunResumeRefresh()) {
        return;
      }
      _leftForegroundSinceLastResume = false;

      unawaited(widget.onNotificationResume());

      final tripsState = widget.tripsBloc.state;
      final isOnTripDetails =
          tripsState is TripDetailsLoading ||
          tripsState is TripDetailsLoaded ||
          tripsState is TripActionInProgress;
      if (isOnTripDetails) return;
      widget.tripsBloc.add(
        const TripsLoadTrips(page: 1, limit: 10, status: null),
      );
    }
  }

  @override
  Widget build(BuildContext context) => widget.child;
}

class LoadlyDriverApp extends StatelessWidget {
  const LoadlyDriverApp({super.key});

  @override
  Widget build(BuildContext context) {
    return ScaffoldMessenger(
      child: BlocBuilder<LocaleBloc, LocaleState>(
        builder: (context, localeState) {
          final locale = localeState is LocaleLoaded
              ? localeState.locale
              : localeState is LocaleInitial
              ? localeState.locale
              : const Locale('ar');
          return CupertinoApp.router(
            title: lookupAppLocalizations(locale).appTitle,
            theme: buildAppTheme(locale.languageCode),
            locale: locale,
            localizationsDelegates: AppLocalizations.localizationsDelegates,
            supportedLocales: AppLocalizations.supportedLocales,
            routerConfig: createAppRouter(),
          );
        },
      ),
    );
  }
}
