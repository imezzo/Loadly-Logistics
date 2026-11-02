# Architecture

How **Loadly Contractor** (`contractor_app`) is structured in this repository. Project overview is in [README.md](../README.md). Setup commands live in [DEVELOPMENT.md](DEVELOPMENT.md). Runtime problems live in [TROUBLESHOOTING.md](TROUBLESHOOTING.md).

Anything that cannot be confirmed from the code is marked **Needs clarification**.

## Overall architecture

The app is a Flutter client for the contractor HTTP API. UI is Cupertino (`CupertinoApp.router` in `lib/main.dart`), Arabic-first, with English as the second locale.

Layers:

1. **Presentation** — screens, widgets, and `flutter_bloc` BLoCs.
2. **Domain** — entities, repository contracts, and use cases that return `Either` from `dartz`.
3. **Data** — DTOs, remote data sources, mappers, and repository implementations.
4. **Core** — Dio client, routing, storage, theme, shared widgets, and cross-cutting services.

**Why this split:** domain code depends on repository interfaces, and data code implements them. Presentation talks to use cases, not to Dio. That boundary is followed for `auth`, `trips`, `drivers`, and `notifications`. It is **not** uniform:

| Feature | What exists |
| --- | --- |
| `auth`, `trips`, `drivers`, `notifications` | presentation + domain + data |
| `profile` | presentation only; profile use cases live under `features/auth` |
| `home`, `requests`, `settings`, `marketplce` | presentation only; they call other features' BLoCs |

The folder `lib/features/marketplce` is spelled that way in source (missing an `a`).

There is no `get_it` / `injectable` graph. `lib/main.dart` (`_runApp`) is the composition root: it builds one Dio client, repositories, use cases, and BLoCs, then provides them with `MultiBlocProvider`.

## Folder structure

```text
lib/
├── main.dart                 # startup, DI, Sentry, root widget
├── l10n/                     # app_en.arb, app_ar.arb, generated gen/
├── core/
│   ├── config/               # store URLs (AppStoreConfig)
│   ├── errors/               # Failure, message mapping, fallback keys
│   ├── locale/               # LocaleBloc
│   ├── models/               # app-update DTOs
│   ├── network/              # Dio, endpoints, interceptors, envelope parsing
│   ├── permissions/          # disclosure before camera/storage permission
│   ├── router/               # GoRouter, path constants
│   ├── services/             # app update, biometrics, FCM stub
│   ├── storage/              # AuthStorage
│   ├── theme/
│   ├── usecase/              # UseCase / UseCaseNoParams
│   ├── utils/
│   └── widgets/
└── features/
    ├── auth/
    ├── drivers/
    ├── home/
    ├── marketplce/
    ├── notifications/
    ├── profile/
    ├── requests/
    ├── settings/
    └── trips/
```

Other roots:

| Path | Role |
| --- | --- |
| `assets/images/`, `assets/fonts/` | UI images and the Zain font family (`pubspec.yaml`) |
| `android/`, `ios/` | Primary hosts. Android `applicationId` is `com.loadly.contractor` |
| `linux/`, `macos/`, `web/`, `windows/` | Default Flutter platform folders. Product behavior is built around Android and iOS device APIs |
| `.github/workflows/build-apk.yml` | Release APK on push to `main` |
| `.fvmrc` | Flutter `3.47.2` |
| `l10n.yaml` | ARB → `lib/l10n/gen` |
| `analysis_options.yaml` | `flutter_lints` plus `prefer_const_constructors` and `prefer_relative_imports` |

`test/` has no Dart tests. `CHANGELOG.md` is not in the repository.

## State management

`flutter_bloc`. BLoCs are created once in `_runApp` and live for the process:

| BLoC | File | Role |
| --- | --- | --- |
| `AuthBloc` | `features/auth/presentation/bloc/auth_bloc.dart` | Login, OTP, MFA, register, password, biometric, logout, session check |
| `LocaleBloc` | `core/locale/bloc/locale_bloc.dart` | Persisted `ar` / `en`; loads itself in the constructor |
| `ProfileBloc` | `features/profile/presentation/bloc/profile_bloc.dart` | Load/update profile via auth use cases |
| `NotificationsBloc` | `features/notifications/presentation/bloc/` | Inbox and mark-as-read |
| `TripBloc` | `features/trips/presentation/bloc/trip/` | Trip lists, detail, book, withdraw; in-memory `cachedTrips` |
| `TripStatisticsBloc` | `features/trips/presentation/bloc/trip_statistics/` | Invitation, status, and payout statistics |
| `BookingRequestBloc` | `features/trips/presentation/bloc/booking_request/` | Booking-request lists |
| `DriverBloc` | `features/drivers/presentation/bloc/driver_bloc.dart` | Drivers, document types, create driver, assign driver |

