// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Arabic (`ar`).
class AppLocalizationsAr extends AppLocalizations {
  AppLocalizationsAr([String locale = 'ar']) : super(locale);

  @override
  String get appTitle => 'شايل للسائق';

  @override
  String get welcome => 'مرحباً بك';

  @override
  String get verificationInstruction =>
      'أدخل رقم موبيلك و هنبعتلك رمز التحقق في اقل من دقيقة.';

  @override
  String get phone => 'رقم الموبايل';

  @override
  String get enterYourPhone => 'أدخل رقم موبيلك';

  @override
  String get enterValidPhone => 'أدخل رقم موبايل صحيح';

  @override
  String get enterValidPhone11Digits => 'أدخل 11 رقم';

  @override
  String get enterValidPhone01 => 'رقم الموبايل لازم يبدأ بـ 01';

  @override
  String get signInToAccount => 'سجّل الدخول إلى حسابك';

  @override
  String get email => 'البريد الإلكتروني';

  @override
  String get password => 'الرقم السري';

  @override
  String get signIn => 'تسجيل الدخول';

  @override
  String get error => 'خطأ';

  @override
  String get ok => 'موافق';

  @override
  String get enterYourEmail => 'أدخل بريدك الإلكتروني';

  @override
  String get enterValidEmail => 'أدخل بريداً إلكترونياً صحيحاً';

  @override
  String get enterYourPassword => 'أدخل كلمة المرور';

  @override
  String get home => 'الرئيسية';

  @override
  String get forgetPassword => 'نسيت كلمة السر؟';

  @override
  String get verificationTitle => 'رمز التحقق';

  @override
  String get verificationSubtitle =>
      'هنبعتلك رمز التحقق, ممكن تختار نبعته على الايميل و لا برسالة نصية على موبيلك؟';

  @override
  String get verificationSubtitlePhone =>
      'هنبعتلك رمز التحقق برسالة نصية على موبيلك.';

  @override
  String get sendViaSms => 'أرسل رمزا برسالة نصية';

  @override
  String get sendViaEmail => 'أرسل رمزًا عبر البريد الإلكتروني';

  @override
  String get continueButton => 'تسجيل الدخول';

  @override
  String otpInstructionSms(String phone) {
    return 'ارسلنا لك رمز التحقيق برسالة نصية على رقمك ($phone)، اكتبه هنا علشان تقدر تستكمل فتح حسابك.';
  }

  @override
  String otpInstructionEmail(String email) {
    return 'ارسلنا لك رمز التحقيق على بريدك الالكتروني $email، اكتبه هنا علشان تقدر تستكمل فتح حسابك.';
  }

  @override
  String get resendPrompt => 'لسه مجاش رسالة ?';

  @override
  String get resendLink => 'إبعته تاني';

  @override
  String get verifyAccount => 'تحقق من حسابك';

  @override
  String get verifyEmail => 'تحقق من البريد الإلكتروني';

  @override
  String get verifyEmailInstruction =>
      'أدخل الرمز المكون من 6 أرقام الذي أرسلناه إلى بريدك الإلكتروني للتحقق من حسابك.';

  @override
  String get enterVerificationCode => 'أدخل رمز التحقق';

  @override
  String get verificationCodeLength => 'يجب أن يتكون الرمز من 6 أرقام';

  @override
  String get verify => 'تحقق';

  @override
  String get resendCode => 'إعادة إرسال الرمز';

  @override
  String get codeSentToEmail => 'تم إرسال رمز التحقق إلى بريدك الإلكتروني.';

  @override
  String get newPassword => 'كلمة المرور الجديدة';

  @override
  String get confirmPassword => 'تأكيد كلمة المرور';

  @override
  String get resetPassword => 'إعادة تعيين كلمة المرور';

  @override
  String get passwordMinLength => 'كلمة المرور يجب أن تكون 6 أحرف على الأقل';

  @override
  String get personalAccount => 'حسابك الشخصي';

  @override
  String get employeeDriverName => 'اسم الموظف/السائق';

  @override
  String get emailLabel => 'البريد الإلكتروني';

  @override
  String get rating => 'معدل';

  @override
  String get trip => 'رحلة';

  @override
  String get trips => 'الرحلات';

  @override
  String get files => 'الملفات';

  @override
  String get myPersonalData => 'بياناتي الشخصية';

  @override
  String get fileName => 'اسم الملف';

  @override
  String get chooseFromGallery => 'أختر من المعرض';

