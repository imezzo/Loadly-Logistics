import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;
import 'package:number_pagination/number_pagination.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/domain/entities/lookup.dart';
import '../../../trips/presentation/bloc/trips_bloc.dart';
import '../../../trips/presentation/bloc/trips_event.dart';
import '../../../trips/presentation/bloc/trips_state.dart';
import '../../domain/entities/get_invoice_statistics_result.dart';
import '../bloc/reports_bloc.dart';
import '../bloc/reports_event.dart';
import '../bloc/reports_state.dart';
import '../widgets/invoice_payment_request_filter_popup.dart';

class InvoicePaymentRequestsListingScreen extends StatefulWidget {
  const InvoicePaymentRequestsListingScreen({super.key});

  @override
  State<InvoicePaymentRequestsListingScreen> createState() =>
      _InvoicePaymentRequestsListingScreenState();
}

class _InvoicePaymentRequestsListingScreenState
    extends State<InvoicePaymentRequestsListingScreen> {
  final TextEditingController _searchController = TextEditingController();
  final RefreshController _refreshController = RefreshController(
    initialRefresh: false,
  );
  int? _pageFilter;
  int? _statusFilter;
  String? _searchFilter;
  int? _invoiceIdFilter;
  String? _startDateFilter;
  String? _endDateFilter;
  GetInvoiceStatisticsResult? invoiceStatistics;
  List<Lookup> contracts = [];

  @override
  void initState() {
    super.initState();
    if (invoiceStatistics == null) {
      _loadDataSequentially();
    } else {
      context.read<ReportsBloc>().add(const ReportsInvoicePaymentRequests());
    }
    context.read<TripsBloc>().add(const TripsLoadContracts());
  }

  @override
  void dispose() {
    _searchController.dispose();
    _refreshController.dispose();
    super.dispose();
  }

  Future<void> _loadDataSequentially() async {
    final bloc = context.read<ReportsBloc>();
    // Trigger first event
    bloc.add(const ReportsGetInvoiceStatistics());

    // Wait for first event to complete
    final firstState = await bloc.stream.firstWhere(
      (state) => state is InvoiceStatisticsLoaded,
    );

    // Check if first event was successful
    if (firstState is InvoiceStatisticsLoaded) {
      // Access data from first event
      invoiceStatistics = firstState.invoiceStatistics;
      // Trigger second event
      bloc.add(const ReportsInvoicePaymentRequests());
    }
  }

  void loadInvoices() {
    context.read<ReportsBloc>().add(
      ReportsInvoicePaymentRequests(
        page: _pageFilter,
        status: _statusFilter,
        search: _searchFilter,
        invoiceId: _invoiceIdFilter,
        startDate: _startDateFilter,
        endDate: _endDateFilter,
      ),
    );
  }

  String formatDate(String isoDate) {
    DateTime dateTime = DateTime.parse(isoDate).toLocal();

    // Get day, month, year
    String day = DateFormat('dd').format(dateTime);
    String month = DateFormat('MMMM').format(dateTime);
    String year = DateFormat('yyyy').format(dateTime);

    return '$day $month $year';
  }

  String _formatCost(double cost) {
    return NumberFormat('#,###.##').format(cost);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go('/reports');
        }
      },
      child: BlocListener<TripsBloc, TripsState>(
        listener: (context, state) {
          if (state is ContractsLoaded) {
            contracts = state.contracts;
          }
        },
        child: CupertinoPageScaffold(
          backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
          navigationBar: const AppNavBar(),
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 15),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                AppSecondaryButton(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 18,
                    vertical: 8,
                  ),
                  onPressed: () => context.go('/reports'),
                ),
                const SizedBox(height: 10),
                Text(
                  l10n.financialTransactionHistory,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w500,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 10),
                BlocBuilder<ReportsBloc, ReportsState>(
                  builder: (context, state) {
                    if (state is InvoiceStatisticsLoading) {
                      return const SizedBox(
                        height: 150,
                        child: Center(
                          child: CupertinoActivityIndicator(
                            color: AppColors.mainBlue,
                            radius: 15,
                          ),
                        ),
                      );
                    }

                    if (invoiceStatistics == null) {
                      return Center(
                        child: Padding(
                          padding: const EdgeInsets.all(5),
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              const Icon(
                                CupertinoIcons.exclamationmark_triangle,
                                size: 32,
                                color: AppColors.red,
                              ),
                              const SizedBox(height: 16),
                              const Text(
                                'Failed to load Invoice Statistics',
                                style: TextStyle(
                                  fontSize: 16,
                                  color: AppColors.darkGray,
                                ),
                                textAlign: TextAlign.center,
                              ),
                              const SizedBox(height: 10),
                              AppPrimaryButton(
                                label: l10n.retry,
                                onPressed: () => _loadDataSequentially(),
                              ),
                            ],
                          ),
                        ),
                      );
                    } else {
                      return SizedBox(
                        height: 150,
                        child: GridView(
                          gridDelegate:
                              const SliverGridDelegateWithFixedCrossAxisCount(
                                crossAxisCount: 2,
                                crossAxisSpacing: 5,
                                mainAxisSpacing: 5,
                                childAspectRatio: 2.2,
                              ),
                          physics: const NeverScrollableScrollPhysics(),
                          children: [
                            TripCostCard(
                              title:
                                  '${invoiceStatistics!.paidInvoicesCount} ${l10n.vs} ${invoiceStatistics!.unpaidInvoicesCount}',
                              subTitle:
                                  '${l10n.paidInvoices} ${l10n.and} ${Directionality.of(context) == TextDirection.rtl ? l10n.unpaid : l10n.unpaidInvoices}',
                            ),
                            TripCostCard(
                              title:
                                  '${_formatCost(invoiceStatistics!.totalInvoicesAmount)} ${l10n.egp}',
                              subTitle: l10n.totalAmount,
                            ),
                            TripCostCard(
                              title:
                                  '${_formatCost(invoiceStatistics!.totalPaidAmount)} ${l10n.egp}',
                              subTitle: l10n.totalPaid,
                              color: AppColors.mainBlue,
                            ),
                            TripCostCard(
                              title:
                                  '${_formatCost(invoiceStatistics!.totalRemainingAmount)} ${l10n.egp}',
                              subTitle: l10n.remainingBalance,
                              color: AppColors.red,
                            ),
                          ],
                        ),
                      );
                    }
                  },
                ),
                const SizedBox(height: 10),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
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
                          controller: _searchController,
                          placeholder: l10n.search,
                          textInputAction: TextInputAction.done,
                          keyboardType: TextInputType.text,
                          style: const TextStyle(color: CupertinoColors.black),
                          prefix: SvgPicture.asset('assets/images/search.svg'),
                          padding: const EdgeInsets.symmetric(
                            horizontal: 16,
                            vertical: 14,
                          ),
                          decoration: null,
                          onSubmitted: (value) {
                            if (value.isEmpty) {
                              _searchFilter = null;
                            } else {
                              _searchFilter = value;
                            }
                            _pageFilter = 1;
                            loadInvoices();
                          },
                        ),
                      ),
                    ),
                    const SizedBox(width: 5),
                    Container(
                      decoration: BoxDecoration(
                        border: Border.all(
                          color:
                              _startDateFilter != null ||
                                  _endDateFilter != null ||
                                  _statusFilter != null ||
                                  _invoiceIdFilter != null
                              ? AppColors.mainBlue
                              : AppColors.mediumBlueGray,
                          width: 0.5,
                        ),
                        borderRadius: BorderRadius.circular(50),
                      ),
                      child: CupertinoButton(
                        padding: EdgeInsets.zero,
                        borderRadius: BorderRadius.circular(50),
                        color: AppColors.white,
                        onPressed: () => Navigator.of(context).push(
                          PageRouteBuilder(
                            opaque: false,
                            barrierColor: CupertinoColors.black.withValues(
                              alpha: 0.5,
                            ),
                            barrierDismissible: true,
                            pageBuilder:
                                (context, animation, secondaryAnimation) {
                                  return InvoicePaymentRequestFilterPopup(
                                    startDate: _startDateFilter != null
                                        ? DateTime.parse(_startDateFilter!)
                                        : null,
                                    endDate: _endDateFilter != null
                                        ? DateTime.parse(_endDateFilter!)
                                        : null,
                                    status: _statusFilter,
                                    invoiceId: _invoiceIdFilter,
                                    fliter:
                                        (
                                          String? startDate,
                                          String? endDate,
                                          int? status,
                                          int? vehicleTypeId,
                                        ) {
                                          setState(() {
                                            _startDateFilter = startDate;
                                            _endDateFilter = endDate;
                                            _statusFilter = status;
                                            _invoiceIdFilter = vehicleTypeId;
                                            _pageFilter = 1;
                                          });
                                          loadInvoices();
                                        },
                                  );
                                },
                            transitionsBuilder:
                                (
                                  context,
                                  animation,
                                  secondaryAnimation,
                                  child,
                                ) {
                                  return ScaleTransition(
                                    scale: Tween<double>(begin: 0.8, end: 1.0)
                                        .animate(
                                          CurvedAnimation(
                                            parent: animation,
                                            curve: Curves.easeOutBack,
                                          ),
                                        ),
                                    child: FadeTransition(
                                      opacity: animation,
                                      child: child,
                                    ),
                                  );
                                },
                          ),
                        ),
                        child: SvgPicture.asset(
                          'assets/images/filter.svg',
                          colorFilter:
                              _startDateFilter != null ||
                                  _endDateFilter != null ||
                                  _statusFilter != null ||
                                  _invoiceIdFilter != null
                              ? const ColorFilter.mode(
                                  AppColors.mainBlue,
                                  BlendMode.srcIn,
                                )
                              : null,
                        ),
                      ),
                    ),
                  ],
                ),
                Expanded(
                  child: BlocBuilder<ReportsBloc, ReportsState>(
                    builder: (context, state) {
                      if (state is InvoicePaymentRequestsLoading ||
                          state is InvoiceStatisticsLoading) {
                        return const Center(
                          child: CupertinoActivityIndicator(
                            color: AppColors.mainBlue,
                            radius: 16,
                          ),
                        );
                      }

                      if (state is InvoicePaymentRequestsLoaded) {
                        if (state.error != null) {
                          return Center(
                            child: Padding(
                              padding: const EdgeInsets.all(24.0),
                              child: Column(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: [
                                  const Icon(
                                    CupertinoIcons.exclamationmark_triangle,
                                    size: 64,
                                    color: AppColors.red,
                                  ),
                                  const SizedBox(height: 16),
                                  Text(
                                    state.error!,
                                    style: const TextStyle(
                                      fontSize: 16,
                                      color: AppColors.darkGray,
                                    ),
                                    textAlign: TextAlign.center,
                                  ),
                                  const SizedBox(height: 24),
                                  AppPrimaryButton(
                                    label: l10n.retry,
                                    onPressed: () {
                                      loadInvoices();
                                    },
                                  ),
                                ],
                              ),
                            ),
                          );
                        }

                        if (state
                            .invoicePaymentRequestList
                            .invoicePaymentRequests!
                            .isEmpty) {
                          return Padding(
                            padding: const EdgeInsets.symmetric(vertical: 5),
                            child: ClipRRect(
                              borderRadius: AppLayout.authContentBorderRadius,
                              child: Container(
                                color: AppColors.white,
                                child: Center(
                                  child: Padding(
                                    padding: const EdgeInsets.all(24.0),
                                    child: Column(
                                      mainAxisAlignment:
                                          MainAxisAlignment.center,
                                      children: [
                                        const Icon(
                                          CupertinoIcons.doc_text,
                                          size: 64,
                                          color: AppColors.mediumBlueGray,
                                        ),
                                        const SizedBox(height: 16),
                                        Text(
                                          l10n.noInvoiceAvailable,
                                          style: const TextStyle(
                                            fontSize: 16,
                                            color: AppColors.darkGray,
                                          ),
                                          textAlign: TextAlign.center,
                                        ),
                                      ],
                                    ),
                                  ),
                                ),
                              ),
                            ),
                          );
                        }

                        return Expanded(
                          child: Column(
                            children: [
                              Expanded(
                                child: SmartRefresher(
                                  controller: _refreshController,
                                  onRefresh: () async {
                                    loadInvoices();
                                    await Future.delayed(
                                      const Duration(milliseconds: 500),
                                    );
                                    _refreshController.refreshCompleted();
                                  },
                                  header: ClassicHeader(
                                    idleText: l10n.pullToRefresh,
                                    releaseText: l10n.releaseToRefresh,
                                    refreshingIcon:
                                        const CupertinoActivityIndicator(
                                          color: AppColors.mainBlue,
                                        ),
                                  ),
                                  child: Container(
                                    margin: const EdgeInsets.only(top: 10),
                                    decoration: BoxDecoration(
                                      borderRadius: BorderRadius.circular(10),
                                      border: BoxBorder.all(
                                        color: AppColors.lightGray,
                                      ),
                                      color: AppColors.white,
                                    ),
                                    child: Column(
                                      children: [
                                        Container(
                                          padding: const EdgeInsets.symmetric(
                                            vertical: 10,
                                          ),
                                          decoration: const BoxDecoration(
                                            color: AppColors.mainGray,
                                            borderRadius: BorderRadius.only(
                                              topLeft: Radius.circular(10),
                                              topRight: Radius.circular(10),
                                            ),
                                          ),
                                          child: Padding(
                                            padding: const EdgeInsets.symmetric(
                                              horizontal: 10,
                                            ),
                                            child: Row(
                                              mainAxisAlignment:
                                                  MainAxisAlignment
                                                      .spaceBetween,
                                              children: [
                                                Text(
                                                  l10n.invoiceId,
                                                  style: const TextStyle(
                                                    fontSize: 14,
                                                    fontWeight: FontWeight.w400,
                                                    color: CupertinoColors
                                                        .systemGrey,
                                                  ),
                                                ),
                                                Text(
                                                  l10n.requestDate,
                                                  style: const TextStyle(
                                                    fontSize: 14,
                                                    fontWeight: FontWeight.w400,
                                                    color: CupertinoColors
                                                        .systemGrey,
                                                  ),
                                                ),
                                                Text(
                                                  l10n.action,
                                                  style: const TextStyle(
                                                    fontSize: 14,
                                                    fontWeight: FontWeight.w400,
                                                    color: CupertinoColors
                                                        .systemGrey,
                                                  ),
                                                ),
                                              ],
                                            ),
                                          ),
                                        ),
                                        Expanded(
                                          child: ListView.separated(
                                            itemBuilder: (context, index) {
                                              final invoicePaymentRequest = state
                                                  .invoicePaymentRequestList
                                                  .invoicePaymentRequests![index];
                                              return Padding(
                                                padding:
                                                    const EdgeInsets.symmetric(
                                                      vertical: 8,
                                                      horizontal: 10,
                                                    ),
                                                child: Row(
                                                  mainAxisAlignment:
                                                      MainAxisAlignment
                                                          .spaceBetween,
                                                  children: [
                                                    Text(
                                                      '${invoicePaymentRequest.id}',
                                                      style: const TextStyle(
                                                        fontSize: 14,
                                                        fontWeight:
                                                            FontWeight.w500,
                                                        color:
                                                            AppColors.darkGray,
                                                      ),
                                                    ),
                                                    Directionality(
                                                      textDirection:
                                                          TextDirection.ltr,
                                                      child: Text(
                                                        formatDate(
                                                          invoicePaymentRequest
                                                              .requestDate!,
                                                        ),
                                                        style: const TextStyle(
                                                          fontSize: 14,
                                                          fontWeight:
                                                              FontWeight.w400,
                                                          color: CupertinoColors
                                                              .systemGrey,
                                                        ),
                                                      ),
                                                    ),
                                                    GestureDetector(
                                                      onTap: () => context.go(
                                                        '/invoice-payment-requests/${invoicePaymentRequest.id}',
                                                        extra:
                                                            invoicePaymentRequest,
                                                      ),
                                                      child: const Icon(
                                                        CupertinoIcons.eye_fill,
                                                        size: 14,
                                                        color:
                                                            AppColors.darkGray,
                                                      ),
                                                    ),
                                                  ],
                                                ),
                                              );
                                            },
                                            separatorBuilder:
                                                (context, index) =>
                                                    const Divider(
                                                      thickness: 0.5,
                                                    ),
                                            itemCount: state
                                                .invoicePaymentRequestList
                                                .invoicePaymentRequests!
                                                .length,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ),
                                ),
                              ),
                              NumberPagination(
                                onPageChanged: (int pageNumber) {
                                  _pageFilter = pageNumber;
                                  loadInvoices();
                                },
                                visiblePagesCount: 3,
                                totalPages: state
                                    .invoicePaymentRequestList
                                    .pagination!
                                    .totalPages!,
                                currentPage: state
                                    .invoicePaymentRequestList
                                    .pagination!
                                    .page!,
                                buttonRadius: 10,
                                selectedButtonColor: AppColors.darkGray,
                                selectedNumberColor: AppColors.white,
                                unSelectedButtonColor: AppColors.white,
                                unSelectedNumberColor: AppColors.darkGray,
                                controlButtonSize: const Size(35, 35),
                                numberButtonSize: const Size(35, 35),
                              ),
                            ],
                          ),
                        );
                      }

                      return const SizedBox.shrink();
                    },
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

class TripCostCard extends StatelessWidget {
  final String title;
  final String subTitle;
  final Color color;
  const TripCostCard({
    super.key,
    required this.title,
    required this.subTitle,
    this.color = AppColors.darkGray,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(vertical: 15, horizontal: 15),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Text(
            title,
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w500,
              color: color,
            ),
          ),
          const SizedBox(height: 15),
          Text(
            subTitle,
            style: const TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w500,
              color: CupertinoColors.systemGrey,
            ),
          ),
        ],
      ),
    );
  }
}
