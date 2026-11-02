# Architecture

Loadly Customer (`loadly_customer`, version `1.0.3+8`) is a Flutter app for company customers. It covers sign-in, a trip dashboard, trip management, live trip tracking on a map, cost and invoice reports, and notifications.

The UI is built with Cupertino widgets and hosted by `CupertinoApp.router` in `LoadlyCustomerApp` (`lib/main.dart`). Feature code follows a lightweight clean architecture: presentation, domain, and data.

## Overall shape

```text
Screen → BLoC event → Use case → Repository → Remote data source → Dio → API
```

Results come back as `Either<Failure, Success>` from `dartz`. Screens listen to BLoC states and navigate with GoRouter.

Why this split: screens stay free of HTTP details, and each feature can change its API mapping without rewriting the UI. The split is applied consistently for auth, home, trips, reports, and notifications. Profile and settings are presentation-only and reuse auth use cases.

## Folder structure

```text
lib/
├── main.dart                 # Startup, manual DI, root BLoC providers
├── core/                     # Shared infrastructure
│   ├── config/               # Store URLs, Google Maps dart-define
│   ├── errors/               # User-facing failure text
│   ├── locale/               # LocaleBloc (en/ar)
│   ├── models/               # App update and notification parse models
│   ├── network/              # Dio client, endpoints, interceptors, parsers
│   ├── permissions/          # In-app permission disclosure dialog
│   ├── router/               # GoRouter and splash
│   ├── services/             # FCM, local notifications, biometrics, app update
│   ├── storage/              # AuthStorage
│   ├── theme/                # Colors, layout, Cupertino theme
│   ├── utils/
│   └── widgets/              # Shared buttons, nav, scaffold, update UI
├── features/
│   ├── auth/
│   ├── home/
│   ├── trips/
│   ├── reports/
│   ├── notifications/
│   ├── profile/              # Presentation only
│   └── settings/             # Presentation only (one screen)
└── l10n/                     # ARB sources and generated localizations
```

A typical feature looks like this:

```text
features/<feature>/
├── data/
│   ├── models/               # JSON DTOs (*_dto.dart, generated *.g.dart)
│   ├── mappers/              # Present for trips and reports
│   ├── repositories/         # *Impl classes
│   └── sources/remote/       # Dio calls
├── domain/
│   ├── entities/
│   ├── failures/
│   ├── repositories/         # Abstract interfaces
│   └── use_cases/
└── presentation/
    ├── bloc/
    ├── views/
    └── widgets/
```

Other top-level areas:

| Path | Role |
| --- | --- |
| `assets/images/` | SVG icons and logos declared in `pubspec.yaml` |
| `test/widget_test.dart` | The only test file |
| `android/`, `ios/` | Mobile projects. Application id / bundle id: `com.loadly.customer` |
| `web/`, `macos/`, `linux/`, `windows/` | Default Flutter platform folders. No release workflow was found for them |
| `.github/workflows/build-apk.yml` | Android release APK pipeline |
| `.fvmrc` | Pins Flutter `3.41.1` |

`android/app/src/main/kotlin/com/loadly/customer/MainActivity.kt` declares `package com.loadly.customer`. The directory name (`loadly`) does not match the package (`loadly`). The class extends `FlutterFragmentActivity`, which the file comment says is required for biometrics.

## State management

