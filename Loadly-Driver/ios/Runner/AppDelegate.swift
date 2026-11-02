import Flutter
import UIKit
import GoogleMaps
import UserNotifications
import CoreLocation
import flutter_foreground_task

/// Registers GMSCachePropertyValueTransformer for Google Maps SDK Core Data.
/// The Google Maps SDK's cache model references this transformer but doesn't provide it,
/// causing a Core Data warning. Registering it early fixes the warning.
@objc(GMSCachePropertyValueTransformer)
final class GMSCachePropertyValueTransformer: NSSecureUnarchiveFromDataTransformer {
  override static var allowedTopLevelClasses: [AnyClass] {
    [
      NSData.self,
      NSNumber.self,
      NSString.self,
      NSDate.self,
      NSArray.self,
      NSDictionary.self,
      NSNull.self,
      NSValue.self,
      NSURL.self,
    ]
  }

  static func register() {
    let transformer = GMSCachePropertyValueTransformer()
    ValueTransformer.setValueTransformer(
      transformer,
      forName: NSValueTransformerName("GMSCachePropertyValueTransformer")
    )
  }
}

@main
@objc class AppDelegate: FlutterAppDelegate, CLLocationManagerDelegate {
  private let tripGpsManager = CLLocationManager()
  private let tripGpsArmedKey = "trip_gps_relaunch_armed"

  override init() {
    super.init()
    GMSCachePropertyValueTransformer.register()
  }

  override func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {
    GMSServices.provideAPIKey("AIzaSyBsArziADK3hi3pExLejSjbjP4xDBtJhDk")
    // Required for flutter_local_notifications to display when app is in foreground
    UNUserNotificationCenter.current().delegate = self as UNUserNotificationCenterDelegate
    GeneratedPluginRegistrant.register(with: self)
    SwiftFlutterForegroundTaskPlugin.setPluginRegistrantCallback { registry in
      GeneratedPluginRegistrant.register(with: registry)
    }
    if UserDefaults.standard.bool(forKey: tripGpsArmedKey) {
      startTripGpsRelaunchMonitor()
    }
    if let registrar = self.registrar(forPlugin: "TripGpsRelaunch") {
      let channel = FlutterMethodChannel(
        name: "com.loadly.loadly_driver/trip_gps",
        binaryMessenger: registrar.messenger()
      )
      channel.setMethodCallHandler { [weak self] call, result in
        guard let self else {
          result(nil)
          return
        }
        switch call.method {
        case "arm":
          UserDefaults.standard.set(true, forKey: self.tripGpsArmedKey)
          self.startTripGpsRelaunchMonitor()
          result(nil)
        case "disarm":
          UserDefaults.standard.set(false, forKey: self.tripGpsArmedKey)
          self.tripGpsManager.stopMonitoringSignificantLocationChanges()
          result(nil)
        case "takePendingFix":
          result(self.takePendingTripGpsFix())
        default:
          result(FlutterMethodNotImplemented)
        }
      }
    }
    return super.application(application, didFinishLaunchingWithOptions: launchOptions)
  }

  private func startTripGpsRelaunchMonitor() {
    guard tripGpsManager.authorizationStatus == .authorizedAlways else { return }
    tripGpsManager.delegate = self
    tripGpsManager.pausesLocationUpdatesAutomatically = false
    tripGpsManager.activityType = .automotiveNavigation
    tripGpsManager.allowsBackgroundLocationUpdates = true
    tripGpsManager.startMonitoringSignificantLocationChanges()
  }

  func locationManager(_ manager: CLLocationManager, didUpdateLocations locations: [CLLocation]) {
    guard let location = locations.last else { return }
    let defaults = UserDefaults.standard
    defaults.set(location.coordinate.latitude, forKey: "trip_gps_pending_lat")
    defaults.set(location.coordinate.longitude, forKey: "trip_gps_pending_lng")
    defaults.set(location.horizontalAccuracy, forKey: "trip_gps_pending_accuracy")
    defaults.set(location.timestamp.timeIntervalSince1970, forKey: "trip_gps_pending_time")
    if location.speed >= 0 {
      defaults.set(location.speed, forKey: "trip_gps_pending_speed")
    } else {
      defaults.removeObject(forKey: "trip_gps_pending_speed")
    }
  }

  private func takePendingTripGpsFix() -> [String: Any]? {
    let defaults = UserDefaults.standard
    guard defaults.object(forKey: "trip_gps_pending_lat") != nil else { return nil }
    var payload: [String: Any] = [
      "latitude": defaults.double(forKey: "trip_gps_pending_lat"),
      "longitude": defaults.double(forKey: "trip_gps_pending_lng"),
      "accuracy": defaults.double(forKey: "trip_gps_pending_accuracy"),
      "timestamp": defaults.double(forKey: "trip_gps_pending_time"),
    ]
    if defaults.object(forKey: "trip_gps_pending_speed") != nil {
      payload["speed"] = defaults.double(forKey: "trip_gps_pending_speed")
    }
    defaults.removeObject(forKey: "trip_gps_pending_lat")
    defaults.removeObject(forKey: "trip_gps_pending_lng")
    defaults.removeObject(forKey: "trip_gps_pending_accuracy")
    defaults.removeObject(forKey: "trip_gps_pending_time")
    defaults.removeObject(forKey: "trip_gps_pending_speed")
    return payload
  }
}
