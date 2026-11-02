# Development guide

Practical steps for working on **Loadly Contractor**. Project overview is in [README.md](../README.md). How the code is organized is in [ARCHITECTURE.md](ARCHITECTURE.md). Failures and misconfiguration are in [TROUBLESHOOTING.md](TROUBLESHOOTING.md).

## Prerequisites

| Requirement | Source |
| --- | --- |
| Flutter **3.47.2** | `.fvmrc` and `.github/workflows/build-apk.yml` |
| Dart SDK **^3.12.0** | `environment.sdk` in `pubspec.yaml` |
| FVM | Optional. Use it so local Flutter matches `.fvmrc` |
| Android toolchain | Android SDK; **Java 17** (`android/app/build.gradle.kts`, CI) |
| iOS | Xcode on macOS. Bundle id in the Xcode project is `com.example.contractorApp` |
| Git | Clone `https://github.com/Loadly-EG/Loadly-Contractor-App.git` |

`compileSdk` is **36**. `minSdk` / `targetSdk` are `flutter.minSdkVersion` and `flutter.targetSdkVersion` (the Flutter SDK’s defaults), not literals in this repo.

Current app version in `pubspec.yaml`: **`1.0.0+6`**.

## Install dependencies

```bash
fvm install
fvm use
fvm flutter pub get
```

Without FVM, install Flutter 3.47.2 and run `flutter pub get`.

Generated localizations are committed under `lib/l10n/gen/`. After editing `lib/l10n/app_en.arb` or `app_ar.arb`:

```bash
flutter gen-l10n
```

`flutter: generate: true` in `pubspec.yaml` also regenerates them on `flutter pub get`. Config is `l10n.yaml`.

After changing `@JsonSerializable` models:

```bash
dart run build_runner build --delete-conflicting-outputs
```

## Run

```bash
flutter devices
flutter run
```

Prefix with `fvm` when the global SDK is not 3.47.2: `fvm flutter run`.

Debug and `flutter run` do **not** need `android/key.properties`.

The API host is the uncommented `kContractorApiBaseUrl` in `lib/core/network/api_client.dart`. The committed default is **staging** (`https://stg-api-agg.loadly-eg.com/api/contractor`). Production is the commented line above it. Only one of those constants should be active. `dotenv` is not loaded.

Optional dart-defines (none are required for a staging debug run):

```bash
flutter run --release --dart-define=SENTRY_DSN=YOUR_SENTRY_DSN
flutter run --dart-define=IOS_APP_STORE_ID=YOUR_APP_STORE_ID
flutter run --dart-define=APP_STORE_DISTRIBUTION=harmonyos --dart-define=HUAWEI_APP_GALLERY_ID=YOUR_APP_GALLERY_ID
flutter run --dart-define=APP_STORE_URL=https://example.com/store
```

`APP_STORE_DISTRIBUTION` defaults to `play` (`lib/core/config/app_store_config.dart`).

Chucker’s HTTP inspector is enabled in release as well (`ChuckerFlutter.showOnRelease = true` in `lib/main.dart`).

## Tests

There are no project tests under `test/` and no `integration_test/` suite. `flutter test` will not exercise the app until tests are added.

```bash
flutter analyze
dart format .
```

Analyzer rules: `package:flutter_lints/flutter.yaml`, plus `prefer_const_constructors` and `prefer_relative_imports` (`analysis_options.yaml`). Platform folders and `build/` are excluded.

## Build

Android debug:

```bash
flutter build apk --debug
```

Android release (fails unless `android/key.properties` exists):

```bash
cp android/key.properties.example android/key.properties
flutter build apk --release
flutter build appbundle --release
```

`storeFile` in `key.properties` is relative to the `android/` directory. Do not commit `key.properties`, `*.jks`, or `*.keystore`.

Outputs:

- APK: `build/app/outputs/flutter-apk/`
- App bundle: `build/app/outputs/bundle/release/`

iOS (Mac + signing):

```bash
flutter build ios --release
```

`ios/Podfile` is not checked in; Flutter generates CocoaPods metadata on a Mac build.

CI (`.github/workflows/build-apk.yml`) builds a release APK on push to `main` with Flutter 3.47.2 and Java 17, then can upload it to S3. It uses GitHub secrets (`ANDROID_KEYSTORE_BASE64`, `ANDROID_STORE_PASSWORD`, `ANDROID_KEY_PASSWORD`, `ANDROID_KEY_ALIAS`, `S3_BUCKET`, optional `TEAMS_WEBHOOK_URL`). Those secrets are not read by the app at runtime. CI does not bump `pubspec.yaml`.

Launcher icons:

```bash
dart run flutter_launcher_icons
```

`pubspec.yaml` points `flutter_launcher_icons` at `assets/images/contractor_icon.png`. That file was **not** in the repository when this guide was written (`assets/images/` has the SVG/PNG UI assets, not that icon). Add the file or change `image_path` before running the command.

## Configuration

| Item | Required to run debug? | Where |
| --- | --- | --- |
| Flutter 3.47.2 and `pub get` | Yes | SDK / FVM |
| Staging or production URL | Yes (staging is already selected) | `lib/core/network/api_client.dart` |
| `android/key.properties` | Only Android release | `android/key.properties` |
| `SENTRY_DSN` | No. Release uses the default in `main.dart` if the define is omitted | `--dart-define` |
| Store dart-defines | No | `AppStoreConfig` |
| Firebase json/plist | Present; FCM is not initialized | `android/app/google-services.json`, `ios/Runner/GoogleService-Info.plist` |