[flutter_bloc](https://pub.dev/packages/flutter_bloc) with one BLoC per feature, created in `main()` and provided once at the root with `MultiBlocProvider`.

| BLoC | File | Responsibility |
| --- | --- | --- |
| `AuthBloc` | `lib/features/auth/presentation/bloc/auth_bloc.dart` | Login, OTP, MFA verify, password flows, logout, biometrics, session check |
| `HomeBloc` | `lib/features/home/presentation/bloc/home_bloc.dart` | Trip statistics and monthly kilometers |
| `TripsBloc` | `lib/features/trips/presentation/bloc/trips_bloc.dart` | Trip list, details, create/update/delete, lookups, tracking |
| `ReportsBloc` | `lib/features/reports/presentation/bloc/reports_bloc.dart` | Cost period, invoice statistics, invoices, payment requests |
| `NotificationsBloc` | `lib/features/notifications/presentation/bloc/notifications_bloc.dart` | Notification list and mark-as-read |
| `ProfileBloc` | `lib/features/profile/presentation/bloc/profile_bloc.dart` | Load and update profile through auth use cases |
| `LocaleBloc` | `lib/core/locale/bloc/locale_bloc.dart` | English/Arabic, persisted in SharedPreferences |

Events and states are separate `Equatable` classes. There is no `BlocProvider` per route; screens call `context.read<SomeBloc>()`.

`UseCase` / `UseCaseNoParams` in `lib/features/auth/domain/use_cases/usecase.dart` are the shared contracts. Their success type is `Either<dynamic, T>`, so failure types are enforced by each repository, not by the base class.

## Navigation

`createAppRouter()` in `lib/core/router/app_router.dart` builds a `GoRouter`. `AppRouterHolder` keeps that instance so interceptors and notification handlers can call `go` without a `BuildContext`.

Splash (`/`) waits two seconds and runs `AppUpdateService.checkForUpdate()`. Then:

- A forced update stays on the update UI (`showAppUpdate`).
- Otherwise the app goes to `/home` when `AuthStorage.isLoggedIn()` is true, and to `/onboarding` when it is not.
- An optional update is shown after that navigation.

`isLoggedIn()` is true when a refresh token is stored. It does not check the access token.

The redirect:

- Logged-in users on auth routes (except `/`) are sent to `/home`.
- Logged-out users on any other route are sent to `/login`.

| Route | Screen |
| --- | --- |
| `/` | Splash |
| `/onboarding` | `OnboardingScreen` |
| `/login` | Phone / OTP entry (`LoginScreen`) |
| `/auth/login` | Email or phone plus password (`PasswordLoginScreen`) |
| `/verification` | Channel choice. Query: `email`, `phone` |
| `/otp` | OTP. Query: `contact`, `viaSms`, `forgetPassword`, `isMfa` |
| `/mfa-channel` | MFA channel. Query: `email`, `phone` |
| `/register` | `RegisterScreen` (see note below) |
| `/auth-success` | `AuthSuccessScreen` |
| `/change-password` | `ChangePasswordScreen` |
| `/verify-email` | `VerifyEmailScreen` |
| `/home` | Dashboard |
| `/trips` | Trip list |
| `/trips/:tripId` | Trip details. `extra` must be a `String` back route |
| `/add-trip` | Create or edit. Query `tripId` optional. `extra` must be `FormData` from `add_trip_screen.dart` (not Dio's `FormData`) |
| `/add-trip-success` | Success screen |
| `/track-trips` | Map tracking |
| `/reports` | Reports menu |
| `/cost-tracking` | Cost list |
| `/cost-tracking/:tripId` | Cost details. `extra` must be `Trip` from `lib/features/trips/domain/entities/trip.dart` |
| `/invoices` | Invoice list |
| `/invoices/:invoiceId` | Invoice details. `extra` must be `Invoice` |
| `/invoice-payment-requests` | Payment-request list |
| `/invoice-payment-requests/:invoicePaymentRequestId` | Details. `extra` must be `InvoicePaymentRequest` |
| `/notifications` | Notification list |
| `/settings` | Settings |
| `/profile` | Profile |

Main tabs use `MainScaffold` and `AppBottomNavBar`: Home, Trips, Real-time (`/track-trips`), Reports, Notifications. Slide direction is a static flag, `NavigationState.slideFromRight`.

Settings is opened from `AppNavBar`, not from the bottom bar. Profile is opened from settings. Back on settings, reports, and several tabs calls `context.go('/home')` because `PopScope` blocks the system pop.

`RegisterScreen._submit` only validates the form and calls `context.go('/auth-success')`. No repository method and no in-app navigation to `/register` were found. **Needs clarification:** whether registration is unfinished or intentionally unused.

The payment-request card on `ReportsScreen` is commented out. The route and bloc support still exist.

## Networking

`createCustomerApiClient()` in `lib/core/network/api_client.dart` builds one shared `Dio`.

Base URL (`kCustomerApiBaseUrl`):

```text
--dart-define=CUSTOMER_API_BASE_URL=...
```

Default in source: `https://stg-api-agg.loadly-eg.com/api`. A production URL is present only as a comment. Paths in `ApiEndpoints` already include `/company/...` or `/lookups/...`. Timeouts are 30 seconds. JSON `Accept` and `Content-Type` are set on the client.

Interceptor order:

1. `DeviceInterceptor` — `device-id`, `device-model`, `device-os`, `device-os-version`, `app-version`, `app-build-number`
2. `LanguageInterceptor` — `lang` header `ar` or `en`, read from SharedPreferences key `app_locale` (same key as `LocaleBloc`). Missing locale defaults to `ar`
3. `FcmTokenInterceptor` — `fcmToken` when a token is cached
4. `AuthInterceptor` — `Authorization: Bearer <access token>`. On 401 it calls `AuthStorage.refreshToken()` (`POST /company/auth/refresh-token`). Success retries the request. Failure clears tokens and sends the user to `/login`, except for paths listed in `_authAttemptPaths`
5. `AppUpdateInterceptor` — reads an app-version envelope from successful login, OTP login, MFA verify, and GET profile responses
6. `ChuckerDioInterceptor` — HTTP inspector. `main()` sets `ChuckerFlutter.showOnRelease = true`, so Chucker is not limited to debug builds

`_authAttemptPaths` contains `/companys/auth/resend-verification` (extra `s`). The real path is `/company/auth/resend-verification`. A 401 on resend verification is therefore not treated as an auth attempt.

Endpoints live in `lib/core/network/api_endpoints.dart`. Grouped:

| Area | Paths |
| --- | --- |
| Auth | `/company/auth/login`, `send-login-otp`, `login-with-otp`, `logout`, `profile`, `forget-password`, `reset-password`, `reset-password-with-code`, `change-password`, `send-mfa-code`, `verify-mfa`, `enable-mfa`, `disable-mfa`, `verify-email`, `resend-verification`, `refresh-token`, `validate-password` |
| Home | `/company/dashboard/trips`, `/company/dashboard/monthly-trips-kilometers` |
| Trips | `/company/trips`, `/company/trips/track`, `/company/maps/extract-place`, lookup paths under `/lookups/...` and `/company/contracts/...` |
| Reports | `/company/dashboard/trip-cost-period-totals`, `/company/dashboard/invoice-statistics`, `/company/invoices`, `/company/invoice-payment-requests` |
| Notifications | `/company/notifications`, `/company/notifications/:id/read` |
| App update | `/app-version/check` |

Response helpers in `api_response_parser.dart` expect an envelope with `success` and `data`. `extractApiMessage` reads `message` or `errors`.

## Models and entities

- **Entities** are hand-written classes under `domain/entities`. Several use `Equatable` (`CustomerProfile`, failures).
- **DTOs** are `@JsonSerializable()` classes under `data/models`, with committed `*.g.dart` files.
- **Mappers** exist for trips (`map_trip_dto_to_entity.dart`) and reports (`invoice_mapper.dart`, `invoice_payment_request_mapper.dart`). Auth and home map inside the repository or data source.
- `freezed` and `freezed_annotation` are in `pubspec.yaml`. No `@freezed` class was found under `lib/`. **Needs clarification:** whether Freezed was planned and not adopted.

`LoginResult.redirect` drives post-login navigation in the login screens:

- `mfa` → `/mfa-channel`
- `forget_password` → `/otp` with `forgetPassword=true`
- `mfaChannel == 0` → `/home` (code comment: super admin)
- any other case → `/mfa-channel`

Channel integers sent by the password and OTP screens: `1` email, `2` SMS. `MfaChannelScreen` defaults to `2`. A comment on `AuthResendVerificationRequested` says `0` is email and `1` is SMS, which does not match those screens or the repository comment (`1` email, `2` SMS). **Needs clarification** from the API contract.

## Repositories and services

| Repository | Implementation | Remote source |
| --- | --- | --- |
| `AuthRepository` | `AuthImpl` | `AuthRemoteDataSourceImpl` |
| `HomeRepository` | `HomeImpl` | `HomeRemoteDataSourceImpl` |
| `TripsRepository` | `TripsImpl` | `TripsRemoteDataSourceImpl` |
| `ReportsRepository` | `ReportsImpl` | `ReportsRemoteDataSourceImpl` |
| `NotificationsRepository` | `NotificationsImpl` | `NotificationsRemoteDataSourceImpl` |

There is no local data source. `safeApiCall` (`lib/core/network/safe_api_call.dart`) is used by `NotificationsImpl` only. The other repositories catch `DioException` themselves.

Services that are not feature repositories:

| Service | Role |
| --- | --- |
| `FCMService` | Firebase Messaging singleton: permission, token, foreground display, tap handling |
| `LocalNotificationService` | Foreground notification display. Channels `general_notifications` and `trip_schedule_update` |
| `BiometricService` | `local_auth` plus credentials in `FlutterSecureStorage` |
| `AppUpdateService` | `GET /app-version/check` with `appType=Company` and the current version |
| `AuthStorage` | Tokens and “remember me” |

`FCMService.setTokenCallback` exists, but `main.dart` does not set it. The token is attached to later API calls by `FcmTokenInterceptor` instead of a dedicated register-token call in this codebase.

## Dependency injection

There is no service locator (`get_it` is not a dependency). `main()` constructs Dio, repositories, use cases, and BLoCs by hand and passes them through constructors. That keeps the graph visible in one file. Adding a feature means editing `lib/main.dart`.

`AuthStorage`, `BiometricService`, `LocalNotificationService`, `FCMService`, and `AppUpdateService` are singletons or static holders, separate from the constructor graph.

## Local storage

No database package is used.

| Store | What |
| --- | --- |
| `SharedPreferences` | Access token, refresh token, remember-me flag, remembered login, locale (`app_locale`) |
| `FlutterSecureStorage` | Remembered password (`auth_remembered_password`) and biometric login, password, and enabled flag |

`AuthStorage.init()` must run before `isLoggedIn()` or token reads. `main()` does this before creating the API client.

Access and refresh tokens are not in secure storage. **Needs clarification:** whether that is an accepted product decision.

## Authentication

Session check on startup (`AuthCheckSessionRequested`) emits `AuthAuthenticated` or `AuthUnauthenticated` from the refresh token. Splash navigation uses `AuthStorage` directly, not that state.

Login methods implemented in the UI:

1. Password — `AuthLoginRequested` → `LoginUseCase` → tokens saved in `AuthBloc` on success.
2. OTP — `AuthSendLoginOtpRequested`, then `AuthLoginWithOtpRequested`.
3. Biometric — `AuthBiometricLoginRequested` on the login screens. `BiometricService.authenticateAndGetCredentials` reads saved credentials, then the bloc calls `LoginUseCase`. That call passes `locale: 'en'`, not the current app locale.
4. MFA verify — `AuthVerifyMfaRequested` after `AuthSendMfaCodeRequested`. Enable and disable MFA events exist on `AuthBloc`, but no screen dispatches `AuthEnableMfaRequested` or `AuthDisableMfaRequested`.

Logout calls the API and then `clearTokens()` even if the API fails. Settings navigates to `/login` on `AuthUnauthenticated`.

Password reset from the login screens uses forget-password with channel `1` or `2`, then `ResetPasswordWithCodeUseCase`. `ResetPasswordUseCase` (token-based) is constructed in `main()` and handled by `AuthBloc`, but no screen dispatches `AuthResetPasswordRequested`.

Biometric enable from settings asks for the password, then `AuthEnableBiometricRequested`. Disable sends `AuthDisableBiometricRequested`. The profile update that would sync `biometricFingerprint` to the API is commented out in `SettingsScreen`.

## Error handling

- Repositories return `Left(Failure)` for HTTP and unknown errors.
- Failure families: `AuthFailure`, `HomeFailure`, `TripsFailure`, `ReportsFailure`, `NotificationsFailure`. Kinds are network, server (message + status code), and unknown. Auth also has `AuthFailureInvalidCredentials`.
- BLoCs map failures to `AuthError`, `AuthLoginError`, or feature error states. Much of that text is hard-coded English in the bloc or `FailureMessageMapper`, not `AppLocalizations`.
- `AuthLoginError` is meant to be shown under the password field. Other auth errors use a `CupertinoAlertDialog`.
- 401 after a failed refresh clears the session and routes to `/login`, except the auth-attempt path list.
- Firebase, FCM, local notifications, and the app-update check catch errors and continue (`debugPrint`). A failed update check returns null and does not block startup.
- Sentry (`SentryFlutter.init`) runs only when `kReleaseMode` is true and `SENTRY_DSN` looks like a Sentry DSN. `tracesSampleRate` is `1.0`. `sendDefaultPii` is `true`. The app is wrapped in `SentryWidget` only on that path. Init failure falls back to running without Sentry.

## Configuration

Compile-time values use `String.fromEnvironment`:

| Define | Read in | Default in source |
| --- | --- | --- |
| `CUSTOMER_API_BASE_URL` | `api_client.dart` | Staging aggregator URL (see that file) |
| `SENTRY_DSN` | `main.dart` | A DSN string in that file |
| `GOOGLE_MAPS_API_KEY` | `google_maps_config.dart` | A key string in that file. Native maps use separate hard-coded keys |
| `IOS_APP_STORE_ID` | `app_store_config.dart` | Empty |
| `HUAWEI_APP_GALLERY_ID` | `app_store_config.dart` | Empty |
| `APP_STORE_DISTRIBUTION` | `app_store_config.dart` | `play` (`harmonyos` or `appgallery` selects Huawei) |
| `APP_STORE_URL` | `app_store_config.dart` | Empty; otherwise Play, App Store, or AppGallery is built from the ids above |

Native Google Maps keys (not the Dart define):

- Android: `com.google.android.geo.API_KEY` in `android/app/src/main/AndroidManifest.xml`
- iOS: `GMSServices.provideAPIKey` in `ios/Runner/AppDelegate.swift`

Firebase has no `firebase_options.dart`. `Firebase.initializeApp()` with no options relies on `android/app/google-services.json` and `ios/Runner/GoogleService-Info.plist`.

Sentry upload settings in `pubspec.yaml`: org `fuelin-wr`, project `loadly-loadly-driver`. The project name says “driver”. **Needs clarification:** shared Sentry project or a leftover name.

Android release signing reads `android/key.properties` (`keyAlias`, `keyPassword`, `storeFile`, `storePassword`) when that file exists. Otherwise the release build type uses the debug signing config (`android/app/build.gradle`).

Do not copy DSN, map keys, or keystore passwords into new docs or tickets.

## Main flow

1. `WidgetsFlutterBinding.ensureInitialized()`.
2. On Android, initialize the Google Maps renderer (`AndroidMapRenderer.latest`) before any map. The comment in `main.dart` says this avoids a blank map after the camera or backgrounding.
3. `Firebase.initializeApp()` (errors are logged and ignored).
4. `AuthStorage.instance.init()`.
5. Create the Dio client and `AppUpdateService.init`.
6. Construct repositories, use cases, and BLoCs.
7. Initialize local notifications and FCM.
8. `AuthCheckSessionRequested`.
9. On any foreground FCM message, `tripsBloc.add(TripsLoadTrips())`.
10. `runApp` inside Sentry when release mode and the DSN check pass.

After login, `handleLoginNavigationWithAppUpdate` can block on a forced update before the redirect from `LoginResult`.

Notification taps:

- `setupNotificationTapHandler` is called twice in `main()`. The method replaces the previous listener, so the second handler wins: trip-schedule bodies are recorded, then the router goes to `/trips`. The earlier handler that went to `/notifications` does not stay registered.
- A cold start from a notification waits 2.5 seconds, then also goes to `/trips`.
- Local-notification taps (foreground banners) go to `/home`.

`TripScheduleUpdate.tryParse` only recognizes English bodies that contain “Trip Schedule Updated” or “trip schedule has been updated” plus a date range.

## How features talk to each other

They do not call each other’s repositories. They share root BLoCs and `AuthStorage`.

- Profile and settings use `ProfileBloc`, which calls `GetProfileUseCase` and `UpdateProfileUseCase` on `AuthRepository`.
- Settings logout and biometric toggles send events to `AuthBloc`. Language buttons send `LocaleChanged` to `LocaleBloc`.
- FCM updates `TripsBloc` directly from `main()`, not through a notifications use case.
- `LanguageInterceptor` and `LocaleBloc` share the `app_locale` preference, so the `lang` header follows the in-app language.
- App-update data can arrive from the dedicated version endpoint (splash) or from auth/profile envelopes (`AppUpdateInterceptor` → `AppUpdateNotifier`). Profile uses `app_update_flow.dart` to present a pending update.

## Packages

| Package | Why it is here |
| --- | --- |
| `flutter_bloc` | Feature state |
| `go_router` | Routes, redirects, path parameters |
| `dio` | HTTP client and interceptors |
| `dartz` | `Either` for repository results |
| `equatable` | Event, state, and failure equality |
| `json_annotation` / `json_serializable` | DTO `fromJson` / `toJson` |
| `firebase_core`, `firebase_messaging` | Push messaging |
| `flutter_local_notifications` | Foreground notification UI |
| `flutter_secure_storage` | Password and biometric secrets |
| `shared_preferences` | Tokens, locale, remember-me |
| `local_auth` | Biometric prompt |
| `google_maps_flutter` | Track-trips map |
| `sentry_flutter` | Release crash and trace reporting |
| `chucker_flutter` | In-app HTTP log |
| `google_fonts` | Archivo (English) and Almarai (Arabic) in `buildAppTheme` |
| `flutter_svg` | Icon and logo assets |
| `cached_network_image` | Profile image |
| `intl` / `flutter_localizations` | Generated `AppLocalizations` |
| `device_info_plus`, `package_info_plus` | Device headers and version check |
| `url_launcher` | Store links and external URLs |
| `carousel_slider`, `dotted_border`, `number_pagination`, `pull_to_refresh`, `slide_countdown` | UI widgets used by screens |
| `build_runner`, `flutter_lints` | Codegen and analyzer |
| `flutter_launcher_icons`, `flutter_native_splash` | Icon and splash generation config in `pubspec.yaml` |

`geolocator` is not a dependency. Trip positions come from `GET /company/trips/track`, not from the device GPS.

## Testing

`test/widget_test.dart` is the only test. It builds `AuthBloc` and `LocaleBloc` by hand, pumps `LoadlyCustomerApp`, and expects a `CupertinoApp`. It does not follow `main()` (no `AuthStorage.init`, Firebase, or the other BLoCs). No unit tests for use cases or repositories were found. The APK workflow does not run `flutter test`.

## Build and deployment

- Version and build number: `version:` in `pubspec.yaml` (`1.0.3+8` at the time of this document).
- Android: Gradle wrapper 8.14, Android Gradle Plugin 8.11.1, Kotlin 2.2.20, Java 17 in CI, `coreLibraryDesugaring` enabled. `minSdk` / `targetSdk` come from the Flutter Gradle plugin.
- iOS: bundle id `com.loadly.customer`. `IPHONEOS_DEPLOYMENT_TARGET` in the Xcode project is `12.0`. The Podfile platform line is commented out. `NSFaceIDUsageDescription` is set.
- CI (`.github/workflows/build-apk.yml`): on push to `main` and `workflow_dispatch`. Installs FVM from `.fvmrc`, runs `fvm flutter build apk --release`, uploads the APK artifact, then a self-hosted job copies it to S3, can email a 7-day presigned URL through Amazon SES, and posts to Microsoft Teams. The workflow does not pass `--dart-define`, so the binary uses the defaults compiled into source. Tests are not a job in this workflow.

Linux, macOS, Windows, and web projects exist in the tree. No pipeline or product decision for shipping them was found in the repo. **Needs clarification:** which platforms are officially supported beyond Android.
