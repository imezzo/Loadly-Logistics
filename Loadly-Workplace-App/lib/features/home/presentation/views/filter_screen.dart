// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/extensions/date_time.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import 'home_screen.dart';

class FilterScreen extends StatefulWidget {
  final FilterData filterData;
  final bool isFinancialCollector;
  const FilterScreen({
    super.key,
    required this.filterData,
    required this.isFinancialCollector,
  });

  @override
  State<FilterScreen> createState() => _FilterScreenState();
}

class _FilterScreenState extends State<FilterScreen> {
  final TextEditingController companyNameController = TextEditingController();
  final TextEditingController driverNameController = TextEditingController();
  final TextEditingController plateController = TextEditingController();
  DateTime? startDate;
  DateTime? endDate;
  int? dateRadio;
  int? tripDocumentStatus;

  @override
  void initState() {
    if (widget.filterData.companyName != null) {
      companyNameController.text = widget.filterData.companyName!;
    }
    if (widget.filterData.driverName != null) {
      driverNameController.text = widget.filterData.driverName!;
    }
    if (widget.filterData.plate != null) {
      plateController.text = widget.filterData.plate!;
    }
    if (widget.filterData.startDate != null &&
        widget.filterData.dateRadio == 4) {
      startDate = DateTime.tryParse(widget.filterData.startDate!);
    }
    if (widget.filterData.endDate != null && widget.filterData.dateRadio == 4) {
      endDate = DateTime.tryParse(widget.filterData.endDate!);
    }
    dateRadio = widget.filterData.dateRadio;

    tripDocumentStatus = widget.filterData.tripDocumentStatuses?.length == 1
        ? widget.filterData.tripDocumentStatuses![0]
        : null;

    super.initState();
  }

  @override
  void dispose() {
    companyNameController.dispose();
    driverNameController.dispose();
    plateController.dispose();
    super.dispose();
  }

  void onStatusChanged(int? value) {
    setState(() {
      tripDocumentStatus = value;
    });
  }