  @override
  String get profilePage => 'صفحة الشخصية';

  @override
  String get notifications => 'إشعارات';

  @override
  String get tripsNav => 'الرحلات';

  @override
  String get homeNav => 'الرئيسية';

  @override
  String secondsRemaining(int seconds) {
    return 'باقي $seconds ثانية';
  }

  @override
  String get loginWithPassword => 'تسجيل الدخول بكلمة المرور';

  @override
  String get loginWithBiometric => 'تسجيل الدخول بالبصمة';

  @override
  String get or => 'أو';

  @override
  String get backToLogin => 'العودة لتسجيل الدخول';

  @override
  String get rememberMe => 'تذكرني';

  @override
  String get online => 'متصل';

  @override
  String get offline => 'غير متصل';

  @override
  String get active => 'نشط';

  @override
  String get inactive => 'غير نشط';

  @override
  String get logout => 'تسجيل الخروج';

  @override
  String get changePassword => 'تغيير كلمة المرور';

  @override
  String get enableFingerprint => 'تفعيل البصمة';

  @override
  String get biometricErrorFailedOrCancelled =>
      'فشل التحقق بالبصمة أو تم الإلغاء.';

  @override
  String get biometricErrorEnableFailed => 'فشل تفعيل تسجيل الدخول بالبصمة.';

  @override
  String get biometricErrorDisableFailed =>
      'فشل إلغاء تفعيل تسجيل الدخول بالبصمة.';

  @override
  String get biometricLocalizedReasonSignIn => 'تأكيد الهوية لتسجيل الدخول';

  @override
  String get biometricLocalizedReasonEnable =>
      'تأكيد الهوية لتفعيل تسجيل الدخول بالبصمة';

  @override
  String get biometricEnablePasswordDialogMessage =>
      'أدخل كلمة المرور لتفعيل تسجيل الدخول بالبصمة.';

  @override
  String get biometricOfferAfterLoginMessage =>
      'هل تريد تفعيل تسجيل الدخول بالبصمة للوصول بشكل أسرع؟';

  @override
  String get biometricAccountInfoUnavailable =>
      'تعذر الحصول على بيانات الحساب. يُرجى المحاولة مرة أخرى.';

  @override
  String get enable2FA => 'تفعيل المصادقة الثنائية';

  @override
  String get support => 'الدعم';

  @override
  String get settings => 'الإعدادات';

  @override
  String get cancel => 'إلغاء';

  @override
  String get passwordsDoNotMatch => 'كلمات المرور غير متطابقة';

  @override
  String get fullNameEnglish => 'الاسم الكامل (إنجليزي)';

  @override
  String get fullNameArabic => 'الاسم الكامل (عربي)';

  @override
  String get userName => 'اسم المستخدم';

  @override
  String get save => 'حفظ';

  @override
  String get editProfile => 'تعديل الملف الشخصي';

  @override
  String get profileUpdatedSuccessfully => 'تم تحديث الملف الشخصي بنجاح';

  @override
  String get imagePickerNotImplemented => 'محدد الصور غير متاح بعد';

  @override
  String get retry => 'إعادة المحاولة';

  @override
  String get user => 'مستخدم';

  @override
  String get cameraPermissionRequired =>
      'إذن الكاميرا مطلوب لالتقاط الصور. يرجى تفعيله من الإعدادات.';

  @override
  String get photoLibraryPermissionRequired =>
      'إذن معرض الصور مطلوب لاختيار الصور. يرجى تفعيله من الإعدادات.';

  @override
  String get permissionDenied => 'تم رفض الإذن';

  @override
  String get openSettings => 'فتح الإعدادات';

  @override
  String get takePhoto => 'التقاط صورة';

  @override
  String get todayNotifications => 'إشعارات اليوم';

  @override
  String get company => 'الشركة';

  @override
  String get companyNotification => 'إشعار الشركة';

  @override
  String get newTripSent => 'تم إرسال رحلة جديدة أضغط للإطلاع على التفاصيل';

  @override
  String get retrievalNotificationSent =>
      'تم إرسال اشعار الاسترجاع للشركة في انتظار الرد عليك';

  @override
  String get replacementCarOnWay => 'سيارة بديلة في الطريق إليك';

  @override
  String get vehicleDetails => 'تفاصيل المركبة';

  @override
  String minsAgo(int mins) {
    return '$mins دقيقة';
  }

  @override
  String hoursAgo(int hours) {
    return '$hours ساعة';
  }

