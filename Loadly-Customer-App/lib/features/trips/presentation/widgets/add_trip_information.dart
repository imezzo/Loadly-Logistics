import 'dart:developer';

import 'package:flutter/cupertino.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_checkbox.dart';
import '../../../../core/widgets/text_field_error.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/destination.dart';
import '../../domain/entities/lookup.dart';
import '../../domain/entities/trip.dart' hide Contract, VehicleType;
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../views/add_trip_screen.dart';

class AddTripInformation extends StatefulWidget {
  final FormData formData;
  final FormDataError formDataError;
  const AddTripInformation({
    super.key,
    required this.formData,
    required this.formDataError,
  });

  @override
  State<AddTripInformation> createState() => _AddTripInformationState();
}

class _AddTripInformationState extends State<AddTripInformation>
    with AutomaticKeepAliveClientMixin {
  final _nameController = TextEditingController();
  final _mobileController = TextEditingController();
  final _distanceController = TextEditingController();
  final _cargoWeightController = TextEditingController();
  final _cargoPriceController = TextEditingController();
  final _helpersController = TextEditingController();
  DateTime? startDate;
  DateTime? endDate;
  bool helperRadio = false;
  bool manualDestination = true;
  List<Lookup> contracts = [];
  List<Lookup> vehicleTypes = [];
  List<Lookup> destinations = [];

  @override
  bool get wantKeepAlive => true;

  @override
  void initState() {
    if (contracts.isEmpty) {
      context.read<TripsBloc>().add(const TripsLoadContracts());
    }
    if (widget.formData.contactPersonName != null) {
      _nameController.text = widget.formData.contactPersonName!;
    }
    if (widget.formData.contactPersonMobile != null) {
      _mobileController.text = widget.formData.contactPersonMobile!;
    }
    if (widget.formData.contractDistanceKm != null) {
      _distanceController.text = widget.formData.contractDistanceKm!.toString();
    }
    if (widget.formData.destination != null) {
      log(widget.formData.destination!.id.toString());
      manualDestination = false;
    }
    if (widget.formData.actualCargoWeightTons != null) {
      _cargoWeightController.text = widget.formData.actualCargoWeightTons!
          .toString();
    }
    if (widget.formData.cargoPrice != null) {
      _cargoPriceController.text = widget.formData.cargoPrice!.toString();
    }
    if (widget.formData.startDate != null) {
      startDate = DateTime.parse(widget.formData.startDate!);
    }
    if (widget.formData.endDate != null) {
      endDate = DateTime.parse(widget.formData.endDate!);
    }
    if (widget.formData.helpersNumber != null) {
      helperRadio = true;
      _helpersController.text = widget.formData.helpersNumber!.toString();
    }
    super.initState();
  }

  @override
  void dispose() {
    _nameController.dispose();
    _mobileController.dispose();
    _distanceController.dispose();
    _cargoWeightController.dispose();
    _cargoPriceController.dispose();
    _helpersController.dispose();
    super.dispose();
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
            Label(label: l10n.tripInformation, isTitle: true),
            const SizedBox(height: 20),
            Label(label: l10n.contactName),
            const SizedBox(height: 5),
            TripField(
              controller: _nameController,
              onChanged: (value) {
                setState(() {
                  widget.formData.contactPersonName = value.trim();
                  widget.formDataError.contactPersonNameError = null;
                });
              },
            ),
            TextFieldError(error: widget.formDataError.contactPersonNameError),
            const SizedBox(height: 10),
            Label(label: l10n.contactNumber),
            const SizedBox(height: 5),
            TripField(
              controller: _mobileController,
              isInt: true,
              prefix: l10n.localeName == 'en'
                  ? const Text(
                      '+2',
                      style: TextStyle(color: AppColors.darkGray),
                    )
                  : null,
              suffix: l10n.localeName == 'ar'
                  ? const Text(
                      '2+',
                      style: TextStyle(color: AppColors.darkGray),
                    )
                  : null,
              onChanged: (value) {
                setState(() {
                  widget.formData.contactPersonMobile = '+2${value.trim()}';
                  if (value.trim().length == 11 &&
                      value.trim().startsWith('01')) {
                    widget.formDataError.contactPersonMobileError = null;
                  }
                });
              },
            ),
            TextFieldError(
              error: widget.formDataError.contactPersonMobileError,
            ),
            const SizedBox(height: 10),
            BlocConsumer<TripsBloc, TripsState>(
              listener: (context, state) {
                if (state is ContractsLoaded && state.error != null) {
                  widget.formDataError.contractIdError = state.error;
                }
                if (state is ContractsLoaded && state.error == null) {
                  contracts = state.contracts;
                  if (widget.formData.waypoints.isNotEmpty &&
                      widget.formData.contractId != null) {
                    context.read<TripsBloc>().add(
                      TripsLoadContractDestinations(
                        contractId: widget.formData.contractId!,
                      ),
                    );
                  }
                  if (widget.formData.contractId != null) {
                    context.read<TripsBloc>().add(
                      TripsLoadConsumptionType(
                        contractId: widget.formData.contractId!,
                      ),
                    );
                  }
                }
                if (state is DestinationsLoaded && state.error == null) {
                  destinations = state.destinations;
                  log(destinations.first.id.toString());
                }
                if (state is ConsumptionTypeLoaded) {
                  setState(() {
                    widget.formData.consumptionType = state.consumptionType;
                  });
                }
                if (state is DestinationLoaded && state.error == null) {
                  setState(() {
                    widget.formData.destination = state.destination;
                    widget.formData.contractDistanceKm = state
                        .destination
                        .expectedKms
                        ?.toDouble();
                    widget.formDataError.contractDistanceKmError = null;
                    _distanceController.text =
                        state.destination.expectedKms == null
                        ? ''
                        : state.destination.expectedKms.toString();
                    widget.formData.waypoints = [];
                    widget.formData.waypoints.add(
                      Waypoint(
                        sequence: widget.formData.waypoints.length,
                        stopType: 0,
                        latitude: widget.formData.destination!.start!.latitude,
                        longitude:
                            widget.formData.destination!.start!.longitude,
                        addressName:
                            widget.formData.destination!.start!.addressName,
                        googlePlaceId:
                            widget.formData.destination?.start?.googlePlaceId,
                        taskDetails: widget.formData.destination?.description,
                      ),
                    );
                    widget.formData.waypoints.add(
                      Waypoint(
                        sequence: widget.formData.waypoints.length,
                        stopType: 1,
                        latitude: widget.formData.destination!.end!.latitude,
                        longitude: widget.formData.destination!.end!.longitude,
                        addressName:
                            widget.formData.destination!.end!.addressName,
                        googlePlaceId:
                            widget.formData.destination?.end?.googlePlaceId,
                        taskDetails: widget.formData.destination?.description,
                      ),
                    );
                  });
                }
              },
              builder: (context, state) {
                return Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Label(label: l10n.contractRefNumber),
                    const SizedBox(height: 5),
                    ContactField(
                      contracts: contracts,
                      contractId: widget.formData.contractId,
                      isLoading: state is ContractsLoading,
                      onChanged: (id, ref) => setState(() {
                        widget.formData.contractId = id;
                        widget.formData.contractRef = ref;
                        widget.formData.destination = null;
                        if (!manualDestination) {
                          _distanceController.text = '';
                          widget.formData.contractDistanceKm = null;
                          widget.formData.waypoints = [];
                        }
                        widget.formDataError.contractIdError = null;
                        context.read<TripsBloc>().add(
                          TripsLoadContractDestinations(contractId: id),
                        );
                        context.read<TripsBloc>().add(
                          TripsLoadConsumptionType(contractId: id),
                        );
                      }),
                    ),
                    TextFieldError(error: widget.formDataError.contractIdError),
                    const SizedBox(height: 10),
                    AppCheckbox(
                      label: l10n.addManualDestination,
                      checked: manualDestination,
                      onTap: () => setState(() {
                        manualDestination = !manualDestination;
                        widget.formData.waypoints = [];
                        widget.formData.destination = null;
                      }),
                    ),
                    const SizedBox(height: 10),
                    Label(label: l10n.contractDestination),
                    const SizedBox(height: 5),
                    ContractDestinationField(
                      destinations: destinations,
                      destination: widget.formData.destination,
                      isLoading: state is DestinationLoading,
                      isDisabled: manualDestination,
                      onChanged: (destinationId) =>
                          context.read<TripsBloc>().add(
                            TripsLoadContractDestination(
                              destinationId: destinationId,
                            ),
                          ),
                    ),
                  ],
                );
              },
            ),
            const SizedBox(height: 10),
            Label(label: l10n.distance),
            const SizedBox(height: 5),
            TripField(
              controller: _distanceController,
              isDouble: true,
              placeholder: l10n.km,
              enabled: manualDestination,
              onChanged: (value) {
                setState(() {
                  widget.formData.contractDistanceKm = double.tryParse(
                    value.trim(),
                  );
                  widget.formDataError.contractDistanceKmError = null;
                });
              },
            ),
            TextFieldError(error: widget.formDataError.contractDistanceKmError),
            const SizedBox(height: 10),
            Label(label: l10n.dateRange),
            const SizedBox(height: 5),
            DateField(
              context: context,
              date: startDate,
              isStart: true,
              onDateChanged: (date) => setState(() {
                startDate = date;
                widget.formData.startDate = date.toUtc().toIso8601String();
                widget.formDataError.startDateError = null;
                if (widget.formData.destination != null &&
                    widget.formData.destination?.numberOfDays != null) {
                  endDate = date.add(
                    Duration(days: widget.formData.destination!.numberOfDays!),
                  );
                  widget.formData.endDate = endDate?.toUtc().toIso8601String();
                  widget.formDataError.endDateError = null;
                }
              }),
            ),
            TextFieldError(error: widget.formDataError.startDateError),
            const SizedBox(height: 10),
            DateField(
              context: context,
              date: endDate,
              isStart: false,
              onDateChanged: (date) => setState(() {
                endDate = date;
                widget.formData.endDate = date.toUtc().toIso8601String();
                widget.formDataError.endDateError = null;
              }),
            ),
            TextFieldError(error: widget.formDataError.endDateError),

            if (widget.formData.consumptionType == 3) ...[
              const SizedBox(height: 20),
              Label(label: l10n.cargo, isTitle: true),
              const SizedBox(height: 10),
              Label(label: l10n.actualCargoWeightTons),
              const SizedBox(height: 5),
              TripField(
                controller: _cargoWeightController,
                onChanged: (value) {
                  setState(() {
                    widget.formData.actualCargoWeightTons = double.tryParse(
                      value.trim(),
                    );
                    widget.formDataError.cargoError = null;
                  });
                },
                isDouble: true,
              ),
              TextFieldError(error: widget.formDataError.cargoError),
              const SizedBox(height: 10),
            ],
            if (widget.formData.consumptionType == 5) ...[
              const SizedBox(height: 20),
              Label(label: l10n.cargo, isTitle: true),
              const SizedBox(height: 10),
              Label(label: l10n.cargoPrice),
              const SizedBox(height: 5),
              TripField(
                controller: _cargoPriceController,
                onChanged: (value) {
                  setState(() {
                    widget.formData.cargoPrice = double.tryParse(value.trim());
                    widget.formDataError.cargoError = null;
                  });
                },
                isDouble: true,
              ),
              TextFieldError(error: widget.formDataError.cargoError),
              const SizedBox(height: 10),
            ],
            // Label(label: l10n.cargoDetails),
            // const SizedBox(height: 5),
            // const TripField(),
            // const SizedBox(height: 10),
            // Label(label: l10n.dimensions),
            // const SizedBox(height: 5),
            // const TripField(isDigitsonly: true),
            // const SizedBox(height: 10),
            // Label(label: l10n.weight),
            // const SizedBox(height: 5),
            // const TripField(isDigitsonly: true),
            // const SizedBox(height: 10),
            // Label(label: l10n.specialHandlingRequirements),
            // const SizedBox(height: 5),
            // const TripField(),
            const SizedBox(height: 20),
            Label(label: l10n.helpers, isTitle: true),
            const SizedBox(height: 10),
            AppCheckbox(
              label: l10n.includeHelpers,
              checked: helperRadio,
              onTap: () => setState(() {
                helperRadio = !helperRadio;
                if (!helperRadio) {
                  widget.formData.helpersNumber = null;
                  _helpersController.text = '';
                }
              }),
            ),
            if (helperRadio) ...[
              const SizedBox(height: 10),
              Label(label: l10n.helpersNumber),
              const SizedBox(height: 5),
              TripField(
                controller: _helpersController,
                isInt: true,
                onChanged: (value) => setState(() {
                  widget.formData.helpersNumber = int.tryParse(value.trim());
                }),
              ),
            ],
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
  final bool isInt;
  final bool isDouble;
  final Widget? prefix;
  final Widget? suffix;
  final String? placeholder;
  final int? maxLines;
  final bool enabled;
  final ValueChanged<String>? onChanged;

  const TripField({
    super.key,
    this.controller,
    this.prefix,
    this.suffix,
    this.placeholder,
    this.maxLines,
    this.onChanged,
    this.enabled = true,
    this.isInt = false,
    this.isDouble = false,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10),
      decoration: BoxDecoration(
        color: enabled ? AppColors.white : AppColors.mainGray,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray),
      ),
      child: CupertinoTextField(
        controller: controller,
        placeholder: placeholder,
        keyboardType: isInt || isDouble
            ? const TextInputType.numberWithOptions(decimal: true)
            : TextInputType.text,
        style: const TextStyle(color: CupertinoColors.black),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        onChanged: onChanged,
        enabled: enabled,
        maxLines: maxLines,
        decoration: null,
        prefix: prefix,
        suffix: suffix,
        onTapOutside: (event) {
          FocusManager.instance.primaryFocus?.unfocus();
        },
        inputFormatters: isDouble
            ? [
                FilteringTextInputFormatter.allow(
                  RegExp(
                    r'^\d+\.?\d*',
                  ), // Allows digits and optional decimal point
                ),
                TextInputFormatter.withFunction((oldValue, newValue) {
                  // Prevent multiple decimal points
                  if (newValue.text.contains('.') &&
                      newValue.text.indexOf('.') !=
                          newValue.text.lastIndexOf('.')) {
                    return oldValue;
                  }
                  return newValue;
                }),
              ]
            : isInt
            ? [FilteringTextInputFormatter.digitsOnly]
            : [],
      ),
    );
  }
}

