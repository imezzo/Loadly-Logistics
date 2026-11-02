import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/trip.dart';

/// Trip status codes (API guide). [kTripCompletedStatus] is the value used when requesting
/// completed trips from the API; 7 is also treated as completed in the UI.
const int kTripScheduledStatus = 2;
const int kTripInProgressStatus = 5;
const int kTripCompletedStatus = kTripStatusCodeCompleted;
const int kTripCancelledStatus = kTripStatusCodeCancelled;

/// Reusable trip card widget used in Trips list and Home screen.
/// When [returnRoute] is set (e.g. '/home'), uses pushReplacement to dispose the
/// previous screen (e.g. Home map) and reduce Google Maps CCTClearcutUploader instances.
/// When [animateBubble] is true, shows a continuous bubble/pulse animation.
class TripCard extends StatefulWidget {
  const TripCard({
    super.key,
    required this.trip,
    required this.l10n,
    this.isRtl,
    this.onTap,
    this.returnRoute,
    this.animateBubble = false,
  });

  final Trip trip;
  final AppLocalizations l10n;
  final bool? isRtl;
  final VoidCallback? onTap;

  /// When set, uses pushReplacement and passes this route for the back button.
  final String? returnRoute;

  /// When true, shows a continuous bubble animation (e.g. on homepage).
  final bool animateBubble;

  @override
  State<TripCard> createState() => _TripCardState();
}

