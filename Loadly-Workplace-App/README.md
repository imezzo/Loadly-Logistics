# Loadly Workplace

Flutter app for Loadly collectors. Collectors sign in, browse and filter trips, and collect or confirm trip documents. A login that includes the `confirm_trip_documents` permission is treated as a financial collector and sees extra document statuses and confirm actions.

Package name: `loadly_workplace`. Version in `pubspec.yaml`: `1.1.2+6`.

The app class is `LoadlyWorkplaceApp`. The Flutter window title and the Android and iOS display names are **Loadly Workplace**.

**Supported targets:** Android and iOS. CI builds an Android release APK (`.github/workflows/build-apk.yml`). Platform folders also exist for web, macOS, Windows, and Linux. `lib/main.dart` and `lib/core/network/api_client.dart` import `dart:io`, so a web build of the current entrypoint does not compile. Desktop is not built by CI.

---

## Documentation

| Document | Contents |
| --- | --- |
| [documentation/ARCHITECTURE.md](documentation/ARCHITECTURE.md) | Layers, routing, API, auth, storage, and how features connect |
| [documentation/DEVELOPMENT.md](documentation/DEVELOPMENT.md) | Setup, commands, and where to add code |
| [documentation/TROUBLESHOOTING.md](documentation/TROUBLESHOOTING.md) | Build, API, auth, and runtime problems |

---

## Flutter version (FVM)

