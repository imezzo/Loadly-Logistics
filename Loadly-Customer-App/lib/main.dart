import 'dart:io';

import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
// ignore: depend_on_referenced_packages - transitive from google_maps_flutter
import 'package:google_maps_flutter_android/google_maps_flutter_android.dart';
// ignore: depend_on_referenced_packages - transitive from google_maps_flutter
import 'package:google_maps_flutter_platform_interface/google_maps_flutter_platform_interface.dart';
import 'package:sentry_flutter/sentry_flutter.dart';
import 'package:firebase_core/firebase_core.dart';

import 'core/locale/bloc/locale_bloc.dart';
import 'core/models/trip_schedule_update.dart';
import 'core/services/app_update_service.dart';
import 'core/services/fcm_service.dart';
import 'core/locale/bloc/locale_state.dart';
import 'core/network/api_client.dart';
import 'core/router/app_router.dart';
import 'core/services/local_notification_service.dart';
import 'core/storage/auth_storage.dart';
import 'core/theme/app_theme.dart';
import 'core/widgets/app_background.dart';
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
import 'features/auth/domain/use_cases/validate_password_usecase.dart';
import 'features/auth/domain/use_cases/verify_mfa_usecase.dart';
import 'features/auth/domain/use_cases/enable_mfa_usecase.dart';
import 'features/auth/domain/use_cases/disable_mfa_usecase.dart';
import 'features/auth/domain/use_cases/change_password_usecase.dart';
import 'features/auth/domain/use_cases/update_profile_usecase.dart';
import 'features/auth/domain/use_cases/verify_email_usecase.dart';
import 'features/auth/domain/use_cases/resend_verification_usecase.dart';
import 'features/auth/presentation/bloc/auth_bloc.dart';
import 'features/auth/presentation/bloc/auth_event.dart';
import 'features/home/data/repositories/home_impl.dart';
import 'features/home/data/sources/remote/home_remote_data_source.dart';
import 'features/home/domain/use_cases/get_monthly_trips_kilometers_usecase.dart';
import 'features/home/domain/use_cases/get_trips_statistics_usecase.dart';
import 'features/home/presentation/bloc/home_bloc.dart';
import 'features/notifications/data/repositories/notifications_impl.dart';
import 'features/notifications/data/sources/remote/notifications_remote_data_source.dart';
import 'features/notifications/domain/use_cases/get_notifications_usecase.dart';
import 'features/notifications/domain/use_cases/mark_notification_read_usecase.dart';
import 'features/notifications/presentation/bloc/notifications_bloc.dart';
import 'features/profile/presentation/bloc/profile_bloc.dart';
import 'features/reports/data/repositories/reports_impl.dart';
import 'features/reports/data/sources/remote/reports_remote_data_source.dart';
import 'features/reports/domain/use_cases/get_invoice_payment_requests_usecase.dart';
import 'features/reports/domain/use_cases/get_invoice_statistics_usecase.dart';
import 'features/reports/domain/use_cases/get_invoices_usecase.dart';
import 'features/reports/domain/use_cases/get_trip_cost_period_usecase.dart';
import 'features/reports/presentation/bloc/reports_bloc.dart';
import 'features/trips/data/repositories/trips_impl.dart';
import 'features/trips/data/sources/remote/trips_remote_data_source.dart';
import 'features/trips/domain/use_cases/add_trip_usecase.dart';
import 'features/trips/domain/use_cases/delete_trip_usecase.dart';
import 'features/trips/domain/use_cases/extract_place_usecase.dart';
import 'features/trips/domain/use_cases/get_contracts_usecase.dart';
import 'features/trips/domain/use_cases/get_trip_details_usecase.dart';
import 'features/trips/domain/use_cases/get_trips_usecase.dart';
import 'features/trips/domain/use_cases/track_trips_usecase.dart';
import 'features/trips/domain/use_cases/update_trip_usecase.dart';
import 'features/trips/presentation/bloc/trips_bloc.dart';
import 'features/trips/presentation/bloc/trips_event.dart';
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

  // Initialize Firebase
  try {
    await Firebase.initializeApp();
    debugPrint('Firebase initialized successfully');
  } catch (e) {
    debugPrint('Error initializing Firebase: $e');
  }

  // Initialize storage first
  await AuthStorage.instance.init();

  final dio = createCustomerApiClient();
  AppUpdateService.init(dio);

  // Auth dependencies
  final authRemoteDataSource = AuthRemoteDataSourceImpl(dio);
  final authRepository = AuthImpl(authRemoteDataSource);

  // Trips dependencies
  final homeRemoteDataSource = HomeRemoteDataSourceImpl(dio);
  final homeRepository = HomeImpl(homeRemoteDataSource);

  // Trips dependencies
  final tripsRemoteDataSource = TripsRemoteDataSourceImpl(dio);
  final tripsRepository = TripsImpl(tripsRemoteDataSource);

  // Reports dependencies
  final reportsRemoteDataSource = ReportsRemoteDataSourceImpl(dio);
  final reportsRepository = ReportsImpl(reportsRemoteDataSource);

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
  final verifyEmailUseCase = VerifyEmailUseCase(authRepository);
  final resendVerificationUseCase = ResendVerificationUseCase(authRepository);
  final validatePasswordUseCase = ValidatePasswordUseCase(authRepository);

  // Initialize Local Notifications (for foreground FCM display)
  try {
    await LocalNotificationService.instance.initialize();
    LocalNotificationService.instance.setOnScheduleUpdateTap(() {
      final router = AppRouterHolder.instance.router;
      if (router != null) {
        router.go('/home');
      }
    });
    LocalNotificationService.instance.setOnGenericNotificationTap(() {
      final router = AppRouterHolder.instance.router;
      if (router != null) {
        router.go('/home');
      }
    });
  } catch (e) {
    debugPrint('Error initializing LocalNotificationService: $e');
  }

  // Initialize FCM Service
  try {
    final fcmService = FCMService();
    await fcmService.initialize(
      requestRuntimeNotificationPermission: Platform.isIOS,
    );

    void handleNotificationTap(RemoteMessage message) {
      final body = message.notification?.body ?? message.data['body'] ?? '';
      final isTripScheduleUpdate = TripScheduleUpdate.tryParse(body) != null;
      final router = AppRouterHolder.instance.router;
      // Refresh trips so home page shows any new trip
      if (router != null) {
        Future.microtask(() {
          try {
            if (isTripScheduleUpdate) {
              LocalNotificationService.instance
                  .addTripScheduleUpdateFromMessage(message);
            }
            router.go('/trips');
          } catch (e) {
            debugPrint('Failed to navigate: $e');
          }
        });
      }
    }

    // Handle notification tap when app is opened from background/terminated state
    fcmService.setupNotificationTapHandler((message) {
      debugPrint('Notification tapped: ${message.data}');
      final router = AppRouterHolder.instance.router;
      if (router != null) {
        // Navigate to notifications screen when notification is tapped
        // You can extend this to navigate to specific screens based on notification type
        Future.microtask(() {
          try {
            router.go('/notifications');
          } catch (e) {
            debugPrint('Failed to navigate to notifications: $e');
          }
        });
      }
    });

    // Handle notification tap when app is opened from background/terminated state
    fcmService.setupNotificationTapHandler(handleNotificationTap);

    // Check if app was opened from a terminated state via notification
    final initialMessage = await fcmService.getInitialMessage();
    if (initialMessage != null) {
      debugPrint('App opened from terminated state via notification');
      final body =
          initialMessage.notification?.body ??
          initialMessage.data['body'] ??
          '';
      final isTripScheduleUpdate = TripScheduleUpdate.tryParse(body) != null;
      // Wait for splash (2s) + router to be ready before navigating
      Future.delayed(const Duration(milliseconds: 2500), () {
        final router = AppRouterHolder.instance.router;
        if (router != null) {
          try {
            if (isTripScheduleUpdate) {
              LocalNotificationService.instance
                  .addTripScheduleUpdateFromMessage(initialMessage);
            }
            router.go('/trips');
          } catch (e) {
            debugPrint('Failed to navigate: $e');
          }
        }
      });
    }
  } catch (e) {
    debugPrint('Error initializing FCM Service: $e');
  }

  // Home use cases
  final getTripsStatisticsUseCase = GetTripsStatisticsUseCase(homeRepository);
  final getMonthlyTripsKilometersUseCase = GetMonthlyTripsKilometersUseCase(
    homeRepository,
  );

  // Trips use cases
  final getTripsUseCase = GetTripsUseCase(tripsRepository);
  final getTripDetailsUseCase = GetTripDetailsUseCase(tripsRepository);
  final addTripUseCase = AddTripUseCase(tripsRepository);
  final updateTripUseCase = UpdateTripUseCase(tripsRepository);
  final deleteTripUsecase = DeleteTripUsecase(tripsRepository);
  final getContractReferenceNumbersUseCase = GetContractReferenceNumbersUseCase(
    tripsRepository,
  );
  final getVehicleTypesUseCase = GetVehicleTypesUseCase(tripsRepository);
  final getConsumptionTypeUseCase = GetConsumptionTypeUseCase(tripsRepository);
  final getContractDestinationsUseCase = GetContractDestinationsUseCase(
    tripsRepository,
  );
  final getContractDestinationUseCase = GetContractDestinationUseCase(
    tripsRepository,
  );
  final extractPlaceUseCase = ExtractPlaceUseCase(tripsRepository);
  final trackTripsUsecase = TrackTripsUsecase(tripsRepository);

  // Reports use cases
  final getTripCostPeriodUseCase = GetTripCostPeriodUseCase(reportsRepository);
  final getInvoiceStatisticsUseCase = GetInvoiceStatisticsUseCase(
    reportsRepository,
  );
  final getInvoicesUseCase = GetInvoicesUseCase(reportsRepository);
  final getInvoicePaymentRequestsUseCase = GetInvoicePaymentRequestsUseCase(
    reportsRepository,
  );

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
  );
  final localeBloc = LocaleBloc();
  final homeBloc = HomeBloc(
    getTripsStatisticsUseCase: getTripsStatisticsUseCase,
    getMonthlyTripsKilometersUseCase: getMonthlyTripsKilometersUseCase,
  );
  final tripsBloc = TripsBloc(
    getTripsUseCase: getTripsUseCase,
    getTripDetailsUseCase: getTripDetailsUseCase,
    getContractReferenceNumbersUseCase: getContractReferenceNumbersUseCase,
    getVehicleTypesUseCase: getVehicleTypesUseCase,
    getConsumptionTypeUseCase: getConsumptionTypeUseCase,
    getContractDestinationsUseCase: getContractDestinationsUseCase,
    getContractDestinationUseCase: getContractDestinationUseCase,
    extractPlaceUseCase: extractPlaceUseCase,
    addTripUseCase: addTripUseCase,
    updateTripUseCase: updateTripUseCase,
    deleteTripUsecase: deleteTripUsecase,
    trackTripsUsecase: trackTripsUsecase,
  );
  final reportsBloc = ReportsBloc(
    getTripCostPeriodUseCase: getTripCostPeriodUseCase,
    getInvoiceStatisticsUseCase: getInvoiceStatisticsUseCase,
    getInvoicesUseCase: getInvoicesUseCase,
    getInvoicePaymentRequestsUseCase: getInvoicePaymentRequestsUseCase,
  );
  final notificationsBloc = NotificationsBloc(
    getNotificationsUseCase: getNotificationsUseCase,
    markNotificationReadUseCase: markNotificationReadUseCase,
  );

  // When any FCM notification arrives (foreground), refresh trips
  FCMService().setOnNotificationReceived(() {
    tripsBloc.add(const TripsLoadTrips());
  });

  // Check session on startup
  authBloc.add(const AuthCheckSessionRequested());
  // Don't request location here - let HomeScreen request it when it's actually shown

  runApp(
    MultiBlocProvider(
      providers: [
        BlocProvider.value(value: authBloc),
        BlocProvider.value(value: localeBloc),
        BlocProvider.value(value: profileBloc),
        BlocProvider.value(value: homeBloc),
        BlocProvider.value(value: tripsBloc),
        BlocProvider.value(value: reportsBloc),
        BlocProvider.value(value: notificationsBloc),
      ],
      child: useSentry
          ? SentryWidget(child: const LoadlyCustomerApp())
          : const LoadlyCustomerApp(),
    ),
  );
}

bool _sentryInitialized = false;

Future<void> main() async {
  // Ensure Flutter binding is initialized before accessing platform channels
  WidgetsFlutterBinding.ensureInitialized();
  ChuckerFlutter.showOnRelease = true;

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

class LoadlyCustomerApp extends StatelessWidget {
  const LoadlyCustomerApp({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<LocaleBloc, LocaleState>(
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
          builder: (context, child) => Stack(
            children: [
              const AppBackground(),
              if (child != null) Positioned.fill(child: child),
            ],
          ),
        );
      },
    );
  }
}