`GetDefaultVehicleTypesUseCase` is the one non-BLoC dependency exposed to the tree, via `RepositoryProvider` in `main.dart`. `RequestsScreen` reads it for the vehicle-type filter.

Events are added from screens (`context.read<TripBloc>().add(...)`). States are plain classes (not `freezed`). `freezed` / `freezed_annotation` are in `pubspec.yaml` but no `@freezed` types were found under `lib/`.

**Why app-scoped BLoCs:** screens under different routes share the same trip, auth, and driver state without a navigation-scoped provider. `HomeScreen` and `RequestsScreen` both dispatch `GetTrips` on the same `TripBloc`, so a list load on one screen replaces that bloc's list state for the other.

## Navigation and routing

`go_router`, built by `createAppRouter()` in `lib/core/router/app_router.dart`. Paths are `AppRoutePaths` in `lib/core/router/route_constants.dart`.

`LoadlyContractorApp` passes `routerConfig: createAppRouter()` inside a `BlocBuilder<LocaleBloc, LocaleState>`. A new `GoRouter` is created whenever that builder runs (including when the locale changes).

Initial route is `/` (`_SplashScreen` in the same file):

1. Waits at least 3.5 seconds and for `AppUpdateService.checkForUpdate()`.
2. If `updateInfo.isForced` is true, shows the blocking update UI and does not continue.
3. Otherwise `go` to `/home` when `AuthStorage.instance.isLoggedIn()`, else `/onboarding`.
4. A non-forced update is shown after navigation, using `ChuckerFlutter.navigatorKey`.

`redirect` on the router:

- Logged-in users hitting auth routes (except splash) go to `/home`.
- Logged-out users hitting any other route go to `/login`.

Auth routes treated as public: onboarding, first-choose, register, login, password login, OTP, MFA, splash.

Bottom tabs (`AppBottomNavBar` in `lib/core/widgets/app_bottom_nav_bar.dart`):

| Index | Path | Screen |
| --- | --- | --- |
| 0 | `/home` | `HomeScreen` |
| 1 | `/trips` | `MyTripsScreen` |
| 2 | `/market` | `MarketplaceScreen` (coming-soon copy from l10n) |
| 3 | `/settings` | `SettingsScreen` |

`MarketplaceScreen` (`lib/features/marketplce/presentation/views/marketplace.dart`) is a real route. It has no marketplace API.

Other routes include trip details (`/trips/:tripId`), requests (`/requests?type=requested|booked`), profile, notifications, change password, add driver, edit booking request, and the two dues screens. Trip details read `tripId` from the path and an optional return route from `state.extra`. Edit booking request expects a `BookingRequest` in `state.extra`; if it is missing the builder returns an empty `SizedBox`.

**Router holder:** two different classes are both named `AppRouterHolder`:

- Defined and `setRouter`'d inside `lib/core/router/app_router.dart`.
- Defined again in `lib/core/router/app_router_holder.dart`.

`AuthInterceptor` imports **`app_router_holder.dart`**. Nothing in `lib/` calls `setRouter` on that second class. A 401 that fails token refresh therefore looks up a router that stays null. **Needs clarification** whether the duplicate was intentional. As written, the interceptor's `router.go(login)` path does not receive the router created in `createAppRouter()`.

The navigator key is `ChuckerFlutter.navigatorKey`.

## API and networking

`createContractorApiClient()` in `lib/core/network/api_client.dart` builds one `Dio` with:

- Base URL `kContractorApiBaseUrl`
- Connect and receive timeouts of 30 seconds
- Interceptors, in order: `DeviceInterceptor`, `LanguageInterceptor`, `AuthInterceptor`, `AppUpdateInterceptor`, `ChuckerDioInterceptor`

The active base URL in source is staging:

`https://stg-api-agg.loadly-eg.com/api/contractor`

Production `https://api-agg.loadly-eg.com/api/contractor` is commented above it. Switch by commenting one constant. There are no flavors and `dotenv` is not called from `lib/`.

`kApiBaseUrl` is that URL with the `/contractor` suffix removed. Lookups and `/app-version/check` use this shared root (`LookupsRemoteDataSourceImpl`, `AppUpdateService`).

Paths are `ApiEndpoints` in `lib/core/network/api_endpoints.dart` (auth, trips, booking requests, drivers, notifications, documents).