Do not commit keystores, passwords, or a replacement Sentry DSN. Override Sentry with `--dart-define=SENTRY_DSN=...` instead of editing the default when you can.

## Add a feature

Match the modules that already have all three layers (`auth`, `trips`, `drivers`, `notifications`):

```text
lib/features/<name>/
├── presentation/
│   ├── bloc/          # event, state, bloc
│   ├── views/         # screens
│   └── widgets/
├── domain/
│   ├── entities/
│   ├── repositories/  # abstract contract
│   ├── use_cases/
│   └── failures/      # when the feature needs its own failure type
└── data/
    ├── models/        # DTOs; @JsonSerializable when JSON is generated
    ├── datasources/   # or sources/remote/
    ├── mappers/
    └── repositories/  # implementation
```

Existing layout is slightly inconsistent. Auth remote code is `data/sources/remote/`. Trips and drivers use `data/datasources/`. Follow the feature you are extending.

Steps that this codebase actually requires:

1. Add the path to `ApiEndpoints` if it is a new contractor route. Use `kApiBaseUrl` only for endpoints that are **not** under `/api/contractor` (see `vehicleTypesDefault` and `appVersionCheck`).
2. Parse envelopes with `api_response_parser.dart` (`requireEnvelope`, `requireEnvelopeDataMap`). Do not assume a raw JSON array at the top level.
3. Return `Either` from the repository via `runApiCall` (`lib/core/network/repository_api_runner.dart`) or `safeApiCall` if you need a custom failure type (`AuthImpl` does this).
4. Add a use case. The shared contracts are `UseCase<T, Params>` and `UseCaseNoParams<T>` in `lib/core/usecase/usecase.dart`. Auth re-exports that file from `features/auth/domain/use_cases/usecase.dart`.
5. Construct the data source, repository, use case, and BLoC in `_runApp` in `lib/main.dart`, and add a `BlocProvider`.
6. Register a `GoRoute` in `createAppRouter()` and a constant on `AppRoutePaths`.
7. Add strings to **both** `app_en.arb` and `app_ar.arb`, then regenerate l10n.
8. Map new fallback keys in `ApiFallbackKeys` and `FailureMessageMapper` if the UI should show a localized error instead of a raw key.

Screens that are only a tab (`home`, `settings`, `marketplce`) do not have domain/data folders. Prefer calling an existing BLoC (`TripBloc`, `AuthBloc`, …) rather than calling Dio from a widget.

`profile` is presentation-only on purpose in the current tree: `GetProfileUseCase` and `UpdateProfileUseCase` belong to `auth`.

## Where code goes

| Kind | Location |
| --- | --- |
| Screen | `features/<feature>/presentation/views/` |
| Feature widget | `features/<feature>/presentation/widgets/` |
| Shared widget | `lib/core/widgets/` |
| BLoC | `features/<feature>/presentation/bloc/` |
| Entity | `features/<feature>/domain/entities/` |
| DTO | `features/<feature>/data/models/` |
| Remote API | `data/datasources/` or `data/sources/remote/` |
| Repository interface | `domain/repositories/` |
| Repository implementation | `data/repositories/` |
| Use case | `domain/use_cases/` |
| Endpoint constant | `lib/core/network/api_endpoints.dart` |
| Route | `lib/core/router/route_constants.dart` and `app_router.dart` |
| Token / prefs session | `lib/core/storage/auth_storage.dart` |
| Theme, colors, Zain font | `lib/core/theme/` |
| Cross-feature service | `lib/core/services/` |

Bottom navigation is `AppBottomNavBar`. Tab shells use `AppMainPageScaffold`.

## Conventions in this repo

- Relative imports inside `lib/` (`prefer_relative_imports`).
- `prefer_const_constructors`.
- Feature BLoCs take use cases in the constructor; they are not looked up from a service locator.
- Use cases and repositories return `Future<Either<Failure, T>>` (auth uses `AuthFailure`).
- User-visible copy goes through `AppLocalizations`, not hard-coded Arabic/English in new widgets. Default locale when nothing is saved is `ar` (`LocaleBloc`).
- Phone numbers sent to auth APIs get a `+2` prefix in `AuthImpl` (`_phoneWithPlus2`). Display strips a leading `+2`.
- Comments and some names are English. The product UI is RTL-first.
- Do not commit signing secrets.

`LoadlyContractorApp.build` calls `createAppRouter()` on every locale rebuild. Do not assume a single `GoRouter` instance for the whole process. See [ARCHITECTURE.md](ARCHITECTURE.md).

## Common problems

Short index. Detail and fixes are in [TROUBLESHOOTING.md](TROUBLESHOOTING.md).

| Symptom | First check |
| --- | --- |
| Calls hit the wrong server | Which `kContractorApiBaseUrl` is uncommented |
| Package solve fails | Flutter is not 3.47.2 |
| Release Gradle error about `key.properties` | Copy `android/key.properties.example` |
| New DTO field is always null | `build_runner` was not run |
| New ARB key missing | `flutter gen-l10n` |
| iOS camera or Face ID fails | `Info.plist` has no camera, photo, or Face ID usage strings |
| Push notifications do nothing | `FCMService` is a stub; Firebase is not initialized in `main.dart` |

## Commands

```bash
fvm flutter pub get
fvm flutter run
fvm flutter analyze
dart format .
flutter gen-l10n
dart run build_runner build --delete-conflicting-outputs
flutter test
flutter build apk --debug
flutter build apk --release
flutter build appbundle --release
flutter build ios --release
dart run flutter_launcher_icons
```
