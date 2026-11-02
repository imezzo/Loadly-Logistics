import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/track_trips_result.dart';

class TrackTripPopup extends StatelessWidget {
  final TrackTripsResult trip;
  const TrackTripPopup({super.key, required this.trip});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 16),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: CupertinoColors.black.withValues(alpha: 0.12),
            blurRadius: 20,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          // ── Header ─────────────────────────────────────────
          Text(
            l10n.trackingDelivery,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w500,
              color: AppColors.darkGray,
            ),
          ),

          // ── Shipment Info ───────────────────────────────────
          Container(
            margin: const EdgeInsets.only(top: 10),
            padding: const EdgeInsets.all(5),
            decoration: BoxDecoration(
              color: AppColors.mainGray,
              borderRadius: BorderRadius.circular(12),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: AppColors.white,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          _CircleButton(
                            icon: SvgPicture.asset(
                              'assets/images/truck.svg',
                              fit: BoxFit.cover,
                            ),
                            onTap: () {},
                            padding: 10,
                            size: 40,
                          ),
                          const SizedBox(width: 10),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  trip.companyName!,
                                  style: const TextStyle(
                                    fontSize: 14,
                                    fontWeight: FontWeight.w400,
                                    color: CupertinoColors.systemGrey,
                                  ),
                                ),
                                const SizedBox(height: 2),
                                Text(
                                  '${l10n.from} ${trip.waypoints!.first.addressName} ${l10n.to} ${trip.waypoints!.last.addressName}',
                                  style: const TextStyle(
                                    fontSize: 14,
                                    fontWeight: FontWeight.w500,
                                    color: AppColors.darkGray,
                                  ),
                                ),
                              ],
                            ),
                          ),

                          // EnRoute Badge
                          Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 5,
                            ),
                            decoration: BoxDecoration(
                              color: CupertinoColors.activeOrange.withValues(
                                alpha: 0.1,
                              ),
                              borderRadius: BorderRadius.circular(16),
                              // border: Border.all(color: const Color(0xFFEAC84A)),
                            ),
                            child: Text(
                              l10n.inProgress,
                              style: const TextStyle(
                                fontSize: 12,
                                fontWeight: FontWeight.w500,
                                color: CupertinoColors.activeOrange,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),

                // ── Driver Info ─────────────────────────────────────
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 10),
                  child: Row(
                    children: [
                      // Avatar
                      Container(
                        width: 42,
                        height: 42,
                        decoration: const BoxDecoration(
                          shape: BoxShape.circle,
                          color: Color(0xFFF2F2F7),
                        ),
                        child: const Icon(
                          CupertinoIcons.person_fill,
                          size: 24,
                          color: CupertinoColors.systemGrey,
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              trip.driverName!,
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w500,
                                color: AppColors.darkGray,
                              ),
                            ),
                            const SizedBox(height: 2),
                            Text(
                              trip.vehicleType!,
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w400,
                                color: CupertinoColors.systemGrey,
                              ),
                              overflow: TextOverflow.ellipsis,
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(width: 8),
                      // Action Buttons
                      _CircleButton(
                        size: 35,
                        icon: const Icon(
                          CupertinoIcons.phone,
                          size: 15,
                          color: AppColors.darkGray,
                        ),
                        onTap: () async => await launchUrl(
                          Uri(scheme: 'tel', path: trip.driverMobile),
                        ),
                      ),
                      const SizedBox(width: 8),
                      _CircleButton(
                        size: 35,
                        icon: const Icon(
                          CupertinoIcons.chat_bubble,
                          size: 15,
                          color: AppColors.darkGray,
                        ),
                        onTap: () async => await launchUrl(
                          Uri(scheme: 'sms', path: trip.driverMobile),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),

          // ── Click to Details ────────────────────────────────
          CupertinoButton(
            onPressed: () =>
                context.go('/trips/${trip.id}', extra: '/track-trips'),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                Text(
                  l10n.clickToDetails,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w500,
                    color: AppColors.mainBlue,
                  ),
                ),
                const SizedBox(width: 4),
                Icon(
                  Directionality.of(context) == TextDirection.rtl
                      ? CupertinoIcons.chevron_left
                      : CupertinoIcons.chevron_right,
                  size: 14,
                  color: AppColors.mainBlue,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _CircleButton extends StatelessWidget {
  final double size;
  final Widget icon;
  final double padding;
  final VoidCallback onTap;

  const _CircleButton({
    required this.icon,
    required this.onTap,
    required this.size,
    this.padding = 0,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: EdgeInsets.all(padding),
        width: size,
        height: size,
        decoration: BoxDecoration(
          shape: BoxShape.circle,
          border: Border.all(color: const Color(0xFFE5E5EA)),
          color: CupertinoColors.white,
        ),
        child: icon,
      ),
    );
  }
}
