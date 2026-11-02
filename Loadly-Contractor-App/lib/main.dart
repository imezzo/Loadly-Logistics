import 'dart:async';

import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart' show DefaultTextStyle, Theme, ThemeData;
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:sentry_flutter/sentry_flutter.dart';

import 'core/notifications/fcm_background_handler.dart';
import 'core/router/app_router.dart';
import 'core/services/app_update_service.dart';
import 'core/services/fcm_service.dart';

import 'core/locale/bloc/locale_bloc.dart';
import 'core/locale/bloc/locale_state.dart';
import 'core/network/api_client.dart';
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
import 'features/auth/domain/use_cases/update_profile_usecase.dart';
import 'features/auth/domain/use_cases/validate_password_usecase.dart';
import 'features/auth/domain/use_cases/verify_mfa_usecase.dart';
import 'features/auth/domain/use_cases/enable_mfa_usecase.dart';
import 'features/auth/domain/use_cases/disable_mfa_usecase.dart';
import 'features/auth/domain/use_cases/change_password_usecase.dart';
import 'features/auth/presentation/bloc/auth_bloc.dart';
import 'features/auth/presentation/bloc/auth_state.dart';
import 'features/auth/presentation/bloc/auth_event.dart';
import 'features/notifications/data/repositories/notifications_impl.dart';
import 'features/notifications/data/sources/remote/notifications_remote_data_source.dart';
import 'features/notifications/domain/use_cases/get_notifications_usecase.dart';
import 'features/notifications/domain/use_cases/mark_notification_read_usecase.dart';
import 'features/notifications/presentation/bloc/notifications_bloc.dart';
import 'features/profile/presentation/bloc/profile_bloc.dart';
import 'l10n/gen/app_localizations.dart';
import 'features/auth/domain/use_cases/register_usecase.dart';
import 'features/auth/domain/use_cases/get_documents_usecase.dart';
import 'features/trips/data/datasources/lookups_remote_datasource.dart';
import 'features/trips/domain/use_cases/get_default_vehicle_types_usecase.dart';

import 'features/trips/data/datasources/trip_remote_datasource.dart';
import 'features/trips/data/repositories/trip_repository_impl.dart';
import 'features/trips/domain/use_cases/book_trip_usecase.dart';
import 'features/trips/domain/use_cases/get_booking_requests_usecase.dart';
import 'features/trips/domain/use_cases/get_trip_statistics_usecase.dart';
import 'features/trips/domain/use_cases/get_trip_usecase.dart';
import 'features/trips/domain/use_cases/get_trips_usecase.dart';
import 'features/trips/domain/use_cases/withdraw_trip_usecase.dart';
import 'features/trips/presentation/bloc/booking_request/booking_request_bloc.dart';
import 'features/trips/presentation/bloc/trip/trip_bloc.dart';
import 'features/trips/presentation/bloc/trip_statistics/trip_statistics_bloc.dart';
import 'features/trips/presentation/bloc/trip_statistics/trip_statistics_event.dart';

