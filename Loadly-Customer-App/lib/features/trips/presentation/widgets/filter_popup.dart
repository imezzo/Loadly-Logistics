// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:flutter/cupertino.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/lookup.dart';

// ignore: must_be_immutable
class FilterPopup extends StatefulWidget {
  DateTime? startDate;
  DateTime? endDate;
  int? status;
  List<Lookup> vehicleTypes;
  int? vehicleTypeId;
  final void Function(String?, String?, int?, int?) fliter;
  FilterPopup({
    super.key,
    required this.startDate,
    required this.endDate,
    required this.status,
    required this.vehicleTypeId,
    required this.vehicleTypes,
    required this.fliter,
  });

  @override
  State<FilterPopup> createState() => _FilterPopupState();
}

class _FilterPopupState extends State<FilterPopup> {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return CupertinoPageScaffold(
      backgroundColor: AppColors.darkGray.withValues(alpha: 0.4),
      child: Center(
        child: Container(
          margin: const EdgeInsets.symmetric(horizontal: 20),
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(12),
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              // Header
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 16),
                decoration: const BoxDecoration(
                  color: AppColors.mainGray,
                  borderRadius: BorderRadius.only(
                    topLeft: Radius.circular(12),
                    topRight: Radius.circular(12),
                  ),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      l10n.filter,
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                        color: AppColors.darkGray,
                      ),
                    ),
                    CupertinoButton(
                      padding: EdgeInsets.zero,
                      onPressed: () => Navigator.pop(context),
                      child: const Icon(
                        CupertinoIcons.xmark,
                        size: 15,
                        color: AppColors.darkGray,
                      ),
                    ),
                  ],
                ),
              ),

              // Content
              Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Date Range
                    Text(
                      l10n.dateRange,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                        color: AppColors.darkGray,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: _buildDateField(
                            context,
                            widget.startDate,
                            true,
                            (newDate) =>
                                setState(() => widget.startDate = newDate),
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: _buildDateField(
                            context,
                            widget.endDate,
                            false,
                            (newDate) =>
                                setState(() => widget.endDate = newDate),
                          ),
                        ),
                      ],
                    ),

                    const SizedBox(height: 24),

                    // Trip Status
                    Text(
                      l10n.tripStatus,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                        color: AppColors.darkGray,
                      ),
                    ),
                    const SizedBox(height: 12),

                    StatusField(
                      status: widget.status,
                      onChanged: (status) =>
                          setState(() => widget.status = status),
                    ),
                    const SizedBox(height: 24),

                    // Vehicle Type
                    Text(
                      l10n.vehicleType,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                        color: AppColors.darkGray,
                      ),
                    ),
                    const SizedBox(height: 12),
                    VehicleTypeField(
                      vehicleTypes: widget.vehicleTypes,
                      onChanged: (id) => setState(() {
                        widget.vehicleTypeId = id;
                      }),
                      isLoading: false,
                      vehicleTypeId: widget.vehicleTypeId,
                    ),
                  ],
                ),
              ),

              // Footer Buttons
              Container(
                padding: const EdgeInsets.all(16),
                decoration: const BoxDecoration(
                  border: Border(
                    top: BorderSide(color: AppColors.lightGray, width: 0.5),
                  ),
                ),
                child: Row(
                  children: [
                    Expanded(
                      child: AppSecondaryButton(
                        onPressed: () {
                          setState(() {
                            widget.startDate = null;
                            widget.endDate = null;
                            widget.status = null;
                            widget.vehicleTypeId = null;
                          });
                        },
                        label: l10n.reset,
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: AppPrimaryButton(
                        onPressed: () {
                          widget.fliter(
                            widget.startDate?.toUtc().toIso8601String(),
                            widget.endDate?.toUtc().toIso8601String(),
                            widget.status,
                            widget.vehicleTypeId,
                          );
                          Navigator.pop(context);
                        },
                        label: l10n.applyNow,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildDateField(
    BuildContext context,
    DateTime? date,
    bool isStart,
    Function(DateTime) onDateChanged,
  ) {
    return GestureDetector(
      onTap: () => _showDatePicker(context, date, onDateChanged, isStart),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
        decoration: BoxDecoration(
          border: Border.all(color: AppColors.lightGray),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              date == null
                  ? isStart
                        ? AppLocalizations.of(context)!.startDate
                        : AppLocalizations.of(context)!.endDate
                  : '${date.day.toString().padLeft(2, '0')} ${_getMonthName(date.month)} ${date.year.toString().substring(2)}',
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w400,
                color: AppColors.darkGray,
              ),
            ),
            const Icon(
              CupertinoIcons.calendar,
              size: 20,
              color: AppColors.darkGray,
            ),
          ],
        ),
      ),
    );
  }

  void _showDatePicker(
    BuildContext context,
    DateTime? date,
    Function(DateTime) onDateChanged,
    bool isStart,
  ) {
    showCupertinoModalPopup(
      context: context,
      builder: (BuildContext context) => Container(
        height: 250,
        color: CupertinoColors.systemBackground.resolveFrom(context),
        child: Column(
          children: [
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              height: 44,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  CupertinoButton(
                    padding: EdgeInsets.zero,
                    onPressed: () {
                      if (date == null) {
                        onDateChanged(DateTime.now());
                      }
                      Navigator.pop(context);
                    },
                    child: Text(AppLocalizations.of(context)!.done),
                  ),
                ],
              ),
            ),
            Expanded(
              child: CupertinoDatePicker(
                mode: CupertinoDatePickerMode.date,
                initialDateTime: isStart
                    ? widget.startDate ?? DateTime.now()
                    : widget.endDate ?? DateTime.now(),
                onDateTimeChanged: (dateTime) {
                  date = dateTime;
                  onDateChanged(dateTime);
                },
              ),
            ),
          ],
        ),
      ),
    );
  }

  String _getMonthName(int month) {
    const months = [
      'Jan',
      'Feb',
      'Mar',
      'Apr',
      'May',
      'Jun',
      'Jul',
      'Aug',
      'Sep',
      'Oct',
      'Nov',
      'Dec',
    ];
    return months[month - 1];
  }
}

