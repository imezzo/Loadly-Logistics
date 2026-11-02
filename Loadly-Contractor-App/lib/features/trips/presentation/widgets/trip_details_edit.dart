import 'dart:io' show File;

import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;
import 'package:url_launcher/url_launcher.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_blue_pattern_background.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_dialog_presenter.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../drivers/domain/entities/driver/driver.dart' as driver_entity;
import '../../../drivers/presentation/bloc/driver_bloc.dart';
import '../../../drivers/presentation/bloc/driver_event.dart';
import '../../../drivers/presentation/bloc/driver_state.dart';
import '../../../home/presentation/widgets/trip_card_details.dart';
import '../../../home/presentation/widgets/trip_review_card.dart';
import '../../domain/entities/book_trip/book_trip_request.dart';
import '../../../drivers/domain/entities/update_trip_driver/update_trip_driver.dart';
import '../../domain/entities/trip/trip.dart';

class TripDetailsEdit extends StatefulWidget {
  const TripDetailsEdit({super.key, required this.trip, required this.onBack});

  final Trip trip;
  final VoidCallback onBack;

  @override
  State<TripDetailsEdit> createState() => _TripDetailsEditState();
}

class _TripDetailsEditState extends State<TripDetailsEdit> {
  driver_entity.Driver? _selectedDriver;
  driver_entity.Driver? _originalDriver;
  bool _isChangingDriver = false;

