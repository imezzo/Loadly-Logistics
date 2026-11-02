# Changelog

## [1.1.0] - 2026-07-01
### Added
- add finicial collector
- implement app update flow with version check and notification
### Changed
- rework trip card
- Refactor trip details and card views for improved document handling and localization
- remove MFA channel screen and update routing logic; adjust login validation for username

## [1.1.1] - 2026-08-02
### Added
- Added hasCustoms and customsStatus fields to CollectorTrips entity
- Introduced new radio button for partially confirmed documents in filter screen
### Fixed
- fix Tools bar is hidden by the 3-button navigation in some phone models
### Changed
- Enhance finance user flow to collect the document also as factory collector do
- Make customs document doesnt appear in collecting list if the trip doesnt have customs
- Make status of document collecting in finance user of trip matched with the collector user

## [1.1.2] - 2026-09-30
### Changed
- update Firebase configuration for Android and iOS
- update Google Maps API key

## [1.1.3] - 2026-10-04
### Fixed
- fix network error on search (https://github.com/Loadly-EG/Loadly-Workplace-App/pull/1)
### Changed
- localized fallback error messages to arabic (https://github.com/Loadly-EG/Loadly-Workplace-App/pull/1)
- localized dates and time to arabic (https://github.com/Loadly-EG/Loadly-Workplace-App/pull/1)
