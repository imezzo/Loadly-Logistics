# Troubleshooting

Issues below are either visible in this repository or marked as general Flutter troubleshooting. Architecture context is in [ARCHITECTURE.md](ARCHITECTURE.md). Setup commands are in [DEVELOPMENT.md](DEVELOPMENT.md).

## Dependency and SDK issues

### `fvm` or the Flutter SDK is missing after clone

`.fvm/` is gitignored. Only `.fvmrc` (Flutter `3.41.2`) is committed.

```bash
fvm install
fvm flutter pub get
```

The editor SDK path is `.fvm/flutter_sdk` (`.vscode/settings.json`). If analysis uses a different Flutter, point the IDE at that path or run commands through `fvm flutter`.

### `flutter pub get` fails, or a plugin will not compile

General: delete `.dart_tool/` and `pubspec.lock` only if you intend to refresh the lockfile. Prefer:

```bash
fvm flutter clean
fvm flutter pub get
```

This app enables Android core library desugaring (`android/app/build.gradle`, `desugar_jdk_libs`). If a new plugin asks for a higher `minSdk`, change `minSdk` deliberately. Today it is `flutter.minSdkVersion`, not a hard-coded number.

### Missing `*.g.dart` or `part '...g.dart'` errors

DTOs use `json_serializable`. Generated files are committed, but they are stale after you edit an annotated class.

```bash
fvm dart run build_runner build --delete-conflicting-outputs
```

`freezed` is a dev dependency, but there are no `@freezed` types. Do not expect `*.freezed.dart` to be generated.

### Localization key missing

Add the key to both `lib/l10n/app_en.arb` and `lib/l10n/app_ar.arb`, then rebuild or run `fvm flutter pub get`. Generated code is `lib/l10n/gen/`. `app_en.arb` is the template (`l10n.yaml`).

## Build failures

### Android Gradle or Java errors

CI builds with **Java 17**, Android Gradle Plugin **8.11.1**, Kotlin **2.3.10**, Gradle **8.14** (`android/settings.gradle`, `android/gradle/wrapper/gradle-wrapper.properties`). A local JDK older than 17 is a common cause of “invalid source release” or Gradle plugin errors. General fix: install JDK 17 and point `JAVA_HOME` at it, then `fvm flutter clean`.

`flutter.sdk` must be set in `android/local.properties`. Flutter writes that file. If it is missing, run `fvm flutter pub get` from the project root.

### Release APK is signed with the debug key

`android/app/build.gradle` uses `android/key.properties` only when the file exists. Otherwise `release` uses `signingConfigs.debug`. Play Console will reject that upload. Add `key.properties` locally. Do not commit keystore passwords.

### ProGuard / R8 warnings about ML Kit

`android/app/proguard-rules.pro` contains `-dontwarn` rules for unused ML Kit text recognizers. Those rules are already in the release build type. If a new R8 error names a different missing class, add a targeted `-dontwarn` only after confirming the class is unused.

### iOS: CocoaPods, platform, or Generated.xcconfig

`ios/Podfile` requires iOS **15.5**. General sequence when pods fail:

```bash
fvm flutter pub get
cd ios
pod install
```

`pod install` fails with “FLUTTER_ROOT not found” when `ios/Flutter/Generated.xcconfig` is missing. `flutter pub get` creates it. Do not commit that generated file as a hand-written fix.

### iOS Face ID purpose string is empty

`ios/Runner/Info.plist` has `NSFaceIDUsageDescription` set to an empty string. Biometric login uses `local_auth`. App Store review and the Face ID prompt need a non-empty purpose string. **Needs clarification** on the approved wording before changing the plist.

### Web, Windows, or Linux compile errors about `dart:io`

`lib/main.dart` imports `dart:io` for `Platform.isIOS`. `lib/core/network/api_client.dart` imports `dart:io` for device headers. Those imports do not compile for web. Platform folders exist, but this entrypoint is a mobile app. Desktop builds can compile `dart:io`, but CI only produces an Android APK. **Needs clarification** whether desktop is a supported target.

### `flutter build` uses the staging API

Expected if you did not pass `--dart-define=WORKPLACE_API_BASE_URL=...`. The GitHub workflow does not pass dart-defines. Confirm the binary’s host before shipping.

## Android and iOS runtime issues

### Push notifications never appear

Check, in order:

