// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Arabic (`ar`).
class AppLocalizationsAr extends AppLocalizations {
  AppLocalizationsAr([String locale = 'ar']) : super(locale);

  @override
  String get appTitle => 'شايل للمقاول';

  @override
  String get welcome => 'مرحباً بك';

  @override
  String get welcomeToLoadly => 'مرحباً في شايل';

  @override
  String get welcomeDesc => 'سجّل الدخول إلى حسابك أو أنشئ حسابًا جديدًا';

  @override
  String get switchLanguage => 'تغيير اللغة';

  @override
  String get createAccount => 'إنشاء حساب';

  @override
  String get createNewAccount => 'إنشاء حساب جديد';

  @override
  String get verificationInstruction => 'أدخل رقم موبيلك و هنبعتلك رمز التحقق';

  @override
  String get phone => 'رقم الهاتف';

  @override
  String get enterYourPhone => 'أدخل رقم موبيلك';

  @override
  String get enterYourFullName => 'أدخل اسمك الكامل';

  @override
  String get enterYourAddress => 'أدخل تفاصيل العنوان';

  @override
  String get enterValidPhone => 'أدخل رقم موبايل صحيح';

  @override
  String get enterValidPhone11Digits => 'أدخل 11 رقم';

  @override
  String get enterValidPhone01 => 'رقم الموبايل لازم يبدأ بـ 01';

  @override
  String get enterYourId => 'الرقم القومي';

  @override
  String get signInToAccount => 'سجّل الدخول إلى حسابك';

  @override
  String get email => 'البريد الإلكتروني';

