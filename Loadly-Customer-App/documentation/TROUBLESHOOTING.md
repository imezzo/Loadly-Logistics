# Troubleshooting

Issues below are either visible in this repository or marked as general Flutter problems. Setup commands are in [DEVELOPMENT.md](DEVELOPMENT.md). Behavior of auth, routing, and the API client is in [ARCHITECTURE.md](ARCHITECTURE.md).

## Dependency and toolchain

### `flutter` is the wrong version

The project pins Flutter **3.41.1** in `.fvmrc`. A global `flutter` on `PATH` can be different.

```bash
fvm install
fvm flutter --version
fvm flutter pub get
```

Use `fvm flutter` / `fvm dart` for every command, including `build_runner`.

### `pub get` fails or packages do not resolve

`pubspec.yaml` requires Dart `^3.8.0`. That matches the Flutter 3.41.1 SDK. If resolution fails:

```bash
fvm flutter clean
fvm flutter pub get
```

Do not change version constraints to force a resolve unless you know the app still builds on 3.41.1.

### Missing `*.g.dart` or `fromJson` errors after a model change

DTOs use `json_serializable`. Generated files are committed, but they go stale when the annotated class changes.

```bash
fvm dart run build_runner build --delete-conflicting-outputs
```

`build.yaml` sets `explicit_to_json: true`. Nested objects need their own `toJson` or generation fails.

### Analyzer import warnings

`analysis_options.yaml` enables `prefer_relative_imports`. Inside `lib/`, import other project files with relative paths, not `package:loadly_customer/...`. Tests may use the package import; `test/widget_test.dart` does.

## Build failures

### Android release is signed with the debug key

`android/app/build.gradle` uses `android/key.properties` only when that file exists. Otherwise `release` uses `signingConfigs.debug`. Play upload will reject that build.

Add `keyAlias`, `keyPassword`, `storeFile`, and `storePassword`. Do not commit the keystore or passwords.

### Android: `flutter.sdk` not set

`android/settings.gradle` reads `flutter.sdk` from `android/local.properties`. That file is created by Flutter and is gitignored. Open the project with `fvm flutter` at least once, or set `flutter.sdk` to the FVM SDK path (`.fvm/versions/3.41.1`).

### Android: desugaring or Java errors

The app module enables `coreLibraryDesugaring` and sets Java 8. CI uses Temurin 17. A very new or very old JDK on the machine can fail the Gradle build. Use JDK 17 for local release builds if the error mentions the Java toolchain.

### iOS: `pod install` or `Generated.xcconfig` missing

From the Podfile: run `fvm flutter pub get` before `pod install`. `ios/Flutter/Generated.xcconfig` is produced by Flutter and is required to find `FLUTTER_ROOT`.

```bash
fvm flutter pub get
cd ios
pod install
```

The Podfile comments out `platform :ios`. The Xcode project sets `IPHONEOS_DEPLOYMENT_TARGET` to `12.0`. If a plugin requires a higher iOS version, CocoaPods will say so; raise the Podfile platform and the Xcode target together.

### CI APK uses staging and the keys in source

`.github/workflows/build-apk.yml` runs `fvm flutter build apk --release` with no `--dart-define`. The APK therefore uses:

- `CUSTOMER_API_BASE_URL` default in `lib/core/network/api_client.dart`
- `SENTRY_DSN` default in `lib/main.dart` (Sentry still starts only in release mode)
- `GOOGLE_MAPS_API_KEY` default in `lib/core/config/google_maps_config.dart`

Native map keys in the manifest and `AppDelegate.swift` are separate and are also compiled in as written.

### CI upload job does not run on GitHub-hosted runners

The `upload-and-notify` job is `runs-on: self-hosted`. It needs AWS credentials on that runner (`apk-presigner` and `smtp-fuelin` profiles), `secrets.S3_BUCKET`, and optionally `secrets.SES_FROM_EMAIL`, `vars.APK_EMAIL_TO`, and `secrets.TEAMS_WEBHOOK_URL`. An empty Teams webhook skips the notify step on purpose. This is environment configuration, not an app bug.

## Android and iOS runtime

### Blank or blue Google Map

`main.dart` initializes `AndroidMapRenderer.latest` before `runApp` because returning from the camera or background left a blank map on some devices. If maps are blank only on Android, confirm that block still runs and that `com.google.android.geo.API_KEY` in `AndroidManifest.xml` is a key allowed for this application id (`com.loadly.customer`).

iOS maps need `GMSServices.provideAPIKey` in `ios/Runner/AppDelegate.swift` and a key that allows the bundle id `com.loadly.customer`.

The Dart define `GOOGLE_MAPS_API_KEY` does not replace those native keys.

### Biometrics do nothing or crash on Android

