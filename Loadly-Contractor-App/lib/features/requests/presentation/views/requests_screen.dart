import 'dart:async';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/utils/api_date_filter.dart';
import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/api_error_view.dart';
import '../../../../core/widgets/app_main_tab_header.dart';
import '../../../../core/widgets/cupertino_lookup_picker.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../core/widgets/pagination_bar.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../home/presentation/widgets/pending_booking_card.dart';
import '../../../home/presentation/widgets/trip_request_card.dart';
import '../../../trips/domain/entities/booking_request/booking_request.dart';
import '../../../trips/domain/exclude_booked_trips.dart';
import '../../../trips/presentation/bloc/booking_request/booking_request_bloc.dart';
import '../../../trips/presentation/bloc/booking_request/booking_request_event.dart';
import '../../../trips/presentation/bloc/booking_request/booking_request_state.dart';
import '../../../trips/presentation/bloc/trip/trip_bloc.dart';
import '../../../trips/presentation/bloc/trip/trip_event.dart';
import '../../../trips/presentation/bloc/trip/trip_state.dart';
import '../../../trips/data/models/lookup_item_dto.dart';
import '../../../trips/domain/use_cases/get_default_vehicle_types_usecase.dart';
import 'transfers_list_type.dart';

class RequestsScreen extends StatefulWidget {
  const RequestsScreen({super.key, required this.listType});

  final TransfersListType listType;

  @override
  State<RequestsScreen> createState() => _RequestsScreenState();
}

class _RequestsScreenState extends State<RequestsScreen> {
  final _searchController = TextEditingController();
  String? _searchQuery;
  int? _vehicleTypeId;
  String? _startDateFrom;
  String? _startDateTo;
  Timer? _refreshTimer;

  @override
  void initState() {
    super.initState();
    _loadData();
    _refreshTimer = Timer.periodic(const Duration(seconds: 30), (_) {
      if (!mounted) return;

      var currentPage = 1;

      if (widget.listType.isBooked) {
        final state = context.read<BookingRequestBloc>().state;

        if (state is BookingRequestLoaded) {
          currentPage = state.currentPage;
        }
      } else {
        final state = context.read<TripBloc>().state;

        if (state is TripLoaded) {
          currentPage = state.currentPage;
        }
      }

      _loadData(page: currentPage);
    });
  }

  void _loadData({int page = 1}) {
    if (widget.listType.isBooked) {
      final trimmed = _searchQuery?.trim();
      final search = (trimmed == null || trimmed.isEmpty) ? null : trimmed;

      context.read<BookingRequestBloc>().add(
        GetBookingRequests(
          page: page,
          pageSize: 10,
          status: 0,
          search: search,
          vehicleTypeId: _vehicleTypeId,
          startDateFrom: _startDateFrom,
          startDateTo: _startDateTo,
        ),
      );
    } else {
      final trimmed = _searchQuery?.trim();
      final search = (trimmed == null || trimmed.isEmpty) ? null : trimmed;

      context.read<TripBloc>().add(
        GetTrips(
          page: page,
          pageSize: 10,
          status: 1,
          openInviteOnly: true,
          search: search,
          vehicleTypeId: _vehicleTypeId,
          startDateFrom: _startDateFrom,
          startDateTo: _startDateTo,
        ),
      );
    }
  }

