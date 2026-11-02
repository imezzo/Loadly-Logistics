import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_pattern_navigation_bar.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../utils/auth_error_extensions.dart';

class ChangePasswordScreen extends StatefulWidget {
  const ChangePasswordScreen({super.key});

  @override
  State<ChangePasswordScreen> createState() => _ChangePasswordScreenState();
}

class _ChangePasswordScreenState extends State<ChangePasswordScreen> {
  final _currentPasswordController = TextEditingController();
  final _newPasswordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  final _currentPasswordFocusNode = FocusNode();
  final _newPasswordFocusNode = FocusNode();
  final _confirmPasswordFocusNode = FocusNode();

  String? _currentPasswordError;
  String? _newPasswordError;
  String? _confirmPasswordError;
  bool _obscureCurrentPassword = true;
  bool _obscureNewPassword = true;
  bool _obscureConfirmPassword = true;

  void _handleBackNavigation() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go(AppRoutePaths.settings);
    }
  }

  @override
  void dispose() {
    _currentPasswordController.dispose();
    _newPasswordController.dispose();
    _confirmPasswordController.dispose();
    _currentPasswordFocusNode.dispose();
    _newPasswordFocusNode.dispose();
    _confirmPasswordFocusNode.dispose();
    super.dispose();
  }

  String? _validateCurrentPassword(AppLocalizations l10n) {
    if (_currentPasswordController.text.isEmpty) {
      return l10n.enterYourPassword;
    }
    return null;
  }

  String? _validateNewPassword(AppLocalizations l10n) {
    if (_newPasswordController.text.isEmpty) {
      return l10n.enterYourPassword;
    }
    if (_newPasswordController.text.length < 6) {
      return l10n.passwordMinLength;
    }
    return null;
  }

  String? _validateConfirmPassword(AppLocalizations l10n) {
    if (_confirmPasswordController.text.isEmpty) {
      return l10n.pleaseConfirmYourPassword;
    }
    if (_confirmPasswordController.text != _newPasswordController.text) {
      return l10n.passwordsDoNotMatch;
    }
    return null;
  }

  void _validateFields(AppLocalizations l10n) {
    setState(() {
      _currentPasswordError = _validateCurrentPassword(l10n);
      _newPasswordError = _validateNewPassword(l10n);
      _confirmPasswordError = _validateConfirmPassword(l10n);
    });
  }

  void _submit(AppLocalizations l10n) {
    _validateFields(l10n);
    if (_currentPasswordError != null ||
        _newPasswordError != null ||
        _confirmPasswordError != null) {
      return;
    }

    context.read<AuthBloc>().add(
      AuthChangePasswordRequested(
        currentPassword: _currentPasswordController.text,
        newPassword: _newPasswordController.text,
        confirmPassword: _confirmPasswordController.text,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return MultiBlocListener(
      listeners: [
        BlocListener<AuthBloc, AuthState>(
          listener: (context, authState) {
            if (authState is AuthChangePasswordSuccess) {
              showCupertinoDialog(
                context: context,
                builder: (dialogContext) => CupertinoAlertDialog(
                  title: Text(l10n.ok),
                  content: Text(l10n.passwordChangedSuccessfully),
                  actions: [
                    CupertinoDialogAction(
                      isDefaultAction: true,
                      onPressed: () {
                        Navigator.of(dialogContext).pop();
                        context.go(AppRoutePaths.passwordLogin);
                      },
                      child: Text(l10n.ok),
                    ),
                  ],
                ),
              );
            } else if (authState is AuthError) {
              // Show error dialog
              final displayMessage = authState.getDisplayMessage(l10n);
              showCupertinoDialog(
                context: context,
                builder: (context) => CupertinoAlertDialog(
                  title: Text(l10n.error),
                  content: Text(displayMessage),
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
      child: PopScope(
        canPop: false,
        onPopInvokedWithResult: (didPop, result) {
          if (!didPop) _handleBackNavigation();
        },
        child: CupertinoPageScaffold(
          navigationBar: AppPatternNavigationBar(
            leading: AppBackButton(onPressed: _handleBackNavigation),
            middle: Text(
              l10n.changePassword,
              style: const TextStyle(
                color: AppColors.white,
                fontWeight: FontWeight.bold,
                fontSize: 16,
              ),
            ),
          ),
          backgroundColor: AppColors.mainBlue,
          child: MainScaffold(
            child: Container(
              color: AppColors.white,
              child: Column(
                children: [
                  // Header
                  // _ChangePasswordHeader(
                  //   title: l10n.changePassword,
                  //   onBack: _handleBackNavigation,
                  // ),
                  // Content
                  Expanded(
                    child: SingleChildScrollView(
                      padding: const EdgeInsets.all(24),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          const SizedBox(height: 20),
                          // Current Password Field
                          _PasswordField(
                            controller: _currentPasswordController,
                            focusNode: _currentPasswordFocusNode,
                            label: l10n.password,
                            placeholder: l10n.enterYourPassword,
                            error: _currentPasswordError,
                            obscureText: _obscureCurrentPassword,
                            onChanged: () {
                              setState(() {
                                _currentPasswordError = null;
                              });
                            },
                            onToggleVisibility: () {
                              setState(() {
                                _obscureCurrentPassword =
                                    !_obscureCurrentPassword;
                              });
                            },
                          ),
                          const SizedBox(height: 20),
                          // New Password Field
                          _PasswordField(
                            controller: _newPasswordController,
                            focusNode: _newPasswordFocusNode,
                            label: l10n.newPassword,
                            placeholder: l10n.enterYourPassword,
                            error: _newPasswordError,
                            obscureText: _obscureNewPassword,
                            onChanged: () {
                              setState(() {
                                _newPasswordError = null;
                                _confirmPasswordError =
                                    null; // Revalidate confirm password
                              });
                            },
                            onToggleVisibility: () {
                              setState(() {
                                _obscureNewPassword = !_obscureNewPassword;
                              });
                            },
                          ),
                          const SizedBox(height: 20),
                          // Confirm Password Field
                          _PasswordField(
                            controller: _confirmPasswordController,
                            focusNode: _confirmPasswordFocusNode,
                            label: l10n.confirmPassword,
                            placeholder: l10n.confirmPassword,
                            error: _confirmPasswordError,
                            obscureText: _obscureConfirmPassword,
                            onChanged: () {
                              setState(() {
                                _confirmPasswordError = null;
                              });
                            },
                            onToggleVisibility: () {
                              setState(() {
                                _obscureConfirmPassword =
                                    !_obscureConfirmPassword;
                              });
                            },
                          ),
                          const SizedBox(height: 32),
                          // Submit Button
                          BlocBuilder<AuthBloc, AuthState>(
                            builder: (context, authState) {
                              final isLoading = authState is AuthLoading;
                              return CupertinoButton(
                                padding: EdgeInsets.zero,
                                onPressed: isLoading ? null : () => _submit(l10n),
                                child: Container(
                                  width: double.infinity,
                                  padding: const EdgeInsets.symmetric(
                                    vertical: 16,
                                  ),
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
                                        l10n.resetPassword,
                                        style: const TextStyle(
                                          fontSize: 16,
                                          fontWeight: FontWeight.w600,
                                          color: AppColors.white,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                              );
                            },
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _PasswordField extends StatelessWidget {
  const _PasswordField({
    required this.controller,
    required this.focusNode,
    required this.label,
    required this.placeholder,
    required this.error,
    required this.obscureText,
    required this.onChanged,
    required this.onToggleVisibility,
  });

  final TextEditingController controller;
  final FocusNode focusNode;
  final String label;
  final String placeholder;
  final String? error;
  final bool obscureText;
  final VoidCallback onChanged;
  final VoidCallback onToggleVisibility;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          label,
          style: const TextStyle(
            color: AppColors.darkGray,
            fontSize: 15,
            fontWeight: FontWeight.w500,
          ),
        ),
        const SizedBox(height: 8),
        Container(
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(10),
            border: Border.all(
              color: error != null
                  ? CupertinoColors.systemRed
                  : AppColors.lightGray,
            ),
          ),
          child: Row(
            textDirection: TextDirection.ltr,
            children: [
              Expanded(
                child: CupertinoTextField(
                  controller: controller,
                  focusNode: focusNode,
                  placeholder: placeholder,
                  obscureText: obscureText,
                  onChanged: (_) => onChanged(),
                  padding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 14,
                  ),
                  decoration: null,
                ),
              ),
              CupertinoButton(
                padding: const EdgeInsets.symmetric(horizontal: 12),
                minimumSize: Size.zero,
                onPressed: onToggleVisibility,
                child: Icon(
                  obscureText ? CupertinoIcons.eye_slash : CupertinoIcons.eye,
                  color: AppColors.mediumBlueGray,
                  size: 20,
                ),
              ),
            ],
          ),
        ),
        if (error != null) ...[
          const SizedBox(height: 8),
          Text(
            error!,
            style: const TextStyle(
              color: CupertinoColors.systemRed,
              fontSize: 13,
            ),
          ),
        ],
      ],
    );
  }
}
