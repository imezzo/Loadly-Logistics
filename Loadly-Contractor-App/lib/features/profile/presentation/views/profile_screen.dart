import 'dart:io';

import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart'
    show RefreshIndicator, AlwaysScrollableScrollPhysics, Divider;
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_main_tab_header.dart';
import '../../../../core/widgets/app_phone_input.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/domain/entities/contractor_profile.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../bloc/profile_bloc.dart';
import '../bloc/profile_event.dart';
import '../bloc/profile_state.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  @override
  void initState() {
    super.initState();
    // Load profile data after the first frame to ensure context has access to BlocProvider
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        context.read<ProfileBloc>().add(const ProfileLoadRequested());
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go('/settings');
        }
      },
      child: MultiBlocListener(
        listeners: [
          BlocListener<AuthBloc, AuthState>(
            listener: (context, authState) {
              // Navigate to login when logout succeeds
              if (authState is AuthUnauthenticated) {
                context.go('/login');
              }
            },
          ),
          BlocListener<ProfileBloc, ProfileState>(
            listener: (context, state) {
              if (state is ProfileLoaded) {
                Future.microtask(() async {
                  if (!context.mounted) return;
                  await presentPendingAppUpdate(context: context);
                });
              }
              if (state is ProfileUpdateSuccess) {
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
                    content: Text(
                      FailureMessageMapper.localizeProfileErrorMessage(
                        state.message,
                        l10n,
                      ),
                    ),
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
          ),
        ],
        child: BlocBuilder<ProfileBloc, ProfileState>(
          builder: (context, profileState) {
            if (profileState is ProfileLoading) {
              return const CupertinoPageScaffold(
                backgroundColor: AppColors.white,
                child: Center(
                  child: CupertinoActivityIndicator(color: AppColors.mainBlue),
                ),
              );
            }

            if (profileState is ProfileError) {
              final l10n = AppLocalizations.of(context)!;
              return CupertinoPageScaffold(
                backgroundColor: AppColors.white,
                child: Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        FailureMessageMapper.localizeProfileErrorMessage(
                          profileState.message,
                          l10n,
                        ),
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
              );
            }

            final profile = profileState is ProfileLoaded
                ? profileState.profile
                : null;
            return CupertinoPageScaffold(
              backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
              child: Column(
                children: [
                  const AppMainTabHeader(bodyHeight: 0),
                  Expanded(child: _PersonalDataTab(profile: profile)),
                ],
              ),
            );
          },
        ),
      ),
    );
  }
}

class _DefaultAvatar extends StatelessWidget {
  const _DefaultAvatar({this.size = 80});

  final double size;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      decoration: const BoxDecoration(
        color: AppColors.lightGray,
        shape: BoxShape.circle,
      ),
      child: Icon(
        CupertinoIcons.person_fill,
        size: size * 0.5,
        color: AppColors.mediumBlueGray,
      ),
    );
  }
}

class _PersonalDataTab extends StatefulWidget {
  const _PersonalDataTab({this.profile});

  final ContractorProfile? profile;

  @override
  State<_PersonalDataTab> createState() => _PersonalDataTabState();
}

