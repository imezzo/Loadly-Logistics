import Flutter
import UIKit
import GoogleMaps
import UserNotifications

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
@objc class AppDelegate: FlutterAppDelegate {
  override init() {
    super.init()
    GMSCachePropertyValueTransformer.register()
  }

  override func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {
    GMSServices.provideAPIKey("AIzaSyDuheVGE2F6FAe9NsMyKs_Dzud0PLVb9Ss")
    // Required for flutter_local_notifications to display when app is in foreground
    UNUserNotificationCenter.current().delegate = self as UNUserNotificationCenterDelegate
    GeneratedPluginRegistrant.register(with: self)
    return super.application(application, didFinishLaunchingWithOptions: launchOptions)
  }
}
