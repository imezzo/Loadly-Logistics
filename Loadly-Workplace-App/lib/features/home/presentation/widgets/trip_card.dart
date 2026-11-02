import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/presentation/bloc/trips_bloc.dart';
import '../../../trips/presentation/bloc/trips_event.dart';
import '../../domain/entities/collector_trips.dart';
import '../views/home_screen.dart';

class TripCard extends StatefulWidget {
  final CollectorTrips trip;
  final bool isFinancialCollector;
  final BulkSelect bulkSelect;
  final VoidCallback onBulkChanged;
  const TripCard({
    super.key,
    required this.trip,
    required this.bulkSelect,
    required this.onBulkChanged,
    required this.isFinancialCollector,
  });

  @override
  State<TripCard> createState() => _TripCardState();
}

class _TripCardState extends State<TripCard> {
  bool isSelected = false;

  String formatDateTime(String isoDate) {
    final dateTime = DateTime.parse(isoDate);
    return DateFormat(
      'dd-MMM-yyyy - h:mm a',
      AppLocalizations.of(context)!.localeName,
    ).format(dateTime.toLocal());
  }

  Future<void> showSendDocumentsDialog(
    BuildContext context, {
    required AppLocalizations l10n,
  }) {
    return showCupertinoModalPopup(
      context: context,
      builder: (context) => Container(
        decoration: const BoxDecoration(
          color: Color(0xFFF7F7F7),
          borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
        ),
        padding: const EdgeInsets.fromLTRB(20, 12, 20, 32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Drag handle
            Container(
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: const Color(0xFFDDDDDD),
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            const SizedBox(height: 24),

            // Green checkmark circle
            Container(
              width: 90,
              height: 90,
              decoration: const BoxDecoration(
                color: Color(0xFFF0F0F0),
                shape: BoxShape.circle,
              ),
              child: Center(
                child: Container(
                  width: 64,
                  height: 64,
                  decoration: const BoxDecoration(
                    color: Color(0xFF1E6B3C),
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    CupertinoIcons.checkmark_alt,
                    color: CupertinoColors.white,
                    size: 34,
                  ),
                ),
              ),
            ),
            const SizedBox(height: 20),

            // Title
            Text(
              l10n.receiptConfirmation,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w700,
                color: AppColors.darkGray,
              ),
            ),
            const SizedBox(height: 10),

            // Subtitle
            Text(
              l10n.receiptConfirmedDesc,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: AppColors.darkGray,
              ),
            ),
            const SizedBox(height: 20),

            // Documents list section
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: Text(
                l10n.documentsCollectedByFactoryCollector,
                style: const TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w700,
                  color: AppColors.darkGray,
                ),
              ),
            ),
            const SizedBox(height: 12),
            // Document items
            Builder(
              builder: (context) {
                final docs = widget.trip.collectorDocuments ?? [];
                final selected = docs.where((e) => e.status == 1).toList();
                if (selected.isEmpty) {
                  return Padding(
                    padding: const EdgeInsets.symmetric(vertical: 12),
                    child: Text(
                      l10n.noDocumentsAvailable,
                      textAlign: TextAlign.center,
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w400,
                        color: AppColors.darkGray,
                      ),
                    ),
                  );
                }

                return Column(
                  children: selected.map((document) {
                    final name = Directionality.of(context) == TextDirection.rtl
                        ? (document.nameAr ?? document.nameEn ?? '')
                        : (document.nameEn ?? document.nameAr ?? '');
                    return Padding(
                      padding: const EdgeInsets.symmetric(vertical: 6),
                      child: Row(
                        children: [
                          Container(
                            width: 26,
                            height: 26,
                            decoration: const BoxDecoration(
                              color: AppColors.darkGray,
                              shape: BoxShape.circle,
                            ),
                            child: const Icon(
                              CupertinoIcons.checkmark_alt,
                              color: CupertinoColors.white,
                              size: 14,
                            ),
                          ),
                          const SizedBox(width: 10),
                          Flexible(
                            child: Text(
                              name.isNotEmpty
                                  ? name
                                  : l10n.noDocumentsAvailable,
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w400,
                                color: AppColors.darkGray,
                              ),
                            ),
                          ),
                        ],
                      ),
                    );
                  }).toList(),
                );
              },
            ),

            const SizedBox(height: 24),

            // Send button
            SizedBox(
              width: double.infinity,
              child: AppPrimaryButton(
                onPressed: () {
                  final tripId = widget.trip.id;
                  Navigator.of(context).pop();
                  if (tripId != null) {
                    context.read<TripsBloc>().add(
                      TripsBulkConfirmDocuments(tripIds: [tripId]),
                    );
                  }
                },
                label: l10n.receiptConfirmation,
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return GestureDetector(
      onTap: () => context.go('/trips/${widget.trip.id}', extra: widget.trip),
      child: Container(
        decoration: BoxDecoration(
          color: const Color(0xFFE5E9EF),
          borderRadius: BorderRadius.circular(20),
        ),
        padding: const EdgeInsets.all(10).copyWith(top: 5),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Header: Trip number + Driver
            _buildHeader(context),
            // const SizedBox(height: 10),
            // Main content card
            _buildContentCard(context),
            const SizedBox(height: 10),
            if (widget.trip.tripDocumentStatus != null &&
                widget.trip.tripDocumentStatus == 2 &&
                !widget.isFinancialCollector) ...[
              Text(
                AppLocalizations.of(context)!.partiallyCollectedDocuments,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w700,
                  color: CupertinoColors.activeOrange,
                ),
              ),
              const SizedBox(height: 10),
            ],
            if (widget.isFinancialCollector) ...[
              Row(
                children: [
                  Icon(
                    widget.trip.tripDocumentStatus == 3
                        ? CupertinoIcons.doc_checkmark_fill
                        : CupertinoIcons.doc_fill,
                  ),
                  const SizedBox(width: 5),
                  Text(
                    widget.trip.tripDocumentStatus == 1
                        ? l10n.noDocumentsCollected
                        : widget.trip.tripDocumentStatus == 2
                        ? l10n.partiallyCollectedDocuments
                        : widget.trip.tripDocumentStatus == 3
                        ? l10n.documentsCollectedFromFactoryCollector
                        : l10n.partiallyConfirmedDocuments,
                    style: TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w400,
                      color: widget.trip.tripDocumentStatus == 1
                          ? AppColors.red
                          : widget.trip.tripDocumentStatus == 2 ||
                                widget.trip.tripDocumentStatus == 4
                          ? CupertinoColors.activeOrange
                          : AppColors.green,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 10),
            ],
            // CTA Button
            _buildReceiveButton(context),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        // Trip date/time
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 5, vertical: 10),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              Text(
                '${AppLocalizations.of(context)!.tripDateIs} :',
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w400,
                  color: AppColors.darkGray,
                ),
              ),
              const SizedBox(width: 5),
              Text(
                formatDateTime(widget.trip.startDate!),
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w400,
                  color: AppColors.darkGray,
                ),
              ),
            ],
          ),
        ),

        // TODO: remove this checkbox if not needed
        // CupertinoCheckbox(
        //   value: isSelected,
        //   activeColor: AppColors.mainBlue,
        //   onChanged: (value) => setState(() {
        //     isSelected = value!;
        //     if (value == true) {
        //       widget.bulkSelect.bulkItems.add(
        //         BulkItem(
        //           tripId: widget.trip.id!,
        //           tripCompany: widget.trip.company == null
        //               ? AppLocalizations.of(context)!.externalTrip
        //               : Directionality.of(context) == TextDirection.rtl
        //               ? widget.trip.company!.nameAr ??
        //                     widget.trip.company!.fullName!
        //               : widget.trip.company!.nameEn ??
        //                     widget.trip.company!.fullName!,
        //         ),
        //       );
        //     } else {
        //       widget.bulkSelect.bulkItems.removeWhere(
        //         (element) => element.tripId == widget.trip.id!,
        //       );
        //     }
        //     widget.onBulkChanged();
        //   }),
        // ),
      ],
    );
  }

  Widget _buildDriverAvatar() {
    return const Icon(
      CupertinoIcons.person_crop_circle_fill,
      color: AppColors.lightGray,
      size: 30,
    );
  }

  Widget _buildContentCard(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: CupertinoColors.white,
        borderRadius: BorderRadius.circular(10),
        boxShadow: [
          const BoxShadow(
            color: Color(0x0F000000),
            blurRadius: 8,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          CupertinoListTile(
            padding: EdgeInsets.zero,
            leading: _buildDriverAvatar(),
            title: Text(
              Directionality.of(context) == TextDirection.rtl
                  ? widget.trip.driver?.fullNameAr ??
                        widget.trip.driver?.fullName ??
                        l10n.noDriver
                  : widget.trip.driver?.fullNameEn ??
                        widget.trip.driver?.fullName ??
                        l10n.noDriver,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: CupertinoColors.systemGrey,
              ),
              overflow: TextOverflow.ellipsis,
            ),
            subtitle: Text(
              '${widget.trip.vehicle?.plateLetters ?? ''} ${widget.trip.vehicle?.plateNo ?? ''}',
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: AppColors.darkGray,
              ),
            ),
            trailing: Container(
              padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 8),
              decoration: BoxDecoration(
                color: AppColors.lightGray,
                borderRadius: BorderRadius.circular(30),
              ),
              child: Text(
                '${AppLocalizations.of(context)!.trip}: ${widget.trip.id}',
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w400,
                  color: AppColors.mainBlue,
                ),
              ),
            ),
          ),

          const SizedBox(height: 10),

          // Company name with icon
          Row(
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              // Medicine bottle icon placeholder
              const Icon(
                CupertinoIcons.square_favorites_fill,
                color: Color(0xFFE5E9EF),
              ),
              const SizedBox(width: 5),
              SizedBox(
                width: MediaQuery.sizeOf(context).width * 0.6,
                child: Text(
                  widget.trip.company == null
                      ? AppLocalizations.of(context)!.externalTrip
                      : Directionality.of(context) == TextDirection.rtl
                      ? widget.trip.company?.nameAr ??
                            widget.trip.company!.fullName!
                      : widget.trip.company?.nameEn ??
                            widget.trip.company!.fullName!,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w400,
                    color: AppColors.darkGray,
                  ),
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),

          //
          const SizedBox(height: 10),

          // Route: Origin ← Destination
          if (widget.trip.waypoints != null &&
              widget.trip.waypoints!.isNotEmpty)
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                // Destination (left side in RTL = arrival)
                _buildLocationBlock(
                  name: widget.trip.waypoints!.first.addressName!,
                  date: formatDateTime(widget.trip.startDate!),
                  context: context,
                ),
                // Arrow
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 8),
                  child: Icon(
                    Directionality.of(context) == TextDirection.rtl
                        ? CupertinoIcons.arrow_left
                        : CupertinoIcons.arrow_right,
                    color: AppColors.darkGray,
                    size: 22,
                  ),
                ),
                // Origin (right side in RTL = departure)
                _buildLocationBlock(
                  name: widget.trip.waypoints!.last.addressName!,
                  date: formatDateTime(widget.trip.endDate!),
                  context: context,
                ),
              ],
            ),
        ],
      ),
    );
  }

  Widget _buildLocationBlock({
    required String name,
    required String date,
    required BuildContext context,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.start,
          children: [
            const Icon(
              CupertinoIcons.location_solid,
              size: 14,
              fontWeight: FontWeight.w400,
              color: CupertinoColors.systemGrey,
            ),
            const SizedBox(width: 4),
            SizedBox(
              width: MediaQuery.sizeOf(context).width * 0.25,
              child: Text(
                name,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w400,
                  color: AppColors.darkGray,
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: 4),
        Align(
          alignment: AlignmentDirectional.centerStart,
          child: Text(
            date,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w400,
              color: CupertinoColors.systemGrey,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildReceiveButton(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return SizedBox(
      width: double.infinity,
      child: AppPrimaryButton(
        padding: const EdgeInsets.symmetric(vertical: 10),
        onPressed: () =>
            context.go('/trips/${widget.trip.id}', extra: widget.trip),
        label:
            widget.trip.tripDocumentStatus == 3 ||
                widget.trip.tripDocumentStatus == 4
            ? l10n.displayDocuments
            : l10n.collectDocuments,
      ),
    );
  }
}
