# Troubleshooting

Problems that show up in **this** repository, plus a few general Flutter checks labeled as such. Project overview is in [README.md](../README.md). Architecture background is in [ARCHITECTURE.md](ARCHITECTURE.md). Setup commands are in [DEVELOPMENT.md](DEVELOPMENT.md).

## Dependency and SDK issues

### Packages fail to resolve, or the SDK is too old

The pin is Flutter **3.47.2** (`.fvmrc`) and Dart **^3.12.0** (`pubspec.yaml`). CI uses the same Flutter version.

```bash
fvm install
fvm use
fvm flutter pub get
```

**General:** if `fvm` is missing, install Flutter 3.47.2 yourself. A newer stable channel can still fail constraint checks when `pubspec.yaml` lags behind it.

### `*.g.dart` errors after a DTO change

Auth and trip DTOs use `json_serializable`. Generated files are committed. If you change `@JsonSerializable` classes and the generated code is stale:

```bash
dart run build_runner build --delete-conflicting-outputs
```

`freezed` is a dev dependency, but no `@freezed` classes were found under `lib/`. Do not expect `build_runner` to generate Freezed files for existing models.

### Localization key not found

Sources: `lib/l10n/app_en.arb`, `lib/l10n/app_ar.arb`. Output: `lib/l10n/gen/`.

```bash
flutter gen-l10n
```

Add the key to **both** ARB files. The template file is `app_en.arb` (`l10n.yaml`).

## Build failures

### Android release: `key.properties` missing

`android/app/build.gradle.kts` throws when a release assemble, bundle, package, or sign task runs and `android/key.properties` is absent:

```text
Release builds require android/key.properties.
Copy android/key.properties.example and configure your release keystore.
```

```bash
cp android/key.properties.example android/key.properties
```

Set `storePassword`, `keyPassword`, `keyAlias`, and `storeFile`. `storeFile` is relative to `android/`. Debug and `flutter run` do not need this file.

### Android: wrong JDK

The app module targets **Java 17** and Kotlin JVM 17. CI uses Temurin 17. JDK 11 will not compile this project.

`android/gradle.properties` sets `kotlin.incremental=false`. The comment there says this avoids cached storage errors on Windows.

**General:** if Gradle downloads fail, check network access to Maven and that `JAVA_HOME` points at JDK 17.

### `compileSdk` 36 not installed

`android/app/build.gradle.kts` sets `compileSdk = 36`. Install Android SDK Platform 36 from the SDK manager.

`minSdk` and `targetSdk` are not hardcoded. They follow the Flutter Gradle plugin values for the SDK you are using.

### iOS build on Windows

iOS compilation needs macOS and Xcode. This is a general Flutter limit, not a script in the repo. `ios/Podfile` is not checked in; the first iOS build on a Mac generates CocoaPods files.

### CI APK

`.github/workflows/build-apk.yml` runs on push to `main`. It needs the Android keystore secrets listed in [DEVELOPMENT.md](DEVELOPMENT.md). A local debug build does not use those secrets. The workflow reads `version:` from `pubspec.yaml`; it does not commit a version bump.

## Android and iOS runtime issues

### Biometrics on Android

`MainActivity` is a `FlutterFragmentActivity` (`android/app/src/main/kotlin/com/example/contractor_app/MainActivity.kt`). `local_auth` needs that base class. If biometrics throw on Android, confirm `MainActivity` still extends `FlutterFragmentActivity`.

The remembered password used after a successful biometric prompt is in `FlutterSecureStorage` (`AuthStorage.getRememberedPassword`). Biometric login does nothing useful if remember-me never stored a password.

### Face ID, camera, or photo library on iOS

`ios/Runner/Info.plist` does not declare `NSFaceIDUsageDescription`, `NSCameraUsageDescription`, or photo-library usage keys. Registration and add-driver use the camera/gallery (`image_picker`). iOS can crash or reject the binary until those keys exist. **Needs clarification** for the production wording.

### Android camera and photos

The manifest declares `CAMERA` and `READ_EXTERNAL_STORAGE` (max SDK 32). The app shows `prominent_user_data_disclosure.dart` before requesting those permissions. On Android 13+ the flow is expected to use the system picker. If a permission dialog never appears, check whether the disclosure dialog was dismissed.

### Bundle id mismatch

| Platform | Id in repo |
| --- | --- |
| Android `applicationId` | `com.loadly.contractor` |
| iOS `PRODUCT_BUNDLE_IDENTIFIER` | `com.example.contractorApp` |

Store links for Android use `com.loadly.contractor` (`AppStoreConfig`). An iOS update link needs `--dart-define=IOS_APP_STORE_ID=...`. **Needs clarification** if the iOS id is still a placeholder.

### Launcher name

Android `android:label` is `Loadly Contractor` (`android/app/src/main/AndroidManifest.xml`). iOS `CFBundleDisplayName` is `Contractor App` and `CFBundleName` is `contractor_app` (`ios/Runner/Info.plist`). Nothing in the repo keeps those strings in sync.

## API and network

### Wrong environment (staging vs production)

Only one of these in `lib/core/network/api_client.dart` is active. The committed line is staging:

```dart
const String kContractorApiBaseUrl =
    'https://stg-api-agg.loadly-eg.com/api/contractor';
```

There is no `.env` switch. `dotenv` is unused. A dart-define does **not** change this URL.

Shared endpoints (vehicle types, app version) use `kApiBaseUrl`, which strips the `/contractor` suffix. If you point `kContractorApiBaseUrl` at a host that does not end with `/contractor`, those calls stay on the same string (`kApiBaseUrl` getter).

Timeouts are 30 seconds (`createContractorApiClient`).

### Requests missing `lang` or device headers

