import 'package:flutter/cupertino.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../core/widgets/text_field_error.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../views/filter_screen.dart';

// ignore: must_be_immutable
class NotRecivedPopup extends StatefulWidget {
  int? status;
  String? note;
  final void Function(int?, String?) submit;
  NotRecivedPopup({super.key, this.status, this.note, required this.submit});

  @override
  State<NotRecivedPopup> createState() => _NotRecivedPopupState();
}

class _NotRecivedPopupState extends State<NotRecivedPopup> {
  final noteController = TextEditingController();
  String? error;

  @override
  void initState() {
    if (widget.note != null) {
      noteController.text = widget.note!;
    }
    super.initState();
  }

  @override
  void dispose() {
    noteController.dispose();
    super.dispose();
  }

  bool vaildate(AppLocalizations l10n) {
    if (widget.status == null) {
      setState(() {
        error = l10n.selectTheReasonsForNotReceivingTheDocument;
      });
      return false;
    } else if (widget.status == 7 &&
        (widget.note == null || widget.note!.isEmpty)) {
      setState(() {
        error = l10n.enterNote;
      });
      return false;
    } else {
      return true;
    }
  }

  void onStatusChanged(int? value) {
    setState(() {
      widget.status = value;
      error = null;
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return CupertinoPageScaffold(
      backgroundColor: AppColors.darkGray.withValues(alpha: 0.4),
      child: Center(
        child: SingleChildScrollView(
          child: Container(
            margin: const EdgeInsets.symmetric(horizontal: 10),
            padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 15),
            decoration: BoxDecoration(
              color: AppColors.white,
              borderRadius: BorderRadius.circular(12),
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      l10n.deliveryPermission,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                        color: AppColors.darkGray,
                      ),
                    ),
                    Text(
                      l10n.notReceived,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                        color: AppColors.red,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                RadioGroup<int>(
                  groupValue: widget.status,
                  onChanged: (value) => onStatusChanged(value),
                  child: Column(
                    children: [
                      // RadioBox(
                      //   title: l10n.stillWithCustomerWaitingForSignature,
                      //   status: 2,
                      //   onPress: () => onStatusChanged(2),
                      // ),
                      // const SizedBox(height: 10),
                      RadioBox(
                        title: l10n.lostByDriver,
                        status: 3,
                        onPress: () => onStatusChanged(3),
                      ),
                      const SizedBox(height: 10),
                      RadioBox(
                        title: l10n.tripDoesNotExist,
                        status: 4,
                        onPress: () => onStatusChanged(4),
                      ),
                      const SizedBox(height: 10),
                      // RadioBox(
                      //   title: l10n.willBeSentLater,
                      //   status: 5,
                      //   onPress: () => onStatusChanged(5),
                      // ),
                      // const SizedBox(height: 10),
                      // RadioBox(
                      //   title: l10n.customerRefusedToSign,
                      //   status: 6,
                      //   onPress: () => onStatusChanged(6),
                      // ),
                      // const SizedBox(height: 10),
                      GestureDetector(
                        onTap: () => onStatusChanged(7),
                        child: Container(
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(10),
                            border: Border.all(
                              width: 0.5,
                              color: AppColors.lightGray,
                            ),
                          ),
                          child: Column(
                            children: [
                              Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  Text(
                                    l10n.other,
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
                                    child: const CupertinoRadio<int>(
                                      value: 7,
                                      activeColor: AppColors.mainBlue,
                                    ),
                                  ),
                                ],
                              ),
                              const SizedBox(height: 10),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 10,
                                ),
                                width: double.infinity,
                                decoration: BoxDecoration(
                                  color: AppColors.white,
                                  borderRadius: BorderRadius.circular(10),
                                  border: Border.all(
                                    color: AppColors.lightGray,
                                  ),
                                ),
                                child: CupertinoTextField(
                                  onTapOutside: (event) {
                                    FocusManager.instance.primaryFocus
                                        ?.unfocus();
                                  },
                                  controller: noteController,
                                  placeholder: l10n.note,
                                  textInputAction: TextInputAction.done,
                                  keyboardType: TextInputType.text,
                                  style: const TextStyle(
                                    color: CupertinoColors.black,
                                  ),
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 16,
                                    vertical: 14,
                                  ),
                                  decoration: null,
                                  onChanged: (value) => setState(() {
                                    widget.note = noteController.text.trim();
                                  }),
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                      TextFieldError(error: error),
                      const SizedBox(height: 10),
                    ],
                  ),
                ),

                // Footer Buttons
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: Row(
                    children: [
                      Expanded(
                        child: AppSecondaryButton(
                          onPressed: () => Navigator.pop(context),
                          label: l10n.cancel,
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: AppPrimaryButton(
                          onPressed: () {
                            if (vaildate(l10n)) {
                              widget.submit(widget.status, widget.note);
                              Navigator.pop(context);
                            }
                          },
                          label: l10n.confirm,
                        ),
                      ),
                    ],
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

class RadioBox extends StatelessWidget {
  final String title;
  final int status;
  final VoidCallback onPress;
  const RadioBox({
    super.key,
    required this.title,
    required this.status,
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
          border: Border.all(width: 0.5, color: AppColors.lightGray),
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
                value: status,
                activeColor: AppColors.mainBlue,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
