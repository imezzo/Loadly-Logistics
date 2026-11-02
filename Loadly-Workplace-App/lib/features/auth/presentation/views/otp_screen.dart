import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart'
    show Material, MaterialType, TextField, InputDecoration, OutlineInputBorder;
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:slide_countdown/slide_countdown.dart';

import '../../../../core/router/app_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../utils/auth_error_extensions.dart';

class OtpScreen extends StatefulWidget {
  const OtpScreen({
    super.key,
    required this.contact,
    this.viaSms = true,
    this.isForgetPassword = false,
    this.isMfa = false,
  });

  final String contact;
  final bool viaSms;
  final bool isForgetPassword;
  final bool isMfa;

  @override
  State<OtpScreen> createState() => _OtpScreenState();
}

class _OtpScreenState extends State<OtpScreen> {
  final TextEditingController _otpController = TextEditingController();
  final TextEditingController _newPasswordController = TextEditingController();
  final TextEditingController _confirmPasswordController =
      TextEditingController();
  final FocusNode _otpFocusNode = FocusNode();
  String? _passwordError;
  String? _newPasswordError;
  String? _confirmPasswordError;
  bool countdown = false;

  @override
  void initState() {
    super.initState();
    AuthScreenGuard.isOnAuthScreen = true;
    _otpController.addListener(() {
      setState(() {});
      if (_otpController.text.length == 6) _verify();
    });
  }

  @override
  void dispose() {
    AuthScreenGuard.isOnAuthScreen = false;
    _otpController.dispose();
    _newPasswordController.dispose();
    _confirmPasswordController.dispose();
    _otpFocusNode.dispose();
    super.dispose();
  }

  String get _code => _otpController.text;

  void _validatePasswords(AppLocalizations l10n) {
    final newPwd = _newPasswordController.text;
    final confirm = _confirmPasswordController.text;

    // Validate new password
    if (newPwd.isNotEmpty && newPwd.length < 6) {
      _newPasswordError = l10n.passwordMinLength;
    } else {
      _newPasswordError = null;
    }

    // Validate confirm password
    if (confirm.isNotEmpty) {
      if (confirm.length < 6) {
        _confirmPasswordError = l10n.passwordMinLength;
      } else if (newPwd.isNotEmpty && newPwd != confirm) {
        _confirmPasswordError = l10n.passwordsDoNotMatch;
      } else {
        _confirmPasswordError = null;
      }
    } else {
      _confirmPasswordError = null;
    }
  }

  bool get _canSubmit {
    if (_code.length != 6) return false;
    if (widget.isForgetPassword) {
      final newPwd = _newPasswordController.text;
      final confirm = _confirmPasswordController.text;
      return newPwd.length >= 6 &&
          confirm.length >= 6 &&
          newPwd == confirm &&
          _newPasswordError == null &&
          _confirmPasswordError == null;
    }
    return true;
  }

  void _verify() {
    final l10n = AppLocalizations.of(context)!;
    if (widget.isForgetPassword) {
      _validatePasswords(l10n);
      setState(() {});
    }

    if (!_canSubmit) return;

    final bloc = context.read<AuthBloc>();
    final code = _code;
    if (code.length != 6) return;

    if (widget.isMfa) {
      // MFA verification flow
      bloc.add(AuthVerifyMfaRequested(code: code));
      return;
    }

    if (widget.isForgetPassword) {
      final newPassword = _newPasswordController.text;
      final confirmPassword = _confirmPasswordController.text;
      if (newPassword != confirmPassword) {
        setState(() => _passwordError = l10n.passwordsDoNotMatch);
        return;
      }
      setState(() {
        _passwordError = null;
      });
      // Extract phone number with +2 prefix
      final digits = extractDigits(widget.contact);
      final identifier = digits.length >= 11
          ? digits.substring(digits.length - 11)
          : digits;
      bloc.add(
        AuthResetPasswordWithCodeRequested(
          code: code,
          login: identifier.length == 11 ? '+2$identifier' : widget.contact,
          newPassword: newPassword,
          confirmPassword: confirmPassword,
        ),
      );
      return;
    }
  }

