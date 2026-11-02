import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;
import 'package:url_launcher/url_launcher.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../widgets/add_trip_review.dart';
import '../widgets/filter_popup.dart';

/// Trip details screen showing comprehensive trip information
class TripDetailsScreen extends StatefulWidget {
  const TripDetailsScreen({
    super.key,
    required this.tripId,
    required this.backRoute,
  });

  final int tripId;
  final String backRoute;

  @override
  State<TripDetailsScreen> createState() => _TripDetailsScreenState();
}

class _TripDetailsScreenState extends State<TripDetailsScreen> {
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    context.read<TripsBloc>().add(TripsLoadTripDetails(tripId: widget.tripId));
  }

  String formatDate(String isoDate) {
    DateTime dateTime = DateTime.parse(isoDate).toLocal();

    // Get day, month, year
    String day = DateFormat('dd').format(dateTime);
    String month = DateFormat('MMMM').format(dateTime);
    String year = DateFormat('yyyy').format(dateTime);

    // Get 12-hour time
    String time = DateFormat('hh:mm').format(dateTime);
    String period = DateFormat('a').format(dateTime); // AM or PM

    return '$day $month $year - $time $period';
  }

  String estimatedDate({
    String? startDate,
    String? endDate,
    required AppLocalizations l10n,
  }) {
    try {
      final hours = DateTime.parse(
        endDate!,
      ).difference(DateTime.parse(startDate!)).inHours.toString();
      return '${l10n.almost} $hours ${l10n.hours}';
    } catch (e) {
      return l10n.unknown;
    }
  }

  @override
  Widget build(BuildContext context) {
    final List<String> tabs = [
      AppLocalizations.of(context)!.tripActivity,
      AppLocalizations.of(context)!.vehicle,
      AppLocalizations.of(context)!.tripDetails,
    ];
    final List<Status> statuses = [
      Status(
        name: AppLocalizations.of(context)!.all,
        code: null,
        color: CupertinoColors.activeBlue,
        icon: CupertinoIcons.infinite,
      ),
      Status(
        name: AppLocalizations.of(context)!.pending,
        code: 1,
        color: CupertinoColors.systemPurple,
        icon: CupertinoIcons.bookmark_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.scheduled,
        code: 2,
        color: CupertinoColors.systemIndigo,
        icon: CupertinoIcons.paperplane_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.inProgress,
        code: 5,
        color: CupertinoColors.activeOrange,
        icon: CupertinoIcons.slowmo,
      ),
      Status(
        name: AppLocalizations.of(context)!.completed,
        code: 6,
        color: CupertinoColors.activeGreen,
        icon: CupertinoIcons.checkmark_alt,
      ),
      Status(
        name: AppLocalizations.of(context)!.cancelled,
        code: 8,
        color: CupertinoColors.systemRed,
        icon: CupertinoIcons.xmark,
      ),
      Status(
        name: AppLocalizations.of(context)!.draft,
        code: -1,
        color: CupertinoColors.systemMint,
        icon: CupertinoIcons.doc_fill,
      ),
    ];
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go(widget.backRoute);
        }
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
        navigationBar: const AppNavBar(),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 5),
          child: ClipRRect(
            borderRadius: AppLayout.authContentBorderRadius,
            child: Container(
              padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 15),
              color: AppColors.white,
              child: BlocBuilder<TripsBloc, TripsState>(
                builder: (context, state) {
                  if (state is TripDetailsLoading) {
                    return const Center(
                      child: CupertinoActivityIndicator(
                        color: AppColors.mainBlue,
                        radius: 16,
                      ),
                    );
                  }

                  if (state is TripDetailsLoaded) {
                    if (state.error != null) {
                      return Center(
                        child: Padding(
                          padding: const EdgeInsets.all(24.0),
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              const Icon(
                                CupertinoIcons.exclamationmark_triangle,
                                size: 64,
                                color: AppColors.red,
                              ),
                              const SizedBox(height: 16),
                              Text(
                                state.error!,
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: AppColors.darkGray,
                                ),
                                textAlign: TextAlign.center,
                              ),
                              const SizedBox(height: 24),
                              Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceEvenly,
                                children: [
                                  AppSecondaryButton(
                                    onPressed: () =>
                                        context.go(widget.backRoute),
                                  ),
                                  AppPrimaryButton(
                                    label: l10n.retry,
                                    onPressed: () =>
                                        context.read<TripsBloc>().add(
                                          TripsLoadTripDetails(
                                            tripId: widget.tripId,
                                          ),
                                        ),
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      );
                    }

                    return SingleChildScrollView(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          // ── Header ─────────────────────────────────────────
                          AppSecondaryButton(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 18,
                              vertical: 8,
                            ),
                            onPressed: () => context.go(widget.backRoute),
                          ),
                          const SizedBox(height: 20),

                          // ── Shipment Info ───────────────────────────────────
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Container(
                                padding: const EdgeInsets.all(10),
                                width: 40,
                                height: 40,
                                decoration: BoxDecoration(
                                  shape: BoxShape.circle,
                                  border: Border.all(
                                    color: const Color(0xFFE5E5EA),
                                  ),
                                  color: CupertinoColors.white,
                                ),
                                child: SvgPicture.asset(
                                  'assets/images/truck.svg',
                                  fit: BoxFit.cover,
                                ),
                              ),

                              const SizedBox(width: 10),
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(
                                      Directionality.of(context) ==
                                              TextDirection.rtl
                                          ? state.trip.company!.nameAr!
                                          : state.trip.company!.nameEn!,
                                      style: const TextStyle(
                                        fontSize: 14,
                                        fontWeight: FontWeight.w400,
                                        color: CupertinoColors.systemGrey,
                                      ),
                                    ),
                                    const SizedBox(height: 2),
                                    Text(
                                      '${l10n.from} ${state.trip.waypoints!.first.addressName} ${l10n.to} ${state.trip.waypoints!.last.addressName}',
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
                                  color: statuses
                                      .firstWhere(
                                        (element) =>
                                            element.code == state.trip.status,
                                      )
                                      .color
                                      .withValues(alpha: 0.1),
                                  borderRadius: BorderRadius.circular(16),
                                ),
                                child: Text(
                                  statuses
                                      .firstWhere(
                                        (element) =>
                                            element.code == state.trip.status,
                                      )
                                      .name,
                                  style: TextStyle(
                                    fontSize: 12,
                                    fontWeight: FontWeight.w500,
                                    color: statuses
                                        .firstWhere(
                                          (element) =>
                                              element.code == state.trip.status,
                                        )
                                        .color,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 10),
                          RichText(
                            text: TextSpan(
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w400,
                                color: CupertinoColors.systemGrey,
                              ),
                              children: [
                                TextSpan(text: '${l10n.yourTripStarted}: '),
                                WidgetSpan(
                                  child: Directionality(
                                    textDirection: TextDirection.ltr,
                                    child: Text(
                                      formatDate(state.trip.startDate!),
                                      style: const TextStyle(
                                        fontSize: 14,
                                        fontWeight: FontWeight.w400,
                                        color: AppColors.darkGray,
                                      ),
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ),
                          const SizedBox(height: 10),
                          RichText(
                            text: TextSpan(
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w400,
                                color: CupertinoColors.systemGrey,
                              ),
                              children: [
                                TextSpan(text: '${l10n.estTime}: '),
                                TextSpan(
                                  text: estimatedDate(
                                    startDate: state.trip.startDate,
                                    endDate: state.trip.endDate,
                                    l10n: l10n,
                                  ),
                                  style: const TextStyle(
                                    fontSize: 14,
                                    fontWeight: FontWeight.w400,
                                    color: AppColors.darkGray,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          const SizedBox(height: 10),

                          // ── Tab Bar ─────────────────────────────────────────
                          Container(
                            decoration: const BoxDecoration(
                              border: Border(
                                bottom: BorderSide(
                                  color: Color(0xFFE5E5EA),
                                  width: 1,
                                ),
                              ),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: List.generate(tabs.length, (index) {
                                final isSelected = index == _selectedIndex;
                                return GestureDetector(
                                  onTap: () =>
                                      setState(() => _selectedIndex = index),
                                  child: AnimatedContainer(
                                    duration: const Duration(milliseconds: 300),
                                    curve: Curves.easeInOut,
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 20,
                                      vertical: 12,
                                    ),
                                    decoration: BoxDecoration(
                                      border: Border(
                                        bottom: BorderSide(
                                          color: isSelected
                                              ? AppColors.mainBlue
                                              : CupertinoColors.transparent,
                                          width: 2,
                                        ),
                                      ),
                                    ),
                                    child: Text(
                                      tabs[index],
                                      style: TextStyle(
                                        fontSize: 14,
                                        fontWeight: isSelected
                                            ? FontWeight.w600
                                            : FontWeight.w400,
                                        color: isSelected
                                            ? AppColors.mainBlue
                                            : CupertinoColors.systemGrey,
                                      ),
                                    ),
                                  ),
                                );
                              }),
                            ),
                          ),

                          // ── Tab Content ──────────────────────────────────────
                          Padding(
                            padding: const EdgeInsets.symmetric(vertical: 10),
                            child: IndexedStack(
                              index: _selectedIndex,
                              children: [
                                state.trip.recordings == null ||
                                        state.trip.recordings!.isEmpty
                                    ? Padding(
                                        padding: const EdgeInsets.only(top: 20),
                                        child: Center(
                                          child: Text(
                                            l10n.noTripActivity,
                                            style: const TextStyle(
                                              fontSize: 16,
                                              fontWeight: FontWeight.w700,
                                              color: AppColors.darkGray,
                                            ),
                                          ),
                                        ),
                                      )
                                    : ListView.builder(
                                        shrinkWrap: true,
                                        physics:
                                            const NeverScrollableScrollPhysics(),
                                        padding: const EdgeInsets.all(16),
                                        itemCount:
                                            state.trip.recordings!.length,
                                        itemBuilder: (context, index) {
                                          return RecoringItem(
                                            recording:
                                                state.trip.recordings![index],
                                            index: index,
                                            isLast:
                                                index ==
                                                state.trip.recordings!.length -
                                                    1,
                                          );
                                        },
                                      ),
                                VehicleDetails(trip: state.trip),
                                TripDetails(trip: state.trip),
                              ],
                            ),
                          ),
                        ],
                      ),
                    );
                  }

                  return const SizedBox.shrink();
                },
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class RecoringItem extends StatelessWidget {
  final Recording recording;
  final int index;
  final bool isLast;

  const RecoringItem({
    super.key,
    required this.recording,
    required this.isLast,
    required this.index,
  });

  String formatDate(String isoDate) {
    DateTime dateTime = DateTime.parse(isoDate).toLocal();

    // Get day, month, year
    String day = DateFormat('dd').format(dateTime);
    String month = DateFormat('MMM').format(dateTime);
    // Get 12-hour time
    String time = DateFormat('hh:mm').format(dateTime);
    String period = DateFormat('a').format(dateTime); // AM or PM

    return '$day $month - $time $period';
  }

  String activty(AppLocalizations l10n) {
    switch (recording.eventType) {
      case 2:
        return l10n.waypointLodaingStarted;
      case 3:
        return l10n.waypointLodaingCompleted;
      case 4:
        return l10n.tripStarted;
      case 5:
        return l10n.waypointArrived;
      case 6:
        return l10n.waypointDeparted;
      case 7:
        return l10n.tripCompleted;
      case 8:
        return l10n.waypointUnloadingStarted;
      case 9:
        return l10n.waypointUnloadingCompleted;
      default:
        return recording.eventName!.replaceAll('_', ' ').toLowerCase();
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return IntrinsicHeight(
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Left side - Circle with line
          Column(
            children: [
              _buildCircleIndicator(),
              if (!isLast) _buildVerticalLine(),
            ],
          ),

          const SizedBox(width: 12),

          // Middle - Content
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  activty(l10n),
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w500,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 6),
                Text(
                  recording.data!.waypoint!.addressName ??
                      AppLocalizations.of(context)!.noAddress,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w400,
                    color: CupertinoColors.systemGrey,
                  ),
                ),
                if (!isLast) const SizedBox(height: 20),
              ],
            ),
          ),

          const SizedBox(width: 8),

          // Right side - Date
          Directionality(
            textDirection: TextDirection.ltr,
            child: Text(
              formatDate(recording.createdAt!),
              style: const TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.w400,
                color: CupertinoColors.systemGrey,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCircleIndicator() {
    return Container(
      width: 20,
      height: 20,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: AppColors.white,
        border: Border.all(color: AppColors.mainBlue, width: 5),
      ),
    );
  }

  Widget _buildVerticalLine() {
    return Expanded(child: Container(width: 2, color: AppColors.mainBlue));
  }
}

class VehicleDetails extends StatelessWidget {
  final Trip trip;
  const VehicleDetails({super.key, required this.trip});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 10),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
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
              Text(
                Directionality.of(context) == TextDirection.rtl
                    ? trip.driver?.fullNameAr ?? l10n.noDriver
                    : trip.driver?.fullNameEn ?? l10n.noDriver,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w500,
                  color: AppColors.darkGray,
                ),
              ),
              const Spacer(),
              const SizedBox(width: 5),
              // Action Buttons
              if (trip.driver != null &&
                  trip.driver!.phone != null &&
                  trip.driver!.phone!.isNotEmpty)
                GestureDetector(
                  onTap: () async => await launchUrl(
                    Uri(scheme: 'tel', path: trip.driver!.phone),
                  ),
                  child: Container(
                    width: 35,
                    height: 35,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      border: Border.all(color: const Color(0xFFE5E5EA)),
                      color: CupertinoColors.white,
                    ),
                    child: const Icon(
                      CupertinoIcons.phone,
                      size: 15,
                      color: AppColors.darkGray,
                    ),
                  ),
                ),
              const SizedBox(width: 8),
              if (trip.driver != null &&
                  trip.driver!.phone != null &&
                  trip.driver!.phone!.isNotEmpty)
                GestureDetector(
                  onTap: () async => await launchUrl(
                    Uri(scheme: 'sms', path: trip.driver!.phone),
                  ),
                  child: Container(
                    width: 35,
                    height: 35,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      border: Border.all(color: const Color(0xFFE5E5EA)),
                      color: CupertinoColors.white,
                    ),
                    child: const Icon(
                      CupertinoIcons.chat_bubble,
                      size: 15,
                      color: AppColors.darkGray,
                    ),
                  ),
                ),
            ],
          ),
          const SizedBox(height: 10),
          Text(
            AppLocalizations.of(context)!.vehicle,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w400,
              color: CupertinoColors.systemGrey,
            ),
          ),
          const SizedBox(height: 5),
          Text(
            Directionality.of(context) == TextDirection.rtl
                ? trip.contractVehicleType!.vehicleType!.nameAr ??
                      l10n.noVehicle
                : trip.contractVehicleType!.vehicleType!.nameEn ??
                      l10n.noVehicle,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w500,
              color: AppColors.darkGray,
            ),
          ),
          trip.vehicle?.vehicleType?.id == null
              ? Padding(
                  padding: const EdgeInsets.only(top: 20),
                  child: Center(
                    child: Text(
                      l10n.noVehicle,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                        color: AppColors.darkGray,
                      ),
                    ),
                  ),
                )
              : Container(
                  margin: const EdgeInsets.symmetric(vertical: 10),
                  padding: const EdgeInsets.all(10),
                  width: double.infinity,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(12),
                    border: BoxBorder.all(color: AppColors.lightGray),
                    color: AppColors.mainGray,
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Image.asset(
                        'assets/images/vehicle.png',
                        fit: BoxFit.cover,
                        width: 50,
                        height: 25,
                      ),
                      const SizedBox(height: 10),
                      Text(
                        Directionality.of(context) == TextDirection.rtl
                            ? trip.vehicle?.vehicleType?.nameAr ??
                                  l10n.noVehicle
                            : trip.vehicle?.vehicleType?.nameEn ??
                                  l10n.noVehicle,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w700,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 5),
                      RichText(
                        text: TextSpan(
                          children: [
                            TextSpan(
                              text: '${AppLocalizations.of(context)!.head}: ',
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w700,
                                color: AppColors.darkGray,
                              ),
                            ),
                            TextSpan(
                              text:
                                  '${AppLocalizations.of(context)!.carId} - ${trip.vehicle!.plateNo} ${trip.vehicle!.plateLetters}',
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w500,
                                color: CupertinoColors.systemGrey,
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
    );
  }
}

class TripDetails extends StatelessWidget {
  final Trip trip;
  const TripDetails({super.key, required this.trip});

  String formatDate(String isoDate) {
    DateTime dateTime = DateTime.parse(isoDate).toLocal();

    // Get day, month, year
    String day = DateFormat('dd').format(dateTime);
    String month = DateFormat('MMMM').format(dateTime);
    String year = DateFormat('yyyy').format(dateTime);

    // Get 12-hour time
    String time = DateFormat('hh:mm').format(dateTime);
    String period = DateFormat('a').format(dateTime); // AM or PM

    return '$day $month $year - $time $period';
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(12),
        border: BoxBorder.all(color: AppColors.lightGray),
      ),
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Label(label: l10n.tripInformation, isTitle: true),
            const SizedBox(height: 20),
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(12),
                border: BoxBorder.all(color: AppColors.lightGray),
                color: AppColors.mainGray,
              ),
              child: Column(
                children: [
                  Row(
                    children: [
                      Label(label: '${l10n.companyName}:'),
                      const SizedBox(width: 10),
                      Flexible(
                        child: Label(
                          label: Directionality.of(context) == TextDirection.rtl
                              ? trip.company!.nameAr!
                              : trip.company!.nameEn!,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Label(label: '${l10n.contactName} :'),
                      const SizedBox(width: 10),
                      Flexible(child: Label(label: trip.contactPersonName!)),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Label(label: '${l10n.contactNumber} :'),
                      const SizedBox(width: 10),
                      Flexible(
                        child: Directionality(
                          textDirection: TextDirection.ltr,
                          child: Label(label: trip.contactPersonMobile!),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Label(label: '${l10n.contractRefNumber} :'),
                      const SizedBox(width: 10),
                      Flexible(
                        child: Label(label: trip.contract!.referenceNumber!),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  if ((trip.contractVehicleType?.vehicleType?.nameAr != null &&
                          trip.contractVehicleType?.vehicleType?.nameEn !=
                              null) ||
                      trip.contractVehicleType?.vehicleType?.name != null) ...[
                    Row(
                      children: [
                        Label(label: '${l10n.vehicleType} :'),
                        const SizedBox(width: 10),
                        Flexible(
                          child: Label(
                            label:
                                Directionality.of(context) == TextDirection.rtl
                                ? trip
                                          .contractVehicleType!
                                          .vehicleType!
                                          .nameAr ??
                                      trip
                                          .contractVehicleType!
                                          .vehicleType!
                                          .name!
                                : trip
                                          .contractVehicleType!
                                          .vehicleType!
                                          .nameEn ??
                                      trip
                                          .contractVehicleType!
                                          .vehicleType!
                                          .name!,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 10),
                  ],
                  Row(
                    children: [
                      Label(label: '${l10n.distance} :'),
                      const SizedBox(width: 10),
                      Flexible(
                        child: Label(label: '${trip.contractDistanceKm}'),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Label(label: '${l10n.startDate} :'),
                      const SizedBox(width: 10),
                      Flexible(
                        child: Directionality(
                          textDirection: TextDirection.ltr,
                          child: Label(label: formatDate(trip.startDate!)),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Label(label: '${l10n.endDate} :'),
                      const SizedBox(width: 10),
                      Flexible(
                        child: Directionality(
                          textDirection: TextDirection.ltr,
                          child: Label(label: formatDate(trip.endDate!)),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  if (trip.helpersCount != null) ...[
                    Row(
                      children: [
                        Label(label: '${l10n.helpersNumber} :'),
                        const SizedBox(width: 10),
                        Flexible(child: Label(label: '${trip.helpersCount}')),
                      ],
                    ),
                    const SizedBox(height: 10),
                  ],
                ],
              ),
            ),
            const SizedBox(height: 20),
            Label(label: l10n.pickDropPoints, isTitle: true),
            const SizedBox(height: 20),
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(12),
                border: BoxBorder.all(color: AppColors.lightGray),
                color: AppColors.mainGray,
              ),
              child: ListView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                padding: const EdgeInsets.all(16),
                itemCount: trip.waypoints!.length,
                itemBuilder: (context, index) {
                  return TrackingPointItem(
                    waypoint: trip.waypoints![index],
                    index: index,
                    isLast: index == trip.waypoints!.length - 1,
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}
