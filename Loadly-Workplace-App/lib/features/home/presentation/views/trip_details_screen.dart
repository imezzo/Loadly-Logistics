import 'dart:io';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/extensions/image_url_list_converter.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../core/locale/user_facing_message.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/presentation/bloc/trips_bloc.dart';
import '../../../trips/presentation/bloc/trips_event.dart';
import '../../../trips/presentation/bloc/trips_state.dart';
import '../../domain/entities/document.dart';
import '../widgets/not_recived_popup.dart';
import '../../../trips/data/models/collect_trip_documents_request_dto.dart';
import '../../domain/entities/collector_trips.dart' as trips;
import '../bloc/home_bloc.dart';
import '../bloc/home_event.dart';
import '../bloc/home_state.dart';
import '../widgets/recived_popup.dart';

class TripDetailsScreen extends StatefulWidget {
  final trips.CollectorTrips trip;

  const TripDetailsScreen({super.key, required this.trip});

  @override
  State<TripDetailsScreen> createState() => _TripDetailsScreenState();
}

class _TripDetailsScreenState extends State<TripDetailsScreen> {
  bool isFinancialCollector =
      AuthStorage.instance.getisFinancialCollector() ?? false;
  bool isLoading = false;
  bool isInit = false;
  bool submit = false;
  List<CollectTripDocumentsRequestDto> documents = [];
  @override
  void initState() {
    init();
    context.read<HomeBloc>().add(const HomeLoadDocuments());
    super.initState();
  }

  void init() async {
    final tripDocs = widget.trip.collectorDocuments;
    if (tripDocs != null && tripDocs.isNotEmpty) {
      isInit = true;
      for (var i = 0; i < tripDocs.length; i++) {
        final fileUrls = tripDocs[i].fileUrls;
        final files = fileUrls != null ? await fileUrls.toFiles() : <File>[];
        setState(() {
          documents.add(
            CollectTripDocumentsRequestDto(
              tripDocs[i].documentId,
              tripDocs[i].status,
              files,
              tripDocs[i].note,
            ),
          );
        });
      }
      isInit = false;
    }
  }

  String formatDateTime(String isoDate) {
    final dateTime = DateTime.parse(isoDate);
    return DateFormat(
      'dd-MMM-yyyy - h:mm a',
      AppLocalizations.of(context)!.localeName,
    ).format(dateTime);
  }

  void navigateToNotRecivedPopUp({
    int? status,
    String? note,
    required final void Function(int?, String?) submit,
  }) {
    Navigator.of(context).push(
      PageRouteBuilder(
        opaque: false,
        barrierColor: CupertinoColors.black.withValues(alpha: 0.5),
        barrierDismissible: true,
        pageBuilder: (context, animation, secondaryAnimation) {
          return NotRecivedPopup(status: status, note: note, submit: submit);
        },
        transitionsBuilder: (context, animation, secondaryAnimation, child) {
          return ScaleTransition(
            scale: Tween<double>(begin: 0.8, end: 1.0).animate(
              CurvedAnimation(parent: animation, curve: Curves.easeOutBack),
            ),
            child: FadeTransition(opacity: animation, child: child),
          );
        },
      ),
    );
  }

  void navigateToRecivedPopUp({
    List<File>? files,
    required final void Function(List<File>?) submit,
  }) {
    Navigator.of(context).push(
      PageRouteBuilder(
        opaque: false,
        barrierColor: CupertinoColors.black.withValues(alpha: 0.5),
        barrierDismissible: true,
        pageBuilder: (context, animation, secondaryAnimation) {
          return RecivedPopup(files: files ?? [], submit: submit);
        },
        transitionsBuilder: (context, animation, secondaryAnimation, child) {
          return ScaleTransition(
            scale: Tween<double>(begin: 0.8, end: 1.0).animate(
              CurvedAnimation(parent: animation, curve: Curves.easeOutBack),
            ),
            child: FadeTransition(opacity: animation, child: child),
          );
        },
      ),
    );
  }