  @override
  void initState() {
    super.initState();
    final initialDriver = _driverFromTrip(widget.trip);
    _selectedDriver = initialDriver;
    _originalDriver = initialDriver;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      context.read<DriverBloc>().add(GetDrivers());
      _syncDriverFromBackend();
    });
  }

  void _syncDriverFromBackend() {
    if (_isChangingDriver || !mounted) return;

    final resolved = _resolveDriverFromBackend(context);
    if (resolved != null) {
      setState(() {
        _selectedDriver = resolved;
        _originalDriver = resolved;
      });
    }
  }

  bool _hasDriverChanged() {
    final selected = _selectedDriver;
    if (selected == null) return false;

    final original = _originalDriver;
    if (original == null) return true;

    final selectedId = selected.id;
    final originalId = original.id;
    if (selectedId != null &&
        originalId != null &&
        selectedId != 0 &&
        originalId != 0) {
      return selectedId != originalId;
    }

    return selected != original;
  }

  driver_entity.Driver? _resolveDriverFromBackend(BuildContext context) {
    final trip = _resolvedTrip(context);
    if (trip?.driver != null) {
      return _mapTripDriver(trip!.driver!);
    }

    final driverId = trip?.driverId;
    if (driverId == null) return _selectedDriver;

    final driverState = context.read<DriverBloc>().state;
    if (driverState is DriverLoaded) {
      for (final driver in driverState.drivers) {
        if (driver.id == driverId) return driver;
      }
    }

    return _selectedDriver;
  }

  Trip? _resolvedTrip(BuildContext context) {
    final details = resolveTripCardDetails(context, trip: widget.trip);
    return details.trip ?? widget.trip;
  }

  driver_entity.Driver? _driverFromTrip(Trip trip) {
    if (trip.driver == null) return null;
    return _mapTripDriver(trip.driver!);
  }

  driver_entity.Driver _mapTripDriver(Driver driver) {
    return driver_entity.Driver(
      id: driver.id,
      fullNameAr: driver.fullNameAr ?? driver.fullName,
      fullNameEn: driver.fullNameEn ?? driver.fullName,
      phone: driver.phone,
      nationalId: driver.nationalId,
    );
  }

  void _confirmChanges(AppLocalizations l10n) {
    final driver = _selectedDriver;
    if (driver == null) {
      showAppAlertDialog(
        context: context,
        title: l10n.confirmChanges,
        message: l10n.pleaseSelectDriver,
      );
      return;
    }

    final tripId = widget.trip.id;
    if (tripId == null) {
      showAppAlertDialog(
        context: context,
        title: l10n.error,
        message: l10n.error,
      );
      return;
    }

    if (driver.isNew) {
      if (driver.documents.isEmpty) {
        showAppAlertDialog(
          context: context,
          title: l10n.confirmChanges,
          message: l10n.pleaseUploadRequiredDocuments,
        );
        return;
      }

      context.read<DriverBloc>().add(
        SubmitTripDriverUpdate(
          UpdateTripDriver(
            tripId: tripId,
            driver: BookTripDriver(
              fullNameEn: driver.fullNameEn ?? '',
              fullNameAr: driver.fullNameAr ?? '',
              phone: driver.phone ?? '',
              nationalId: driver.nationalId ?? '',
              documents: driver.documents
                  .map(
                    (document) => BookTripDriverDocument(
                      documentId: document.documentId,
                      filePath: document.filePath,
                      expiryDate: document.expiryDate,
                    ),
                  )
                  .toList(),
            ),
          ),
        ),
      );
      return;
    }

    context.read<DriverBloc>().add(
      SubmitTripDriverUpdate(
        UpdateTripDriver(tripId: tripId, driverId: driver.id),
      ),
    );
  }

  void _goBack() => widget.onBack();

  void _showDriverAssignedSuccessDialog() {
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
                  l10n.youHaveSuccessfullyAddedTheDriver,
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w400,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 10),
                Text(
                  l10n.youHaveSuccessfullyAddedTheDriverDesc(
                    DateFormat(
                      'd MMM yyyy - h:mm a',
                      AppLocalizations.of(context)!.localeName,
                    ).format(DateTime.parse(widget.trip.startDate!).toLocal()),
                  ),
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
                    label: l10n.back,
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
    final trip = _resolvedTrip(context) ?? widget.trip;
    final details = resolveTripCardDetails(context, trip: widget.trip);
    final statuses = _statuses(l10n);
    final status = _statusFor(
      statusCode: details.statusCode,
      statusName: details.statusName,
      statuses: statuses,
    );
    final tripId = details.tripId ?? trip.id ?? '—';
    final showDriverSelection =
        _isChangingDriver ||
        ((trip.status == 1 || trip.status == 2) && _selectedDriver == null);

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          _goBack();
        }
      },
      child: BlocListener<DriverBloc, DriverState>(
        listenWhen: (previous, current) => current is DriverLoaded,
        listener: (context, state) => _syncDriverFromBackend(),
        child: BlocListener<DriverBloc, DriverState>(
          listenWhen: (previous, current) =>
              current is TripDriverUpdated || current is TripDriverUpdateError,
          listener: (context, state) {
            if (state is TripDriverUpdated) {
              if ((trip.status == 1 || trip.status == 2) &&
                  trip.driver == null) {
                _showDriverAssignedSuccessDialog();
              } else {
                showAppAlertDialog(
                  context: context,
                  title: l10n.editRequestUpdatedTitle,
                  message: l10n.editRequestUpdatedMessage,
                ).then((_) {
                  if (!context.mounted) return;
                  _goBack();
                });
              }
            } else if (state is TripDriverUpdateError) {
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
                  _EditAcceptedHeader(
                    title: '${l10n.trip} #$tripId',
                    onBack: _goBack,
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
                            if ((trip.status == 1 || trip.status == 2) &&
                                trip.driver == null) ...[
                              _StatusBanner(
                                text: l10n
                                    .yourSupervisorHasApprovedTheTripPleaseAddDriver,
                              ),
                              Container(
                                height: 1,
                                color: const Color(0xFFE5E9ED),
                              ),
                            ],
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
                            const SizedBox(height: 12),
                            Align(
                              alignment: AlignmentDirectional.centerStart,
                              child: _StatusBadge(
                                label: status.name,
                                color: status.color,
                                icon: status.icon,
                              ),
                            ),
                            const SizedBox(height: 10),
                            _DateSection(dateText: details.dateStr),
                            const SizedBox(height: 8),
                            _VehicleSection(name: details.vehicleName),
                            const SizedBox(height: 8),
                            _PointsSection(from: details.from, to: details.to),
                            const SizedBox(height: 8),
                            _TripDetailsSection(trip: trip),
                            const SizedBox(height: 8),
                            _FinancialDetailsSection(
                              trip: trip,
                              priceText: details.priceText,
                            ),
                            const SizedBox(height: 12),
                            if (!showDriverSelection && _selectedDriver != null)
                              _DriverDetailsSection(
                                status: trip.status!,
                                driver: _selectedDriver!,
                                onRemove: () => setState(() {
                                  _isChangingDriver = true;
                                  _selectedDriver = null;
                                }),
                              )
                            else if (showDriverSelection) ...[
                              Padding(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 10,
                                ),
                                child: Text(
                                  l10n.selectDriver,
                                  textAlign: TextAlign.start,
                                  style: const TextStyle(
                                    fontFamily: appFontFamily,
                                    fontSize: 13,
                                    fontWeight: FontWeight.w700,
                                    color: Color(0xFF222222),
                                  ),
                                ),
                              ),
                              const SizedBox(height: 8),
                              _ChangeDriverSection(
                                selectedDriver: _selectedDriver,
                                onDriverSelected: (driver) {
                                  setState(() => _selectedDriver = driver);
                                },
                                changeDriverLabel: l10n.changeDriver,
                                chooseNewDriverLabel: l10n.chooseNewDriver,
                                driverSubtitle: l10n.driversFromYourList,
                                addDriverLabel: l10n.tapToAddNewDriver,
                              ),
                            ],
                            if (_hasDriverChanged()) ...[
                              const SizedBox(height: 12),
                              Padding(
                                padding: const EdgeInsets.fromLTRB(
                                  10,
                                  0,
                                  10,
                                  12,
                                ),
                                child: BlocBuilder<DriverBloc, DriverState>(
                                  builder: (context, state) {
                                    final isUpdating =
                                        state is TripDriverUpdating;

                                    return SizedBox(
                                      width: double.infinity,
                                      child: AppButton(
                                        label:
                                            (trip.status == 1 ||
                                                    trip.status == 2) &&
                                                trip.driver == null
                                            ? l10n.readyForTrip
                                            : l10n.confirmChanges,
                                        loading: isUpdating,
                                        onPressed: isUpdating
                                            ? null
                                            : () => _confirmChanges(l10n),
                                      ),
                                    );
                                  },
                                ),
                              ),
                            ],
                            const SizedBox(height: 12),
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
      ),
    );
  }
}

