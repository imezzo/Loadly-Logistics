import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart'
    show RefreshIndicator, AlwaysScrollableScrollPhysics;
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/locale/bloc/locale_bloc.dart';
import '../../../../core/services/biometric_service.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/locale/bloc/locale_event.dart';
import '../../../../core/locale/bloc/locale_state.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../core/widgets/app_version.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../core/locale/user_facing_message.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_event.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../auth/domain/entities/collector_profile.dart';
import '../../../auth/presentation/utils/auth_error_extensions.dart';
import '../../../home/presentation/bloc/home_bloc.dart';
import '../../../home/presentation/views/home_screen.dart';
import '../bloc/profile_bloc.dart';
import '../bloc/profile_event.dart';
import '../bloc/profile_state.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  bool _fingerprintEnabled = false;
  bool _biometricAvailable = false;

  void _handleBackNavigation() => context.go('/home');

  static String? _loginFromProfile(CollectorProfile? profile) {
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
            onPressed: () async {
              final password = passwordController.text;
              final authPassword = await AuthStorage.instance.getauthPassword();
              passwordController.dispose();
              if (context.mounted) Navigator.of(ctx).pop();
              if (password.isEmpty) {
                onCancel();
                return;
              }
              if (password != authPassword) {
                if (context.mounted) {
                  showCupertinoDialog(
                    context: context,
                    builder: (ctx) => CupertinoAlertDialog(
                      title: Text(AppLocalizations.of(context)!.error),
                      content: Text(
                        AppLocalizations.of(context)!.incorrectPassword,
                      ),
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
                return;
              }
              if (context.mounted) {
                context.read<AuthBloc>().add(
                  AuthEnableBiometricRequested(
                    login: login,
                    password: password,
                    localizedAuthenticateReason:
                        l10n.biometricLocalizedReasonEnable,
                  ),
                );
              }
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
                // _fingerprintEnabled = p.biometricFingerprint ?? false;
                _fingerprintEnabled = fingerprint;
              });
            }
            if (profileState is ProfileUpdateSuccess) {
              if (context.mounted) {
                context.read<ProfileBloc>().add(const ProfileLoadRequested());
              }
            }
          },
        ),
        BlocListener<AuthBloc, AuthState>(
          listener: (context, authState) {
            if (authState is AuthUnauthenticated) {
              context.read<HomeBloc>().filterData = FilterData();
              context.go('/auth/login');
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
              final isBiometricError =
                  authState.errorCode != null &&
                  authState.errorCode!.contains('biometric');
              if (isBiometricError) {
                setState(() => _fingerprintEnabled = !_fingerprintEnabled);
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
                        localizeUserMessage(l10n, profileState.message),
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
                              avatarUrl: profile?.profilePicture,
                            ),
                            const SizedBox(height: 24),
                            // Settings Section
                            Padding(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 20,
                              ),
                              child: _ProfileSettingsSection(
                                profile: profile,
                                fingerprintEnabled: _fingerprintEnabled,
                                biometricAvailable: _biometricAvailable,
                                loginFromProfile: _loginFromProfile,
                                showEnableBiometricDialog:
                                    _showEnableBiometricPasswordDialog,
                                onFingerprintChanged: (v) =>
                                    setState(() => _fingerprintEnabled = v),
                              ),
                            ),
                            const SizedBox(height: 20),
                            // Language Switcher
                            Padding(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 20,
                              ),
                              child: _LanguageSwitcherRow(),
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
    this.avatarUrl,
  });

  final String name;
  final String email;
  final String? avatarUrl;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 20),
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(24),
        border: Border.all(color: AppColors.lightGray.withValues(alpha: 0.8)),
        boxShadow: [
          BoxShadow(
            color: AppColors.lightGray.withValues(alpha: 0.2),
            blurRadius: 6,
            offset: const Offset(0, 2),
          ),
        ],
      ),
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
                      : const _DefaultAvatar(size: 72, forDarkBackground: true),
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
                        color: AppColors.darkGray,
                        letterSpacing: -0.3,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    if (email.isNotEmpty) ...[
                      const SizedBox(height: 6),
                      Text(
                        email,
                        style: const TextStyle(
                          fontSize: 14,
                          color: CupertinoColors.systemGrey,
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
        ],
      ),
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

class _ProfileSettingsSection extends StatelessWidget {
  const _ProfileSettingsSection({
    required this.profile,
    required this.fingerprintEnabled,
    required this.biometricAvailable,
    required this.loginFromProfile,
    required this.showEnableBiometricDialog,
    required this.onFingerprintChanged,
  });

  final CollectorProfile? profile;
  final bool fingerprintEnabled;
  final bool biometricAvailable;
  final String? Function(CollectorProfile?) loginFromProfile;
  final void Function({
    required BuildContext context,
    required String login,
    required AppLocalizations l10n,
    required VoidCallback onCancel,
  })
  showEnableBiometricDialog;
  final ValueChanged<bool> onFingerprintChanged;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
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
      ],
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
