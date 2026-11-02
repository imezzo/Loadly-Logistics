import 'dart:io';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart'
    show RefreshIndicator, AlwaysScrollableScrollPhysics;
import 'package:device_info_plus/device_info_plus.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';
import 'package:permission_handler/permission_handler.dart';

import '../../../../core/permissions/prominent_user_data_disclosure.dart';
import '../../../../core/locale/bloc/locale_bloc.dart';
import '../../../../core/services/biometric_service.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/locale/bloc/locale_event.dart';
import '../../../../core/locale/bloc/locale_state.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../core/widgets/app_version.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_phone_input.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_event.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../auth/domain/entities/driver_profile.dart';
import '../../../auth/presentation/utils/auth_error_extensions.dart';
import '../../../home/presentation/bloc/home_bloc.dart';
import '../../../home/presentation/bloc/home_event.dart';
import '../bloc/profile_bloc.dart';
import '../bloc/profile_event.dart';
import '../bloc/profile_state.dart';

/// Android 13+ (API 33): system photo picker — no READ_MEDIA_IMAGES (Play policy).
Future<int?> _androidSdkInt() async {
  if (!Platform.isAndroid) return null;
  return (await DeviceInfoPlugin().androidInfo).version.sdkInt;
}

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  int _selectedTab = 0; // 0 = Personal Data, 1 = Files
  bool _fingerprintEnabled = false;
  bool _twoFAEnabled = false;
  bool _biometricAvailable = false;

  void _handleBackNavigation() => context.go('/home');

  static String? _loginFromProfile(DriverProfile? profile) {
    if (profile == null) return null;
    final phone = profile.phone?.trim();
    if (phone != null && phone.isNotEmpty) {
      final digits = extractDigits(phone);
      if (digits.length >= 10) {
        final normalized = digits.startsWith('2') ? digits : '2$digits';
        return normalized.startsWith('2') ? '+$normalized' : '+2$normalized';
      }
    }
    final email = profile.email.trim();
    return email.isNotEmpty ? email : null;
  }

  void _showEnableBiometricPasswordDialog({
    required BuildContext context,
    required String login,
    required AppLocalizations l10n,
    required VoidCallback onCancel,
  }) {
    final passwordController = TextEditingController();
    showCupertinoDialog<void>(
      context: context,
      builder: (ctx) => CupertinoAlertDialog(
        title: Text(l10n.enableFingerprint),
        content: Padding(
          padding: const EdgeInsets.only(top: 12),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                l10n.biometricEnablePasswordDialogMessage,
                style: const TextStyle(fontSize: 14, color: AppColors.darkGray),
              ),
              const SizedBox(height: 12),
              CupertinoTextField(
                controller: passwordController,
                placeholder: l10n.password,
                obscureText: true,
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 10,
                ),
                decoration: BoxDecoration(
                  border: Border.all(color: AppColors.lightGray),
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
            ],
          ),
        ),
        actions: [
          CupertinoDialogAction(
            onPressed: () {
              passwordController.dispose();
              Navigator.of(ctx).pop();
              onCancel();
            },
            child: Text(l10n.cancel),
          ),
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () {
              final password = passwordController.text;
              passwordController.dispose();
              Navigator.of(ctx).pop();
              if (password.isEmpty) {
                onCancel();
                return;
              }
              context.read<AuthBloc>().add(
                AuthValidatePasswordAndEnableBiometricRequested(
                  login: login,
                  password: password,
                  reason: l10n.biometricLocalizedReasonEnable,
                ),
              );
            },
            child: Text(l10n.ok),
          ),
        ],
      ),
    );
  }

  @override
  void initState() {
    super.initState();
    // Load profile data after the first frame to ensure context has access to BlocProvider
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      if (mounted) {
        context.read<ProfileBloc>().add(const ProfileLoadRequested());
        final isAvailable = await BiometricService.instance.isAvailable();
        // final isEnabled = await BiometricService.instance.isBiometricEnabled();
        if (mounted) {
          setState(() {
            _biometricAvailable = isAvailable;
            // _fingerprintEnabled = isEnabled;
          });
        }
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return MultiBlocListener(
      listeners: [
        BlocListener<ProfileBloc, ProfileState>(
          listener: (context, profileState) async {
            // Update HomeBloc with isOnline status when profile is loaded
            if (profileState is ProfileLoaded &&
                profileState.profile?.isOnline != null) {
              context.read<HomeBloc>().add(
                HomeSetOnlineStatus(isOnline: profileState.profile!.isOnline!),
              );
            }
            if (profileState is ProfileLoaded) {
              Future.microtask(() async {
                if (!context.mounted) return;
                await presentPendingAppUpdate(context: context);
              });
            }
            // Sync fingerprint and 2FA state from profile
            if (profileState is ProfileLoaded && profileState.profile != null) {
              final p = profileState.profile!;
              final fingerprint = await BiometricService.instance
                  .isBiometricEnabledForThisUser(
                    login: _loginFromProfile(p) ?? '',
                  );
              setState(() {
                _twoFAEnabled = p.mfaChannel != null && p.mfaChannel! > 0;
                // _fingerprintEnabled = p.biometricFingerprint ?? false;
                _fingerprintEnabled = fingerprint;
              });
            }
            if (profileState is ProfileUpdateSuccess && context.mounted) {
              context.read<ProfileBloc>().add(const ProfileLoadRequested());
            }
          },
        ),
        BlocListener<AuthBloc, AuthState>(
          listener: (context, authState) {
            if (authState is AuthUnauthenticated) {
              context.go('/login');
            }
            if (authState is AuthEnableMfaSuccess ||
                authState is AuthDisableMfaSuccess) {
              context.read<ProfileBloc>().add(const ProfileLoadRequested());
            }
            if (authState is AuthEnableBiometricSuccess) {
              setState(() => _fingerprintEnabled = true);
              // final profileState = context.read<ProfileBloc>().state;
              // final profile = profileState is ProfileLoaded
              //     ? profileState.profile
              //     : null;
              // context.read<ProfileBloc>().add(
              //   ProfileUpdateRequested(
              //     fullName: profile?.fullName,
              //     fullNameAr: profile?.fullNameAr,
              //     userName: profile?.userName,
              //     email: profile?.email,
              //     phone: profile?.phone,
              //     biometricFingerprint: true,
              //   ),
              // );
            }
            if (authState is AuthDisableBiometricSuccess) {
              setState(() => _fingerprintEnabled = false);
              // final profileState = context.read<ProfileBloc>().state;
              // final profile = profileState is ProfileLoaded
              //     ? profileState.profile
              //     : null;
              // context.read<ProfileBloc>().add(
              //   ProfileUpdateRequested(
              //     fullName: profile?.fullName,
              //     fullNameAr: profile?.fullNameAr,
              //     userName: profile?.userName,
              //     email: profile?.email,
              //     phone: profile?.phone,
              //     biometricFingerprint: false,
              //   ),
              // );
            }
            if (authState is AuthError) {
              final route = ModalRoute.of(context);
              if (route == null || !route.isCurrent) {
                return;
              }
              final isBiometricError =
                  authState.errorCode != null &&
                  authState.errorCode!.contains('biometric');
              if (isBiometricError) {
                setState(() => _fingerprintEnabled = !_fingerprintEnabled);
              } else {
                setState(() => _twoFAEnabled = !_twoFAEnabled);
              }
              final displayMessage = authState.getDisplayMessage(
                AppLocalizations.of(context)!,
              );
              showCupertinoDialog(
                context: context,
                builder: (ctx) => CupertinoAlertDialog(
                  title: Text(AppLocalizations.of(context)!.error),
                  content: Text(displayMessage),
                  actions: [
                    CupertinoDialogAction(
                      isDefaultAction: true,
                      onPressed: () => Navigator.of(ctx).pop(),
                      child: Text(AppLocalizations.of(context)!.ok),
                    ),
                  ],
                ),
              );
            }
          },
        ),
      ],
      child: BlocBuilder<ProfileBloc, ProfileState>(
        builder: (context, profileState) {
          if (profileState is ProfileLoading) {
            return const CupertinoPageScaffold(
              backgroundColor: AppColors.white,
              child: MainScaffold(
                child: Center(
                  child: CupertinoActivityIndicator(color: AppColors.mainBlue),
                ),
              ),
            );
          }

          if (profileState is ProfileError) {
            final l10n = AppLocalizations.of(context)!;
            return CupertinoPageScaffold(
              backgroundColor: AppColors.white,
              child: MainScaffold(
                child: Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        profileState.message,
                        style: const TextStyle(color: AppColors.darkGray),
                      ),
                      const SizedBox(height: 16),
                      CupertinoButton(
                        onPressed: () {
                          context.read<ProfileBloc>().add(
                            const ProfileLoadRequested(),
                          );
                        },
                        child: Text(l10n.retry),
                      ),
                    ],
                  ),
                ),
              ),
            );
          }

          final profile = profileState is ProfileLoaded
              ? profileState.profile
              : null;
          final isRTL = Directionality.of(context) == TextDirection.rtl;
          final displayName =
              isRTL &&
                  profile?.fullNameAr != null &&
                  profile!.fullNameAr!.isNotEmpty
              ? profile.fullNameAr!
              : profile?.fullName ?? profile?.userName ?? '';

          return PopScope(
            canPop: false,
            onPopInvokedWithResult: (didPop, result) {
              if (!didPop) _handleBackNavigation();
            },
            child: CupertinoPageScaffold(
              backgroundColor: AppColors.mainBlue,
              navigationBar: CupertinoNavigationBar(
                backgroundColor: AppColors.mainBlue,
                brightness: Brightness.dark,
                leading: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    AppBackButton(onPressed: _handleBackNavigation),
                    const SizedBox(width: 8),
                    Text(
                      l10n.profilePage,
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
                    color: AppColors.white,
                    child: RefreshIndicator(
                      onRefresh: () async {
                        final bloc = context.read<ProfileBloc>();
                        bloc.add(const ProfileLoadRequested());
                        await bloc.stream
                            .where(
                              (s) => s is ProfileLoaded || s is ProfileError,
                            )
                            .first;
                      },
                      child: SingleChildScrollView(
                        physics: const AlwaysScrollableScrollPhysics(),
                        child: Column(
                          children: [
                            const SizedBox(height: 8),
                            // Profile Summary Card
                            _ProfileSummaryCard(
                              name: displayName.isNotEmpty
                                  ? displayName
                                  : l10n.user,
                              email: profile?.email ?? '',
                              rating: profile?.driverRating ?? 0.0,
                              trips: profile?.tripsCount ?? 0,
                              avatarUrl: profile?.profilePicture,
                            ),
                            const SizedBox(height: 24),
                            // Tabs
                            _ProfileTabs(
                              selectedIndex: _selectedTab,
                              onTabChanged: (index) {
                                setState(() {
                                  _selectedTab = index;
                                });
                              },
                            ),
                            const SizedBox(height: 24),
                            // Tab Content
                            if (_selectedTab == 0)
                              _PersonalDataTab(profile: profile)
                            else
                              _FilesTab(profile: profile),
                            const SizedBox(height: 24),
                            // Language Switcher
                            Padding(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 20,
                              ),
                              child: _LanguageSwitcherRow(),
                            ),
                            const SizedBox(height: 20),
                            // Settings Section
                            Padding(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 20,
                              ),
                              child: _ProfileSettingsSection(
                                profile: profile,
                                fingerprintEnabled: _fingerprintEnabled,
                                twoFAEnabled: _twoFAEnabled,
                                biometricAvailable: _biometricAvailable,
                                loginFromProfile: _loginFromProfile,
                                showEnableBiometricDialog:
                                    _showEnableBiometricPasswordDialog,
                                onFingerprintChanged: (v) =>
                                    setState(() => _fingerprintEnabled = v),
                                onTwoFAChanged: (v) =>
                                    setState(() => _twoFAEnabled = v),
                              ),
                            ),
                            const SizedBox(height: 16),
                            // Logout Button
                            Padding(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 20,
                              ),
                              child: _LogoutButton(
                                onLogout: () {
                                  context.read<AuthBloc>().add(
                                    const AuthLogoutRequested(),
                                  );
                                },
                              ),
                            ),
                            const SizedBox(height: 24),
                            const Center(child: AppVersionWidget()),
                            const SizedBox(height: 32),
                          ],
                        ),
                      ),
                    ),
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}

class _ProfileSummaryCard extends StatelessWidget {
  const _ProfileSummaryCard({
    required this.name,
    required this.email,
    required this.rating,
    required this.trips,
    this.avatarUrl,
  });

  final String name;
  final String email;
  final double rating;
  final int trips;
  final String? avatarUrl;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 20),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            AppColors.mainBlue,
            AppColors.mainBlue.withValues(alpha: 0.85),
          ],
        ),
        borderRadius: BorderRadius.circular(28),
        boxShadow: [
          BoxShadow(
            color: AppColors.mainBlue.withValues(alpha: 0.35),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          children: [
            // Avatar and name row
            Row(
              textDirection: TextDirection.ltr,
              children: [
                Container(
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    border: Border.all(
                      color: AppColors.white.withValues(alpha: 0.5),
                      width: 3,
                    ),
                    boxShadow: [
                      BoxShadow(
                        color: const Color(0xFF000000).withValues(alpha: 0.2),
                        blurRadius: 12,
                        offset: const Offset(0, 4),
                      ),
                    ],
                  ),
                  child: ClipOval(
                    child: avatarUrl != null && avatarUrl!.isNotEmpty
                        ? Image.network(
                            avatarUrl!,
                            width: 72,
                            height: 72,
                            fit: BoxFit.cover,
                            errorBuilder: (context, error, stackTrace) =>
                                const _DefaultAvatar(
                                  size: 72,
                                  forDarkBackground: true,
                                ),
                          )
                        : const _DefaultAvatar(
                            size: 72,
                            forDarkBackground: true,
                          ),
                  ),
                ),
                const SizedBox(width: 20),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        name,
                        style: const TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.w700,
                          color: AppColors.white,
                          letterSpacing: -0.3,
                        ),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                      if (email.isNotEmpty) ...[
                        const SizedBox(height: 6),
                        Text(
                          email,
                          style: TextStyle(
                            fontSize: 14,
                            color: AppColors.white.withValues(alpha: 0.9),
                          ),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),
            // Metrics row
            Row(
              textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
              children: [
                Expanded(
                  child: _MetricChip(
                    icon: CupertinoIcons.star_fill,
                    iconColor: const Color(0xFFFFD700),
                    value: rating.toStringAsFixed(1),
                    label: l10n.rating,
                  ),
                ),
                Container(
                  width: 1,
                  height: 36,
                  color: AppColors.white.withValues(alpha: 0.3),
                ),
                Expanded(
                  child: _MetricChip(
                    icon: CupertinoIcons.car_detailed,
                    iconColor: AppColors.white,
                    value: trips.toString(),
                    label: l10n.trip,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _MetricChip extends StatelessWidget {
  const _MetricChip({
    required this.icon,
    required this.iconColor,
    required this.value,
    required this.label,
  });

  final IconData icon;
  final Color iconColor;
  final String value;
  final String label;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, color: iconColor, size: 18),
            const SizedBox(width: 6),
            Text(
              value,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w700,
                color: AppColors.white,
              ),
            ),
          ],
        ),
        const SizedBox(height: 2),
        Text(
          label,
          style: TextStyle(
            fontSize: 12,
            color: AppColors.white.withValues(alpha: 0.85),
          ),
        ),
      ],
    );
  }
}

class _DefaultAvatar extends StatelessWidget {
  const _DefaultAvatar({this.size = 80, this.forDarkBackground = false});

  final double size;

  /// When true, uses light colors for use on dark backgrounds (e.g. profile card).
  final bool forDarkBackground;

  @override
  Widget build(BuildContext context) {
    final bgColor = forDarkBackground
        ? AppColors.white.withValues(alpha: 0.3)
        : AppColors.lightGray;
    final iconColor = forDarkBackground
        ? AppColors.white.withValues(alpha: 0.9)
        : AppColors.mediumBlueGray;

    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(color: bgColor, shape: BoxShape.circle),
      child: Icon(
        CupertinoIcons.person_fill,
        size: size * 0.5,
        color: iconColor,
      ),
    );
  }
}