class _EditAcceptedHeader extends StatelessWidget {
  const _EditAcceptedHeader({required this.title, required this.onBack});

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
          const Icon(
            CupertinoIcons.rectangle_stack_person_crop,
            color: AppColors.mainBlue,
            size: 18,
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              text,
              textAlign: TextAlign.start,
              style: const TextStyle(
                fontFamily: appFontFamily,
                color: AppColors.mainBlue,
                fontSize: 12,
                fontWeight: FontWeight.w400,
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

  static const _timelineBlue = Color(0xFF0874C9);

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFE5E9ED)),
        borderRadius: BorderRadius.circular(14),
      ),
      child: IntrinsicHeight(
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            const SizedBox(
              width: 12,
              child: Column(
                children: [
                  _TimelineDot(color: _timelineBlue),
                  Expanded(
                    child: CustomPaint(
                      painter: _DashedLinePainter(color: _timelineBlue),
                      child: SizedBox(width: 12),
                    ),
                  ),
                  _TimelineDot(color: _timelineBlue),
                ],
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _PointItem(title: l10n.dropoffPoint, value: to),
                  const SizedBox(height: 20),
                  _PointItem(title: l10n.pickupPoint, value: from),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _TimelineDot extends StatelessWidget {
  const _TimelineDot({required this.color});

  final Color color;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 12,
      height: 12,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: CupertinoColors.white,
        border: Border.all(color: color, width: 2),
      ),
    );
  }
}

