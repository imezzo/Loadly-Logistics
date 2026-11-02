/// Guard to indicate whether the app is currently showing an auth-related flow
/// where unauthorized responses should not trigger a redirect to login.
abstract final class AuthScreenGuard {
  static bool isOnAuthScreen = false;
}
