import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../l10n/gen/app_localizations.dart';
import '../locale/bloc/locale_bloc.dart';
import '../locale/bloc/locale_event.dart';
import '../locale/bloc/locale_state.dart';
import '../theme/app_colors.dart';

/// Globe icon button that opens language picker (AR / EN).
class LanguageSwitcher extends StatelessWidget {
  const LanguageSwitcher({super.key});

  static const _languages = ['ar', 'en'];
  static const _labels = {'ar': 'العربية', 'en': 'English'};

  void _showLanguagePicker(BuildContext context) {
    final localeState = context.read<LocaleBloc>().state;
    final currentCode = localeState is LocaleLoaded
        ? localeState.locale.languageCode
        : localeState is LocaleInitial
        ? localeState.locale.languageCode
        : 'ar';

    showCupertinoModalPopup<void>(
      context: context,
      builder: (context) => CupertinoActionSheet(
        actions: [
          for (final code in _languages)
            CupertinoActionSheetAction(
              onPressed: () {
                Navigator.of(context).pop();
                if (code != currentCode) {
                  context.read<LocaleBloc>().add(LocaleChanged(code));
                }
              },
              child: Text(
                _labels[code]!,
                style: TextStyle(
                  fontWeight: code == currentCode ? FontWeight.w600 : null,
                  color: code == currentCode
                      ? AppColors.mainBlue
                      : CupertinoColors.activeBlue,
                ),
              ),
            ),
        ],
        cancelButton: CupertinoActionSheetAction(
          isDefaultAction: true,
          onPressed: () => Navigator.of(context).pop(),
          child: Text(AppLocalizations.of(context)!.cancel),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return CupertinoButton(
      padding: const EdgeInsets.all(6),
      minimumSize: Size.zero,
      onPressed: () => _showLanguagePicker(context),
      child: Text(
        AppLocalizations.of(context)!.switchLanguage,
        style: const TextStyle(
          fontSize: 14,
          color: AppColors.mainBlue,
          fontWeight: FontWeight.w400,
          decoration: TextDecoration.underline,
        ),
      ),
    );
  }
}