  @override
  String get tripDetails => 'تفاصيل رحلة';

  @override
  String get tripId => 'رقم الرحلة';

  @override
  String get status => 'الحالة';

  @override
  String get statusRequested => 'مطلوبة';

  @override
  String get statusPending => 'قيد الانتظار';

  @override
  String get statusScheduled => 'مجدولة';

  @override
  String get statusDriverAccepted => 'تم قبول السائق';

  @override
  String get statusFirstPickupLoading => 'جاري التحميل';

  @override
  String get statusInProgress => 'قيد التنفيذ';

  @override
  String get statusCompleted => 'مكتملة';

  @override
  String get statusReviewed => 'تمت مراجعتها';

  @override
  String get statusCancelled => 'ملغاة';

  @override
  String get statusUnknown => '—';

  @override
  String get startTrip => 'بداية الرحلة';

  @override
  String get startShippingAndLoading => 'بداية تحميل الشحنة';

  @override
  String tripStartsOn(String date, String time) {
    return 'الرحلة تبدا يوم : $date الساعة $time';
  }

  @override
  String get startLoadingShipment => 'بدء تحميل الشحنة';

  @override
  String get pickupLocation => 'الاستلام';

  @override
  String get startTripLocation => 'بداية الرحلة';

  @override
  String get deliveryLocation => 'التوصيل';

  @override
  String get depotLocation => 'العودة';

  @override
  String get weight => 'الوزن';

  @override
  String get shipmentType => 'نوع الشحنة';

  @override
  String get tripDuration => 'مدة الرحلة';

  @override
  String get shipmentDimensions => 'إبعاد الشحنة';

  @override
  String get approximately => 'تقربياً';

  @override
  String get hours => 'ساعة';

  @override
  String get stopPoints => 'نقاط التوقف';

  @override
  String get tripPoints => 'نقاط الرحلة';

  @override
  String get stop => 'توقف';

  @override
  String get clientDetails => 'تفاصيل العميل';

  @override
  String get phoneNumber => 'رقم التلفون';

  @override
  String get responsiblePersonName => 'اسم المسئول';

  @override
  String get isThereAProblemCallSupport => 'فيه مشكلة؟ كلم الدعم';

  @override
  String get clickToConfirm => 'أضغط للتأكيد';

  @override
  String get tripDetailsPlaceholder => 'هنا التفاصيل تتكتب';

  @override
  String get noTripsAvailable => 'لا توجد رحلات متاحة';

  @override
  String get noNotifications => 'لا توجد إشعارات';

  @override
  String get noFilesAvailable => 'لا توجد ملفات';

  @override
  String get loadingTrips => 'جاري تحميل الرحلات...';

  @override
  String get errorLoadingTrips => 'خطأ في تحميل الرحلات';

  @override
  String get currentTripTab => 'الرحلة الحالية';

  @override
  String get previousTripsTab => 'الرحلات السابقة';

  @override
  String get yourCurrentTrip => 'رحلتك الحالية';

  @override
  String get yourPreviousTrips => 'رحلاتك السابقة';

  @override
  String get shipmentDetails => 'ما هي تفاصيل الشحنة';

  @override
  String tripOnDayAtTime(String date, String time) {
    return 'الرحلة في يوم : $date الساعة $time';
  }

  @override
  String get onTheWayToLoading => 'في الطريق للتحميل';

  @override
  String get completed => 'مكتملة';

  @override
  String get tripScheduleUpdated => 'تحديث موعد الرحلة';

  @override
  String tripScheduleUpdatedMessage(String oldSchedule, String newSchedule) {
    return 'تم تحديث موعد رحلتك: $oldSchedule إلى $newSchedule';
  }

  @override
  String get newSchedule => 'الموعد الجديد';

  @override
  String get acceptTrip => 'قبول الرحلة';

  @override
  String get arriveAtWaypoint => 'وصلت';

  @override
  String get departFromWaypoint => 'اتحرك دلوقتى';

  @override
  String get completeLoading => 'تم التحميل';

  @override
  String get startUnloading => 'بداية تعتيق الشحنة';

  @override
  String get completeUnloading => 'تم التعتيق';

  @override
  String get optionalNotePlaceholder => 'ملاحظة اختيارية';

  @override
  String get completeTrip => 'نهاية الرحلة';

  @override
  String get openCameraForPhotos => 'أفتح الكاميرا للتصوير';

  @override
  String get openCamera => 'افتح الكاميرا';

  @override
  String get processing => 'جاري المعالجة...';