  void _resend() {
    if (widget.isForgetPassword) {
      // Extract 11-digit phone number from contact (remove any non-digits)
      final digits = extractDigits(widget.contact);
      final identifier = digits.length >= 11
          ? digits.substring(digits.length - 11)
          : digits;

      final bloc = context.read<AuthBloc>();
      if (identifier.length == 11) {
        // Format as +2XXXXXXXXXXX
        final identifierWithPrefix = '+2$identifier';
        bloc.add(
          AuthForgetPasswordRequested(
            identifier: identifierWithPrefix,
            channel: 2, // 2 for SMS
          ),
        );
      } else {
        bloc.add(
          AuthForgetPasswordRequested(
            identifier: widget.contact,
            channel: 1, // 2 for email
          ),
        );
      }
    } else {
      // Resend OTP for mfa
      context.read<AuthBloc>().add(
        AuthSendMfaCodeRequested(channel: widget.viaSms ? 2 : 1),
      );
    }
    setState(() {
      countdown = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final instruction = widget.viaSms
        ? l10n.otpInstructionSms(widget.contact)
        : l10n.otpInstructionEmail(widget.contact);

    return BlocListener<AuthBloc, AuthState>(
      listener: (context, state) {
        if (state is AuthError) {
          setState(() {
            _passwordError = state.getDisplayMessage(l10n);
          });
        } else if (state is AuthForgetPasswordSuccess) {
          // Clear OTP field when resend succeeds
          _otpController.clear();
          setState(() {
            _passwordError = null;
          });
        } else if (state is AuthResetPasswordSuccess) {
          Future.microtask(() {
            if (context.mounted) {
              context.go('/auth/login');
            }
          });
        } else if (state is AuthLoginSuccess) {
          // Check redirect field to determine navigation
          Future.microtask(() async {
            if (!context.mounted) return;
            await handleLoginNavigationWithAppUpdate(
              context: context,
              navigate: () {
                if (!context.mounted) return;
                if (state.loginResult.redirect == 'mfa') {
                  final email = state.loginResult.email ?? '';
                  final phone = state.loginResult.phone ?? '';
                  context.go(
                    '/mfa-channel?email=${Uri.encodeComponent(email)}&phone=${Uri.encodeComponent(phone)}',
                  );
                } else if (state.loginResult.redirect == 'forget_password' &&
                    state.loginResult.phoneForOtp != null) {
                  // Already on OTP screen, but redirect indicates forget password flow
                  // This shouldn't normally happen, but handle it gracefully
                  final contact = Uri.encodeComponent(
                    state.loginResult.phoneForOtp!,
                  );
                  final isPhone = !state.loginResult.phoneForOtp!.contains('@');
                  context.go(
                    '/otp?contact=$contact&viaSms=$isPhone&forgetPassword=true',
                  );
                } else {
                  // Navigate to home after successful login with OTP
                  // Use go() to replace the entire navigation stack
                  context.go('/home');
                }
              },
            );
          });
        }
      },
      child: BlocBuilder<AuthBloc, AuthState>(
        builder: (context, authState) {
          final isLoading = authState is AuthLoading;

          return PopScope(
            canPop: false,
            // Do not navigate in onPopInvokedWithResult: opening/dismissing
            // the system notification center can trigger a pop attempt and
            // would incorrectly send the user back to login. Use the explicit
            // back button in the header instead.
            child: CupertinoPageScaffold(
              // Use mainBlue so the area under the keyboard isn't black when
              // the scaffold is resized (transparent would show black).
              backgroundColor: AppColors.mainBlue,
              child: Column(
                children: [
                  Stack(
                    clipBehavior: Clip.none,
                    children: [
                      const _OtpHeader(),
                      Positioned(
                        top: 0,
                        left: 0,
                        right: 0,
                        child: SafeArea(
                          bottom: false,
                          child: Padding(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 8,
                              vertical: 8,
                            ),
                            child: Align(
                              alignment: AlignmentDirectional.centerStart,
                              child: AppBackButton(
                                onPressed: () => context.go('/auth/login'),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                  Expanded(
                    child: Stack(
                      clipBehavior: Clip.none,
                      children: [
                        Positioned(
                          top: AppLayout.authContentTop,
                          left: 0,
                          right: 0,
                          bottom: 0,
                          child: ClipRRect(
                            borderRadius: AppLayout.authContentBorderRadius,
                            child: Container(
                              color: AppColors.white,
                              child: Column(
                                children: [
                                  Expanded(
                                    child: SingleChildScrollView(
                                      padding: const EdgeInsets.fromLTRB(
                                        24,
                                        0,
                                        24,
                                        8,
                                      ),
                                      child: AutofillGroup(
                                        child: Material(
                                          type: MaterialType.transparency,
                                          child: Column(
                                            crossAxisAlignment:
                                                CrossAxisAlignment.stretch,
                                            children: [
                                              const SizedBox(height: 40),
                                              Align(
                                                alignment: AlignmentDirectional
                                                    .centerStart,
                                                child: Image.asset(
                                                  'assets/images/loadly_logo.png',
                                                  width: 48,
                                                  height: 58,
                                                  fit: BoxFit.contain,
                                                ),
                                              ),
                                              const SizedBox(height: 20),
                                              Text(
                                                l10n.verificationTitle,
                                                style:
                                                    CupertinoTheme.of(context)
                                                        .textTheme
                                                        .navLargeTitleTextStyle
                                                        .copyWith(
                                                          color: AppColors
                                                              .darkGray,
                                                          fontWeight:
                                                              FontWeight.bold,
                                                        ),
                                              ),
                                              const SizedBox(height: 8),
                                              Text(
                                                instruction,
                                                style:
                                                    CupertinoTheme.of(context)
                                                        .textTheme
                                                        .textStyle
                                                        .copyWith(
                                                          color: AppColors
                                                              .mediumBlueGray,
                                                          fontSize: 15,
                                                          height: 1.4,
                                                        ),
                                              ),
                                              const SizedBox(height: 20),
                                              Directionality(
                                                textDirection:
                                                    TextDirection.ltr,
                                                child: Material(
                                                  type:
                                                      MaterialType.transparency,
                                                  child: TextField(
                                                    controller: _otpController,
                                                    focusNode: _otpFocusNode,
                                                    autofocus: true,
                                                    maxLength: 6,
                                                    textAlign: TextAlign.center,
                                                    keyboardType:
                                                        TextInputType.text,
                                                    autofillHints: const [
                                                      AutofillHints.oneTimeCode,
                                                    ],
                                                    inputFormatters: [
                                                      if (widget.isMfa)
                                                        FilteringTextInputFormatter
                                                            .digitsOnly,
                                                      LengthLimitingTextInputFormatter(
                                                        6,
                                                      ),
                                                    ],
                                                    style: const TextStyle(
                                                      color: AppColors.darkGray,
                                                      fontSize: 24,
                                                      fontWeight:
                                                          FontWeight.bold,
                                                      letterSpacing: 8,
                                                    ),
                                                    decoration: InputDecoration(
                                                      counterText: '',
                                                      hintText: '------',
                                                      hintStyle:
                                                          const TextStyle(
                                                            color: AppColors
                                                                .mediumBlueGray,
                                                            fontSize: 24,
                                                            letterSpacing: 8,
                                                          ),
                                                      filled: true,
                                                      fillColor:
                                                          AppColors.white,
                                                      border: OutlineInputBorder(
                                                        borderRadius:
                                                            BorderRadius.circular(
                                                              10,
                                                            ),
                                                        borderSide:
                                                            const BorderSide(
                                                              color: AppColors
                                                                  .lightGray,
                                                            ),
                                                      ),
                                                      enabledBorder: OutlineInputBorder(
                                                        borderRadius:
                                                            BorderRadius.circular(
                                                              10,
                                                            ),
                                                        borderSide:
                                                            const BorderSide(
                                                              color: AppColors
                                                                  .lightGray,
                                                            ),
                                                      ),
                                                      focusedBorder:
                                                          OutlineInputBorder(
                                                            borderRadius:
                                                                BorderRadius.circular(
                                                                  10,
                                                                ),
                                                            borderSide:
                                                                const BorderSide(
                                                                  color: AppColors
                                                                      .mainBlue,
                                                                  width: 2,
                                                                ),
                                                          ),
                                                    ),
                                                    onSubmitted: (_) =>
                                                        _verify(),
                                                  ),
                                                ),
                                              ),
                                              if (widget.isForgetPassword) ...[
                                                const SizedBox(height: 24),
                                                _PasswordField(
                                                  controller:
                                                      _newPasswordController,
                                                  label: l10n.newPassword,
                                                  placeholder:
                                                      l10n.enterYourPassword,
                                                  error: _newPasswordError,
                                                  onChanged: () {
                                                    _validatePasswords(l10n);
                                                    setState(() {});
                                                  },
                                                ),
                                                const SizedBox(height: 16),
                                                _PasswordField(
                                                  controller:
                                                      _confirmPasswordController,
                                                  label: l10n.confirmPassword,
                                                  placeholder:
                                                      l10n.confirmPassword,
                                                  error: _confirmPasswordError,
                                                  onChanged: () {
                                                    _validatePasswords(l10n);
                                                    setState(() {});
                                                  },
                                                ),
                                                if (_passwordError != null) ...[
                                                  const SizedBox(height: 8),
                                                  Align(
                                                    alignment:
                                                        AlignmentDirectional
                                                            .centerStart,
                                                    child: Text(
                                                      _passwordError!,
                                                      style: const TextStyle(
                                                        color: CupertinoColors
                                                            .systemRed,
                                                        fontSize: 13,
                                                      ),
                                                    ),
                                                  ),
                                                ],
                                              ],
                                              const SizedBox(height: 12),
                                              Row(
                                                mainAxisAlignment:
                                                    MainAxisAlignment.center,
                                                children: [
                                                  Text(
                                                    l10n.resendPrompt,
                                                    style: const TextStyle(
                                                      color: AppColors
                                                          .mediumBlueGray,
                                                      fontSize: 14,
                                                    ),
                                                  ),
                                                  if (!countdown)
                                                    Directionality(
                                                      textDirection:
                                                          TextDirection.ltr,
                                                      child: SlideCountdownSeparated(
                                                        duration:
                                                            const Duration(
                                                              seconds: 59,
                                                            ),
                                                        shouldShowDays:
                                                            (duration) => true,
                                                        decoration:
                                                            const BoxDecoration(
                                                              color: CupertinoColors
                                                                  .transparent,
                                                            ),
                                                        style: const TextStyle(
                                                          color: AppColors
                                                              .mainBlue,
                                                        ),
                                                        onDone: () =>
                                                            setState(() {
                                                              countdown = true;
                                                            }),
                                                      ),
                                                    ),
                                                  if (countdown)
                                                    CupertinoButton(
                                                      padding:
                                                          const EdgeInsets.symmetric(
                                                            horizontal: 8,
                                                          ),
                                                      onPressed: _resend,
                                                      child: Text(
                                                        l10n.resendLink,
                                                        style: const TextStyle(
                                                          color: AppColors
                                                              .mainBlue,
                                                          fontSize: 14,
                                                          fontWeight:
                                                              FontWeight.w600,
                                                        ),
                                                      ),
                                                    ),
                                                ],
                                              ),
                                              const SizedBox(height: 16),
                                              AppButton(
                                                label: widget.isForgetPassword
                                                    ? l10n.resetPassword
                                                    : l10n.verifyAccount,
                                                onPressed: _canSubmit
                                                    ? _verify
                                                    : null,
                                                loading: isLoading,
                                              ),
                                              const SizedBox(height: 12),
                                            ],
                                          ),
                                        ),
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}

class _PasswordField extends StatelessWidget {
  const _PasswordField({
    required this.controller,
    required this.label,
    required this.placeholder,
    required this.onChanged,
    this.error,
  });

  final TextEditingController controller;
  final String label;
  final String placeholder;
  final VoidCallback onChanged;
  final String? error;

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
            border: Border.all(
              color: error != null
                  ? CupertinoColors.systemRed
                  : AppColors.lightGray,
            ),
          ),
          child: CupertinoTextField(
            controller: controller,
            placeholder: placeholder,
            obscureText: true,
            onChanged: (_) => onChanged(),
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            decoration: null,
          ),
        ),
        if (error != null) ...[
          const SizedBox(height: 8),
          Align(
            alignment: AlignmentDirectional.centerStart,
            child: Text(
              error!,
              style: const TextStyle(
                color: CupertinoColors.systemRed,
                fontSize: 13,
              ),
            ),
          ),
        ],
      ],
    );
  }
}

class _OtpHeader extends StatelessWidget {
  const _OtpHeader();

  @override
  Widget build(BuildContext context) {
    return ClipPath(
      clipper: _OtpHeaderClipper(),
      child: Container(
        width: double.infinity,
        height: AppLayout.authHeaderHeight,
        color: AppColors.mainBlue,
      ),
    );
  }
}

class _OtpHeaderClipper extends CustomClipper<Path> {
  @override
  Path getClip(Size size) {
    return Path()
      ..lineTo(size.width, 0)
      ..lineTo(size.width, size.height)
      ..quadraticBezierTo(
        size.width * 0.3,
        size.height - 40,
        0,
        size.height * 0.6,
      )
      ..lineTo(0, 0)
      ..close();
  }

  @override
  bool shouldReclip(covariant CustomClipper<Path> oldClipper) => false;
}
