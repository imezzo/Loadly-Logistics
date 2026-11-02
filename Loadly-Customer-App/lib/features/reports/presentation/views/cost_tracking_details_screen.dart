import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/domain/entities/trip.dart';

class CostTrackingDetailsScreen extends StatelessWidget {
  final Trip trip;
  const CostTrackingDetailsScreen({super.key, required this.trip});

  String formatDate(String? isoDate, AppLocalizations l10n) {
    if (isoDate == null) {
      return l10n.unknown;
    }
    DateTime dateTime = DateTime.parse(isoDate).toLocal();

    // Get day, month, year
    String day = DateFormat('dd').format(dateTime);
    String month = DateFormat('MMMM').format(dateTime);
    String year = DateFormat('yyyy').format(dateTime);

    return '$day $month $year';
  }

  String _formatCost(double? cost, AppLocalizations l10n) {
    if (cost == null) {
      return l10n.unknown;
    }
    return NumberFormat('#,###.##').format(cost);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go('/cost-tracking');
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
                onPressed: () => context.go('/cost-tracking'),
              ),
              const SizedBox(height: 10),
              Text(
                '${l10n.tripId}: ${trip.id ?? l10n.unknown}',
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                  color: AppColors.darkGray,
                ),
              ),
              const SizedBox(height: 10),
              Container(
                height: MediaQuery.sizeOf(context).height * 0.40,
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
                        Subtitle(title: l10n.tripId),
                        const SizedBox(height: 10),
                        Title(title: '${trip.id ?? l10n.unknown}'),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.contract),
                        const SizedBox(height: 10),
                        Title(
                          title: trip.contract?.referenceNumber ?? l10n.unknown,
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.tripDate),
                        const SizedBox(height: 10),
                        Directionality(
                          textDirection: TextDirection.ltr,
                          child: Title(title: formatDate(trip.startDate, l10n)),
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.km),
                        const SizedBox(height: 10),
                        Title(
                          title:
                              '${_formatCost(trip.contractDistanceKm, l10n)} ${l10n.km}',
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.subTotal),
                        const SizedBox(height: 10),
                        Title(
                          title: trip.tripCostWithoutAddon == null
                              ? l10n.unknown
                              : '${_formatCost(trip.tripCostWithoutAddon, l10n)} ${l10n.egp}',
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.addons),
                        const SizedBox(height: 10),
                        Title(
                          title: trip.totalAddonsCost == null
                              ? l10n.unknown
                              : '${_formatCost(trip.totalAddonsCost, l10n)} ${l10n.egp}',
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.total),
                        const SizedBox(height: 10),
                        Title(
                          title: trip.totalTripCost == null
                              ? l10n.unknown
                              : '${_formatCost(trip.totalTripCost, l10n)} ${l10n.egp}',
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.contactNumber),
                        const SizedBox(height: 10),
                        Directionality(
                          textDirection: TextDirection.ltr,
                          child: Title(
                            title: trip.contactPersonMobile ?? l10n.unknown,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 15),
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
  final bool underline;
  const Title({
    super.key,
    required this.title,
    this.color = AppColors.darkGray,
    this.underline = false,
  });

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: TextStyle(
        fontSize: 14,
        fontWeight: FontWeight.w500,
        color: color,
        decoration: underline ? TextDecoration.underline : null,
      ),
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
