# Loadly Customer.

Flutter customer app for Loadly. Signed-in company users view trip statistics, manage trips, track trips on a map, open cost and invoice reports, and receive notifications.

**Supported platforms:** Android and iOS. Flutter platform folders also exist for Web, macOS, Linux, and Windows. The only release workflow in the repo builds an Android APK.

**App version:** `1.0.3+8` (`pubspec.yaml`)

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
14. [Code Generation](#code-generation)
15. [Testing](#testing)
16. [Key Files Reference](#key-files-reference)

---

## Overview

Loadly Customer is a mobile application for company customers to:

- **View a dashboard** — Trip counts and monthly kilometers (`HomeBloc`)
- **Manage trips** — List, details, create, update, and delete
- **Track trips** — Map markers from `GET /company/trips/track` (not device GPS)
- **Open reports** — Cost tracking and invoices
- **Receive notifications** — Push notifications via Firebase Cloud Messaging, plus an in-app list
- **Manage profile** — Load and update personal data
- **Authenticate** — Password, OTP, or biometrics; MFA verification when the API requires it
- **Change settings** — Language, biometric login, change password, logout

---

## Flutter Version (FVM)

This project uses [FVM](https://fvm.app/) with Flutter **3.41.1**.

- **First time / after clone:**  
  `fvm install`
- **Run the app:**  
  `fvm flutter run`
- **Code generation:**  
  `fvm dart run build_runner build --delete-conflicting-outputs`
- **Other commands:**  
  Use `fvm flutter` / `fvm dart` instead of `flutter` / `dart` (e.g. `fvm flutter pub get`, `fvm flutter analyze`).

The version is pinned in `.fvmrc`. Commit `.fvmrc`; do not commit `.fvm/`.

---

## Tech Stack

| Category               | Technology                                    |
| ---------------------- | --------------------------------------------- |
| **Framework**          | Flutter 3.41.1                                |
| **Language**           | Dart ^3.8.0                                   |
| **UI**                 | Cupertino (`CupertinoApp.router`)             |
| **State Management**   | BLoC (flutter_bloc ^9.1.1)                    |
| **Navigation**         | GoRouter ^17.1.0                              |
| **HTTP Client**        | Dio ^5.4.0                                    |
| **Architecture**       | Clean Architecture (domain/data/presentation) |
| **Maps**               | google_maps_flutter ^2.5.0                    |
| **Push Notifications** | Firebase Cloud Messaging ^16.1.1              |
| **Error Monitoring**   | Sentry ^9.12.0 (release mode only)            |
| **Code Generation**    | json_serializable, build_runner               |

`freezed` is listed in `pubspec.yaml`, but no `@freezed` class exists under `lib/`. DTOs use `@JsonSerializable()`.

---

## Project Structure

```text
lib/
├── main.dart                    # App entry, DI, initialization
├── core/                        # Shared infrastructure
│   ├── config/                  # Store URLs, Google Maps dart-define
│   ├── locale/                  # Localization (LocaleBloc)
│   ├── network/                 # API layer (Dio, endpoints, interceptors)
│   ├── router/                  # GoRouter config
│   ├── services/                # FCM, local notifications, biometrics, app update
│   ├── storage/                 # Auth storage
│   ├── theme/                   # App theme, colors, layout
│   └── widgets/                 # Reusable UI components
├── features/                    # Feature modules (Clean Architecture)
│   ├── auth/                    # Authentication
│   ├── home/                    # Dashboard statistics
│   ├── trips/                   # Trip list, form, details, tracking
│   ├── reports/                 # Cost tracking and invoices
│   ├── notifications/           # In-app notification list
│   ├── profile/                 # Profile screen (uses auth use cases)
│   └── settings/                # Settings screen (no own data layer)
└── l10n/                        # Localization (app_en.arb, app_ar.arb, gen/)
```

Profile and settings are presentation-only. They call auth use cases and `AuthBloc` / `ProfileBloc` / `LocaleBloc`.

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

Repositories return `Either<Failure, Success>` from `dartz`. There is no local database.

### Dependency Injection

Manual DI in `main.dart` (no service locator):

- **Shared:** Dio client (`createCustomerApiClient`), `AuthStorage`
- **Auth:** `AuthRemoteDataSource` → `AuthImpl` → `AuthBloc` + use cases
- **Home:** `HomeRemoteDataSource` → `HomeImpl` → `HomeBloc` + use cases
- **Trips:** `TripsRemoteDataSource` → `TripsImpl` → `TripsBloc` + use cases
- **Reports:** `ReportsRemoteDataSource` → `ReportsImpl` → `ReportsBloc` + use cases
- **Notifications:** `NotificationsRemoteDataSource` → `NotificationsImpl` → `NotificationsBloc` + use cases
- **Profile:** `ProfileBloc` (uses `GetProfileUseCase` and `UpdateProfileUseCase` on `AuthRepository`)
- **Locale:** `LocaleBloc`

BLoCs are created once and provided at the root with `MultiBlocProvider`.

---

## Setup & Configuration

### Prerequisites

- Flutter 3.41.1 (via [FVM](https://fvm.app/))
- Dart ^3.8.0

### First-Time Setup

```bash
fvm install
fvm flutter pub get
fvm dart run build_runner build --delete-conflicting-outputs
```

### Environment Variables (Dart Defines)

| Variable                  | Default in source                                      | Description                                      |
| ------------------------- | ------------------------------------------------------ | ------------------------------------------------ |
| `CUSTOMER_API_BASE_URL`   | `https://stg-api-agg.loadly-eg.com/api`                | API root. Paths already include `/company/...`   |
| `SENTRY_DSN`              | Set in `lib/main.dart`                                 | Sentry DSN. Used only in release mode            |
| `GOOGLE_MAPS_API_KEY`     | Set in `lib/core/config/google_maps_config.dart`       | Dart-side Maps key. Does not replace native keys |
| `IOS_APP_STORE_ID`        | Empty                                                  | Numeric App Store id for the update flow         |
| `HUAWEI_APP_GALLERY_ID`   | Empty                                                  | AppGallery id                                    |
| `APP_STORE_DISTRIBUTION`  | `play`                                                 | `harmonyos` or `appgallery` selects Huawei       |
| `APP_STORE_URL`           | Empty                                                  | Full store URL override                          |

**Example (release build):**

```bash
fvm flutter build apk --dart-define=CUSTOMER_API_BASE_URL=https://api.example.com/api
```

Do not append `/company` to the base URL.

Native Google Maps keys are separate from the Dart define:

- Android: `com.google.android.geo.API_KEY` in `android/app/src/main/AndroidManifest.xml`
- iOS: `GMSServices.provideAPIKey` in `ios/Runner/AppDelegate.swift`

### Firebase

- `Firebase.initializeApp()` is called in `main.dart` with no Dart options (no `firebase_options.dart`)
- Native config is `android/app/google-services.json` and `ios/Runner/GoogleService-Info.plist`
- Init errors are logged and the app continues
- FCM is used for push notifications. The token is sent on later API calls as the `fcmToken` header

### Sentry

- Project: `loadly-loadly-driver`
- Org: `fuelin-wr`
- Configured in `pubspec.yaml`; DSN via `--dart-define=SENTRY_DSN=...` or the default in `lib/main.dart`
- Enabled only in release mode, and only when the DSN looks like a Sentry DSN

### Android signing

Release builds read `android/key.properties` when that file exists (`keyAlias`, `keyPassword`, `storeFile`, `storePassword`). If it is missing, release uses the debug signing config.

Application id and iOS bundle id: `com.loadly.customer`.

---

## Features & Screens

### Auth Screens (`features/auth/`)

| Screen          | Route           | Description                                      |
| --------------- | --------------- | ------------------------------------------------ |
| Splash          | `/`             | Logo, update check, then home or onboarding      |
| Onboarding      | `/onboarding`   | First screen when no refresh token is stored     |
| Login           | `/login`        | Phone OTP entry and biometric login              |
| Password Login  | `/auth/login`   | Email or phone plus password                     |
| OTP             | `/otp`          | OTP for login, forgot password, or MFA           |
| Verification    | `/verification` | Choose verification channel                      |
| MFA Channel     | `/mfa-channel`  | Choose Email or SMS, then send an MFA code       |
| Change Password | `/change-password` | Change password; opened from settings         |
| Verify Email    | `/verify-email` | Email verification                               |
| Register        | `/register`     | Local form only. Submit does not call the API    |
| Auth Success    | `/auth-success` | Success screen used by the register form         |

`/register` is not linked from any other screen.

### Home (`features/home/`)

| Screen | Route   | Description                                              |
| ------ | ------- | -------------------------------------------------------- |
| Home   | `/home` | Trip statistics and monthly kilometers, with date filters |

### Trips (`features/trips/`)

| Screen         | Route           | Description                                      |
| -------------- | --------------- | ------------------------------------------------ |
| Trips List     | `/trips`        | Paginated, filterable trip list                  |
| Trip Details   | `/trips/:tripId`| Trip information. `extra` must be a back-route `String` |
| Add / Edit     | `/add-trip`     | Create or edit. Optional query `tripId`. `extra` is `FormData` from `add_trip_screen.dart` |
| Add Success    | `/add-trip-success` | Confirmation after create                    |
| Track Trips    | `/track-trips`  | Google Map of trips returned by the track API    |

### Reports (`features/reports/`)

| Screen            | Route                         | Description                                      |
| ----------------- | ----------------------------- | ------------------------------------------------ |
| Reports           | `/reports`                    | Menu for cost tracking and invoices              |
| Cost Tracking     | `/cost-tracking`              | Trip cost period list                            |
| Cost Details      | `/cost-tracking/:tripId`      | `extra` must be a trips `Trip`                   |
| Invoices          | `/invoices`                   | Invoice list                                     |
| Invoice Details   | `/invoices/:invoiceId`        | `extra` must be an `Invoice`                     |
| Payment Requests  | `/invoice-payment-requests`   | Implemented, but the Reports menu card is commented out |

### Profile, Notifications & Settings

| Screen        | Route            | Description                                              |
| ------------- | ---------------- | -------------------------------------------------------- |
| Profile       | `/profile`       | Profile fields and photo. Opened from settings           |
| Notifications | `/notifications` | Notification list and mark-as-read                       |
| Settings      | `/settings`      | Language, biometric toggle, change password, logout      |

Settings is opened from `AppNavBar`, not from the bottom bar. Logout is `AuthLogoutRequested` on `AuthBloc`, not `ProfileBloc`.

---

## API Integration

### Base URL

- Default: `https://stg-api-agg.loadly-eg.com/api`
- Override: `--dart-define=CUSTOMER_API_BASE_URL=...`
- A production host is present only as a comment in `lib/core/network/api_client.dart`

Timeouts are 30 seconds. Requests send `Accept` and `Content-Type` `application/json`.

### Interceptors (Order)

1. **DeviceInterceptor** — `device-id`, `device-model`, `device-os`, `device-os-version`, `app-version`, `app-build-number`
2. **LanguageInterceptor** — `lang` header (`ar` or `en`) from SharedPreferences key `app_locale`. Missing locale defaults to `ar`
3. **FcmTokenInterceptor** — `fcmToken` header when a token is cached
4. **AuthInterceptor** — `Authorization: Bearer <access token>`. On 401, refresh via `POST /company/auth/refresh-token`, then retry. If refresh fails, clear tokens and go to `/login`, except for the auth-attempt paths in `AuthInterceptor`
5. **AppUpdateInterceptor** — Reads an app-version envelope from successful login, OTP login, MFA verify, and GET profile
6. **ChuckerDioInterceptor** — In-app HTTP inspector. `ChuckerFlutter.showOnRelease` is `true`, so it is not limited to debug builds

### Endpoints

Constants live in `lib/core/network/api_endpoints.dart`.

#### Auth Endpoints

| Method | Path                                 | Description            |
| ------ | ------------------------------------ | ---------------------- |
| POST   | `/company/auth/login`                | Password login         |
| POST   | `/company/auth/send-login-otp`       | Send login OTP         |
| POST   | `/company/auth/login-with-otp`       | Login with OTP         |
| POST   | `/company/auth/logout`               | Logout                 |
| GET    | `/company/auth/profile`              | Get profile            |
| PUT    | `/company/auth/profile`              | Update profile         |
| POST   | `/company/auth/forget-password`      | Request password reset |
| POST   | `/company/auth/reset-password`       | Reset with token (wired in the bloc; no screen sends it) |
| POST   | `/company/auth/reset-password-with-code` | Reset with OTP code |
| POST   | `/company/auth/change-password`      | Change password        |
| POST   | `/company/auth/send-mfa-code`        | Send MFA code          |
| POST   | `/company/auth/verify-mfa`           | Verify MFA             |
| POST   | `/company/auth/enable-mfa`           | Enable MFA (use case only; no screen dispatches it) |
| POST   | `/company/auth/disable-mfa`          | Disable MFA (use case only; no screen dispatches it) |
| POST   | `/company/auth/verify-email`         | Verify email           |
| POST   | `/company/auth/resend-verification`  | Resend verification    |
| POST   | `/company/auth/refresh-token`        | Refresh access token   |
| POST   | `/company/auth/validate-password`    | Validate password      |

#### Home

| Method | Path                                          | Description              |
| ------ | --------------------------------------------- | ------------------------ |
| GET    | `/company/dashboard/trips`                    | Trip statistics          |
| GET    | `/company/dashboard/monthly-trips-kilometers` | Monthly kilometers       |

#### Trips Endpoints

| Method | Path                                         | Description                    |
| ------ | -------------------------------------------- | ------------------------------ |
| GET    | `/company/trips`                             | List trips                     |
| GET    | `/company/trips/:id`                         | Trip details                   |
| POST   | `/company/trips`                             | Create trip                    |
| PUT    | `/company/trips/:id`                         | Update trip                    |
| DELETE | `/company/trips/:id`                         | Delete trip                    |
| GET    | `/company/trips/track`                       | Positions for the map          |
| GET    | `/lookups/contractreferencenumbers/default`  | Contract reference lookups     |
| GET    | `/lookups/vehicletypes/default`              | Vehicle type lookups           |
| GET    | `/lookups/contractdestinations/default`      | Destination lookups            |
| GET    | `/company/contracts/destinations/:id`        | One destination                |
| GET    | `/company/contracts/:contractId/consumption-type` | Consumption type          |
| GET    | `/company/maps/extract-place`                | Place from a maps link         |

#### Reports Endpoints

| Method | Path                                          | Description              |
| ------ | --------------------------------------------- | ------------------------ |
| GET    | `/company/dashboard/trip-cost-period-totals`  | Cost period totals       |
| GET    | `/company/dashboard/invoice-statistics`       | Invoice statistics       |
| GET    | `/company/invoices`                           | Invoice list             |
| GET    | `/company/invoice-payment-requests`           | Payment requests         |

#### Notifications and app update

| Method | Path                              | Description          |
| ------ | --------------------------------- | -------------------- |
| GET    | `/company/notifications`          | Notification list    |
| PATCH  | `/company/notifications/:id/read` | Mark one as read     |
| GET    | `/app-version/check`              | Forced/optional update (`appType=Company`) |

### Error Handling

- Uses `dartz` `Either<Failure, Success>`
- Failure types: `AuthFailure`, `HomeFailure`, `TripsFailure`, `ReportsFailure`, `NotificationsFailure`
- 401 on a protected call: try refresh, then clear tokens and redirect to `/login`
- Many BLoC error strings are English literals, not `AppLocalizations`

---

## Authentication

### Login Methods

1. **Password Login** — Email or phone + password → `LoginUseCase` → API → `AuthBloc` stores tokens
2. **OTP Login** — Phone → `SendLoginOtpUseCase` → OTP + token → `LoginWithOtpUseCase` → store tokens
3. **Biometric Login** — `BiometricService.authenticateAndGetCredentials` reads secure storage → `LoginUseCase`. That call passes locale `en`, not the current app locale

After `AuthLoginSuccess`, screens navigate from `LoginResult.redirect`:

- `mfa` → `/mfa-channel`
- `forget_password` → `/otp` with `forgetPassword=true`
- `mfaChannel == 0` → `/home`
- otherwise → `/mfa-channel`

### Multi-Factor Authentication (MFA)

- **Verify:** choose channel on `/mfa-channel` (`1` email, `2` SMS) → `SendMfaCodeUseCase` → `VerifyMfaUseCase`
- **Enable / disable:** `EnableMfaUseCase` and `DisableMfaUseCase` are constructed and handled by `AuthBloc`. No screen dispatches those events

### Session Management

- Access and refresh tokens in `AuthStorage` (`SharedPreferences`)
- Remembered password and biometric credentials in `FlutterSecureStorage`
- Access token sent as `Authorization: Bearer <token>`
- `isLoggedIn()` is true when a refresh token is stored. An access token alone does not count
- Splash waits 2 seconds, checks for an app update, then goes to `/home` or `/onboarding`
- Logged-in users who open an auth route are redirected to `/home`. Logged-out users who open any other route are redirected to `/login`
- 401 after a failed refresh → clear tokens, redirect to `/login`
- Logout calls the API, then clears tokens even if the API fails. It does not clear biometric credentials or the remembered password

### Password Reset

1. Forget password from the password login screen (channel `2` for a phone, `1` for email)
2. Receive OTP
3. Reset with code + new password → `ResetPasswordWithCodeUseCase`

---

## State Management

### BLoCs

| BLoC                  | Responsibility                                              |
| --------------------- | ----------------------------------------------------------- |
| **AuthBloc**          | Login, logout, OTP, MFA verify, password flows, biometrics |
| **HomeBloc**          | Trip statistics and monthly kilometers                      |
| **ProfileBloc**       | Load and update profile                                     |
| **TripsBloc**         | Trip list, details, create/update/delete, lookups, tracking |
| **ReportsBloc**       | Cost period, invoice statistics, invoices, payment requests |
| **NotificationsBloc** | Notification list and mark-as-read                          |
| **LocaleBloc**        | Language switching (en/ar), stored under `app_locale`       |

### Event → State Flow

```text
User Action → Event → BLoC → Use Case → Repository → State → UI
```

A foreground FCM message also calls `tripsBloc.add(TripsLoadTrips())` from `main.dart`.

---

## Routing & Navigation

### GoRouter Configuration

Defined in `lib/core/router/app_router.dart`. `AppRouterHolder` keeps the router so interceptors and notification handlers can navigate without a `BuildContext`.

- **Splash** (`/`) — 2s delay plus update check, then `/home` or `/onboarding`. A forced update stays on the update UI
- **Onboarding** (`/onboarding`) — Unauthenticated entry. Continues to `/login`
- **Login** (`/login`) — Phone OTP and biometric login
- **Home** (`/home`) — Dashboard (authenticated)
- **Main areas** — `/trips`, `/track-trips`, `/reports`, `/notifications`, `/settings`, `/profile`
- **Auth flows** — `/auth/login`, `/otp`, `/verification`, `/mfa-channel`, `/change-password`, `/verify-email`
- **Trip details** — `/trips/:tripId`

Detail routes cast `state.extra`. The wrong type throws when the route opens.

### Bottom Navigation

Home, Trips, Real-time (`/track-trips`), Reports, Notifications — via `MainScaffold` and `AppBottomNavBar`.

In `AppBottomNavBar.handleItemTap`, the Reports case has no `break` before Notifications, so a Reports tap also navigates to `/notifications`.

---

## Localization

### Supported Languages

- **Arabic (ar)** — Default when nothing is stored, RTL, Almarai (via `google_fonts`)
- **English (en)** — LTR, Archivo (via `google_fonts`)

### Implementation

- ARB files: `lib/l10n/app_en.arb`, `lib/l10n/app_ar.arb`
- Config: `l10n.yaml` (`flutter: generate: true`)
- Generated: `lib/l10n/gen/app_localizations.dart`
- `LocaleBloc` — current locale, persisted in SharedPreferences key `app_locale`
- API `lang` header set by `LanguageInterceptor` from that same key

### Usage

```dart
final l10n = AppLocalizations.of(context)!;
Text(l10n.appTitle)
```

Regenerate with `fvm flutter gen-l10n`.

---

## Services & Infrastructure

### FCM Service (`fcm_service.dart`)

- Permission, token, foreground messages, and notification taps
- Token sent in API headers via `FcmTokenInterceptor` (no separate register-token call in `main.dart`)
- Background handler: `fcm_background_handler.dart`
- Startup requests the system notification permission only on iOS. Android does not request it during startup
- `setupNotificationTapHandler` replaces the previous listener. `main.dart` registers it twice, so the second handler wins and opens `/trips`. Local-notification taps open `/home`

### Local notifications (`local_notification_service.dart`)

Shows foreground FCM messages. Android channels: `general_notifications` and `trip_schedule_update`. The manifest default channel id is `general_notifications`.

### Biometric Service (`biometric_service.dart`)

- Check availability and biometric types
- Enable/disable biometric login from settings
- Store login and password in `FlutterSecureStorage`
- Authenticate with `local_auth`
- Android `MainActivity` extends `FlutterFragmentActivity` so biometrics can run

Syncing `biometricFingerprint` back to the profile API is commented out in `SettingsScreen`.

### Auth Storage (`auth_storage.dart`)

- Access token, refresh token
- Remember-me flag and remembered login (SharedPreferences)
- Remembered password (secure storage)
- `init()` before the API client is created
- `refreshToken()` posts to `/company/auth/refresh-token`

### App update (`app_update_service.dart`)

Splash calls `GET /app-version/check` with the current version and `appType` `Company`. A forced update blocks leaving splash. Failures are ignored and startup continues.

---

## Code Generation

### Commands

```bash
fvm dart run build_runner build --delete-conflicting-outputs
```

`build.yaml` sets `json_serializable` to `explicit_to_json: true` and `field_rename: none`.

### Generated Files

- **JSON Serializable:** `*.g.dart` — `fromJson` / `toJson` for `@JsonSerializable()` DTOs

### Regenerate After Changes

Run `build_runner` after changing `@JsonSerializable` classes, and commit the updated `*.g.dart` files.

---

## Testing

### Run Tests

```bash
fvm flutter test
```

### Test Structure

- `test/widget_test.dart` — Pumps `LoadlyCustomerApp` and expects a `CupertinoApp`

That test builds `AuthBloc` and `LocaleBloc` itself. It does not run `main()` (no Firebase init, no `AuthStorage.init`, and no home, trips, reports, or notifications blocs). The APK workflow does not run tests.

---

## Key Files Reference

| File                                                         | Purpose                        |
| ------------------------------------------------------------ | ------------------------------ |
| `lib/main.dart`                                              | App init, DI, Firebase, Sentry |
| `lib/core/router/app_router.dart`                            | Routes, splash, redirects      |
| `lib/core/network/api_client.dart`                           | Dio client, interceptors, base URL |
| `lib/core/network/api_endpoints.dart`                        | Endpoint constants             |
| `lib/core/storage/auth_storage.dart`                         | Token storage                  |
| `lib/core/services/fcm_service.dart`                         | Push notifications             |
| `lib/core/services/biometric_service.dart`                   | Biometric auth                 |
| `lib/core/services/app_update_service.dart`                  | Store update check             |
| `lib/features/auth/presentation/bloc/auth_bloc.dart`         | Auth state                     |
| `lib/features/home/presentation/bloc/home_bloc.dart`         | Dashboard state                |
| `lib/features/profile/presentation/bloc/profile_bloc.dart`   | Profile state                  |
| `lib/features/trips/presentation/bloc/trips_bloc.dart`       | Trips state                    |
| `lib/features/reports/presentation/bloc/reports_bloc.dart`   | Reports state                  |
| `lib/features/notifications/presentation/bloc/notifications_bloc.dart` | Notifications state   |
| `lib/core/locale/bloc/locale_bloc.dart`                      | Locale state                   |
| `pubspec.yaml`                                               | Dependencies, Sentry config    |
| `.fvmrc`                                                     | Flutter version (3.41.1)       |
| `.github/workflows/build-apk.yml`                            | Release APK, S3, email, Teams  |

---

## Summary

Loadly Customer uses **Clean Architecture** with **BLoC** for state management, **GoRouter** for navigation, and **Dio** with interceptors for API calls. It supports **Firebase** push notifications, **biometric** authentication, **Arabic/English** localization, **Google Maps** for trip tracking, and **Sentry** for release error monitoring. Features are auth, a statistics home screen, trips, reports, notifications, profile, and settings.

---

## Further documentation

| Guide | Contents |
| --- | --- |
| [ARCHITECTURE.md](documentation/ARCHITECTURE.md) | Layers, DI, storage, and how features connect |
| [DEVELOPMENT.md](documentation/DEVELOPMENT.md) | Setup, commands, and where to add a feature |
| [TROUBLESHOOTING.md](documentation/TROUBLESHOOTING.md) | Build, device, API, and auth problems |
| [CHANGELOG.md](CHANGELOG.md) | Version history |

## Getting Started

- [Flutter documentation](https://docs.flutter.dev/)
- [Lab: Write your first Flutter app](https://docs.flutter.dev/get-started/codelab)
- [Cookbook: Useful Flutter samples](https://docs.flutter.dev/cookbook)
