import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/invoice.dart';

class InvoiceDetailsScreen extends StatelessWidget {
  final Invoice invoice;
  const InvoiceDetailsScreen({super.key, required this.invoice});

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
    final List<Status> statuses = [
      Status(
        name: AppLocalizations.of(context)!.unpaid,
        code: 0,
        color: CupertinoColors.systemRed,
        icon: CupertinoIcons.xmark,
      ),
      Status(
        name: AppLocalizations.of(context)!.partiallyPaid,
        code: 1,
        color: CupertinoColors.systemIndigo,
        icon: CupertinoIcons.slowmo,
      ),
      Status(
        name: AppLocalizations.of(context)!.paidInvoice,
        code: 2,
        color: CupertinoColors.activeGreen,
        icon: CupertinoIcons.checkmark_alt,
      ),
    ];
    final status = statuses.firstWhere(
      (element) => element.code == invoice.status,
    );

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go('/invoices');
        }
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
        navigationBar: const AppNavBar(),
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 15),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              AppSecondaryButton(
                padding: const EdgeInsets.symmetric(
                  horizontal: 18,
                  vertical: 8,
                ),
                onPressed: () => context.go('/invoices'),
              ),
              const SizedBox(height: 10),
              Text(
                invoice.invoiceNumber ?? l10n.unknown,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                  color: AppColors.darkGray,
                ),
              ),
              const SizedBox(height: 10),
              Container(
                height: 290,
                padding: const EdgeInsets.all(15),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(12),
                  color: AppColors.white,
                ),
                child: GridView.count(
                  crossAxisCount: 2,
                  childAspectRatio: 2.3,
                  physics: const NeverScrollableScrollPhysics(),
                  crossAxisSpacing: 10,
                  children: [
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.invoiceId),
                        const SizedBox(height: 10),
                        Title(
                          title: invoice.id != null
                              ? invoice.id.toString()
                              : l10n.unknown,
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.contractRefNumber),
                        const SizedBox(height: 10),
                        Title(
                          title:
                              invoice.contractReferenceNumber ?? l10n.unknown,
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.dueDate),
                        const SizedBox(height: 10),
                        Directionality(
                          textDirection: TextDirection.ltr,
                          child: Title(
                            title: invoice.dueDate == null
                                ? l10n.unknown
                                : formatDate(invoice.dueDate!),
                          ),
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.totalAmount),
                        const SizedBox(height: 10),
                        Title(
                          title: invoice.totalAmount == null
                              ? l10n.unknown
                              : '${_formatCost(invoice.totalAmount!)} ${l10n.egp}',
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.paid),
                        const SizedBox(height: 10),
                        Title(
                          title: invoice.paidAmount == null
                              ? l10n.unknown
                              : '${_formatCost(invoice.paidAmount!)} ${l10n.egp}',
                          color: AppColors.green,
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.remaining),
                        const SizedBox(height: 10),
                        Title(
                          title: invoice.remainingAmount == null
                              ? l10n.unknown
                              : '${_formatCost(invoice.remainingAmount!)} ${l10n.egp}',
                          color: AppColors.red,
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.status),
                        const SizedBox(height: 10),
                        Container(
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(8),
                            color: status.color.withValues(alpha: 0.05),
                            border: Border.all(width: 0.5, color: status.color),
                          ),
                          child: Title(title: status.name, color: status.color),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 15),
              Container(
                padding: const EdgeInsets.symmetric(
                  vertical: 15,
                  horizontal: 5,
                ).copyWith(bottom: 20),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(12),
                  color: AppColors.white,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        SvgPicture.asset('assets/images/logo.svg'),
                        if (invoice.contract != null)
                          Column(
                            children: [
                              if (invoice.contract?.contractTypeName != null)
                                Container(
                                  padding: const EdgeInsets.all(10),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(8),
                                    color: AppColors.green.withValues(
                                      alpha: 0.1,
                                    ),
                                  ),
                                  child: Title(
                                    title:
                                        invoice.contract?.contractTypeName ??
                                        l10n.unknown,
                                    color: AppColors.green,
                                  ),
                                ),
                              const SizedBox(height: 10),
                              SizedBox(
                                width: MediaQuery.sizeOf(context).width * 0.6,
                                child: Title(
                                  maxLines: 2,
                                  title:
                                      '${l10n.contract}: ${Directionality.of(context) == TextDirection.rtl ? invoice.contract?.nameAr ?? l10n.unknown : invoice.contract?.nameEn ?? l10n.unknown}',
                                ),
                              ),
                            ],
                          ),
                      ],
                    ),
                    const SizedBox(height: 15),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Container(
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(8),
                            color: AppColors.mainGray,
                          ),
                          child: Row(
                            children: [
                              Subtitle(title: '${l10n.dueDate} '),
                              Directionality(
                                textDirection: TextDirection.ltr,
                                child: Subtitle(
                                  title: invoice.dueDate == null
                                      ? l10n.unknown
                                      : formatDate(invoice.dueDate!),
                                ),
                              ),
                            ],
                          ),
                        ),
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Subtitle(title: l10n.totalAmount),
                            const SizedBox(height: 10),
                            Title(
                              title: invoice.totalAmount == null
                                  ? l10n.unknown
                                  : '${_formatCost(invoice.totalAmount!)} ${l10n.egp}',
                            ),
                          ],
                        ),
                      ],
                    ),
                    const SizedBox(height: 15),
                    Container(
                      margin: const EdgeInsets.only(top: 10),
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(12),
                        border: BoxBorder.all(color: AppColors.lightGray),
                        color: AppColors.white,
                      ),
                      child: Column(
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(vertical: 10),
                            decoration: const BoxDecoration(
                              color: AppColors.mainGray,
                              borderRadius: BorderRadius.only(
                                topLeft: Radius.circular(12),
                                topRight: Radius.circular(12),
                              ),
                            ),
                            child: Padding(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 5,
                              ),
                              child: Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  Tabletitle(title: l10n.tripId, flex: 1),
                                  Tabletitle(title: l10n.km, flex: 1),
                                  Tabletitle(title: l10n.subTotal, flex: 2),
                                  Tabletitle(title: l10n.addons, flex: 2),
                                  Tabletitle(title: l10n.total, flex: 3),
                                ],
                              ),
                            ),
                          ),
                          ListView.separated(
                            shrinkWrap: true,
                            physics: const NeverScrollableScrollPhysics(),
                            itemBuilder: (context, index) {
                              return Padding(
                                padding: const EdgeInsets.symmetric(
                                  vertical: 8,
                                  horizontal: 5,
                                ),
                                child: Row(
                                  mainAxisAlignment:
                                      MainAxisAlignment.spaceBetween,
                                  children: [
                                    Tabletitle(
                                      title: invoice.trips?[index].id == null
                                          ? l10n.unknown
                                          : '${invoice.trips?[index].id}',
                                      flex: 1,
                                    ),
                                    Tabletitle(
                                      title:
                                          invoice
                                                  .trips?[index]
                                                  .actualDistanceKm ==
                                              null
                                          ? l10n.unknown
                                          : '${_formatCost(invoice.trips![index].actualDistanceKm!)} ${l10n.km}',
                                      flex: 1,
                                    ),
                                    Tabletitle(
                                      title:
                                          invoice
                                                  .trips?[index]
                                                  .tripCostWithoutAddon ==
                                              null
                                          ? l10n.unknown
                                          : '${_formatCost(invoice.trips![index].tripCostWithoutAddon!)} ${l10n.egp}',
                                      flex: 2,
                                    ),
                                    Tabletitle(
                                      title:
                                          invoice
                                                  .trips?[index]
                                                  .totalAddonsCost ==
                                              null
                                          ? l10n.unknown
                                          : '${_formatCost(invoice.trips![index].totalAddonsCost!)} ${l10n.egp}',
                                      flex: 2,
                                    ),
                                    Tabletitle(
                                      title:
                                          invoice.trips?[index].totalTripCost ==
                                              null
                                          ? l10n.unknown
                                          : '${_formatCost(invoice.trips![index].totalTripCost!)} ${l10n.egp}',
                                      flex: 3,
                                    ),
                                  ],
                                ),
                              );
                            },
                            separatorBuilder: (context, index) =>
                                const Divider(thickness: 0.5),
                            itemCount:
                                invoice.trips?.length ?? invoice.tripCount ?? 0,
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 15),
                    Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 40),
                      child: Column(
                        children: [
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Title(title: l10n.subTotal),
                              Title(
                                title: invoice.subTotal == null
                                    ? l10n.unknown
                                    : '${_formatCost(invoice.subTotal!)} ${l10n.egp}',
                              ),
                            ],
                          ),
                          const SizedBox(height: 10),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Title(
                                title: invoice.discount == null
                                    ? l10n.discount
                                    : '${l10n.discount} (${Directionality.of(context) == TextDirection.rtl ? invoice.discount?.nameAr : invoice.discount?.nameEn})',
                              ),
                              Title(
                                title:
                                    invoice.discount == null ||
                                        invoice.discount?.value == null
                                    ? '0%'
                                    : invoice.discount!.type != null &&
                                          invoice.discount!.type! ==
                                              'Percentage'
                                    ? '${_formatCost(invoice.discount!.value!)}%'
                                    : _formatCost(invoice.discount!.value!),
                              ),
                            ],
                          ),
                          const SizedBox(height: 10),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Title(
                                title: invoice.tax == null
                                    ? l10n.tax
                                    : '${l10n.tax} (${Directionality.of(context) == TextDirection.rtl ? invoice.tax?.nameAr : invoice.tax?.nameEn})',
                              ),
                              Title(
                                title:
                                    invoice.tax == null ||
                                        invoice.tax?.value == null
                                    ? '0%'
                                    : invoice.tax!.type != null &&
                                          invoice.tax!.type! == 'Percentage'
                                    ? '${_formatCost(invoice.tax!.value!)}%'
                                    : _formatCost(invoice.tax!.value!),
                              ),
                            ],
                          ),
                          const SizedBox(height: 10),
                          const Divider(thickness: 0.5),
                          const SizedBox(height: 10),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Title(title: l10n.total),
                              Title(
                                title: invoice.totalAmount == null
                                    ? l10n.unknown
                                    : '${_formatCost(invoice.totalAmount!)} ${l10n.egp}',
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class Title extends StatelessWidget {
  final String title;
  final Color color;
  final int? maxLines;
  const Title({
    super.key,
    required this.title,
    this.color = AppColors.darkGray,
    this.maxLines,
  });

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: TextStyle(fontSize: 14, fontWeight: FontWeight.w500, color: color),
      maxLines: maxLines,
    );
  }
}

class Subtitle extends StatelessWidget {
  final String title;
  const Subtitle({super.key, required this.title});

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: const TextStyle(
        fontSize: 14,
        fontWeight: FontWeight.w400,
        color: CupertinoColors.systemGrey,
      ),
    );
  }
}

class Tabletitle extends StatelessWidget {
  final String title;
  final int flex;
  const Tabletitle({super.key, required this.title, required this.flex});

  @override
  Widget build(BuildContext context) {
    return Expanded(
      // flex: flex,
      child: Text(
        title,
        style: const TextStyle(
          fontSize: 10,
          fontWeight: FontWeight.w500,
          color: AppColors.darkGray,
        ),
      ),
    );
  }
}

class Status {
  String name;
  int? code;
  Color color;
  IconData icon;

  Status({
    required this.name,
    required this.code,
    required this.color,
    required this.icon,
  });
}
