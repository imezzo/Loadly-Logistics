# Troubleshooting

Project-specific items are taken from this repo (comments, config, and control flow). Items marked **General** are ordinary Flutter problems, not bugs observed here.

## Dependency issues

**`fvm` is not recognized.**  
Install FVM, then `fvm install` in the repo. Use `fvm flutter` and `fvm dart`, not a global Flutter that is not 3.41.2. The IDE SDK path is `.fvm/flutter_sdk` only after `fvm install`.

**Packages fail to resolve.**  
Run `fvm flutter pub get`. The SDK constraint is `sdk: ^3.8.0`. An older Flutter will fail that constraint.

**Generated JSON code is missing or wrong.**  
DTOs use `json_serializable`. Run:

```bash
fvm dart run build_runner build --delete-conflicting-outputs
```

**Localization getters are missing.**  
Add the key to `lib/l10n/app_en.arb` and `lib/l10n/app_ar.arb`, then `fvm flutter gen-l10n` or `fvm flutter pub get`. Output is `lib/l10n/gen/`.

**`MissingPluginException` after adding a plugin or hot restart.**  
`odometer_image_validator.dart` catches this for ML Kit and comments that a full restart is required. Stop the process and `fvm flutter run` again. **General:** new native plugins are not registered by hot reload.

**`dotenv` / `.env` changes do nothing.**  
No Dart file imports `dotenv`. Changing `.env` does not change the API URL. Use `--dart-define`. See [DEVELOPMENT.md](DEVELOPMENT.md).

## Build failures

**Android Gradle uses an unexpected application id or Kotlin version.**  
Both `android/settings.gradle` and `android/settings.gradle.kts` exist. Both `android/app/build.gradle` and `android/app/build.gradle.kts` exist, with different `applicationId` values (`com.loadly.driver` vs `com.loadly.loadly_driver`) and different Kotlin plugin versions (`2.3.10` vs `2.2.20`). **Needs clarification** which pair your Gradle invocation selects.

**Release build installs but Play Console rejects the signature.**  
`android/app/build.gradle.kts` signs release with the debug keystore. The comment in that file says this is so `flutter run --release` works.

**CI APK talks to staging.**  
`.github/workflows/build-apk.yml` runs `fvm flutter build apk --release` with no `--dart-define`. The binary keeps the `defaultValue` URL, Sentry DSN, and Maps key in source.

**`flutter.sdk not set in local.properties`.**  
Android `settings.gradle` / `settings.gradle.kts` require `flutter.sdk` in `android/local.properties`. `fvm flutter` writes this after a successful Flutter tool run. **General:** do not commit `local.properties`.

**Web build fails on `dart:io`.**  
`lib/main.dart` imports `dart:io`. A web target will not compile this entrypoint as written.

**iOS pod install errors. General.**  
From `ios/`: `pod install` after `fvm flutter pub get`. Podfile.lock is in the repo (`ios/Podfile.lock`).

## Android and iOS

**Blank or blue Google Map after the camera or after backgrounding (Android).**  
`main.dart` initializes `AndroidMapRenderer.latest` before `runApp` for this case. If you still see it, confirm that init is not throwing (it is caught and printed as `Map renderer init:`).

**Map is blank and logcat mentions the API key.**  
The Maps SDK key is `com.google.android.geo.API_KEY` in `android/app/src/main/AndroidManifest.xml`. REST geocoding uses `GOOGLE_MAPS_API_KEY` (dart-define or the default in `google_maps_config.dart`). Those can be restricted differently in Google Cloud. The comment in `google_maps_config.dart` says Android/iOS app restrictions often do not cover Web Service REST calls.

**Location never uploads while online.**  
`DriverLocationBackgroundService.start` returns without an error log if permission is denied or location services are off. Uploads are skipped when the previous upload was less than one minute ago. The endpoint is `POST /location/ping`. Android shows a foreground notification from Geolocator (`FOREGROUND_SERVICE_LOCATION` is in the manifest).

**Background location permission dialog.**  
`HomeBloc` has disclosure events (`HomeContinueBackgroundLocationDisclosure` and related). The prominent disclosure UI is under `features/home/presentation/widgets/` and `lib/core/permissions/prominent_user_data_disclosure.dart`.

**Push tap does nothing or opens the wrong screen.**  
Cold start stores the route on `NotificationLaunchHandler` until splash calls `navigateIfPending`. Android needs the `FLUTTER_NOTIFICATION_CLICK` intent filter and the default notification channel `general_notifications` (both in `AndroidManifest.xml`). Data-only vs notification payloads are interpreted in `notification_click_action.dart`.

**iOS notifications never prompt.**  
`FCMService.initialize` is called with `requestRuntimeNotificationPermission: Platform.isIOS`, so iOS asks during startup. If the user previously denied notification permission, iOS will not show the dialog again until they change it in Settings. **General.**

**Android 13+ notifications never prompt at launch.**  
That matches the code. `main.dart` does not request notification permission on Android. `HomeScreen` shows `notificationPermissionDisclosureTitle` and, if the user continues, calls `FCMService.requestRuntimeNotificationPermission()`. `HomeBloc._onContinueBackgroundLocationDisclosure` also calls `Permission.notification.request()` on Android before `Permission.locationAlways`. `POST_NOTIFICATIONS` is declared in `AndroidManifest.xml`.

**Face ID purpose text is blank.**  
`NSFaceIDUsageDescription` in `ios/Runner/Info.plist` is an empty string.

**Biometric login does nothing.**  
`BiometricService` stores login and password only after a successful `local_auth` prompt and (from settings) password validation. It is per saved login (`isBiometricEnabledForThisUser`).