`MainActivity` extends `FlutterFragmentActivity`. The manifest also declares `USE_BIOMETRIC` and `USE_FINGERPRINT`. Replacing `MainActivity` with `FlutterActivity` breaks `local_auth`.

The Kotlin file lives under `kotlin/com/loadly/customer/` while the package line is `com.loadly.customer`. Leave the package line aligned with `applicationId` / `namespace` (`com.loadly.customer`) if you move the file.

### Face ID prompt has no explanation (iOS)

`NSFaceIDUsageDescription` is set in `ios/Runner/Info.plist`. If you add another permission (camera, photos, location), iOS will terminate the app until the matching `NS*UsageDescription` exists. This app does not request location; trip pins come from the track-trips API.

### Push notifications never appear

Checked-in pieces:

- `google-services.json` (Android) and `GoogleService-Info.plist` (iOS)
- Android permission `POST_NOTIFICATIONS`
- Default channel id `general_notifications` in the manifest, matching `LocalNotificationService`
- `Firebase.initializeApp()` with no Dart options. A missing or wrong native plist/json is caught and logged; the rest of the app still starts

iOS: `FCMService.getToken` retries when the error text contains `apns-token-not-set` (up to 5 times, 1 second apart). A simulator often has no APNS token. Test pushes on a device with push capability.

Android 13+: `main()` requests notification permission during startup only on iOS (`requestRuntimeNotificationPermission: Platform.isIOS`). `showProminentUserDataDisclosure` is never called, and nothing else calls `FCMService.requestRuntimeNotificationPermission()`. On Android 13+ the system notification permission dialog is therefore not shown at startup.

Foreground messages are shown by `LocalNotificationService`. Background display is handled by the OS plus `firebaseMessagingBackgroundHandler`.

### Notification tap opens Trips, not Notifications

`main()` registers two tap handlers. `FCMService.setupNotificationTapHandler` cancels the previous listener, so only the second one runs: it goes to `/trips` (and records a trip-schedule update when the body parses). Local notification taps go to `/home`. A cold start from a notification waits 2.5 seconds, then goes to `/trips`.

Trip-schedule parsing only matches specific English phrases in `TripScheduleUpdate.tryParse`. Arabic bodies will not parse.

### Reports tab opens Notifications

In `AppBottomNavBar.handleItemTap`, `case 3` (`/reports`) has no `break` before `case 4` (`/notifications`). Tapping Reports calls `go('/reports')` and then `go('/notifications')`. That is the current code, not a router misconfiguration.

## API and network

### Every call goes to staging

Default base URL is the staging host in `kCustomerApiBaseUrl`. Override with `--dart-define=CUSTOMER_API_BASE_URL=...`. The path must be the API root (`.../api`), because `ApiEndpoints` already includes `/company/...`.

Timeouts are 30 seconds (`connectTimeout` and `receiveTimeout`). `ApiFailureKind.network` is only connection and timeout types. Other Dio errors become server failures and may surface the API `message`.

### 401 logs the user out

`AuthInterceptor` tries `POST /company/auth/refresh-token` with the stored refresh token, using a separate `Dio` that does not attach interceptors. If refresh fails, tokens are cleared and the router goes to `/login`.

Exceptions (no redirect) are the paths in `_authAttemptPaths`. The resend-verification entry is misspelled as `/companys/auth/resend-verification`. A 401 from `POST /company/auth/resend-verification` can therefore clear the session.

`isLoggedIn()` is true only when a refresh token is stored. An access token alone does not count as logged in, and splash will send the user to `/onboarding`.

### Inspect a failing request

Chucker is enabled, including release (`ChuckerFlutter.showOnRelease = true` in `main()`). The in-app inspector shows the headers added by the interceptors (`Authorization`, `lang`, `fcmToken`, device headers).

### Envelope or “Missing data” errors

`api_response_parser.dart` expects JSON with `success` and, for several calls, a `data` object. `success: false` throws a `DioException` whose `error` is the API `message` or the first `errors` entry. If the body shape changed, update the data source that calls `requireEnvelope` / `throwIfEnvelopeFailed`, not the screen.

### Language sent to the API is wrong

`LanguageInterceptor` reads SharedPreferences `app_locale` on each request. `LocaleBloc` writes that same key when the user changes language. If the key is missing, the header is `ar`.

## Configuration

### Sentry is silent in debug

`SentryFlutter.init` runs only when all of these are true: release mode, DSN non-empty, DSN contains `sentry.io`, DSN does not contain `your-other-dsn`. Debug and profile builds call `_runApp` without Sentry.

`pubspec.yaml` sentry org is `fuelin-wr` and project is `loadly-loadly-driver`. Symbol upload uses that project name. If events do not show up, confirm they are in that Sentry project, not a project named for the customer app.

### App update never appears

