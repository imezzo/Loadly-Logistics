// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/destination.dart';
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../widgets/add_trip_information.dart';
import '../widgets/add_trip_review.dart';
import '../widgets/add_trip_waypoints.dart';
import '../widgets/page_indicator.dart';

class AddTripScreen extends StatefulWidget {
  final int? tripId;
  final FormData formData;
  const AddTripScreen({super.key, required this.formData, this.tripId});

  @override
  State<AddTripScreen> createState() => _AddTripScreenState();
}

class _AddTripScreenState extends State<AddTripScreen> {
  final _pageController = PageController();
  int _currentPage = 0;
  final int _totalPages = 3;
  // final FormData formData = FormData(waypoints: []);
  final FormDataError formDataError = FormDataError();
  bool _canSubmit = false;
  bool _submitRequest = false;

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  void _addTrip() {
    context.read<TripsBloc>().add(
      TripsAddtrip(
        contactPersonName: widget.formData.contactPersonName,
        contactPersonMobile: widget.formData.contactPersonMobile,
        contractId: widget.formData.contractId,
        destinationId: widget.formData.destination?.id,
        submitRequest: _submitRequest,
        contractDistanceKm: widget.formData.contractDistanceKm,
        actualCargoWeightTons: widget.formData.actualCargoWeightTons,
        cargoPrice: widget.formData.cargoPrice,
        startDate: widget.formData.startDate,
        endDate: widget.formData.endDate,
        helpersCount: widget.formData.helpersNumber,
        waypoints: widget.formData.waypoints,
      ),
    );
  }

  void _updateTrip() {
    context.read<TripsBloc>().add(
      TripsUpdatetrip(
        tripId: widget.tripId,
        contactPersonName: widget.formData.contactPersonName,
        contactPersonMobile: widget.formData.contactPersonMobile,
        contractId: widget.formData.contractId,
        destinationId: widget.formData.destination?.id,
        submitRequest: _submitRequest,
        contractDistanceKm: widget.formData.contractDistanceKm,
        actualCargoWeightTons: widget.formData.actualCargoWeightTons,
        cargoPrice: widget.formData.cargoPrice,
        startDate: widget.formData.startDate,
        endDate: widget.formData.endDate,
        helpersCount: widget.formData.helpersNumber,
        waypoints: widget.formData.waypoints,
      ),
    );
  }

