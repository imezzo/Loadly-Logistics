import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:slide_countdown/slide_countdown.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../widgets/auth_navbar.dart';
import '../widgets/password_field.dart';
import '../widgets/privacy_policy.dart';
import '../../../../core/widgets/text_field_error.dart';

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
    _otpController.addListener(() {
      setState(() {});
      if (_otpController.text.length == 6) _verify();
    });
  }

  @override
  void dispose() {
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
      final phoneNumber = digits.length >= 11
          ? digits.substring(digits.length - 11)
          : digits;
      bloc.add(
        AuthResetPasswordWithCodeRequested(
          code: code,
          login: phoneNumber.length == 11 ? '+2$phoneNumber' : widget.contact,
          newPassword: newPassword,
          confirmPassword: confirmPassword,
        ),
      );
      return;
    }

    // Login with OTP flow
    final token = bloc.loginOtpToken;
    final login = bloc.loginOtpPhone;
    if (token == null || login == null) return;

    bloc.add(AuthLoginWithOtpRequested(token: token, login: login, code: code));
  }

  void _resend() {
    if (widget.isForgetPassword) {
      // Extract 11-digit phone number from contact (remove any non-digits)
      final digits = extractDigits(widget.contact);
      final identifier = digits.length >= 11
          ? digits.substring(digits.length - 11)
          : digits;

      if (identifier.length == 11) {
        // Format as +2XXXXXXXXXXX
        final identifierWithPrefix = '+2$identifier';
        context.read<AuthBloc>().add(
          AuthForgetPasswordRequested(
            identifier: identifierWithPrefix,
            channel: 2, // 2 for SMS
          ),
        );
      } else {
        context.read<AuthBloc>().add(
          AuthForgetPasswordRequested(
            identifier: widget.contact,
            channel: 1, // 1 for email
          ),
        );
      }
    } else if (widget.isMfa) {
      // Resend OTP for mfa
      context.read<AuthBloc>().add(
        AuthSendMfaCodeRequested(channel: widget.viaSms ? 2 : 1),
      );
    } else {
      // Resend OTP for normal login flow
      final bloc = context.read<AuthBloc>();
      bloc.add(AuthSendLoginOtpRequested(login: widget.contact));
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
            _passwordError = state.message;
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
            onPopInvokedWithResult: (bool didPop, dynamic result) {
              if (!didPop) context.go('/login');
            },
            child: CupertinoPageScaffold(
              backgroundColor: AppColors.white,
              navigationBar: const AuthNavbar(),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Divider(thickness: 0.5),
                  Expanded(
                    child: SingleChildScrollView(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 15,
                      ).copyWith(top: 20),
                      child: ConstrainedBox(
                        constraints: BoxConstraints(
                          minHeight:
                              MediaQuery.of(context).size.height -
                              MediaQuery.of(context).padding.top -
                              MediaQuery.of(context).padding.bottom -
                              80,
                        ),
                        child: IntrinsicHeight(
                          child: Column(
                            children: [
                              Text(
                                widget.isForgetPassword
                                    ? l10n.forgetPassword
                                    : l10n.verificationTitle,
                                textAlign: TextAlign.center,
                                style: const TextStyle(
                                  fontSize: 24,
                                  fontWeight: FontWeight.w500,
                                  color: CupertinoColors.black,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                instruction,
                                textAlign: TextAlign.center,
                                style: const TextStyle(
                                  fontSize: 14,
                                  color: CupertinoColors.systemGrey,
                                  fontWeight: FontWeight.w400,
                                ),
                              ),
                              const SizedBox(height: 40),
                              Stack(
                                alignment: Alignment.center,
                                children: [
                                  Directionality(
                                    textDirection: TextDirection.ltr,
                                    child: Row(
                                      mainAxisAlignment:
                                          MainAxisAlignment.center,
                                      crossAxisAlignment:
                                          CrossAxisAlignment.center,
                                      mainAxisSize: MainAxisSize.min,
                                      children: [
                                        for (int i = 0; i < 6; i++) ...[
                                          _OtpDigitBox(
                                            character:
                                                _otpController.text.length > i
                                                ? _otpController.text[i]
                                                : null,
                                          ),
                                          if (i < 5) const SizedBox(width: 6),
                                        ],
                                      ],
                                    ),
                                  ),
                                  Positioned.fill(
                                    child: CupertinoTextField(
                                      controller: _otpController,
                                      focusNode: _otpFocusNode,
                                      autofocus: true,
                                      onTapOutside: (event) {
                                        FocusManager.instance.primaryFocus
                                            ?.unfocus();
                                      },
                                      keyboardType: TextInputType.text,
                                      maxLength: 6,
                                      inputFormatters: [
                                        if (widget.isMfa)
                                          FilteringTextInputFormatter
                                              .digitsOnly,
                                        LengthLimitingTextInputFormatter(6),
                                      ],
                                      style: const TextStyle(
                                        color: Color(0x00000000),
                                        fontSize: 1,
                                      ),
                                      cursorColor: const Color(0x00000000),
                                      decoration: const BoxDecoration(),
                                    ),
                                  ),
                                ],
                              ),
                              Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  Text(
                                    l10n.resendPrompt,
                                    style: const TextStyle(
                                      fontSize: 14,
                                      color: CupertinoColors.systemGrey,
                                      fontWeight: FontWeight.w400,
                                    ),
                                  ),
                                  if (!countdown)
                                    Directionality(
                                      textDirection: TextDirection.ltr,
                                      child: SlideCountdownSeparated(
                                        duration: const Duration(seconds: 59),
                                        shouldShowDays: (duration) => true,
                                        decoration: const BoxDecoration(
                                          color: Colors.transparent,
                                        ),
                                        style: const TextStyle(
                                          color: AppColors.mainBlue,
                                        ),
                                        onDone: () => setState(() {
                                          countdown = true;
                                        }),
                                      ),
                                    ),
                                  if (countdown)
                                    CupertinoButton(
                                      padding: EdgeInsets.zero,
                                      onPressed: _resend,
                                      child: Text(
                                        l10n.resendLink,
                                        style: const TextStyle(
                                          color: AppColors.mainBlue,
                                          fontSize: 15,
                                          fontWeight: FontWeight.w500,
                                        ),
                                      ),
                                    ),
                                ],
                              ),
                              TextFieldError(error: _passwordError),
                              if (widget.isForgetPassword) ...[
                                const Spacer(),
                                PasswordField(
                                  controller: _newPasswordController,
                                  onChanged: (_) {
                                    _validatePasswords(l10n);
                                    setState(() {});
                                  },
                                ),
                                TextFieldError(error: _newPasswordError),
                                const SizedBox(height: 10),
                                PasswordField(
                                  confirmPassword: true,
                                  controller: _confirmPasswordController,
                                  onChanged: (_) {
                                    _validatePasswords(l10n);
                                    setState(() {});
                                  },
                                ),
                                TextFieldError(error: _confirmPasswordError),
                              ],
                              const Spacer(),
                              SizedBox(
                                width: double.infinity,
                                child: AppPrimaryButton(
                                  label: l10n.continuee,
                                  onPressed: _canSubmit ? _verify : null,
                                  loading: isLoading,
                                ),
                              ),
                              const SizedBox(height: 10),
                              SizedBox(
                                width: double.infinity,
                                child: AppSecondaryButton(
                                  isLoading: isLoading,
                                  onPressed: () => context.go(
                                    widget.isForgetPassword
                                        ? '/auth/login'
                                        : '/login',
                                  ),
                                ),
                              ),
                              const PrivacyPolicy(),
                            ],
                          ),
                        ),
                      ),
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

class _OtpDigitBox extends StatelessWidget {
  const _OtpDigitBox({this.character});

  final String? character;

  @override
  Widget build(BuildContext context) {
    final isEmpty = character == null || character!.isEmpty;
    return SizedBox(
      width: 50,
      height: 56,
      child: Container(
        decoration: BoxDecoration(
          color: AppColors.white,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(color: AppColors.lightGray),
        ),
        alignment: Alignment.center,
        child: Center(
          child: Text(
            isEmpty ? '-' : character!,
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: isEmpty ? AppColors.mediumBlueGray : AppColors.darkGray,
            ),
            textAlign: TextAlign.center,
          ),
        ),
      ),
    );
  }
}
