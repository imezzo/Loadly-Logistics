import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_blue_pattern_background.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_dialog_presenter.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/trip/trip_bloc.dart';
import '../bloc/trip/trip_event.dart';
import '../bloc/trip/trip_state.dart';
import '../../domain/entities/trip/trip.dart' hide Driver;

class TripDetailsBooking extends StatefulWidget {
  const TripDetailsBooking({
    super.key,
    required this.trip,
    required this.onBack,
  });

  final Trip trip;
  final VoidCallback onBack;

  @override
  State<TripDetailsBooking> createState() => _TripDetailsBookingState();
}

class _TripDetailsBookingState extends State<TripDetailsBooking> {
  late Trip _trip;

  @override
  void initState() {
    super.initState();
    _trip = widget.trip;
  }

  @override
  void didUpdateWidget(covariant TripDetailsBooking oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.trip.id != widget.trip.id ||
        oldWidget.trip.updatedAt != widget.trip.updatedAt) {
      _trip = widget.trip;
    }
  }

  @override
  void dispose() {
    super.dispose();
  }

  void _bookTrip() {
    context.read<TripBloc>().add(BookTrip(widget.trip.id!));
  }

  void _showBookingSuccessDialog() {
    final l10n = AppLocalizations.of(context)!;

    showCupertinoModalPopup<void>(
      context: context,
      barrierDismissible: false,
      builder: (bottomSheetContext) {
        return Container(
          width: double.infinity,
          padding: const EdgeInsets.all(24),
          decoration: const BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.only(
              topLeft: Radius.circular(24),
              topRight: Radius.circular(24),
            ),
          ),
          child: SafeArea(
            top: false,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                SvgPicture.asset(
                  'assets/images/register_success.svg',
                  fit: BoxFit.cover,
                ),
                const SizedBox(height: 24),
                Text(
                  l10n.tripBookedTitle,
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w400,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 10),
                Text(
                  l10n.tripBookedMessage,
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w400,
                    color: CupertinoColors.systemGrey,
                  ),
                ),
                const SizedBox(height: 20),
                SizedBox(
                  width: double.infinity,
                  child: AppButton(
                    onPressed: () {
                      Navigator.of(bottomSheetContext).pop();
                      widget.onBack();
                    },
                    label: l10n.goToHomepage,
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocListener<TripBloc, TripState>(
      listenWhen: (previous, current) =>
          current is TripBooked || current is TripBookError,
      listener: (context, state) {
        if (state is TripBooked) {
          _showBookingSuccessDialog();
        } else if (state is TripBookError) {
          showAppAlertDialog(
            context: context,
            title: l10n.bookTrip,
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
              _BookTripHeader(title: l10n.bookTrip, onBack: widget.onBack),
              Expanded(
                child: SingleChildScrollView(
                  padding: const EdgeInsets.fromLTRB(10, 0, 10, 16),
                  child: _TripCard(trip: _trip),
                ),
              ),
              SafeArea(
                top: false,
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(16, 0, 16, 12),
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      BlocBuilder<TripBloc, TripState>(
                        builder: (context, state) {
                          final isBooking = state is TripBooking;
                          return SizedBox(
                            width: double.infinity,
                            child: AppButton(
                              loading: isBooking,
                              onPressed: _bookTrip,
                              label: l10n.bookTrip,
                            ),
                          );
                        },
                      ),
                    ],
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

class _BookTripHeader extends StatelessWidget {
  const _BookTripHeader({required this.title, required this.onBack});

  final String title;
  final VoidCallback onBack;

  @override
  Widget build(BuildContext context) {
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
                  color: CupertinoColors.white,
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                ),
              ),
              PositionedDirectional(
                start: 8,
                child: AppBackButton(onPressed: onBack),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _TripCard extends StatelessWidget {
  const _TripCard({required this.trip});

  final Trip trip;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      margin: const EdgeInsets.only(bottom: 8, top: 8),
      decoration: BoxDecoration(
        color: CupertinoColors.white,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: CupertinoColors.black.withValues(alpha: 0.08),
            blurRadius: 16,
            offset: const Offset(0, 5),
          ),
        ],
      ),
      child: Column(
        children: [
          _WarningBanner(sentAt: trip.createdAt ?? trip.updatedAt),
          const SizedBox(height: 8),
          _CompanySection(
            name: trip.companyDisplayName(
                  preferArabic:
                      Directionality.of(context) == TextDirection.rtl,
                ) ??
                '',
          ),
          const SizedBox(height: 8),
          _DateSection(
            date: trip.startDate == null
                ? null
                : DateTime.tryParse(trip.startDate!)?.toLocal(),
          ),
          const SizedBox(height: 8),
          _PointsSection(
            from: trip.waypoints?.first.addressName ?? '—',
            to: trip.waypoints?.last.addressName ?? '—',
          ),
          const SizedBox(height: 8),
          _VehicleSection(
            name:
                trip.vehicleTypeDisplayName(
                  preferArabic: Directionality.of(context) == TextDirection.rtl,
                ) ??
                '—',
          ),
          const SizedBox(height: 8),
          _PriceSection(price: trip.contractorPrice),
          const SizedBox(height: 8),
        ],
      ),
    );
  }
}

class _WarningBanner extends StatelessWidget {
  const _WarningBanner({this.sentAt});

  final DateTime? sentAt;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRtl = Directionality.of(context) == TextDirection.rtl;
    final elapsed = _elapsedSince(l10n, sentAt, isRtl: isRtl);
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
      decoration: const BoxDecoration(
        color: Color(0xFFF5FAFF),
        borderRadius: BorderRadius.only(
          topLeft: Radius.circular(14),
          topRight: Radius.circular(14),
        ),
      ),
      child: Row(
        children: [
          const Icon(CupertinoIcons.clock, color: Color(0xFF0874C9), size: 19),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              l10n.loadlyAdminSendThisTripFromAgo(elapsed),
              textAlign: TextAlign.start,
              style: const TextStyle(
                color: Color(0xFF0874C9),
                fontSize: 13,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

String _elapsedSince(
  AppLocalizations l10n,
  DateTime? sentAt, {
  required bool isRtl,
}) {
  if (sentAt == null) {
    return l10n.lessThanAMinute;
  }

  var diff = DateTime.now().difference(sentAt);
  if (diff.isNegative) diff = Duration.zero;

  if (diff.inMinutes < 1) {
    return l10n.lessThanAMinute;
  }
  if (diff.inHours < 1) {
    return _formatElapsed(
      l10n,
      diff.inMinutes,
      isRtl: isRtl,
      unit: _ElapsedUnit.minute,
    );
  }
  if (diff.inDays < 1) {
    return _formatElapsed(
      l10n,
      diff.inHours,
      isRtl: isRtl,
      unit: _ElapsedUnit.hour,
    );
  }
  return _formatElapsed(l10n, diff.inDays, isRtl: isRtl, unit: _ElapsedUnit.day);
}

enum _ElapsedUnit { minute, hour, day }

String _formatElapsed(
  AppLocalizations l10n,
  int count, {
  required bool isRtl,
  required _ElapsedUnit unit,
}) {
  if (!isRtl) {
    final label = switch (unit) {
      _ElapsedUnit.minute =>
        count == 1 ? l10n.elapsedMinuteUnit : l10n.elapsedMinutesUnit,
      _ElapsedUnit.hour =>
        count == 1 ? l10n.elapsedHourUnit : l10n.elapsedHoursUnit,
      _ElapsedUnit.day =>
        count == 1 ? l10n.elapsedDayUnit : l10n.elapsedDaysUnit,
    };
    return '$count $label';
  }

  if (count == 2) {
    return switch (unit) {
      _ElapsedUnit.minute => l10n.elapsedTwoMinutes,
      _ElapsedUnit.hour => l10n.elapsedTwoHours,
      _ElapsedUnit.day => l10n.elapsedTwoDays,
    };
  }

  final few = count >= 3 && count <= 10;
  return switch (unit) {
    _ElapsedUnit.minute => count == 1
        ? l10n.elapsedMinuteSingular
        : (few
              ? l10n.elapsedMinutesFew(count)
              : l10n.elapsedMinutesOther(count)),
    _ElapsedUnit.hour => count == 1
        ? l10n.elapsedHourSingular
        : (few ? l10n.elapsedHoursFew(count) : l10n.elapsedHoursOther(count)),
    _ElapsedUnit.day => count == 1
        ? l10n.elapsedDaySingular
        : (few ? l10n.elapsedDaysFew(count) : l10n.elapsedDaysOther(count)),
  };
}

class _CompanySection extends StatelessWidget {
  const _CompanySection({this.name});

  final String? name;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 15),
      child: Row(
        children: [
          Text(
            name?.isNotEmpty == true ? name! : '—',
            textAlign: TextAlign.start,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
              color: Color(0xFF181818),
            ),
          ),
        ],
      ),
    );
  }
}

class _DateSection extends StatelessWidget {
  const _DateSection({this.date});

  final DateTime? date;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final dateStr = date == null
        ? '—'
        : DateFormat('dd-MM-yyyy  •  hh:mm a').format(date!.toLocal());

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
              fontSize: 13,
              color: Color(0xFF858585),
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 5),
          Text(
            dateStr,
            style: const TextStyle(
              fontSize: 13,
              color: Color(0xFF777777),
              fontWeight: FontWeight.bold,
            ),
          ),
        ],
      ),
    );
  }
}

class _PointsSection extends StatelessWidget {
  const _PointsSection({this.from, this.to});

  final String? from;
  final String? to;

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
      child: Row(
        children: [
          Expanded(
            child: _PointItem(
              title: l10n.pickupPoint,
              value: from?.isNotEmpty == true ? from! : '—',
            ),
          ),
          Container(width: 1, height: 45, color: const Color(0xFFE5E5E5)),
          Expanded(
            child: _PointItem(
              title: l10n.dropoffPoint,
              value: to?.isNotEmpty == true ? to! : '—',
            ),
          ),
        ],
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
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Text(
          title,
          style: const TextStyle(
            fontSize: 12,
            color: Color(0xFF969696),
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 5),
        Text(
          value,
          textAlign: TextAlign.center,
          style: const TextStyle(
            fontSize: 13,
            color: Color(0xFF222222),
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }
}

class _VehicleSection extends StatelessWidget {
  const _VehicleSection({this.name});

  final String? name;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Container(
      height: 56,
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 9),
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
            child: Padding(
              padding: const EdgeInsets.all(
                1,
              ), // عدّل الرقم لو الصورة كبيرة/صغيرة
              child: SvgPicture.asset(
                'assets/images/trip_review_truck.svg', // ← مسار الصورة عندك
                fit: BoxFit.contain,
              ),
            ),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              children: [
                Text(
                  l10n.vehicleType,
                  style: const TextStyle(
                    fontSize: 12,
                    color: Color(0xFF969696),
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text(
                  name?.isNotEmpty == true ? name! : '—',
                  textAlign: TextAlign.start,
                  style: const TextStyle(
                    fontSize: 14,
                    color: Color(0xFF222222),
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _PriceSection extends StatelessWidget {
  const _PriceSection({this.price});

  final double? price;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final formatted = price == null
        ? '—'
        : l10n.priceInEgp(NumberFormat('#,###').format(price!.round()));

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
            formatted,
            style: const TextStyle(
              fontSize: 14,
              color: Color(0xFF222222),
              fontWeight: FontWeight.bold,
            ),
          ),
          const Spacer(),
          Text(
            l10n.tripCost,
            style: const TextStyle(
              fontSize: 13,
              color: Color(0xFF333333),
              fontWeight: FontWeight.bold,
            ),
          ),
        ],
      ),
    );
  }
}
