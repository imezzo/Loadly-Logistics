import 'package:flutter/cupertino.dart';

import 'app_colors.dart';

/// Bundled font family from [pubspec.yaml].
const appFontFamily = 'Zain';

/// Loadly design system theme.
/// English and Arabic now use the Zain font family globally.
CupertinoThemeData buildAppTheme([String? languageCode]) {
  return CupertinoThemeData(
    primaryColor: AppColors.mainBlue,
    primaryContrastingColor: AppColors.white,
    barBackgroundColor: AppColors.mainBlue,
    brightness: Brightness.light,
    textTheme: _buildTextTheme(appFontFamily),
  );
}

CupertinoTextThemeData _buildTextTheme(String fontFamily) {
  final base = const CupertinoTextThemeData();

  return CupertinoTextThemeData(
    navTitleTextStyle: base.navTitleTextStyle.copyWith(
      fontFamily: fontFamily,
      color: AppColors.mainBlue,
    ),
    navLargeTitleTextStyle: base.navLargeTitleTextStyle.copyWith(
      fontFamily: fontFamily,
      color: AppColors.mainBlue,
      fontWeight: FontWeight.w700,
    ),
    textStyle: base.textStyle.copyWith(
      fontFamily: fontFamily,
      color: AppColors.mainBlue,
    ),
    actionTextStyle: base.actionTextStyle.copyWith(
      fontFamily: fontFamily,
      color: AppColors.mainBlue,
    ),
    tabLabelTextStyle: base.tabLabelTextStyle.copyWith(fontFamily: fontFamily),
    navActionTextStyle: base.navActionTextStyle.copyWith(
      fontFamily: fontFamily,
      color: AppColors.white,
    ),
    pickerTextStyle: base.pickerTextStyle.copyWith(fontFamily: fontFamily),
    dateTimePickerTextStyle: base.dateTimePickerTextStyle.copyWith(
      fontFamily: fontFamily,
    ),
  );
}