Successful JSON is an envelope. `requireEnvelope` / `requireEnvelopeDataMap` in `lib/core/network/api_response_parser.dart` read `success` and `data` (also `Success` / `Data` in some helpers). List totals are read from several possible keys (`totalCount`, `pagination`, `meta`, and PascalCase variants).

Request headers added by interceptors:

| Header | Source |
| --- | --- |
| `Authorization: Bearer …` | `AuthStorage.getAccessToken()` when non-empty |
| `lang` | `app_locale` in SharedPreferences; default `ar` |
| `device-id`, `device-model`, `device-os`, `device-os-version` | `device_info_plus` |
| `app-version`, `app-build-number` | `package_info_plus` |

`AuthInterceptor` on HTTP 401 calls `AuthStorage.refreshToken()` (`POST /auth/refresh-token` on a **separate** Dio that does not use these interceptors). If refresh returns tokens, it retries the original request. Paths in `_authAttemptPaths` (login, OTP, password, MFA, and similar) are not treated as session expiry. `AuthScreenGuard.isOnAuthScreen` is set only by `OtpScreen` and also skips the logout redirect.

`AppUpdateInterceptor` reads version policy embedded in login/profile envelopes (`AppVersionEnvelopeParser`). Splash uses `AppUpdateService.checkForUpdate()` against `GET {kApiBaseUrl}/app-version/check` with `appType=Contractor`.

## Models and entities

- **Entities** are hand-written classes under `domain/entities` (for example `Trip`, `BookingRequest`, `ContractorProfile`, `Driver`).
- **DTOs** under `data/models` (auth and many trip models) use `@JsonSerializable`. Generated `*.g.dart` files are checked in.
- **Mappers** such as `trip_mapper.dart` and `toEntity()` on some DTOs convert data models to entities. `AuthImpl` maps `ContractorProfileDto` inline and normalizes phone numbers (strip `+2` for display, send `+2` plus digits to the API).

Regenerate after DTO annotation changes:

```bash
dart run build_runner build --delete-conflicting-outputs
```

## Repositories and services

| Type | Examples |
| --- | --- |
| Repository contract | `AuthRepository`, `TripRepository`, `DriverRepository`, `NotificationsRepository` |
| Implementation | `AuthImpl`, `TripRepositoryImpl`, `DriverRepositoryImpl`, `NotificationsImpl` |
| Remote source | `AuthRemoteDataSourceImpl`, `TripRemoteDataSourceImpl`, `DriverRemoteDataSourceImpl`, `NotificationsRemoteDataSourceImpl`, `LookupsRemoteDataSourceImpl` |

`TripRepositoryImpl` and the driver/notification repositories wrap calls in `runApiCall` (`lib/core/network/repository_api_runner.dart`), which uses `safeApiCall` and returns `Either<Failure, T>`. `AuthImpl` uses `safeApiCall` with `AuthFailure` instead of the shared `Failure` type.

`GetDefaultVehicleTypesUseCase` calls `LookupsRemoteDataSource` directly. There is no lookups repository.

Services in `lib/core/services/`:

| Class | Behavior |
| --- | --- |
| `AppUpdateService` | Version check; initialized with the app Dio in `main.dart` |
| `BiometricService` | `local_auth` for biometric login |
| `FCMService` | In-memory token stub. It does not call Firebase |

`firebase_messaging` is a dependency. `Firebase.initializeApp` is not called from `lib/`. `android/app/google-services.json` and `ios/Runner/GoogleService-Info.plist` are present. The Android Gradle file does not apply the Google Services plugin. Push delivery is not wired.

## Dependency injection

Manual construction in `_runApp` (`lib/main.dart`). Order:

1. `AuthStorage.instance.init()`
2. `createContractorApiClient()` and `AppUpdateService.init(dio)`
3. Data sources → repositories → use cases → BLoCs
4. If a token exists but `getContractorUserId()` is null, `GetProfileUseCase` runs and stores the profile id
5. `authBloc.add(AuthCheckSessionRequested())` **before** `runApp`
6. `RepositoryProvider` + `MultiBlocProvider` around `LoadlyContractorApp`

`AuthStorage` and `LocaleBloc`'s preferences are singletons, not constructor-injected. `AuthInterceptor` and `LanguageInterceptor` read them directly.

**Why not a DI package:** none is configured. New dependencies must be constructed in `main.dart` and passed into the BLoC constructor.

## Local storage and cache

No SQL or document database package is used.

