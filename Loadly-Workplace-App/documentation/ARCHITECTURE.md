# Architecture

Loadly Workplace (`loadly_workplace`, version `1.1.2+6` in `pubspec.yaml`) is a Flutter app for collectors. Collectors sign in, browse trips, filter them, and collect or confirm trip documents. A user whose login permissions include `confirm_trip_documents` is treated as a financial collector and sees extra document statuses and confirm actions.

The UI is Cupertino-first (`CupertinoApp.router` in `LoadlyWorkplaceApp`). The window title and the Android and iOS display names are `Loadly Workplace`.

## Overall architecture

Features follow a lightweight clean architecture:

```text
Presentation (views, widgets, BLoC)
        │
        ▼
Domain (entities, use cases, repository interfaces, failures)
        │
        ▼
Data (repository implementations, remote data sources, DTOs, mappers)
        │
        ▼
Dio  →  collector API
```

`lib/core/` holds shared infrastructure: HTTP, routing, storage, theme, localization, and services.

Why this split: domain interfaces (`AuthRepository`, `HomeRepository`, `TripsRepository`) do not depend on Dio. Data implementations (`AuthImpl`, `HomeImpl`, `TripsImpl`) translate HTTP into `Either<Failure, T>` via `safeApiCall`. Presentation talks to use cases, not to Dio.

Exceptions to the pattern:

- `profile` has only a presentation layer. `ProfileBloc` calls `GetProfileUseCase` from `features/auth`.
- Trip list data lives in `features/home`. `features/trips` is the write side (collect / confirm). Trip details UI is `features/home/presentation/views/trip_details_screen.dart`.
- There is no `features/notifications` or `features/settings` module. Push notifications navigate to `/home`.

## Folder structure

```text
lib/
├── main.dart                 # startup, manual DI, Firebase, Sentry
├── core/
│   ├── config/               # store URLs and dart-define overrides
│   ├── errors/               # FailureMessageMapper
│   ├── extensions/
│   ├── locale/bloc/          # LocaleBloc
│   ├── models/               # app-update DTOs
│   ├── network/              # Dio client, endpoints, error mapping
│   ├── permissions/          # in-app permission disclosure dialog
│   ├── router/               # GoRouter
│   ├── services/             # FCM, biometrics, app update, temp files
│   ├── storage/              # AuthStorage
│   ├── theme/
│   ├── utils/
│   └── widgets/              # shared buttons, scaffold, update UI
├── features/
│   ├── auth/                 # data + domain + presentation
│   ├── home/                 # trip list, filters, trip details
│   ├── profile/              # presentation only
│   └── trips/                # collect/confirm data + domain + BLoC
└── l10n/                     # app_en.arb, app_ar.arb, generated gen/
assets/images/                # logo, icons, background
documentation/                # ARCHITECTURE, DEVELOPMENT, TROUBLESHOOTING
test/widget_test.dart         # empty placeholder
android/, ios/, web/, macos/, linux/, windows/
.github/workflows/build-apk.yml
```

`features/auth/auth.dart` is a barrel export. Other features do not have one.

## State management