  void _openFilterSheet() {
    showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (sheetContext) {
        return _RequestsFilterSheet(
          initialStartDateFrom: _startDateFrom,
          initialStartDateTo: _startDateTo,
          initialVehicleTypeId: _vehicleTypeId,
          onApply: (startDateFrom, startDateTo, vehicleTypeId) {
            setState(() {
              _startDateFrom = startDateFrom;
              _startDateTo = startDateTo;
              _vehicleTypeId = vehicleTypeId;
            });
            Navigator.of(sheetContext).pop();
            _loadData(page: 1);
          },
          onClear: () {
            setState(() {
              _startDateFrom = null;
              _startDateTo = null;
              _vehicleTypeId = null;
            });
            Navigator.of(sheetContext).pop();
            _loadData(page: 1);
          },
        );
      },
    );
  }

  @override
  void dispose() {
    _refreshTimer?.cancel();
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isBooked = widget.listType.isBooked;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go(AppRoutePaths.home);
        }
      },
      child: BlocListener<AuthBloc, AuthState>(
        listener: (context, authState) {
          if (authState is AuthUnauthenticated) {
            context.go(AppRoutePaths.login);
          }
        },
        child: AppMainPageScaffold(
          backgroundColor: AppColors.white,
          child: SingleChildScrollView(
            physics: const ClampingScrollPhysics(),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Stack(
                  clipBehavior: Clip.none,
                  children: [
                    AppMainTabHeader(
                      bodyHeight: MediaQuery.sizeOf(context).height * 0.08,
                    ),
                    Padding(
                      padding: AppMainTabHeader.overlayContentPadding(context),
                      child: isBooked
                          ? const _BookedHeaderCard()
                          : const _RequestedHeaderCard(),
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.all(15),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const SizedBox(height: 12),

                      _SearchRow(
                        controller: _searchController,
                        placeholder: l10n.searchForTrip,
                        onChanged: () {
                          _searchQuery = _searchController.text;
                          _loadData(page: 1);
                        },
                        onFilterPressed: _openFilterSheet,
                      ),
                      const SizedBox(height: 10),
                      _Title(
                        title: isBooked
                            ? l10n.bookedTransfers
                            : l10n.requestedTransfers,
                        fontSize: 16,
                        color: isBooked ? AppColors.white : AppColors.darkGray,
                      ),
                      if (isBooked)
                        _BookedTripsList(
                          onPageSelected: (page) => _loadData(page: page),
                          onRetry: () => _loadData(page: 1),
                        )
                      else
                        _RequestedTripsList(
                          onPageSelected: (page) => _loadData(page: page),
                          onRetry: () => _loadData(page: 1),
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

class _RequestedHeaderCard extends StatelessWidget {
  const _RequestedHeaderCard();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<TripBloc, TripState>(
      builder: (context, state) {
        final count = state is TripLoaded ? state.totalCount : 0;
        return _HeaderShell(
          child: CupertinoListTile(
            padding: EdgeInsets.zero,
            leading: SvgPicture.asset(
              'assets/images/requests.svg',
              width: 28,
              height: 28,
              colorFilter: const ColorFilter.mode(
                AppColors.mainBlue,
                BlendMode.srcIn,
              ),
            ),
            title: _Title(
              title: l10n.newTripRequestsCount(count.toString()),
              fontSize: 16,
              fontWeight: FontWeight.w700,
            ),
            subtitle: _SubTitle(title: l10n.allYourTripRequests, fontSize: 12),
          ),
        );
      },
    );
  }
}

class _BookedHeaderCard extends StatelessWidget {
  const _BookedHeaderCard();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<BookingRequestBloc, BookingRequestState>(
      builder: (context, bookingState) {
        final count = bookingState is BookingRequestLoaded
            ? bookingState.totalCount
            : 0;
        return _HeaderShell(
          child: CupertinoListTile(
            padding: EdgeInsets.zero,
            leading: SvgPicture.asset(
              'assets/images/newspaper.svg',
              width: 28,
              height: 28,
              colorFilter: const ColorFilter.mode(
                AppColors.mainBlue,
                BlendMode.srcIn,
              ),
            ),
            title: _Title(
              title: l10n.bookedTransfersCount(count.toString()),
              fontSize: 16,
              fontWeight: FontWeight.w700,
            ),
            subtitle: _SubTitle(
              title: l10n.allYourBookedTripRequests,
              fontSize: 12,
              maxLines: 2,
            ),
          ),
        );
      },
    );
  }
}

class _HeaderShell extends StatelessWidget {
  const _HeaderShell({required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 15),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(10),
        boxShadow: [
          BoxShadow(
            color: CupertinoColors.systemGrey.withValues(alpha: 0.2),
            spreadRadius: 2,
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
        color: AppColors.white,
      ),
      child: child,
    );
  }
}

class _SearchRow extends StatelessWidget {
  const _SearchRow({
    required this.controller,
    required this.placeholder,
    required this.onChanged,
    required this.onFilterPressed,
  });

  final TextEditingController controller;
  final String placeholder;
  final VoidCallback onChanged;
  final VoidCallback onFilterPressed;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 10),
            decoration: BoxDecoration(
              color: AppColors.white,
              borderRadius: BorderRadius.circular(50),
              border: Border.all(color: AppColors.lightGray),
            ),
            child: CupertinoTextField(
              controller: controller,
              placeholder: placeholder,
              style: const TextStyle(
                fontFamily: appFontFamily,
                color: AppColors.darkGray,
                fontSize: 14,
                fontWeight: FontWeight.w400,
              ),
              onChanged: (_) => onChanged(),
              prefix: SvgPicture.asset('assets/images/search.svg'),
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
              decoration: null,
            ),
          ),
        ),
        const SizedBox(width: 5),
        Container(
          decoration: BoxDecoration(
            border: Border.all(color: AppColors.mediumBlueGray, width: 0.5),
            borderRadius: BorderRadius.circular(50),
          ),
          child: CupertinoButton(
            padding: EdgeInsets.zero,
            borderRadius: BorderRadius.circular(50),
            color: AppColors.white,
            onPressed: onFilterPressed,
            child: SvgPicture.asset('assets/images/filter.svg'),
          ),
        ),
      ],
    );
  }
}