  void _showErrorDialog(String error, AppLocalizations l10n) {
    showCupertinoDialog(
      context: context,
      builder: (context) => CupertinoAlertDialog(
        title: Text(l10n.error),
        content: Text(error),
        actions: [
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () => Navigator.pop(context),
            child: Text(l10n.ok),
          ),
        ],
      ),
    );
  }

  void _showDraftPopup(AppLocalizations l10n) {
    showCupertinoModalPopup(
      context: context,
      builder: (BuildContext context) => BlocBuilder<TripsBloc, TripsState>(
        builder: (context, state) {
          final isLoading =
              state is AddTripLoading || state is UpdateTripLoading;
          return Container(
            height: 250,
            padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 20),
            decoration: const BoxDecoration(
              color: AppColors.white,
              borderRadius: BorderRadius.only(
                topLeft: Radius.circular(12),
                topRight: Radius.circular(12),
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Text(
                  l10n.saveTripAsDraft,
                  style: const TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.w500,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 10),
                Text(
                  textAlign: TextAlign.center,
                  l10n.saveTripAsDraftDesc,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w400,
                    color: CupertinoColors.systemGrey,
                  ),
                ),
                const SizedBox(height: 15),
                SizedBox(
                  width: double.infinity,
                  child: AppPrimaryButton(
                    onPressed: isLoading
                        ? null
                        : () {
                            _submitRequest = false;
                            if (widget.tripId == null) {
                              _addTrip();
                            } else {
                              _updateTrip();
                            }
                          },
                    loading: isLoading,
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Text(l10n.saveAsDraft),
                        const SizedBox(width: 10),
                        const Icon(
                          CupertinoIcons.doc_fill,
                          color: AppColors.white,
                          size: 20,
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(height: 10),
                SizedBox(
                  width: double.infinity,
                  child: AppSecondaryButton(
                    onPressed: isLoading
                        ? () {}
                        : () => Navigator.of(context).pop(),
                    color: AppColors.red,
                    label: l10n.discard,
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  void _validateInformation(AppLocalizations l10n) {
    _canSubmit = true;
    if (widget.formData.contactPersonName == null ||
        widget.formData.contactPersonName!.isEmpty) {
      setState(() {
        formDataError.contactPersonNameError = l10n.enterContactName;
        _canSubmit = false;
      });
    }
    if (widget.formData.contactPersonMobile == null ||
        widget.formData.contactPersonMobile!.isEmpty ||
        widget.formData.contactPersonMobile!.length != 13) {
      setState(() {
        formDataError.contactPersonMobileError = l10n.enterValidPhone11Digits;
        _canSubmit = false;
      });
    }
    if (!widget.formData.contactPersonMobile!.startsWith('+201')) {
      setState(() {
        formDataError.contactPersonMobileError = l10n.enterValidPhone01;
        _canSubmit = false;
      });
    }
    if (widget.formData.contractId == null) {
      setState(() {
        formDataError.contractIdError = l10n.pickContractRef;
        _canSubmit = false;
      });
    }
    if (widget.formData.contractDistanceKm == null) {
      setState(() {
        formDataError.contractDistanceKmError = l10n.enterDistance;
        _canSubmit = false;
      });
    }
    if ((widget.formData.consumptionType == 3 ||
            widget.formData.consumptionType == 5) &&
        widget.formData.actualCargoWeightTons == null &&
        widget.formData.cargoPrice == null) {
      setState(() {
        formDataError.cargoError = l10n.enterCargo;
        _canSubmit = false;
      });
    }
    if (widget.formData.startDate == null ||
        widget.formData.startDate!.isEmpty) {
      setState(() {
        formDataError.startDateError = l10n.pickStartDate;
        _canSubmit = false;
      });
    }
    if (widget.formData.endDate == null || widget.formData.endDate!.isEmpty) {
      setState(() {
        formDataError.endDateError = l10n.pickEndDate;
        _canSubmit = false;
      });
    }
  }

  void _validateWaypoints(AppLocalizations l10n) {
    if (widget.formData.waypoints.isEmpty ||
        widget.formData.waypoints
            .where((element) => element.stopType == 0)
            .isEmpty ||
        widget.formData.waypoints
            .where((element) => element.stopType == 1)
            .isEmpty) {
      setState(() {
        formDataError.waypointsError = l10n.addPickDropPoint;
        _canSubmit = false;
      });
    } else {
      setState(() {
        formDataError.waypointsError = null;
        _canSubmit = true;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          if (_currentPage > 0) {
            _pageController.previousPage(
              duration: const Duration(milliseconds: 500),
              curve: Curves.easeInOut,
            );
          } else {
            context.go('/trips');
          }
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
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        l10n.requestNewTrip,
                        style: const TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                      Text(
                        '${l10n.step} ${_currentPage + 1}',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Text(
                    l10n.requestNewTripDesc,
                    style: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w400,
                      color: CupertinoColors.systemGrey,
                    ),
                  ),
                  const SizedBox(height: 20),
                  AnimatedCustomPageIndicator(
                    currentPage: _currentPage,
                    totalPages: _totalPages,
                  ),
                  const SizedBox(height: 20),
                  Expanded(
                    child: PageView(
                      physics: const NeverScrollableScrollPhysics(),
                      controller: _pageController,
                      onPageChanged: (index) {
                        setState(() {
                          _currentPage = index;
                        });
                      },
                      children: [
                        AddTripInformation(
                          formData: widget.formData,
                          formDataError: formDataError,
                        ),
                        AddTripWaypoints(
                          formData: widget.formData,
                          formDataError: formDataError,
                        ),
                        AddTripReview(formData: widget.formData),
                      ],
                    ),
                  ),
                  BlocConsumer<TripsBloc, TripsState>(
                    listener: (context, state) {
                      if (state is AddTripDone) {
                        if (state.error != null) {
                          _showErrorDialog(state.error!, l10n);
                        } else {
                          if (_submitRequest == false) {
                            context.go('/trips');
                          } else {
                            context.go('/add-trip-success');
                          }
                        }
                      }
                      if (state is UpdateTripDone) {
                        if (state.error != null) {
                          _showErrorDialog(state.error!, l10n);
                        } else {
                          if (_submitRequest == false) {
                            context.go('/trips');
                          } else {
                            context.go('/add-trip-success');
                          }
                        }
                      }
                    },
                    builder: (context, state) {
                      return SizedBox(
                        width: double.infinity,
                        child: AppPrimaryButton(
                          onPressed:
                              state is AddTripLoading ||
                                  state is UpdateTripLoading
                              ? null
                              : () {
                                  if (_currentPage < _totalPages - 1) {
                                    if (_currentPage == 0) {
                                      _validateInformation(l10n);
                                      if (_canSubmit) {
                                        _pageController.nextPage(
                                          duration: const Duration(
                                            milliseconds: 500,
                                          ),
                                          curve: Curves.easeInOut,
                                        );
                                      }
                                    } else {
                                      _validateWaypoints(l10n);
                                      if (_canSubmit) {
                                        _pageController.nextPage(
                                          duration: const Duration(
                                            milliseconds: 500,
                                          ),
                                          curve: Curves.easeInOut,
                                        );
                                      }
                                    }
                                  } else {
                                    _submitRequest = true;
                                    if (widget.tripId == null) {
                                      _addTrip();
                                    } else {
                                      _updateTrip();
                                    }
                                  }
                                },
                          label: _currentPage < _totalPages - 1
                              ? l10n.continuee
                              : l10n.requestTrip,
                          loading:
                              state is AddTripLoading ||
                              state is UpdateTripLoading,
                        ),
                      );
                    },
                  ),
                  if (_currentPage == _totalPages - 1) ...[
                    const SizedBox(height: 10),
                    SizedBox(
                      width: double.infinity,
                      child: AppSecondaryButton(
                        onPressed: () => _showDraftPopup(l10n),
                        color: AppColors.mainBlue,
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Text(l10n.saveAsDraft),
                            const SizedBox(width: 10),
                            const Icon(
                              CupertinoIcons.doc_fill,
                              color: AppColors.mainBlue,
                              size: 20,
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                  const SizedBox(height: 10),
                  SizedBox(
                    width: double.infinity,
                    child: AppSecondaryButton(
                      onPressed: () {
                        if (_currentPage > 0) {
                          _pageController.previousPage(
                            duration: const Duration(milliseconds: 500),
                            curve: Curves.easeInOut,
                          );
                        } else {
                          context.go('/trips');
                        }
                      },
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

// Create a data model class
class FormData {
  String? contactPersonName;
  String? contactPersonMobile;
  int? contractId;
  String? contractRef;
  int? consumptionType;
  double? contractDistanceKm;
  double? actualCargoWeightTons;
  double? cargoPrice;
  String? startDate;
  String? endDate;
  int? helpersNumber;
  List<Waypoint> waypoints;
  Destination? destination;
  int? destinationId;

  FormData({
    this.contactPersonName,
    this.contactPersonMobile,
    this.contractId,
    this.contractRef,
    this.consumptionType,
    this.contractDistanceKm,
    this.actualCargoWeightTons,
    this.cargoPrice,
    this.startDate,
    this.endDate,
    this.helpersNumber,
    required this.waypoints,
    this.destination,
    this.destinationId,
  });
}

// Create a data model class
class FormDataError {
  String? contactPersonNameError;
  String? contactPersonMobileError;
  String? contractIdError;
  String? contractDistanceKmError;
  String? cargoError;
  String? startDateError;
  String? endDateError;
  String? waypointsError;

  FormDataError({
    this.contactPersonNameError,
    this.contactPersonMobileError,
    this.contractIdError,
    this.contractDistanceKmError,
    this.cargoError,
    this.startDateError,
    this.endDateError,
    this.waypointsError,
  });
}