1. Firebase init did not throw. `main.dart` catches the error and continues with a `debugPrint`.
2. `google-services.json` / `GoogleService-Info.plist` match application id `com.loadly.workplace`.
3. Android 13+ notification permission. Startup requests the runtime permission only when `Platform.isIOS` (`FCMService.initialize(requestRuntimeNotificationPermission: Platform.isIOS)`). On Android the service comment says to show `showProminentUserDataDisclosure` before requesting. This audit did not find a call site that requests Android notification permission after that disclosure. **Needs clarification** if Android 13 prompts are triggered from a screen that was not obvious in startup.
4. Foreground messages use channel id `general_notifications`, also set as `com.google.firebase.messaging.default_notification_channel_id` in `AndroidManifest.xml`.
5. A notification tap always routes to `/home`. There is no notifications inbox.

### Camera or photo picker fails

Document photos use `image_picker` and `permission_handler` (`lib/features/home/presentation/widgets/recived_popup.dart`). Android declares `CAMERA` and `READ_EXTERNAL_STORAGE` (max SDK 32). iOS purpose strings exist for camera and photo library (`Info.plist`). If the system prompt never appears, the permission was denied earlier. General: clear the app’s permission in system settings and try again.

Startup deletes image files in the temp directory (`TempFileService`). A photo that lived only in temp storage will be gone on the next cold start.

### Maps or location do nothing

The Dart app does not depend on `google_maps_flutter` or `geolocator`. Location usage strings in `Info.plist` and a Maps API key meta-data entry in `AndroidManifest.xml` are not referenced by `lib/`. Do not expect a map on the home screen. The home screen is a trip list.

### Bottom system navigation covers the bottom bar

`AppBottomNavBar` adds `MediaQuery.viewPadding.bottom`. Changelog 1.1.1 records a fix for 3-button navigation. If a device still overlaps, check that the screen uses `MainScaffold` (home and profile do).

## API and network

### Calls go to the wrong environment

The define read by `kWorkplaceApiBaseUrl` is `WORKPLACE_API_BASE_URL`. `Workplace_API_BASE_URL` (different capitalization) is ignored, and the staging default is used.

Default: `https://stg-api-agg.loadly-eg.com/api/collector`.

Version check uses the same host with `/collector` removed, then `/app-version/check`.

### Inspect the actual request

`ChuckerDioInterceptor` is on the shared client, and `ChuckerFlutter.showOnRelease = true`. Use the in-app Chucker UI to see URL, headers, and body. Headers added by interceptors include `Authorization`, `lang`, `fcmToken`, and device fields.

### Timeouts

Connect and receive timeouts are 30 seconds (`createWorkplaceApiClient`). `safeApiCall` maps those to a network failure. The UI text is often the hard-coded “Network error…” string in the BLoC, not a localized ARB string.

### `success: false` with HTTP 200

Auth and trip writes throw a `DioException` when the envelope’s `success` is false (`throwIfEnvelopeFailed`, and the same check inside `TripsRemoteDataSourceImpl`). The `message` or first `errors` entry becomes the failure message. A 200 is not treated as success by itself.

### Trip list is empty or the wrong subset

`HomeScreen.loadTrips` always sends `status: 6` plus `tripDocumentStatuses` `[1, 2]` or `[1, 2, 3, 4]` for a financial collector, unless `FilterData` overrides the document statuses. `status: 6` is not named in code. **Needs clarification** with the API contract if the list looks wrong.

Document types are `GET /documents?pageSize=100&entityId=5`. `entityId` `5` is not named in code.

### Bulk collect/confirm vs single trip

Single-trip collect and confirm are `multipart/form-data`. Bulk endpoints post JSON (`BulkCollectTripDocumentsRequestDto`). Sending JSON to the single-trip path, or form data to the bulk path, will fail on the server.

## Configuration

### Sentry is silent in debug

`SentryFlutter.init` runs only in release mode, and only when the DSN contains `sentry.io`. Debug and profile builds call `_runApp` without Sentry. `sendDefaultPii` is `true` in the options that do run.

Symbol upload is configured in `pubspec.yaml` (`sentry.upload_debug_symbols`, project `loadly-loadly-driver`, org `fuelin-wr`). Upload needs Sentry credentials in the environment that runs the build (`sentry.properties` is gitignored). A missing auth token fails the upload step, not the Flutter compile, unless the plugin is invoked.

### Update prompt never shows, or blocks the app