| Store | What |
| --- | --- |
| `SharedPreferences` via `AuthStorage` | Access token, refresh token, online flag, remember-me, remembered login, contractor user id, per-user set of known home open-invite trip ids |
| `FlutterSecureStorage` via `AuthStorage` | Remembered password only |
| `SharedPreferences` key `app_locale` | Locale (`LocaleBloc` and `LanguageInterceptor`, same key) |
| `TripBloc.cachedTrips` | In-memory map of trips loaded in this session |

Access and refresh tokens are in SharedPreferences, not secure storage. The code does not say why.

`HomeNewTripHighlightTracker` (owned by `TripBloc`) highlights open-invite trips that were not in the persisted known-id set. `HomeScreen` polls `GetTrips(pageSize: 10, status: 1, openInviteOnly: true)` and `GetInvitationStatistics` every 30 seconds. Highlights are tracked only for that unfiltered first page (`TripBloc.shouldTrackHomeNewTripHighlights`).

## Authentication and authorization

Session means a non-empty access token (`AuthStorage.isLoggedIn()`). There is no role/permission matrix in the client. The API enforces authorization; the app attaches the bearer token and reacts to 401.

Login flows implemented on `AuthBloc`: password login, send-OTP + OTP login, MFA send/verify, forgot/reset password, registration with documents, enable/disable MFA, change password, validate password, biometric login using the remembered password after `local_auth` succeeds.

Logout (`AuthLogoutRequested`) calls the logout use case and clears tokens. `LoadlyContractorApp` listens for a new `AuthUnauthenticated` and resets home-trip highlights plus trip statistics. Several screens also `go` to login when they see `AuthUnauthenticated`.

Router `redirect` is synchronous and only checks the token already in memory. It does not wait for `AuthCheckSessionRequested`.

## Error handling

1. Dio throws `DioException`.
2. `safeApiCall` / `mapDioExceptionToFailure` classify network, unauthorized, server, and unknown (`api_failure_mapper.dart`).
3. Repositories return `Left(failure)`.
4. BLoCs emit error states with `failure.message`.
5. `FailureMessageMapper.forMessage` maps known keys (`network_error`, `ApiFallbackKeys.*`) to `AppLocalizations`. Other messages are shown as returned when `isUserFacingApiMessage` allows them; otherwise a generic fallback is used (`repository_api_runner.dart`).

UI lists use `ApiErrorView` (`lib/core/widgets/api_error_view.dart`). Unknown routes use `_ErrorScreen` in `app_router.dart`.

Sentry (`sentry_flutter`) starts only when **all** of these are true: release mode (`kReleaseMode`), DSN from `SENTRY_DSN` (or the default compiled in `main.dart`) contains `sentry.io` and does not contain `your-other-dsn`, and `SentryFlutter.init` succeeds. `tracesSampleRate` is `0.2`. `sendDefaultPii` is false. Debug builds do not init Sentry. If init throws, the app still runs without it.

Chucker is on in release: `ChuckerFlutter.showOnRelease = true` in `main()`.

## Environment and configuration

| Mechanism | Where |
| --- | --- |
| API host | Commented constant in `api_client.dart`. Default in git is staging |
| `SENTRY_DSN` | `--dart-define`, with a non-empty default in `main.dart` |
| `IOS_APP_STORE_ID`, `HUAWEI_APP_GALLERY_ID`, `APP_STORE_DISTRIBUTION` (default `play`), `APP_STORE_URL` | `AppStoreConfig` via `String.fromEnvironment` |
| Android signing | `android/key.properties` (gitignored). Required only for release Gradle tasks |
| App version | `version:` in `pubspec.yaml` (`1.0.0+6` at the time of this document) |

`dotenv` is a dependency and is unused in `lib/`. Do not expect a `.env` file to change the API URL.

## Main application flow

```text
main()
  → WidgetsFlutterBinding
  → Chucker showOnRelease
  → SentryFlutter.init only in release with a valid DSN
  → _runApp
       → AuthStorage.init
       → Dio + repositories + BLoCs
       → optional profile fetch for contractor user id
       → AuthCheckSessionRequested
       → CupertinoApp.router
            → splash
                 → version check
                 → home or onboarding
            → login / register / OTP / MFA
            → tabs: home, my trips, market (placeholder), settings
```

Home loads open invitations. Trip details book (`/trips/{id}/apply`), withdraw, or change driver. Requested vs booked lists are `RequestsScreen` plus `BookingRequestBloc` / `TripBloc`. Settings reaches profile, language, password, biometrics, dues, and logout. Notifications are an in-app list from `/notifications`, not FCM.

## How features talk to each other

They share the BLoCs from `main.dart`. They do not call each other's repositories.

