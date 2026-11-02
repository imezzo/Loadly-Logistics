import 'package:flutter/cupertino.dart';
import 'package:google_fonts/google_fonts.dart';

import 'app_colors.dart';

/// Loadly design system theme.
/// English: Archivo, Arabic: Almarai.
CupertinoThemeData buildAppTheme([String? languageCode]) {
  final isArabic = languageCode == 'ar';
  final fontFamily = isArabic ? 'Almarai' : 'Archivo';

  return CupertinoThemeData(
    primaryColor: AppColors.mainBlue,
    primaryContrastingColor: AppColors.white,
    barBackgroundColor: AppColors.mainGray,
    brightness: Brightness.light,
    textTheme: _buildTextTheme(fontFamily),
  );
}

CupertinoTextThemeData _buildTextTheme(String fontFamily) {
  final base = const CupertinoTextThemeData();
  final archivoOrAlmarai = fontFamily == 'Almarai'
      ? GoogleFonts.almarai().fontFamily
      : GoogleFonts.archivo().fontFamily;

  return CupertinoTextThemeData(
    navTitleTextStyle: base.navTitleTextStyle.copyWith(
      fontFamily: archivoOrAlmarai,
      color: AppColors.mainBlue,
    ),
    navLargeTitleTextStyle: base.navLargeTitleTextStyle.copyWith(
      fontFamily: archivoOrAlmarai,
      color: AppColors.mainBlue,
      fontWeight: FontWeight.w700,
    ),
    textStyle: base.textStyle.copyWith(
      fontFamily: archivoOrAlmarai,
      color: AppColors.mainBlue,
    ),
    actionTextStyle: base.actionTextStyle.copyWith(
      fontFamily: archivoOrAlmarai,
      color: AppColors.mainBlue,
    ),
    tabLabelTextStyle: base.tabLabelTextStyle.copyWith(
      fontFamily: archivoOrAlmarai,
    ),
    navActionTextStyle: base.navActionTextStyle.copyWith(
      fontFamily: archivoOrAlmarai,
      color: AppColors.white,
    ),
    pickerTextStyle: base.pickerTextStyle.copyWith(
      fontFamily: archivoOrAlmarai,
    ),
    dateTimePickerTextStyle: base.dateTimePickerTextStyle.copyWith(
      fontFamily: archivoOrAlmarai,
    ),
  );
}
