import 'dart:io' show Platform;

import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart' show ScaffoldMessenger;
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:sentry_flutter/sentry_flutter.dart';

import 'core/services/app_update_service.dart';
import 'core/services/temp_file_service.dart';
import 'features/home/domain/use_cases/get_trips_usecase.dart';
import 'features/trips/domain/use_cases/bulk_collect_documents_usecase.dart';
import 'features/trips/domain/use_cases/bulk_confirm_documents_usecase.dart';
import 'features/trips/domain/use_cases/collect_documents_usecase.dart';
import 'features/home/domain/use_cases/get_documents_usecase.dart';
import 'features/trips/domain/use_cases/confirm_documents_usecase.dart';
import 'features/trips/presentation/bloc/trips_bloc.dart';
import 'core/locale/bloc/locale_bloc.dart';
import 'core/services/fcm_service.dart';
import 'core/services/local_notification_service.dart';
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
import 'features/auth/domain/use_cases/logout_usecase.dart';
import 'features/auth/domain/use_cases/reset_password_usecase.dart';
import 'features/auth/domain/use_cases/send_mfa_code_usecase.dart';
import 'features/auth/domain/use_cases/verify_mfa_usecase.dart';
import 'features/auth/presentation/bloc/auth_bloc.dart';
import 'features/auth/presentation/bloc/auth_event.dart';
import 'features/home/data/repositories/home_impl.dart';
import 'features/home/data/sources/remote/home_remote_data_source.dart';
import 'features/home/presentation/bloc/home_bloc.dart';
import 'features/profile/presentation/bloc/profile_bloc.dart';
import 'features/trips/data/repositories/trips_impl.dart';
import 'features/trips/data/sources/remote/trips_remote_data_source.dart';
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
  // Initialize Firebase
  try {
    await Firebase.initializeApp();
    debugPrint('Firebase initialized successfully');
  } catch (e) {
    debugPrint('Error initializing Firebase: $e');
  }

  // Initialize storage first
  await AuthStorage.instance.init();

  final dio = createWorkplaceApiClient();
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

  // Initialize use cases
  final loginUseCase = LoginUseCase(authRepository);
  final forgetPasswordUseCase = ForgetPasswordUseCase(authRepository);
  final resetPasswordUseCase = ResetPasswordUseCase(authRepository);
  final resetPasswordWithCodeUseCase = ResetPasswordWithCodeUseCase(
    authRepository,
  );
  final logoutUseCase = LogoutUseCase(authRepository);
  final getProfileUseCase = GetProfileUseCase(authRepository);
  final sendMfaCodeUseCase = SendMfaCodeUseCase(authRepository);
  final verifyMfaUseCase = VerifyMfaUseCase(authRepository);

  // Initialize Local Notifications (for foreground FCM display)
  try {
    await LocalNotificationService.instance.initialize();
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
      final router = AppRouterHolder.instance.router;
      if (router != null) {
        Future.microtask(() {
          try {
            router.go('/home');
          } catch (e) {
            debugPrint('Failed to navigate: $e');
          }
        });
      }
    }

    // Handle notification tap when app is opened from background/terminated state
    fcmService.setupNotificationTapHandler(handleNotificationTap);

    // Check if app was opened from a terminated state via notification
    final initialMessage = await fcmService.getInitialMessage();
    if (initialMessage != null) {
      debugPrint('App opened from terminated state via notification');
      // Wait for splash (2s) + router to be ready before navigating
      Future.delayed(const Duration(milliseconds: 2500), () {
        final router = AppRouterHolder.instance.router;
        if (router != null) {
          try {
            router.go('/home');
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
  final getTripsUseCase = GetTripsUseCase(homeRepository);
  final getDocumentsUsecase = GetDocumentsUsecase(homeRepository);

  // Trips use cases
  final collectDocumentsUsecase = CollectDocumentsUsecase(tripsRepository);
  final confirmDocumentsUsecase = ConfirmDocumentsUsecase(tripsRepository);
  final bulkCollectDocumentsUsecase = BulkCollectDocumentsUsecase(
    tripsRepository,
  );
  final bulkConfirmDocumentsUsecase = BulkConfirmDocumentsUsecase(
    tripsRepository,
  );

  // Initialize BloC with use cases
  final authBloc = AuthBloc(
    loginUseCase: loginUseCase,
    forgetPasswordUseCase: forgetPasswordUseCase,
    resetPasswordUseCase: resetPasswordUseCase,
    resetPasswordWithCodeUseCase: resetPasswordWithCodeUseCase,
    logoutUseCase: logoutUseCase,
    sendMfaCodeUseCase: sendMfaCodeUseCase,
    verifyMfaUseCase: verifyMfaUseCase,
  );
  final profileBloc = ProfileBloc(getProfileUseCase: getProfileUseCase);
  final localeBloc = LocaleBloc();
  final homeBloc = HomeBloc(
    getTripsUseCase: getTripsUseCase,
    getDocumentsUsecase: getDocumentsUsecase,
  );
  final tripsBloc = TripsBloc(
    collectDocumentsUsecase: collectDocumentsUsecase,
    confirmDocumentsUsecase: confirmDocumentsUsecase,
    bulkCollectDocumentsUsecase: bulkCollectDocumentsUsecase,
    bulkConfirmDocumentsUsecase: bulkConfirmDocumentsUsecase,
  );

  // When any FCM notification arrives (foreground), refresh trips
  FCMService().setOnNotificationReceived(() {});

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
        BlocProvider.value(value: tripsBloc),
      ],
      child: useSentry
          ? SentryWidget(child: const LoadlyWorkplaceApp())
          : const LoadlyWorkplaceApp(),
    ),
  );
}

bool _sentryInitialized = false;

Future<void> main() async {
  // Ensure Flutter binding is initialized before accessing platform channels
  WidgetsFlutterBinding.ensureInitialized();
  await TempFileService().cleanOnStartup();
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

class LoadlyWorkplaceApp extends StatelessWidget {
  const LoadlyWorkplaceApp({super.key});

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