This project uses [FVM](https://fvm.app/) with Flutter **3.41.2** (`.fvmrc`). The Dart SDK constraint is `^3.8.0`.

```bash
fvm install
fvm flutter pub get
fvm dart run build_runner build --delete-conflicting-outputs
fvm flutter run
```

Use `fvm flutter` and `fvm dart` so the pinned SDK is used. `.fvm/` is local and gitignored. VS Code uses `.fvm/flutter_sdk` via `.vscode/settings.json`. `updateVscodeSettings` is off in `.fvmrc`, so FVM does not overwrite that file.

---

## Tech stack

| Category | Technology |
| --- | --- |
| Framework | Flutter 3.41.2 |
| Language | Dart `^3.8.0` |
| UI | Cupertino (`CupertinoApp.router`) |
| State | BLoC (`flutter_bloc`) |
| Navigation | GoRouter |
| HTTP | Dio |
| Results | `dartz` `Either` |
| Architecture | Feature folders with domain / data / presentation |
| Push | Firebase Cloud Messaging and `flutter_local_notifications` |
| Errors | Sentry in release mode |
| JSON | `json_serializable` and `build_runner` |
| Localization | English and Arabic ARB files (`lib/l10n/`) |

There is no maps or geolocation package. The home screen is a trip list, not a map.

---

## Project structure

```text
lib/
├── main.dart                 # Startup, manual dependency injection, Firebase, Sentry
├── core/                     # Network, router, storage, theme, locale, services, shared widgets
├── features/
│   ├── auth/                 # Login, MFA, password reset, biometrics
│   ├── home/                 # Trip list, filters, trip details
│   ├── profile/              # Profile screen (uses auth's GetProfileUseCase)
│   └── trips/                # Collect and confirm documents
└── l10n/                     # app_en.arb, app_ar.arb, generated gen/
assets/images/
documentation/                # ARCHITECTURE, DEVELOPMENT, TROUBLESHOOTING
```

Dependencies are constructed in `lib/main.dart`. There is no service-locator package.

---

## Setup

### First time

```bash
fvm install
fvm flutter pub get
fvm dart run build_runner build --delete-conflicting-outputs
```

Firebase uses `android/app/google-services.json` and `ios/Runner/GoogleService-Info.plist`. There is no `lib/firebase_options.dart`. `Firebase.initializeApp()` reads that native config.

Release Android signing uses `android/key.properties` when that file exists. Without it, the release build is signed with the debug key (`android/app/build.gradle`).

iOS deployment target in `ios/Podfile` is **15.5**. Application id and bundle id are `com.loadly.workplace`.

### API base URL

The compile-time define is `WORKPLACE_API_BASE_URL`. The default in `lib/core/network/api_client.dart` is:

`https://stg-api-agg.loadly-eg.com/api/collector`

```bash
fvm flutter run --dart-define=WORKPLACE_API_BASE_URL=https://stg-api-agg.loadly-eg.com/api/collector
```

Other defines (`SENTRY_DSN`, `IOS_APP_STORE_ID`, `HUAWEI_APP_GALLERY_ID`, `APP_STORE_DISTRIBUTION`, `APP_STORE_URL`) are listed in [documentation/DEVELOPMENT.md](documentation/DEVELOPMENT.md). CI does not pass `--dart-define`, so a `main` APK uses the defaults in source.

Sentry (`pubspec.yaml`) uploads symbols for project `loadly-loadly-driver`, org `fuelin-wr`. It initializes only in release mode.

---

## What the app does

- **Sign in** with username and password. MFA and forgot-password both continue on `/otp`. Biometric login replays a saved password.
- **List trips** on `/home`, with search, pagination, and filters (`/filter`).
- **Open a trip** at `/trips/:id`. The route requires the `CollectorTrips` object as GoRouter `extra`.
- **Collect or confirm documents**, including bulk actions on the home screen. Photos are chosen with `image_picker`.
- **Profile** at `/profile`: account data, language, biometrics, logout, app version.
- **Push notifications** via FCM. A tap opens `/home`. There is no notifications inbox.
- **App update check** on the splash screen (`/app-version/check`). A forced update blocks the app. An optional update is a dialog.

Bottom navigation is Home and Profile (`MainScaffold`, `AppBottomNavBar`).

### Routes

| Path | Screen |
| --- | --- |
| `/` | Splash (about 2 seconds, plus the update check), then `/home` or `/auth/login` |
| `/auth/login` | Password login |
| `/otp` | MFA or forgot-password code |
| `/home` | Trip list |
| `/profile` | Profile |
| `/filter` | Filters (`extra` must be `FilterData`) |
| `/trips/:tripId` | Trip details (`extra` must be `CollectorTrips`) |
| `/trips` | `TripsListScreen` is registered. No screen navigates here |

A logged-in session is a non-empty refresh token in `AuthStorage`. Logged-in users are redirected away from `/auth/login` and `/otp`. Other paths require that token.

### Collector types

| | Factory collector | Financial collector |
| --- | --- | --- |
| How it is set | Login permissions do not include `confirm_trip_documents` | Permissions include `confirm_trip_documents` |
| Default document statuses | `1`, `2` | `1`, `2`, `3`, `4` |

`HomeScreen` always requests trips with `status: 6`. Document types are loaded with `entityId: 5`. Those two numbers are not named in the code.

---

## API

Shared Dio client: `createWorkplaceApiClient()` in `lib/core/network/api_client.dart`.

Interceptor order: device headers, `lang`, FCM token, bearer token (refresh on 401), app-update capture, Chucker. Chucker is enabled in release (`ChuckerFlutter.showOnRelease = true`).

| Area | Paths |
| --- | --- |
| Auth | `/auth/login`, `/auth/logout`, `/auth/profile`, `/auth/forget-password`, `/auth/reset-password`, `/auth/reset-password-with-code`, `/auth/send-mfa-code`, `/auth/verify-mfa`, `/auth/refresh-token` |
| Trips | `GET /trips`, `GET /documents` |
| Documents | `POST /trips/{id}/documents/collect`, `POST /trips/{id}/documents/confirm`, `POST /trips/documents/bulk-collect`, `POST /trips/documents/bulk-confirm` |
| App update | `GET {api root}/app-version/check` (`appType` is `Collector`) |

The `{api root}` is the base URL with a trailing `/collector` removed. Request and error details are in [documentation/ARCHITECTURE.md](documentation/ARCHITECTURE.md).

---

## State and storage

App-wide BLoCs from `main.dart`: `AuthBloc`, `HomeBloc`, `TripsBloc`, `ProfileBloc`, `LocaleBloc`.

```text
UI → BLoC event → use case → repository → remote data source → Dio
```

Tokens and the financial-collector flag are in SharedPreferences. Passwords and biometric secrets are in `FlutterSecureStorage`. Locale key `app_locale` defaults to Arabic. There is no local database.

---

## Localization

- English (`en`), LTR, Manrope
- Arabic (`ar`), RTL, Almarai — this is the default

ARB files: `lib/l10n/app_en.arb`, `lib/l10n/app_ar.arb`. Generated code: `lib/l10n/gen/`. The API `lang` header follows `app_locale`.

```dart
final l10n = AppLocalizations.of(context)!;
Text(l10n.appTitle)
```

---

## Code generation

After changing a `@JsonSerializable` class:

```bash
fvm dart run build_runner build --delete-conflicting-outputs
```

`freezed` is a dependency, but no `@freezed` class exists in `lib/`.

---

## Tests and builds

```bash
fvm flutter test
fvm flutter analyze
fvm flutter build apk --release
fvm flutter build appbundle --release
fvm flutter build ios --release
```

`test/widget_test.dart` is an empty `main`. `flutter test` does not exercise the app.

Pushing to `main` (or running the workflow manually) builds a release APK, uploads it to S3, and can email a link and post to Teams. See `.github/workflows/build-apk.yml`.

---

## Key files

| File | Purpose |
| --- | --- |
| `lib/main.dart` | App start, dependency graph, Firebase, Sentry |
| `lib/core/router/app_router.dart` | Routes, splash, auth redirects |
| `lib/core/network/api_client.dart` | Dio client and interceptors |
| `lib/core/network/api_endpoints.dart` | Path constants |
| `lib/core/storage/auth_storage.dart` | Tokens and financial-collector flag |
| `lib/features/auth/presentation/bloc/auth_bloc.dart` | Login, MFA, logout, biometrics |
| `lib/features/home/presentation/bloc/home_bloc.dart` | Trip list and document catalog |
| `lib/features/trips/presentation/bloc/trips_bloc.dart` | Collect and confirm |
| `pubspec.yaml` | Dependencies, version, Sentry, icons, splash |
| `.fvmrc` | Flutter 3.41.2 |