  @override
  String get password => 'كلمة المرور';

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
  String get forgetPassword => 'نسيت كلمة المرور؟';

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
    return 'ارسلنا لك رمز التحقيق برسالة نصية على رقمك ($phone)، يرجى إدخال الرمز أدناه للمتابعة.';
  }

  @override
  String otpInstructionEmail(String email) {
    return 'ارسلنا لك رمز التحقيق على بريدك الالكتروني $email، يرجى إدخال الرمز أدناه للمتابعة.';
  }

  @override
  String get resendPrompt => 'لم تحصل على الرمز؟';

  @override
  String get resendLink => 'إعادة إرسال';

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
  String get trips => 'نقلاتي';

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
  String get tollgatePhotos => 'صورة ايصال الكارتات';

  @override
  String get tollgatesTotalAmount => 'إجمالي مبلغ الكارتات';

  @override
  String get tollgatesWeightTotalAmount => 'إجمالي وزن الكارتات';

  @override
  String get tollgateWeightImages => 'صور وزن الكارتات';

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
  String get apply => 'تطبيق';

  @override
  String get clear => 'مسح';

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
  String get showRoadRoute => 'عرض المسار على الطرق';

  @override
  String get loadingRoadRoute => 'جاري تحميل المسار…';

  @override
  String get tripCompletedSuccessfully => 'تم إكمال الرحلة بنجاح';

  @override
  String get tripCompletedSuccessMessage => 'شكراً لإكمال الرحلة. أحسنت!';

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
      'كيف يستخدم شايل للمقاول موقعك';

  @override
  String get locationPermissionDisclosureBody =>
      'يجمع تطبيق شايل للمقاول موقع جهازك—بما في ذلك في الخلفية عندما لا يكون التطبيق ظاهراً على الشاشة—أثناء كونك متصلاً أو أثناء تنفيذ الرحلات. نستخدمه لعرض موضعك على الخريطة، والتبديل بدقة بين وضعي الاتصال، والتخطيط لمسارات الاستلام والتسليم، ومشاركة الموقع المباشر مع شركتك. يُرسل الموقع فقط إلى خوادم شايل عند الحاجة لهذه ميزات مقاول.\n\nسيُطلب منك أولاً السماح بالموقع أثناء استخدام التطبيق. إذا لزم التتبع المباشر الكامل، ستظهر شاشة ثانية تشرح الوصول في الخلفية، ثم خطوة إذن أخرى من النظام (مثل «السماح طوال الوقت»). اضغط «متابعة» للبدء.';

  @override
  String get locationBackgroundPermissionDisclosureTitle => 'الموقع في الخلفية';

  @override
  String get locationBackgroundPermissionDisclosureBody =>
      'لكي تصل شركتك بتحديثات مباشرة موثوقة أثناء القيادة، يحتاج شايل للمقاول إلى إذن للوصول إلى الموقع في الخلفية (عندما يكون التطبيق خلف تطبيقات أخرى أو الشاشة مطفأة). يُستخدم ذلك فقط لميزات مقاول الموضحة في الشاشة السابقة—وليس للإعلانات.\n\nاضغط «متابعة» لفتح طلب الإذن التالي من النظام. على بعض الأجهزة يجب اختيار «السماح طوال الوقت» أو تفعيل الموقع في الخلفية من الإعدادات.';

  @override
  String get locationTrackingNotificationTitle => 'شايل للمقاول متصل';

  @override
  String get locationTrackingNotificationText =>
      'مشاركة موقعك مع شايل أثناء كونك متصلاً.';

  @override
  String get notificationPermissionDisclosureTitle => 'الإشعارات';

  @override
  String get notificationPermissionDisclosureBody =>
      'يرسل تطبيق شايل للمقاول إشعارات عن الرحلات والجداول والتحديثات المهمة. لإيصالها يستخدم التطبيق الإشعارات الفورية ورمز جهاز يُخزَّن على خوادمنا. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get cameraPermissionDisclosureTitle => 'الوصول إلى الكاميرا';

  @override
  String get cameraPermissionDisclosureBody =>
      'يستخدم تطبيق شايل للمقاول الكاميرا فقط عندما تختار التقاط صورة—مثلاً لصورة الملف الشخصي أو المستندات المطلوبة. تُرفع الصور التي تلتقطها إلى خوادمنا لحساب مقاول. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get photosPermissionDisclosureTitle => 'الصور والملفات';

  @override
  String get photosPermissionDisclosureBody =>
      'يصل تطبيق شايل للمقاول إلى الصور التي تختارها من معرض الصور أو تخزين الجهاز لرفع صور الملف الشخصي أو المستندات. لا نصل إلا للملفات التي تختارها أنت. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get appUpdateRequiredTitle => 'التحديث مطلوب';

  @override
  String get appUpdateAvailableTitle => 'تحديث متاح';

  @override
  String get appUpdateRequiredMessage =>
      'يلزم إصدار جديد من تطبيق شايل للمقاول للمتابعة. يرجى التحديث إلى أحدث إصدار.';

  @override
  String get appUpdateAvailableMessage =>
      'يتوفر إصدار جديد من تطبيق شايل للمقاول يتضمن تحسينات وإصلاحات.';

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
  String get failedToLoadDrivers => 'تعذر تحميل السائقين.';

  @override
  String get failedToLoadDocumentTypes => 'تعذر تحميل أنواع المستندات.';

  @override
  String get failedToCreateDriver => 'تعذر إنشاء السائق.';

  @override
  String get failedToUpdateTripDriver => 'تعذر تحديث سائق الرحلة.';

  @override
  String get failedToLoadTrip => 'تعذر تحميل الرحلة.';

  @override
  String get failedToLoadTrips => 'تعذر تحميل الرحلات.';

  @override
  String get failedToLoadBookingRequests => 'تعذر تحميل طلبات الحجز.';

  @override
  String get failedToBookTrip => 'تعذر حجز الرحلة.';

  @override
  String get failedToWithdrawBooking => 'تعذر سحب الحجز.';

  @override
  String get failedToLoadVehicleTypes => 'تعذر تحميل أنواع المركبات.';

  @override
  String get failedToLoadNotifications => 'تعذر تحميل الإشعارات.';

  @override
  String get failedToLoadMore => 'تعذر تحميل المزيد.';

  @override
  String get failedToMarkAsRead => 'تعذر التعليم كمقروء.';

  @override
  String get registrationFailed => 'تعذر إكمال التسجيل.';

  @override
  String get invalidResponse => 'استجابة غير صالحة. يرجى المحاولة مرة أخرى.';

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
  String get continuee => 'متابعة';

  @override
  String get trackYourPackageEasily => 'تتبع شحنتك بسهولة';

  @override
  String get trackYourPackageEasilyDesc =>
      'أدخل رقم الشحنة واحصل على تحديثات فورية عن رحلة شحنتك.';

  @override
  String get realtimeNotifications => 'إشعارات فورية';

  @override
  String get realtimeNotificationsDesc =>
      'احصل على إشعارات فورية حول حالة شحنتك. يمكنك تخصيص الإشعارات بسهولة.';

  @override
  String get uploadYourDocuments => 'ارفع مستنداتك';

  @override
  String get contractorInformation => 'معلومات المقاول';

  @override
  String get fullName => 'الاسم الكامل';

  @override
  String get addressDetails => 'تفاصيل العنوان';

  @override
  String get uploadImageOrBrowse => 'رفع صورة أو تصفح';

  @override
  String get pngJpgUpTo => 'PNG, JPG حتى 10 ميجابايت';

  @override
  String get registerConfirm =>
      'آوكد ان البيانات الشخصية  المقدمة كلها صحيحة و آوافق علي الشروط و الاحكام.';

  @override
  String get yourRequestSuccessfullyRegistered => 'تم تسجيل طلبك بنجاح';

  @override
  String get yourRequestSuccessfullyRegisteredDesc =>
      'سنراجع مستنداتك، وسنفتح حسابك لتلقي الرحلات من شايل قريبًا جدًا، ربما تستغرق مراجعة أوراقك يومين.';

  @override
  String get goToHomepage => 'الذهاب إلى الصفحة الرئيسية';

  @override
  String get market => 'السوق';

  @override
  String get marketScreenTitle => 'السوق';

  @override
  String get marketScreenHeaderDesc =>
      'تسوق قطع الغيار والإطارات والمعدات لأسطولك.';

  @override
  String get marketComingSoonTitle => 'إنتظرونا ...!';

  @override
  String get marketComingSoonBody =>
      'قريبا سيتم إصدار السوق لشراء كل مستلزمات سيارتك بإسعار جيدة .';

  @override
  String get marketHomePageButton => 'الصفحة الرئيسية';

  @override
  String get account => 'الحساب';

  @override
  String get enterPasswordEnableBiometric =>
      'أدخل كلمة المرور لتفعيل تسجيل الدخول باستخدام البصمة.';

  @override
  String get biometric => 'التحقق بالبصمة';

  @override
  String get enableBiometric =>
      'هل ترغب في تفعيل تسجيل الدخول باستخدام البصمة للدخول بشكل أسرع؟';

  @override
  String get biometricDesc => 'قم بتفعيل بصمة الإصبع ورمز الأمان لفتح التطبيق.';

  @override
  String get languages => 'اللغات';

  @override
  String get helpAndCenter => 'الدعم الفنى';

  @override
  String get helpAndCenterDesc =>
      'تواصل مع فريق الدعم لدينا عبر الدردشة أو الاتصال للحصول على المساعدة.';

  @override
  String get logoutDesc =>
      'يرجى تفعيل خاصية التحقق بالبصمة أو كلمة المرور لتسهيل عملية تسجيل الدخول مرة أخرى.';

  @override
  String get edit => 'تعديل';

  @override
  String get saveChanges => 'حفظ التعديلات';

  @override
  String get back => 'رجوع';

  @override
  String get totalTripsCount => 'عدد النقلات كلها';

  @override
  String get unpaidAmount => 'المبلغ المستحق';

  @override
  String get personalInformation => 'المعلومات الشخصية';

  @override
  String get shipping => 'الشحن';

  @override
  String get completedTripsDue => 'النقلات المنتهية المستحقة';

  @override
  String get completedTripsDueUntilNow => 'الرحلات المنتهية المستحقة حتى الان';

  @override
  String get tripsAndFinancialDue => 'النقلات و استحقاق المالية';

  @override
  String get contactSupport => 'تواصل مع الدعم';

  @override
  String get trackTheTrip => 'تتبع الرحلة';

  @override
  String get trackTheTripDesc =>
      'أدخل رقم الرحلة واحصل على التتبع في الوقت الفعلي.';

  @override
  String get enterTripID => 'أدخل رقم الرحلة';

  @override
  String get tripRequest => 'طلب رحلة';

  @override
  String get tripsRequest => 'طلبات الرحلات';

  @override
  String get tripsRequestDesc => 'جميع طلبات الرحلات لديك.';

  @override
  String get tripsReview => 'مراجعة الرحلات';

  @override
  String get myTripsReview => 'مراجعة النقلات';

  @override
  String get completedTrips => 'النقلات المنتهية';

  @override
  String get showAll => 'أظهر الكل';

  @override
  String loadlyAdminSendThisTripFromAgo(String time) {
    return 'أرسل مسؤول شايل هذه الرحلة منذ $time.';
  }

  @override
  String get tripDate => 'موعد الرحلة';

  @override
  String get tripFrom => 'الرحلة من';

  @override
  String get tripTo => 'الرحلة إلى';

  @override
  String get vehicleType => 'نوع المركبة';

  @override
  String get tripCost => 'سعر الرحلة';

  @override
  String get tripValue => 'قيمة';

  @override
  String get waitingForSupervisorApproval => 'في انتظار موافقة المشرف';

  @override
  String get editRequest => 'تعديل الطلب';

  @override
  String get cannotEditThisTrip => 'لا يمكن التعديل على هذه الرحلة';

  @override
  String get confirmCancelTrip => 'هل أنت متأكد من إلغاء هذه الرحلة؟';

  @override
  String get editRequestUpdatedTitle => 'تم تعديل الطلب';

  @override
  String get editRequestUpdatedMessage => 'تم تحديث بيانات طلب الحجز بنجاح';

  @override
  String get cancelTrip => 'الغاء الرحلة';

  @override
  String get bookTrip => 'احجز الرحلة';

  @override
  String get requestTripBooking => 'طلب حجز الرحلة';

  @override
  String get selectDriver => 'اختر سائق';

  @override
  String get searchForDriver => 'البحث عن سائق';

  @override
  String get noDrivers => 'لا يوجد سائقون';

  @override
  String get chooseThisDriver => 'اختر هذا السائق';

  @override
  String get chooseDriverPlaceholder => 'اختر السائق';

  @override
  String get driversFromYourList => 'السائقون من قائمتك';

  @override
  String get tapToAddNewDriver => 'اضغط هنا لإضافة سائق جديد';

  @override
  String get orAddNewDriver => 'أو اضف سائق جديد';

  @override
  String get addNewDriver => 'إضافة سائق جديد';

  @override
  String get addDriver => 'إضافة سائق';

  @override
  String get driverInformation => 'معلومات السائق';

  @override
  String get driverDocumentsHint => 'البطاقة و رخصة القيادة و رخصة العربية';

  @override
  String get pleaseUploadRequiredDocuments => 'يرجى رفع المستندات المطلوبة';

  @override
  String get invalidNationalId => 'رقم قومي غير صحيح';

  @override
  String get pickupPoint => 'نقطة الاستلام';

  @override
  String get dropoffPoint => 'نقطة الوصول';

  @override
  String get tripBookedTitle => 'لقد تم حجز هذه الرحلة لك.';

  @override
  String get tripBookedUpdateTitle => 'تم تعديل طلب حجزك';

  @override
  String get tripBookedMessage =>
      'انتظر مسؤل الشركة للموافقة لك علي هذه الرحلة';

  @override
  String get pleaseSelectDriver => 'يرجى اختيار سائق أولاً';

  @override
  String get favoriteList => 'القائمة المفضلة';

  @override
  String get preview => 'معاينة';

  @override
  String newTripRequestsCount(String count) {
    return 'طلبات نقلات جديدة ($count)';
  }

  @override
  String get allYourTripRequests => 'جميع طلبات النقلات لديك';

  @override
  String get allYourBookedTripRequests =>
      'جميع طلبات النقلات التي قدمت عليها في انتظار موافقة المسئول';

  @override
  String get notStarted => 'لم تبدأ';

  @override
  String get finishedTransfers => 'نقلات انتهت';

  @override
  String get cancellations => 'إلغاء';

  @override
  String get requestedTransfers => 'نقلات مطلوبة';

  @override
  String get homeNewTripBadge => 'جديد';

  @override
  String get bookedTransfers => 'نقلات محجوزة';

  @override
  String requestedTransfersCount(String count) {
    return 'نقلات مطلوبة ($count)';
  }

  @override
  String bookedTransfersCount(String count) {
    return 'نقلات محجوزة ($count)';
  }

  @override
  String priceInEgp(String amount) {
    return '$amount جنيه';
  }

  @override
  String get all => 'الكل';

  @override
  String get requested => 'مطلوبة';

  @override
  String get pending => 'في الإنتظار';

  @override
  String get scheduled => 'مجدولة';

  @override
  String get inProgress => 'في الطريق';

  @override
  String get completed => 'مكتملة';

  @override
  String get tripCompleted => 'اكتملت';

  @override
  String get tripWasCancelled => 'تم الالغاء';

  @override
  String get delivered => 'تم التسليم';

  @override
  String get loadingCargo => 'جاري التحميل';

  @override
  String get reviewed => 'تمت المراجعة';

  @override
  String get cancelled => 'ملغاة';

  @override
  String get draft => 'مسودة';

  @override
  String get tripInProgress => 'رحلة قيد التنفيذ';

  @override
  String get newRequests => 'طلبات جديدة';

  @override
  String get alreadyBooked => 'محجوزة بالفعل';

  @override
  String get searchForTrip => 'البحث عن رحلة';

  @override
  String get filter => 'تصنيف';

  @override
  String get applyFilter => 'اعرض التصنيف';

  @override
  String get clearFilter => 'مسح التصنيف';

  @override
  String get searchForPlace => 'بحث عن المكان';

  @override
  String get searchForPlaceDesc =>
      'ابحث عن مدينة او منطقة متاح فيها رحلات يمكنك الوصول إليها';

  @override
  String get movingFrom => 'التحرك من';

  @override
  String get confirm => 'تأكيد';

  @override
  String get skip => 'تخطي';

  @override
  String get pleaseConfirmYourPassword => 'يرجى تأكيد كلمة المرور الخاصة بك';

  @override
  String get passwordChangedSuccessfully => 'تم تغيير كلمة المرور بنجاح';

  @override
  String tripDetailsNumber(String id) {
    return 'تفاصيل الرحلة #$id';
  }

  @override
  String get notDue => 'غير مستحقة';

  @override
  String get financialDetails => 'تفاصيل مالية';

  @override
  String get driverDetails => 'تفاصيل السائق';

  @override
  String get tripDispatchTime => 'موعد إرسال الرحلة';

  @override
  String get tripAcceptanceTime => 'موعد قبول الرحلة';

  @override
  String get elapsedTripDuration => 'وقت الرحلة المستغرق';

  @override
  String get distanceInKm => 'عدد الكيلو متر';

  @override
  String kmCount(String count) {
    return '$count كيلو متر';
  }

  @override
  String get notes => 'ملاحظات';

  @override
  String get noNotes => 'لا يوجد';

  @override
  String get otherDiscounts => 'خصومات اخرى';

  @override
  String hoursCount(String count, String unit) {
    return '$count $unit';
  }

  @override
  String get changeDriver => 'تغيير السائق';

  @override
  String get chooseNewDriver => 'اختر السائق جديد';

  @override
  String get cancelBooking => 'إلغاء الحجز';

  @override
  String get confirmChanges => 'تأكيد التغييرات';

  @override
  String get assignTheDriver => 'قم بتعيين السائق';

  @override
  String get chooseDriver => 'آختر السائق';

  @override
  String get yourSupervisorHasApprovedTheTripPleaseAddDriver =>
      'وافق لك مسؤول شايل علي الرحلة، من فضلك قم بإضافة سائق.';

  @override
  String get ifDriverIsNotAppointedWithinTheTripWillBeSentToAnotherContractor =>
      'في حالة عدم تعيين سائق خلال ؟؟ سيتم ارسال الرحلة لمقاول اخر';

  @override
  String get readyForTrip => 'جاهز للرحلة';

  @override
  String get youHaveSuccessfullyAddedTheDriver => 'لقد قمت إضافة السائق بنجاح';

  @override
  String youHaveSuccessfullyAddedTheDriverDesc(String date) {
    return 'الرحلة ستتحرك في $date كن جاهزا لها٫ في حدوث اي استفسارات لا تتردد بمكالمة المسئول';
  }

  @override
  String get waitingForManagerApproval => 'في انتظار موافقة المسئول';

  @override
  String get today => 'اليوم';

  @override
  String get chooseVehicleType => 'اختر نوع المركبة';

  @override
  String get tripStartDate => 'موعد بدء الرحلة';

  @override
  String get tripEndDate => 'موعد نهاية الرحلة';

  @override
  String get tripStatus => 'حالة الرحلة';

  @override
  String get justNow => 'الآن';

  @override
  String get timeAm => 'ص';

  @override
  String get timePm => 'م';

  @override
  String get meridiemAm => 'صباحاً';

  @override
  String get meridiemPm => 'مساءً';

  @override
  String get lessThanAMinute => 'أقل من دقيقة';

  @override
  String get elapsedMinuteUnit => 'دقيقة';

  @override
  String get elapsedMinutesUnit => 'دقائق';

  @override
  String get elapsedHourUnit => 'ساعة';

  @override
  String get elapsedHoursUnit => 'ساعات';

  @override
  String get elapsedDayUnit => 'يوم';

  @override
  String get elapsedDaysUnit => 'أيام';

  @override
  String get elapsedTwoMinutes => 'دقيقتين';

  @override
  String get elapsedTwoHours => 'ساعتين';

  @override
  String get elapsedTwoDays => 'يومين';

  @override
  String get elapsedMinuteSingular => 'دقيقة';

  @override
  String elapsedMinutesFew(int count) {
    return '$count دقائق';
  }

  @override
  String elapsedMinutesOther(int count) {
    return '$count دقيقة';
  }

  @override
  String get elapsedHourSingular => 'ساعة';

  @override
  String elapsedHoursFew(int count) {
    return '$count ساعات';
  }

  @override
  String elapsedHoursOther(int count) {
    return '$count ساعة';
  }

  @override
  String get elapsedDaySingular => 'يوم';

  @override
  String elapsedDaysFew(int count) {
    return '$count أيام';
  }

  @override
  String elapsedDaysOther(int count) {
    return '$count يوم';
  }

  @override
  String elapsedMinutesShort(int count) {
    return '$count min';
  }

  @override
  String priceInArabicEgpFull(String amount) {
    return '$amount جنيه مصري';
  }

  @override
  String get routeError => 'خطأ في المسار';

  @override
  String fileSizeKb(String size) {
    return '$size KB';
  }

  @override
  String fileSizeMb(String size) {
    return '$size MB';
  }

  @override
  String financialDueAllTripsCount(String count) {
    return 'عدد كل النقلات : $count نقلة';
  }

  @override
  String get dueTripsCountLabel => 'عدد النقلات المستحقة';

  @override
  String get amountDueUntilNow => 'المبلغ المستحق حتى الآن';

  @override
  String get financialDueAmountDescription =>
      'مبلغ مستحق هو المبلغ الذي تستلمه من شركة شايل بعد تسليم مستندات النقلة.';

  @override
  String get allTrips => 'كل النقلات';

  @override
  String get dueTransferTripBadge => 'نقلة مستحقة';

  @override
  String get entitlementFulfilledBadge => 'تم الاستحقاق';

  @override
  String get transferTripPricePrefix => 'سعر النقلة :';
}