`LanguageInterceptor` reads SharedPreferences key `app_locale` (same key as `LocaleBloc`). Empty or failed prefs → `lang: ar`.

`DeviceInterceptor` swallows device-info errors and continues without device headers (`debugPrint` only). Desktop/web builds still get a fallback `DeviceInfo` of `unknown` when the platform is not Android, iOS, macOS, Windows, or Linux.

### HTTP 401 does not return to login

`AuthInterceptor` tries `AuthStorage.refreshToken()` first. Refresh uses a plain `Dio` (no interceptors) against `kContractorApiBaseUrl`.

It does **not** clear the session when:

- The path is in `_authAttemptPaths` (login, OTP, reset password, MFA, validate password, and the other paths listed in `api_client.dart`), or
- `AuthScreenGuard.isOnAuthScreen` is true (`OtpScreen` sets this).

When refresh fails on other calls, the interceptor clears tokens and tries `AppRouterHolder` from `lib/core/router/app_router_holder.dart`. `createAppRouter()` registers a **different** `AppRouterHolder` defined in `app_router.dart`. Nothing calls `setRouter` on the holder the interceptor imports, so `router` stays null and `go(login)` is skipped. Screens that listen for `AuthUnauthenticated` can still navigate to login after an explicit logout. A failed refresh alone may leave you on a protected route until the next `redirect`. See [ARCHITECTURE.md](ARCHITECTURE.md).

### Envelope parse errors

Clients expect `{ "success": ..., "data": ... }` (`requireEnvelope` in `api_response_parser.dart`). An empty body throws a `DioException` with error `Empty response body`. List screens also accept `TotalCount` and nested `pagination` / `meta`. If a new endpoint returns a different shape, map it in the data source instead of using the entity JSON directly.

### Chucker

Chucker is attached to the app Dio and `showOnRelease` is true. The inspector uses `ChuckerFlutter.navigatorKey`, which is also the `GoRouter` navigator key. If the overlay never appears, confirm you are looking at traffic from `createContractorApiClient()`, not the separate refresh `Dio` inside `AuthStorage`.

## Environment and configuration

### Sentry is silent in debug

`SentryFlutter.init` runs only in release mode, and only when the DSN looks valid (`main.dart`: non-empty, contains `sentry.io`, does not contain `your-other-dsn`). Override with `--dart-define=SENTRY_DSN=...`. A thrown init is caught and the app continues without Sentry.

### In-app update opens the wrong store

`AppStoreConfig.versionCheckPlatform` sends `iOS` on iOS, `HarmonyOS` when `APP_STORE_DISTRIBUTION` is `harmonyos` or `appgallery`, otherwise `Android`. The store URL falls back to Play Store `com.loadly.contractor` unless `APP_STORE_URL`, `IOS_APP_STORE_ID`, or `HUAWEI_APP_GALLERY_ID` is set. An empty iOS id on an iPhone build still produces the Play Store URL (see `storeUrl` in `app_store_config.dart`).

### Forced update on splash

`_SplashScreen` waits for `AppUpdateService.checkForUpdate()`. If `isForced` is true, it shows the update UI and does not `go` to home or onboarding. The check uses `package_info_plus` version (from `pubspec.yaml`) and `GET {kApiBaseUrl}/app-version/check?appType=Contractor`. A failed check is logged and treated as “no update” (`checkForUpdate` returns null). If splash never leaves and no update UI appears, the wait is the 3.5 second animation plus a hung request up to the Dio timeout. **Needs clarification** of the server policy for the version you are running.

### Firebase / push

`FCMService` only stores a string in memory. `main.dart` does not call `Firebase.initializeApp`. The Google Services Gradle plugin is not applied. Config files on disk do not mean push works.

## Authentication

### Session looks logged out after restart

`isLoggedIn()` is “access token string is non-empty” in SharedPreferences. `AuthStorage.init()` must finish before reads; `_runApp` awaits it. If prefs fail to init, getters return null.

Tokens are not in `FlutterSecureStorage`. Only the remember-me password is. Clearing app data clears the session.

### Remember me does not fill the password

`saveRememberMe` writes the login to SharedPreferences and the password to secure storage. `getRememberedPassword()` is async. A failed secure-storage read (platform keychain) returns null while the login can still be present.

### OTP screen and unexpected logout

While `OtpScreen` is mounted, `AuthScreenGuard.isOnAuthScreen` is true so a 401 does not clear tokens. If you leave that screen by a path that skips `dispose`, the flag could stay true. The only writer is `otp_screen.dart`.

### Profile id missing for home highlights

Home “new trip” ids are stored per contractor user id. `_runApp` fetches the profile when a token exists and the id is missing. If that call fails, highlights have no user key until a later successful `ProfileBloc` load (`saveContractorUserId`).

## Other runtime errors

### Edit booking request is a blank page

`/edit-booking-request` casts `state.extra` to `BookingRequest?`. A null extra builds `SizedBox.shrink()`. Open that route with the booking object in `extra`, the same way existing navigation does.

### Home and another trip screen show the wrong list

`TripBloc` is a single app-wide instance. `GetTrips` from home (`status: 1`, `openInviteOnly: true`) and `GetTrips` from my-trips or requests share that state. This is current behavior, not a second cache.

### Market tab

`/market` opens `MarketplaceScreen`, which shows the localized coming-soon text and a button back to home. There is no market API.

### Analyzer noise in platform projects

`analysis_options.yaml` excludes `android/`, `ios/`, `web/`, `windows/`, `macos/`, `linux/`, and `build/`. Run `flutter analyze` from the project root so those excludes apply.

### Tests

`flutter test` has no suite. A green test run does not mean the app was verified. `ios/RunnerTests` is a placeholder XCTest.
