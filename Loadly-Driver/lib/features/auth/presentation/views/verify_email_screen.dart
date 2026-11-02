import 'package:flutter/cupertino.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_input.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../profile/presentation/bloc/profile_bloc.dart';
import '../../../profile/presentation/bloc/profile_event.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../utils/auth_error_extensions.dart';

class VerifyEmailScreen extends StatefulWidget {
  const VerifyEmailScreen({super.key});

  @override
  State<VerifyEmailScreen> createState() => _VerifyEmailScreenState();
}

class _VerifyEmailScreenState extends State<VerifyEmailScreen> {
  final _codeController = TextEditingController();
  final _codeFocusNode = FocusNode();
  String? _codeError;

  void _handleBackNavigation() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go('/profile');
    }
  }

  @override
  void initState() {
    super.initState();
    // Request verification code (email) when screen opens so driver receives it. channel 1 = email
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        context.read<AuthBloc>().add(
          const AuthResendVerificationRequested(channel: 1),
        );
      }
    });
  }

  @override
  void dispose() {
    _codeController.dispose();
    _codeFocusNode.dispose();
    super.dispose();
  }

  void _submit(AppLocalizations l10n) {
    final code = _codeController.text.trim();
    if (code.isEmpty) {
      setState(() => _codeError = l10n.enterVerificationCode);
      return;
    }
    if (code.length != 6) {
      setState(() => _codeError = l10n.verificationCodeLength);
      return;
    }
    setState(() => _codeError = null);
    context.read<AuthBloc>().add(AuthVerifyEmailRequested(code: code));
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return MultiBlocListener(
      listeners: [
        BlocListener<AuthBloc, AuthState>(
          listener: (context, authState) {
            if (authState is AuthVerifyEmailSuccess) {
              context.read<ProfileBloc>().add(const ProfileLoadRequested());
              if (context.canPop()) {
                context.pop();
              } else {
                context.go('/profile');
              }
            } else if (authState is AuthResendVerificationSuccess) {
              final l10n = AppLocalizations.of(context)!;
              showCupertinoDialog(
                context: context,
                builder: (context) => CupertinoAlertDialog(
                  content: Text(l10n.codeSentToEmail),
                  actions: [
                    CupertinoDialogAction(
                      isDefaultAction: true,
                      onPressed: () => Navigator.of(context).pop(),
                      child: Text(l10n.ok),
                    ),
                  ],
                ),
              );
            } else if (authState is AuthError) {
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
          backgroundColor: AppColors.white,
          child: MainScaffold(
            child: Column(
              children: [
                _VerifyEmailHeader(
                  title: l10n.verifyEmail,
                  onBack: _handleBackNavigation,
                ),
                Expanded(
                  child: SingleChildScrollView(
                    padding: const EdgeInsets.all(24),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        const SizedBox(height: 20),
                        Text(
                          l10n.verifyEmailInstruction,
                          style: const TextStyle(
                            color: AppColors.darkGray,
                            fontSize: 15,
                          ),
                        ),
                        const SizedBox(height: 24),
                        AppInput(
                          controller: _codeController,
                          label: l10n.verificationTitle,
                          placeholder: '123456',
                          keyboardType: TextInputType.number,
                          maxLength: 6,
                          inputFormatters: [
                            FilteringTextInputFormatter.digitsOnly,
                          ],
                          onChanged: (_) {
                            setState(() => _codeError = null);
                            if (_codeController.text.length == 6) _submit(l10n);
                          },
                        ),
                        if (_codeError != null) ...[
                          const SizedBox(height: 8),
                          Text(
                            _codeError!,
                            style: const TextStyle(
                              color: CupertinoColors.systemRed,
                              fontSize: 13,
                            ),
                          ),
                        ],
                        const SizedBox(height: 16),
                        BlocBuilder<AuthBloc, AuthState>(
                          builder: (context, authState) {
                            final isLoading = authState is AuthLoading;
                            return CupertinoButton(
                              padding: EdgeInsets.zero,
                              onPressed: isLoading
                                  ? null
                                  : () => context.read<AuthBloc>().add(
                                      const AuthResendVerificationRequested(
                                        channel: 1,
                                      ),
                                    ),
                              child: Text(
                                l10n.resendCode,
                                style: TextStyle(
                                  fontSize: 15,
                                  color: isLoading
                                      ? AppColors.lightGray
                                      : AppColors.mainBlue,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            );
                          },
                        ),
                        const SizedBox(height: 24),
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
                                      l10n.verifyAccount,
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
    );
  }
}

class _VerifyEmailHeader extends StatelessWidget {
  const _VerifyEmailHeader({required this.title, required this.onBack});

  final String title;
  final VoidCallback onBack;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      height: 100,
      color: AppColors.mainBlue,
      child: SafeArea(
        bottom: false,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 12),
          child: Row(
            children: [
              AppBackButton(onPressed: onBack),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  title,
                  style: const TextStyle(
                    color: AppColors.white,
                    fontSize: 17,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
