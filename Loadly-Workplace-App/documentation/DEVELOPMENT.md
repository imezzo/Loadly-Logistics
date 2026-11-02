# Development guide

Practical setup for Loadly Workplace. Architecture and request flow are in [ARCHITECTURE.md](ARCHITECTURE.md). Failures and fixes are in [TROUBLESHOOTING.md](TROUBLESHOOTING.md).

## Prerequisites

- [FVM](https://fvm.app/)
- Flutter **3.41.2**, pinned in `.fvmrc`
- Dart SDK constraint in `pubspec.yaml`: `^3.8.0`. The exact Dart version bundled with Flutter 3.41.2 is not recorded in the repo
- Git
- Android: Android SDK, a device or emulator. CI uses **Java 17** (Temurin)
- iOS (macOS only): Xcode, CocoaPods. `ios/Podfile` sets platform **iOS 15.5**

VS Code / Cursor should use the FVM SDK. `.vscode/settings.json` sets `dart.flutterSdkPath` to `.fvm/flutter_sdk`. `.fvmrc` has `updateVscodeSettings: false`, so FVM will not overwrite that file.

## Install and run

From the repository root:

```bash
fvm install
fvm flutter pub get
fvm dart run build_runner build --delete-conflicting-outputs
fvm flutter run
```

Use `fvm flutter` and `fvm dart` so the pinned SDK is used. `.fvm/` is created locally and is gitignored.

Run on a chosen device:

```bash
fvm flutter devices
fvm flutter run -d <deviceId>
```

### API and other compile-time defines

There is no `.env` file. Overrides are `--dart-define` flags (see `lib/core/network/api_client.dart` and `lib/core/config/app_store_config.dart`).

```bash
fvm flutter run --dart-define=WORKPLACE_API_BASE_URL=https://stg-api-agg.loadly-eg.com/api/collector
```

| Define | Purpose |
| --- | --- |
| `WORKPLACE_API_BASE_URL` | Collector API. Default in source is the staging collector URL |
| `SENTRY_DSN` | Release crash reporting. A default is already in `lib/main.dart` |
| `IOS_APP_STORE_ID` | Numeric App Store id for the update button |
| `HUAWEI_APP_GALLERY_ID` | AppGallery id |
| `APP_STORE_DISTRIBUTION` | `play` (default), or `harmonyos` / `appgallery` |
| `APP_STORE_URL` | Full store URL, if you do not want the built-in one |

The name is `WORKPLACE_API_BASE_URL`. An older README name, `Workplace_API_BASE_URL`, is not read by the app.

Firebase needs `android/app/google-services.json` and `ios/Runner/GoogleService-Info.plist`. Both are part of this project. `Firebase.initializeApp()` uses that native config. There is no `lib/firebase_options.dart`.

Release Android signing reads `android/key.properties` when that file exists (`storeFile`, `storePassword`, `keyAlias`, `keyPassword`). Without it, release builds are signed with the debug key. Do not commit keystore passwords.

## Tests and static analysis

```bash
fvm flutter test
fvm flutter analyze
```

`test/widget_test.dart` is an empty `main`. `flutter test` currently does not exercise the app. Add tests under `test/` next to the feature you change. The project has no golden tests, mock framework, or bloc_test dependency.

Sonar config is `sonar-project.properties` (sources: `lib/`, generated files excluded). Its `sonar.projectVersion` is `1.0.4` and is not tied to `pubspec.yaml`.

## Builds

```bash
fvm flutter build apk --release
fvm flutter build appbundle --release
fvm flutter build ios --release
```

Pass the same `--dart-define` values you need in that binary. CI (`.github/workflows/build-apk.yml`) does not pass any, so a `main` APK uses the defaults in source.

App version is `version:` in `pubspec.yaml` (`1.1.2+6` means version name `1.1.2`, build number `6`). CI reads that line and passes `--build-name` and `--build-number`.

Launcher icons and the white native splash are configured at the bottom of `pubspec.yaml` (`flutter_launcher_icons`, `flutter_native_splash`). Regenerate only when those sections change:

```bash
fvm dart run flutter_launcher_icons
fvm dart run flutter_native_splash:create
```

### Code generation

DTOs annotated with `@JsonSerializable` need a matching `*.g.dart` file. After changing those classes:

```bash
fvm dart run build_runner build --delete-conflicting-outputs
```

`build.yaml` sets `explicit_to_json: true`. Localization is generated from `lib/l10n/*.arb` by Flutter (`l10n.yaml`, `flutter: generate: true`). A normal `flutter pub get` / build refreshes `lib/l10n/gen/`. Do not edit files in `gen/` or `*.g.dart` by hand.

## Where code goes

| You are adding | Put it in |
| --- | --- |
| Screen | `lib/features/<feature>/presentation/views/` |
| Widget used by one feature | `lib/features/<feature>/presentation/widgets/` |
| Widget used by several features | `lib/core/widgets/` |
| BLoC event/state | `presentation/bloc/` next to the feature |
| Entity, repository interface, use case, failure | `domain/` |
| DTO, remote data source, repository implementation, mapper | `data/` |
| Endpoint constant | `lib/core/network/api_endpoints.dart` |
| Route | `lib/core/router/app_router.dart` |
| User-visible string | `lib/l10n/app_en.arb` and `lib/l10n/app_ar.arb` |
| Shared HTTP / storage / device service | `lib/core/` |

`profile` has no `data` or `domain` folder. Profile loading reuses auth’s `GetProfileUseCase`. Trip **reads** belong with `home`. Trip **collect/confirm writes** belong with `trips`.

## How to add a feature

Follow `auth` for a full vertical slice, or `trips` for an action on data that `home` already loads.

1. Add the endpoint to `ApiEndpoints`.
2. Add a DTO with `@JsonSerializable` and run `build_runner` if the body is JSON.
3. Add a method on the remote data source. Parse the `{ success, data, message, errors }` envelope with `requireEnvelopeDataMap` / `throwIfEnvelopeFailed`, unless the response shape differs (trips list does).
4. Add a domain failure type and a repository interface method that returns `Either<YourFailure, T>`.
5. Implement the repository with `safeApiCall` (see `AuthImpl` and `HomeImpl`).
6. Add a use case that only forwards to the repository.
7. Construct the data source, repository, and use case in `_runApp` in `lib/main.dart`, and pass the use case into the BLoC.
8. Handle a new event on the BLoC. Map failures to a state the screen already knows how to show.
9. Register a `GoRoute` if there is a new screen. Protected routes are anything other than `/`, `/auth/login`, and `/otp`. If the screen needs an object, pass it with `extra` and cast it in the builder (see `/trips/:tripId` and `/filter`). A cold open of those routes without `extra` throws.
10. Add English and Arabic strings. Read them with `AppLocalizations.of(context)!`.

Financial-collector behavior is not a separate feature module. It is `AuthStorage.getisFinancialCollector()`, set at login from the `confirm_trip_documents` permission.

## Conventions found in the code

Enforced by `analysis_options.yaml`:

- `package:flutter_lints/flutter.yaml`
- `prefer_const_constructors`
- `prefer_relative_imports` (import `../bloc/home_bloc.dart`, not a package import, inside `lib/`)

Observed, not enforced by a custom lint:

- UI is mostly `Cupertino*` widgets. A few Material widgets are imported (`RefreshIndicator` on the profile screen, `ScaffoldMessenger` in `main.dart`).
- Events and states extend `Equatable`.
- Repository methods return `Future<Either<Failure, T>>` from `dartz`.
- New JSON models use `@JsonSerializable`, not hand-written `fromJson`, except `TokenModel` in `auth_storage.dart`.
- Singletons: `AuthStorage.instance`, `BiometricService.instance`, `FCMService()`, `LocaleBloc`’s prefs future.
- User-facing copy belongs in ARB files. Several BLoC error strings are still hard-coded English (`HomeBloc`, `TripsBloc`, `FailureMessageMapper` defaults).
- Names are inconsistent (`GetDocumentsUsecase` vs `LoginUseCase`, `saveisFinancialCollector`). Match the file you are editing rather than introducing a third spelling.

Two classes are both named `CollectorDocuments` (domain entity vs multipart request wrapper). Import with a prefix if a file needs both.

## Commands

```bash
fvm install
fvm flutter pub get
fvm flutter run
fvm flutter test
fvm flutter analyze
fvm dart run build_runner build --delete-conflicting-outputs
fvm flutter build apk --release
fvm flutter build appbundle --release
fvm flutter build ios --release
fvm flutter clean
```

Clean rebuild when native plugins or generated registrants look stale:

```bash
fvm flutter clean
fvm flutter pub get
```

On iOS, from `ios/`: `pod install` (after `flutter pub get`, which writes `ios/Flutter/Generated.xcconfig`).

## Common problems

Short versions. Detail and checks are in [TROUBLESHOOTING.md](TROUBLESHOOTING.md).

| Symptom | What to check |
| --- | --- |
| `flutter` is the wrong version | Use `fvm flutter`. Run `fvm install` after clone |
| Missing `*.g.dart` or JSON errors after a DTO edit | Re-run `build_runner` |
| App talks to the wrong host | Define name is `WORKPLACE_API_BASE_URL`. CI does not set it |
| Login works then every call returns 401 | Refresh token flow in `AuthInterceptor` / `AuthStorage.refreshToken` |
| `/trips/123` or `/filter` crashes on open | Those routes cast `state.extra`. Open them from the app, not as a bare URL |
| Android release not accepted by Play | `android/key.properties` missing, so the build is debug-signed |
| iOS build cannot find pods | `pod install` in `ios/` with the FVM Flutter root generated |
| Web build fails on `dart:io` | `main.dart` and `api_client.dart` import `dart:io`. Web is not a supported entrypoint |
