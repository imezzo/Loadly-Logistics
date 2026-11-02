import 'package:go_router/go_router.dart';

/// Global router instance holder for interceptors and other cross-cutting
/// services that need navigation without a BuildContext.
class AppRouterHolder {
  AppRouterHolder._();
  static final AppRouterHolder instance = AppRouterHolder._();

  GoRouter? _router;

  void setRouter(GoRouter router) {
    _router = router;
  }

  GoRouter? get router => _router;
}
