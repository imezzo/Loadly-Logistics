import 'package:flutter/cupertino.dart';
import 'package:google_fonts/google_fonts.dart';

import 'app_colors.dart';

/// Loadly design system theme.
/// English: Manrope, Arabic: Almarai.
CupertinoThemeData buildAppTheme([String? languageCode]) {
  final isArabic = languageCode == 'ar';
  final fontFamily = isArabic ? 'Almarai' : 'Manrope';

  return CupertinoThemeData(
    primaryColor: AppColors.mainBlue,
    primaryContrastingColor: AppColors.white,
    barBackgroundColor: AppColors.mainBlue,
    brightness: Brightness.light,
    textTheme: _buildTextTheme(fontFamily),
  );
}

CupertinoTextThemeData _buildTextTheme(String fontFamily) {
  final base = const CupertinoTextThemeData();
  final manropeOrAlmarai = fontFamily == 'Almarai'
      ? GoogleFonts.almarai().fontFamily
      : GoogleFonts.manrope().fontFamily;

  return CupertinoTextThemeData(
    navTitleTextStyle: base.navTitleTextStyle.copyWith(
      fontFamily: manropeOrAlmarai,
      color: AppColors.mainBlue,
    ),
    navLargeTitleTextStyle: base.navLargeTitleTextStyle.copyWith(
      fontFamily: manropeOrAlmarai,
      color: AppColors.mainBlue,
      fontWeight: FontWeight.w700,
    ),
    textStyle: base.textStyle.copyWith(
      fontFamily: manropeOrAlmarai,
      color: AppColors.mainBlue,
    ),
    actionTextStyle: base.actionTextStyle.copyWith(
      fontFamily: manropeOrAlmarai,
      color: AppColors.mainBlue,
    ),
    tabLabelTextStyle: base.tabLabelTextStyle.copyWith(
      fontFamily: manropeOrAlmarai,
    ),
    navActionTextStyle: base.navActionTextStyle.copyWith(
      fontFamily: manropeOrAlmarai,
      color: AppColors.white,
    ),
    pickerTextStyle: base.pickerTextStyle.copyWith(
      fontFamily: manropeOrAlmarai,
    ),
    dateTimePickerTextStyle: base.dateTimePickerTextStyle.copyWith(
      fontFamily: manropeOrAlmarai,
    ),
  );
}