import 'features/drivers/data/datasources/driver_remote_datasource.dart';
import 'features/drivers/data/repositories/driver_repository_impl.dart';
import 'features/drivers/domain/use_cases/create_driver_usecase.dart';
import 'features/drivers/domain/use_cases/get_driver_document_types_usecase.dart';
import 'features/drivers/domain/use_cases/get_drivers_usecase.dart';
import 'features/drivers/domain/use_cases/update_trip_driver_usecase.dart';
import 'features/drivers/presentation/bloc/driver_bloc.dart';

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
  // Initialize storage first
  await AuthStorage.instance.init();

  final dio = createContractorApiClient();
  AppUpdateService.init(dio);

  // Auth dependencies
  final authRemoteDataSource = AuthRemoteDataSourceImpl(dio);
  final authRepository = AuthImpl(authRemoteDataSource);

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
  final sendMfaCodeUseCase = SendMfaCodeUseCase(authRepository);
  final verifyMfaUseCase = VerifyMfaUseCase(authRepository);
  final enableMfaUseCase = EnableMfaUseCase(authRepository);
  final disableMfaUseCase = DisableMfaUseCase(authRepository);
  final getProfileUseCase = GetProfileUseCase(authRepository);
  final updateProfileUseCase = UpdateProfileUseCase(authRepository);
  final changePasswordUseCase = ChangePasswordUseCase(authRepository);
  final validatePasswordUseCase = ValidatePasswordUseCase(authRepository);
  final registerUseCase = RegisterUseCase(authRepository);
  final getDocumentsUseCase = GetDocumentsUseCase(authRepository);
  // Trips
  final tripRemoteDataSource = TripRemoteDataSourceImpl(dio);
  final tripRepository = TripRepositoryImpl(tripRemoteDataSource);
  final getTripsUseCase = GetTripsUseCase(tripRepository);
  final getTripUseCase = GetTripUseCase(tripRepository);
  final bookTripUseCase = BookTripUseCase(tripRepository);
  final withdrawTripUseCase = WithdrawTripUseCase(tripRepository);
  final getBookingRequestsUseCase = GetBookingRequestsUseCase(tripRepository);
  final getInvitationStatisticsUseCase = GetInvitationStatisticsUseCase(
    tripRepository,
  );
  final getStatusStatisticsUseCase = GetStatusStatisticsUseCase(tripRepository);
  final getPayoutStatisticsUseCase = GetPayoutStatisticsUseCase(tripRepository);

  final tripBloc = TripBloc(
    getTripsUseCase: getTripsUseCase,
    getTripUseCase: getTripUseCase,
    bookTripUseCase: bookTripUseCase,
    withdrawTripUseCase: withdrawTripUseCase,
  );
  final tripStatisticsBloc = TripStatisticsBloc(
    getInvitationStatisticsUseCase: getInvitationStatisticsUseCase,
    getStatusStatisticsUseCase: getStatusStatisticsUseCase,
    getPayoutStatisticsUseCase: getPayoutStatisticsUseCase,
  );
  final lookupsRemoteDataSource = LookupsRemoteDataSourceImpl(dio);
  final getDefaultVehicleTypesUseCase = GetDefaultVehicleTypesUseCase(
    lookupsRemoteDataSource,
  );
  final bookingRequestBloc = BookingRequestBloc(getBookingRequestsUseCase);
  // drivers
  final driverRemoteDataSource = DriverRemoteDataSourceImpl(dio);
  final driverRepository = DriverRepositoryImpl(driverRemoteDataSource);
  final getDriversUseCase = GetDriversUseCase(driverRepository);
  final getDriverDocumentTypesUseCase = GetDriverDocumentTypesUseCase(
    driverRepository,
  );
  final createDriverUseCase = CreateDriverUseCase(driverRepository);
  final updateTripDriverUseCase = UpdateTripDriverUseCase(driverRepository);
  final driverBloc = DriverBloc(
    getDriversUseCase: getDriversUseCase,
    getDriverDocumentTypesUseCase: getDriverDocumentTypesUseCase,
    createDriverUseCase: createDriverUseCase,
    updateTripDriverUseCase: updateTripDriverUseCase,
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
    validatePasswordUseCase: validatePasswordUseCase,
    registerUseCase: registerUseCase,
    getDocumentsUseCase: getDocumentsUseCase,
  );
  final profileBloc = ProfileBloc(
    getProfileUseCase: getProfileUseCase,
    updateProfileUseCase: updateProfileUseCase,
  );

  final localeBloc = LocaleBloc();

  final notificationsBloc = NotificationsBloc(
    getNotificationsUseCase: getNotificationsUseCase,
    markNotificationReadUseCase: markNotificationReadUseCase,
  );

  // Check session on startup
  if (AuthStorage.instance.isLoggedIn() &&
      AuthStorage.instance.getContractorUserId() == null) {
    final profileResult = await getProfileUseCase();
    await profileResult.fold((_) async {}, (profile) async {
      await AuthStorage.instance.saveContractorUserId(profile.id);
    });
  }
  authBloc.add(const AuthCheckSessionRequested());

  runApp(
    RepositoryProvider<GetDefaultVehicleTypesUseCase>.value(
      value: getDefaultVehicleTypesUseCase,
      child: MultiBlocProvider(
        providers: [
          BlocProvider.value(value: authBloc),
          BlocProvider.value(value: localeBloc),
          BlocProvider.value(value: profileBloc),
          BlocProvider.value(value: notificationsBloc),
          BlocProvider.value(value: tripBloc),
          BlocProvider.value(value: tripStatisticsBloc),
          BlocProvider.value(value: bookingRequestBloc),
          BlocProvider.value(value: driverBloc),
        ],
        child: useSentry
            ? SentryWidget(child: const LoadlyContractorApp())
            : const LoadlyContractorApp(),
      ),
    ),
  );
}

