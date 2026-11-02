# Development guide

Practical setup for Loadly Customer. Architecture and request flow are in [ARCHITECTURE.md](ARCHITECTURE.md). Failures and environment mistakes are in [TROUBLESHOOTING.md](TROUBLESHOOTING.md).

## Prerequisites

- [FVM](https://fvm.app/)
- Flutter **3.41.1**, pinned in `.fvmrc`
- Dart SDK constraint in `pubspec.yaml`: `^3.8.0` (the SDK that ships with Flutter 3.41.1 satisfies this)
- Android: Android SDK, a device or emulator. CI uses Java 17
- iOS: Xcode and CocoaPods, on macOS
- Network access to the API host you pass as `CUSTOMER_API_BASE_URL` (the source default is the staging host in `lib/core/network/api_client.dart`)

`.vscode/settings.json` sets `dart.flutterSdkPath` to `.fvm/versions/3.41.1`. Use `fvm flutter` and `fvm dart` so local and CI toolchains match. `.fvm/` is gitignored; `.fvmrc` is not.

## Install and run

```bash
fvm install
fvm flutter pub get
fvm dart run build_runner build --delete-conflicting-outputs
fvm flutter run
```

`build_runner` is required after a fresh clone if generated `*.g.dart` files are missing or stale. They are currently committed.

Select a device with `fvm flutter devices`, then `fvm flutter run -d <deviceId>`.

Point a build at another API:

```bash
fvm flutter run --dart-define=CUSTOMER_API_BASE_URL=https://your-host/api
```

The value is the API root. Paths already start with `/company` or `/lookups`. Do not append `/company` again.

Other defines (all optional; defaults live in source):

| Define | Effect |
| --- | --- |
| `SENTRY_DSN` | Release-only error reporting. Ignored unless it contains `sentry.io` |
| `GOOGLE_MAPS_API_KEY` | Dart-side Maps Web Service key in `lib/core/config/google_maps_config.dart` |
| `IOS_APP_STORE_ID` | App Store URL used by the update flow |
| `HUAWEI_APP_GALLERY_ID` | AppGallery id |
| `APP_STORE_DISTRIBUTION` | `play` (default), `harmonyos`, or `appgallery` |
| `APP_STORE_URL` | Full store URL override |

Android and iOS map rendering still use the keys in `AndroidManifest.xml` and `ios/Runner/AppDelegate.swift`. Changing only `GOOGLE_MAPS_API_KEY` does not change the native SDK key.

Firebase uses the checked-in `android/app/google-services.json` and `ios/Runner/GoogleService-Info.plist`. There is no `firebase_options.dart`.

Release Android signing: create `android/key.properties` with `keyAlias`, `keyPassword`, `storeFile`, and `storePassword`. If that file is absent, `android/app/build.gradle` signs release with the debug keystore.

## Tests

```bash
fvm flutter test
fvm flutter analyze
```

The suite is `test/widget_test.dart` only. It expects a `CupertinoApp`. It does not initialize Firebase or `AuthStorage`.

## Builds

```bash
fvm flutter build apk --release
fvm flutter build appbundle --release
fvm flutter build ios --release
```

Add the same `--dart-define` flags you use for `flutter run`. The GitHub Action `.github/workflows/build-apk.yml` runs `fvm flutter build apk --release` with no defines, so CI APKs keep the defaults in source.

App version is `version: x.y.z+build` in `pubspec.yaml`. Android `versionName` / `versionCode` and iOS `CFBundleShortVersionString` / `CFBundleVersion` come from that line.

Launcher icons and the native splash are configured in `pubspec.yaml` (`flutter_launcher_icons`, `flutter_native_splash`). Regenerate only when those assets change:

```bash
fvm dart run flutter_launcher_icons
fvm dart run flutter_native_splash:create
```

## Localization

- Sources: `lib/l10n/app_en.arb` (template) and `lib/l10n/app_ar.arb`
- Config: `l10n.yaml` (`flutter: generate: true` in `pubspec.yaml`)
- Output: `lib/l10n/gen/app_localizations*.dart`
- Default locale when nothing is stored: Arabic (`LocaleBloc`, `LoadlyCustomerApp`)
- In widgets: `AppLocalizations.of(context)!`

```bash
fvm flutter gen-l10n
```

`LocaleBloc` writes `app_locale`. `LanguageInterceptor` reads that key for the `lang` header.

## Code generation

`build.yaml` sets `json_serializable` options (`explicit_to_json: true`, `field_rename: none`).

After editing a `@JsonSerializable` class:

```bash
fvm dart run build_runner build --delete-conflicting-outputs
```

Commit the updated `*.g.dart` files with the source change.

## Where to put new code

Match an existing feature (`trips` is the fullest example).

| Kind of code | Location |
| --- | --- |
| Screen | `lib/features/<feature>/presentation/views/` |
| Feature-only widget | `lib/features/<feature>/presentation/widgets/` |
| Widget used by several features | `lib/core/widgets/` |
| BLoC, events, states | `lib/features/<feature>/presentation/bloc/` |
| Entity, repository interface, use case, failure | `lib/features/<feature>/domain/` |
| DTO, remote data source, repository implementation | `lib/features/<feature>/data/` |
| HTTP path constant | `lib/core/network/api_endpoints.dart` |
| Route | `lib/core/router/app_router.dart` |
| Shared service | `lib/core/services/` |
| Strings | `lib/l10n/app_en.arb` and `lib/l10n/app_ar.arb` |

### Adding a feature

1. Add `domain` (entity, `*Failure`, repository interface, use case), `data` (DTO, remote data source, `*Impl`), and `presentation` (bloc, screen).
2. Map `DioException` to the feature failure the same way `HomeImpl` or `TripsImpl` does, and return `Either`.
3. Construct the data source, repository, use case, and BLoC in `lib/main.dart`, and add a `BlocProvider.value`.
4. Register a `GoRoute`. If the screen is a main tab, add it in `MainScaffold` and `AppBottomNavBar`.
5. If the route must stay reachable while logged out, add the path to the `isAuthRoute` check in `createAppRouter`. Otherwise the redirect sends logged-out users to `/login`.
6. Run `build_runner` if you added a DTO.
7. Add ARB keys for any new user-visible copy.

Profile and settings do not have their own repositories. Profile updates go through `UpdateProfileUseCase` on `AuthRepository`. Follow that only when the API is already an auth endpoint.

Detail routes such as `/trips/:tripId` and `/invoices/:invoiceId` require `state.extra` of a specific type. Passing the wrong `extra` throws at runtime. See the route table in [ARCHITECTURE.md](ARCHITECTURE.md).

## Conventions found in the project

From `analysis_options.yaml`:

- `package:flutter_lints/flutter.yaml`
- `prefer_const_constructors: true`
- `prefer_relative_imports: true` (imports inside `lib/` are relative, not `package:loadly_customer/...`)

Also visible in the code:

- UI is Cupertino (`CupertinoPageScaffold`, `CupertinoButton`, `showCupertinoDialog`). Material widgets are imported only where Cupertino has no equivalent (`RefreshIndicator`, some `Divider`s).
- One BLoC class per feature, events and states in their own files, `Equatable` for events, states, and failures.
- Repository implementations are named `AuthImpl`, `TripsImpl`, and so on, not `*RepositoryImpl`.
- Remote sources are `*RemoteDataSource` / `*RemoteDataSourceImpl` and take the shared `Dio`.
- DTOs use `@JsonSerializable()` and a `part '*.g.dart'` directive.
- User-facing screen strings go through `AppLocalizations`. Many BLoC error strings are still English literals.
- Theme: `AppColors`, `AppLayout`, and `buildAppTheme` (Archivo for English, Almarai for Arabic via `google_fonts`).
- Comments and names sometimes describe an older driver-style flow. Trust the current methods over those comments.

## Commands

| Task | Command |
| --- | --- |
| Install Flutter from `.fvmrc` | `fvm install` |
| Dependencies | `fvm flutter pub get` |
| Run | `fvm flutter run` |
| Analyze | `fvm flutter analyze` |
| Test | `fvm flutter test` |
| Format | `fvm dart format .` |
| JSON codegen | `fvm dart run build_runner build --delete-conflicting-outputs` |
| Localizations | `fvm flutter gen-l10n` |
| Release APK | `fvm flutter build apk --release` |
| App bundle | `fvm flutter build appbundle --release` |
| iOS release | `fvm flutter build ios --release` |

## Problems you will hit during setup

These are the ones this repo is set up to cause. Longer explanations are in [TROUBLESHOOTING.md](TROUBLESHOOTING.md).

| Symptom | What to check |
| --- | --- |
| `flutter` and FVM disagree | Run `fvm flutter --version` and confirm `.fvmrc` is `3.41.1` |
| Missing `*.g.dart` or JSON errors after a DTO edit | Re-run `build_runner` with `--delete-conflicting-outputs` |
| Calls hit staging | Pass `CUSTOMER_API_BASE_URL`. CI does not |
| Release APK is signed with debug | Add `android/key.properties` |
| iOS pods fail | From `ios/`: `pod install` after `fvm flutter pub get` |
| Blank Android map after leaving the app | `main.dart` must run `initializeWithRenderer` before the first `GoogleMap`. Do not remove that block |
| Logged-out user cannot open a new route | Add the path to `isAuthRoute` in `app_router.dart` |