  Future<void> showSavedDialog({
    required BuildContext context,
    required String title,
    String? body,
    IconData? icon,
  }) {
    showCupertinoDialog(
      context: context,
      barrierDismissible: icon == null ? false : true,
      builder: (context) => CupertinoAlertDialog(
        content: Padding(
          padding: const EdgeInsets.symmetric(vertical: 12),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              // Hourglass icon in circle
              Container(
                width: 90,
                height: 90,
                decoration: const BoxDecoration(
                  color: Color(0xFFF2F2F2),
                  shape: BoxShape.circle,
                ),
                child: Icon(
                  icon ?? CupertinoIcons.hourglass,
                  size: 48,
                  color: CupertinoColors.black,
                ),
              ),
              const SizedBox(height: 20),

              // Title
              Text(
                title,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w700,
                  color: AppColors.darkGray,
                ),
              ),
              const SizedBox(height: 12),

              // Body text
              if (body != null)
                Text(
                  body,
                  style: const TextStyle(
                    fontSize: 14,
                    color: AppColors.darkGray,
                    fontWeight: FontWeight.w400,
                  ),
                ),
            ],
          ),
        ),
      ),
    );

    // Auto close after 3 seconds
    return Future.delayed(const Duration(seconds: 3), () {
      if (context.mounted && icon == null) context.go('/home');
    });
  }

  Future<void> showSendDocumentsDialog(
    BuildContext context, {
    required List<Document> documents,
    required AppLocalizations l10n,
    required VoidCallback onSend,
  }) {
    return showCupertinoModalPopup(
      context: context,
      builder: (context) => Container(
        decoration: const BoxDecoration(
          color: Color(0xFFF7F7F7),
          borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
        ),
        padding: const EdgeInsets.fromLTRB(20, 12, 20, 32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Drag handle
            Container(
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: const Color(0xFFDDDDDD),
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            const SizedBox(height: 24),

            // Green checkmark circle
            Container(
              width: 90,
              height: 90,
              decoration: const BoxDecoration(
                color: Color(0xFFF0F0F0),
                shape: BoxShape.circle,
              ),
              child: Center(
                child: Container(
                  width: 64,
                  height: 64,
                  decoration: const BoxDecoration(
                    color: Color(0xFF1E6B3C),
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    CupertinoIcons.checkmark_alt,
                    color: CupertinoColors.white,
                    size: 34,
                  ),
                ),
              ),
            ),
            const SizedBox(height: 20),

            // Title
            Text(
              isFinancialCollector
                  ? l10n.receiptConfirmation
                  : l10n.saveAndSendDocuments,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w700,
                color: AppColors.darkGray,
              ),
            ),
            const SizedBox(height: 10),

            // Subtitle
            Text(
              isFinancialCollector
                  ? l10n.receiptConfirmedDesc
                  : l10n.saveAndSendDocumentsDesc,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: AppColors.darkGray,
              ),
            ),
            const SizedBox(height: 20),

            // Documents list section
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: Text(
                isFinancialCollector
                    ? l10n.documentsCollectedByFactoryCollector
                    : l10n.theDocumentsThatWillBeSent,
                style: const TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w700,
                  color: AppColors.darkGray,
                ),
              ),
            ),
            const SizedBox(height: 12),

            // Document items
            ...documents.map(
              (document) => Padding(
                padding: const EdgeInsets.symmetric(vertical: 6),
                child: Row(
                  children: [
                    Container(
                      width: 26,
                      height: 26,
                      decoration: const BoxDecoration(
                        color: AppColors.darkGray,
                        shape: BoxShape.circle,
                      ),
                      child: const Icon(
                        CupertinoIcons.checkmark_alt,
                        color: CupertinoColors.white,
                        size: 14,
                      ),
                    ),
                    const SizedBox(width: 10),
                    Text(
                      Directionality.of(context) == TextDirection.rtl
                          ? (document.nameAr ?? document.nameEn ?? '')
                          : (document.nameEn ?? document.nameAr ?? ''),
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w400,
                        color: AppColors.darkGray,
                      ),
                    ),
                  ],
                ),
              ),
            ),

            const SizedBox(height: 24),

            // Send button
            SizedBox(
              width: double.infinity,
              child: AppPrimaryButton(
                onPressed: () {
                  Navigator.of(context).pop();
                  onSend();
                },
                label: isFinancialCollector
                    ? l10n.receiptConfirmation
                    : l10n.sendDocuments,
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _showEdit({
    required int docId,
    required int docIndex,
    required CollectTripDocumentsRequestDto doc,
  }) {
    final l10n = AppLocalizations.of(context)!;

    showCupertinoModalPopup<void>(
      context: context,
      builder: (ctx) => CupertinoActionSheet(
        actions: [
          CupertinoActionSheetAction(
            onPressed: () {
              Navigator.of(ctx).pop();
              navigateToRecivedPopUp(
                files: doc.files,
                submit: (files) => setState(() {
                  documents[docIndex] = CollectTripDocumentsRequestDto(
                    docId,
                    isFinancialCollector ? 8 : 1,
                    files,
                    null,
                  );
                }),
              );
            },
            child: Text(
              isFinancialCollector ? l10n.receiptConfirmation : l10n.received,
              style: const TextStyle(color: AppColors.mainBlue),
            ),
          ),
          CupertinoActionSheetAction(
            onPressed: () {
              Navigator.of(ctx).pop();
              navigateToNotRecivedPopUp(
                status: doc.status,
                note: doc.note,
                submit: (status, note) => setState(() {
                  documents[docIndex] = CollectTripDocumentsRequestDto(
                    docId,
                    status,
                    [],
                    note,
                  );
                }),
              );
            },
            child: Text(
              l10n.notReceived,
              style: const TextStyle(color: AppColors.mainBlue),
            ),
          ),
        ],
        cancelButton: CupertinoActionSheetAction(
          isDefaultAction: true,
          onPressed: () => Navigator.of(ctx).pop(),
          child: Text(l10n.cancel),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final List<Status> statuses = [
      Status(
        name: AppLocalizations.of(context)!.received,
        code: 1,
        color: CupertinoColors.activeGreen,
        icon: CupertinoIcons.checkmark_circle_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.stillWithCustomer,
        code: 2,
        color: CupertinoColors.activeOrange,
        icon: CupertinoIcons.xmark_circle_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.lostDocument,
        code: 3,
        color: CupertinoColors.systemRed,
        icon: CupertinoIcons.xmark_circle_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.tripDoesNotExist,
        code: 4,
        color: CupertinoColors.systemGrey,
        icon: CupertinoIcons.exclamationmark_circle_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.willBeSentLater,
        code: 5,
        color: CupertinoColors.systemTeal,
        icon: CupertinoIcons.xmark_circle_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.customerRefusedToSign,
        code: 6,
        color: CupertinoColors.systemIndigo,
        icon: CupertinoIcons.xmark_circle_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.other,
        code: 7,
        color: CupertinoColors.systemMint,
        icon: CupertinoIcons.xmark_circle_fill,
      ),
      Status(
        name: AppLocalizations.of(context)!.confirmed,
        code: 8,
        color: CupertinoColors.systemGreen,
        icon: CupertinoIcons.checkmark_circle_fill,
      ),
    ];
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) context.go('/home');
      },
      child: BlocListener<TripsBloc, TripsState>(
        listener: (context, state) async {
          if (state is CollectDocumentsLoading ||
              state is ConfirmDocumentsLoading) {
            setState(() {
              isLoading = true;
            });
          }
          if (state is CollectDocumentsDone) {
            setState(() {
              isLoading = false;
            });
            if (state.error != null) {
              await showSavedDialog(
                context: context,
                title: localizeUserMessage(l10n, state.error),
                icon: CupertinoIcons.exclamationmark_triangle_fill,
              );
            } else {
              if (submit) {
                await showSavedDialog(context: context, title: l10n.saved);
              } else {
                await showSavedDialog(
                  context: context,
                  title: l10n.saved,
                  body: l10n.savedDesc,
                );
              }
            }
          }
          if (state is ConfirmDocumentsDone) {
            setState(() {
              isLoading = false;
            });
            if (state.error != null) {
              if (context.mounted) {
                await showSavedDialog(
                  context: context,
                  title: localizeUserMessage(l10n, state.error),
                  icon: CupertinoIcons.exclamationmark_triangle_fill,
                );
              }
            } else {
              if (submit) {
                if (context.mounted) {
                  await showSavedDialog(context: context, title: l10n.saved);
                }
              } else {
                if (context.mounted) {
                  await showSavedDialog(
                    context: context,
                    title: l10n.saved,
                    body: l10n.savedDesc,
                  );
                }
              }
            }
          }
        },
        child: CupertinoPageScaffold(
          backgroundColor: AppColors.mainBlue,
          navigationBar: CupertinoNavigationBar(
            backgroundColor: AppColors.mainBlue,
            brightness: Brightness.dark,
            leading: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                AppBackButton(onPressed: () => context.go('/home')),
                const SizedBox(width: 8),
                Text(
                  '${l10n.trip}: ${widget.trip.id}',
                  style: const TextStyle(
                    color: AppColors.white,
                    fontWeight: FontWeight.w600,
                    fontSize: 17,
                  ),
                ),
              ],
            ),
          ),
          child: MainScaffold(
            child: ClipRRect(
              borderRadius: AppLayout.authContentBorderRadius,
              child: Container(
                padding: const EdgeInsets.all(15),
                color: AppColors.white,
                child: SingleChildScrollView(
                  child: Column(
                    children: [
                      Container(
                        decoration: BoxDecoration(
                          color: AppColors.white,
                          borderRadius: BorderRadius.circular(20),
                          border: Border.all(
                            color: const Color(0xFFE5E9EF),
                            width: 1,
                          ),
                        ),
                        padding: const EdgeInsets.all(16),
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            // Header: Trip number + Driver
                            _buildHeader(context),
                            const SizedBox(height: 10),
                            // Main content card
                            _buildContentCard(context),
                          ],
                        ),
                      ),
                      const SizedBox(height: 10),
                      BlocBuilder<HomeBloc, HomeState>(
                        builder: (context, state) {
                          if (state is DocumentsLoading || isInit) {
                            return Padding(
                              padding: EdgeInsets.symmetric(
                                vertical:
                                    MediaQuery.sizeOf(context).height * 0.25,
                              ),
                              child: const Center(
                                child: CupertinoActivityIndicator(
                                  color: AppColors.mainBlue,
                                  radius: 16,
                                ),
                              ),
                            );
                          }

                          if (state is DocumentsLoaded) {
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
                                        localizeUserMessage(l10n, state.error),
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
                                          context.read<HomeBloc>().add(
                                            const HomeLoadDocuments(),
                                          );
                                        },
                                      ),
                                    ],
                                  ),
                                ),
                              );
                            }

                            if (state.documentsList.isEmpty) {
                              return Padding(
                                padding: const EdgeInsets.symmetric(
                                  vertical: 5,
                                ),
                                child: ClipRRect(
                                  borderRadius:
                                      AppLayout.authContentBorderRadius,
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
                                              l10n.noDocumentsAvailable,
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
                            final filteredDocuments =
                                widget.trip.hasCustoms ?? false
                                ? state.documentsList
                                      .where(
                                        (document) =>
                                            document.nameEn != "Fuel Receipts",
                                      )
                                      .toList()
                                : state.documentsList
                                      .where(
                                        (document) =>
                                            document.nameEn !=
                                                "Customs Documents" &&
                                            document.nameEn != "Fuel Receipts",
                                      )
                                      .toList();
                            return Column(
                              children: [
                                ListView.separated(
                                  shrinkWrap: true,
                                  physics: const NeverScrollableScrollPhysics(),
                                  itemBuilder: (context, index) => Container(
                                    padding: const EdgeInsets.all(10),
                                    decoration: BoxDecoration(
                                      borderRadius: BorderRadius.circular(10),
                                      color: const Color(
                                        0xFFE5E9EF,
                                      ).withValues(alpha: 0.2),
                                    ),
                                    child:
                                        documents.hasDocumentId(
                                          filteredDocuments[index].id!,
                                        )
                                        ? Builder(
                                            builder: (context) {
                                              final doc = documents.findById(
                                                filteredDocuments[index].id!,
                                              );
                                              final docIndex = documents
                                                  .indexOf(doc!);
                                              final status = statuses
                                                  .firstWhere(
                                                    (element) =>
                                                        element.code ==
                                                        doc.status,
                                                  );
                                              return Column(
                                                crossAxisAlignment:
                                                    CrossAxisAlignment.start,
                                                children: [
                                                  Row(
                                                    children: [
                                                      Icon(
                                                        status.icon,
                                                        color: status.color,
                                                      ),
                                                      const SizedBox(width: 5),
                                                      Text(
                                                        Directionality.of(
                                                                  context,
                                                                ) ==
                                                                TextDirection
                                                                    .rtl
                                                            ? (filteredDocuments[index]
                                                                      .nameAr ??
                                                                  filteredDocuments[index]
                                                                      .nameEn ??
                                                                  '')
                                                            : (filteredDocuments[index]
                                                                      .nameEn ??
                                                                  filteredDocuments[index]
                                                                      .nameAr ??
                                                                  ''),
                                                        style: const TextStyle(
                                                          fontSize: 14,
                                                          fontWeight:
                                                              FontWeight.w400,
                                                          color: AppColors
                                                              .darkGray,
                                                        ),
                                                      ),
                                                      const Spacer(),
                                                      TextButton.icon(
                                                        onPressed: () => _showEdit(
                                                          doc: doc,
                                                          docIndex: docIndex,
                                                          docId:
                                                              filteredDocuments[index]
                                                                  .id!,
                                                        ),
                                                        label: Text(l10n.edit),
                                                        icon: const Icon(
                                                          CupertinoIcons
                                                              .pencil_ellipsis_rectangle,
                                                        ),
                                                        iconAlignment:
                                                            IconAlignment.end,
                                                      ),
                                                    ],
                                                  ),
                                                  const SizedBox(height: 10),
                                                  Row(
                                                    mainAxisAlignment:
                                                        MainAxisAlignment
                                                            .spaceBetween,
                                                    children: [
                                                      DocumentButton(
                                                        onPressed: () {},
                                                        padding:
                                                            const EdgeInsets.symmetric(
                                                              vertical: 5,
                                                              horizontal: 20,
                                                            ),
                                                        label: status.name,
                                                        color: status.color,
                                                      ),
                                                      if (isFinancialCollector &&
                                                          doc.status == 1)
                                                        DocumentButton(
                                                          onPressed: () {
                                                            setState(() {
                                                              documents[docIndex] =
                                                                  CollectTripDocumentsRequestDto(
                                                                    filteredDocuments[index]
                                                                        .id!,
                                                                    8,
                                                                    doc.files,
                                                                    doc.note,
                                                                  );
                                                            });
                                                          },
                                                          padding:
                                                              const EdgeInsets.symmetric(
                                                                vertical: 5,
                                                                horizontal: 20,
                                                              ),
                                                          label: l10n
                                                              .receiptConfirmationWithMark,
                                                          color: CupertinoColors
                                                              .systemGrey,
                                                        ),
                                                    ],
                                                  ),
                                                  if (isFinancialCollector &&
                                                      doc.status == 1) ...[
                                                    const SizedBox(height: 5),
                                                    Row(
                                                      children: [
                                                        const Icon(
                                                          CupertinoIcons.person,
                                                          size: 15,
                                                        ),
                                                        const SizedBox(
                                                          width: 5,
                                                        ),
                                                        Text(
                                                          l10n.factoryCollector,
                                                          style:
                                                              const TextStyle(
                                                                fontSize: 12,

                                                                color: AppColors
                                                                    .darkGray,
                                                              ),
                                                        ),
                                                      ],
                                                    ),
                                                  ],
                                                  if (doc.status == 7 &&
                                                      doc.note != null &&
                                                      doc.note!.isNotEmpty) ...[
                                                    const SizedBox(height: 5),
                                                    Text(
                                                      doc.note!,
                                                      style: const TextStyle(
                                                        fontSize: 12,

                                                        color:
                                                            AppColors.darkGray,
                                                      ),
                                                    ),
                                                  ],
                                                  if (doc.files != null &&
                                                      doc.files!.isNotEmpty)
                                                    Column(
                                                      crossAxisAlignment:
                                                          CrossAxisAlignment
                                                              .start,
                                                      children: [
                                                        const SizedBox(
                                                          height: 10,
                                                        ),
                                                        Text(
                                                          l10n.documentsPictures,
                                                          style:
                                                              const TextStyle(
                                                                fontSize: 12,
                                                                fontWeight:
                                                                    FontWeight
                                                                        .w700,
                                                                color: AppColors
                                                                    .darkGray,
                                                              ),
                                                        ),
                                                        Container(
                                                          width:
                                                              double.infinity,
                                                          height: 150,
                                                          padding:
                                                              const EdgeInsets.all(
                                                                10,
                                                              ),
                                                          decoration: BoxDecoration(
                                                            borderRadius:
                                                                BorderRadius.circular(
                                                                  12,
                                                                ),
                                                          ),
                                                          child: ListView.separated(
                                                            scrollDirection:
                                                                Axis.horizontal,
                                                            itemBuilder: (context, index) => Column(
                                                              children: [
                                                                ClipRRect(
                                                                  borderRadius:
                                                                      BorderRadius.circular(
                                                                        10,
                                                                      ),
                                                                  child: TweenAnimationBuilder<double>(
                                                                    tween: Tween(
                                                                      begin:
                                                                          0.0,
                                                                      end: 1.0,
                                                                    ),
                                                                    duration:
                                                                        const Duration(
                                                                          seconds:
                                                                              1,
                                                                        ),
                                                                    curve: Curves
                                                                        .easeIn,
                                                                    builder:
                                                                        (
                                                                          context,
                                                                          value,
                                                                          child,
                                                                        ) {
                                                                          return Opacity(
                                                                            opacity:
                                                                                value,
                                                                            child: Image.file(
                                                                              doc.files![index],
                                                                              width: 100,
                                                                              height: 100,
                                                                              fit: BoxFit.cover,
                                                                            ),
                                                                          );
                                                                        },
                                                                  ),
                                                                ),
                                                                const SizedBox(
                                                                  height: 10,
                                                                ),
                                                                GestureDetector(
                                                                  onTap: () => setState(() {
                                                                    doc.files!
                                                                        .removeAt(
                                                                          index,
                                                                        );
                                                                  }),
                                                                  child: const Icon(
                                                                    CupertinoIcons
                                                                        .delete,
                                                                    color: CupertinoColors
                                                                        .systemRed,
                                                                    size: 20,
                                                                  ),
                                                                ),
                                                              ],
                                                            ),
                                                            separatorBuilder:
                                                                (
                                                                  context,
                                                                  index,
                                                                ) =>
                                                                    const SizedBox(
                                                                      width: 5,
                                                                    ),
                                                            itemCount: doc
                                                                .files!
                                                                .length,
                                                          ),
                                                        ),
                                                      ],
                                                    ),
                                                ],
                                              );
                                            },
                                          )
                                        : Column(
                                            crossAxisAlignment:
                                                CrossAxisAlignment.start,
                                            children: [
                                              Text(
                                                Directionality.of(context) ==
                                                        TextDirection.rtl
                                                    ? (filteredDocuments[index]
                                                              .nameAr ??
                                                          filteredDocuments[index]
                                                              .nameEn ??
                                                          '')
                                                    : (filteredDocuments[index]
                                                              .nameEn ??
                                                          filteredDocuments[index]
                                                              .nameAr ??
                                                          ''),
                                                style: const TextStyle(
                                                  fontSize: 14,
                                                  fontWeight: FontWeight.w400,
                                                  color: AppColors.darkGray,
                                                ),
                                              ),
                                              const SizedBox(height: 10),
                                              Row(
                                                mainAxisAlignment:
                                                    MainAxisAlignment
                                                        .spaceEvenly,
                                                children: [
                                                  DocumentButton(
                                                    padding:
                                                        const EdgeInsets.symmetric(
                                                          vertical: 5,
                                                          horizontal: 20,
                                                        ),
                                                    onPressed: () => navigateToRecivedPopUp(
                                                      submit: (files) => setState(() {
                                                        documents.add(
                                                          CollectTripDocumentsRequestDto(
                                                            filteredDocuments[index]
                                                                .id!,
                                                            isFinancialCollector
                                                                ? 8
                                                                : 1,
                                                            files,
                                                            null,
                                                          ),
                                                        );
                                                      }),
                                                    ),
                                                    label: l10n.received,
                                                    color: CupertinoColors
                                                        .activeGreen,
                                                  ),
                                                  DocumentButton(
                                                    padding:
                                                        const EdgeInsets.symmetric(
                                                          vertical: 5,
                                                          horizontal: 20,
                                                        ),
                                                    onPressed: () =>
                                                        navigateToNotRecivedPopUp(
                                                          submit:
                                                              (
                                                                status,
                                                                note,
                                                              ) => setState(() {
                                                                documents.add(
                                                                  CollectTripDocumentsRequestDto(
                                                                    filteredDocuments[index]
                                                                        .id!,
                                                                    status,
                                                                    [],
                                                                    note,
                                                                  ),
                                                                );
                                                              }),
                                                        ),
                                                    label: l10n.notReceived,
                                                    color: CupertinoColors
                                                        .systemRed,
                                                  ),
                                                ],
                                              ),
                                            ],
                                          ),
                                  ),
                                  separatorBuilder: (context, index) =>
                                      const SizedBox(height: 10),
                                  itemCount: filteredDocuments.length,
                                ),
                                const SizedBox(height: 15),
                                SizedBox(
                                  width: double.infinity,
                                  child: AppPrimaryButton(
                                    loading: isLoading,
                                    onPressed: () {
                                      submit = true;
                                      final receivedDocs =
                                          documents.findByStatus(
                                            isFinancialCollector ? 8 : 1,
                                          ) ??
                                          [];
                                      showSendDocumentsDialog(
                                        context,
                                        l10n: l10n,
                                        documents: receivedDocs
                                            .map(
                                              (e) => state.documentsList
                                                  .firstWhere(
                                                    (element) =>
                                                        element.id ==
                                                        e.documentId,
                                                  ),
                                            )
                                            .toList(),
                                        onSend: () {
                                          if (isFinancialCollector) {
                                            final receivedDocss =
                                                documents.findByStatus(1) ?? [];
                                            if (receivedDocss.isNotEmpty) {
                                              for (final doc in receivedDocss) {
                                                documents[documents.indexOf(
                                                      doc,
                                                    )] =
                                                    CollectTripDocumentsRequestDto(
                                                      doc.documentId,
                                                      8,
                                                      doc.files,
                                                      doc.note,
                                                    );
                                              }
                                            }
                                          }
                                          if (isFinancialCollector ||
                                              (!isFinancialCollector &&
                                                  documents.isEmpty)) {
                                            for (final doc
                                                in filteredDocuments) {
                                              if (!documents.hasDocumentId(
                                                doc.id!,
                                              )) {
                                                documents.add(
                                                  CollectTripDocumentsRequestDto(
                                                    doc.id!,
                                                    isFinancialCollector
                                                        ? 8
                                                        : 1,
                                                    [],
                                                    null,
                                                  ),
                                                );
                                              }
                                            }
                                          }
                                          context.read<TripsBloc>().add(
                                            isFinancialCollector
                                                ? TripsConfirmDocuments(
                                                    tripId: widget.trip.id!,
                                                    collectorDocuments:
                                                        documents,
                                                    submit: submit,
                                                  )
                                                : TripsCollectDocuments(
                                                    tripId: widget.trip.id!,
                                                    collectorDocuments:
                                                        documents,
                                                    submit: submit,
                                                  ),
                                          );
                                        },
                                      );
                                    },
                                    label: isFinancialCollector
                                        ? l10n.confirmAllDocuments
                                        : l10n.saveAndSendDocuments,
                                  ),
                                ),
                                const SizedBox(height: 10),
                                SizedBox(
                                  width: double.infinity,
                                  child: AppSecondaryButton(
                                    isLoading: isLoading,
                                    onPressed: () {
                                      submit = false;
                                      context.read<TripsBloc>().add(
                                        isFinancialCollector
                                            ? TripsConfirmDocuments(
                                                tripId: widget.trip.id!,
                                                collectorDocuments: documents,
                                                submit: submit,
                                              )
                                            : TripsCollectDocuments(
                                                tripId: widget.trip.id!,
                                                collectorDocuments: documents,
                                                submit: submit,
                                              ),
                                      );
                                    },
                                    label: isFinancialCollector
                                        ? l10n.confirmAndContinueLater
                                        : l10n.saveAndContinueLater,
                                  ),
                                ),
                              ],
                            );
                          }
                          return const SizedBox.shrink();
                        },
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildHeader(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        _buildDriverAvatar(),
        const SizedBox(width: 5),
        Row(
          children: [
            SizedBox(
              width: MediaQuery.sizeOf(context).width * 0.45,
              child: Text(
                Directionality.of(context) == TextDirection.rtl
                    ? widget.trip.driver?.fullNameAr ??
                          widget.trip.driver?.fullName ??
                          l10n.noDriver
                    : widget.trip.driver?.fullNameEn ??
                          widget.trip.driver?.fullName ??
                          l10n.noDriver,
                style: const TextStyle(
                  fontSize: 17,
                  fontWeight: FontWeight.bold,
                  color: AppColors.darkGray,
                ),
                overflow: TextOverflow.ellipsis,
              ),
            ),
          ],
        ),
        // Trip number pill
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 10),
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(30),
          ),
          child: Text(
            '${AppLocalizations.of(context)!.trip}: ${widget.trip.id}',
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w400,
              color: AppColors.mainBlue,
            ),
          ),
        ),

        // Driver info
      ],
    );
  }

  Widget _buildDriverAvatar() {
    return const Icon(
      CupertinoIcons.person_crop_circle_fill,
      color: Color(0xFFE5E9EF),
      size: 30,
    );
  }

  Widget _buildContentCard(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Car plate
        Text(
          '${widget.trip.vehicle?.plateLetters} ${widget.trip.vehicle?.plateNo}',
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w700,
            color: AppColors.darkGray,
          ),
        ),
        const SizedBox(height: 10),

        // Company name with icon
        Row(
          mainAxisAlignment: MainAxisAlignment.start,
          children: [
            // Medicine bottle icon placeholder
            const Icon(
              CupertinoIcons.square_favorites_fill,
              color: Color(0xFFE5E9EF),
            ),
            const SizedBox(width: 5),
            SizedBox(
              width: MediaQuery.sizeOf(context).width * 0.6,
              child: Text(
                widget.trip.company == null
                    ? AppLocalizations.of(context)!.externalTrip
                    : Directionality.of(context) == TextDirection.rtl
                    ? widget.trip.company!.nameAr ??
                          widget.trip.company!.fullName!
                    : widget.trip.company!.nameEn ??
                          widget.trip.company!.fullName!,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w400,
                  color: AppColors.darkGray,
                ),
                overflow: TextOverflow.ellipsis,
              ),
            ),
          ],
        ),
        const SizedBox(height: 10),

        // Trip date/time
        Row(
          mainAxisAlignment: MainAxisAlignment.start,
          children: [
            Text(
              '${AppLocalizations.of(context)!.tripDateIs} :',
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: CupertinoColors.systemGrey,
              ),
            ),
            const SizedBox(width: 5),
            Text(
              formatDateTime(widget.trip.startDate!),
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: CupertinoColors.systemGrey,
              ),
            ),
          ],
        ),
        const SizedBox(height: 10),
        if (widget.trip.waypoints != null && widget.trip.waypoints!.isNotEmpty)
          // Route: Origin ← Destination
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              // Destination (left side in RTL = arrival)
              _buildLocationBlock(
                name: widget.trip.waypoints!.first.addressName!,
                date: formatDateTime(widget.trip.startDate!),
                context: context,
              ),
              // Arrow
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 8),
                child: Icon(
                  Directionality.of(context) == TextDirection.rtl
                      ? CupertinoIcons.arrow_left
                      : CupertinoIcons.arrow_right,
                  color: AppColors.darkGray,
                  size: 22,
                ),
              ),
              // Origin (right side in RTL = departure)
              _buildLocationBlock(
                name: widget.trip.waypoints!.last.addressName!,
                date: formatDateTime(widget.trip.endDate!),
                context: context,
              ),
            ],
          ),
      ],
    );
  }

  Widget _buildLocationBlock({
    required String name,
    required String date,
    required BuildContext context,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.start,
          children: [
            const Icon(
              CupertinoIcons.location_solid,
              size: 14,
              fontWeight: FontWeight.w400,
              color: CupertinoColors.systemGrey,
            ),
            const SizedBox(width: 4),
            SizedBox(
              width: MediaQuery.sizeOf(context).width * 0.25,
              child: Text(
                name,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w400,
                  color: AppColors.darkGray,
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: 4),
        Align(
          alignment: AlignmentDirectional.centerStart,
          child: Text(
            date,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w400,
              color: CupertinoColors.systemGrey,
            ),
          ),
        ),
      ],
    );
  }
}

class DocumentButton extends StatelessWidget {
  const DocumentButton({
    super.key,
    required this.onPressed,
    this.label,
    this.loading = false,
    this.padding,
    this.minSize = kMinInteractiveDimensionCupertino,
    this.color,
  }) : assert(label != null, 'Provide either child or label');

  final VoidCallback? onPressed;
  final String? label;
  final bool loading;
  final EdgeInsetsGeometry? padding;
  final double minSize;
  final Color? color;

  @override
  Widget build(BuildContext context) {
    return CupertinoButton(
      padding:
          padding ?? const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
      borderRadius: BorderRadius.circular(50),
      minimumSize: Size(minSize, minSize),
      color: color?.withValues(alpha: 0.2) ?? AppColors.mainBlue,
      disabledColor: AppColors.mainBlue.withValues(alpha: 0.5),
      onPressed: loading ? null : onPressed,
      child: loading
          ? const CupertinoActivityIndicator(color: AppColors.white)
          : DefaultTextStyle(
              style: TextStyle(
                color: color ?? AppColors.white,
                fontSize: 16,
                fontWeight: FontWeight.w500,
              ),
              child: Text(label!),
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
