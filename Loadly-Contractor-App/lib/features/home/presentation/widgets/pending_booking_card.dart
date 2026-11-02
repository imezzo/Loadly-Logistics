import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/domain/entities/booking_request/booking_request.dart';
import '../../../trips/domain/entities/trip/trip.dart';
import '../../../trips/presentation/bloc/trip/trip_bloc.dart';
import '../../../trips/presentation/bloc/trip/trip_state.dart';
import 'trip_card_details.dart';

const _headerGreen = Color(0xFF2EA154);

class PendingBookingCard extends StatelessWidget {
  const PendingBookingCard({super.key, this.booking, this.trip, this.onEdit})
    : assert(booking != null || trip != null);

  final BookingRequest? booking;
  final Trip? trip;
  final VoidCallback? onEdit;

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

        return Container(
          decoration: BoxDecoration(
            color: _headerGreen,
            borderRadius: BorderRadius.circular(14),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(12, 10, 12, 10),
                child: Row(
                  children: [
                    const Icon(
                      CupertinoIcons.clock,
                      color: AppColors.white,
                      size: 18,
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        l10n.waitingForSupervisorApproval,
                        style: const TextStyle(
                          color: AppColors.white,
                          fontSize: 13,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
              Container(
                margin: const EdgeInsets.fromLTRB(8, 0, 8, 8),
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppColors.white,
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                details.companyName,
                                maxLines: 2,
                                overflow: TextOverflow.ellipsis,
                                style: const TextStyle(
                                  fontSize: 16,
                                  fontWeight: FontWeight.w700,
                                  color: AppColors.darkGray,
                                ),
                              ),
                              const SizedBox(height: 4),
                              Text(
                                '${l10n.trip} #$tripNumber',
                                style: const TextStyle(
                                  fontSize: 13,
                                  fontWeight: FontWeight.w500,
                                  color: Color(0xFF7A8A99),
                                ),
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(width: 8),
                        Container(
                          constraints: const BoxConstraints(maxWidth: 140),
                          padding: const EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 6,
                          ),
                          decoration: BoxDecoration(
                            color: const Color(0xFFEFF7FF),
                            borderRadius: BorderRadius.circular(20),
                            border: Border.all(color: AppColors.mainBlue),
                          ),
                          child: Text(
                            '${l10n.tripValue}: ${details.priceText}',
                            textAlign: TextAlign.center,
                            style: const TextStyle(
                              fontSize: 12,
                              fontWeight: FontWeight.w600,
                              color: AppColors.mainBlue,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 10),
                    Text(
                      '${l10n.tripDate} ${details.dateStr}',
                      style: const TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.w400,
                        color: Color(0xFF7A8A99),
                      ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: _LabeledValue(
                            label: l10n.tripFrom,
                            value: details.from,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: _LabeledValue(
                            label: l10n.tripTo,
                            value: details.to,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Expanded(
                          child: _LabeledValue(
                            label: l10n.vehicleType,
                            value: details.vehicleName,
                          ),
                        ),
                        const SizedBox(width: 8),
                        _EditRequestButton(
                          label: l10n.editRequest,
                          onTap: onEdit ?? () => _handleEditRequest(context),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  void _handleEditRequest(BuildContext context) {
    if (booking == null) return;

    context.go(AppRoutePaths.editBookingRequest, extra: booking);
  }
}

class _EditRequestButton extends StatelessWidget {
  const _EditRequestButton({required this.label, this.onTap});

  final String label;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        decoration: BoxDecoration(
          color: AppColors.white,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(color: AppColors.mainBlue, width: 1.5),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              label,
              style: const TextStyle(
                color: AppColors.mainBlue,
                fontSize: 13,
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(width: 6),
            const Icon(
              CupertinoIcons.gear_alt,
              color: AppColors.mainBlue,
              size: 18,
            ),
          ],
        ),
      ),
    );
  }
}

class _LabeledValue extends StatelessWidget {
  const _LabeledValue({required this.label, required this.value});

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
            fontWeight: FontWeight.w600,
            color: AppColors.darkGray,
          ),
        ),
      ],
    );
  }
}