class _RequestedTripsList extends StatelessWidget {
  const _RequestedTripsList({
    required this.onPageSelected,
    required this.onRetry,
  });

  final ValueChanged<int> onPageSelected;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<TripBloc, TripState>(
      builder: (context, state) {
        if (state is TripError) {
          return ApiErrorView(message: state.message, onRetry: onRetry);
        }

        if (state is TripLoaded) {
          return BlocBuilder<BookingRequestBloc, BookingRequestState>(
            builder: (context, bookingState) {
              final bookings = bookingState is BookingRequestLoaded
                  ? bookingState.requests
                  : const <BookingRequest>[];
              final trips = excludeBookedTrips(state.trips, bookings);

              return Column(
                children: [
                  if (trips.isEmpty)
                    Center(
                      child: Padding(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 15,
                          vertical: 50,
                        ),
                        child: Column(
                          children: [
                            SvgPicture.asset('assets/images/no_trips.svg'),
                            Text(l10n.noTripsAvailable),
                          ],
                        ),
                      ),
                    )
                  else
                    ListView.separated(
                      padding: const EdgeInsets.only(top: 10),
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemCount: trips.length,
                      separatorBuilder: (_, _) => const SizedBox(height: 10),
                      itemBuilder: (context, index) {
                        return TripRequestCard(trip: trips[index]);
                      },
                    ),
                  PaginationBar(
                    currentPage: state.currentPage,
                    totalCount: state.totalCount,
                    pageSize: state.pageSize,
                    onPageSelected: onPageSelected,
                  ),
                ],
              );
            },
          );
        }

        return const Padding(
          padding: EdgeInsets.symmetric(vertical: 24),
          child: Center(
            child: CircularProgressIndicator(color: AppColors.mainBlue),
          ),
        );
      },
    );
  }
}

class _BookedTripsList extends StatelessWidget {
  const _BookedTripsList({required this.onPageSelected, required this.onRetry});

  final ValueChanged<int> onPageSelected;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<BookingRequestBloc, BookingRequestState>(
      builder: (context, state) {
        if (state is BookingRequestError) {
          return ApiErrorView(message: state.message, onRetry: onRetry);
        }

        if (state is BookingRequestLoaded) {
          final requests = state.requests;

          return Column(
            children: [
              if (requests.isEmpty)
                Center(
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 15,
                      vertical: 50,
                    ),
                    child: Column(
                      children: [
                        SvgPicture.asset('assets/images/no_trips.svg'),
                        Text(l10n.noTripsAvailable),
                      ],
                    ),
                  ),
                )
              else
                ListView.separated(
                  padding: const EdgeInsets.only(top: 10),
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  itemCount: requests.length,
                  separatorBuilder: (_, _) => const SizedBox(height: 10),
                  itemBuilder: (context, index) {
                    return PendingBookingCard(booking: requests[index]);
                  },
                ),
              PaginationBar(
                currentPage: state.currentPage,
                totalCount: state.totalCount,
                pageSize: state.pageSize,
                onPageSelected: onPageSelected,
              ),
            ],
          );
        }

        return const Padding(
          padding: EdgeInsets.symmetric(vertical: 24),
          child: Center(
            child: CircularProgressIndicator(color: AppColors.mainBlue),
          ),
        );
      },
    );
  }
}

class _Title extends StatelessWidget {
  const _Title({
    required this.title,
    required this.fontSize,
    this.fontWeight = FontWeight.w400,
    this.color = AppColors.darkGray,
  });

  final String title;
  final double fontSize;
  final FontWeight fontWeight;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: TextStyle(
        fontSize: fontSize,
        fontWeight: fontWeight,
        color: color,
      ),
    );
  }
}

class _SubTitle extends StatelessWidget {
  const _SubTitle({required this.title, required this.fontSize, this.maxLines});

  final String title;
  final double fontSize;
  final int? maxLines;
  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: TextStyle(
        fontSize: fontSize,
        color: CupertinoColors.systemGrey,
        fontWeight: FontWeight.w400,
      ),
      maxLines: maxLines,
    );
  }
}