`AppUpdateService.checkForUpdate` swallows errors and returns null, so splash continues. The request is `GET /app-version/check` with `platform` (`Android`, `iOS`, or `HarmonyOS`), `appType` `Company`, and `version` from `PackageInfo`. A forced update blocks leaving splash. An optional update is shown after navigation.

Login, OTP login, MFA verify, and GET profile can also attach an update through `AppUpdateInterceptor`. Profile updates (non-GET `/company/auth/profile`) are ignored.

Store buttons need `IOS_APP_STORE_ID` or `APP_STORE_URL` for iOS. Android defaults to the Play listing for `com.loadly.customer`.

### Firebase “initialized” log is an error

`main()` catches `Firebase.initializeApp` failures and prints them. The usual cause is a missing or mismatched `google-services.json` / `GoogleService-Info.plist` for the application id. There is no `firebase_options.dart` to pass into `initializeApp`.

## Authentication

### Login succeeds in the API but the app stays on the login screen

`AuthBloc` saves tokens only when `LoginResult.accessToken` / `refreshToken` are non-null. Navigation then depends on `redirect` and `mfaChannel` (see [ARCHITECTURE.md](ARCHITECTURE.md)). `mfa` and the non-zero `mfaChannel` path go to `/mfa-channel`, not `/home`.

Splash and the router treat “logged in” as “refresh token present”. If the API returns only an access token, the next cold start goes to onboarding.

### OTP or MFA channel looks wrong

Screens send channel `1` for email and `2` for SMS (`PasswordLoginScreen`, `OtpScreen`, `MfaChannelScreen`). A comment on `AuthResendVerificationRequested` says `0` is email and `1` is SMS. If a resend hits the wrong channel, compare the integer actually posted with the API contract before changing the comment.

### Biometric login fails

Credentials are in `FlutterSecureStorage` (`biometric_login`, `biometric_password`, `biometric_enabled`). Enable from settings requires a profile phone or email (`_loginFromProfile`). If that is missing, settings shows `biometricAccountInfoUnavailable` and does not enable biometrics.

The call that would persist `biometricFingerprint` on the profile API is commented out in `SettingsScreen`. The device flag and the server profile can disagree.

### Remembered password is missing after reinstall

The login id is in SharedPreferences. The password is in secure storage. Clearing app data or a keystore reset on Android drops the secure value while the remember-me flag can still be set.

### Logout still shows the user as logged in

Logout always calls `AuthStorage.clearTokens()`, which removes access and refresh tokens. It does not clear biometric credentials or the remembered password. A later biometric login can sign in again. That matches the current `clearTokens()` implementation.

## Runtime errors in the UI

### Detail screen crashes on open

These routes cast `state.extra` and throw if it is missing or the wrong type:

| Route | Required `extra` |
| --- | --- |
| `/trips/:tripId` | `String` back route |
| `/add-trip` | `FormData` defined in `add_trip_screen.dart` |
| `/cost-tracking/:tripId` | `Trip` from `lib/features/trips/domain/entities/trip.dart` |
| `/invoices/:invoiceId` | `Invoice` |
| `/invoice-payment-requests/:id` | `InvoicePaymentRequest` |

`/trips/:tripId` also `int.parse`s the path segment.

### Register does not create an account

`RegisterScreen._submit` validates locally and navigates to `/auth-success`. It does not call `AuthRepository`. No other screen navigates to `/register`.

### Invoice payment requests are not on the Reports screen

The card that calls `context.go('/invoice-payment-requests')` is commented out in `ReportsScreen`. The route, bloc event, and API method still exist. Open the route directly only if you have a valid `extra` on the details page.

### `AppLocalizations.of(context)` is null

`LoadlyCustomerApp` passes `AppLocalizations.localizationsDelegates` and `supportedLocales`. A widget test or a second `CupertinoApp` that omits those delegates will throw when a screen reads `AppLocalizations.of(context)!`. `test/widget_test.dart` pumps `LoadlyCustomerApp`, which does register them. The test does not provide `HomeBloc`, `TripsBloc`, and the other feature blocs; opening those screens from the test will fail with a provider error.

### Help in settings does nothing

The Help row in `SettingsScreen` uses `onPress: () {}`.

## General Flutter problems

These are not specific bugs found in this repo. They show up often on this toolchain.

| Symptom | What usually fixes it |
| --- | --- |
| Stale native build after a plugin upgrade | `fvm flutter clean`, then `pub get`, and `pod install` on iOS |
| Gradle daemon lock on Windows | Stop the daemon (`cd android` then `./gradlew --stop`) and rebuild |
| “Lost connection to device” on hot restart during startup | Restart with `fvm flutter run`. Startup awaits Firebase, storage, and FCM before `runApp` |
| iOS build cannot find a module after a plugin change | Delete `ios/Pods` and `ios/Podfile.lock`, then `pod install` |
