import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_blue_pattern_background.dart';
import '../../../../core/widgets/app_dialog_presenter.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../home/presentation/widgets/trip_card_details.dart';
import '../../domain/entities/booking_request/booking_request.dart';
import '../bloc/trip/trip_bloc.dart';
import '../bloc/trip/trip_event.dart';
import '../bloc/trip/trip_state.dart';

class EditBookingRequestScreen extends StatefulWidget {
  const EditBookingRequestScreen({super.key, required this.booking});

  final BookingRequest booking;

  @override
  State<EditBookingRequestScreen> createState() =>
      _EditBookingRequestScreenState();
}

class _EditBookingRequestScreenState extends State<EditBookingRequestScreen> {
  int? _tripIdFromBooking(BookingRequest booking) {
    return booking.tripId ?? booking.trip?.id;
  }

  Future<void> _confirmCancelBooking(AppLocalizations l10n) async {
    if (widget.booking.status != BookingRequest.editableTripStatus) {
      showAppAlertDialog(
        context: context,
        title: l10n.cancelTrip,
        message: l10n.cannotEditThisTrip,
      );
      return;
    }

    final tripId = _tripIdFromBooking(widget.booking);
    if (tripId == null) {
      showAppAlertDialog(
        context: context,
        title: l10n.error,
        message: l10n.error,
      );
      return;
    }

    final confirmed = await showCupertinoDialog<bool>(
      context: context,
      builder: (dialogContext) => CupertinoAlertDialog(
        title: Text(l10n.cancelTrip),
        content: Text(l10n.confirmCancelTrip),
        actions: [
          CupertinoDialogAction(
            onPressed: () => Navigator.of(dialogContext).pop(false),
            child: Text(l10n.cancel),
          ),
          CupertinoDialogAction(
            isDestructiveAction: true,
            isDefaultAction: true,
            onPressed: () => Navigator.of(dialogContext).pop(true),
            child: Text(l10n.confirm),
          ),
        ],
      ),
    );

    if (confirmed == true && mounted) {
      context.read<TripBloc>().add(WithdrawTrip(tripId));
    }
  }