class _DashedLinePainter extends CustomPainter {
  const _DashedLinePainter({required this.color});

  final Color color;

  @override
  void paint(Canvas canvas, Size size) {
    const dashHeight = 4.0;
    const dashSpace = 4.0;
    final paint = Paint()
      ..color = color
      ..strokeWidth = 2;

    var startY = 0.0;
    while (startY < size.height) {
      canvas.drawLine(
        Offset(size.width / 2, startY),
        Offset(size.width / 2, startY + dashHeight),
        paint,
      );
      startY += dashHeight + dashSpace;
    }
  }

  @override
  bool shouldRepaint(covariant _DashedLinePainter oldDelegate) {
    return oldDelegate.color != color;
  }
}

class _PointItem extends StatelessWidget {
  const _PointItem({required this.title, required this.value});

  final String title;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisAlignment: MainAxisAlignment.start,
      children: [
        Text(
          title,
          textAlign: TextAlign.start,
          style: const TextStyle(
            fontFamily: appFontFamily,
            fontSize: 12,
            color: Color(0xFF969696),
            fontWeight: FontWeight.w400,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          value,
          textAlign: TextAlign.start,
          style: const TextStyle(
            fontFamily: appFontFamily,
            fontSize: 14,
            color: Color(0xFF222222),
            fontWeight: FontWeight.w700,
          ),
        ),
      ],
    );
  }
}

class _TripDetailsSection extends StatelessWidget {
  const _TripDetailsSection({required this.trip});

  final Trip trip;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFE5E9ED)),
        borderRadius: BorderRadius.circular(14),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          _TripDetailRow(
            label: l10n.tripDispatchTime,
            value: _formatDateTime(trip.createdAt, trip.startDate),
          ),
          _TripDetailRow(
            label: l10n.tripAcceptanceTime,
            value: _formatDateTime(trip.updatedAt, trip.endDate),
          ),
          _TripDetailRow(
            label: l10n.elapsedTripDuration,
            value: _elapsedDuration(l10n, trip),
          ),
          _TripDetailRow(
            label: l10n.distanceInKm,
            value: _distanceText(l10n, trip),
          ),
          _TripDetailRow(label: l10n.notes, value: l10n.noNotes, isLast: true),
        ],
      ),
    );
  }

  static String _formatDateTime(DateTime? dateTime, String? isoString) {
    final parsed =
        dateTime ?? (isoString == null ? null : DateTime.tryParse(isoString));
    if (parsed == null) return '—';
    return DateFormat('dd-MM-yyyy - hh:mm a').format(parsed.toLocal());
  }

  static String _elapsedDuration(AppLocalizations l10n, Trip trip) {
    final start = trip.startDate == null
        ? trip.createdAt
        : DateTime.tryParse(trip.startDate!) ?? trip.createdAt;
    final end = trip.endDate == null
        ? trip.updatedAt
        : DateTime.tryParse(trip.endDate!) ?? trip.updatedAt;

    if (start != null && end != null && end.isAfter(start)) {
      final hours = end.difference(start).inHours;
      if (hours > 0) {
        return l10n.hoursCount('$hours', l10n.hours);
      }
      return l10n.elapsedMinutesShort(end.difference(start).inMinutes);
    }

    if (trip.numberOfDays != null && trip.numberOfDays! > 0) {
      return l10n.hoursCount('${trip.numberOfDays! * 24}', l10n.hours);
    }
    if (trip.cutoffDuration != null && trip.cutoffDuration! > 0) {
      return l10n.hoursCount('${trip.cutoffDuration}', l10n.hours);
    }
    return '—';
  }

  static String _distanceText(AppLocalizations l10n, Trip trip) {
    final km =
        trip.actualDistanceKm ??
        trip.contractDistanceKm?.toDouble() ??
        trip.contractDestination?.expectedKms?.toDouble();
    if (km == null) return '—';
    final formatted = km == km.roundToDouble()
        ? km.round().toString()
        : km.toStringAsFixed(1);
    return l10n.kmCount(formatted);
  }
}