## API and network

**All requests hit the wrong host.**  
Override `DRIVER_API_BASE_URL`. The default in `api_client.dart` is the staging host. A production URL is only a comment.

**401 on a normal screen sends you to login.**  
Expected when refresh fails: `AuthInterceptor` clears tokens and `go('/login')`. Refresh uses `POST /auth/refresh-token` with the stored refresh token. If that body shape changes, refresh returns null and the session ends.

**401 on the login or OTP screen also redirects.**  
It should not, for paths listed in `AuthInterceptor._authAttemptPaths`, and not while `AuthScreenGuard.isOnAuthScreen` is true. Splash sets that flag. If a new auth route forgets both, a 401 will kick the user to `/login`.

**Calls succeed in the UI but not in the odometer suggest request, or the reverse.**  
Trip step document validation uses the get_it `TripsRepository`, which has its own Dio. Other trip actions use the Dio created in `main.dart`. Both are built by `createDriverApiClient()`, so interceptors match, but they do not share in-memory interceptor state such as `AuthInterceptor._isRefreshing`.

**Server error text looks like a localization key.**  
`FailureMessageMapper.forAuth` can return keys such as `auth.error_500`. The auth UI is expected to map those. Other features sometimes show the API `message` or an English fallback from `FailureMessageMapper`.

**`lang` header is wrong.**  
`LanguageInterceptor` reads `app_locale` from SharedPreferences, not the widget locale at request time. Default when missing is `ar`.

**Chucker / HTTP inspector in a release build.**  
`ChuckerFlutter.showOnRelease = true` in `main.dart`. This is current behavior, not a debug-only flag.

**Version check does not block startup when the network is down.**  
Splash still navigates after the check future finishes. A failed check does not force an update.

## Environment and configuration

**Sentry events missing in debug.**  
`SentryFlutter.init` runs only when `kReleaseMode` is true and the DSN string contains `sentry.io`. Debug builds call `_runApp` without Sentry.

**Update button opens the wrong store.**  
`openAppStoreListing` prefers `storeUrl` from the API, then `AppStoreConfig.storeUrl`. Play default package is `com.loadly.driver`. The Kotlin `applicationId` is `com.loadly.loadly_driver`. HarmonyOS builds need `APP_STORE_DISTRIBUTION=harmonyos` and `HUAWEI_APP_GALLERY_ID`. The client does not auto-detect Huawei devices.

**Firebase fails to initialize.**  
`main.dart` catches the error, prints `Error initializing Firebase:`, and continues. Push and background handlers will not work until `google-services.json` / `GoogleService-Info.plist` match the Firebase project. Those files are local config; do not commit a different environment by accident.

## Authentication

**App opens login even though you logged in before.**  
`isLoggedIn()` is “access token is non-empty” in SharedPreferences. `AuthStorage.init()` must have completed. Logout and failed refresh call `clearTokens()`, which also removes the online flag.

**Remembered password missing, login id still filled.**  
The login id is in SharedPreferences. The password is in `FlutterSecureStorage` (`auth_remembered_password`). Secure storage can fail on a device with no lock screen. **General:** Android Keystore / iOS Keychain errors surface as empty reads in `getRememberedPassword`.

**Change-password or login 401 should not clear a session you are trying to create.**  
Those paths are listed in `_authAttemptPaths`. A 401 on other APIs will.

**Logged-in user cannot open `/login`.**  
The router redirect sends authenticated users from auth routes to `/home`.

**`/change-password` or `/verify-email` while logged out opens login.**  
Those routes are not in `isAuthRoute`, so they are protected.

## Common runtime errors

**Trip action says photos could not be found.**  
`TripsFailureMissingFiles` and `FailureMessageMapper.forTrips` use the message that one or more photos could not be found. `TripStepCaptureStore` keeps paths in SharedPreferences; if the file was deleted, the upload fails this way.

**Odometer photo rejected on device.**  
`validateOdometerImage` returns l10n keys `trips.odometer_image_unreadable` or `trips.odometer_image_not_detected` when ML Kit text does not look like an odometer. Server suggest is a separate call (`/odometer/suggest`).

**Infinite loading was a known trips-tab bug.**  
`CHANGELOG.md` `[1.2.0]` lists this as fixed. If it returns, capture the `TripsBloc` state (`TripsLoading` vs `TripDetailsLoading`) before changing the list screen.

**Splash stays on the logo.**  
Forced update (`updateInfo.isForced`) shows `AppUpdateScreen` and returns without `go()`. That is intentional. The splash also waits until `checkForUpdate()` finishes. That method catches errors and returns null; Dio’s connect and receive timeouts are 30 seconds, so a dead host delays splash by about that long plus the 2 second minimum, then navigation continues.

**Notifications refresh the trip list and interrupt details.**  
On resume, `_AppLifecycleRefresher` skips `TripsLoadTrips` when state is `TripDetailsLoading`, `TripDetailsLoaded`, or `TripActionInProgress`. Other states reload page 1.

## General Flutter problems

These are not specific defects in this repo:

- A stuck Gradle daemon: `cd android` and `./gradlew --stop`, then rebuild.
- A corrupt pub cache: `fvm flutter pub cache repair`, then `fvm flutter pub get`.
- iOS “module not found” after a plugin upgrade: delete `ios/Pods` and `ios/Podfile.lock` only if you intend to resolve pods again, then `pod install`.
- State that looks stale after a hot reload of `main.dart`: the composition root and `getIt.registerSingleton` do not re-run cleanly. Full restart.