class _TripCardState extends State<TripCard>
    with SingleTickerProviderStateMixin {
  late AnimationController _bubbleController;
  late Animation<double> _bubbleScaleAnimation;
  late Animation<double> _bubbleOpacityAnimation;

  @override
  void initState() {
    super.initState();
    _bubbleController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 2000),
    );
    _bubbleScaleAnimation = Tween<double>(begin: 1.0, end: 2.2).animate(
      CurvedAnimation(parent: _bubbleController, curve: Curves.easeOut),
    );
    _bubbleOpacityAnimation = Tween<double>(begin: 0.7, end: 0.0).animate(
      CurvedAnimation(parent: _bubbleController, curve: Curves.easeOut),
    );
    if (widget.animateBubble) {
      _bubbleController.repeat();
    }
  }

  @override
  void didUpdateWidget(TripCard oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.animateBubble && !oldWidget.animateBubble) {
      _bubbleController.repeat();
    } else if (!widget.animateBubble && oldWidget.animateBubble) {
      _bubbleController.stop();
    }
  }

  @override
  void dispose() {
    _bubbleController.dispose();
    super.dispose();
  }

  String _companyName(BuildContext context) {
    final rtl = widget.isRtl ?? Directionality.of(context) == TextDirection.rtl;
    return rtl
        ? (widget.trip.companyNameAr ??
              widget.trip.companyNameEn ??
              widget.trip.title)
        : (widget.trip.companyNameEn ??
              widget.trip.companyNameAr ??
              widget.trip.title);
  }

  String _statusLabel() {
    return _statusCodeToLabel(widget.trip.statusCode);
  }

  String _statusCodeToLabel(int? code) {
    if (code == null) return widget.l10n.statusUnknown;
    switch (code) {
      case 0:
        return widget.l10n.statusRequested;
      case 1:
        return widget.l10n.statusPending;
      case 2:
        return widget.l10n.statusScheduled;
      case 3:
        return widget.l10n.statusDriverAccepted;
      case 4:
        return widget.l10n.statusFirstPickupLoading;
      case 5:
        return widget.l10n.statusInProgress;
      case kTripStatusCodeCompleted:
      case kTripStatusCodeCompletedReviewed:
        return widget.l10n.statusCompleted;
      case kTripStatusCodeCancelled:
        return widget.l10n.statusCancelled;
      default:
        return widget.l10n.statusUnknown;
    }
  }

  void _defaultOnTap(BuildContext context) {
    if (tripStatusIsCompleted(widget.trip.statusCode)) {
      context.go('/trips/success', extra: {'trip': widget.trip});
      return;
    }
    if (widget.returnRoute != null) {
      context.pushReplacement(
        '/trips/${widget.trip.id}',
        extra: {'returnTo': widget.returnRoute!},
      );
    } else {
      context.push('/trips/${widget.trip.id}');
    }
  }

  bool get _isTerminalStatus => tripStatusIsTerminal(widget.trip.statusCode);

  Widget _buildStatusChip() {
    final isTerminal = _isTerminalStatus;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: isTerminal ? AppColors.lightGray : const Color(0xFFE8F5E9),
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (!isTerminal)
            const Icon(CupertinoIcons.clock, size: 14, color: AppColors.green),
          if (!isTerminal) const SizedBox(width: 6),
          Text(
            _statusLabel(),
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w500,
              color: isTerminal ? AppColors.darkGray : AppColors.green,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCardContent(BuildContext context, VoidCallback onPressed) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray, width: 1),
        boxShadow: [
          BoxShadow(
            color: AppColors.lightGray.withValues(alpha: 0.3),
            blurRadius: 4,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: CupertinoButton(
        padding: EdgeInsets.zero,
        onPressed: onPressed,
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Company name with truck icon and Trip ID chip
              Row(
                children: [
                  const Icon(
                    CupertinoIcons.cube_box_fill,
                    size: 20,
                    color: AppColors.mainBlue,
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      _companyName(context),
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w600,
                        color: AppColors.darkGray,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 10,
                      vertical: 4,
                    ),
                    decoration: BoxDecoration(
                      color: AppColors.mainBlue.withValues(alpha: 0.12),
                      borderRadius: BorderRadius.circular(16),
                      border: Border.all(
                        color: AppColors.mainBlue.withValues(alpha: 0.3),
                        width: 1,
                      ),
                    ),
                    child: Text(
                      '${widget.l10n.trip} #${widget.trip.id}',
                      style: const TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.w500,
                        color: AppColors.mainBlue,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              // Trip schedule
              Text(
                widget.l10n.tripOnDayAtTime(
                  widget.trip.startDate,
                  widget.trip.startTime,
                ),
                style: const TextStyle(fontSize: 14, color: AppColors.darkGray),
              ),
              const SizedBox(height: 8),
              // Pickup location
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Icon(
                    CupertinoIcons.location,
                    size: 16,
                    color: AppColors.mediumBlueGray,
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      widget.trip.pickupLocation.address,
                      style: const TextStyle(
                        fontSize: 14,
                        color: AppColors.darkGray,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              // Status chip - centered
              Center(child: _buildStatusChip()),
              const SizedBox(height: 16),
              // Shipment Details button
              SizedBox(
                width: double.infinity,
                child: AppButton(
                  label: widget.l10n.shipmentDetails,
                  onPressed: onPressed,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final onPressed = widget.onTap ?? () => _defaultOnTap(context);

    if (!widget.animateBubble) {
      return _buildCardContent(context, onPressed);
    }

    return Stack(
      alignment: Alignment.center,
      clipBehavior: Clip.none,
      children: [
        _buildCardContent(context, onPressed),
        // Bubble animation - expanding border around the card
        Positioned.fill(
          child: IgnorePointer(
            child: LayoutBuilder(
              builder: (context, constraints) {
                return AnimatedBuilder(
                  animation: _bubbleController,
                  builder: (context, child) {
                    return Center(
                      child: Transform.scale(
                        scale: _bubbleScaleAnimation.value,
                        alignment: Alignment.center,
                        child: Opacity(
                          opacity: _bubbleOpacityAnimation.value,
                          child: Container(
                            width: constraints.maxWidth,
                            height: constraints.maxHeight,
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(12),
                              border: Border.all(
                                color: AppColors.mainBlue,
                                width: 2.5,
                              ),
                              boxShadow: [
                                BoxShadow(
                                  color: AppColors.mainBlue.withValues(
                                    alpha: 0.3,
                                  ),
                                  blurRadius: 8,
                                  spreadRadius: 1,
                                ),
                              ],
                            ),
                          ),
                        ),
                      ),
                    );
                  },
                );
              },
            ),
          ),
        ),
      ],
    );
  }
}