- Splash waits for `AppUpdateService.checkForUpdate`. Any error is logged and treated as “no update”, so a bad version endpoint does not block startup.
- Forced update (`forceUpdate`, or current version lower than `minVersion`) pushes a full-screen route and does **not** continue to home or login (`app_router.dart`).
- Optional update is a dialog after navigation.
- Store button URL comes from the API `storeUrl` when present, otherwise `AppStoreConfig.storeUrl`. iOS needs `IOS_APP_STORE_ID` or `APP_STORE_URL` if the API does not send a URL. Huawei builds need `APP_STORE_DISTRIBUTION=harmonyos` (or `appgallery`) and `HUAWEI_APP_GALLERY_ID`.

### Language header does not match the UI

`LocaleBloc` stores `app_locale`. `LanguageInterceptor` reads that key on each request. Default when missing is `ar`. Change language from the profile screen (`LocaleChanged`) so both the UI and the header update.

## Authentication

### User is sent back to login immediately

`isLoggedIn()` requires a non-empty **refresh** token in SharedPreferences. Saving only an access token is not enough. Splash and the GoRouter redirect both use this check.

### 401 on an API call logs the user out

`AuthInterceptor` posts `/auth/refresh-token` with `{ "refreshToken": ... }` using a **separate** `Dio` (no interceptors, same base URL). If that call fails, tokens are cleared and the app goes to `/auth/login`.

Auth attempt paths (`/auth/login`, MFA, reset, and some paths that have no client method) do not redirect. Other 401s do.

`OtpScreen` sets `AuthScreenGuard.isOnAuthScreen`, but the interceptor never reads it. A 401 during OTP on a non-auth path can still redirect. **Needs clarification** if that guard was supposed to suppress the redirect.

### Login succeeds but home is not opened

`PasswordLoginScreen` branches on `LoginResult.redirect` and `mfaChannel`:

- `mfa`, or any `mfaChannel` other than `0`, opens `/otp` and sends channel `2` (SMS).
- `forget_password` opens the OTP screen in reset mode.
- `mfaChannel == 0` opens `/home`.

If the API returns an unexpected `redirect`, the `else` branch still starts MFA. Compare the login JSON in Chucker with `LoginResponseDto`.

### Financial collector sees the factory collector UI

The flag is set only in `AuthBloc` when login (or the same check on biometric login) finds permission `confirm_trip_documents`. It is stored under `auth_is_financial_collector`. Logout clears it. The home route reads the flag when the route is built. If permissions are missing from `LoginUserDto.permissions`, the user is stored as a non-financial collector.

### Biometric login fails

`BiometricService` stores login and password in secure storage and replays password login. It fails when biometrics are unavailable, the user cancels, or the saved login does not match. iOS needs a non-empty `NSFaceIDUsageDescription` (currently empty). Android uses `local_auth` with default `AndroidOptions()`.

### Remembered password is missing

The login id is in SharedPreferences (`auth_remembered_login`). The password is in secure storage (`auth_remembered_password`). Reinstalling the app or restoring a backup can drop the keychain or encrypted prefs entry while leaving the login id.

## Runtime errors

### `/filter` or `/trips/:tripId` throws on `as FilterData` / `as CollectorTrips`

Both builders cast `state.extra` with no null check (`app_router.dart`). Open filter from the home screen and trip details from a trip card (`context.go('/trips/${id}', extra: trip)`). A notification or a manual URL does not pass `extra`.

### Locale switch resets navigation

`createAppRouter()` runs inside `LoadlyWorkplaceApp.build`, which rebuilds when `LocaleBloc` changes. That constructs a new `GoRouter` and replaces `AppRouterHolder`. **Needs clarification** if a language change is expected to keep the current stack.

### App update check or Firebase errors only show in the console

Both are caught. Look for `App update check failed` or `Error initializing Firebase` in the debug console. They do not surface a dialog.

### Tests pass but cover nothing

`test/widget_test.dart` is `void main() {}`. A green `fvm flutter test` does not mean the BLoCs or API mappers work.

## General Flutter checks

These are not project-specific bugs. Use them when the failure is not explained above.

- Stale build: `fvm flutter clean`, then `fvm flutter pub get`, then rebuild.
- Hot reload did not pick up a native change (manifest, plist, new plugin): stop the app and `fvm flutter run` again.
- Gradle daemon stuck: `cd android` and `./gradlew --stop` (Windows: `gradlew.bat --stop`).
- CocoaPods repo out of date: `pod repo update`, then `pod install` in `ios/`.