class Status {
  String name;
  int? code;
  Color color;
  IconData icon;

  Status({
    required this.name,
    required this.code,
    required this.color,
    required this.icon,
  });
}

// ignore: must_be_immutable
class StatusField extends StatelessWidget {
  int? status;
  final Function(int?) onChanged;
  StatusField({super.key, required this.status, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    final List<Status> statuses = [
      Status(
        name: AppLocalizations.of(context)!.all,
        code: null,
        color: CupertinoColors.activeBlue,
        icon: CupertinoIcons.infinite,
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
    final FixedExtentScrollController controller = FixedExtentScrollController(
      initialItem: statuses.indexOf(
        statuses.firstWhere((statuses) => statuses.code == status),
      ),
    );
    final statusItem = statuses.firstWhere(
      (statuses) => statuses.code == status,
    );
    return GestureDetector(
      onTap: () => showCupertinoModalPopup(
        context: context,
        builder: (BuildContext context) => Container(
          height: 250,
          decoration: const BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.only(
              topLeft: Radius.circular(12),
              topRight: Radius.circular(12),
            ),
          ),
          child: Column(
            children: [
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 16),
                height: 44,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    CupertinoButton(
                      padding: EdgeInsets.zero,
                      onPressed: () => Navigator.pop(context),
                      child: Text(AppLocalizations.of(context)!.done),
                    ),
                  ],
                ),
              ),
              Expanded(
                child: CupertinoPicker(
                  scrollController: controller,
                  itemExtent: 40,
                  onSelectedItemChanged: (index) =>
                      onChanged(statuses[index].code),
                  children: statuses
                      .map((status) => Center(child: Text(status.name)))
                      .toList(),
                ),
              ),
            ],
          ),
        ),
      ),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
        decoration: BoxDecoration(
          border: Border.all(color: AppColors.lightGray),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Row(
              children: [
                Icon(statusItem.icon, size: 18, color: statusItem.color),
                const SizedBox(width: 8),
                Text(
                  statusItem.name,
                  style: TextStyle(fontSize: 15, color: statusItem.color),
                ),
              ],
            ),
            const Icon(
              CupertinoIcons.chevron_down,
              size: 18,
              color: AppColors.darkGray,
            ),
          ],
        ),
      ),
    );
  }
}

// ignore: must_be_immutable
class VehicleTypeField extends StatelessWidget {
  final List<Lookup> vehicleTypes;
  int? vehicleTypeId;
  final Function(int) onChanged;
  final bool isLoading;
  VehicleTypeField({
    super.key,
    required this.vehicleTypes,
    required this.onChanged,
    required this.isLoading,
    required this.vehicleTypeId,
  });

  @override
  Widget build(BuildContext context) {
    final FixedExtentScrollController controller = FixedExtentScrollController(
      initialItem: vehicleTypeId == null || vehicleTypes.isEmpty
          ? 0
          : vehicleTypes.indexOf(
              vehicleTypes.firstWhere(
                (vehicleType) => vehicleType.id == vehicleTypeId,
              ),
            ),
    );
    return GestureDetector(
      onTap: () => showCupertinoModalPopup(
        context: context,
        builder: (BuildContext context) => Container(
          height: 250,
          decoration: const BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.only(
              topLeft: Radius.circular(12),
              topRight: Radius.circular(12),
            ),
          ),
          child: Column(
            children: [
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 16),
                height: 44,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    CupertinoButton(
                      padding: EdgeInsets.zero,
                      onPressed: () {
                        if (vehicleTypeId == null && vehicleTypes.isNotEmpty) {
                          onChanged(vehicleTypes[0].id!);
                        }
                        Navigator.pop(context);
                      },
                      child: Text(AppLocalizations.of(context)!.done),
                    ),
                  ],
                ),
              ),
              Expanded(
                child: CupertinoPicker(
                  scrollController: controller,
                  itemExtent: 40,
                  onSelectedItemChanged: (index) {
                    vehicleTypeId = vehicleTypes[index].id!;
                    onChanged(vehicleTypes[index].id!);
                  },
                  children: vehicleTypes
                      .map((vehicle) => Center(child: Text(vehicle.value!)))
                      .toList(),
                ),
              ),
            ],
          ),
        ),
      ),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
        decoration: BoxDecoration(
          border: Border.all(color: AppColors.lightGray),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              vehicleTypeId == null
                  ? AppLocalizations.of(context)!.all
                  : vehicleTypes
                        .firstWhere((element) => element.id == vehicleTypeId)
                        .value!,
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w400,
                color: AppColors.darkGray,
              ),
            ),
            isLoading
                ? const CupertinoActivityIndicator(
                    color: AppColors.mainBlue,
                    radius: 10,
                  )
                : const Icon(
                    CupertinoIcons.chevron_down,
                    size: 18,
                    color: AppColors.darkGray,
                  ),
          ],
        ),
      ),
    );
  }
}