  @override
  String get shipmentPhotos => 'صور الشحنة';

  @override
  String get receiptNote => 'إيصال الاستلام';

  @override
  String get deliveryNote => 'إيصال التسليم';

  @override
  String get unloadingPhotos => 'صور التفريغ';

  @override
  String get tollgatePhotos => 'صورة ايصال الكارتات (مرور)';

  @override
  String get tollgateWeightPhotos => 'صورة ايصال الكارتات (موازيين)';

  @override
  String get tollgatesTotalAmount => 'إجمالي مبلغ الكارتات';

  @override
  String get notePlaceholder => 'ملاحظة';

  @override
  String get noteRequired => 'الملاحظة مطلوبة';

  @override
  String get tripPointNoNote => 'لا توجد ملاحظة';

  @override
  String get currentLocationLoading => 'جاري جلب الموقع...';

  @override
  String get viewDocument => 'عرض المستند';

  @override
  String get odometer => 'عداد الكيلومترات';

  @override
  String get startOdometer => 'عداد كيلومترات البداية';

  @override
  String get endOdometer => 'عداد كيلومترات النهاية';

  @override
  String get unloadingOdometer => 'عداد كيلومترات التعتيق';

  @override
  String get odometerImage => 'صورة عداد الكيلومترات';

  @override
  String get receiptImage => 'صورة الإيصال';

  @override
  String get shipmentImages => 'صور الشحنة';

  @override
  String get deliveryNoteImage => 'صورة إذن التسليم';

  @override
  String get tollgateImages => 'صور بوابة الرسوم';

  @override
  String get tripErrorGeneric => 'حدث خطأ. يرجى المحاولة مرة أخرى.';

  @override
  String get tripErrorMustBeScheduledToAccept =>
      'يجب أن تكون الرحلة مجدولة للقبول.';

  @override
  String get tripErrorDeliveryNoteRequired =>
      'صورة إذن التسليم مطلوبة عند التوصيل.';

  @override
  String get tripErrorWaypointMustBePending => 'يجب أن تكون نقطة التوقف معلقة.';

  @override
  String get tripErrorWaypointMustBeArrived =>
      'يجب الوصول إلى نقطة التوقف أولاً.';

  @override
  String get tripErrorWaypointMustBeLoadingStarted => 'يجب بدء التحميل أولاً.';

  @override
  String get tripErrorWaypointMustBeLoadingCompleted =>
      'يجب إكمال التحميل أولاً.';

  @override
  String get tripErrorAllWaypointsMustBeCompleted =>
      'يجب إكمال جميع نقاط التوقف أولاً.';

  @override
  String get tripErrorFirstWaypointMustBeLoadingCompleted =>
      'يجب إكمال تحميل أول استلام أولاً.';

  @override
  String get tripErrorTripMustBeInProgress => 'يجب أن تكون الرحلة قيد التنفيذ.';

  @override
  String get tripErrorTripMustBeDriverAccepted => 'يجب قبول الرحلة أولاً.';

  @override
  String get tripErrorTripMustBeFirstPickupLoading =>
      'يجب إكمال تحميل أول استلام أولاً.';

  @override
  String get tripErrorInvalidImageFormat =>
      'يجب أن تكون الصورة JPEG أو PNG أو WebP.';

  @override
  String get tripErrorImageTooLarge => 'يجب أن تكون الصورة 10 ميجابايت أو أقل.';

  @override
  String get tripErrorInvalidOdometerImage =>
      'يرجى التقاط صورة واضحة لعداد الكيلومترات في السيارة.';

  @override
  String get tripErrorUnreadableOdometerImage =>
      'تعذر قراءة عداد الكيلومترات. يرجى إعادة التصوير بوضوح أفضل.';

  @override
  String get loadMore => 'تحميل المزيد';

  @override
  String get filterByDate => 'تصفية حسب التاريخ';

  @override
  String get fromDate => 'من';

  @override
  String get toDate => 'إلى';

  @override
  String get applyFilter => 'تطبيق';

  @override
  String get clearFilter => 'مسح';

  @override
  String get documents => 'المستندات';

  @override
  String get documentOpenFailed => 'تعذر فتح المستند. نسخ الرابط؟';

  @override
  String get copyUrl => 'نسخ الرابط';

  @override
  String get urlCopied => 'تم نسخ الرابط';

  @override
  String get documentTypeMissing => 'تعذر تحديد نوع المستند.';

  @override
  String get language => 'اللغة';

