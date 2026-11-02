import 'package:flutter/cupertino.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/text_field_error.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../views/add_trip_screen.dart';

class AddTripWaypoints extends StatefulWidget {
  final FormData formData;
  final FormDataError formDataError;
  const AddTripWaypoints({
    super.key,
    required this.formData,
    required this.formDataError,
  });

  @override
  State<AddTripWaypoints> createState() => _AddTripWaypointsState();
}

class _AddTripWaypointsState extends State<AddTripWaypoints>
    with AutomaticKeepAliveClientMixin {
  final _urlController = TextEditingController();
  final _addressController = TextEditingController();
  final _noteController = TextEditingController();
  String? urlError;
  String? addressError;
  int pointRadio = 0;
  double? longitude;
  double? latitude;
  String? addressName;
  String? googlePlaceId;
  bool extractPlaceDone = true;
  @override
  bool get wantKeepAlive => true;

  @override
  void dispose() {
    _urlController.dispose();
    _addressController.dispose();
    _noteController.dispose();
    super.dispose();
  }

  @override
  void initState() {
    if (widget.formData.destination != null) {}
    super.initState();
  }

  void _deletePoint(int index, AppLocalizations l10n) {
    showCupertinoDialog(
      context: context,
      builder: (context) => CupertinoAlertDialog(
        title: Text(l10n.deletePoint),
        content: Text(
          '${l10n.wantToDelete} ${widget.formData.waypoints[index].stopType == 0 ? l10n.pickupPoint : l10n.dropoffPoint}',
        ),
        actions: [
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () => Navigator.pop(context),
            child: Text(l10n.cancel),
          ),
          CupertinoDialogAction(
            isDestructiveAction: true,
            onPressed: () {
              setState(() {
                widget.formData.waypoints.removeAt(index);
                for (int i = 0; i < widget.formData.waypoints.length; i++) {
                  widget.formData.waypoints[i] = widget.formData.waypoints[i]
                      .copyWith(sequence: i);
                }
              });
              Navigator.pop(context);
            },
            child: Text(l10n.delete),
          ),
        ],
      ),
    );
  }

  void _addPoint(AppLocalizations l10n) {
    setState(() {
      if (longitude == null || latitude == null) {
        urlError = l10n.enterGoogleMapsUrl;
      }
      if (_addressController.text.trim().isEmpty) {
        addressError = l10n.enterAddress;
      }
      if (longitude != null &&
          latitude != null &&
          _addressController.text.trim().isNotEmpty) {
        widget.formData.waypoints.add(
          Waypoint(
            sequence: widget.formData.waypoints.length,
            stopType: pointRadio,
            latitude: latitude,
            longitude: longitude,
            addressName: addressName,
            googlePlaceId: googlePlaceId,
            taskDetails: _noteController.text.trim().isEmpty
                ? null
                : _noteController.text.trim(),
          ),
        );
        extractPlaceDone = true;
        urlError = null;
        _urlController.text = '';
        _addressController.text = '';
        _noteController.text = '';
        longitude = null;
        latitude = null;
        addressName = null;
      }
    });
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
            Visibility(
              visible: widget.formData.destination == null,
              maintainState: true,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Label(label: l10n.pickDropPoints, isTitle: true),
                  const SizedBox(height: 20),
                  Label(label: l10n.chooseThePoint),
                  const SizedBox(height: 10),
                  RadioGroup<int>(
                    groupValue: pointRadio,
                    onChanged: (value) => setState(() {
                      pointRadio = value!;
                    }),
                    child: Row(
                      children: [
                        const CupertinoRadio<int>(
                          value: 0,
                          activeColor: AppColors.mainBlue,
                        ),
                        const SizedBox(width: 5),
                        Label(label: l10n.pickupPoint),
                        const Spacer(),
                        const CupertinoRadio<int>(
                          value: 1,
                          activeColor: AppColors.mainBlue,
                        ),
                        const SizedBox(width: 5),
                        Label(label: l10n.dropoffPoint),
                      ],
                    ),
                  ),
                  const SizedBox(height: 20),
                  Label(label: l10n.googleMapLink),
                  const SizedBox(height: 5),
                  BlocConsumer<TripsBloc, TripsState>(
                    listener: (context, state) {
                      setState(() {
                        if (state is ExtractPlaceLoaded) {
                          if (state.error != null) {
                            urlError = l10n.enterGoogleMapsUrl;
                          } else {
                            latitude = state.extractedPlace.lat;
                            longitude = state.extractedPlace.lng;
                            googlePlaceId = state.extractedPlace.placeId;
                            urlError = null;
                            if (state.extractedPlace.address != null) {
                              addressName = state.extractedPlace.address;
                              _addressController.text = addressName!;
                              addressError = null;
                            }
                          }
                        }
                      });
                    },
                    builder: (context, state) {
                      return TripField(
                        placeholder: l10n.url,
                        controller: _urlController,
                        onChanged: (value) => setState(() {
                          urlError = null;
                          extractPlaceDone = false;
                          context.read<TripsBloc>().add(
                            TripsExtractPlace(url: value.trim()),
                          );
                        }),
                        suffix: extractPlaceDone
                            ? null
                            : state is ExtractPlaceLoading
                            ? const CupertinoActivityIndicator(
                                color: AppColors.mainBlue,
                                radius: 10,
                              )
                            : state is ExtractPlaceLoaded && state.error != null
                            ? const Icon(
                                CupertinoIcons.xmark,
                                size: 18,
                                color: CupertinoColors.systemRed,
                              )
                            : state is ExtractPlaceLoaded && state.error == null
                            ? const Icon(
                                CupertinoIcons.checkmark_alt,
                                size: 18,
                                color: CupertinoColors.activeGreen,
                              )
                            : null,
                      );
                    },
                  ),
                  TextFieldError(error: urlError),
                  const SizedBox(height: 20),
                  Label(label: l10n.address),
                  const SizedBox(height: 5),
                  TripField(
                    controller: _addressController,
                    maxLines: 3,
                    onTapOutside: (event) {
                      FocusManager.instance.primaryFocus?.unfocus();
                    },
                    onChanged: (value) => setState(() {
                      addressName = value;
                      addressError = null;
                    }),
                  ),
                  TextFieldError(error: addressError),
                  const SizedBox(height: 20),
                  Label(label: l10n.addNote),
                  const SizedBox(height: 5),
                  TripField(
                    controller: _noteController,
                    maxLines: 3,
                    onTapOutside: (event) {
                      FocusManager.instance.primaryFocus?.unfocus();
                    },
                  ),
                  TextFieldError(error: widget.formDataError.waypointsError),
                  const SizedBox(height: 20),
                  Container(
                    decoration: BoxDecoration(
                      border: Border.all(color: AppColors.mainBlue, width: 0.5),
                      borderRadius: BorderRadius.circular(50),
                    ),
                    child: CupertinoButton(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 24,
                        vertical: 14,
                      ),
                      borderRadius: BorderRadius.circular(50),
                      color: AppColors.white,
                      disabledColor: AppColors.white.withValues(alpha: 0.5),
                      onPressed: () => _addPoint(l10n),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Text(
                            l10n.addWaypoint,
                            style: const TextStyle(
                              color: AppColors.mainBlue,
                              fontSize: 16,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                          const SizedBox(width: 5),
                          SvgPicture.asset('assets/images/add_circle.svg'),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),
            ListView.builder(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              padding: const EdgeInsets.all(16),
              itemCount: widget.formData.waypoints.length,
              itemBuilder: (context, index) {
                return TrackingPointItem(
                  waypoint: widget.formData.waypoints[index],
                  index: index,
                  isLast: index == widget.formData.waypoints.length - 1,
                  contractDestination: widget.formData.destination != null,
                  onDelete: () => _deletePoint(index, l10n),
                );
              },
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

class TripField extends StatelessWidget {
  final TextEditingController? controller;
  final bool isDigitsonly;
  final Widget? prefix;
  final Widget? suffix;
  final String? placeholder;
  final int? maxLines;
  final ValueChanged<PointerDownEvent>? onTapOutside;
  final ValueChanged<String>? onChanged;
  final ValueChanged<String>? onSubmitted;

  const TripField({
    super.key,
    this.controller,
    this.isDigitsonly = false,
    this.prefix,
    this.placeholder,
    this.maxLines,
    this.onChanged,
    this.suffix,
    this.onSubmitted,
    this.onTapOutside,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray),
      ),
      child: CupertinoTextField(
        controller: controller,
        placeholder: placeholder,
        keyboardType: isDigitsonly ? TextInputType.number : TextInputType.text,
        style: const TextStyle(color: CupertinoColors.black),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        maxLines: maxLines,
        onTapOutside: onTapOutside,
        onChanged: onChanged,
        onSubmitted: onSubmitted,
        decoration: null,
        prefix: prefix,
        suffix: suffix,
        inputFormatters: isDigitsonly
            ? [FilteringTextInputFormatter.digitsOnly]
            : [],
      ),
    );
  }
}

class TrackingPointItem extends StatelessWidget {
  final Waypoint waypoint;
  final int index;
  final bool isLast;
  final bool contractDestination;
  final VoidCallback onDelete;

  const TrackingPointItem({
    super.key,
    required this.waypoint,
    required this.isLast,
    required this.onDelete,
    required this.index,
    required this.contractDestination,
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

          const SizedBox(width: 8),

          // Right side - Delete button
          if (!contractDestination)
            CupertinoButton(
              padding: EdgeInsets.zero,
              onPressed: onDelete,
              child: SvgPicture.asset(
                'assets/images/trash.svg',
                width: 20,
                height: 20,
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