class _TripDetailRow extends StatelessWidget {
  const _TripDetailRow({
    required this.label,
    required this.value,
    this.isLast = false,
  });

  final String label;
  final String value;
  final bool isLast;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(bottom: isLast ? 0 : 10),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            ': $label',
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 14,
              color: Color(0xFF7A8A99),
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              value,
              textAlign: TextAlign.end,
              style: const TextStyle(
                fontFamily: appFontFamily,
                fontSize: 14,
                color: Color(0xFF222222),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _FinancialDetailsSection extends StatelessWidget {
  const _FinancialDetailsSection({required this.trip, required this.priceText});

  final Trip? trip;
  final String priceText;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isArabic = Localizations.localeOf(context).languageCode == 'ar';

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFE5E9ED)),
        borderRadius: BorderRadius.circular(14),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            l10n.financialDetails,
            textAlign: TextAlign.start,
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 15,
              fontWeight: FontWeight.w700,
              color: Color(0xFF222222),
            ),
          ),
          const SizedBox(height: 14),
          _TripDetailRow(
            label: l10n.tripCost,
            value: _tripCostText(l10n, priceText, isArabic: isArabic),
          ),
          _TripDetailRow(
            label: l10n.otherDiscounts,
            value: _discountText(
              l10n,
              trip?.totalAddonsCost,
              isArabic: isArabic,
            ),
            isLast: true,
          ),
        ],
      ),
    );
  }

  static String _tripCostText(
    AppLocalizations l10n,
    String priceText, {
    required bool isArabic,
  }) {
    final amount = priceText.trim().isEmpty || priceText == '—'
        ? '-'
        : priceText;
    return _egpPrice(l10n, amount, isArabic: isArabic);
  }

  static String _discountText(
    AppLocalizations l10n,
    double? totalAddonsCost, {
    required bool isArabic,
  }) {
    if (totalAddonsCost == null) {
      return _egpPrice(l10n, '-', isArabic: isArabic);
    }

    final formatted = totalAddonsCost == totalAddonsCost.roundToDouble()
        ? NumberFormat(
            '#,###',
          ).format(totalAddonsCost.round()).replaceAll(',', '.')
        : NumberFormat('#,##0.00').format(totalAddonsCost).replaceAll(',', '.');
    return _egpPrice(l10n, formatted, isArabic: isArabic);
  }

  static String _egpPrice(
    AppLocalizations l10n,
    String amount, {
    required bool isArabic,
  }) {
    if (isArabic) {
      return l10n.priceInArabicEgpFull(amount);
    }
    return l10n.priceInEgp(amount);
  }
}

class _DriverDetailsSection extends StatelessWidget {
  const _DriverDetailsSection({
    required this.status,
    required this.driver,
    required this.onRemove,
  });

