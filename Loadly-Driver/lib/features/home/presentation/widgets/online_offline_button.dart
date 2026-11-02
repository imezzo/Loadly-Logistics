import 'package:flutter/cupertino.dart';

import '../../../../core/theme/app_colors.dart';

/// Circular button widget for toggling online/offline status
class OnlineOfflineButton extends StatefulWidget {
  final bool isOnline;
  final VoidCallback onToggle;
  final bool isLoading;

  const OnlineOfflineButton({
    super.key,
    required this.isOnline,
    required this.onToggle,
    this.isLoading = false,
  });

  @override
  State<OnlineOfflineButton> createState() => _OnlineOfflineButtonState();
}

class _OnlineOfflineButtonState extends State<OnlineOfflineButton>
    with TickerProviderStateMixin {
  late AnimationController _scaleController;
  late AnimationController _pulseController;
  late AnimationController _rippleController;
  late Animation<double> _scaleAnimation;
  late Animation<double> _pulseAnimation;
  late Animation<double> _rippleAnimation;
  late Animation<double> _rippleOpacityAnimation;

  @override
  void initState() {
    super.initState();

    _scaleController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 150),
    );
    _scaleAnimation = Tween<double>(begin: 1.0, end: 0.95).animate(
      CurvedAnimation(parent: _scaleController, curve: Curves.easeInOut),
    );

    _pulseController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 600),
    );
    _pulseAnimation = Tween<double>(
      begin: 1.0,
      end: 1.15,
    ).animate(CurvedAnimation(parent: _pulseController, curve: Curves.easeOut));

    _rippleController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 600),
    );
    _rippleAnimation = Tween<double>(begin: 1.0, end: 2.5).animate(
      CurvedAnimation(parent: _rippleController, curve: Curves.easeOut),
    );
    _rippleOpacityAnimation = Tween<double>(begin: 0.6, end: 0.0).animate(
      CurvedAnimation(parent: _rippleController, curve: Curves.easeOut),
    );
  }

  @override
  void didUpdateWidget(OnlineOfflineButton oldWidget) {
    super.didUpdateWidget(oldWidget);
    // Trigger pulse animation when state changes
    if (oldWidget.isOnline != widget.isOnline) {
      _pulseController.forward().then((_) {
        _pulseController.reverse();
      });
    }
  }

  @override
  void dispose() {
    _scaleController.dispose();
    _pulseController.dispose();
    _rippleController.dispose();
    super.dispose();
  }

  void _onTapDown(TapDownDetails details) {
    _scaleController.forward();
    // Start ripple/bubble animation
    _rippleController.reset();
    _rippleController.forward();
  }

  void _onTapUp(TapUpDetails details) {
    _scaleController.reverse();
    if (!widget.isLoading) {
      widget.onToggle();
    }
  }

  void _onTapCancel() {
    _scaleController.reverse();
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTapDown: _onTapDown,
      onTapUp: _onTapUp,
      onTapCancel: _onTapCancel,
      child: Stack(
        alignment: Alignment.center,
        children: [
          // Ripple/bubble effect
          AnimatedBuilder(
            animation: _rippleController,
            builder: (context, child) {
              return Transform.scale(
                scale: _rippleAnimation.value,
                child: Opacity(
                  opacity: _rippleOpacityAnimation.value,
                  child: Container(
                    width: 80,
                    height: 80,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      border: Border.all(
                        color: widget.isOnline
                            ? AppColors.green.withValues(alpha: 0.8)
                            : AppColors.red.withValues(alpha: 0.8),
                        width: 2,
                      ),
                    ),
                  ),
                ),
              );
            },
          ),
          // Main button
          Transform.scale(
            scale: _scaleAnimation.value * _pulseAnimation.value,
            child: AnimatedContainer(
              duration: const Duration(milliseconds: 400),
              curve: Curves.easeInOut,
              width: 80,
              height: 80,
              decoration: BoxDecoration(
                color: AppColors.lightGray,
                shape: BoxShape.circle,
                border: Border.all(
                  color: widget.isOnline ? AppColors.green : AppColors.red,
                  width: 2.5,
                ),
                boxShadow: [
                  BoxShadow(
                    color: widget.isOnline
                        ? AppColors.green.withValues(alpha: 0.5)
                        : AppColors.red.withValues(alpha: 0.5),
                    blurRadius: widget.isOnline ? 24 : 24,
                    offset: const Offset(0, 6),
                    spreadRadius: widget.isOnline ? 4 : 4,
                  ),
                ],
              ),
              child: widget.isLoading
                  ? const Center(
                      child: CupertinoActivityIndicator(
                        color: AppColors.white,
                        radius: 12,
                      ),
                    )
                  : const Center(
                      child: Icon(
                        CupertinoIcons.power,
                        color: AppColors.darkGray,
                        size: 36,
                      ),
                    ),
            ),
          ),
        ],
      ),
    );
  }
}
