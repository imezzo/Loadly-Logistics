import 'package:flutter/cupertino.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/theme/app_colors.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/trip.dart';
import '../views/add_trip_screen.dart';

class AddTripReview extends StatefulWidget {
  final FormData formData;
  const AddTripReview({super.key, required this.formData});

  @override
  State<AddTripReview> createState() => _AddTripReviewState();
}

class _AddTripReviewState extends State<AddTripReview>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

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
    super.build(context);
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
            Center(child: Label(label: l10n.reviewSubmit, isTitle: true)),
            const SizedBox(height: 20),
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
                      Label(label: '${l10n.contactName} :'),
                      const SizedBox(width: 10),
                      Flexible(
                        child: Label(label: widget.formData.contactPersonName!),
                      ),
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
                          child: Label(
                            label: widget.formData.contactPersonMobile!,
                          ),
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
                        child: Label(label: widget.formData.contractRef!),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Label(label: '${l10n.distance} :'),
                      const SizedBox(width: 10),
                      Flexible(
                        child: Label(
                          label: '${widget.formData.contractDistanceKm}',
                        ),
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
                          child: Label(
                            label: formatDate(widget.formData.startDate!),
                          ),
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
                          child: Label(
                            label: formatDate(widget.formData.endDate!),
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  if (widget.formData.actualCargoWeightTons != null) ...[
                    Row(
                      children: [
                        Label(label: '${l10n.actualCargoWeightTons} :'),
                        const SizedBox(width: 10),
                        Flexible(
                          child: Label(
                            label: '${widget.formData.actualCargoWeightTons}',
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 10),
                  ],
                  if (widget.formData.cargoPrice != null) ...[
                    Row(
                      children: [
                        Label(label: '${l10n.cargoPrice} :'),
                        const SizedBox(width: 10),
                        Flexible(
                          child: Label(label: '${widget.formData.cargoPrice}'),
                        ),
                      ],
                    ),
                    const SizedBox(height: 10),
                  ],
                  if (widget.formData.helpersNumber != null) ...[
                    Row(
                      children: [
                        Label(label: '${l10n.helpersNumber} :'),
                        const SizedBox(width: 10),
                        Flexible(
                          child: Label(
                            label: '${widget.formData.helpersNumber}',
                          ),
                        ),
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
                itemCount: widget.formData.waypoints.length,
                itemBuilder: (context, index) {
                  return TrackingPointItem(
                    waypoint: widget.formData.waypoints[index],
                    index: index,
                    isLast: index == widget.formData.waypoints.length - 1,
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

class Label extends StatelessWidget {
  final String label;
  final bool isTitle;
  const Label({super.key, required this.label, this.isTitle = false});

  @override
  Widget build(BuildContext context) {
    return Text(
      label,
      style: TextStyle(
        fontSize: isTitle ? 18 : 16,
        fontWeight: FontWeight.w500,
        color: AppColors.darkGray,
      ),
    );
  }
}

class TrackingPointItem extends StatelessWidget {
  final Waypoint waypoint;
  final int index;
  final bool isLast;

  const TrackingPointItem({
    super.key,
    required this.waypoint,
    required this.isLast,
    required this.index,
  });

  @override
  Widget build(BuildContext context) {
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
                  '${index + 1}-  ${waypoint.stopType == 0 ? AppLocalizations.of(context)!.pickupPoint : AppLocalizations.of(context)!.dropoffPoint}',
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w500,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 6),
                Row(
                  children: [
                    const Icon(
                      CupertinoIcons.location_solid,
                      color: AppColors.darkGray,
                      size: 18,
                    ),
                    const SizedBox(width: 5),
                    Flexible(
                      child: Text(
                        waypoint.addressName!,
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w500,
                          color: CupertinoColors.systemGrey,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 6),
                RichText(
                  text: TextSpan(
                    children: [
                      TextSpan(
                        text: '${AppLocalizations.of(context)!.note} : ',
                        style: const TextStyle(
                          fontSize: 14,
                          color: CupertinoColors.systemGrey,
                          fontWeight: FontWeight.w400,
                        ),
                      ),
                      TextSpan(
                        text:
                            waypoint.taskDetails ??
                            AppLocalizations.of(context)!.noNote,
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w400,
                          color: CupertinoColors.systemGrey,
                        ),
                      ),
                    ],
                  ),
                ),
                if (!isLast) const SizedBox(height: 20),
              ],
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