  final int status;
  final driver_entity.Driver driver;
  final VoidCallback onRemove;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final name = _driverName(context, driver);
    final phone = driver.phone?.trim();
    final hasPhone = phone != null && phone.isNotEmpty;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10),
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFE5E9ED)),
        borderRadius: BorderRadius.circular(14),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  l10n.driverDetails,
                  textAlign: TextAlign.start,
                  style: const TextStyle(
                    fontFamily: appFontFamily,
                    fontSize: 15,
                    fontWeight: FontWeight.w700,
                    color: Color(0xFF222222),
                  ),
                ),
              ),
              if (status == 2)
                CupertinoButton(
                  padding: EdgeInsets.zero,
                  minimumSize: Size.zero,
                  onPressed: onRemove,
                  child: Container(
                    width: 35,
                    height: 35,
                    decoration: const BoxDecoration(
                      color: Color(0xFFFFF0F0),
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(
                      CupertinoIcons.delete,
                      size: 15,
                      color: AppColors.red,
                    ),
                  ),
                ),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              _DriverAvatar(driver: driver, name: name),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      name,
                      textAlign: TextAlign.start,
                      style: const TextStyle(
                        fontFamily: appFontFamily,
                        fontSize: 14,
                        fontWeight: FontWeight.w700,
                        color: Color(0xFF222222),
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      hasPhone ? phone : '—',
                      textAlign: TextAlign.start,
                      style: const TextStyle(
                        fontFamily: appFontFamily,
                        fontSize: 13,
                        color: Color(0xFF7A8A99),
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),
              CupertinoButton(
                padding: EdgeInsets.zero,
                minimumSize: Size.zero,
                onPressed: hasPhone ? () => _callDriver(phone) : null,
                child: Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: CupertinoColors.white,
                    border: Border.all(color: const Color(0xFFDADADA)),
                  ),
                  child: Icon(
                    CupertinoIcons.phone,
                    size: 18,
                    color: hasPhone
                        ? const Color(0xFF666666)
                        : const Color(0xFFCCCCCC),
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _DriverAvatar extends StatelessWidget {
  const _DriverAvatar({required this.driver, required this.name});

  final driver_entity.Driver driver;
  final String name;

  @override
  Widget build(BuildContext context) {
    final documentPath = driver.documents.isNotEmpty
        ? driver.documents.first.filePath
        : null;
    final hasDocumentImage =
        documentPath != null &&
        documentPath.isNotEmpty &&
        File(documentPath).existsSync();

    return Container(
      width: 44,
      height: 44,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        border: Border.all(color: const Color(0xFFE5E9ED)),
      ),
      clipBehavior: Clip.antiAlias,
      child: hasDocumentImage
          ? Image.file(File(documentPath), fit: BoxFit.cover)
          : ColoredBox(
              color: const Color(0xFFEFF7FF),
              child: Center(
                child: name.isNotEmpty
                    ? Text(
                        name[0].toUpperCase(),
                        style: const TextStyle(
                          fontFamily: appFontFamily,
                          color: AppColors.mainBlue,
                          fontWeight: FontWeight.w700,
                          fontSize: 16,
                        ),
                      )
                    : const Icon(
                        CupertinoIcons.person_crop_circle,
                        color: AppColors.mainBlue,
                        size: 24,
                      ),
              ),
            ),
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

class _ChangeDriverSection extends StatefulWidget {
  const _ChangeDriverSection({
    required this.selectedDriver,
    required this.onDriverSelected,
    required this.changeDriverLabel,
    required this.chooseNewDriverLabel,
    required this.driverSubtitle,
    required this.addDriverLabel,
  });

  final driver_entity.Driver? selectedDriver;
  final ValueChanged<driver_entity.Driver> onDriverSelected;
  final String changeDriverLabel;
  final String chooseNewDriverLabel;
  final String driverSubtitle;
  final String addDriverLabel;

  @override
  State<_ChangeDriverSection> createState() => _ChangeDriverSectionState();
}

class _ChangeDriverSectionState extends State<_ChangeDriverSection> {
  @override
  void initState() {
    super.initState();
    context.read<DriverBloc>().add(GetDrivers());
  }

  Future<void> _openDriversSheet() async {
    final selected = await showCupertinoModalPopup<driver_entity.Driver>(
      context: context,
      builder: (sheetContext) =>
          _DriversSheet(initiallySelected: widget.selectedDriver),
    );

    if (selected != null && mounted) {
      widget.onDriverSelected(selected);
    }
  }

  Future<void> _openAddDriver() async {
    final created = await context.push<driver_entity.Driver?>(
      AppRoutePaths.addDriver,
    );
    if (created != null && mounted) {
      widget.onDriverSelected(created);
    }
  }

  @override
  Widget build(BuildContext context) {
    final driverTitle = widget.selectedDriver == null
        ? widget.chooseNewDriverLabel
        : _driverDisplayName(widget.selectedDriver!);

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 10),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          border: Border.all(color: const Color(0xFFE5E9ED)),
          borderRadius: BorderRadius.circular(13),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: Text(
                widget.changeDriverLabel,
                style: const TextStyle(
                  fontFamily: appFontFamily,
                  fontSize: 12,
                  color: Color(0xFF969696),
                  fontWeight: FontWeight.w700,
                ),
              ),
            ),
            const SizedBox(height: 8),
            GestureDetector(
              onTap: _openDriversSheet,
              child: Container(
                height: 57,
                decoration: BoxDecoration(
                  border: Border.all(color: const Color(0xFFE5E9ED)),
                  borderRadius: BorderRadius.circular(13),
                ),
                child: Row(
                  children: [
                    Container(
                      width: 41,
                      height: 41,
                      margin: const EdgeInsetsDirectional.only(start: 7),
                      decoration: BoxDecoration(
                        color: const Color(0xFFF0F8FE),
                        borderRadius: BorderRadius.circular(9),
                      ),
                      child: const Icon(
                        CupertinoIcons.person_crop_rectangle,
                        color: Color(0xFF0874C9),
                        size: 22,
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            driverTitle,
                            textAlign: TextAlign.start,
                            style: const TextStyle(
                              fontFamily: appFontFamily,
                              fontSize: 13,
                              color: Color(0xFF333333),
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          Text(
                            widget.driverSubtitle,
                            textAlign: TextAlign.start,
                            style: const TextStyle(
                              fontFamily: appFontFamily,
                              fontSize: 11,
                              color: Color(0xFF999999),
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                        ],
                      ),
                    ),
                    Container(
                      width: 32,
                      height: 32,
                      margin: const EdgeInsetsDirectional.only(end: 8),
                      decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        border: Border.all(color: const Color(0xFFDADADA)),
                      ),
                      child: const Icon(
                        CupertinoIcons.add,
                        size: 17,
                        color: Color(0xFF222222),
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 8),
            GestureDetector(
              onTap: _openAddDriver,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(
                    CupertinoIcons.add,
                    size: 17,
                    color: Color(0xFF0874C9),
                  ),
                  const SizedBox(width: 6),
                  Text(
                    widget.addDriverLabel,
                    style: const TextStyle(
                      fontFamily: appFontFamily,
                      fontSize: 12,
                      color: Color(0xFF0874C9),
                      fontWeight: FontWeight.w700,
                      decoration: TextDecoration.underline,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _DriversSheet extends StatefulWidget {
  const _DriversSheet({this.initiallySelected});

  final driver_entity.Driver? initiallySelected;

  @override
  State<_DriversSheet> createState() => _DriversSheetState();
}

class _DriversSheetState extends State<_DriversSheet> {
  late final TextEditingController _searchController;
  driver_entity.Driver? _tempSelected;

  @override
  void initState() {
    super.initState();
    _searchController = TextEditingController();
    _tempSelected = widget.initiallySelected;
    context.read<DriverBloc>().add(GetDrivers());
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      height: MediaQuery.sizeOf(context).height * 0.7,
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 16),
      decoration: const BoxDecoration(
        color: CupertinoColors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      child: Column(
        children: [
          Container(
            width: 40,
            height: 4,
            decoration: BoxDecoration(
              color: const Color(0xFFDADADA),
              borderRadius: BorderRadius.circular(4),
            ),
          ),
          const SizedBox(height: 12),
          Align(
            alignment: AlignmentDirectional.centerStart,
            child: Text(
              l10n.selectDriver,
              style: const TextStyle(
                fontFamily: appFontFamily,
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Color(0xFF222222),
              ),
            ),
          ),
          const SizedBox(height: 12),
          CupertinoTextField(
            controller: _searchController,
            placeholder: l10n.searchForDriver,
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
            decoration: BoxDecoration(
              color: const Color(0xFFF7F7F7),
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: const Color(0xFFE5E9ED)),
            ),
            onChanged: (_) => setState(() {}),
          ),
          const SizedBox(height: 12),
          Expanded(
            child: BlocBuilder<DriverBloc, DriverState>(
              builder: (context, state) {
                if (state is DriverError) {
                  return Center(
                    child: Text(
                      FailureMessageMapper.forMessage(state.message, l10n),
                    ),
                  );
                }

                final loaded = state is DriverLoaded ? state : null;
                final drivers =
                    loaded?.drivers ?? const <driver_entity.Driver>[];
                final loading =
                    state is DriverLoading || (loaded?.loadingDrivers ?? false);

                if (loading && drivers.isEmpty) {
                  return const Center(child: CupertinoActivityIndicator());
                }

                final query = _searchController.text.trim();
                final filtered = drivers.where((driver) {
                  if (query.isEmpty) return true;
                  final name = _driverDisplayName(driver);
                  return name.contains(query) ||
                      (driver.phone?.contains(query) ?? false);
                }).toList();

                if (filtered.isEmpty) {
                  return Center(child: Text(l10n.noDrivers));
                }

                return ListView.separated(
                  itemCount: filtered.length,
                  separatorBuilder: (_, _) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final driver = filtered[index];
                    final isSelected = _tempSelected?.id == driver.id;

                    return GestureDetector(
                      onTap: () => setState(() => _tempSelected = driver),
                      child: Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: isSelected
                              ? const Color(0xFFEFF7FF)
                              : CupertinoColors.white,
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                            color: isSelected
                                ? const Color(0xFF0874C9)
                                : const Color(0xFFE5E9ED),
                          ),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              _driverDisplayName(driver),
                              style: const TextStyle(
                                fontFamily: appFontFamily,
                                fontSize: 14,
                                fontWeight: FontWeight.bold,
                                color: Color(0xFF222222),
                              ),
                            ),
                            const SizedBox(height: 4),
                            Text(
                              driver.phone ?? '',
                              style: const TextStyle(
                                fontFamily: appFontFamily,
                                fontSize: 12,
                                color: Color(0xFF888888),
                              ),
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                );
              },
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            child: CupertinoButton(
              color: const Color(0xFF0874C9),
              borderRadius: BorderRadius.circular(12),
              onPressed: _tempSelected == null
                  ? null
                  : () => Navigator.of(context).pop(_tempSelected),
              child: Text(
                l10n.chooseThisDriver,
                style: const TextStyle(
                  fontFamily: appFontFamily,
                  color: CupertinoColors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

String _driverDisplayName(driver_entity.Driver driver) {
  if (driver.fullNameAr != null && driver.fullNameAr!.isNotEmpty) {
    return driver.fullNameAr!;
  }
  if (driver.fullNameEn != null && driver.fullNameEn!.isNotEmpty) {
    return driver.fullNameEn!;
  }
  return driver.phone ?? '';
}

String _driverName(BuildContext context, driver_entity.Driver driver) {
  final isRtl = Directionality.of(context) == TextDirection.rtl;
  if (isRtl) {
    return driver.fullNameAr ?? driver.fullNameEn ?? driver.phone ?? '—';
  }
  return driver.fullNameEn ?? driver.fullNameAr ?? driver.phone ?? '—';
}

Future<void> _callDriver(String phone) async {
  final uri = Uri(scheme: 'tel', path: phone);
  if (await canLaunchUrl(uri)) {
    await launchUrl(uri);
  }
}

class _StatusBadge extends StatelessWidget {
  const _StatusBadge({required this.label, required this.color, this.icon});

  final String label;
  final Color color;
  final IconData? icon;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(16),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (icon != null) ...[
            Icon(icon, size: 14, color: color),
            const SizedBox(width: 4),
          ],
          Text(
            label,
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: color,
            ),
          ),
        ],
      ),
    );
  }
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
