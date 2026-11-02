import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/invoice_payment_request.dart';

class InvoicePaymentRequestDetailsScreen extends StatelessWidget {
  final InvoicePaymentRequest invoicePaymentRequest;
  const InvoicePaymentRequestDetailsScreen({
    super.key,
    required this.invoicePaymentRequest,
  });

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
        name: AppLocalizations.of(context)!.pending,
        code: 1,
        color: CupertinoColors.activeOrange,
        icon: CupertinoIcons.slowmo,
      ),
      Status(
        name: AppLocalizations.of(context)!.received,
        code: 2,
        color: CupertinoColors.activeGreen,
        icon: CupertinoIcons.checkmark_alt,
      ),
      Status(
        name: AppLocalizations.of(context)!.rejected,
        code: 3,
        color: CupertinoColors.systemRed,
        icon: CupertinoIcons.xmark,
      ),
    ];
    Status status;
    try {
      status = statuses.firstWhere(
        (element) => element.code == invoicePaymentRequest.status,
      );
    } catch (e) {
      status = statuses[0];
    }

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go('/invoice-payment-requests');
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
                onPressed: () => context.go('/invoice-payment-requests'),
              ),
              const SizedBox(height: 10),
              Text(
                '${l10n.invoicePaymentRequest}: ${invoicePaymentRequest.id ?? l10n.unknown}',
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                  color: AppColors.darkGray,
                ),
              ),
              const SizedBox(height: 10),
              Container(
                height: 250,
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
                          title:
                              '${invoicePaymentRequest.invoiceId ?? l10n.unknown}',
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.invoiceNumber),
                        const SizedBox(height: 10),
                        Title(
                          title:
                              invoicePaymentRequest.invoiceNumber ??
                              l10n.unknown,
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.requestDate),
                        const SizedBox(height: 10),
                        Directionality(
                          textDirection: TextDirection.ltr,
                          child: Title(
                            title: invoicePaymentRequest.requestDate == null
                                ? l10n.unknown
                                : formatDate(
                                    invoicePaymentRequest.requestDate!,
                                  ),
                          ),
                        ),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Subtitle(title: l10n.amount),
                        const SizedBox(height: 10),
                        Title(
                          title:
                              '${invoicePaymentRequest.amount == null ? l10n.unknown : _formatCost(invoicePaymentRequest.amount!)} ${l10n.egp}',
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