// ignore: must_be_immutable
class ContactField extends StatelessWidget {
  final List<Lookup> contracts;
  int? contractId;
  final Function(int, String) onChanged;
  final bool isLoading;
  ContactField({
    super.key,
    required this.contracts,
    required this.onChanged,
    required this.isLoading,
    required this.contractId,
  });

  @override
  Widget build(BuildContext context) {
    final FixedExtentScrollController controller = FixedExtentScrollController(
      initialItem: contractId == null || contracts.isEmpty
          ? 0
          : contracts.indexOf(
              contracts.firstWhere((contract) => contract.id == contractId),
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
                        if (contractId == null && contracts.isNotEmpty) {
                          onChanged(contracts[0].id!, contracts[0].value!);
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
                    contractId = contracts[index].id!;
                    onChanged(contracts[index].id!, contracts[index].value!);
                  },
                  children: contracts
                      .map(
                        (contract) => Directionality(
                          textDirection: TextDirection.ltr,
                          child: Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 10),
                            child: Center(
                              child: FittedBox(
                                fit: BoxFit.scaleDown,
                                child: Text(contract.value ?? ''),
                              ),
                            ),
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
            Expanded(
              child: Directionality(
                textDirection: TextDirection.ltr,
                child: Text(
                  contractId == null || contracts.isEmpty
                      ? ''
                      : contracts
                            .firstWhere((element) => element.id == contractId)
                            .value!,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w400,
                    color: AppColors.darkGray,
                  ),
                ),
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

// ignore: must_be_immutable
class ContractDestinationField extends StatelessWidget {
  final List<Lookup> destinations;
  Destination? destination;
  final Function(int) onChanged;
  final bool isLoading;
  final bool isDisabled;
  ContractDestinationField({
    super.key,
    required this.destinations,
    required this.onChanged,
    required this.isLoading,
    required this.destination,
    required this.isDisabled,
  });

  @override
  Widget build(BuildContext context) {
    final FixedExtentScrollController controller = FixedExtentScrollController(
      initialItem: destination == null || destinations.isEmpty
          ? 0
          : destinations.indexOf(
              destinations.firstWhere(
                (destinationItem) => destinationItem.id == destination?.id,
                orElse: () => destinations.first,
              ),
            ),
    );
    return GestureDetector(
      onTap: isDisabled
          ? () {}
          : () => showCupertinoModalPopup(
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
                      color: isDisabled ? AppColors.lightGray : null,
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.end,
                        children: [
                          CupertinoButton(
                            padding: EdgeInsets.zero,
                            onPressed: () {
                              if (destination == null &&
                                  destinations.isNotEmpty) {
                                onChanged(destinations[0].id!);
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
                          // destination = destinations[index];
                          onChanged(destinations[index].id!);
                        },
                        children: destinations
                            .map(
                              (destination) =>
                                  Center(child: Text(destination.value ?? '')),
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
          color: isDisabled ? AppColors.mainGray : null,
          border: Border.all(color: AppColors.lightGray),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              destination == null
                  ? ''
                  : Directionality.of(context) == TextDirection.rtl
                  ? destination?.nameAr ?? ''
                  : destination?.nameEn ?? '',
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

// ignore: must_be_immutable
class DateField extends StatelessWidget {
  final BuildContext context;
  DateTime? date;
  final bool isStart;
  final Function(DateTime) onDateChanged;
  DateField({
    super.key,
    required this.context,
    required this.isStart,
    required this.onDateChanged,
    this.date,
  });

  void _showDatePicker(BuildContext context, Function(DateTime) onDateChanged) {
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
                mode: CupertinoDatePickerMode.dateAndTime,
                initialDateTime: date ?? DateTime.now(),
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

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => _showDatePicker(context, onDateChanged),
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
                    : DateFormat("dd MMMM yyyy - hh:mm a").format(date!),
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
}
