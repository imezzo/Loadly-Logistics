import 'dart:io';

import 'package:dotted_border/dotted_border.dart';
import 'package:flutter/cupertino.dart';
import 'package:image_picker/image_picker.dart';
import 'package:permission_handler/permission_handler.dart';

import '../../../../core/permissions/prominent_user_data_disclosure.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';

// ignore: must_be_immutable
class RecivedPopup extends StatefulWidget {
  List<File>? files;
  final void Function(List<File>?) submit;
  RecivedPopup({super.key, this.files, required this.submit});

  @override
  State<RecivedPopup> createState() => _RecivedPopupState();
}

class _RecivedPopupState extends State<RecivedPopup> {
  @override
  void dispose() {
    super.dispose();
  }

  Future<void> _pickImageFromCamera() async {
    final l10n = AppLocalizations.of(context)!;

    // Check camera permission
    final cameraStatus = await Permission.camera.status;
    if (!mounted) return;
    if (cameraStatus.isDenied) {
      final disclosed = await showProminentUserDataDisclosure(
        context,
        title: l10n.cameraPermissionDisclosureTitle,
        body: l10n.cameraPermissionDisclosureBody,
      );
      if (!disclosed || !mounted) return;
      final result = await Permission.camera.request();
      if (result.isDenied) {
        _showPermissionDeniedDialog(
          l10n.cameraPermissionRequired,
          l10n.permissionDenied,
        );
        return;
      }
    }

    if (cameraStatus.isPermanentlyDenied) {
      _showPermissionDeniedDialog(
        l10n.cameraPermissionRequired,
        l10n.permissionDenied,
      );
      return;
    }

    final ImagePicker picker = ImagePicker();
    final XFile? image = await picker.pickImage(
      source: ImageSource.camera,
      imageQuality: 85,
    );

    if (image != null && mounted) {
      setState(() {
        widget.files?.add(File(image.path));
      });
    }
  }

  void _showPermissionDeniedDialog(String message, String title) {
    final l10n = AppLocalizations.of(context)!;

    showCupertinoDialog<void>(
      context: context,
      builder: (BuildContext context) => CupertinoAlertDialog(
        title: Text(title),
        content: Text(message),
        actions: <CupertinoDialogAction>[
          CupertinoDialogAction(
            child: Text(l10n.cancel),
            onPressed: () {
              Navigator.pop(context);
            },
          ),
          CupertinoDialogAction(
            isDefaultAction: true,
            child: Text(l10n.openSettings),
            onPressed: () {
              Navigator.pop(context);
              openAppSettings();
            },
          ),
        ],
      ),
    );
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
                      l10n.received,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                        color: AppColors.green,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                Text(
                  l10n.takeAPhotoOfTheDocument,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w400,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 10),
                DottedBorder(
                  options: const RoundedRectDottedBorderOptions(
                    radius: Radius.circular(24),
                    color: AppColors.mainBlue,
                    strokeWidth: 1,
                    dashPattern: [8, 7], // 6px dash, 3px gap
                  ),
                  child: Container(
                    width: double.infinity,
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(24),
                      color: const Color(0xFFE5E9EF),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        CupertinoButton(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 15,
                            vertical: 5,
                          ),
                          borderRadius: BorderRadius.circular(50),
                          color: AppColors.white,
                          onPressed: _pickImageFromCamera,
                          child: Row(
                            children: [
                              const Icon(CupertinoIcons.camera),
                              const SizedBox(width: 5),
                              Text(
                                l10n.openCamera,
                                style: const TextStyle(
                                  fontSize: 12,
                                  fontWeight: FontWeight.w400,
                                  color: AppColors.darkGray,
                                ),
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(width: 10),
                        Text(
                          l10n.openCameraDocument,
                          style: const TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.w700,
                            color: AppColors.darkGray,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(height: 10),
                if (widget.files!.isNotEmpty)
                  Container(
                    width: double.infinity,
                    height: 150,
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(12),
                      color: const Color(0xFFE5E9EF).withValues(alpha: 0.2),
                    ),
                    child: ListView.separated(
                      scrollDirection: Axis.horizontal,
                      itemBuilder: (context, index) => Column(
                        children: [
                          ClipRRect(
                            borderRadius: BorderRadius.circular(10),
                            child: TweenAnimationBuilder<double>(
                              tween: Tween(begin: 0.0, end: 1.0),
                              duration: const Duration(seconds: 1),
                              curve: Curves.easeIn,
                              builder: (context, value, child) {
                                return Opacity(
                                  opacity: value,
                                  child: Image.file(
                                    widget.files![index],
                                    width: 100,
                                    height: 100,
                                    fit: BoxFit.cover,
                                  ),
                                );
                              },
                            ),
                          ),
                          const SizedBox(height: 10),
                          GestureDetector(
                            onTap: () => setState(() {
                              widget.files!.removeAt(index);
                            }),
                            child: const Icon(
                              CupertinoIcons.delete,
                              color: CupertinoColors.systemRed,
                              size: 20,
                            ),
                          ),
                        ],
                      ),
                      separatorBuilder: (context, index) =>
                          const SizedBox(width: 5),
                      itemCount: widget.files!.length,
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
                            widget.submit(widget.files);
                            Navigator.pop(context);
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
  const RadioBox({super.key, required this.title, required this.status});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(10),
        border: Border.all(width: 0.5, color: const Color(0xFFE5E9EF)),
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
          CupertinoRadio<int>(value: status, activeColor: AppColors.mainBlue),
        ],
      ),
    );
  }
}
