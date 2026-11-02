# Loadly Contractor

Mobile app for **contractors** on the Loadly logistics platform. Contractors sign in, review open trip invitations, apply for trips, manage assigned work and drivers, and track financial dues.

| Field | Value |
| --- | --- |
| **Package name** | `contractor_app` |
| **App title** | Loadly Contractor |
| **Current version** | `1.0.0+6` (from `pubspec.yaml`) |
| **Primary platforms** | Android, iOS |
| **Default language** | Arabic (`ar`), with English (`en`) |

This README describes the project as implemented in the repository. Anything that could not be verified in source is marked as **needs confirmation**.

---

## Table of contents

- [Project overview](#project-overview)
- [Features](#features)
- [Tech stack](#tech-stack)
- [Project structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation and setup](#installation-and-setup)
- [Running the app](#running-the-app)
- [Configuration](#configuration)
- [Testing](#testing)
- [Code quality and development](#code-quality-and-development)
- [Build and release](#build-and-release)
- [Environment variables and secrets](#environment-variables-and-secrets)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)
- [Documentation](#documentation)
- [Additional documentation](#additional-documentation)

---

## Project overview

**Loadly Contractor** is a Flutter client for the contractor API (`/api/contractor`). It is built for transport contractors who receive trip invitations, book work, assign drivers, and follow payment status.

The UI is Cupertino-based, RTL-first, and localized in Arabic and English. Routing is handled with `go_router`. Session tokens are persisted locally and attached to API calls.

### Target users

Contractors working with Loadly (Loadly) who need to:

- Discover open trip invitations
- Apply for or withdraw from trips
- Assign or change drivers and upload driver documents
- Track requested vs booked transfers
- Review completed trips and financial dues
- Manage account, language, password, and biometric login

---

## Features

Implemented user-facing functionality:

### Onboarding and authentication

- Animated splash, then onboarding or home depending on session
- Forced or optional in-app update check against `/app-version/check`
- Registration with name, phone, address, national ID, required documents, and camera/gallery upload
- Phone OTP login
- Password login (phone or email)
- MFA verification via SMS or email after login when required
- Forgot / reset password with OTP
- Remember me (login stored in preferences; password in secure storage)
- Biometric login (Face ID / fingerprint / device credentials) via `local_auth`
- Session restore on startup, token refresh, and redirect to login on unauthorized API responses

### Home and trips

- Home list of **open-invite** trips (`status: 1`, `openInviteOnly: true`), refreshed every 30 seconds
- Highlight for newly seen open-invite trips
- Local search on the home list by trip ID or company name
- Shortcuts to requested and booked transfer lists
- **My trips** with pagination and filters (scheduled / in progress / completed, vehicle type, search, dates)
- Trip details:
  - Open invitations (`status == 1`): book the trip
  - Other statuses: view details, assign/change driver, call driver, withdraw where allowed
- Booking-request lists: requested vs booked, with search, filters, and periodic refresh
- Edit / cancel a booked request (withdraw) when the booking is still editable
- Add driver with required documents

### Financial and account

- Completed trips due (completed trips that are not paid to the contractor)
- Trips financial due list
- In-app notifications from the contractor API (pagination, date filter, mark as read, open trip when the payload targets a trip)
- Profile view and update (name, email, phone, and related fields)
- Settings: language (Arabic / English), change password, biometric toggle, dues screens, logout

### Platform and quality

- Arabic / English localization (`lib/l10n`)
- `lang` header sent on API requests from the current locale
- Device and app-version headers on API requests
- Chucker HTTP inspector (currently enabled including release builds)
- Sentry error reporting in **release** mode when a valid DSN is present
- Play policy-style prominent disclosure before camera/storage permission requests

### Not fully implemented or unused

- **Market** (`/market`, `MarketplaceScreen`) is a coming-soon screen. It does not call a marketplace API.
- **FCM** helpers exist (`firebase_messaging`, `FCMService` stub, local-notification display utilities), but `Firebase.initializeApp` is not called from `main.dart`. Push delivery should be treated as **not fully wired** until confirmed.
- `dotenv`, `google_fonts`, `freezed`, and `flutter_native_splash` are declared in `pubspec.yaml` but are not used in application code (or have no project config) at the time of writing.

---

## Tech stack

| Area | Technology | Notes |
| --- | --- | --- |
| SDK | Flutter **3.47.2** (`.fvmrc`), Dart **^3.12.0** | CI uses the same Flutter version |
| UI | Flutter Cupertino, custom theme, bundled **Zain** font | `CupertinoApp.router` |
| State | `flutter_bloc` | Feature BLoCs provided from `main.dart` |
| Architecture | Feature-first clean architecture | `presentation` / `domain` / `data` |
| Routing | `go_router` | Auth redirects in `createAppRouter()` |
| Networking | `dio` | Interceptors for auth, language, device, app-update, Chucker |
| Functional errors | `dartz` (`Either`) | Use cases and repositories |
| Serialization | `json_serializable` / `json_annotation` | Run `build_runner` after DTO changes |
| Local storage | `shared_preferences`, `flutter_secure_storage` | Tokens, locale, remember-me, biometrics |
| Auth extras | `local_auth` | Biometric login |
| Images | `cached_network_image`, `image_picker`, `flutter_svg` | Profile, documents, assets |
| Permissions | `permission_handler` | Camera / storage / photos |
| Localization | `flutter_localizations`, generated ARB | `l10n.yaml` → `lib/l10n/gen` |
| Updates | `package_info_plus`, `url_launcher` | Version check + store URL |
| Observability | `sentry_flutter` (release), `chucker_flutter` | See configuration |
| Firebase | `firebase_messaging` + platform config files | Messaging layer is currently a stub |
| Icons | `flutter_launcher_icons` | `assets/images/contractor_icon.png` |

There is **no Flutter flavor** setup. API environment is selected in source (see [Configuration](#configuration)).

---

## Project structure

```text
Loadly-Contractor/
├── android/                 # Android host (applicationId com.loadly.contractor)
├── ios/                     # iOS host
├── lib/
│   ├── main.dart            # Binding, DI, Sentry, root app
│   ├── l10n/                # ARB sources + generated localizations
│   ├── core/                # Shared networking, storage, theme, router, widgets
│   └── features/            # Feature modules
├── assets/images/           # SVG/PNG used by the UI
├── assets/fonts/            # Zain font family
├── documentation/           # Architecture, development, and troubleshooting
│   ├── ARCHITECTURE.md
│   ├── DEVELOPMENT.md
│   └── TROUBLESHOOTING.md
├── .github/workflows/       # Release APK CI
├── analysis_options.yaml
├── l10n.yaml
├── pubspec.yaml
└── .fvmrc                   # Pinned Flutter version
```

`linux/`, `macos/`, `web/`, and `windows/` exist as Flutter platform folders. The product UI and permissions are built for **Android and iOS**. Desktop/web support should not be assumed without confirmation.

### `lib/core`

Shared infrastructure used by every feature:

| Path | Responsibility |
| --- | --- |
| `network/` | Dio client, endpoints, interceptors, envelope parsing, error mapping |
| `router/` | `GoRouter` routes, path constants, session redirects |
| `storage/` | Auth tokens, remember-me, contractor user id |
| `services/` | App update check, biometric auth, FCM stub |
| `config/` | Store listing URLs (Play / App Store / AppGallery) |
| `locale/` | `LocaleBloc` (persisted `ar` / `en`) |
| `theme/` | Colors, Cupertino theme, Zain font |
| `widgets/` | Shared buttons, nav bar, pagination, update UI, error views |
| `errors/` | Failure types and user-facing message mapping |
| `permissions/` | Prominent disclosure dialogs before runtime permissions |

### `lib/features`

Each major domain follows **presentation → domain → data**:

```text
feature/
├── presentation/   # screens, widgets, BLoC
├── domain/         # entities, use cases, repository contracts
└── data/           # DTOs, remote sources, repository implementations, mappers
```

| Feature | Role |
| --- | --- |
| `auth` | Login, OTP, MFA, register, password, session |
| `trips` | Trip lists, details, book/withdraw, booking requests, lookups |
| `drivers` | Driver list/create, document types, update trip driver |
| `home` | Home screen and trip cards (uses `TripBloc`) |
| `requests` | Requested / booked transfer lists |
| `notifications` | In-app notification inbox |
| `profile` | Load/update contractor profile |
| `settings` | Language, biometric, password, dues navigation, logout |

Composition root is `lib/main.dart`: Dio client, repositories, use cases, and BLoCs are constructed there and provided with `MultiBlocProvider`.

---

## Prerequisites

| Requirement | Version / notes |
| --- | --- |
| Flutter | **3.47.2** (pinned in `.fvmrc` and CI) |
| Dart | SDK **^3.12.0** (`pubspec.yaml`) |
| FVM (recommended) | To use the pinned Flutter SDK |
| Android | Android Studio / SDK; **Java 17**; `compileSdk` 36 |
| iOS | Xcode on macOS; deployment target **13.0** |
| Git | To clone the repository |

Android `minSdk` / `targetSdk` come from `flutter.minSdkVersion` / `flutter.targetSdkVersion` (not hardcoded). Confirm the values for Flutter 3.47.2 with `flutter config` / the Flutter SDK if you need the numeric SDK levels.

iOS **needs confirmation** for production identifiers: `PRODUCT_BUNDLE_IDENTIFIER` in the iOS project is still `com.example.contractorApp`, while Android uses `com.loadly.contractor`. Display names also differ (`Loadly Contractor` on Android, `Contractor App` on iOS).

---

## Installation and setup

### 1. Clone

```bash
git clone https://github.com/Loadly-EG/Loadly-Contractor-App.git
cd Loadly-Contractor-App
```

### 2. Use the pinned Flutter SDK

```bash
# If FVM is installed
fvm install
fvm use
fvm flutter pub get
```

Without FVM, install Flutter **3.47.2** and run:

```bash
flutter pub get
```

### 3. Android Firebase config

`android/app/google-services.json` is present in the repo. The Android Gradle file does **not** apply the Google Services plugin, and Dart does not call `Firebase.initializeApp`. Keep the file if you will enable messaging; do not treat FCM as active until that wiring exists.

### 4. iOS Firebase config

`ios/Runner/GoogleService-Info.plist` is present. Same caveat as Android: platform files exist, but the Flutter FCM service is a stub.

### 5. Android release signing (release builds only)

Debug / `flutter run` does **not** require a keystore.

For release APK/AAB:

```bash
cp android/key.properties.example android/key.properties
```

Edit `android/key.properties` (never commit this file):

```properties
storePassword=YOUR_KEYSTORE_PASSWORD
keyPassword=YOUR_KEY_PASSWORD
keyAlias=upload
storeFile=upload-keystore.jks
```

`storeFile` is relative to the `android/` directory. Place the `.jks` / `.keystore` accordingly. Those files are gitignored.

### 6. API environment

The contractor API base URL is selected in `lib/core/network/api_client.dart` by commenting one constant and leaving the other:

```dart
/// Switch API by commenting one line and leaving the other active.
// const String kContractorApiBaseUrl =
//     'https://api-agg.loadly-eg.com/api/contractor';
const String kContractorApiBaseUrl =
    'https://stg-api-agg.loadly-eg.com/api/contractor';
```

The active default in source is **staging**. There is no `.env` load at runtime (`dotenv` is unused).

### 7. Optional dart-defines

See [Environment variables and secrets](#environment-variables-and-secrets). None of these are required to run a debug build against staging.

---

## Running the app

From the project root:

```bash
# List devices
flutter devices

# Android
flutter run -d android

# iOS (macOS + Xcode required)
flutter run -d ios
```

With FVM, prefix commands with `fvm`:

```bash
fvm flutter run
```

### Useful flags

```bash
# Override Sentry DSN (release uses Sentry when the DSN is valid)
flutter run --release --dart-define=SENTRY_DSN=YOUR_SENTRY_DSN

# Huawei / HarmonyOS store links for in-app update
flutter run --dart-define=APP_STORE_DISTRIBUTION=harmonyos \
  --dart-define=HUAWEI_APP_GALLERY_ID=YOUR_APP_GALLERY_ID

# iOS App Store id for update links
flutter run --dart-define=IOS_APP_STORE_ID=YOUR_APP_STORE_ID
```

There are **no product flavors** (`dev` / `prod` Gradle flavors). Staging vs production is the commented URL pair in `api_client.dart`.

Chucker is initialized with `ChuckerFlutter.showOnRelease = true` in `main.dart`, so the HTTP inspector is available in debug **and** release unless that flag is changed.

---

## Configuration

### Required vs optional

| Item | Required? | Where |
| --- | --- | --- |
| Flutter 3.47.2 + `flutter pub get` | **Required** | Local SDK / FVM |
| Staging or production API URL | **Required** (source default is staging) | `lib/core/network/api_client.dart` |
| `android/key.properties` + keystore | **Required for Android release only** | `android/key.properties` |
| `SENTRY_DSN` | Optional (release has a source default) | `--dart-define=SENTRY_DSN=...` |
| `IOS_APP_STORE_ID` | Optional | `--dart-define` |
| `HUAWEI_APP_GALLERY_ID` | Optional | `--dart-define` |
| `APP_STORE_DISTRIBUTION` | Optional (`play` default) | `--dart-define` |
| `APP_STORE_URL` | Optional full URL override | `--dart-define` |
| Firebase config files | Present; FCM not fully initialized | Android / iOS project files |

### API

| Constant | Purpose |
| --- | --- |
| `kContractorApiBaseUrl` | Contractor API, e.g. `https://stg-api-agg.loadly-eg.com/api/contractor` |
| `kApiBaseUrl` | Shared API root (strips `/contractor`) for lookups and version check |

Representative contractor paths (see `lib/core/network/api_endpoints.dart`):

- Auth: `/auth/login`, `/auth/send-login-otp`, `/auth/login-with-otp`, `/auth/register`, `/auth/refresh-token`, MFA and password endpoints
- Trips: `/trips`, `/trips/{id}`, `/trips/{id}/apply`, `/trips/{id}/withdraw`, `/trips/{id}/driver`
- Booking requests: `/booking-requests`
- Drivers: `/drivers`
- Notifications: `/notifications`, `/notifications/{id}/read`
- Shared (not under `/contractor`): `/lookups/vehicletypes/default`, `/app-version/check`

Requests send:

- `Authorization: Bearer <access token>` when logged in
- `lang: ar` or `lang: en`
- Device and app headers (`device-id`, `device-model`, `device-os`, `app-version`, …)

### App update / store distribution

`AppStoreConfig` (`lib/core/config/app_store_config.dart`):

| dart-define | Default | Effect |
| --- | --- | --- |
| `APP_STORE_DISTRIBUTION` | `play` | Use `harmonyos` or `appgallery` for Huawei |
| `IOS_APP_STORE_ID` | empty | Builds `https://apps.apple.com/app/id{id}` |
| `HUAWEI_APP_GALLERY_ID` | empty | Builds AppGallery URL |
| `APP_STORE_URL` | empty | Overrides the full store URL |
| Android package | `com.loadly.contractor` | Play Store listing URL |

Version check uses `appType: Contractor` and platform `Android`, `iOS`, or `HarmonyOS`.

### Localization

- Sources: `lib/l10n/app_en.arb`, `lib/l10n/app_ar.arb`
- Config: `l10n.yaml` (`flutter: generate: true`)
- Output: `lib/l10n/gen/`
- Default locale if none is saved: **Arabic**

After ARB changes, run `flutter gen-l10n` or `flutter pub get`.

### Code generation

JSON DTOs use `json_serializable`:

```bash
dart run build_runner build --delete-conflicting-outputs
```

---

## Testing

| Suite | Status |
| --- | --- |
| Dart unit / widget tests | **None found.** The `test/` directory is empty. |
| Integration tests | **None found.** No `integration_test/` suite. |
| iOS `RunnerTests` | Placeholder XCTest only |

Standard Flutter commands still apply once tests are added:

```bash
flutter test
flutter analyze
```

---

## Code quality and development

### Analysis and formatting

```bash
flutter analyze
dart format .
```

`analysis_options.yaml` includes `package:flutter_lints/flutter.yaml` and enables:

- `prefer_const_constructors`
- `prefer_relative_imports`

Analyzer excludes `build/` and platform folders (`android/`, `ios/`, `web/`, `windows/`, `macos/`, `linux/`).

### Conventions observed in this repo

- Feature modules over a single `src/` dump
- BLoC events/states per feature
- Domain use cases returning `Either<Failure, T>`
- Relative imports inside `lib/`
- Arabic-first UX; phone numbers sent as `+2` plus 11 Egyptian digits where login uses phone
- Do not commit `android/key.properties`, `*.jks`, or `*.keystore`

### Recommended workflow

1. Create a branch from the latest `main`.
2. Match Flutter **3.47.2**.
3. Keep API URL changes explicit in `api_client.dart` (do not mix staging/production accidentally).
4. Run `dart format .` and `flutter analyze`.
5. Run `build_runner` if you change `@JsonSerializable` models.
6. Open a pull request (see [Contributing](#contributing)).

### HTTP debugging

Chucker is registered on the Dio client and uses the Chucker navigator key as the app navigator. It is currently shown in release as well (`ChuckerFlutter.showOnRelease = true`).

---

## Build and release

### Android (local)

```bash
# Debug APK
flutter build apk --debug

# Release APK (requires android/key.properties)
flutter build apk --release

# App Bundle
flutter build appbundle --release
```

Outputs:

- APK: `build/app/outputs/flutter-apk/`
- App Bundle: `build/app/outputs/bundle/release/`

Release Gradle tasks fail if `android/key.properties` is missing.

### Android (CI)

`.github/workflows/build-apk.yml`:

- Triggers: push to `main`, or `workflow_dispatch`
- Builds `flutter build apk --release` using the version already in `pubspec.yaml`
- Builds with Flutter **3.47.2** and Java **17**
- Uses GitHub secrets for the upload keystore (see [Environment variables and secrets](#environment-variables-and-secrets))
- Uploads the APK to S3 and can notify Microsoft Teams

### iOS

```bash
flutter build ios --release
```

Requires a Mac, Xcode, signing team, and a correct bundle id. Confirm App Store identifiers (`IOS_APP_STORE_ID`) before relying on in-app update links.

`ios/Podfile` is not checked in; Flutter generates CocoaPods files during iOS builds on macOS.

### Sentry

Sentry initializes only in **release** (`kReleaseMode`) when the DSN looks valid. Override with `--dart-define=SENTRY_DSN=YOUR_SENTRY_DSN`. Prefer dart-define over committing environment-specific DSNs.

### Launcher icons

```bash
dart run flutter_launcher_icons
```

Configured in `pubspec.yaml` from `assets/images/contractor_icon.png`.

---

## Environment variables and secrets

**Never commit real passwords, keystores, DSNs, or tokens.** Use placeholders locally and CI secrets in GitHub.

### App runtime (`--dart-define`)

| Variable | Required | Description |
| --- | --- | --- |
| `SENTRY_DSN` | Optional | Sentry DSN. Example: `https://YOUR_SENTRY_KEY@o000000.ingest.sentry.io/0000000` |
| `IOS_APP_STORE_ID` | Optional | Numeric App Store id |
| `HUAWEI_APP_GALLERY_ID` | Optional | AppGallery app id |
| `APP_STORE_DISTRIBUTION` | Optional | `play` (default), `harmonyos`, or `appgallery` |
| `APP_STORE_URL` | Optional | Full store URL override |

Example:

```bash
flutter build apk --release \
  --dart-define=SENTRY_DSN=YOUR_SENTRY_DSN \
  --dart-define=APP_STORE_DISTRIBUTION=play
```

### Android signing (`android/key.properties`)

| Key | Example |
| --- | --- |
| `storePassword` | `YOUR_KEYSTORE_PASSWORD` |
| `keyPassword` | `YOUR_KEY_PASSWORD` |
| `keyAlias` | `upload` |
| `storeFile` | `upload-keystore.jks` |

### GitHub Actions secrets (CI only)

Used by `.github/workflows/build-apk.yml`. These are **not** read by the Flutter app at runtime.

| Secret | Purpose |
| --- | --- |
| `ANDROID_KEYSTORE_BASE64` | Base64-encoded upload keystore |
| `ANDROID_STORE_PASSWORD` | Keystore password |
| `ANDROID_KEY_PASSWORD` | Key password |
| `ANDROID_KEY_ALIAS` | Key alias |
| `S3_BUCKET` | Bucket for APK upload |
| `TEAMS_WEBHOOK_URL` | Optional Teams notification |
| `GITHUB_TOKEN` | Provided by Actions |

There is no checked-in `.env` file. Do not add secrets to `lib/main.dart` or Gradle files.

---

## Troubleshooting

Common failures are listed below. Longer notes (SDK, CI, auth, and device issues) are in [TROUBLESHOOTING.md](documentation/TROUBLESHOOTING.md).

### Wrong API environment

Symptoms: login or trip calls fail, or you hit production by accident.

Check which `kContractorApiBaseUrl` is uncommented in `lib/core/network/api_client.dart`. Only one line should be active.

### Flutter / Dart version mismatch

The project is pinned to Flutter **3.47.2**. If packages fail to resolve, run `fvm use` or install that SDK. Dart must satisfy `^3.12.0`.

### Android release: `key.properties` missing

```text
Release builds require android/key.properties.
Copy android/key.properties.example and configure your release keystore.
```

Copy the example file and point `storeFile` at a keystore under `android/`. Debug builds do not need this.

### Android Java / Gradle

The app compiles with **Java 17**, Android Gradle Plugin **9.0.1**, and Gradle **9.1.0**. JDK 11 will fail. `kotlin.incremental=false` is set in `android/gradle.properties` to avoid Windows cache issues.

### Biometrics

`MainActivity` extends `FlutterFragmentActivity`, which `local_auth` requires on Android.

On iOS, `Info.plist` currently has **no** `NSFaceIDUsageDescription`. Face ID may fail or be rejected until that usage string is added. **Needs confirmation** of the intended production copy.

### Camera / photos (register and add-driver)

Android declares `CAMERA` and `READ_EXTERNAL_STORAGE` (max SDK 32). On Android 13+ the app uses the system photo picker and avoids broad media permission.

iOS `Info.plist` currently has **no** `NSCameraUsageDescription` or photo-library usage keys. Camera/gallery on iOS may crash or be rejected until those keys are added.

### Firebase / push notifications

`google-services.json` and `GoogleService-Info.plist` exist, but:

- `google-services` Gradle plugin is not applied in `android/app/build.gradle.kts`
- `FCMService` is a token stub
- `main.dart` does not initialize Firebase

Do not expect remote FCM delivery until this is completed.

### Forced update loop on splash

Splash waits for `AppUpdateService.checkForUpdate()`. If the API reports a forced update, the app shows a blocking update screen and will not navigate to home/onboarding. Confirm the `/app-version/check` policy for `appType=Contractor` and the current `pubspec` version.

### iOS bundle identifier

If TestFlight / device install fails, confirm whether the bundle id should remain `com.example.contractorApp` or be aligned with Android `com.loadly.contractor`. **Needs confirmation.**

### Localization not updating

Run `flutter pub get` or `flutter gen-l10n` after editing ARB files. Generated files live in `lib/l10n/gen/`.

---

## Contributing

There is no `CONTRIBUTING.md` in this repository. The following is a **recommendation** based on the existing GitHub remote and `main` CI workflow.

1. Branch from up-to-date `main`.
2. Keep changes focused (one feature or fix per PR).
3. Do not commit secrets, keystores, or real DSNs.
4. Run `dart format .` and `flutter analyze` before you push.
5. Open a pull request against `main`. Pushing to `main` triggers the release APK workflow.

Bump the `version:` field in `pubspec.yaml` when you want a new APK version. CI no longer commits version changes.

---

## License

This repository does **not** include a `LICENSE` file. Do not assume an open-source license. Ask the Loadly / Loadly owners before redistributing.

---

## Documentation

These guides live in [`documentation/`](documentation/).

| Guide | What it covers |
| --- | --- |
| [ARCHITECTURE.md](documentation/ARCHITECTURE.md) | Layers, routing, networking, state, storage, and how features connect |
| [DEVELOPMENT.md](documentation/DEVELOPMENT.md) | Setup, run, test, build, and where to add a feature |
| [TROUBLESHOOTING.md](documentation/TROUBLESHOOTING.md) | Build, API, auth, and device problems seen in this repo |

## Additional documentation

| Resource | Location |
| --- | --- |
| Android release signing template | [`android/key.properties.example`](android/key.properties.example) |
| Release APK workflow | [`.github/workflows/build-apk.yml`](.github/workflows/build-apk.yml) |
| Analyzer / lints | [`analysis_options.yaml`](analysis_options.yaml) |
| Localization config | [`l10n.yaml`](l10n.yaml) |
| Flutter version pin | [`.fvmrc`](.fvmrc) |
| iOS launch-image placeholder notes | [`ios/Runner/Assets.xcassets/LaunchImage.imageset/README.md`](ios/Runner/Assets.xcassets/LaunchImage.imageset/README.md) |

Flutter framework docs: [https://docs.flutter.dev/](https://docs.flutter.dev/)