  void onDateRadioChanged(int? value) {
    setState(() {
      dateRadio = value;
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) context.go('/home');
      },
      child: CupertinoPageScaffold(
        resizeToAvoidBottomInset: false,
        backgroundColor: AppColors.mainBlue,
        navigationBar: CupertinoNavigationBar(
          backgroundColor: AppColors.mainBlue,
          brightness: Brightness.dark,
          leading: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              AppBackButton(onPressed: () => context.go('/home')),
              const SizedBox(width: 8),
              Text(
                l10n.filter,
                style: const TextStyle(
                  color: AppColors.white,
                  fontWeight: FontWeight.w600,
                  fontSize: 17,
                ),
              ),
            ],
          ),
        ),
        child: MainScaffold(
          child: ClipRRect(
            borderRadius: AppLayout.authContentBorderRadius,
            child: Container(
              padding: const EdgeInsets.all(15),
              color: AppColors.white,
              child: SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Label(title: l10n.companyName),
                    const SizedBox(height: 5),
                    FilterField(
                      controller: companyNameController,
                      placeholder: l10n.companyName,
                    ),
                    const SizedBox(height: 10),
                    Label(title: l10n.driverName),
                    const SizedBox(height: 5),
                    FilterField(
                      controller: driverNameController,
                      placeholder: l10n.driverName,
                    ),
                    const SizedBox(height: 10),
                    Label(title: l10n.carNumber),
                    const SizedBox(height: 5),
                    FilterField(
                      controller: plateController,
                      placeholder: l10n.carNumber,
                    ),
                    const SizedBox(height: 15),
                    Label(title: l10n.searchByDate),
                    const SizedBox(height: 10),
                    RadioGroup<int>(
                      groupValue: dateRadio,
                      onChanged: (value) => onDateRadioChanged(value),
                      child: Column(
                        children: [
                          RadioBox(
                            title: l10n.today,
                            value: 1,
                            onPress: () => onDateRadioChanged(1),
                          ),
                          const SizedBox(height: 10),
                          RadioBox(
                            title: l10n.yesterday,
                            value: 2,
                            onPress: () => onDateRadioChanged(2),
                          ),
                          const SizedBox(height: 10),
                          RadioBox(
                            title: l10n.lastWeek,
                            value: 3,
                            onPress: () => onDateRadioChanged(3),
                          ),
                          const SizedBox(height: 10),
                          Container(
                            padding: const EdgeInsets.all(10),
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(10),
                              border: Border.all(
                                width: 1,
                                color: AppColors.lightGray,
                              ),
                            ),
                            child: Column(
                              children: [
                                GestureDetector(
                                  onTap: () => onDateRadioChanged(4),
                                  child: Row(
                                    children: [
                                      Label(title: l10n.chooseDate),
                                      const Spacer(),
                                      CustomPaint(
                                        painter: RadioBorderPainter(
                                          borderWidth: 3,
                                          borderColor: AppColors.lightGray,
                                        ),
                                        child: const CupertinoRadio<int>(
                                          value: 4,
                                          activeColor: AppColors.mainBlue,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                                const SizedBox(height: 10),
                                Row(
                                  children: [
                                    Expanded(
                                      child: _buildDateField(
                                        context,
                                        startDate,
                                        dateRadio,
                                        true,
                                        (newDate) =>
                                            setState(() => startDate = newDate),
                                      ),
                                    ),
                                    const SizedBox(width: 12),
                                    Expanded(
                                      child: _buildDateField(
                                        context,
                                        endDate,
                                        dateRadio,
                                        false,
                                        (newDate) =>
                                            setState(() => endDate = newDate),
                                      ),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 20),
                    Label(title: l10n.tripStatus),
                    const SizedBox(height: 5),
                    RadioGroup<int>(
                      groupValue: tripDocumentStatus,
                      onChanged: (value) => onStatusChanged(value),
                      child: Column(
                        children: [
                          RadioBox(
                            title: l10n.noDocumentsCollected,
                            value: 1,
                            onPress: () => onStatusChanged(1),
                          ),
                          const SizedBox(height: 10),
                          RadioBox(
                            title: l10n.partiallyCollectedDocuments,
                            value: 2,
                            onPress: () => onStatusChanged(2),
                          ),
                          if (widget.isFinancialCollector) ...[
                            const SizedBox(height: 10),
                            RadioBox(
                              title: l10n.withCollector,
                              value: 3,
                              onPress: () => onStatusChanged(3),
                            ),
                            const SizedBox(height: 10),
                            RadioBox(
                              title: l10n.partiallyConfirmedDocuments,
                              value: 4,
                              onPress: () => onStatusChanged(4),
                            ),
                          ],
                        ],
                      ),
                    ),

                    const SizedBox(height: 15),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                      children: [
                        AppSecondaryButton(
                          onPressed: () {
                            setState(() {
                              dateRadio = null;
                              startDate = null;
                              endDate = null;
                              tripDocumentStatus = null;
                              companyNameController.text = '';
                              driverNameController.text = '';
                              plateController.text = '';
                              widget.filterData.search = null;
                              widget.filterData.companyName = null;
                              widget.filterData.driverName = null;
                              widget.filterData.plate = null;
                              widget.filterData.dateRadio = null;
                              widget.filterData.endDate = null;
                              widget.filterData.startDate = null;
                              widget.filterData.tripDocumentStatuses =
                                  widget.isFinancialCollector
                                  ? [1, 2, 3, 4]
                                  : [1, 2];
                            });
                          },
                          label: l10n.resetFilter,
                          padding: const EdgeInsets.symmetric(
                            vertical: 10,
                            horizontal: 50,
                          ),
                        ),
                        AppPrimaryButton(
                          onPressed: () {
                            widget.filterData.page = 1;
                            widget.filterData.search = null;
                            if (companyNameController.text.trim().isNotEmpty) {
                              widget.filterData.companyName =
                                  companyNameController.text.trim();
                            }
                            if (driverNameController.text.trim().isNotEmpty) {
                              widget.filterData.driverName =
                                  driverNameController.text.trim();
                            }
                            if (plateController.text.trim().isNotEmpty) {
                              widget.filterData.plate = plateController.text
                                  .trim();
                            }
                            widget.filterData.dateRadio = dateRadio;
                            if (dateRadio == 1) {
                              widget.filterData.startDate = DateTime.now()
                                  .startOfDay
                                  .toUtc()
                                  .toIso8601String();
                              widget.filterData.endDate = DateTime.now()
                                  .endOfDay
                                  .toUtc()
                                  .toIso8601String();
                            } else if (dateRadio == 2) {
                              widget.filterData.startDate = DateTime.now()
                                  .yesterday
                                  .startOfDay
                                  .toUtc()
                                  .toIso8601String();
                              widget.filterData.endDate = DateTime.now()
                                  .yesterday
                                  .endOfDay
                                  .toUtc()
                                  .toIso8601String();
                            } else if (dateRadio == 3) {
                              widget.filterData.startDate = DateTime.now()
                                  .daysAgo(7)
                                  .startOfDay
                                  .toUtc()
                                  .toIso8601String();
                              widget.filterData.endDate = DateTime.now()
                                  .toUtc()
                                  .toIso8601String();
                            } else {
                              if (startDate != null) {
                                widget.filterData.startDate = startDate
                                    ?.toUtc()
                                    .toIso8601String();
                              }
                              if (endDate != null) {
                                widget.filterData.endDate = endDate
                                    ?.toUtc()
                                    .toIso8601String();
                              }
                            }
                            if (tripDocumentStatus != null) {
                              widget.filterData.tripDocumentStatuses = [
                                ?tripDocumentStatus,
                              ];
                            }

                            context.go('/home');
                          },
                          label: l10n.search,
                          padding: const EdgeInsets.symmetric(
                            vertical: 10,
                            horizontal: 50,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildDateField(
    BuildContext context,
    DateTime? date,
    int? dateRadio,
    bool isStart,
    Function(DateTime) onDateChanged,
  ) {
    return GestureDetector(
      onTap: dateRadio != 4
          ? () {}
          : () => _showDatePicker(context, date, onDateChanged, isStart),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
        decoration: BoxDecoration(
          border: Border.all(color: AppColors.lightGray),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Directionality(
              textDirection: TextDirection.ltr,
              child: Text(
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
                    ? startDate ?? DateTime.now()
                    : endDate ?? DateTime.now(),
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

class Label extends StatelessWidget {
  final String title;
  const Label({super.key, required this.title});

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: const TextStyle(
        fontSize: 16,
        fontWeight: FontWeight.w400,
        color: AppColors.darkGray,
      ),
    );
  }
}

class FilterField extends StatelessWidget {
  final TextEditingController controller;
  final ValueChanged<String>? onChanged;
  final String? placeholder;
  const FilterField({
    super.key,
    required this.controller,
    this.onChanged,
    this.placeholder,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10),
      width: double.infinity,
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(50),
        border: Border.all(color: AppColors.lightGray),
      ),
      child: CupertinoTextField(
        controller: controller,
        placeholder: placeholder ?? AppLocalizations.of(context)!.search,
        textInputAction: TextInputAction.done,
        keyboardType: TextInputType.text,
        style: const TextStyle(color: CupertinoColors.black, fontSize: 13),
        prefix: const Icon(CupertinoIcons.search),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: null,
        onTapOutside: (event) {
          FocusManager.instance.primaryFocus?.unfocus();
        },
        onChanged: onChanged,
      ),
    );
  }
}

class RadioBox extends StatelessWidget {
  final String title;
  final int value;
  final VoidCallback onPress;
  const RadioBox({
    super.key,
    required this.title,
    required this.value,
    required this.onPress,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onPress,
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          border: Border.all(width: 1, color: AppColors.lightGray),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              title,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: AppColors.darkGray,
              ),
            ),
            CustomPaint(
              painter: RadioBorderPainter(
                borderWidth: 3,
                borderColor: AppColors.lightGray,
              ),
              child: CupertinoRadio<int>(
                value: value,
                activeColor: AppColors.mainBlue,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class RadioBorderPainter extends CustomPainter {
  final double borderWidth;
  final Color borderColor;

  RadioBorderPainter({required this.borderWidth, required this.borderColor});

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = borderColor
      ..style = PaintingStyle.stroke
      ..strokeWidth = borderWidth;

    canvas.drawCircle(
      Offset(size.width / 2, size.height / 2),
      (size.width / 2) - borderWidth / 2,
      paint,
    );
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
