import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/domain/entities/booking_request/booking_request.dart';
import '../../../trips/domain/entities/trip/trip.dart';
import '../../../trips/presentation/bloc/trip/trip_bloc.dart';
import '../../../trips/presentation/bloc/trip/trip_state.dart';
import 'trip_card_details.dart';

class TripReviewCard extends StatelessWidget {
  const TripReviewCard({super.key, this.booking, this.trip})
    : assert(booking != null || trip != null);

  final BookingRequest? booking;
  final Trip? trip;

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<TripBloc, TripState>(
      builder: (context, _) {
        final l10n = AppLocalizations.of(context)!;
        final details = resolveTripCardDetails(
          context,
          booking: booking,
          trip: trip,
        );
        final tripNumber = details.tripId == null ? '—' : '${details.tripId}';
        final statuses = _statuses(l10n);
        final status = _statusFor(
          statusCode: details.statusCode,
          statusName: details.statusName,
          statuses: statuses,
        );

        return GestureDetector(
          onTap: () {
            final id = details.tripId ?? trip?.id;
            if (id != null) {
              context.go(
                AppRoutePaths.tripById(id),
                extra: AppRoutePaths.trips,
              );
            }
          },
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 2, vertical: 4),
            decoration: BoxDecoration(
              color:
                  (trip?.status == 1 || trip?.status == 2) &&
                      trip?.driver == null
                  ? AppColors.mainBlue
                  : AppColors.white,
              borderRadius: BorderRadius.circular(10),
              border:
                  (trip?.status == 1 || trip?.status == 2) &&
                      trip?.driver == null
                  ? null
                  : Border.all(color: AppColors.lightGray),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(vertical: 10),
                  decoration: BoxDecoration(
                    color: AppColors.white,
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      CupertinoListTile(
                        leading: SvgPicture.asset(
                          'assets/images/trip_review_truck.svg',
                          width: 40,
                          height: 40,
                        ),
                        leadingSize: 40,
                        title: _Title(
                          title: '${l10n.trip} #$tripNumber',
                          fontSize: 14,
                          color: AppColors.darkGray,
                        ),
                        subtitle: _Title(
                          title: details.companyName,
                          fontSize: 14,
                        ),
                        trailing: Padding(
                          padding: const EdgeInsetsDirectional.only(end: 6),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 12,
                                  vertical: 5,
                                ),
                                decoration: BoxDecoration(
                                  color: status.color.withValues(alpha: 0.1),
                                  borderRadius: BorderRadius.circular(16),
                                ),
                                child: Text(
                                  status.name,
                                  style: TextStyle(
                                    fontSize: 12,
                                    fontWeight: FontWeight.w500,
                                    color: status.color,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                      Padding(
                        padding: EdgeInsets.symmetric(
                          horizontal: MediaQuery.sizeOf(context).width * 0.2,
                        ),
                        child: _Title(title: details.dateStr, fontSize: 14),
                      ),
                      Padding(
                        padding: const EdgeInsets.fromLTRB(16, 0, 16, 4),
                        child: Row(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            SvgPicture.asset('assets/images/location.svg'),
                            const SizedBox(width: 12),
                            Expanded(
                              child: Row(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Expanded(
                                    child: _LocationValue(
                                      label: l10n.tripFrom,
                                      value: details.from,
                                    ),
                                  ),
                                  const SizedBox(width: 12),
                                  Expanded(
                                    child: _LocationValue(
                                      label: l10n.tripTo,
                                      value: details.to,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
                if ((trip?.status == 1 || trip?.status == 2) &&
                    trip?.driver == null) ...[
                  Padding(
                    padding: const EdgeInsets.fromLTRB(5, 8, 5, 5),
                    child: Row(
                      children: [
                        const Icon(
                          CupertinoIcons.clock,
                          color: AppColors.white,
                          size: 20,
                        ),
                        const SizedBox(width: 10),
                        Text(
                          l10n.assignTheDriver,
                          style: const TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.w400,
                            color: AppColors.white,
                          ),
                        ),
                        const Spacer(),
                        CupertinoButton(
                          onPressed: () {
                            final id = trip?.id ?? details.tripId;
                            if (id != null) {
                              context.go(
                                AppRoutePaths.tripById(id),
                                extra: AppRoutePaths.trips,
                              );
                            }
                          },
                          padding: const EdgeInsets.symmetric(
                            horizontal: 10,
                            vertical: 5,
                          ),
                          color: AppColors.white,
                          borderRadius: BorderRadius.circular(10),
                          child: Text(
                            l10n.chooseDriver,
                            style: const TextStyle(
                              fontSize: 12,
                              fontWeight: FontWeight.w400,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ],
            ),
          ),
        );
      },
    );
  }
}

Status _statusFor({
  required int? statusCode,
  required String? statusName,
  required List<Status> statuses,
}) {
  if (statusCode != null) {
    for (final status in statuses) {
      if (status.code == statusCode) return status;
    }
  }

  if (statusName != null && statusName.trim().isNotEmpty) {
    final lower = statusName.toLowerCase();
    for (final status in statuses) {
      if (status.name.toLowerCase() == lower) return status;
    }
    return Status(
      name: statusName,
      code: statusCode,
      color: CupertinoColors.activeBlue,
      icon: CupertinoIcons.doc_text,
    );
  }

  return statuses.first;
}

List<Status> _statuses(AppLocalizations l10n) {
  return [
    Status(
      name: l10n.all,
      code: null,
      color: CupertinoColors.activeBlue,
      icon: CupertinoIcons.infinite,
    ),
    Status(
      name: l10n.requested,
      code: 0,
      color: CupertinoColors.systemIndigo,
      icon: CupertinoIcons.paperplane_fill,
    ),
    Status(
      name: l10n.pending,
      code: 1,
      color: CupertinoColors.systemPurple,
      icon: CupertinoIcons.bookmark_fill,
    ),
    Status(
      name: l10n.scheduled,
      code: 2,
      color: AppColors.newgreen,
      icon: CupertinoIcons.table_fill,
    ),
    Status(
      name: l10n.loadingCargo,
      code: 3,
      color: const Color(0xFFE89B5D),
      icon: CupertinoIcons.cube_box_fill,
    ),
    Status(
      name: l10n.inProgress,
      code: 5,
      color: AppColors.orange,
      icon: CupertinoIcons.slowmo,
    ),
    Status(
      name: l10n.tripCompleted,
      code: 6,
      color: const Color(0xFF0874C9),
      icon: CupertinoIcons.checkmark_alt,
    ),
    Status(
      name: l10n.reviewed,
      code: 7,
      color: CupertinoColors.systemTeal,
      icon: CupertinoIcons.doc_checkmark_fill,
    ),
    Status(
      name: l10n.tripWasCancelled,
      code: 8,
      color: AppColors.red,
      icon: CupertinoIcons.xmark,
    ),
    Status(
      name: l10n.draft,
      code: -1,
      color: CupertinoColors.systemMint,
      icon: CupertinoIcons.doc_fill,
    ),
  ];
}

class _Title extends StatelessWidget {
  const _Title({
    required this.title,
    required this.fontSize,
    this.color = CupertinoColors.systemGrey,
  });

  final String title;
  final double fontSize;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      maxLines: 2,
      overflow: TextOverflow.ellipsis,
      style: TextStyle(
        fontSize: fontSize,
        color: color,
        fontWeight: FontWeight.w400,
      ),
    );
  }
}

class _LocationValue extends StatelessWidget {
  const _LocationValue({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: const TextStyle(
            fontSize: 12,
            fontWeight: FontWeight.w400,
            color: Color(0xFF7A8A99),
          ),
        ),
        const SizedBox(height: 2),
        Text(
          value,
          softWrap: true,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w400,
            color: AppColors.darkGray,
          ),
        ),
      ],
    );
  }
}

class Status {
  Status({
    required this.name,
    required this.code,
    required this.color,
    required this.icon,
  });

  String name;
  int? code;
  Color color;
  IconData icon;
}