  @override
  String get languageArabic => 'العربية';

  @override
  String get languageEnglish => 'الإنجليزية';

  @override
  String get navigateToWaypoint => 'التنقل إلى نقطة التوقف';

  @override
  String get navigateToPickup => 'التنقل إلى الاستلام';

  @override
  String get navigateToDropoff => 'التنقل إلى التسليم';

  @override
  String get navigateToDepot => 'التنقل إلى العودة';

  @override
  String get showRoadRoute => 'عرض المسار على الطرق';

  @override
  String get loadingRoadRoute => 'جاري تحميل المسار…';

  @override
  String get tripCompletedSuccessfully => 'تم إكمال الرحلة بنجاح';

  @override
  String get tripCompletedSuccessMessage => 'شكراً لإكمال الرحلة. أحسنت!';

  @override
  String get tripDrivenDistanceLive => 'المسافة المقطوعة (مباشر)';

  @override
  String get tripDrivenDistanceFinal => 'المسافة المقطوعة';

  @override
  String get tripDrivenDistancePartial => 'المسافة المقطوعة (قبل الإلغاء)';

  @override
  String tripDrivenDistanceKm(String km) {
    return '$km كم';
  }

  @override
  String get tripDrivenDistanceGpsHint =>
      'من GPS على هذا الجهاز (وليس عداد السيارة).';

  @override
  String get tripOdometerNotificationTitle => 'استخدام موقعك';

  @override
  String get tripOdometerNotificationText => 'شايل للسائق يصل إلى موقعك.';

  @override
  String get backToTrips => 'العودة إلى الرحلات';

  @override
  String get homeLocationServicesDisabled =>
      'خدمات الموقع متوقفة. شغّلها عشان تقدر تستخدم الخريطة وتبقى متصل.';

  @override
  String get homeLocationPermissionDenied =>
      'تم رفض إذن الموقع. اسمح بالوصول للموقع عشان تستخدم الخريطة وتبقى متصل.';

  @override
  String get homeLocationPermissionDeniedForever =>
      'إذن الموقع مرفوض بشكل دائم. فعّله من إعدادات التطبيق.';

  @override
  String get homeLocationUnavailable =>
      'الموقع مش متاح حالياً. جرّب تعيد تشغيل التطبيق.';

  @override
  String get homeLocationGetPositionFailed => 'ماقدرناش نحدد موقعك. حاول تاني.';

  @override
  String get homeLocationGetInitialFailed =>
      'ماقدرناش نبدأ تتبع الموقع. حاول تاني.';

  @override
  String get homeLocationStreamError => 'تتبع الموقع توقف. حاول تاني.';

  @override
  String appVersion(String version) {
    return 'الإصدار $version';
  }

  @override
  String get prominentDisclosureContinue => 'متابعة';

  @override
  String get prominentDisclosureNotNow => 'ليس الآن';

  @override
  String get locationPermissionDisclosureTitle =>
      'كيف يستخدم شايل للسائق موقعك';

  @override
  String get locationPermissionDisclosureBody =>
      'يجمع تطبيق شايل للسائق موقع جهازك—بما في ذلك في الخلفية عندما لا يكون التطبيق ظاهراً على الشاشة—أثناء كونك متصلاً أو أثناء تنفيذ الرحلات. نستخدمه لعرض موضعك على الخريطة، والتبديل بدقة بين وضعي الاتصال، والتخطيط لمسارات الاستلام والتسليم، ومشاركة الموقع المباشر مع شركتك. يُرسل الموقع فقط إلى خوادم شايل عند الحاجة لهذه ميزات السائق.\n\nسيُطلب منك أولاً السماح بالموقع أثناء استخدام التطبيق. إذا لزم التتبع المباشر الكامل، ستظهر شاشة ثانية تشرح الوصول في الخلفية، ثم خطوة إذن أخرى من النظام (مثل «السماح طوال الوقت»). اضغط «متابعة» للبدء.';

  @override
  String get locationBackgroundPermissionDisclosureTitle => 'الموقع في الخلفية';

  @override
  String get locationBackgroundPermissionDisclosureBody =>
      'لكي تصل شركتك بتحديثات مباشرة موثوقة أثناء القيادة، يحتاج شايل للسائق إلى إذن للوصول إلى الموقع في الخلفية (عندما يكون التطبيق خلف تطبيقات أخرى أو الشاشة مطفأة). يُستخدم ذلك فقط لميزات السائق الموضحة في الشاشة السابقة—وليس للإعلانات.\n\nاضغط «متابعة» لفتح طلب الإذن التالي من النظام. على بعض الأجهزة يجب اختيار «السماح طوال الوقت» أو تفعيل الموقع في الخلفية من الإعدادات.';