- **Home** reads `TripBloc` and `TripStatisticsBloc`. Highlight persistence uses `AuthStorage` contractor user id.
- **Requests** reads `BookingRequestBloc`, `TripBloc`, and `GetDefaultVehicleTypesUseCase`. Cards are widgets from `features/home`.
- **Trip details / add driver** use `TripBloc` and `DriverBloc`.
- **Profile** uses auth use cases (`GetProfileUseCase`, `UpdateProfileUseCase`) and writes the user id back to `AuthStorage`.
- **Settings** uses `AuthBloc` and `LocaleBloc`, and navigates to dues routes owned by `trips`.
- **Notifications** use `NotificationsBloc`. Opening a trip uses the router; the payload handling is in the notifications presentation code.
- **Logout** is observed by the root `BlocListener<AuthBloc>` and by individual screens.

`core` does not import feature BLoCs except `FailureMessageMapper`, which imports `AuthFailure` and `NotificationsFailure` so one mapper can cover those types.

## Third-party packages

| Package | Why it is here |
| --- | --- |
| `flutter_bloc`, `equatable` | Screen state. `equatable` is for value equality on some state/entity types |
| `go_router` | Declarative routes and auth redirects |
| `dio` | HTTP client and interceptors |
| `dartz` | `Either` for repository/use-case results |
| `json_annotation` / `json_serializable` | DTO `fromJson` / `toJson` |
| `shared_preferences`, `flutter_secure_storage` | Session, locale, remembered password |
| `local_auth` | Biometric unlock of remembered credentials |
| `cached_network_image`, `image_picker`, `flutter_svg` | Remote images, document capture, SVG icons |
| `permission_handler` | Runtime permissions after the disclosure dialog |
| `pin_code_fields` | OTP entry |
| `device_info_plus`, `package_info_plus` | Device and version headers; version check |
| `url_launcher` | Open the store listing from the update UI |
| `sentry_flutter` | Release crash reporting |
| `chucker_flutter` | In-app HTTP log |
| `intl` + `flutter_localizations` | Generated ARB strings |
| `dotted_border`, `cupertino_icons`, `flutter_sficon` | UI |
| `firebase_messaging`, `flutter_local_notifications` | Declared; messaging is not initialized from `main.dart` |
| `dotenv`, `google_fonts`, `freezed_annotation` | Declared; no usages found under `lib/` |
| `flutter_native_splash` | Dev dependency; no `flutter_native_splash:` block in `pubspec.yaml` |
| `sentry_dart_plugin` | Dev dependency; no plugin config block found in `pubspec.yaml` |

The UI font is the bundled **Zain** family (`appFontFamily` in `lib/core/theme/app_theme.dart`), not `google_fonts`.

## Testing

No `*_test.dart` files were found. `test/` is empty. `ios/RunnerTests/RunnerTests.swift` is an Xcode placeholder. `flutter test` currently has nothing to run.

## Build and deployment

- Android release needs `android/key.properties` (see `android/key.properties.example`). `android/app/build.gradle.kts` throws if that file is missing when a release assemble/bundle/package/sign task runs. Debug does not need it.
- `compileSdk` is 36. Java / Kotlin target is 17. `minSdk` and `targetSdk` come from the Flutter Gradle plugin (`flutter.minSdkVersion` / `flutter.targetSdkVersion`), not a number in this repo.
- CI (`.github/workflows/build-apk.yml`) on push to `main` or `workflow_dispatch`: Flutter 3.47.2, Java 17, `flutter build apk --release`, using the version already in `pubspec.yaml`. Keystore and S3/Teams values are GitHub Actions secrets, not dart-defines.
- iOS bundle id in `ios/Runner.xcodeproj/project.pbxproj` is `com.example.contractorApp`. Android is `com.loadly.contractor`. **Needs clarification** which id production should use.
- `ios/Runner/Info.plist` has no `NSCameraUsageDescription`, photo-library usage strings, or `NSFaceIDUsageDescription`. Camera, gallery, and Face ID on iOS are not fully declared in that plist.

## Needs clarification

- Whether `AppRouterHolder` in `app_router_holder.dart` should be the same instance `createAppRouter()` registers. Today the 401 redirect cannot see that router.
- Production vs staging is a source edit, not a build flavor. Confirm which URL a given build is supposed to ship.
- FCM: platform config files exist; Dart and Android Gradle do not initialize Firebase.
- iOS bundle id and missing privacy usage strings.
- No changelog file, so release history is not documented in-repo.
- `sentry_dart_plugin` and `flutter_native_splash` have no project config in `pubspec.yaml`. Whether they are planned is not stated in code.
