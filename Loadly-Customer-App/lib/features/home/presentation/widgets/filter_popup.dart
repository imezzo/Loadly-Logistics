import 'package:flutter/cupertino.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_checkbox.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';

// ignore: must_be_immutable
class FilterPopup extends StatefulWidget {
  int? tripsStatisticsYear;
  int? monthlyTripsKilometersYear;
  int? month;
  bool lastMonth;
  bool lastWeek;
  bool isTripsStatistics;
  final void Function(int?, int?, int?, bool, bool) fliter;
  FilterPopup({
    super.key,
    this.tripsStatisticsYear,
    this.monthlyTripsKilometersYear,
    this.month,
    required this.lastMonth,
    required this.lastWeek,
    required this.isTripsStatistics,
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
                    if (widget.isTripsStatistics) ...[
                      Text(
                        l10n.year,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 12),
                      YearPicker(
                        year: widget.tripsStatisticsYear,
                        onDateChanged: (newYear) => setState(() {
                          widget.tripsStatisticsYear = newYear;
                          widget.lastMonth = false;
                          widget.lastWeek = false;
                        }),
                      ),
                      const SizedBox(height: 15),
                      AppCheckbox(
                        label: l10n.lastMonth,
                        checked: widget.lastMonth,
                        onTap: () => setState(() {
                          widget.lastMonth = !widget.lastMonth;
                          widget.lastWeek = false;
                          widget.tripsStatisticsYear = null;
                        }),
                      ),
                      const SizedBox(height: 15),
                      AppCheckbox(
                        label: l10n.lastWeek,
                        checked: widget.lastWeek,
                        onTap: () => setState(() {
                          widget.lastWeek = !widget.lastWeek;
                          widget.lastMonth = false;
                          widget.tripsStatisticsYear = null;
                        }),
                      ),
                    ],
                    if (!widget.isTripsStatistics) ...[
                      Text(
                        l10n.year,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 12),
                      YearPicker(
                        year: widget.monthlyTripsKilometersYear,
                        onDateChanged: (newYear) => setState(
                          () => widget.monthlyTripsKilometersYear = newYear,
                        ),
                      ),
                      const SizedBox(height: 12),
                      Text(
                        l10n.month,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 12),
                      MonthPicker(
                        month: widget.month,
                        onDateChanged: (newMonth) =>
                            setState(() => widget.month = newMonth),
                      ),
                    ],
                    const SizedBox(height: 12),
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
                            if (widget.isTripsStatistics) {
                              widget.tripsStatisticsYear = null;
                              widget.lastMonth = false;
                              widget.lastWeek = false;
                            } else {
                              widget.monthlyTripsKilometersYear = null;
                              widget.month = null;
                            }
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
                            widget.tripsStatisticsYear,
                            widget.monthlyTripsKilometersYear,
                            widget.month,
                            widget.lastMonth,
                            widget.lastWeek,
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
}

// ignore: must_be_immutable
class YearPicker extends StatelessWidget {
  int? year;
  Function(int) onDateChanged;
  YearPicker({super.key, this.year, required this.onDateChanged});

  @override
  Widget build(BuildContext context) {
    final List<int> years = List.generate(182, (i) => 2019 + i); // 2010 – 2200

    return GestureDetector(
      onTap: () => showCupertinoModalPopup(
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
                        if (year == null) {
                          onDateChanged(DateTime.now().year);
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
                  itemExtent: 40,
                  scrollController: FixedExtentScrollController(
                    initialItem: years.indexOf(year ?? DateTime.now().year),
                  ),
                  onSelectedItemChanged: (int index) {
                    year = years[index];
                    onDateChanged(years[index]);
                  },
                  children: years
                      .map(
                        (y) => Center(
                          child: Text(
                            '$y',
                            style: const TextStyle(fontSize: 20),
                          ),
                        ),
                      )
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
              year == null ? AppLocalizations.of(context)!.year : '$year',
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
}

// ignore: must_be_immutable
class MonthPicker extends StatelessWidget {
  int? month;
  Function(int) onDateChanged;
  MonthPicker({super.key, this.month, required this.onDateChanged});

  @override
  Widget build(BuildContext context) {
    const List<String> months = [
      'January',
      'February',
      'March',
      'April',
      'May',
      'June',
      'July',
      'August',
      'September',
      'October',
      'November',
      'December',
    ];

    return GestureDetector(
      onTap: () => showCupertinoModalPopup(
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
                        if (month == null) {
                          onDateChanged(DateTime.now().month);
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
                  itemExtent: 40,
                  scrollController: FixedExtentScrollController(
                    initialItem: month != null
                        ? month! - 1
                        : DateTime.now().month - 1,
                  ),
                  onSelectedItemChanged: (int index) {
                    month = index + 1;
                    onDateChanged(month!);
                  },
                  children: months
                      .map(
                        (m) => Center(
                          child: Text(m, style: const TextStyle(fontSize: 20)),
                        ),
                      )
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
              month == null
                  ? AppLocalizations.of(context)!.month
                  : months[month! - 1],
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
}
