import 'package:flutter/cupertino.dart';

import '../../l10n/gen/app_localizations.dart';
import '../theme/app_colors.dart';
import '../theme/app_theme.dart';

typedef CupertinoLookupPickerOption = ({int? id, String label});

/// Shows a wheel picker for lookup items (e.g. vehicle type filters).
/// First option is typically "All" with [CupertinoLookupPickerOption.id] == null.
Future<int?> showCupertinoLookupPicker({
  required BuildContext context,
  required List<CupertinoLookupPickerOption> options,
  int? selectedId,
}) {
  if (options.isEmpty) {
    return Future.value(selectedId);
  }

  final initialIndex = lookupPickerInitialIndex(
    options: options,
    selectedId: selectedId,
  );
  final l10n = AppLocalizations.of(context)!;

  return showCupertinoModalPopup<int?>(
    context: context,
    builder: (sheetContext) {
      var selectedIndex = initialIndex;

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
                  onPressed: () => Navigator.pop(
                    sheetContext,
                    options[selectedIndex].id,
                  ),
                  child: Text(l10n.confirm),
                ),
              ],
            ),
            Expanded(
              child: CupertinoPicker(
                scrollController: FixedExtentScrollController(
                  initialItem: initialIndex,
                ),
                itemExtent: 36,
                onSelectedItemChanged: (index) => selectedIndex = index,
                children: [
                  for (final option in options)
                    Center(
                      child: Text(
                        option.label,
                        textAlign: TextAlign.center,
                        style: const TextStyle(
                          fontSize: 16,
                          fontFamily: appFontFamily,
                          color: AppColors.darkGray,
                        ),
                      ),
                    ),
                ],
              ),
            ),
          ],
        ),
      );
    },
  );
}

int lookupPickerInitialIndex({
  required List<CupertinoLookupPickerOption> options,
  int? selectedId,
}) {
  if (selectedId == null) return 0;
  final idx = options.indexWhere((o) => o.id == selectedId);
  return idx >= 0 ? idx : 0;
}

List<CupertinoLookupPickerOption> vehicleTypeFilterOptions({
  required String allLabel,
  required Iterable<({int id, String label})> vehicleTypes,
}) {
  return [
    (id: null, label: allLabel),
    for (final item in vehicleTypes) (id: item.id, label: item.label),
  ];
}
