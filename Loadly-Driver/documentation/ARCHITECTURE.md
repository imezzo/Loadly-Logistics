# Architecture

Loadly Driver (`loadly_driver`) is a Flutter driver app. Drivers sign in, go online, share location, run trip steps (loading, waypoints, customs, unloading, completion), and receive push notifications.

Current app version in `pubspec.yaml`: **1.2.0+9**. Dart SDK constraint: `^3.8.0`. Flutter is pinned to **3.41.2** in `.fvmrc`.

This document describes the code as it is. Where the reason for a choice is not in the source or existing docs, it is marked **Needs clarification**.

## Overall architecture

The app is organized as **feature modules** with a **domain / data / presentation** split for auth, home, trips, and notifications. Profile and settings are presentation-only and reuse auth use cases.

```text
UI (Cupertino screens)
  → BLoC event
    → use case
      → repository interface (domain)
        → repository implementation (data)
          → remote data source
            → Dio
              → driver API
```

Results come back as `Either<Failure, T>` from the `dartz` package. BLoCs map failures to states. Screens render those states.

There is no local database. Persistence is `SharedPreferences`, `FlutterSecureStorage`, and in-memory singletons.

The widget tree is a `CupertinoApp.router` (`LoadlyDriverApp` in `lib/main.dart`), not `MaterialApp`. A Material `ScaffoldMessenger` wraps the app so Material snackbars can still be shown.

## Folder structure

```text
lib/
├── main.dart                 # Startup, composition root, root BLoCs
├── l10n/                     # ARB sources and generated localizations
├── core/                     # Shared infrastructure
│   ├── config/               # Store URLs, Google Maps key (dart-define)
│   ├── errors/               # Failure → message mapping
│   ├── locale/               # LocaleBloc
│   ├── location/             # Current position and background uploads
│   ├── models/               # Shared DTOs (app update, schedule payload)
│   ├── network/              # Dio client, endpoints, error mapping
│   ├── permissions/          # Prominent disclosure copy/helpers
│   ├── router/               # GoRouter
│   ├── services/             # FCM, biometrics, notifications, app update
│   ├── storage/              # AuthStorage
│   ├── theme/                # Colors, layout, Cupertino theme
│   ├── utils/                # Dates, geocoding, get_it, notification helpers
│   └── widgets/              # Shared UI (nav bar, dialogs, update screen)
└── features/
    ├── auth/                 # domain + data + presentation
    ├── home/                 # online/offline and map home
    ├── trips/                # list, details, step actions, OCR helper
    ├── notifications/        # inbox
    ├── profile/              # presentation only
    └── settings/             # presentation only
```

Other top-level areas:

| Path | Role |
| --- | --- |
| `assets/images/` | Logo, icons, background. Declared in `pubspec.yaml`. |
| `test/` | Unit tests and one widget test. |
| `android/`, `ios/` | Primary mobile targets. |
| `web/`, `macos/`, `linux/`, `windows/` | Flutter platform folders. See [Platforms](#platforms). |
| `.github/workflows/build-apk.yml` | Release APK build, S3 upload, email, Teams. |

`lib/features/auth/auth.dart` is a barrel export. It does not export every auth type (for example MFA and OTP use cases are imported directly).

## State management

**flutter_bloc** is the only state-management library used by features.

Root blocs are created in `lib/main.dart` and provided once with `MultiBlocProvider`:

| BLoC | File | Responsibility |
| --- | --- | --- |
| `AuthBloc` | `features/auth/presentation/bloc/auth_bloc.dart` | Login, OTP, MFA, password, email verification, biometrics, logout, session flag |
| `ProfileBloc` | `features/profile/presentation/bloc/profile_bloc.dart` | Load/update profile and driver documents via auth use cases |
| `HomeBloc` | `features/home/presentation/bloc/home_bloc.dart` | Online/offline, location permission, background reporting, schedule-update card |
| `TripsBloc` | `features/trips/presentation/bloc/trips_bloc.dart` | Trip list, details, and step actions |
| `NotificationsBloc` | `features/notifications/presentation/bloc/notifications_bloc.dart` | Notification list and mark-as-read |
| `LocaleBloc` | `core/locale/bloc/locale_bloc.dart` | `en` / `ar`, persisted under SharedPreferences key `app_locale` |

`AuthBloc` session check (`AuthCheckSessionRequested`) only looks at whether `AuthStorage` has an access token. It does not call the profile API.

**Why BLoC:** not documented in code. The README names BLoC as the project choice.

## Navigation and routing

`createAppRouter()` in `lib/core/router/app_router.dart` builds a `GoRouter`.

- Initial route: `/` (`_SplashScreen`).
- `navigatorKey`: `ChuckerFlutter.navigatorKey`.
- `observers`: a `RouteObserver` used by the trips list to reload when a route is popped.
- `AppRouterHolder` stores the router so interceptors and notification code can navigate without a `BuildContext`.

Auth redirect:

- Logged-in users who open `/login`, `/auth/login`, `/verification`, `/otp`, or `/mfa-channel` are sent to `/home`. `/` is not redirected this way.
- Logged-out users who open any other route are sent to `/login`.

`AuthScreenGuard.isOnAuthScreen` is set while the splash (and auth screens that opt in) is visible so a 401 during auth does not force `/login`.

| Route | Screen |
| --- | --- |
| `/` | Splash |
| `/login` | `LoginScreen` |
| `/auth/login` | `PasswordLoginScreen` |
| `/verification` | `VerificationScreen` (query: `email`, `phone`) |
| `/mfa-channel` | `MfaChannelScreen` |
| `/otp` | `OtpScreen` (query: `contact`, `viaSms`, `forgetPassword`, `isMfa`) |
| `/home` | `HomeScreen` |
| `/profile` | `ProfileScreen` |
| `/notifications` | `NotificationsScreen` |
| `/settings` | `SettingsScreen` |
| `/change-password` | `ChangePasswordScreen` (requires a session) |
| `/verify-email` | `VerifyEmailScreen` (requires a session) |
| `/trips` | `TripsListScreen` |
| `/trips/success` | `TripSuccessScreen` (`extra['trip']`) |
| `/trips/:tripId` | `TripDetailsScreen` |
| `/trips/:tripId/customs` | `TripCustoms` (`extra` is a `Trip`) |

`/trips/success` is registered before `/trips/:tripId` so `success` is not parsed as an id.

`MainScaffold` plus `AppBottomNavBar` wrap the main tabs. Indexes in `MainScaffold.getSelectedIndex`: 0 profile, 1 notifications, 2 trips, 3 home. The online/offline control sits in the bar notch and talks to `HomeBloc`.

## API and networking

`createDriverApiClient()` in `lib/core/network/api_client.dart` builds Dio:

- Base URL: `kDriverApiBaseUrl` from `--dart-define=DRIVER_API_BASE_URL`.
- Default: `https://stg-api-agg.loadly-eg.com/api/driver`.
- A production URL is commented next to that default.
- Connect and receive timeouts: 30 seconds.
- `kApiBaseUrl` strips a trailing `/driver`. `AppUpdateService.fetchVersionCheck` calls `$kApiBaseUrl/app-version/check`, so the version check is not under `/api/driver`.

Interceptor order:

1. `DeviceInterceptor` — `device-id`, `device-model`, `device-os`, `device-os-version`, `app-version`, `app-build-number`
2. `LanguageInterceptor` — `lang` header `ar` or `en` from `app_locale` (default `ar`)
3. `FcmTokenInterceptor` — `fcmToken` when `FCMService.currentToken` is set
4. `AuthInterceptor` — `Authorization: Bearer <access token>`
5. `AppUpdateInterceptor` — reads version policy from login, OTP login, MFA verify, and GET profile envelopes
6. `ChuckerDioInterceptor` — HTTP inspector

On HTTP 401, `AuthInterceptor` calls `AuthStorage.refreshToken()` (`POST /auth/refresh-token`). If refresh fails, tokens are cleared and the router goes to `/login`, unless the path is an auth attempt (login, OTP, password reset, and similar) or `AuthScreenGuard.isOnAuthScreen` is true.

`AuthStorage.refreshToken()` uses a **separate Dio** with no interceptors, so the refresh call does not recurse through `AuthInterceptor`.

Remote data sources throw `DioException` when the JSON envelope has `success != true`. `api_response_parser.dart` (`requireEnvelope`, `throwIfEnvelopeFailed`, `requireEnvelopeDataMap`) and `extractApiMessage` in `api_failure_mapper.dart` implement that.

Paths are centralized in `lib/core/network/api_endpoints.dart`.

### Auth

| Method | Path |
| --- | --- |
| POST | `/auth/login` |
| POST | `/auth/send-login-otp` |
| POST | `/auth/login-with-otp` |
| POST | `/auth/logout` |
| GET/PUT | `/auth/profile` |
| POST | `/auth/forget-password` |
| POST | `/auth/reset-password` |
| POST | `/auth/reset-password-with-code` |
| POST | `/auth/change-password` |
| POST | `/auth/send-mfa-code` |
| POST | `/auth/verify-mfa` |
| POST | `/auth/enable-mfa` |
| POST | `/auth/disable-mfa` |
| POST | `/auth/verify-email` |
| POST | `/auth/resend-verification` |
| POST | `/auth/refresh-token` |
| POST | `/auth/validate-password` |
| POST | `/driver/documents` |
| PUT | `/driver/documents/{id}` |

### Driver status

| Method | Path |
| --- | --- |
| POST | `/go-online` |
| POST | `/go-offline` |
| POST | `/location/ping` |

### Trips

List and details: `GET /trips`, `GET /trips/{id}`.

Actions (all under `/trips/{tripId}/…`): `start`, `complete`, waypoint `loading/start`, `loading/complete`, `unloading/start`, `unloading/complete`, `arrive`, `depart`, `customs/enter`, `customs/exit`, `documents/download`, `odometer/suggest`.

### Notifications

| Method | Path |
| --- | --- |
| GET | `/notifications` |
| PATCH | `/notifications/{id}/read` |

### App version

`GET /app-version/check` with query `platform`, `appType`, and `version`. Platform string comes from `AppStoreConfig.versionCheckPlatform` (`Android`, `iOS`, or `HarmonyOS`).

## Models and entities

- **Entities** live under `features/*/domain/entities/` and use `equatable`. They are what BLoCs and screens use. Example: `Trip` in `features/trips/domain/entities/trip.dart`.
- **DTOs** live under `features/*/data/models/` and use `@JsonSerializable`. `build.yaml` sets `explicit_to_json: true`. Generated files are `*.g.dart`.
- Repositories map DTO → entity. Example: `AuthImpl` maps `DriverProfileDto` and normalizes Egyptian phone numbers with a `+2` prefix on the way to the API.

`freezed` and `freezed_annotation` are in `pubspec.yaml`. No `@freezed` class was found under `lib/`. **Needs clarification** whether Freezed is planned or leftover.

Trip status integers used in UI (comments in `trip_details_constants.dart` and `trip_action_buttons.dart`):

| Code | Meaning in those files |
| --- | --- |
| 0 | Requested |
| 1 | Pending |
| 2 | Scheduled |
| 3 | Driver accepted |
| 4 | First pickup loading (`kFirstPickupLoading` in `trip_action_buttons.dart` only) |
| 5 | In progress |
| 6 | Completed |
| 7 | Completed, reviewed |
| 8 | Cancelled |

Waypoint status integers in the same files: 0 pending, 1 arrived, 2 loading started, 3 loading completed, 4 departed, 5 completed, 7 unloading started, 8 unloading completed. Code 6 is not named. **Needs clarification** against the API guide if these comments drift.

## Repositories and services

| Repository | Implementation | Data source |
| --- | --- | --- |
| `AuthRepository` | `AuthImpl` | `AuthRemoteDataSourceImpl` |
| `HomeRepository` | `HomeImpl` | `HomeRemoteDataSourceImpl` |
| `TripsRepository` | `TripsImpl` | `TripsRemoteDataSourceImpl` |
| `NotificationsRepository` | `NotificationsImpl` | `NotificationsRemoteDataSourceImpl` |

Use cases are thin wrappers. `UseCase<T, Params>` and `UseCaseNoParams<T>` in `features/auth/domain/use_cases/usecase.dart` are the shared shapes. Trip actions are grouped in `features/trips/domain/use_cases/trip_actions_usecase.dart`.

Services that are not feature repositories:

| Service | Role |
| --- | --- |
| `FCMService` | Firebase Messaging token, foreground messages, tap handler |
| `firebaseMessagingBackgroundHandler` | Background/killed FCM entry (`fcm_background_handler.dart`) |
| `LocalNotificationService` | Shows notifications while the app is in the foreground |
| `NotificationLaunchHandler` | Holds a route until splash finishes, then navigates |
| `NotificationDeepLinkStore` | Persists an open-trip route when FCM data is missing on tap |
| `BiometricService` | `local_auth` plus credentials in secure storage |
| `DriverLocationBackgroundService` | While online, uploads location about once per minute |
| `AppUpdateService` | Startup `GET /app-version/check` |
| `AppUpdateNotifier` | Pending update captured from login/profile responses |

`DriverLocationBackgroundService` does not keep a location history. It uploads the latest fix through `UpdateLocationUseCase` (`POST /location/ping`). On Android it uses a Geolocator foreground notification.

Odometer photos are checked two ways:

- On device: `validateOdometerImage` in `features/trips/core/odometer_image_validator.dart` (ML Kit text recognition).
- On the server: `POST /trips/{id}/odometer/suggest` via `ValidateOdometerImageUseCase`. The documents UI resolves `TripsRepository` from `getIt` for that call (`trip_step_documents_section.dart`).

`TripStepCaptureStore` saves in-progress photo paths and notes in SharedPreferences so a reload can restore the same capture slot.

## Dependency injection

Composition is mostly **manual** in `_runApp` inside `lib/main.dart`: one Dio from `createDriverApiClient()`, then data sources, repositories, use cases, and blocs.

`get_it` is also initialized by `setup()` in `lib/core/utils/service_locator.dart`. That registers a **second** Dio, a second `TripsRemoteDataSource`, and a second `TripsRepository`. The only `getIt.get` call site found is the odometer suggest flow in `trip_step_documents_section.dart`.

**Why both styles exist:** not documented. New features should follow the `main.dart` graph unless they specifically need the get_it `TripsRepository`.

## Local storage and cache

| Store | What |
| --- | --- |
| `SharedPreferences` via `AuthStorage` | Access token, refresh token, online flag, remember-me, remembered login |
| `FlutterSecureStorage` via `AuthStorage` | Remembered password |
| `FlutterSecureStorage` via `BiometricService` | Biometric login, password, enabled flag |
| `SharedPreferences` | Locale (`app_locale`), trip-step capture snapshots |
| `NotificationDeepLinkStore` | Pending notification route (see that file) |

There is no SQLite, Hive, or Isar usage in `lib/`.

Access and refresh tokens are stored in SharedPreferences, not secure storage. **Why:** not documented.

## Authentication and authorization

Login methods implemented in `AuthBloc`:

1. Password — `LoginUseCase` → `POST /auth/login`.
2. OTP — `SendLoginOtpUseCase` then `LoginWithOtpUseCase`.
3. Biometric — `BiometricService` reads saved login/password, then the password login path (`AuthBiometricLoginRequested`).

MFA: enable channel, send code, verify (`POST /auth/verify-mfa` returns a `LoginResult`). Disable is `DisableMfaUseCase`.

Password reset: forget-password with a channel, then `ResetPasswordWithCodeUseCase`. `ResetPasswordUseCase` is the token-based reset.

Logout calls the API and always clears local tokens afterward.

The router treats “logged in” as “access token string is non-empty”. A 401 that fails refresh clears tokens and opens `/login`.

Biometric enable from settings validates the password with `POST /auth/validate-password` before storing it (`AuthValidatePasswordAndEnableBiometricRequested`).

## Error handling

1. HTTP and envelope errors become `DioException`.
2. `safeApiCall` (`lib/core/network/safe_api_call.dart`) maps them to `Left(Failure)`.
3. `mapDioExceptionToFailure` classifies network timeouts, 401, and other responses. Server message extraction prefers `message`, then `errors`.
4. Feature failures: `AuthFailure` (sealed), `HomeFailure`, `TripsFailure` (includes `TripsFailureMissingFiles`), `NotificationsFailure`.
5. `FailureMessageMapper` turns failures into strings or l10n keys such as `auth.network_error`. Auth screens resolve those keys in `auth_error_extensions.dart`. Trip API messages are localized in `features/trips/core/trip_error_localizer.dart`.

Startup failures for Firebase, local notifications, and FCM are caught and logged with `debugPrint`. The app still launches.

Sentry (`sentry_flutter`) initializes only when all of these are true: release mode, DSN looks valid, and init has not already run. Options in `main()` set `tracesSampleRate = 1.0` and `sendDefaultPii = true`. **Why those options:** not documented.

If the version check fails, the splash still continues to `/home` or `/login`. That behavior is implemented in the splash flow together with `AppUpdateService` (a failed check does not set a forced update).

## Environment and configuration

Configuration is compile-time `--dart-define`, not runtime env files.

| Define | Read in | Default in source |
| --- | --- | --- |
| `DRIVER_API_BASE_URL` | `api_client.dart` | Staging driver API URL |
| `SENTRY_DSN` | `main.dart` | A DSN literal. Do not copy it into new docs or chat logs. |
| `GOOGLE_MAPS_API_KEY` | `google_maps_config.dart` | A key literal used for Geocoding/Directions REST. Native maps also have a key in `AndroidManifest.xml`. |
| `IOS_APP_STORE_ID` | `app_store_config.dart` | Empty |
| `HUAWEI_APP_GALLERY_ID` | `app_store_config.dart` | Empty |
| `APP_STORE_DISTRIBUTION` | `app_store_config.dart` | `play` (`harmonyos` or `appgallery` selects AppGallery) |
| `APP_STORE_URL` | `app_store_config.dart` | Empty (full URL override) |

The `dotenv` package is in `pubspec.yaml` and is not imported anywhere under `lib/`. A `.env` file exists in the repo root and is not listed in `.gitignore`. The app does not load it. **Needs clarification** whether `.env` is still used by a person or script outside Dart.

`ChuckerFlutter.showOnRelease = true` in `main()`, so the HTTP inspector is not limited to debug builds.

Play Store package id in `AppStoreConfig` is `com.loadly.driver`. The Kotlin Android script sets `applicationId` to `com.loadly.loadly_driver`. The Groovy script uses `com.loadly.driver`. Which script the build selects was not verified.

## Main application flow

1. `main()` ensures bindings, sets Chucker to show in release, and registers `FirebaseMessaging.onBackgroundMessage` before other Firebase work.
2. In release, with a valid DSN, `SentryFlutter.init` runs the app. Otherwise `_runApp` runs directly.
3. On Android, Google Maps is initialized with `AndroidMapRenderer.latest` before any map widget. The comment in `main.dart` says this avoids a blank map after the camera or backgrounding.
4. `setup()` registers get_it. `AuthStorage.init()` and `NotificationDeepLinkStore.init()` run. Firebase is initialized; failure is logged.
5. Dio, repositories, use cases, and blocs are constructed. If `AuthStorage.getIsOnline()` is true, background location starts immediately.
6. `AuthCheckSessionRequested` is dispatched.
7. `FCMService` is initialized. The system notification prompt at startup runs only when `Platform.isIOS` (`requestRuntimeNotificationPermission: Platform.isIOS`). On Android, `HomeScreen` shows an in-app disclosure and then calls `FCMService.requestRuntimeNotificationPermission()`. `HomeBloc` also requests `Permission.notification` when the user continues the background-location disclosure. The comment on `FCMService.initialize` says Android 13+ should prompt only after that disclosure.
8. `CupertinoApp.router` starts at splash.
9. Splash waits for a 2 second delay and `AppUpdateService.checkForUpdate()`.
   - Forced update: blocking `AppUpdateScreen`, no navigation.
   - Otherwise: `go` to `/home` or `/login` from `AuthStorage.isLoggedIn()`, then `NotificationLaunchHandler.navigateIfPending`.
   - Optional update: dialog after navigation.
10. Home requests location when the screen is shown (`HomeGetInitialLocation`). Going online calls `/go-online` with coordinates and starts `DriverLocationBackgroundService`.

`_AppLifecycleRefresher` reloads the trip list when the app resumes, unless the trips state is trip details, details loading, or an action in progress. It also asks `NotificationLaunchHandler` to consume a platform notification launch.

## How features communicate

Blocs do not call each other. `main.dart` and a few screens bridge them:

- Any foreground FCM message calls `tripsBloc.add(TripsLoadTrips(...))`.
- Notification taps go through `NotificationLaunchHandler` (trip route, schedule update, or generic). Schedule updates are applied on `HomeScreen` as `HomeTripScheduleUpdateReceived`.
- Local notification taps in `main.dart` currently `router.go('/trips')` for both schedule and generic payloads.
- Profile and settings both use `ProfileBloc` and `AuthBloc`. Settings reads `BiometricService` directly for device capability.
- After login or profile load, `presentPendingAppUpdate` / `handleLoginNavigationWithAppUpdate` in `lib/core/widgets/app_update_flow.dart` show an update captured by `AppUpdateInterceptor`.
- The bottom bar reads `HomeBloc` for online state and `NotificationsBloc` for unread state.

## Third-party packages

| Package | Use in this project |
| --- | --- |
| `flutter_bloc` | Feature and locale state |
| `go_router` | Routes and auth redirect |
| `dio` | HTTP |
| `dartz` | `Either` results |
| `equatable` | Entities, events, failures |
| `json_annotation` / `json_serializable` | DTO JSON |
| `get_it` | Second Dio and `TripsRepository` only |
| `shared_preferences` | Tokens, locale, flags, capture drafts |
| `flutter_secure_storage` | Passwords and biometric secrets |
| `firebase_core`, `firebase_messaging` | Push |
| `flutter_local_notifications` | Foreground display |
| `geolocator` | Permission, stream, background location |
| `permission_handler` | Permission flows in `HomeBloc` |
| `google_maps_flutter` | Map |
| `google_polyline_algorithm` | Decode route polylines on trip details |
| `google_mlkit_text_recognition` | On-device odometer OCR |
| `image_picker` | Camera/gallery for profile and trip documents |
| `local_auth` | Biometrics |
| `sentry_flutter` | Release crash/perf reporting |
| `chucker_flutter` | HTTP inspector |
| `google_fonts` | Manrope (English) and Almarai (Arabic) |
| `flutter_localizations`, `intl` | Generated l10n |
| `url_launcher` | Store links, maps, documents |
| `device_info_plus`, `package_info_plus` | Device headers and version check |
| `flutter_sficon` | Icons on the login screen |
| `cupertino_icons` | Cupertino icons |

Declared in `pubspec.yaml` with **no import under `lib/`** at the time of this audit: `dotenv`, `dotted_border`, `location`, `freezed` / `freezed_annotation` (dev + runtime annotation). **Needs clarification** before removing them.

## Localization

- Config: `l10n.yaml` (`arb-dir: lib/l10n`, template `app_en.arb`, output `lib/l10n/gen`).
- `flutter: generate: true` in `pubspec.yaml`.
- Supported locales are whatever `AppLocalizations.supportedLocales` generates from the ARB files (English and Arabic).
- Default locale when nothing is stored: `ar` (`LocaleBloc`, `LoadlyDriverApp`, `LanguageInterceptor`).
- Theme: `buildAppTheme` uses Almarai for `ar` and Manrope otherwise. Comment in `app_theme.dart`: “Loadly design system theme.”

## Testing structure

`fvm flutter test` runs `test/`.

| File | What it covers |
| --- | --- |
| `test/widget_test.dart` | Pumps `LoadlyDriverApp` with a hand-built `AuthBloc` and `LocaleBloc` |
| `test/app_version_check_resolver_test.dart` | Force vs optional update rules |
| `test/app_version_envelope_parser_test.dart` | Version fields inside API envelopes |
| `test/core/services/notification_launch_handler_test.dart` | Pending notification routes |
| `test/core/services/local_notification_service_test.dart` | Local notification helpers |
| `test/core/utils/notification_click_action_test.dart` | Tap payload → route |
| `test/core/utils/google_reverse_geocoding_test.dart` | Address formatting from Geocoding JSON |
| `test/core/location/current_position_reader_test.dart` | Position reader |
| `test/features/trips/trip_step_capture_store_test.dart` | Capture snapshot persistence |

There is no `integration_test/` directory. BLoCs and repositories are not covered by a dedicated test folder.

## Build and deployment

- Analyzer: `analysis_options.yaml` includes `package:flutter_lints/flutter.yaml` and adds `prefer_const_constructors` and `prefer_relative_imports`.
- Icons: `flutter_launcher_icons` from `assets/images/driver_icon.png`.
- Splash: `flutter_native_splash` is a white screen with no image (comment: Flutter splash shows the logo).
- Sentry project/org in `pubspec.yaml`: `loadly-loadly-driver` / `fuelin-wr`. Symbol upload uses `sentry_dart_plugin`. `sentry.properties` is gitignored.
- Android Gradle wrapper: Gradle 8.14 (`android/gradle/wrapper/gradle-wrapper.properties`). Java compatibility in `android/app/build.gradle.kts` is 17.
- CI (`.github/workflows/build-apk.yml`): on push to `main` and `workflow_dispatch`. Installs FVM from `.fvmrc`, runs `fvm flutter build apk --release` with **no** `--dart-define` flags, uploads the APK to S3 from a self-hosted job, emails via SES, and posts to Teams. Artifact retention is 1 day. The presigned URL lifetime in the script is 604800 seconds (7 days).

Release signing in `android/app/build.gradle.kts` uses the debug keystore. The comment in that file says this is so `flutter run --release` works.

### Platforms

Android and iOS are the targets the manifest, entitlements, and CI are written for.

- Android application id in `android/app/build.gradle.kts`: `com.loadly.loadly_driver`.
- Android application id in `android/app/build.gradle`: `com.loadly.driver`.
- iOS bundle id in `ios/Runner.xcodeproj`: `com.loadly.driver`.
- Both Groovy and Kotlin Gradle settings exist, with different Kotlin plugin versions (`2.3.10` in `settings.gradle`, `2.2.20` in `settings.gradle.kts`). Which file Gradle selects was **not verified** in this audit.

`lib/main.dart` imports `dart:io`. That prevents a standard Flutter web build of this entrypoint. Web, macOS, Linux, and Windows folders exist. **Needs clarification** which of those are supported.

## README drift

`README.md` is useful but older than parts of the code. Prefer this file when they disagree. Known mismatches:

- Password login route is `/auth/login`, not `/password-login`.
- Location upload path is `/location/ping`, not `/location/update`.
- Trip API is more than list and details (step actions, customs, odometer suggest).
- 401 tries refresh before logout.
- Tokens are in SharedPreferences; passwords are in secure storage.
- Chucker is explicitly enabled for release.
- Dependency injection is manual in `main.dart` plus a small get_it graph.