class _RequestsFilterSheet extends StatefulWidget {
  const _RequestsFilterSheet({
    required this.initialStartDateFrom,
    required this.initialStartDateTo,
    required this.initialVehicleTypeId,
    required this.onApply,
    required this.onClear,
  });

  final String? initialStartDateFrom;
  final String? initialStartDateTo;
  final int? initialVehicleTypeId;
  final void Function(
    String? startDateFrom,
    String? startDateTo,
    int? vehicleTypeId,
  )
  onApply;
  final VoidCallback onClear;

  @override
  State<_RequestsFilterSheet> createState() => _RequestsFilterSheetState();
}

class _RequestsFilterSheetState extends State<_RequestsFilterSheet> {
  /// موعد بدء الرحلة → `StartDateFrom`
  DateTime? _startDateFrom;

  /// موعد نهاية الرحلة → `StartDateTo`
  DateTime? _startDateTo;
  List<LookupItemDto> _vehicleTypes = [];
  bool _loadingVehicleTypes = true;
  int? _selectedVehicleTypeId;

  @override
  void initState() {
    super.initState();
    _startDateFrom = localFilterDayFromApi(widget.initialStartDateFrom);
    _startDateTo = localFilterDayFromApi(widget.initialStartDateTo);
    _selectedVehicleTypeId = widget.initialVehicleTypeId;
    _loadVehicleTypes();
  }

  Future<void> _loadVehicleTypes() async {
    final useCase = context.read<GetDefaultVehicleTypesUseCase>();
    final result = await useCase(page: 1, limit: 20);

    if (!mounted) return;

    result.fold(
      (_) => setState(() => _loadingVehicleTypes = false),
      (items) => setState(() {
        _vehicleTypes = items;
        _loadingVehicleTypes = false;
      }),
    );
  }

