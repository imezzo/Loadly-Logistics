import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart' hide TextDirection;
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
import '../../../home/presentation/widgets/trip_review_card.dart';
import '../../domain/entities/trip/trip_statistics.dart';
import '../bloc/trip/trip_bloc.dart';
import '../bloc/trip/trip_event.dart';
import '../bloc/trip/trip_state.dart';
import '../bloc/trip_statistics/trip_statistics_bloc.dart';
import '../bloc/trip_statistics/trip_statistics_event.dart';
import '../bloc/trip_statistics/trip_statistics_state.dart';
import '../../data/models/lookup_item_dto.dart';
import '../../domain/use_cases/get_default_vehicle_types_usecase.dart';

class CompletedTripsDueScreen extends StatefulWidget {
  const CompletedTripsDueScreen({super.key});

  @override
  State<CompletedTripsDueScreen> createState() =>
      _CompletedTripsDueScreenState();
}

class _CompletedTripsDueScreenState extends State<CompletedTripsDueScreen> {
  final _searchController = TextEditingController();
  String? _searchQuery;
  int? _vehicleTypeId;
  String? _startDateFrom;
  String? _startDateTo;

  @override
  void initState() {
    super.initState();

    _loadData(page: 1);
  }

  void _loadData({int page = 1}) {
    final trimmed = _searchQuery?.trim();
    final search = (trimmed == null || trimmed.isEmpty) ? null : trimmed;

    context.read<TripBloc>().add(
      GetTrips(
        page: page,
        pageSize: 10,
        status: 6,
        search: search,
        vehicleTypeId: _vehicleTypeId,
        startDateFrom: _startDateFrom,
        startDateTo: _startDateTo,
        isContractorPaid: false,
      ),
    );

    context.read<TripStatisticsBloc>().add(GetPayoutStatistics());
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
        return _CompletedTripsDueFilterSheet(
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
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go(AppRoutePaths.settings);
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
                      bodyHeight: MediaQuery.sizeOf(context).height * 0.11,
                    ),
                    Padding(
                      padding: AppMainTabHeader.overlayContentPadding(context),
                      child: const _StatsHeader(),
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.all(15),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
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
                      Text(
                        l10n.myTripsReview,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w400,
                          color: AppColors.darkGray,
                        ),
                      ),
                      _MyTripsList(
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

class _StatsHeader extends StatelessWidget {
  const _StatsHeader();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<TripStatisticsBloc, TripStatisticsState>(
      builder: (context, statsState) {
        final stats = statsState is TripStatisticsReady
            ? _TripStats.fromPayout(statsState.payoutStatistics)
            : _TripStats.empty;

        return Container(
          width: double.infinity,
          padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 8),
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
          child: Column(
            children: [
              CupertinoListTile(
                leading: SvgPicture.asset(
                  'assets/images/medel.svg',
                  colorFilter: const ColorFilter.mode(
                    AppColors.mainBlue,
                    BlendMode.srcIn,
                  ),
                ),
                title: Text(
                  l10n.completedTripsDueUntilNow,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w400,
                    color: AppColors.darkGray,
                  ),
                ),
              ),
              const Divider(color: AppColors.mainGray),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [
                  Column(
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      Text(
                        l10n.dueTripsCountLabel,
                        style: const TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.w400,
                          color: AppColors.darkGray,
                        ),
                      ),
                      Text(
                        '${stats.completedUnpaidTrips}',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w700,
                          color: AppColors.darkGray,
                        ),
                      ),
                    ],
                  ),
                  Column(
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      Text(
                        l10n.unpaidAmount,
                        style: const TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.w400,
                          color: AppColors.darkGray,
                        ),
                      ),
                      Text(
                        stats.unpaidTotalAmountFormatted,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w700,
                          color: AppColors.darkGray,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ],
          ),
        );
      },
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
              onChanged: (_) => onChanged(),
              style: const TextStyle(
                fontFamily: appFontFamily,
                color: AppColors.darkGray,
                fontSize: 14,
                fontWeight: FontWeight.w400,
              ),
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

class _MyTripsList extends StatelessWidget {
  const _MyTripsList({required this.onPageSelected, required this.onRetry});

  final ValueChanged<int> onPageSelected;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<TripBloc, TripState>(
      builder: (context, tripState) {
        if (tripState is TripInitial || tripState is TripLoading) {
          return const Padding(
            padding: EdgeInsets.symmetric(vertical: 24),
            child: Center(
              child: CircularProgressIndicator(color: AppColors.mainBlue),
            ),
          );
        }

        if (tripState is TripError) {
          return ApiErrorView(message: tripState.message, onRetry: onRetry);
        }

        if (tripState is! TripLoaded) {
          return const SizedBox.shrink();
        }

        final trips = tripState.trips;

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
                  return TripReviewCard(trip: trips[index]);
                },
              ),
            PaginationBar(
              currentPage: tripState.currentPage,
              totalCount: tripState.totalCount,
              pageSize: tripState.pageSize,
              onPageSelected: onPageSelected,
            ),
          ],
        );
      },
    );
  }
}

class _TripStats {
  const _TripStats({
    required this.completedUnpaidTrips,
    required this.unpaidTotalAmount,
  });

  final int completedUnpaidTrips;
  final double unpaidTotalAmount;

  String get unpaidTotalAmountFormatted =>
      NumberFormat('#,###').format(unpaidTotalAmount);

  static const empty = _TripStats(
    completedUnpaidTrips: 0,
    unpaidTotalAmount: 0,
  );

  factory _TripStats.fromPayout(PayoutStatistics? payout) {
    return _TripStats(
      completedUnpaidTrips: payout?.completedUnpaidTrips ?? 0,
      unpaidTotalAmount: payout?.completedUnpaidTripsTotalAmount ?? 0,
    );
  }
}

class _CompletedTripsDueFilterSheet extends StatefulWidget {
  const _CompletedTripsDueFilterSheet({
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
  State<_CompletedTripsDueFilterSheet> createState() =>
      _CompletedTripsDueFilterSheetState();
}

class _CompletedTripsDueFilterSheetState
    extends State<_CompletedTripsDueFilterSheet> {
  DateTime? _startDateFrom;
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
            child: _CompletedTripsFilterDateField(
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
            child: _CompletedTripsFilterDateField(
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
            child: _FilterDropdownField(
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

class _CompletedTripsFilterDateField extends StatelessWidget {
  const _CompletedTripsFilterDateField({
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

class _FilterDropdownField extends StatelessWidget {
  const _FilterDropdownField({required this.child});

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