class _ProfileTabs extends StatelessWidget {
  const _ProfileTabs({required this.selectedIndex, required this.onTabChanged});

  final int selectedIndex;
  final ValueChanged<int> onTabChanged;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20),
      child: Row(
        textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
        children: [
          Expanded(
            child: _TabButton(
              label: l10n.myPersonalData,
              isSelected: selectedIndex == 0,
              onTap: () => onTabChanged(0),
            ),
          ),
          Expanded(
            child: _TabButton(
              label: l10n.files,
              isSelected: selectedIndex == 1,
              onTap: () => onTabChanged(1),
            ),
          ),
        ],
      ),
    );
  }
}

class _TabButton extends StatelessWidget {
  const _TabButton({
    required this.label,
    required this.isSelected,
    required this.onTap,
  });

  final String label;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return CupertinoButton(
      padding: const EdgeInsets.symmetric(vertical: 12),
      onPressed: onTap,
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            label,
            style: TextStyle(
              fontSize: 15,
              fontWeight: isSelected ? FontWeight.w600 : FontWeight.w500,
              color: isSelected ? AppColors.mainBlue : AppColors.mediumBlueGray,
            ),
            overflow: TextOverflow.ellipsis,
          ),
          const SizedBox(height: 10),
          AnimatedContainer(
            duration: const Duration(milliseconds: 200),
            curve: Curves.easeInOut,
            height: 2,
            width: 40,
            decoration: BoxDecoration(
              color: isSelected
                  ? AppColors.mainBlue
                  : AppColors.lightGray.withValues(alpha: 0.5),
              borderRadius: BorderRadius.circular(1),
            ),
          ),
        ],
      ),
    );
  }
}

