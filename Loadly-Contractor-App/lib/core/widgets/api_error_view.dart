import 'package:flutter/material.dart';

import '../../l10n/gen/app_localizations.dart';
import '../errors/failure_message_mapper.dart';
import '../theme/app_colors.dart';

class ApiErrorView extends StatelessWidget {
  const ApiErrorView({required this.message, this.onRetry, super.key});

  final String message;
  final VoidCallback? onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final displayMessage = FailureMessageMapper.forMessage(message, l10n);

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 16),
      child: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(displayMessage, textAlign: TextAlign.center),
            if (onRetry != null) ...[
              const SizedBox(height: 12),
              TextButton(
                onPressed: onRetry,
                child: Text(
                  l10n.retry,
                  style: const TextStyle(color: AppColors.mainBlue),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
