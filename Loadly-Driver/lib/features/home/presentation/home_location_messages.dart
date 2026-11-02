import '../../../l10n/gen/app_localizations.dart';
import 'bloc/home_location_issue.dart';

String messageForHomeLocationIssue(
  AppLocalizations l10n,
  HomeLocationIssue issue,
) {
  return switch (issue.kind) {
    HomeLocationIssueKind.servicesDisabled => l10n.homeLocationServicesDisabled,
    HomeLocationIssueKind.permissionDenied => l10n.homeLocationPermissionDenied,
    HomeLocationIssueKind.permissionDeniedForever =>
      l10n.homeLocationPermissionDeniedForever,
    HomeLocationIssueKind.getPositionFailed =>
      l10n.homeLocationGetPositionFailed,
    HomeLocationIssueKind.getInitialPositionFailed =>
      l10n.homeLocationGetInitialFailed,
    HomeLocationIssueKind.streamError => l10n.homeLocationStreamError,
    HomeLocationIssueKind.unavailable => l10n.homeLocationUnavailable,
  };
}