class _FilesTab extends StatefulWidget {
  const _FilesTab({this.profile});

  final DriverProfile? profile;

  @override
  State<_FilesTab> createState() => _FilesTabState();
}

class _FilesTabState extends State<_FilesTab> {
  List<_FileItem> _files = const <_FileItem>[];

  @override
  void initState() {
    super.initState();
    _syncFilesFromProfile();
  }

  @override
  void didUpdateWidget(covariant _FilesTab oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.profile != widget.profile) {
      _syncFilesFromProfile();
    }
  }

  String _extractFileName(String? url) {
    if (url == null || url.isEmpty) return '';
    final uri = Uri.tryParse(url);
    final segment = uri?.pathSegments.isNotEmpty == true
        ? uri!.pathSegments.last
        : url.split('/').last;
    if (segment.isEmpty) return '';
    return Uri.decodeComponent(segment);
  }

  void _syncFilesFromProfile() {
    final docs = widget.profile?.documents ?? const <DriverProfileDocument>[];
    setState(() {
      _files = docs
          .map(
            (doc) => _FileItem(
              id: doc.id,
              documentTypeId: doc.documentId ?? doc.document?.id,
              name: (doc.document?.nameEn?.trim().isNotEmpty ?? false)
                  ? doc.document!.nameEn!.trim()
                  : _extractFileName(doc.fileUrl),
              status: doc.status,
              expiryDate: doc.expiryDate,
            ),
          )
          .toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // Displayed files (read-only list from profile)
          if (_files.isEmpty)
            Container(
              padding: const EdgeInsets.symmetric(vertical: 40, horizontal: 24),
              margin: const EdgeInsets.only(top: 8),
              decoration: BoxDecoration(
                color: AppColors.lightGray.withValues(alpha: 0.3),
                borderRadius: BorderRadius.circular(16),
              ),
              child: Column(
                children: [
                  Icon(
                    CupertinoIcons.doc_on_doc,
                    size: 48,
                    color: AppColors.mediumBlueGray.withValues(alpha: 0.6),
                  ),
                  const SizedBox(height: 12),
                  Text(
                    l10n.noFilesAvailable,
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontSize: 15,
                      color: AppColors.mediumBlueGray.withValues(alpha: 0.9),
                    ),
                  ),
                ],
              ),
            ),
          ..._files.map(
            (file) => Padding(
              padding: const EdgeInsets.only(bottom: 14),
              child: _FileCard(
                file: file,
                onUpdate: () => _pickAndSaveDocument(file),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _pickAndSaveDocument(_FileItem file) async {
    final hasPermission = await _ensureGalleryPermission();
    if (!hasPermission || !mounted) return;

    final picker = ImagePicker();
    final selected = await picker.pickImage(
      source: ImageSource.gallery,
      imageQuality: 90,
    );
    if (selected == null || !mounted) return;

    final documentTypeId = file.documentTypeId;
    if (documentTypeId == null) {
      _showMissingDocumentTypeDialog();
      return;
    }

    context.read<ProfileBloc>().add(
      ProfileDocumentSaveRequested(
        documentRecordId: file.id > 0 ? file.id : null,
        documentTypeId: documentTypeId,
        filePath: selected.path,
      ),
    );
  }

  Future<bool> _ensureGalleryPermission() async {
    if (Platform.isAndroid) {
      final sdk = await _androidSdkInt();
      if (!mounted) return false;
      if (sdk != null && sdk >= 33) return true;

      var status = await Permission.storage.status;
      if (!mounted) return false;
      if (status.isDenied) {
        final l10n = AppLocalizations.of(context)!;
        final disclosed = await showProminentUserDataDisclosure(
          context,
          title: l10n.photosPermissionDisclosureTitle,
          body: l10n.photosPermissionDisclosureBody,
        );
        if (!disclosed || !mounted) return false;
        status = await Permission.storage.request();
      }
      return status.isGranted;
    }

    var status = await Permission.photos.status;
    if (!mounted) return false;
    if (status.isDenied) {
      final l10n = AppLocalizations.of(context)!;
      final disclosed = await showProminentUserDataDisclosure(
        context,
        title: l10n.photosPermissionDisclosureTitle,
        body: l10n.photosPermissionDisclosureBody,
      );
      if (!disclosed || !mounted) return false;
      status = await Permission.photos.request();
    }
    return status.isGranted || status.isLimited;
  }

  void _showMissingDocumentTypeDialog() {
    if (!mounted) return;
    final l10n = AppLocalizations.of(context)!;
    showCupertinoDialog<void>(
      context: context,
      builder: (BuildContext context) => CupertinoAlertDialog(
        content: Text(l10n.documentTypeMissing),
        actions: <CupertinoDialogAction>[
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () => Navigator.pop(context),
            child: Text(l10n.ok),
          ),
        ],
      ),
    );
  }
}

class _FileCard extends StatelessWidget {
  const _FileCard({required this.file, required this.onUpdate});

  final _FileItem file;
  final VoidCallback onUpdate;

  String _statusLabel(AppLocalizations l10n, int? status) {
    if (status == 1) return l10n.active;
    if (status == 0) return l10n.inactive;
    return status?.toString() ?? '—';
  }

  Color _statusColor(int? status) {
    if (status == 1) return AppColors.green;
    if (status == 0) return AppColors.red;
    return AppColors.mediumBlueGray;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.lightGray.withValues(alpha: 0.8)),
        boxShadow: [
          BoxShadow(
            color: AppColors.lightGray.withValues(alpha: 0.2),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Row(
        textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // File icon container
          Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              color: AppColors.mainBlue.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(12),
            ),
            child: const Icon(
              CupertinoIcons.doc_fill,
              color: AppColors.mainBlue,
              size: 24,
            ),
          ),
          const SizedBox(width: 14),
          // File info
          Expanded(
            child: Column(
              crossAxisAlignment: isRTL
                  ? CrossAxisAlignment.end
                  : CrossAxisAlignment.start,
              children: [
                Text(
                  file.name,
                  style: const TextStyle(
                    fontSize: 15,
                    fontWeight: FontWeight.w600,
                    color: AppColors.darkGray,
                  ),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
                if (file.expiryDate != null && file.expiryDate!.isNotEmpty) ...[
                  const SizedBox(height: 6),
                  Row(
                    mainAxisSize: MainAxisSize.min,
                    textDirection: isRTL
                        ? TextDirection.rtl
                        : TextDirection.ltr,
                    children: [
                      const Icon(
                        CupertinoIcons.calendar,
                        size: 14,
                        color: AppColors.mediumBlueGray,
                      ),
                      const SizedBox(width: 4),
                      Text(
                        file.expiryDate!.split('T').first,
                        style: const TextStyle(
                          fontSize: 13,
                          color: AppColors.mediumBlueGray,
                        ),
                      ),
                    ],
                  ),
                ],
                if (file.status != null) ...[
                  const SizedBox(height: 8),
                  Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 10,
                      vertical: 4,
                    ),
                    decoration: BoxDecoration(
                      color: _statusColor(file.status).withValues(alpha: 0.15),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Text(
                      _statusLabel(l10n, file.status),
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.w600,
                        color: _statusColor(file.status),
                      ),
                    ),
                  ),
                ],
                const SizedBox(height: 12),
                CupertinoButton(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 14,
                    vertical: 8,
                  ),
                  color: AppColors.mainBlue,
                  borderRadius: BorderRadius.circular(10),
                  minimumSize: Size.zero,
                  onPressed: onUpdate,
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    textDirection: isRTL
                        ? TextDirection.rtl
                        : TextDirection.ltr,
                    children: [
                      const Icon(
                        CupertinoIcons.photo,
                        color: AppColors.white,
                        size: 16,
                      ),
                      const SizedBox(width: 6),
                      Text(
                        l10n.chooseFromGallery,
                        style: const TextStyle(
                          color: AppColors.white,
                          fontSize: 13,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _FileItem {
  _FileItem({
    required this.id,
    required this.documentTypeId,
    required this.name,
    this.status,
    this.expiryDate,
  });

  final int id;
  final int? documentTypeId;
  final String name;
  final int? status;
  final String? expiryDate;
}

class _PersonalDataTab extends StatefulWidget {
  const _PersonalDataTab({this.profile});

  final DriverProfile? profile;

  @override
  State<_PersonalDataTab> createState() => _PersonalDataTabState();
}

class _PersonalDataTabState extends State<_PersonalDataTab> {
  bool _isEditing = false;
  late TextEditingController _fullNameController;
  late TextEditingController _fullNameArController;
  late TextEditingController _userNameController;
  late TextEditingController _emailController;
  late TextEditingController _phoneController;
  String? _profilePicturePath;

  @override
  void initState() {
    super.initState();
    _fullNameController = TextEditingController();
    _fullNameArController = TextEditingController();
    _userNameController = TextEditingController();
    _emailController = TextEditingController();
    _phoneController = TextEditingController();
    _updateControllers();
    _profilePicturePath = null;
  }

  /// Trims +2 prefix for display (widget shows +2 in the UI).
  static String _phoneForDisplay(String? phone) {
    if (phone == null || phone.isEmpty) return '';
    String s = phone.trim();
    if (s.startsWith('+')) s = s.substring(1);
    if (s.startsWith('2')) s = s.substring(1);
    return s;
  }

  /// Ensures phone is sent with +2 prefix to the API.
  static String? _phoneForSave(String? raw) {
    if (raw == null || raw.isEmpty) return null;
    final digits = extractDigits(raw);
    if (digits.isEmpty) return null;
    return '+2$digits';
  }

  void _updateControllers() {
    final profile = widget.profile;
    _fullNameController.text = profile?.fullName ?? '';
    _fullNameArController.text = profile?.fullNameAr ?? '';
    _userNameController.text = profile?.userName ?? '';
    _emailController.text = profile?.email ?? '';
    _phoneController.text = _phoneForDisplay(profile?.phone);
  }

  @override
  void didUpdateWidget(_PersonalDataTab oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.profile != widget.profile) {
      _updateControllers();
    }
  }

  @override
  void dispose() {
    _fullNameController.dispose();
    _fullNameArController.dispose();
    _userNameController.dispose();
    _emailController.dispose();
    _phoneController.dispose();
    super.dispose();
  }

  void _submit() {
    context.read<ProfileBloc>().add(
      ProfileUpdateRequested(
        fullName: _fullNameController.text.trim().isNotEmpty
            ? _fullNameController.text.trim()
            : null,
        fullNameAr: _fullNameArController.text.trim().isNotEmpty
            ? _fullNameArController.text.trim()
            : null,
        userName: _userNameController.text.trim().isNotEmpty
            ? _userNameController.text.trim()
            : null,
        email: _emailController.text.trim().isNotEmpty
            ? _emailController.text.trim()
            : null,
        phone: _phoneForSave(_phoneController.text.trim()),
        profilePicturePath: _profilePicturePath,
      ),
    );
  }

  Future<void> _showImageSourceDialog() async {
    final l10n = AppLocalizations.of(context)!;

    showCupertinoModalPopup<void>(
      context: context,
      builder: (BuildContext context) => CupertinoActionSheet(
        actions: <CupertinoActionSheetAction>[
          CupertinoActionSheetAction(
            onPressed: () {
              Navigator.pop(context);
              _pickImageFromCamera();
            },
            child: Text(l10n.takePhoto),
          ),
          CupertinoActionSheetAction(
            onPressed: () {
              Navigator.pop(context);
              _pickImageFromGallery();
            },
            child: Text(l10n.chooseFromGallery),
          ),
        ],
        cancelButton: CupertinoActionSheetAction(
          isDefaultAction: true,
          onPressed: () {
            Navigator.pop(context);
          },
          child: Text(l10n.cancel),
        ),
      ),
    );
  }

  Future<void> _pickImageFromCamera() async {
    final l10n = AppLocalizations.of(context)!;

    // Check camera permission
    final cameraStatus = await Permission.camera.status;
    if (!mounted) return;
    if (cameraStatus.isDenied) {
      final disclosed = await showProminentUserDataDisclosure(
        context,
        title: l10n.cameraPermissionDisclosureTitle,
        body: l10n.cameraPermissionDisclosureBody,
      );
      if (!disclosed || !mounted) return;
      final result = await Permission.camera.request();
      if (result.isDenied) {
        _showPermissionDeniedDialog(
          l10n.cameraPermissionRequired,
          l10n.permissionDenied,
        );
        return;
      }
    }

    if (cameraStatus.isPermanentlyDenied) {
      _showPermissionDeniedDialog(
        l10n.cameraPermissionRequired,
        l10n.permissionDenied,
      );
      return;
    }

    final ImagePicker picker = ImagePicker();
    final XFile? image = await picker.pickImage(
      source: ImageSource.camera,
      imageQuality: 85,
    );

    if (image != null && mounted) {
      setState(() {
        _profilePicturePath = image.path;
      });
    }
  }

  Future<void> _pickImageFromGallery() async {
    final l10n = AppLocalizations.of(context)!;

    if (Platform.isAndroid) {
      final sdk = await _androidSdkInt();
      if (!mounted) return;
      if (sdk != null && sdk < 33) {
        var storageStatus = await Permission.storage.status;
        if (!mounted) return;
        if (storageStatus.isPermanentlyDenied) {
          _showPermissionDeniedDialog(
            l10n.photoLibraryPermissionRequired,
            l10n.permissionDenied,
          );
          return;
        }
        if (storageStatus.isDenied) {
          final disclosed = await showProminentUserDataDisclosure(
            context,
            title: l10n.photosPermissionDisclosureTitle,
            body: l10n.photosPermissionDisclosureBody,
          );
          if (!disclosed || !mounted) return;
          storageStatus = await Permission.storage.request();
        }
        if (!storageStatus.isGranted) {
          _showPermissionDeniedDialog(
            l10n.photoLibraryPermissionRequired,
            l10n.permissionDenied,
          );
          return;
        }
      }
    } else {
      var photoStatus = await Permission.photos.status;
      if (!mounted) return;
      if (photoStatus.isPermanentlyDenied) {
        _showPermissionDeniedDialog(
          l10n.photoLibraryPermissionRequired,
          l10n.permissionDenied,
        );
        return;
      }
      if (photoStatus.isDenied) {
        final disclosed = await showProminentUserDataDisclosure(
          context,
          title: l10n.photosPermissionDisclosureTitle,
          body: l10n.photosPermissionDisclosureBody,
        );
        if (!disclosed || !mounted) return;
        photoStatus = await Permission.photos.request();
      }
      if (photoStatus.isDenied || photoStatus.isPermanentlyDenied) {
        _showPermissionDeniedDialog(
          l10n.photoLibraryPermissionRequired,
          l10n.permissionDenied,
        );
        return;
      }
      if (!photoStatus.isGranted && !photoStatus.isLimited) {
        _showPermissionDeniedDialog(
          l10n.photoLibraryPermissionRequired,
          l10n.permissionDenied,
        );
        return;
      }
    }

    final ImagePicker picker = ImagePicker();
    final XFile? image = await picker.pickImage(
      source: ImageSource.gallery,
      imageQuality: 85,
    );

    if (image != null && mounted) {
      setState(() {
        _profilePicturePath = image.path;
      });
    }
  }

  void _showPermissionDeniedDialog(String message, String title) {
    final l10n = AppLocalizations.of(context)!;

    showCupertinoDialog<void>(
      context: context,
      builder: (BuildContext context) => CupertinoAlertDialog(
        title: Text(title),
        content: Text(message),
        actions: <CupertinoDialogAction>[
          CupertinoDialogAction(
            child: Text(l10n.cancel),
            onPressed: () {
              Navigator.pop(context);
            },
          ),
          CupertinoDialogAction(
            isDefaultAction: true,
            child: Text(l10n.openSettings),
            onPressed: () {
              Navigator.pop(context);
              openAppSettings();
            },
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocListener<ProfileBloc, ProfileState>(
      listener: (context, state) {
        if (state is ProfileUpdateSuccess) {
          setState(() => _isEditing = false);
          // Reload profile to sync UI
          context.read<ProfileBloc>().add(const ProfileLoadRequested());
          // Show success message
          showCupertinoDialog(
            context: context,
            builder: (context) => CupertinoAlertDialog(
              title: Text(l10n.ok),
              content: Text(l10n.profileUpdatedSuccessfully),
              actions: [
                CupertinoDialogAction(
                  isDefaultAction: true,
                  onPressed: () => Navigator.of(context).pop(),
                  child: Text(l10n.ok),
                ),
              ],
            ),
          );
        } else if (state is ProfileError) {
          // Show error message
          showCupertinoDialog(
            context: context,
            builder: (context) => CupertinoAlertDialog(
              title: Text(l10n.error),
              content: Text(state.message),
              actions: [
                CupertinoDialogAction(
                  isDefaultAction: true,
                  onPressed: () => Navigator.of(context).pop(),
                  child: Text(l10n.ok),
                ),
              ],
            ),
          );
        }
      },
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (_isEditing) ...[
              // Profile Picture Section (only when editing)
              Center(
                child: Column(
                  children: [
                    Stack(
                      children: [
                        ClipOval(
                          child: _profilePicturePath != null
                              ? Image.file(
                                  File(_profilePicturePath!),
                                  width: 100,
                                  height: 100,
                                  fit: BoxFit.cover,
                                  errorBuilder: (context, error, stackTrace) =>
                                      const _DefaultAvatar(size: 100),
                                )
                              : widget.profile?.profilePicture != null &&
                                    widget.profile!.profilePicture!.isNotEmpty
                              ? Image.network(
                                  widget.profile!.profilePicture!,
                                  width: 100,
                                  height: 100,
                                  fit: BoxFit.cover,
                                  errorBuilder: (context, error, stackTrace) =>
                                      const _DefaultAvatar(size: 100),
                                )
                              : const _DefaultAvatar(size: 100),
                        ),
                        Positioned(
                          bottom: 0,
                          right: 0,
                          child: CupertinoButton(
                            padding: EdgeInsets.zero,
                            minimumSize: Size.zero,
                            onPressed: _showImageSourceDialog,
                            child: Container(
                              width: 32,
                              height: 32,
                              decoration: BoxDecoration(
                                color: AppColors.mainBlue,
                                shape: BoxShape.circle,
                                border: Border.all(
                                  color: AppColors.white,
                                  width: 2,
                                ),
                              ),
                              child: const Icon(
                                CupertinoIcons.camera,
                                color: AppColors.white,
                                size: 18,
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    Text(
                      l10n.chooseFromGallery,
                      style: const TextStyle(
                        fontSize: 14,
                        color: AppColors.mainBlue,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 20),
              // Full Name (English)
              _EditableField(
                label: l10n.fullNameEnglish,
                controller: _fullNameController,
                icon: CupertinoIcons.person,
              ),
              const SizedBox(height: 14),
              // Full Name (Arabic)
              _EditableField(
                label: l10n.fullNameArabic,
                controller: _fullNameArController,
                icon: CupertinoIcons.textformat,
              ),
              const SizedBox(height: 14),
              // User Name
              _EditableField(
                label: l10n.userName,
                controller: _userNameController,
                icon: CupertinoIcons.at,
              ),
              const SizedBox(height: 14),
              // Email (with Verify button when not verified in API or when user changed email)
              _EmailFieldWithVerify(
                label: l10n.email,
                controller: _emailController,
                emailVerified: widget.profile?.emailVerified ?? false,
                profileEmail: widget.profile?.email,
                onVerify: () => context.push('/verify-email'),
              ),
              const SizedBox(height: 14),
              // Phone
              AppPhoneInput(
                controller: _phoneController,
                label: l10n.phone,
                placeholder: l10n.phone,
              ),
            ] else ...[
              const SizedBox(height: 20),
              // Read-only personal data
              _ReadOnlyField(
                label: l10n.fullNameEnglish,
                value: widget.profile?.fullName ?? '',
                icon: CupertinoIcons.person,
              ),
              const SizedBox(height: 12),
              _ReadOnlyField(
                label: l10n.fullNameArabic,
                value: widget.profile?.fullNameAr ?? '',
                icon: CupertinoIcons.textformat,
              ),
              const SizedBox(height: 12),
              _ReadOnlyField(
                label: l10n.userName,
                value: widget.profile?.userName ?? '',
                icon: CupertinoIcons.at,
              ),
              const SizedBox(height: 12),
              _ReadOnlyField(
                label: l10n.email,
                value: widget.profile?.email ?? '',
                icon: CupertinoIcons.mail_solid,
              ),
              const SizedBox(height: 12),
              _ReadOnlyField(
                label: l10n.phone,
                value: widget.profile?.phone ?? '',
                icon: CupertinoIcons.phone_fill,
              ),
            ],
            const SizedBox(height: 32),
            if (_isEditing)
              BlocBuilder<ProfileBloc, ProfileState>(
                builder: (context, state) {
                  final isLoading = state is ProfileLoading;
                  return Row(
                    children: [
                      Expanded(
                        child: CupertinoButton(
                          padding: EdgeInsets.zero,
                          onPressed: isLoading
                              ? null
                              : () => setState(() => _isEditing = false),
                          child: Container(
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            decoration: BoxDecoration(
                              color: isLoading
                                  ? AppColors.lightGray
                                  : AppColors.white,
                              borderRadius: BorderRadius.circular(10),
                              border: Border.all(color: AppColors.mainBlue),
                            ),
                            child: Center(
                              child: Text(
                                l10n.cancel,
                                style: TextStyle(
                                  fontSize: 16,
                                  fontWeight: FontWeight.w600,
                                  color: isLoading
                                      ? AppColors.mediumBlueGray
                                      : AppColors.mainBlue,
                                ),
                              ),
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        flex: 2,
                        child: CupertinoButton(
                          padding: EdgeInsets.zero,
                          onPressed: isLoading ? null : _submit,
                          child: Container(
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            decoration: BoxDecoration(
                              color: isLoading
                                  ? AppColors.lightGray
                                  : AppColors.mainBlue,
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                if (isLoading) ...[
                                  const CupertinoActivityIndicator(
                                    color: AppColors.white,
                                    radius: 10,
                                  ),
                                  const SizedBox(width: 8),
                                ],
                                Text(
                                  l10n.save,
                                  style: const TextStyle(
                                    fontSize: 16,
                                    fontWeight: FontWeight.w600,
                                    color: AppColors.white,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      ),
                    ],
                  );
                },
              )
            else
              CupertinoButton(
                padding: EdgeInsets.zero,
                onPressed: () => setState(() => _isEditing = true),
                child: Container(
                  width: double.infinity,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  decoration: BoxDecoration(
                    color: AppColors.mainBlue,
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(
                        CupertinoIcons.pencil,
                        color: AppColors.white,
                        size: 20,
                      ),
                      const SizedBox(width: 8),
                      Text(
                        l10n.editProfile,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                          color: AppColors.white,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

class _EmailFieldWithVerify extends StatefulWidget {
  const _EmailFieldWithVerify({
    required this.label,
    required this.controller,
    required this.emailVerified,
    this.profileEmail,
    required this.onVerify,
  });

  final String label;
  final TextEditingController controller;

  /// From API: true when email is verified, false when not.
  final bool emailVerified;

  /// Original email from profile (API). When user edits email and it differs, show verify.
  final String? profileEmail;
  final VoidCallback onVerify;

  @override
  State<_EmailFieldWithVerify> createState() => _EmailFieldWithVerifyState();
}

class _EmailFieldWithVerifyState extends State<_EmailFieldWithVerify> {
  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_onEmailChanged);
  }

  @override
  void didUpdateWidget(_EmailFieldWithVerify oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.controller != widget.controller) {
      oldWidget.controller.removeListener(_onEmailChanged);
      widget.controller.addListener(_onEmailChanged);
    }
  }

  @override
  void dispose() {
    widget.controller.removeListener(_onEmailChanged);
    super.dispose();
  }

  void _onEmailChanged() => setState(() {});

  bool get _showVerify {
    final notVerifiedFromApi = !widget.emailVerified;
    final profileEmail = widget.profileEmail?.trim() ?? '';
    final currentEmail = widget.controller.text.trim();
    final userChangedEmail =
        currentEmail.isNotEmpty && currentEmail != profileEmail;
    return notVerifiedFromApi || userChangedEmail;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Align(
          alignment: AlignmentDirectional.centerStart,
          child: Text(
            widget.label,
            style: const TextStyle(
              color: AppColors.darkGray,
              fontSize: 15,
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
        const SizedBox(height: 8),
        Row(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Expanded(
              child: Container(
                decoration: BoxDecoration(
                  color: AppColors.white,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColors.lightGray),
                ),
                child: CupertinoTextField(
                  controller: widget.controller,
                  keyboardType: TextInputType.emailAddress,
                  padding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 14,
                  ),
                  decoration: null,
                ),
              ),
            ),
            if (_showVerify) ...[
              const SizedBox(width: 12),
              CupertinoButton(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 12,
                ),
                color: AppColors.mainBlue,
                onPressed: widget.onVerify,
                child: Text(
                  l10n.verify,
                  style: const TextStyle(
                    fontSize: 15,
                    fontWeight: FontWeight.w600,
                    color: AppColors.white,
                  ),
                ),
              ),
            ],
          ],
        ),
      ],
    );
  }
}

class _ReadOnlyField extends StatelessWidget {
  const _ReadOnlyField({required this.label, required this.value, this.icon});

  final String label;
  final String value;
  final IconData? icon;

  @override
  Widget build(BuildContext context) {
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.lightGray.withValues(alpha: 0.8)),
        boxShadow: [
          BoxShadow(
            color: AppColors.lightGray.withValues(alpha: 0.2),
            blurRadius: 6,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Row(
        textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
        children: [
          if (icon != null) ...[
            Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                color: AppColors.mainBlue.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(10),
              ),
              child: Icon(icon, color: AppColors.mainBlue, size: 20),
            ),
            const SizedBox(width: 14),
          ],
          Expanded(
            child: Column(
              crossAxisAlignment: isRTL
                  ? CrossAxisAlignment.end
                  : CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: const TextStyle(
                    fontSize: 12,
                    color: AppColors.mediumBlueGray,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  value.isEmpty ? '—' : value,
                  style: const TextStyle(
                    fontSize: 16,
                    color: AppColors.darkGray,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _EditableField extends StatelessWidget {
  const _EditableField({
    required this.label,
    required this.controller,
    this.icon,
  });

  final String label;
  final TextEditingController controller;
  final IconData? icon;

  @override
  Widget build(BuildContext context) {
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (icon != null)
          Padding(
            padding: const EdgeInsets.only(bottom: 8),
            child: Row(
              textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
              children: [
                Icon(icon, size: 18, color: AppColors.mainBlue),
                const SizedBox(width: 8),
                Text(
                  label,
                  style: const TextStyle(
                    color: AppColors.darkGray,
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          )
        else
          Padding(
            padding: const EdgeInsets.only(bottom: 8),
            child: Text(
              label,
              style: const TextStyle(
                color: AppColors.darkGray,
                fontSize: 14,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        Container(
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(14),
            border: Border.all(
              color: AppColors.lightGray.withValues(alpha: 0.8),
            ),
            boxShadow: [
              BoxShadow(
                color: AppColors.lightGray.withValues(alpha: 0.15),
                blurRadius: 6,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: CupertinoTextField(
            controller: controller,
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            decoration: null,
          ),
        ),
      ],
    );
  }
}

class _ProfileSettingsSection extends StatelessWidget {
  const _ProfileSettingsSection({
    required this.profile,
    required this.fingerprintEnabled,
    required this.twoFAEnabled,
    required this.biometricAvailable,
    required this.loginFromProfile,
    required this.showEnableBiometricDialog,
    required this.onFingerprintChanged,
    required this.onTwoFAChanged,
  });

  final DriverProfile? profile;
  final bool fingerprintEnabled;
  final bool twoFAEnabled;
  final bool biometricAvailable;
  final String? Function(DriverProfile?) loginFromProfile;
  final void Function({
    required BuildContext context,
    required String login,
    required AppLocalizations l10n,
    required VoidCallback onCancel,
  })
  showEnableBiometricDialog;
  final ValueChanged<bool> onFingerprintChanged;
  final ValueChanged<bool> onTwoFAChanged;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _SettingsMenuItem(
          icon: CupertinoIcons.lock,
          title: l10n.changePassword,
          onTap: () => context.push('/change-password'),
        ),
        if (biometricAvailable)
          BlocBuilder<AuthBloc, AuthState>(
            builder: (context, authState) {
              final isLoading = authState is AuthLoading;
              return BlocBuilder<ProfileBloc, ProfileState>(
                builder: (context, profileState) {
                  final isProfileLoading = profileState is ProfileLoading;
                  return _SettingsSwitchItem(
                    icon: CupertinoIcons.person_fill,
                    title: l10n.enableFingerprint,
                    value: fingerprintEnabled,
                    isLoading: isLoading || isProfileLoading,
                    onChanged: (value) async {
                      if (value) {
                        final login = loginFromProfile(profile);
                        if (login == null || login.isEmpty) {
                          if (context.mounted) {
                            showCupertinoDialog(
                              context: context,
                              builder: (ctx) => CupertinoAlertDialog(
                                title: Text(l10n.error),
                                content: Text(
                                  l10n.biometricAccountInfoUnavailable,
                                ),
                                actions: [
                                  CupertinoDialogAction(
                                    isDefaultAction: true,
                                    onPressed: () => Navigator.of(ctx).pop(),
                                    child: Text(l10n.ok),
                                  ),
                                ],
                              ),
                            );
                          }
                          onFingerprintChanged(false);
                          return;
                        }
                        if (context.mounted) {
                          showEnableBiometricDialog(
                            context: context,
                            login: login,
                            l10n: l10n,
                            onCancel: () => onFingerprintChanged(false),
                          );
                        }
                      } else {
                        onFingerprintChanged(value);
                        context.read<AuthBloc>().add(
                          const AuthDisableBiometricRequested(),
                        );
                      }
                    },
                  );
                },
              );
            },
          ),
        BlocBuilder<AuthBloc, AuthState>(
          builder: (context, authState) {
            final isLoading = authState is AuthLoading;
            return _SettingsSwitchItem(
              icon: CupertinoIcons.shield,
              title: l10n.enable2FA,
              value: twoFAEnabled,
              isLoading: isLoading,
              onChanged: (value) {
                onTwoFAChanged(value);
                if (value) {
                  context.read<AuthBloc>().add(
                    const AuthEnableMfaRequested(channel: 1),
                  );
                } else {
                  context.read<AuthBloc>().add(const AuthDisableMfaRequested());
                }
              },
            );
          },
        ),
        _SettingsMenuItem(
          icon: CupertinoIcons.question_circle,
          title: l10n.support,
          onTap: () {
            // Support functionality to be implemented
          },
        ),
      ],
    );
  }
}

class _SettingsMenuItem extends StatelessWidget {
  const _SettingsMenuItem({
    required this.icon,
    required this.title,
    required this.onTap,
  });

  final IconData icon;
  final String title;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: CupertinoButton(
        padding: EdgeInsets.zero,
        onPressed: onTap,
        child: Container(
          width: double.infinity,
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(
              color: AppColors.lightGray.withValues(alpha: 0.8),
            ),
            boxShadow: [
              BoxShadow(
                color: AppColors.lightGray.withValues(alpha: 0.2),
                blurRadius: 6,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Row(
            textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
            children: [
              Container(
                width: 44,
                height: 44,
                decoration: BoxDecoration(
                  color: AppColors.mainBlue.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(icon, color: AppColors.mainBlue, size: 22),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Text(
                  title,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: AppColors.darkGray,
                  ),
                ),
              ),
              Icon(
                isRTL
                    ? CupertinoIcons.chevron_back
                    : CupertinoIcons.chevron_forward,
                color: AppColors.mediumBlueGray,
                size: 18,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _SettingsSwitchItem extends StatelessWidget {
  const _SettingsSwitchItem({
    required this.icon,
    required this.title,
    required this.value,
    required this.onChanged,
    this.isLoading = false,
  });

  final IconData icon;
  final String title;
  final bool value;
  final ValueChanged<bool> onChanged;
  final bool isLoading;

  @override
  Widget build(BuildContext context) {
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Container(
        width: double.infinity,
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: AppColors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: AppColors.lightGray.withValues(alpha: 0.8)),
          boxShadow: [
            BoxShadow(
              color: AppColors.lightGray.withValues(alpha: 0.2),
              blurRadius: 6,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Row(
          textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
          children: [
            Container(
              width: 44,
              height: 44,
              decoration: BoxDecoration(
                color: AppColors.mainBlue.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(icon, color: AppColors.mainBlue, size: 22),
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Text(
                title,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w600,
                  color: AppColors.darkGray,
                ),
              ),
            ),
            isLoading
                ? const CupertinoActivityIndicator(radius: 10)
                : CupertinoSwitch(
                    value: value,
                    onChanged: isLoading ? null : onChanged,
                    activeTrackColor: AppColors.mainBlue,
                  ),
          ],
        ),
      ),
    );
  }
}

class _LanguageSwitcherRow extends StatelessWidget {
  static const _languages = ['ar', 'en'];

  String _labelForLanguage(String code, AppLocalizations l10n) {
    switch (code) {
      case 'ar':
        return l10n.languageArabic;
      case 'en':
        return l10n.languageEnglish;
      default:
        return code;
    }
  }

  void _showLanguagePicker(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final localeState = context.read<LocaleBloc>().state;
    final currentCode = localeState is LocaleLoaded
        ? localeState.locale.languageCode
        : localeState is LocaleInitial
        ? localeState.locale.languageCode
        : 'ar';

    showCupertinoModalPopup<void>(
      context: context,
      builder: (ctx) => CupertinoActionSheet(
        actions: [
          for (final code in _languages)
            CupertinoActionSheetAction(
              onPressed: () {
                Navigator.of(ctx).pop();
                if (code != currentCode) {
                  context.read<LocaleBloc>().add(LocaleChanged(code));
                }
              },
              child: Text(
                _labelForLanguage(code, l10n),
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
          onPressed: () => Navigator.of(ctx).pop(),
          child: Text(l10n.cancel),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return BlocBuilder<LocaleBloc, LocaleState>(
      builder: (context, localeState) {
        final currentCode = localeState is LocaleLoaded
            ? localeState.locale.languageCode
            : localeState is LocaleInitial
            ? localeState.locale.languageCode
            : 'ar';
        final currentLabel = _labelForLanguage(currentCode, l10n);

        return CupertinoButton(
          padding: EdgeInsets.zero,
          onPressed: () => _showLanguagePicker(context),
          child: Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 16),
            decoration: BoxDecoration(
              color: AppColors.white,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(
                color: AppColors.lightGray.withValues(alpha: 0.8),
              ),
              boxShadow: [
                BoxShadow(
                  color: AppColors.lightGray.withValues(alpha: 0.2),
                  blurRadius: 6,
                  offset: const Offset(0, 2),
                ),
              ],
            ),
            child: Row(
              textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
              children: [
                Container(
                  width: 44,
                  height: 44,
                  decoration: BoxDecoration(
                    color: AppColors.mainBlue.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: const Icon(
                    CupertinoIcons.globe,
                    color: AppColors.mainBlue,
                    size: 24,
                  ),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        l10n.language,
                        textAlign: TextAlign.start,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Text(
                        currentLabel,
                        textAlign: TextAlign.start,
                        style: const TextStyle(
                          fontSize: 14,
                          color: AppColors.mediumBlueGray,
                        ),
                      ),
                    ],
                  ),
                ),
                Icon(
                  isRTL
                      ? CupertinoIcons.chevron_back
                      : CupertinoIcons.chevron_forward,
                  size: 18,
                  color: AppColors.mediumBlueGray,
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}

class _LogoutButton extends StatelessWidget {
  const _LogoutButton({required this.onLogout});

  final VoidCallback onLogout;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return BlocBuilder<AuthBloc, AuthState>(
      builder: (context, authState) {
        final isLoading = authState is AuthLoading;

        return CupertinoButton(
          padding: EdgeInsets.zero,
          onPressed: isLoading ? null : onLogout,
          child: Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 16),
            decoration: BoxDecoration(
              color: isLoading
                  ? AppColors.lightGray
                  : AppColors.red.withValues(alpha: 0.12),
              borderRadius: BorderRadius.circular(16),
              border: isLoading
                  ? null
                  : Border.all(
                      color: AppColors.red.withValues(alpha: 0.4),
                      width: 1.5,
                    ),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
              children: [
                if (isLoading)
                  const CupertinoActivityIndicator(
                    color: AppColors.mediumBlueGray,
                    radius: 10,
                  )
                else ...[
                  const Icon(
                    CupertinoIcons.arrow_right_square,
                    size: 20,
                    color: AppColors.red,
                  ),
                  const SizedBox(width: 10),
                  Text(
                    l10n.logout,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                      color: AppColors.red,
                    ),
                  ),
                ],
              ],
            ),
          ),
        );
      },
    );
  }
}