  void _goBackToBookedRequests() {
    context.go(AppRoutePaths.requestsPath(booked: true));
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final booking = widget.booking;
    final details = resolveTripCardDetails(context, booking: booking);
    final tripId = details.tripId ?? booking.tripId ?? '—';

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          _goBackToBookedRequests();
        }
      },
      child: BlocListener<TripBloc, TripState>(
        listenWhen: (previous, current) =>
            current is TripWithdrawn || current is TripWithdrawError,
        listener: (context, state) {
          if (state is TripWithdrawn) {
            showAppAlertDialog(
              context: context,
              title: l10n.tripWasCancelled,
              message: l10n.tripWasCancelled,
            ).then((_) {
              if (context.mounted) {
                _goBackToBookedRequests();
              }
            });
          } else if (state is TripWithdrawError) {
            showAppAlertDialog(
              context: context,
              title: l10n.error,
              message: state.message.isEmpty
                  ? l10n.error
                  : FailureMessageMapper.forMessage(state.message, l10n),
            );
          }
        },
        child: CupertinoPageScaffold(
          backgroundColor: const Color(0xFFFDFDFD),
          child: MainScaffold(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                _Header(
                  title: '${l10n.trip} #$tripId',
                  onBack: _goBackToBookedRequests,
                ),
                Expanded(
                  child: SingleChildScrollView(
                    padding: const EdgeInsets.fromLTRB(10, 8, 10, 16),
                    child: Container(
                      width: double.infinity,
                      decoration: BoxDecoration(
                        color: CupertinoColors.white,
                        borderRadius: BorderRadius.circular(14),
                        boxShadow: [
                          BoxShadow(
                            color: CupertinoColors.black.withValues(
                              alpha: 0.08,
                            ),
                            blurRadius: 16,
                            offset: const Offset(0, 5),
                          ),
                        ],
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          _StatusBanner(text: l10n.waitingForManagerApproval),
                          Container(height: 1, color: const Color(0xFFE5E9ED)),
                          Padding(
                            padding: const EdgeInsets.fromLTRB(15, 12, 15, 0),
                            child: Text(
                              details.companyName,
                              textAlign: TextAlign.start,
                              style: const TextStyle(
                                fontFamily: appFontFamily,
                                fontSize: 16,
                                fontWeight: FontWeight.w700,
                                color: Color(0xFF181818),
                              ),
                            ),
                          ),
                          const SizedBox(height: 10),
                          _DateSection(dateText: details.dateStr),
                          const SizedBox(height: 8),
                          _PointsSection(from: details.from, to: details.to),
                          const SizedBox(height: 8),
                          _VehicleSection(name: details.vehicleName),
                          const SizedBox(height: 8),
                          _PriceSection(price: details.priceText),
                          const SizedBox(height: 12),
                          Padding(
                            padding: const EdgeInsets.fromLTRB(10, 0, 10, 12),
                            child: BlocBuilder<TripBloc, TripState>(
                              builder: (context, state) {
                                final isWithdrawing = state is TripWithdrawing;

                                return GestureDetector(
                                  onTap: isWithdrawing
                                      ? null
                                      : () => _confirmCancelBooking(l10n),
                                  child: Container(
                                    width: double.infinity,
                                    padding: const EdgeInsets.symmetric(
                                      vertical: 12,
                                    ),
                                    decoration: BoxDecoration(
                                      color: const Color(0xFFFFF0F0),
                                      borderRadius: BorderRadius.circular(10),
                                    ),
                                    alignment: Alignment.center,
                                    child: isWithdrawing
                                        ? const CupertinoActivityIndicator(
                                            color: AppColors.red,
                                          )
                                        : Text(
                                            l10n.cancelBooking,
                                            style: const TextStyle(
                                              fontFamily: appFontFamily,
                                              fontSize: 14,
                                              fontWeight: FontWeight.w600,
                                              color: AppColors.red,
                                            ),
                                          ),
                                  ),
                                );
                              },
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _Header extends StatelessWidget {
  const _Header({required this.title, required this.onBack});

  final String title;
  final VoidCallback onBack;

  @override
  Widget build(BuildContext context) {
    final isRtl = Directionality.of(context) == TextDirection.rtl;

    return AppBluePatternBackground(
      child: SafeArea(
        bottom: false,
        child: SizedBox(
          height: 56,
          width: double.infinity,
          child: Stack(
            alignment: Alignment.center,
            children: [
              Text(
                title,
                style: const TextStyle(
                  fontFamily: appFontFamily,
                  color: CupertinoColors.white,
                  fontSize: 18,
                  fontWeight: FontWeight.w700,
                ),
              ),
              PositionedDirectional(
                start: 12,
                child: CupertinoButton(
                  padding: EdgeInsets.zero,
                  minimumSize: Size.zero,
                  onPressed: onBack,
                  child: Icon(
                    isRtl
                        ? CupertinoIcons.chevron_forward
                        : CupertinoIcons.chevron_back,
                    color: CupertinoColors.white,
                    size: 22,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _StatusBanner extends StatelessWidget {
  const _StatusBanner({required this.text});

  final String text;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(12, 12, 12, 10),
      child: Row(
        children: [
          const Icon(CupertinoIcons.clock, color: Color(0xFF0874C9), size: 18),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              text,
              textAlign: TextAlign.start,
              style: const TextStyle(
                fontFamily: appFontFamily,
                color: Color(0xFF0874C9),
                fontSize: 13,
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _DateSection extends StatelessWidget {
  const _DateSection({required this.dateText});

  final String dateText;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      width: double.infinity,
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(vertical: 10),
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFE5E9ED)),
        borderRadius: BorderRadius.circular(14),
      ),
      child: Column(
        children: [
          Text(
            l10n.tripDate,
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 13,
              color: Color(0xFF000000),
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 5),
          Text(
            dateText,
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 13,
              color: Color(0xFF777777),
              fontWeight: FontWeight.w700,
            ),
          ),
        ],
      ),
    );
  }
}

class _PointsSection extends StatelessWidget {
  const _PointsSection({required this.from, required this.to});

  final String from;
  final String to;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFE5E9ED)),
        borderRadius: BorderRadius.circular(14),
      ),
      child: IntrinsicHeight(
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Expanded(
              child: _PointItem(title: l10n.pickupPoint, value: from),
            ),
            Container(width: 1, color: const Color(0xFFE5E5E5)),
            Expanded(
              child: _PointItem(title: l10n.dropoffPoint, value: to),
            ),
          ],
        ),
      ),
    );
  }
}

class _PointItem extends StatelessWidget {
  const _PointItem({required this.title, required this.value});

  final String title;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.center,
      mainAxisAlignment: MainAxisAlignment.start,
      children: [
        Text(
          title,
          textAlign: TextAlign.center,
          style: const TextStyle(
            fontFamily: appFontFamily,
            fontSize: 12,
            color: Color(0xFF969696),
            fontWeight: FontWeight.w700,
          ),
        ),
        const SizedBox(height: 5),
        Text(
          value,
          textAlign: TextAlign.center,
          style: const TextStyle(
            fontFamily: appFontFamily,
            fontSize: 13,
            color: Color(0xFF222222),
            fontWeight: FontWeight.w700,
          ),
        ),
      ],
    );
  }
}

class _VehicleSection extends StatelessWidget {
  const _VehicleSection({required this.name});

  final String name;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 9, vertical: 10),
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFE5E9ED)),
        borderRadius: BorderRadius.circular(14),
      ),
      child: Row(
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: const Color(0xFFF0F8FE),
              borderRadius: BorderRadius.circular(9),
            ),
            child: SvgPicture.asset(
              'assets/images/trip_review_truck.svg',
              width: 28,
              height: 28,
              fit: BoxFit.scaleDown,
            ),
          ),
          const SizedBox(width: 10),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                l10n.vehicleType,
                textAlign: TextAlign.start,
                style: const TextStyle(
                  fontFamily: appFontFamily,
                  fontSize: 12,
                  color: Color(0xFF969696),
                  fontWeight: FontWeight.w700,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                name,
                textAlign: TextAlign.start,
                style: const TextStyle(
                  fontFamily: appFontFamily,
                  fontSize: 14,
                  color: Color(0xFF222222),
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
          const Spacer(),
        ],
      ),
    );
  }
}

class _PriceSection extends StatelessWidget {
  const _PriceSection({required this.price});

  final String price;

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 49,
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 13),
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFE5E9ED)),
        borderRadius: BorderRadius.circular(13),
      ),
      child: Row(
        children: [
          Text(
            AppLocalizations.of(context)!.tripCost,
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 13,
              color: Color(0xFF333333),
              fontWeight: FontWeight.w700,
            ),
          ),
          const Spacer(),
          Text(
            price,
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 14,
              color: Color(0xFF222222),
              fontWeight: FontWeight.w700,
            ),
          ),
        ],
      ),
    );
  }
}