[flutter_bloc](https://pub.dev/packages/flutter_bloc) with `Equatable` events and states. BLoCs are created once in `main.dart` and provided app-wide with `MultiBlocProvider`.

| BLoC | File | Responsibility |
| --- | --- | --- |
| `AuthBloc` | `features/auth/presentation/bloc/auth_bloc.dart` | Login, logout, MFA, password reset, biometrics, session check |
| `HomeBloc` | `features/home/presentation/bloc/home_bloc.dart` | Load trips and document types. Also holds in-memory `FilterData` |
| `TripsBloc` | `features/trips/presentation/bloc/trips_bloc.dart` | Collect, confirm, and bulk variants |
| `ProfileBloc` | `features/profile/presentation/bloc/profile_bloc.dart` | Load collector profile |
| `LocaleBloc` | `core/locale/bloc/locale_bloc.dart` | Arabic/English, persisted in SharedPreferences |

Typical flow:

```text
Widget → Bloc event → use case → repository → remote data source → Dio
                                                              │
Widget ← Bloc state ← Either<Failure, T> ←───────────────────┘
```

`HomeBloc`’s class comment still says it manages online/offline and location. The handlers only implement `HomeLoadTrips` and `HomeLoadDocuments`.

## Navigation

[go_router](https://pub.dev/packages/go_router) in `lib/core/router/app_router.dart`. `createAppRouter()` stores the router on `AppRouterHolder` so interceptors and FCM can call `router.go` without a `BuildContext`.

`LoadlyWorkplaceApp.build` calls `createAppRouter()` inside a `BlocBuilder<LocaleBloc, LocaleState>`. A locale change rebuilds the app and constructs a new `GoRouter`. Why that is done is not commented. **Needs clarification** whether replacing the router on language change is intentional.

### Routes

| Path | Screen | Notes |
| --- | --- | --- |
| `/` | `_SplashScreen` | Waits 2 seconds and for an update check, then `/home` or `/auth/login` |
| `/auth/login` | `PasswordLoginScreen` | |
| `/otp` | `OtpScreen` | Query: `contact`, `viaSms`, `forgetPassword`, `isMfa` |
| `/home` | `HomeScreen` | `isFinancialCollector` from `AuthStorage` |
| `/profile` | `ProfileScreen` | |
| `/filter` | `FilterScreen` | Requires `state.extra` as `FilterData` |
| `/trips/:tripId` | `TripDetailsScreen` | Requires `state.extra` as `CollectorTrips` |
| `/trips` | `TripsListScreen` | Registered. No Dart code navigates here. The trips tab in `AppBottomNavBar` is commented out |

Redirects use `AuthStorage.instance.isLoggedIn()` (refresh token present):

- Logged-in users hitting `/auth/login` or `/otp` are sent to `/home`.
- Logged-out users hitting any other path are sent to `/auth/login`.
- `/` is allowed for both so the splash can run.

`AuthScreenGuard` is set by `OtpScreen`, but `AuthInterceptor` does not read it. **Needs clarification** whether 401 handling on the OTP screen was meant to use that flag.

Bottom navigation (`MainScaffold` + `AppBottomNavBar`) has two items: Home (`/home`) and Profile (`/profile`).

## API and networking

`createWorkplaceApiClient()` in `lib/core/network/api_client.dart` builds one shared `Dio` instance.

Base URL (`kWorkplaceApiBaseUrl`):

```dart
String.fromEnvironment(
  'WORKPLACE_API_BASE_URL',
  defaultValue: 'https://stg-api-agg.loadly-eg.com/api/collector',
);
```

`kApiBaseUrl` strips a trailing `/collector` so version check can call the shared `/api` root.

Timeouts: connect and receive, 30 seconds. Default headers: `Accept` and `Content-Type` `application/json`.

Interceptor order:

1. `DeviceInterceptor` — `device-id`, `device-model`, `device-os`, `device-os-version`, `app-version`, `app-build-number`
2. `LanguageInterceptor` — `lang` from SharedPreferences key `app_locale` (default `ar`)
3. `FcmTokenInterceptor` — `fcmToken` when a token exists
4. `AuthInterceptor` — `Authorization: Bearer <access token>`; on 401 tries refresh
5. `AppUpdateInterceptor` — reads version policy from login, verify-MFA, and GET profile envelopes
6. `ChuckerDioInterceptor` — HTTP inspector. `main.dart` sets `ChuckerFlutter.showOnRelease = true`

Successful JSON bodies are treated as an envelope: `{ success, message, data, errors }`. Helpers live in `api_response_parser.dart`. List endpoints for trips return `data` plus `pagination` and are parsed in the data source, not through `requireEnvelopeDataMap`.

### Endpoints (`ApiEndpoints`)

| Method | Path | Used by |
| --- | --- | --- |
| POST | `/auth/login` | `AuthRemoteDataSourceImpl.login` |
| POST | `/auth/logout` | `logout` |
| GET | `/auth/profile` | `getProfile` |
| POST | `/auth/forget-password` | `forgetPassword` |
| POST | `/auth/reset-password` | `resetPassword` |
| POST | `/auth/reset-password-with-code` | `resetPasswordWithCode` |
| POST | `/auth/send-mfa-code` | `sendMfaCode` |
| POST | `/auth/verify-mfa` | `verifyMfa` |
| POST | `/auth/refresh-token` | `AuthStorage.refreshToken` (separate Dio, no interceptors) |
| GET | `/trips` | `HomeRemoteDataSourceImpl.getCollectorTrips` |
| GET | `/documents` | `getDocuments` (`pageSize=100`, `entityId=5`) |
| POST | `/trips/{id}/documents/collect` | multipart collect |
| POST | `/trips/{id}/documents/confirm` | multipart confirm |
| POST | `/trips/documents/bulk-collect` | JSON bulk collect |
| POST | `/trips/documents/bulk-confirm` | JSON bulk confirm |
| GET | `{api root}/app-version/check` | `AppUpdateService` (`platform`, `appType=Collector`, `version`) |

`HomeScreen.loadTrips` always sends `status: 6`. The meaning of `6` is not named in code. **Needs clarification.**

`entityId: 5` on the documents request is also unnamed. **Needs clarification.**

The interceptor still lists paths such as `/auth/send-login-otp` and `/auth/enable-mfa`. Those paths are not in `ApiEndpoints` and have no data-source methods.

## Models and entities

Domain entities are hand-written `Equatable` classes. DTOs use `json_serializable` (`build.yaml` sets `explicit_to_json: true`). `freezed` is in `pubspec.yaml`, but no `@freezed` class exists in `lib/`.

| Type | Location | Role |
| --- | --- | --- |
| `LoginResult` | `features/auth/domain/entities/login_result.dart` | Tokens, `redirect`, MFA channel, permissions |
| `CollectorProfile` | `features/auth/domain/entities/collector_profile.dart` | Profile shown on the profile screen |
| `CollectorTrips`, `CollectorTripsList` | `features/home/domain/entities/collector_trips.dart` | Trip, company, vehicle, driver, waypoints, documents, pagination. Includes `hasCustoms` and `customsStatus` |
| `Document` | `features/home/domain/entities/document.dart` | Document type catalog |
| `LoginRequestDto`, `LoginResponseDto`, MFA DTOs | `features/auth/data/models/` | Auth JSON |
| `CollectorTripsDto`, `DocumentDto` | `features/home/data/models/` | Trip and document JSON |
| `CollectTripDocumentsRequestDto` | `features/trips/data/models/` | Multipart collect/confirm body |
| `BulkCollectTripDocumentsRequestDto` | `features/trips/data/models/` | Bulk JSON body |
| `AppVersionCheckDto`, `AppUpdateInfo` | `core/models/` | Update policy |

There are two unrelated classes named `CollectorDocuments`: the domain entity in `collector_trips.dart`, and the form-data wrapper in `collect_trip_documents_request_dto.dart`.

Mappers: `collector_trips_mapper.dart`, `document_mapper.dart`. Profile mapping is inline in `AuthImpl`.

`PermissionsExtension.hasPermission` (`core/extensions/permission_search.dart`) treats a permission map as granted when any list value contains the permission string.

## Repositories and services

| Piece | Role |
| --- | --- |
| `AuthRemoteDataSource` / `AuthImpl` | Auth HTTP and `AuthFailure` mapping |
| `HomeRemoteDataSource` / `HomeImpl` | Trips query and document catalog |
| `TripsRemoteDataSource` / `TripsImpl` | Collect and confirm |
| `FCMService` | Singleton. Token, foreground messages, tap handler |
| `LocalNotificationService` | Shows FCM messages while the app is in the foreground. Channel id `general_notifications` |
| `BiometricService` | `local_auth` plus credentials in `FlutterSecureStorage` |
| `AppUpdateService` | GET version check. Failures are logged and return `null` (startup continues) |
| `TempFileService` | On startup, deletes image files in the temp directory |
| `AuthStorage` | Tokens, financial-collector flag, remembered login |

Use cases are thin wrappers around repositories. The base types are `UseCase<T, Params>` and `UseCaseNoParams<T>` in `features/auth/domain/use_cases/usecase.dart`. Home and trips use cases follow the same shape but do not all share that file.

## Dependency injection

There is no service locator (`get_it` is not a dependency). `main.dart` `_runApp` constructs Dio, data sources, repositories, use cases, and BLoCs by hand and passes them through constructors.

Why: the graph is small and visible in one function. The cost is that tests and new screens cannot replace a dependency without editing `main.dart` or constructing BLoCs themselves. No test does that today.

## Local storage

No SQL database, Hive, or Drift package is in `pubspec.yaml`.

| Store | What | Keys (selected) |
| --- | --- | --- |
| `SharedPreferences` | Access token, refresh token, financial-collector flag, remember-me, remembered login, locale | `auth_access_token`, `auth_refresh_token`, `auth_is_financial_collector`, `app_locale` |
| `FlutterSecureStorage` | Remembered password, last auth password, biometric login/password/enabled flag | `auth_remembered_password`, `auth_password`, `biometric_*` |

`isLoggedIn()` is true when a non-empty refresh token is stored. Access tokens are not in secure storage. Why tokens are in SharedPreferences is not commented.

`HomeBloc.filterData` is memory only and is lost when the process dies.

Device and package info are cached in static fields on `DeviceInterceptor` for the process lifetime.

## Authentication and authorization

Login (`AuthLoginRequested`):

1. `LoginUseCase` → `POST /auth/login`.
2. On success, `AuthBloc` saves access and refresh tokens.
3. If `permissions` contains `confirm_trip_documents`, `saveisFinancialCollector(true)`. The same check runs after MFA verify and after biometric login. The flag changes default trip-document filters and which actions the trip card shows.
4. The password is written to secure storage (`saveauthPassword`).
5. Navigation in `PasswordLoginScreen` depends on `LoginResult.redirect` and `mfaChannel`:
   - `redirect == 'mfa'`: send MFA code with channel `2` (SMS, per comments in the login and OTP screens) and open `/otp?...&isMfa=true`.
   - `redirect == 'forget_password'`: open `/otp` with `forgetPassword=true`.
   - `mfaChannel == 0`: go to `/home`. The code comment calls this the super-admin path.
   - Otherwise: same MFA SMS path as `redirect == 'mfa'`.

Channel numbers used in the UI: `1` email, `2` SMS (`password_login_screen.dart`, `otp_screen.dart`).

Forgot password posts the identifier and channel, then opens the OTP screen. Reset uses `ResetPasswordWithCodeUseCase`. `ResetPasswordUseCase` (token-based) is wired in `AuthBloc` but this audit did not find a screen that dispatches `AuthResetPasswordRequested`. **Needs clarification** if that flow is still required.

Biometrics: `AuthBiometricLoginRequested` reads credentials from `BiometricService` and runs the password login use case. That path hard-codes `locale` to `'en'` (`auth_bloc.dart`). Enable/disable is handled by `AuthBloc` and the profile screen. The login screen checks `isAvailable()` and `isBiometricEnabled()`.

Session: startup dispatches `AuthCheckSessionRequested`, which only checks the local refresh token. It does not call the profile API. Splash navigation uses the same local check.

Logout: `POST /auth/logout`, then `clearTokens()` even if the API fails. `clearTokens()` removes tokens, the financial-collector flag, and the secure auth password. It does not clear remember-me or biometric keys.

401: `AuthInterceptor` calls `AuthStorage.refreshToken()`. If refresh returns tokens, the original request is retried with a **new** `Dio` that has no interceptors. If refresh fails, tokens are cleared and the router goes to `/auth/login`, except for a hard-coded list of auth attempt paths. Queued requests during refresh are retried but their responses are not delivered to the original callers.

## Error handling

`safeApiCall` catches `DioException` and maps it with `mapDioExceptionToFailure`:

| Condition | `ApiFailureKind` |
| --- | --- |
| HTTP 401 | `unauthorized` |
| Timeout or connection error | `network` |
| Anything else, including other HTTP statuses | `server` |

Repositories map that into feature failures:

- `AuthFailureNetwork`, `AuthFailureInvalidCredentials` (login 401 only), `AuthFailureServer`, `AuthFailureUnknown`
- `HomeFailureNetwork`, `HomeFailureServer`, `HomeFailureUnknown`
- `TripsFailureNetwork`, `TripsFailureServer`, `TripsFailureUnknown`

`FailureMessageMapper` turns failures into English strings. Several BLoC branches still inline their own English messages instead of using the mapper (`HomeBloc`, `TripsBloc`). Localization of those strings is therefore inconsistent. API `message` / `errors` text is shown when present.

Firebase, FCM, local notifications, and the version check are wrapped in try/catch in `main` or the service. Failures are `debugPrint`ed and the app continues.

Sentry (`sentry_flutter`) initializes only when all of these are true: release mode (`kReleaseMode`), a DSN that contains `sentry.io`, and init has not already run. `tracesSampleRate` is `1.0`. `sendDefaultPii` is `true`. The DSN comes from `--dart-define=SENTRY_DSN=...`, with a default compiled into `main.dart`. Do not commit additional DSNs into docs or chat logs. `pubspec.yaml` `sentry:` uploads debug symbols for project `loadly-loadly-driver`, org `fuelin-wr`.

## Configuration

All environment switches found in code are compile-time `--dart-define` values:

| Define | Where | Default in code |
| --- | --- | --- |
| `WORKPLACE_API_BASE_URL` | `api_client.dart` | `https://stg-api-agg.loadly-eg.com/api/collector` |
| `SENTRY_DSN` | `main.dart` | A DSN string in source. Override in real builds |
| `IOS_APP_STORE_ID` | `AppStoreConfig` | empty |
| `HUAWEI_APP_GALLERY_ID` | `AppStoreConfig` | empty |
| `APP_STORE_DISTRIBUTION` | `AppStoreConfig` | `play` (`harmonyos` or `appgallery` selects Huawei) |
| `APP_STORE_URL` | `AppStoreConfig` | empty; otherwise Play, App Store, or AppGallery URL is built |

There is no `.env` loader. Firebase is `Firebase.initializeApp()` with no `firebase_options.dart`. Native config is `android/app/google-services.json` and `ios/Runner/GoogleService-Info.plist`.

Android application id and iOS bundle id: `com.loadly.workplace`.

## Main application flow

```text
main()
  ├─ TempFileService.cleanOnStartup()
  ├─ (release + valid DSN) SentryFlutter.init
  └─ _runApp()
       ├─ Firebase.initializeApp()
       ├─ AuthStorage.init()
       ├─ createWorkplaceApiClient(); AppUpdateService.init
       ├─ construct repositories, use cases, BLoCs
       ├─ LocalNotificationService + FCMService
       ├─ AuthCheckSessionRequested
       └─ runApp(MultiBlocProvider → LoadlyWorkplaceApp)

Splash (/)
  ├─ AppUpdateService.checkForUpdate() in parallel with a 2s delay
  ├─ forced update → full-screen update route (does not continue)
  └─ else go /home or /auth/login, then optional update dialog
```

Notification tap (foreground, background, or a cold start after 2.5s) calls `router.go('/home')`. `FCMService.setOnNotificationReceived` is passed an empty callback, so a push does not refresh the trip list.

### How features talk to each other

- **Auth → Home:** login stores the financial-collector flag. `HomeScreen` and `FilterScreen` read it from `AuthStorage` when the route is built.
- **Home → Trips:** trip cards and details dispatch `TripsBloc` events (`TripsCollectDocuments`, `TripsConfirmDocuments`, bulk events). `TripsBloc` does not load trips. On the home screen, a successful bulk collect or confirm calls `loadTrips()`. Trip details shows a dialog on `CollectDocumentsDone` / `ConfirmDocumentsDone` and does not dispatch `HomeLoadTrips`. Leaving details uses `context.go('/home')`, and `HomeScreen.initState` loads trips when that route is built. Trip details loads the document catalog with `HomeLoadDocuments`.
- **Home filter:** `FilterData` is a mutable field on the singleton `HomeBloc`. `/filter` receives it as `extra` and writes back into the same object.
- **Profile → Auth:** profile loads through auth’s `GetProfileUseCase`. Logout is `AuthLogoutRequested`. Language changes go through `LocaleBloc`, which the `lang` header reads from the same SharedPreferences key.
- **App update:** splash calls `AppUpdateService`. `AppUpdateInterceptor` can also stash a pending update from login/profile via `AppUpdateNotifier`. The profile screen uses `app_update_flow.dart`.

### Collector vs financial collector

| | Factory collector (flag false) | Financial collector (flag true) |
| --- | --- | --- |
| How it is set | Login permissions do not include `confirm_trip_documents` | Permissions include `confirm_trip_documents` |
| Default `tripDocumentStatuses` | `[1, 2]` | `[1, 2, 3, 4]` |
| Filter radios | 1 no documents collected, 2 partially collected | Also 3 collected by factory collector, 4 partially confirmed |

Status integers are the API values sent as `tripDocumentStatuses`. Labels come from ARB keys in `filter_screen.dart`.

On trip details, the document catalog is filtered by the English `nameEn` string (`trip_details_screen.dart`). If `hasCustoms` is true, `Fuel Receipts` is omitted. If `hasCustoms` is false or null, both `Customs Documents` and `Fuel Receipts` are omitted. Why those names are excluded is not commented.

## Third-party packages

| Package | Why it is in the project |
| --- | --- |
| `flutter_bloc`, `equatable` | Events, states, value equality |
| `go_router` | Declarative routes and redirects |
| `dio` | HTTP client and interceptors |
| `dartz` | `Either` for repository results |
| `json_annotation` / `json_serializable` / `build_runner` | DTO `fromJson` / `toJson` |
| `firebase_core`, `firebase_messaging` | Push messaging |
| `flutter_local_notifications` | Foreground notification display |
| `flutter_secure_storage` | Passwords and biometric secrets |
| `shared_preferences` | Tokens, flags, locale |
| `local_auth` | Biometric prompt |
| `image_picker`, `permission_handler` | Camera/gallery photos on received-document popups |
| `sentry_flutter`, `sentry_dart_plugin` | Release crash reporting and symbol upload |
| `chucker_flutter` | In-app HTTP log, including release |
| `google_fonts` | Manrope (English) and Almarai (Arabic) |
| `pull_to_refresh`, `number_pagination` | Trip list refresh and pages |
| `url_launcher` | Open the store listing from the update prompt |
| `package_info_plus`, `device_info_plus` | Version and device headers |
| `intl`, `flutter_localizations` | Generated l10n |
| `path_provider` | Temp image cleanup |
| `dotted_border`, `flutter_spinkit`, `slide_countdown`, `flutter_sficon` | UI details |
| `flutter_native_splash`, `flutter_launcher_icons` | Splash color and launcher icons (dev)

`google_maps_flutter` and `geolocator` are **not** dependencies. `AndroidManifest.xml` still contains a Google Maps API key meta-data entry, and `Info.plist` still has location usage strings. No Dart file imports a maps or location plugin. **Needs clarification** whether that native config is leftover.

## Localization

- ARB: `lib/l10n/app_en.arb` (template), `lib/l10n/app_ar.arb`
- Config: `l10n.yaml` → `lib/l10n/gen/`
- `flutter: generate: true` in `pubspec.yaml`
- Default locale: `ar` (`LocaleBloc` and `LanguageInterceptor`)
- Theme: `buildAppTheme` switches Google font by language code

Usage: `AppLocalizations.of(context)!`.

## Testing

`test/widget_test.dart` contains only `void main() {}`. There are no unit tests for BLoCs, repositories, or use cases. iOS and macOS have placeholder `RunnerTests` Swift targets; this audit did not treat them as app tests.

`analysis_options.yaml` includes `package:flutter_lints/flutter.yaml` and enables `prefer_const_constructors` and `prefer_relative_imports`.

`sonar-project.properties` points Sonar at `lib/`, excludes generated files, and sets `sonar.projectVersion=1.0.4`, which does not match `pubspec.yaml` `1.1.2+6`.

## Build and deployment

| Item | Source |
| --- | --- |
| Flutter | `.fvmrc` → `3.41.2`. `.fvm/` is gitignored. VS Code SDK path is `.fvm/flutter_sdk` |
| Dart constraint | `sdk: ^3.8.0` |
| Android | `applicationId` `com.loadly.workplace`. `compileSdk` / `minSdk` / `targetSdk` use Flutter’s values. Java/Kotlin 1.8 in `android/app/build.gradle`. AGP `8.11.1`, Kotlin `2.3.10`, Gradle `8.14`. Core library desugaring is on. Google services plugin is applied |
| Release signing | If `android/key.properties` exists, that keystore is used. Otherwise release uses the debug signing config (comment in `build.gradle`) |
| iOS | Bundle id `com.loadly.workplace`. `ios/Podfile` platform `15.5`. Push background mode `remote-notification` |
| CI | `.github/workflows/build-apk.yml` on push to `main` and `workflow_dispatch`. Ubuntu runner installs FVM, builds `flutter build apk --release`, uploads the APK to S3 from a self-hosted job, can email via SES, and posts a Teams card. The workflow does not pass `--dart-define`, so the APK uses the in-code API and Sentry defaults |
| Icons / splash | `flutter_launcher_icons` uses `assets/images/workplace_icon.png`. Native splash is a white screen (`flutter_native_splash`) |

Platform folders exist for web, macOS, Windows, and Linux. `lib/main.dart` and `lib/core/network/api_client.dart` import `dart:io`, so a web build of the current entrypoint is not supported. CI only builds an Android APK. **Needs clarification** which platforms are still release targets.