  Future<DateTime?> _showCupertinoDatePicker({
    required DateTime initial,
    DateTime? minimumDate,
    DateTime? maximumDate,
  }) {
    final l10n = AppLocalizations.of(context)!;

    return showCupertinoModalPopup<DateTime>(
      context: context,
      builder: (sheetContext) {
        var selected = initial;

        return Container(
          height: 280,
          color: AppColors.white,
          child: Column(
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  CupertinoButton(
                    onPressed: () => Navigator.pop(sheetContext),
                    child: Text(l10n.cancel),
                  ),
                  CupertinoButton(
                    onPressed: () => Navigator.pop(sheetContext, selected),
                    child: Text(l10n.confirm),
                  ),
                ],
              ),
              Expanded(
                child: CupertinoDatePicker(
                  mode: CupertinoDatePickerMode.date,
                  initialDateTime: initial,
                  minimumDate: minimumDate ?? DateTime(2020),
                  maximumDate: maximumDate ?? DateTime(2100),
                  onDateTimeChanged: (date) => selected = date,
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  Future<void> _pickStartDateFrom() async {
    final initial = _startDateFrom ?? DateTime.now();
    final picked = await _showCupertinoDatePicker(
      initial: initial,
      maximumDate: _startDateTo,
    );
    if (picked == null || !mounted) return;

    setState(() => _startDateFrom = startOfLocalDay(picked));
  }

  Future<void> _pickStartDateTo() async {
    final initial = _startDateTo ?? DateTime.now();
    final picked = await _showCupertinoDatePicker(
      initial: initial,
      minimumDate: _startDateFrom,
    );
    if (picked == null || !mounted) return;

    setState(() => _startDateTo = startOfLocalDay(picked));
  }

  Future<void> _pickVehicleType() async {
    if (_vehicleTypes.isEmpty) return;

    final l10n = AppLocalizations.of(context)!;
    final options = vehicleTypeFilterOptions(
      allLabel: l10n.all,
      vehicleTypes: _vehicleTypes.map((e) => (id: e.id, label: e.value)),
    );

    final picked = await showCupertinoLookupPicker(
      context: context,
      options: options,
      selectedId: _selectedVehicleTypeId,
    );

    if (!mounted) return;
    setState(() => _selectedVehicleTypeId = picked);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final startDateFromLabel = filterDateLabel(
      _startDateFrom,
      todayLabel: l10n.today,
    );
    final startDateToLabel = filterDateLabel(
      _startDateTo,
      todayLabel: l10n.today,
    );
    final vehicleLabel = _selectedVehicleTypeId == null
        ? l10n.all
        : _vehicleTypes
              .firstWhere(
                (e) => e.id == _selectedVehicleTypeId,
                orElse: () => const LookupItemDto(id: 0, value: ''),
              )
              .value;

    return Padding(
      padding: EdgeInsets.only(
        left: 20,
        right: 20,
        top: 12,
        bottom: MediaQuery.paddingOf(context).bottom + 24,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Center(
            child: Container(
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: AppColors.lightGray,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
          ),
          const SizedBox(height: 16),
          Text(
            l10n.filter,
            textAlign: TextAlign.right,
            style: const TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.w700,
              color: AppColors.darkGray,
              fontFamily: appFontFamily,
            ),
          ),
          const SizedBox(height: 20),
          Align(
            alignment: Alignment.centerRight,
            child: Text(
              l10n.tripStartDate,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: AppColors.darkGray,
                fontFamily: appFontFamily,
              ),
            ),
          ),
          const SizedBox(height: 8),
          GestureDetector(
            onTap: _pickStartDateFrom,
            child: _RequestsFilterDateField(
              value: startDateFromLabel.isEmpty
                  ? l10n.today
                  : startDateFromLabel,
              isPlaceholder: startDateFromLabel.isEmpty,
            ),
          ),
          const SizedBox(height: 16),
          Align(
            alignment: Alignment.centerRight,
            child: Text(
              l10n.tripEndDate,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: AppColors.darkGray,
                fontFamily: appFontFamily,
              ),
            ),
          ),
          const SizedBox(height: 8),
          GestureDetector(
            onTap: _pickStartDateTo,
            child: _RequestsFilterDateField(
              value: startDateToLabel.isEmpty ? l10n.today : startDateToLabel,
              isPlaceholder: startDateToLabel.isEmpty,
            ),
          ),
          const SizedBox(height: 16),
          Align(
            alignment: Alignment.centerRight,
            child: Text(
              l10n.vehicleType,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: AppColors.darkGray,
                fontFamily: appFontFamily,
              ),
            ),
          ),
          const SizedBox(height: 8),
          GestureDetector(
            onTap: _loadingVehicleTypes ? null : _pickVehicleType,
            child: _RequestsFilterDropdownField(
              child: Align(
                alignment: Alignment.centerRight,
                child: _loadingVehicleTypes
                    ? const CupertinoActivityIndicator()
                    : Text(
                        vehicleLabel,
                        style: TextStyle(
                          fontSize: 14,
                          color: _selectedVehicleTypeId == null
                              ? AppColors.mediumBlueGray
                              : AppColors.darkGray,
                          fontFamily: appFontFamily,
                        ),
                      ),
              ),
            ),
          ),
          const SizedBox(height: 24),
          SizedBox(
            height: 48,
            child: CupertinoButton(
              padding: EdgeInsets.zero,
              color: AppColors.mainBlue,
              borderRadius: BorderRadius.circular(10),
              onPressed: () {
                final dates = apiDateFilterParams(
                  startDateLocal: _startDateFrom,
                  endDateLocal: _startDateTo,
                );
                widget.onApply(
                  dates.startDateFrom,
                  dates.startDateTo,
                  _selectedVehicleTypeId,
                );
              },
              child: Text(
                l10n.applyFilter,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w600,
                  fontFamily: appFontFamily,
                  color: AppColors.white,
                ),
              ),
            ),
          ),
          const SizedBox(height: 12),
          CupertinoButton(
            padding: EdgeInsets.zero,
            onPressed: widget.onClear,
            child: Text(
              l10n.clearFilter,
              style: const TextStyle(
                fontSize: 14,
                color: AppColors.mediumBlueGray,
                fontFamily: appFontFamily,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _RequestsFilterDateField extends StatelessWidget {
  const _RequestsFilterDateField({
    required this.value,
    required this.isPlaceholder,
  });

  final String value;
  final bool isPlaceholder;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        border: Border.all(color: AppColors.lightGray),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        textDirection: TextDirection.ltr,
        children: [
          const Icon(
            Icons.calendar_today_outlined,
            size: 20,
            color: AppColors.mediumBlueGray,
          ),
          const Spacer(),
          Text(
            value,
            style: TextStyle(
              fontSize: 14,
              color: isPlaceholder
                  ? AppColors.mediumBlueGray
                  : AppColors.darkGray,
              fontFamily: appFontFamily,
            ),
          ),
        ],
      ),
    );
  }
}

class _RequestsFilterDropdownField extends StatelessWidget {
  const _RequestsFilterDropdownField({required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        border: Border.all(color: AppColors.lightGray),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        children: [
          const Icon(Icons.keyboard_arrow_down, color: AppColors.darkGray),
          const SizedBox(width: 8),
          Expanded(child: child),
        ],
      ),
    );
  }
}