class _PersonalDataTabState extends State<_PersonalDataTab> {
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
    String s = phone.trim().replaceFirst(RegExp(r'^\+'), '');
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
    _userNameController.text = profile?.userName ?? '';
    _emailController.text = profile?.email ?? '';
    _phoneController.text = _phoneForDisplay(profile?.phone);
  }

  @override
  void didChangeDependencies() {
    _updateControllers();
    super.didChangeDependencies();
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

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Column(
      children: [
        Expanded(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 5).copyWith(top: 5),
            child: ClipRRect(
              borderRadius: AppLayout.authContentBorderRadius,
              child: Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 15,
                  vertical: 15,
                ),
                color: AppColors.white,
                child: RefreshIndicator(
                  onRefresh: () async {
                    final bloc = context.read<ProfileBloc>();
                    bloc.add(const ProfileLoadRequested());
                    await bloc.stream
                        .where((s) => s is ProfileLoaded || s is ProfileError)
                        .first;
                  },
                  child: SingleChildScrollView(
                    physics: const AlwaysScrollableScrollPhysics(),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        Row(
                          children: [
                            AppButton(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 18,
                                vertical: 8,
                              ),
                              onPressed: () {
                                if (context.canPop()) {
                                  context.pop();
                                } else {
                                  context.go('/settings');
                                }
                              },
                            ),
                          ],
                        ),
                        const SizedBox(height: 20),
                        // Profile Picture Section
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Expanded(
                              child: Row(
                                children: [
                                  ClipOval(
                                    child: _profilePicturePath != null
                                        ? Image.file(
                                            File(_profilePicturePath!),
                                            width: 70,
                                            height: 70,
                                            fit: BoxFit.cover,
                                            errorBuilder:
                                                (context, error, stackTrace) =>
                                                    const _DefaultAvatar(
                                                      size: 70,
                                                    ),
                                          )
                                        : widget.profile?.profilePicture !=
                                                  null &&
                                              widget
                                                  .profile!
                                                  .profilePicture!
                                                  .isNotEmpty
                                        ? CachedNetworkImage(
                                            imageUrl:
                                                widget.profile!.profilePicture!,
                                            width: 70,
                                            height: 70,
                                            fit: BoxFit.cover,
                                            errorWidget:
                                                (context, url, error) =>
                                                    const _DefaultAvatar(
                                                      size: 70,
                                                    ),
                                          )
                                        : const _DefaultAvatar(size: 70),
                                  ),
                                  const SizedBox(width: 10),
                                  Flexible(
                                    child: FittedBox(
                                      child: Text(
                                        widget.profile?.fullName ?? '',
                                        style: const TextStyle(
                                          fontSize: 16,
                                          color: AppColors.darkGray,
                                          fontWeight: FontWeight.w500,
                                        ),
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 24),
                        const Divider(thickness: 0.5),
                        const SizedBox(height: 24),
                        // Full Name (English)
                        _EditableField(
                          label: l10n.fullName,
                          controller: _fullNameController,
                        ),
                        const SizedBox(height: 16),
                        // User Name
                        _EditableField(
                          label: l10n.userName,
                          controller: _userNameController,
                        ),
                        const SizedBox(height: 16),
                        // Email (with Verify button when not verified in API or when user changed email)
                        _EmailFieldWithVerify(
                          label: l10n.email,
                          controller: _emailController,
                          profileEmail: widget.profile?.email,
                        ),
                        const SizedBox(height: 16),
                        // Phone
                        AppPhoneInput(
                          controller: _phoneController,
                          label: l10n.phone,
                          placeholder: l10n.phone,
                        ),
                        const SizedBox(height: 32),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(10),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              AppButton(
                onPressed: () {
                  if (context.canPop()) {
                    context.pop();
                  } else {
                    context.go('/settings');
                  }
                },
                label: l10n.cancel,
              ),
              const SizedBox(width: 10),
              BlocBuilder<ProfileBloc, ProfileState>(
                builder: (context, profileState) {
                  final isLoading = profileState is ProfileLoading;
                  return AppButton(
                    onPressed: () => _submit(),
                    label: l10n.saveChanges,
                    loading: isLoading,
                  );
                },
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _EmailFieldWithVerify extends StatefulWidget {
  const _EmailFieldWithVerify({
    required this.label,
    required this.controller,
    this.profileEmail,
  });

  final String label;
  final TextEditingController controller;

  /// Original email from profile (API). When user edits email and it differs, show verify.
  final String? profileEmail;

  @override
  State<_EmailFieldWithVerify> createState() => _EmailFieldWithVerifyState();
}

class _EmailFieldWithVerifyState extends State<_EmailFieldWithVerify> {
  @override
  Widget build(BuildContext context) {
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
          ],
        ),
      ],
    );
  }
}

class _EditableField extends StatelessWidget {
  const _EditableField({required this.label, required this.controller});

  final String label;
  final TextEditingController controller;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Align(
          alignment: AlignmentDirectional.centerStart,
          child: Text(
            label,
            style: const TextStyle(
              color: AppColors.darkGray,
              fontSize: 15,
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
        const SizedBox(height: 8),
        Container(
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(10),
            border: Border.all(color: AppColors.lightGray),
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