  @override
  String get locationTrackingNotificationTitle => 'شايل للسائق متصل';

  @override
  String get locationTrackingNotificationText =>
      'مشاركة موقعك مع شايل أثناء كونك متصلاً.';

  @override
  String get notificationPermissionDisclosureTitle => 'الإشعارات';

  @override
  String get notificationPermissionDisclosureBody =>
      'يرسل تطبيق شايل للسائق إشعارات عن الرحلات والجداول والتحديثات المهمة. لإيصالها يستخدم التطبيق الإشعارات الفورية ورمز جهاز يُخزَّن على خوادمنا. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get cameraPermissionDisclosureTitle => 'الوصول إلى الكاميرا';

  @override
  String get cameraPermissionDisclosureBody =>
      'يستخدم تطبيق شايل للسائق الكاميرا فقط عندما تختار التقاط صورة—مثلاً لصورة الملف الشخصي أو المستندات المطلوبة. تُرفع الصور التي تلتقطها إلى خوادمنا لحساب السائق. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get photosPermissionDisclosureTitle => 'الصور والملفات';

  @override
  String get photosPermissionDisclosureBody =>
      'يصل تطبيق شايل للسائق إلى الصور التي تختارها من معرض الصور أو تخزين الجهاز لرفع صور الملف الشخصي أو المستندات. لا نصل إلا للملفات التي تختارها أنت. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get appUpdateRequiredTitle => 'التحديث مطلوب';

  @override
  String get appUpdateAvailableTitle => 'تحديث متاح';

  @override
  String get appUpdateRequiredMessage =>
      'يلزم إصدار جديد من تطبيق شايل للسائق للمتابعة. يرجى التحديث إلى أحدث إصدار.';

  @override
  String get appUpdateAvailableMessage =>
      'يتوفر إصدار جديد من تطبيق شايل للسائق يتضمن تحسينات وإصلاحات.';

  @override
  String appUpdateLatestVersion(String version) {
    return 'أحدث إصدار: $version';
  }

  @override
  String get appUpdateNow => 'حدّث الآن';

  @override
  String get appUpdateLater => 'لاحقاً';

  @override
  String get enterCustoms => 'وصلت الجمرك';

  @override
  String get exitCustoms => 'خروج من الجمرك';

  @override
  String get customsTotalAmount => 'مبلغ الجمارك';

  @override
  String get documentImage => 'تصوير المستندات';

  @override
  String get tripErrorImageMissingRetake =>
      'لم يتم العثور على واحدة أو أكثر من الصور. يرجى إعادة التقاطها.';

  @override
  String get questionMark => '؟';

  @override
  String get invalidCredentials => 'بيانات الدخول غير صحيحة.';

  @override
  String get invalidPhoneNumber => 'رقم الهاتف غير صالح.';

  @override
  String get networkError => 'لا يوجد اتصال بالإنترنت.';

  @override
  String get unknownError => 'حدث خطأ ما.';

  @override
  String get requestFailed => 'فشل الطلب.';

  @override
  String get error400 => 'طلب غير صحيح. يرجى التحقق من المدخلات.';

  @override
  String get error401 =>
      'جلسة غير صالحة أو منتهية. يرجى تسجيل الدخول مرة أخرى.';

  @override
  String get error404 => 'المورد غير موجود.';

  @override
  String get error405 => 'هذا الإجراء غير مسموح به.';

  @override
  String get error422 => 'البيانات المدخلة غير صالحة.';

  @override
  String get error500 => 'خطأ في الخادم. يرجى المحاولة لاحقًا.';

  @override
  String get errorDefault => 'حدث خطأ ما. يرجى المحاولة مرة أخرى.';

  @override
  String get cameraCancelled => 'تم إلغاء الكاميرا';

  @override
  String get passwordChangedSuccessfully => 'تم تغيير كلمة المرور بنجاح';

  @override
  String get pleaseConfirmYourPassword => 'يُرجى تأكيد كلمة المرور';

  @override
  String get successfullyWentOnline => 'تم الاتصال بنجاح';

  @override
  String get successfullyWentOffline => 'تم قطع الاتصال بنجاح';

  @override
  String get callSupport => 'كلم الدعم';
}
