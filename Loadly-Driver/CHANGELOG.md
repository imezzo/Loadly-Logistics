# Changelog

## [1.1.2] - 2026-07-19
### Added
- add handling for missing image files and update localization strings
### Changed
- Update notification handling and FCM integration for improved user experience
- update error handling to use localized strings in alert dialogs
- update error messages to use localized strings in auth and improve error handling
- update Google maps api key

## [1.2.0] - 2026-08-16
### Added
- add automate odometer reading via OCR
- add odometer image and reading to complete unloading step
- add tollgate weight images to complete trip
### Fixed
- fixed reverse geocoding to show current postion address
- fixed Notification is read bug
- fixed When try to exit notification the app navigate to error page
- fixed Document localization on trip success
- fixed When loged in with password and accept the pop up of enable fingerprint it doent reflect in profile still disabled
- fixed When enabling fingerprint the app accepts any value you enter and fail to login if entered any value not like the password
- fixed when asigning a password (Change password) app logs out
- fixed In trip when traviling between two points no destination (road) displayed in the map
- fixed bottom navbar in 3 buttons navigation
- fixed Infinite loading appears when entering current trips tap
### Changed
- Remove tollgates total amount field
- Changed trip start date timezone from utc to local time
- Changed trip start time from 24h fromat to 12h format
- Changed notifications card to show 3 hours ago insted of 180 min ago

## [1.2.1] - 2026-09-30
### Added
- add refresh token mechanism in auth (https://github.com/Loadly-EG/Loadly-Driver/pull/16)
- add architecture, development, and troubleshooting documentation (https://github.com/Loadly-EG/Loadly-Driver/pull/30)
### Fixed
- make vehicleId optional (https://github.com/Loadly-EG/Loadly-Driver/pull/29)
### Changed
- Convert UTC timestamps in notification body text to local 12-hour time for backend notificatios (https://github.com/Loadly-EG/Loadly-Driver/pull/17)
- Convert UTC timestamps in notification body text to local 12-hour time for forground notificatios (https://github.com/Loadly-EG/Loadly-Driver/pull/19)
- Replace platform geocoding package with Google Geocoding API for trip start/current address (https://github.com/Loadly-EG/Loadly-Driver/pull/25)
- enhance trip document handling and camera integration (https://github.com/Loadly-EG/Loadly-Driver/pull/27)
- enhance map marker icons and trip route visualization (https://github.com/Loadly-EG/Loadly-Driver/pull/28)
- update Firebase configuration for Android and iOS (https://github.com/Loadly-EG/Loadly-Driver/pull/31)

## [1.2.2] - 2026-10-01
### Changed
- Stop converting notification title and body dates from UTC to local (https://github.com/Loadly-EG/Loadly-Driver/pull/34)

## [1.3.0] - 2026-10-28
### Added
- support optional trailing depot waypoint (https://github.com/Loadly-EG/Loadly-Driver/pull/36)
- add support for notification attachments in notifications (https://github.com/Loadly-EG/Loadly-Driver/pull/40)
- implement attachment preview functionality in notifications (https://github.com/Loadly-EG/Loadly-Driver/pull/42)
### Fixed
- improve layout of departFromWaypoint button in trip step documents section (https://github.com/Loadly-EG/Loadly-Driver/pull/43)
### Changed
- Drop Long Polling in Listing Trips (https://github.com/Loadly-EG/Loadly-Driver/pull/35)
- make odometer and odometerImages optional in CompleteUnloadingParams (https://github.com/Loadly-EG/Loadly-Driver/pull/37)
- Use API typeName for trip document labels on trip success screen (https://github.com/Loadly-EG/Loadly-Driver/pull/38)
- Remove delivery note capture from depart-dropoff trip step (https://github.com/Loadly-EG/Loadly-Driver/pull/39)
- enhance notification attachments support with PDF handling  (https://github.com/Loadly-EG/Loadly-Driver/pull/41)
