import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart'
    show
        CircularProgressIndicator,
        Icon,
        Icons,
        RoundedRectangleBorder,
        showModalBottomSheet;
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/utils/api_date_filter.dart';
import '../../../../core/widgets/api_error_view.dart';
import '../../../../core/widgets/app_blue_pattern_background.dart';
import '../../../../core/widgets/cupertino_lookup_picker.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../core/widgets/pagination_bar.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../data/models/lookup_item_dto.dart';
import '../../domain/use_cases/get_default_vehicle_types_usecase.dart';
import '../bloc/trip/trip_bloc.dart';
import '../bloc/trip/trip_event.dart';
import '../bloc/trip/trip_state.dart';
import '../bloc/trip_statistics/trip_statistics_bloc.dart';
import '../bloc/trip_statistics/trip_statistics_event.dart';
import '../bloc/trip_statistics/trip_statistics_state.dart';
import '../widgets/trips_financial_due_card.dart';

class TripsFinancialDueScreen extends StatefulWidget {
  const TripsFinancialDueScreen({super.key});

  @override
  State<TripsFinancialDueScreen> createState() =>
      _TripsFinancialDueScreenState();
}

class _TripsFinancialDueScreenState extends State<TripsFinancialDueScreen> {
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
      ),
    );

    final statisticsBloc = context.read<TripStatisticsBloc>();
    statisticsBloc.add(GetStatusStatistics());
    statisticsBloc.add(GetPayoutStatistics());
  }

  void _openFilterSheet() {
    showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (sheetContext) {
        return _TripsFinancialDueFilterSheet(
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

  void _handleBack() {
    if (context.canPop()) {
      context.pop();
      return;
    }
    context.go(AppRoutePaths.settings);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) _handleBack();
      },
      child: BlocListener<AuthBloc, AuthState>(
        listener: (context, authState) {
          if (authState is AuthUnauthenticated) {
            context.go(AppRoutePaths.login);
          }
        },
        child: CupertinoPageScaffold(
          backgroundColor: AppColors.white,
          child: MainScaffold(
            selectedIndex: 3,
            child: Column(
              children: [
                _TripsFinancialDuePageHeader(
                  title: l10n.tripsAndFinancialDue,
                  onBack: _handleBack,
                ),
                Expanded(
                  child: SingleChildScrollView(
                    physics: const ClampingScrollPhysics(),
                    padding: const EdgeInsets.fromLTRB(15, 12, 15, 24),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        BlocBuilder<TripStatisticsBloc, TripStatisticsState>(
                          builder: (context, statsState) {
                            final stats = statsState is TripStatisticsReady
                                ? _FinancialDueStats.fromStatistics(statsState)
                                : _FinancialDueStats.empty;
                            return _SummaryStatsCard(
                              totalTrips: stats.totalTrips,
                              dueTripsCount: stats.dueTripsCount,
                              dueAmountText: stats.dueAmountText,
                            );
                          },
                        ),
                        const SizedBox(height: 16),
                        _SearchFilterRow(
                          controller: _searchController,
                          placeholder: l10n.searchForTrip,
                          onChanged: () {
                            _searchQuery = _searchController.text;
                            _loadData(page: 1);
                          },
                          onFilterPressed: _openFilterSheet,
                        ),
                        const SizedBox(height: 16),
                        Text(
                          l10n.allTrips,
                          style: const TextStyle(
                            fontFamily: appFontFamily,
                            fontSize: 16,
                            fontWeight: FontWeight.w400,
                            color: AppColors.darkGray,
                          ),
                        ),
                        _TripsFinancialDueList(
                          onPageSelected: (page) => _loadData(page: page),
                          onRetry: () => _loadData(page: 1),
                        ),
                      ],
                    ),
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

class _FinancialDueStats {
  const _FinancialDueStats({
    required this.totalTrips,
    required this.dueTripsCount,
    required this.dueAmountText,
  });

  final int totalTrips;
  final int dueTripsCount;
  final String dueAmountText;

  static const empty = _FinancialDueStats(
    totalTrips: 0,
    dueTripsCount: 0,
    dueAmountText: '0',
  );

  factory _FinancialDueStats.fromStatistics(TripStatisticsReady state) {
    final dueAmount =
        state.payoutStatistics?.completedUnpaidTripsTotalAmount ?? 0;
    final dueAmountText = NumberFormat('#,###').format(dueAmount);

    return _FinancialDueStats(
      totalTrips: state.statusStatistics?.completedTrips ?? 0,
      dueTripsCount: state.payoutStatistics?.completedUnpaidTrips ?? 0,
      dueAmountText: dueAmountText,
    );
  }
}

class _TripsFinancialDueList extends StatelessWidget {
  const _TripsFinancialDueList({
    required this.onPageSelected,
    required this.onRetry,
  });

  final ValueChanged<int> onPageSelected;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRtl = Directionality.of(context) == TextDirection.rtl;

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
                separatorBuilder: (context, index) =>
                    const SizedBox(height: 12),
                itemBuilder: (context, index) {
                  final data = TripsFinancialDueCardData.fromTrip(
                    trips[index],
                    preferArabic: isRtl,
                  );
                  return TripsFinancialDueCard(data: data);
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

class _TripsFinancialDuePageHeader extends StatelessWidget {
  const _TripsFinancialDuePageHeader({
    required this.title,
    required this.onBack,
  });

  final String title;
  final VoidCallback onBack;

  @override
  Widget build(BuildContext context) {
    final isRtl = Directionality.of(context) == TextDirection.rtl;

    return AppBluePatternBackground(
      child: SafeArea(
        bottom: false,
        child: SizedBox(
          height: 76,
          width: double.infinity,
          child: Stack(
            fit: StackFit.expand,
            children: [
              Stack(
                alignment: Alignment.center,
                children: [
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 48),
                    child: Text(
                      title,
                      textAlign: TextAlign.center,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        fontFamily: appFontFamily,
                        color: AppColors.white,
                        fontSize: 17,
                        fontWeight: FontWeight.w600,
                        height: 1.2,
                      ),
                    ),
                  ),
                  PositionedDirectional(
                    start: 0,
                    top: 0,
                    bottom: 0,
                    child: CupertinoButton(
                      padding: const EdgeInsetsDirectional.only(start: 8),
                      minimumSize: Size.zero,
                      onPressed: onBack,
                      child: Icon(
                        isRtl
                            ? CupertinoIcons.chevron_forward
                            : CupertinoIcons.chevron_back,
                        color: AppColors.white,
                        size: 28,
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _SummaryStatsCard extends StatelessWidget {
  const _SummaryStatsCard({
    required this.totalTrips,
    required this.dueTripsCount,
    required this.dueAmountText,
  });

  final int totalTrips;
  final int dueTripsCount;
  final String dueAmountText;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 12),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(10),
        boxShadow: [
          BoxShadow(
            color: const Color.fromARGB(
              255,
              113,
              111,
              125,
            ).withValues(alpha: 0.18),
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            l10n.financialDueAllTripsCount('$totalTrips'),
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 14,
              fontWeight: FontWeight.w400,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 12),

          Row(
            children: [
              Expanded(
                child: Column(
                  children: [
                    Text(
                      l10n.dueTripsCountLabel,
                      textAlign: TextAlign.center,
                      style: const TextStyle(
                        fontFamily: appFontFamily,
                        fontSize: 12,
                        fontWeight: FontWeight.w400,
                        color: AppColors.darkGray,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      '$dueTripsCount',
                      style: const TextStyle(
                        fontFamily: appFontFamily,
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                        color: AppColors.darkGray,
                      ),
                    ),
                  ],
                ),
              ),
              Expanded(
                child: Column(
                  children: [
                    Text(
                      l10n.amountDueUntilNow,
                      textAlign: TextAlign.center,
                      style: const TextStyle(
                        fontFamily: appFontFamily,
                        fontSize: 12,
                        fontWeight: FontWeight.w400,
                        color: AppColors.darkGray,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      dueAmountText,
                      style: const TextStyle(
                        fontFamily: appFontFamily,
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                        color: AppColors.darkGray,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Text(
            l10n.financialDueAmountDescription,
            textAlign: TextAlign.center,
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 12,
              fontWeight: FontWeight.w400,
              color: CupertinoColors.systemGrey,
            ),
          ),
        ],
      ),
    );
  }
}

class _SearchFilterRow extends StatelessWidget {
  const _SearchFilterRow({
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
        const SizedBox(width: 8),
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
            child: Padding(
              padding: const EdgeInsets.all(12),
              child: SvgPicture.asset('assets/images/filter.svg'),
            ),
          ),
        ),
      ],
    );
  }
}

class _TripsFinancialDueFilterSheet extends StatefulWidget {
  const _TripsFinancialDueFilterSheet({
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
  State<_TripsFinancialDueFilterSheet> createState() =>
      _TripsFinancialDueFilterSheetState();
}

class _TripsFinancialDueFilterSheetState
    extends State<_TripsFinancialDueFilterSheet> {
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
            child: _FinancialDueFilterDateField(
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
            child: _FinancialDueFilterDateField(
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
            child: _FinancialDueFilterDropdownField(
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

class _FinancialDueFilterDateField extends StatelessWidget {
  const _FinancialDueFilterDateField({
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

class _FinancialDueFilterDropdownField extends StatelessWidget {
  const _FinancialDueFilterDropdownField({required this.child});

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
