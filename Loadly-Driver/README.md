# Loadly Driver

Flutter driver app for Loadly, enabling drivers to manage their availability, track trips, and receive notifications.

**Supported platforms:** Android, iOS, Web, macOS

---

## Table of Contents

1. [Overview](#overview)
2. [Flutter Version (FVM)](#flutter-version-fvm)
3. [Tech Stack](#tech-stack)
4. [Project Structure](#project-structure)
5. [Architecture](#architecture)
6. [Setup & Configuration](#setup--configuration)
7. [Features & Screens](#features--screens)
8. [API Integration](#api-integration)
9. [Authentication](#authentication)
10. [State Management](#state-management)
11. [Routing & Navigation](#routing--navigation)
12. [Localization](#localization)
13. [Services & Infrastructure](#services--infrastructure)
14. [App Updates](#app-updates)
15. [Code Generation](#code-generation)
16. [Testing](#testing)
17. [Key Files Reference](#key-files-reference)
18. [Documentation](#documentation)

---

## Overview

Loadly Driver is a mobile application for drivers to:

- **Go online/offline** — Toggle availability for trip requests
- **Track location** — Share live location with the platform
- **Manage trips** — View trip list and details
- **Receive notifications** — Push notifications via Firebase Cloud Messaging
- **Manage profile** — Update personal data, documents, and settings
- **Authenticate** — Login via password, OTP, or biometrics; MFA support
- **Stay up to date** — Force or optional update prompts on startup via the version-check API

---

## Flutter Version (FVM)

This project uses [FVM](https://fvm.app/) with Flutter **3.41.2**.

- **First time / after clone:**  
  `fvm install`
- **Run the app:**  
  `fvm flutter run`
- **Code generation:**  
  `fvm dart run build_runner build --delete-conflicting-outputs`
- **Other commands:**  
  Use `fvm flutter` / `fvm dart` instead of `flutter` / `dart` (e.g. `fvm flutter pub get`, `fvm flutter analyze`).

The version is pinned in `.fvmrc` (committed). The `.fvm/` directory is local (symlink + cache) and is gitignored. After clone, run `fvm install`. VS Code uses `.fvm/flutter_sdk` via `.vscode/settings.json` (`updateVscodeSettings` is off in `.fvmrc` so FVM does not overwrite it).

---

## Tech Stack

| Category            | Technology                                |
| ------------------- | ----------------------------------------- |
| **Framework**       | Flutter 3.41.2                            |
| **Language**        | Dart ^3.8.0                               |
| **State Management**| BLoC (flutter_bloc ^9.1.1)                |
| **Navigation**      | GoRouter ^17.1.0                           |
| **HTTP Client**     | Dio ^5.4.0                                 |
| **Architecture**    | Clean Architecture (domain/data/presentation) |
| **Maps**            | google_maps_flutter ^2.5.0                |
| **Location**        | geolocator ^14.0.2                        |
| **Push Notifications** | Firebase Cloud Messaging ^16.1.1       |
| **Error Monitoring**   | Sentry ^9.12.0                         |
| **Code Generation**   | freezed, json_serializable, build_runner |

---

## Project Structure

```text
lib/
├── main.dart                    # App entry, DI, initialization
├── core/                        # Shared infrastructure
│   ├── locale/                  # Localization (LocaleBloc)
│   ├── network/                 # API layer (Dio, endpoints)
│   ├── router/                  # GoRouter config
│   ├── services/                # FCM, biometrics, app update check
│   ├── config/                  # App store URLs (update flow)
│   ├── models/                  # Shared models (e.g. app update DTOs)
│   ├── storage/                 # Auth storage
│   ├── theme/                   # App theme, colors, layout
│   └── widgets/                 # Reusable UI components
├── features/                    # Feature modules (Clean Architecture)
│   ├── auth/                    # Authentication
│   ├── home/                    # Home, online/offline, location
│   ├── profile/                 # User profile
│   ├── trips/                   # Trips list & details
│   ├── notifications/           # Notifications screen
│   └── settings/                # App settings
└── l10n/                        # Localization (app_en.arb, app_ar.arb)
```

---

## Architecture

### Clean Architecture Layers

```text
┌─────────────────────────────────────────────────────────────┐
│  Presentation (UI, BLoC, Views)                             │
├─────────────────────────────────────────────────────────────┤
│  Domain (Entities, Use Cases, Repository Interfaces)         │
├─────────────────────────────────────────────────────────────┤
│  Data (Repositories Impl, Data Sources, DTOs)               │
└─────────────────────────────────────────────────────────────┘
```

### Data Flow

```text
UI (Screen) → BLoC Event → Use Case → Repository → Remote Data Source → Dio → API
```

### Dependency Injection

Manual DI in `main.dart`:

- **Shared:** Dio client, AuthStorage
- **Auth:** AuthRemoteDataSource → AuthRepository → AuthBloc + use cases
- **Home:** HomeRemoteDataSource → HomeRepository → HomeBloc + use cases
- **Profile:** ProfileBloc (uses AuthRepository)
- **Trips:** TripsRemoteDataSource → TripsRepository → TripsBloc + use cases

---

## Setup & Configuration

### Prerequisites

- Flutter 3.41.2 (via [FVM](https://fvm.app/))
- Dart ^3.8.0

### First-Time Setup

```bash
fvm install
fvm flutter pub get
fvm dart run build_runner build --delete-conflicting-outputs
```

### Environment Variables (Dart Defines)

| Variable              | Default                                       | Description                |
| --------------------- | --------------------------------------------- | -------------------------- |
| `DRIVER_API_BASE_URL` | `https://stg-api-agg.loadly-eg.com/api/driver` | API base URL               |
| `SENTRY_DSN`          | (configured)                                  | Sentry error reporting DSN |
| `IOS_APP_STORE_ID`    | (empty)                                       | Numeric App Store id for iOS update links |
| `HUAWEI_APP_GALLERY_ID` | (empty)                                     | Huawei AppGallery app id for AppGallery builds |
| `APP_STORE_DISTRIBUTION` | `play`                                     | Android store channel: `play` or `harmonyos` |
| `APP_STORE_URL`       | (empty)                                       | Full store URL override for update prompts |

**Example (release build):**

```bash
fvm flutter build apk --dart-define=DRIVER_API_BASE_URL=https://api.example.com/api/driver
```

### Firebase

- Firebase is initialized in `main.dart`
- Add `google-services.json` (Android) and `GoogleService-Info.plist` (iOS) to the project
- FCM is used for push notifications

### Sentry

- Project: `loadly-loadly-driver`
- Org: `fuelin-wr`
- Configured in `pubspec.yaml`; DSN via `--dart-define=SENTRY_DSN=...`
- Enabled in release mode

---

## Features & Screens

### Auth Screens (`features/auth/`)

| Screen          | Route               | Description                          |
| --------------- | ------------------- | ------------------------------------ |
| Login           | `/login`            | Entry point; phone or password login |
| Password Login  | `/password-login`   | Email/password login                 |
| OTP             | `/otp`              | OTP verification                     |
| Verification    | `/verification`     | Choose verification channel          |
| MFA Channel     | `/mfa-channel`      | MFA setup (Email/SMS)                |
| Change Password | `/change-password` | Change password flow                  |
| Verify Email    | `/verify-email`     | Email verification                   |

### Home (`features/home/`)

| Screen | Route   | Description                                |
| ------ | ------- | ------------------------------------------ |
| Home   | `/home` | Map, online/offline toggle, location tracking |

### Profile (`features/profile/`)

| Screen  | Route      | Description                                      |
| ------- | ---------- | ------------------------------------------------ |
| Profile | `/profile` | Profile summary, files, personal data, logout    |

### Trips (`features/trips/`)

| Screen       | Route       | Description      |
| ------------ | ----------- | ---------------- |
| Trips List   | `/trips`    | List of trips    |
| Trip Details | `/trips/:id`| Trip information |

### Notifications & Settings

| Screen        | Route            | Description             |
| ------------- | ----------------- | ----------------------- |
| Notifications | `/notifications`  | Push notifications list |
| Settings      | `/settings`       | App settings           |

---

## API Integration

### Base URL

- Default: `https://stg-api-agg.loadly-eg.com/api/driver`
- Override: `--dart-define=DRIVER_API_BASE_URL=...`

### Interceptors (Order)

1. **DeviceInterceptor** — Device info headers
2. **LanguageInterceptor** — `lang` header (ar/en)
3. **FcmTokenInterceptor** — FCM token header
4. **AuthInterceptor** — Bearer token; on 401: clear tokens, redirect to `/login`
5. **ChuckerDioInterceptor** — Debug HTTP inspector (dev only)

### Endpoints

#### Auth Endpoints

| Method | Path                         | Description           |
| ------ | ---------------------------- | --------------------- |
| POST   | `/auth/login`                | Password login        |
| POST   | `/auth/send-login-otp`       | Send OTP to phone     |
| POST   | `/auth/login-with-otp`       | Login with OTP        |
| POST   | `/auth/logout`               | Logout                |
| GET    | `/auth/profile`              | Get profile           |
| PUT    | `/auth/profile`              | Update profile        |
| POST   | `/auth/forget-password`      | Request password reset|
| POST   | `/auth/reset-password`       | Reset password        |
| POST   | `/auth/reset-password-with-code` | Reset with OTP code |
| POST   | `/auth/change-password`      | Change password       |
| POST   | `/auth/send-mfa-code`        | Send MFA code         |
| POST   | `/auth/verify-mfa`           | Verify MFA            |
| POST   | `/auth/enable-mfa`           | Enable MFA            |
| POST   | `/auth/disable-mfa`          | Disable MFA           |
| POST   | `/auth/verify-email`         | Verify email          |
| POST   | `/auth/resend-verification`  | Resend verification   |

#### Home / Driver Status

| Method | Path               | Description     |
| ------ | ------------------ | --------------- |
| POST   | `/go-online`       | Go online       |
| POST   | `/go-offline`      | Go offline      |
| POST   | `/location/update` | Update location |

#### Trips Endpoints

| Method | Path         | Description   |
| ------ | ------------ | ------------- |
| GET    | `/trips`     | List trips    |
| GET    | `/trips/:id` | Trip details  |

#### App Version Check

| Method | Path                  | Description                                      |
| ------ | --------------------- | ------------------------------------------------ |
| GET    | `/app-version/check`  | Compare client version against min/latest policy |

Query parameters: `platform` (`Android` / `iOS` / `HarmonyOS`), `appType` (`Driver`), `version` (installed app version from `package_info_plus`).

Response `data` fields used by the client:

| Field            | Type   | Description                                      |
| ---------------- | ------ | ------------------------------------------------ |
| `clientVersion`  | string | Echo of the requested version                    |
| `minVersion`     | string | Minimum supported version                        |
| `latestVersion`  | string | Latest published version                         |
| `forceUpdate`    | bool   | Server flag to require an update                 |
| `softUpdate`     | bool   | Server flag to suggest an optional update        |
| `storeUrl`       | string | Play Store / App Store URL (optional override)   |

### Error Handling

- Uses `dartz` `Either<Failure, Success>`
- Failure types: `AuthFailure`, `HomeFailure`, `TripsFailure`
- 401: Auto-logout and redirect to login

---

## Authentication

### Login Methods

1. **Password Login** — Email + password → `LoginUseCase` → API → store tokens
2. **OTP Login** — Phone → `SendLoginOtpUseCase` → OTP + token → `LoginWithOtpUseCase` → store tokens
3. **Biometric Login** — `BiometricService` reads credentials from secure storage → password login

### Multi-Factor Authentication (MFA)

- **Enable:** Choose channel (Email/SMS) → `EnableMfaUseCase`
- **Verify:** `SendMfaCodeUseCase` → `VerifyMfaUseCase`
- **Disable:** `DisableMfaUseCase`

### Session Management

- Tokens in `AuthStorage` (SharedPreferences)
- Access token in `Authorization: Bearer <token>`
- Session checked on startup (splash → `/home` or `/login`)
- 401 → clear tokens, redirect to `/login`

### Password Reset

1. Forget password → enter identifier → choose channel
2. Receive OTP
3. Reset with code + new password → `ResetPasswordWithCodeUseCase`

---

## State Management

### BLoCs

| BLoC         | Responsibility                                      |
| ------------ | ---------------------------------------------------- |
| **AuthBloc** | Login, logout, MFA, profile, password flows          |
| **HomeBloc** | Online/offline, location tracking, initial location  |
| **ProfileBloc** | Profile data, update profile, logout              |
| **TripsBloc**  | Trips list, trip details                           |
| **LocaleBloc** | Language switching (en/ar)                          |

### Event → State Flow

```text
User Action → Event → BLoC → Use Case → Repository → State → UI
```

---

## Routing & Navigation

### GoRouter Configuration

- **Splash** (`/`) — Runs a 2s splash while checking for app updates in parallel; then routes to `/home` or `/login` (see [App Updates](#app-updates))
- **Chucker** — `ChuckerFlutter.navigatorKey` is passed to `GoRouter.navigatorKey` (not the deprecated `navigatorObserver`) so the HTTP inspector works reliably with nested navigators
- **Login** (`/login`) — Entry for unauthenticated users
- **Home** (`/home`) — Main screen (authenticated)
- **Profile** (`/profile`), **Trips** (`/trips`), **Notifications** (`/notifications`), **Settings** (`/settings`)
- **Auth flows** — `/password-login`, `/otp`, `/verification`, `/mfa-channel`, `/change-password`, `/verify-email`
- **Trip details** — `/trips/:id`

### Bottom Navigation

Home, Trips, Notifications, Profile — via `MainScaffold` and `AppBottomNavBar`.

---

## Localization

### Supported Languages

- **English (en)** — Default, LTR, Manrope font
- **Arabic (ar)** — RTL, Almarai font

### Implementation

- ARB files: `l10n/app_en.arb`, `l10n/app_ar.arb`
- Generated: `l10n/gen/app_localizations.dart`
- `LocaleBloc` — current locale, persisted in SharedPreferences
- API `lang` header set by `LanguageInterceptor`

### Usage

```dart
final l10n = AppLocalizations.of(context)!;
Text(l10n.appTitle)
```

---

## Services & Infrastructure

### FCM Service (`fcm_service.dart`)

- Request permissions, get token
- Token sent in API headers via `FcmTokenInterceptor`
- Background handler: `fcm_background_handler.dart`

### Biometric Service (`biometric_service.dart`)

- Check availability, get biometric types
- Enable/disable biometric login
- Store credentials in `FlutterSecureStorage`
- Authenticate with `local_auth` 3.0 API

### Auth Storage (`auth_storage.dart`)

- Access token, refresh token, user ID
- Online status
- Init before app run

---

## App Updates

On startup, the splash screen checks the driver API for a version policy and shows a prompt when an update is required or recommended.

### Flow

```text
Splash (/)
  ├─ Parallel: 2s delay + GET /app-version/check
  ├─ Force update → blocking full-screen prompt (no navigation)
  ├─ Up to date → /home or /login
  └─ Optional update → navigate first, then show dismissible dialog
```

1. **`AppUpdateService`** reads the installed version via `package_info_plus`, calls `/app-version/check`, and maps the response with **`AppVersionCheckResolver`**.
2. **`AppUpdateInterceptor`** also reads version policy from **login** and **profile** API envelopes (`/auth/login`, `/auth/login-with-otp`, `/auth/verify-mfa`, `GET /auth/profile`) and stores a pending prompt in **`AppUpdateNotifier`**.
3. **`VersionComparator`** compares semantic versions (e.g. `1.2.3`, `1.2.3+4`).
4. **`showAppUpdate`** (in `app_update_presenter.dart`) presents the UI:
   - **Force** — full-screen `AppUpdateScreen`; back navigation blocked; user must open the store.
   - **Optional** — `CupertinoAlertDialog` with **Update** and **Later**.
5. **Store link** — `openAppStoreListing()` opens `storeUrl` from the API, or falls back to **`AppStoreConfig.storeUrl`**.

Login/profile responses can include version policy as:

- A nested object: `appVersionCheck`, `versionCheck`, or `appVersion` (on the envelope root or inside `data`)
- Or flat fields on `data`: `minVersion`, `latestVersion`, `forceUpdate`, `softUpdate`, `storeUrl`

After a successful login or profile load, **`presentPendingAppUpdate`** / **`handleLoginNavigationWithAppUpdate`** show the prompt (force updates block navigation; optional updates appear after navigation, same as splash).

### Huawei AppGallery

The app does **not** auto-detect Huawei devices. AppGallery is supported via an explicit **AppGallery build** or server-provided `storeUrl`:

| Approach | When to use |
| -------- | ----------- |
| API `storeUrl` | Backend returns the correct listing URL per platform (recommended) |
| `APP_STORE_URL` | One-off override for any store |
| `APP_STORE_DISTRIBUTION=harmonyos` + `HUAWEI_APP_GALLERY_ID` | Dedicated Huawei APK; sends `platform=HarmonyOS` to the version-check API |

**Example (Huawei AppGallery release):**

```bash
fvm flutter build apk \
  --dart-define=APP_STORE_DISTRIBUTION=harmonyos \
  --dart-define=HUAWEI_APP_GALLERY_ID=101234567
```

Default store URL for AppGallery builds: `https://appgallery.huawei.com/app/C{id}`.

### When an update is shown

| Condition | Update type |
| --------- | ----------- |
| `forceUpdate == true` | Force |
| Installed version &lt; `minVersion` | Force |
| `softUpdate == true` | Optional |
| Installed version &lt; `latestVersion` | Optional |

If the version check fails (network error, malformed response), the app continues normally without blocking the user.

### Store URL configuration

Configured in `lib/core/config/app_store_config.dart`:

| Source | Behavior |
| ------ | -------- |
| `APP_STORE_URL` dart-define | Used when set (full URL override) |
| `IOS_APP_STORE_ID` dart-define | iOS App Store link: `https://apps.apple.com/app/id{id}` |
| `APP_STORE_DISTRIBUTION=harmonyos` + `HUAWEI_APP_GALLERY_ID` | AppGallery link: `https://appgallery.huawei.com/app/C{id}` |
| Default | Android Play Store: `com.loadly.driver` |

**Example (iOS release with App Store id):**

```bash
fvm flutter build ipa --dart-define=IOS_APP_STORE_ID=1234567890
```

### Initialization

`AppUpdateService.init(dio)` is called from `main.dart` after the shared `Dio` client is created. The splash screen uses `AppUpdateService.instance?.checkForUpdate()`.

### Key files

| File | Purpose |
| ---- | ------- |
| `lib/core/services/app_update_service.dart` | Fetches version policy from API |
| `lib/core/network/app_update_interceptor.dart` | Parses version policy from login/profile responses |
| `lib/core/services/app_version_envelope_parser.dart` | Extracts version DTO from API envelopes |
| `lib/core/widgets/app_update_flow.dart` | Presents pending updates after login/profile |
| `lib/core/services/app_version_check_resolver.dart` | Maps API DTO → force/optional prompt |
| `lib/core/models/app_update_info.dart` | Update prompt model |
| `lib/core/models/app_version_check_dto.dart` | API response DTO |
| `lib/core/utils/version_comparator.dart` | Semantic version comparison |
| `lib/core/config/app_store_config.dart` | Default store URLs |
| `lib/core/widgets/app_update_presenter.dart` | Dialog / route presentation |
| `lib/core/widgets/app_update_screen.dart` | Full-screen update UI |
| `lib/core/router/app_router.dart` | Splash integration |

---

## Code Generation

### Commands

```bash
fvm dart run build_runner build --delete-conflicting-outputs
```

### Generated Files

- **Freezed:** `*.freezed.dart` — Immutable models, unions, copyWith
- **JSON Serializable:** `*.g.dart` — `fromJson` / `toJson`

### Regenerate After Changes

Run `build_runner` after changing:

- `@freezed` classes
- `@JsonSerializable` classes

---

## Testing

### Run Tests

```bash
fvm flutter test
```

### Test Structure

- `test/widget_test.dart` — Widget tests

---

## Key Files Reference

| File                                              | Purpose                    |
| ------------------------------------------------- | -------------------------- |
| `lib/main.dart`                                      | App init, DI, Firebase, Sentry |
| `lib/core/router/app_router.dart`                    | Routes, splash, redirects, update check |
| `lib/core/services/app_update_service.dart`         | Startup version-check API      |
| `lib/core/widgets/app_update_presenter.dart`        | Force/optional update UI       |
| `lib/core/config/app_store_config.dart`             | Store listing URLs             |
| `lib/core/network/api_client.dart`                  | Dio client, interceptors       |
| `lib/core/network/api_endpoints.dart`               | Endpoint constants             |
| `lib/core/storage/auth_storage.dart`                | Token storage                  |
| `lib/core/services/fcm_service.dart`                | Push notifications             |
| `lib/core/services/biometric_service.dart`          | Biometric auth                 |
| `lib/features/auth/presentation/bloc/auth_bloc.dart`| Auth state                     |
| `lib/features/home/presentation/bloc/home_bloc.dart` | Home state                     |
| `lib/features/profile/presentation/bloc/profile_bloc.dart` | Profile state          |
| `lib/features/trips/presentation/bloc/trips_bloc.dart` | Trips state               |
| `lib/core/locale/bloc/locale_bloc.dart`              | Locale state                   |
| `pubspec.yaml`                                       | Dependencies, Sentry config    |
| `.fvmrc`                                             | Flutter version (3.41.2)       |

---

## Summary

Loadly Driver uses **Clean Architecture** with **BLoC** for state management, **GoRouter** for navigation, and **Dio** with interceptors for API calls. It supports **Firebase** push notifications, **biometric** authentication, **English/Arabic** localization, **Google Maps** for location, **Sentry** for error monitoring, and **force/optional app update** prompts on startup. The codebase is organized by features with clear domain/data/presentation separation.

---

## Documentation

The sections above are a product overview. When they disagree with the code, use these files:

- [documentation/ARCHITECTURE.md](documentation/ARCHITECTURE.md) — structure, data flow, networking, and startup
- [documentation/DEVELOPMENT.md](documentation/DEVELOPMENT.md) — setup, commands, and where to add code
- [documentation/TROUBLESHOOTING.md](documentation/TROUBLESHOOTING.md) — build and runtime problems
- [documentation/TRIP_ROUTE_SYNC.md](documentation/TRIP_ROUTE_SYNC.md) — GPS route column codec and `POST /trips/{id}/route-sync` API

## Getting Started

- [Flutter documentation](https://docs.flutter.dev/)
- [Lab: Write your first Flutter app](https://docs.flutter.dev/get-started/codelab)
- [Cookbook: Useful Flutter samples](https://docs.flutter.dev/cookbook)