bool _sentryInitialized = false;

Future<void> main() async {
  // Ensure Flutter binding is initialized before accessing platform channels
  WidgetsFlutterBinding.ensureInitialized();
  if (FCMService.instance.isSupportedPlatform) {
    try {
      await FCMService.ensureFirebaseInitialized();
      FirebaseMessaging.onBackgroundMessage(firebaseMessagingBackgroundHandler);
    } catch (error, stackTrace) {
      debugPrint(
        '[FCM] background handler registration failed: $error\n$stackTrace',
      );
    }
    try {
      await FCMService.instance.initialize();
    } catch (error, stackTrace) {
      debugPrint('[FCM] startup initialization failed: $error\n$stackTrace');
    }
  }
  ChuckerFlutter.showOnRelease = true;

  if (_isValidSentryDsn && !_sentryInitialized && kReleaseMode) {
    try {
      await SentryFlutter.init(
        (options) {
          options.dsn = _sentryDsn;
          options.tracesSampleRate = 0.2;
          options.sendDefaultPii = false;
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

class LoadlyContractorApp extends StatelessWidget {
  const LoadlyContractorApp({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocListener<AuthBloc, AuthState>(
      listenWhen: (previous, current) =>
          current is AuthAuthenticated ||
          current is AuthUnauthenticated ||
          current is AuthLoginSuccess,
      listener: (context, state) {
        FCMService.instance.setAuthenticated(
          _sessionAllowsProtectedNavigation(state),
        );
      },
      child: BlocListener<AuthBloc, AuthState>(
        listenWhen: (previous, current) =>
            current is AuthUnauthenticated && previous is! AuthUnauthenticated,
        listener: (context, state) {
          context.read<TripBloc>().resetHomeNewTripHighlightsForSessionChange();
          context.read<TripStatisticsBloc>().add(ResetTripStatistics());
        },
        child: BlocBuilder<LocaleBloc, LocaleState>(
          builder: (context, localeState) {
            final locale = localeState is LocaleLoaded
                ? localeState.locale
                : localeState is LocaleInitial
                ? localeState.locale
                : const Locale('ar');
            final l10nTitle = lookupAppLocalizations(locale);
            return Theme(
              data: ThemeData(fontFamily: appFontFamily),
              child: DefaultTextStyle(
                style: const TextStyle(fontFamily: appFontFamily),
                child: CupertinoApp.router(
                  title: l10nTitle.appTitle,
                  theme: buildAppTheme(locale.languageCode),
                  locale: locale,
                  localizationsDelegates:
                      AppLocalizations.localizationsDelegates,
                  supportedLocales: AppLocalizations.supportedLocales,
                  routerConfig: createAppRouter(),
                ),
              ),
            );
          },
        ),
      ),
    );
  }
}

bool _sessionAllowsProtectedNavigation(AuthState state) {
  if (state is AuthAuthenticated) return true;
  if (state is AuthUnauthenticated) return false;
  if (state is AuthLoginSuccess) {
    final redirect = state.loginResult.redirect;
    if (redirect == 'mfa' || redirect == 'forget_password') return false;
    return AuthStorage.instance.isLoggedIn();
  }
  return false;
}
