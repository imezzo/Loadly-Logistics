// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Arabic (`ar`).
class AppLocalizationsAr extends AppLocalizations {
  AppLocalizationsAr([String locale = 'ar']) : super(locale);

  @override
  String get appTitle => 'شايل لمكان العمل';

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
  String get resendPrompt => 'لسه مجاش رسالة؟';

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
  String get startShippingAndLoading => 'بداية الشحن والتحميل';

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
  String get shipmentDetails => 'تفاصيل الشحنة';

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
  String get departFromWaypoint => 'مغادرة';

  @override
  String get completeLoading => 'إكمال التحميل';

  @override
  String get startUnloading => 'بدء التفريغ';

  @override
  String get completeUnloading => 'إكمال التفريغ';

  @override
  String get optionalNotePlaceholder => 'ملاحظة اختيارية';

  @override
  String get completeTrip => 'إكمال الرحلة';

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
  String get locationPermissionDisclosureTitle => 'كيف يستخدم شايل  موقعك';

  @override
  String get locationPermissionDisclosureBody =>
      'يجمع تطبيق شايل  موقع جهازك—بما في ذلك في الخلفية عندما لا يكون التطبيق ظاهراً على الشاشة—أثناء كونك متصلاً أو أثناء تنفيذ الرحلات. نستخدمه لعرض موضعك على الخريطة، والتبديل بدقة بين وضعي الاتصال، والتخطيط لمسارات الاستلام والتسليم، ومشاركة الموقع المباشر مع شركتك. يُرسل الموقع فقط إلى خوادم شايل عند الحاجة لهذه ميزات السائق.\n\nسيُطلب منك أولاً السماح بالموقع أثناء استخدام التطبيق. إذا لزم التتبع المباشر الكامل، ستظهر شاشة ثانية تشرح الوصول في الخلفية، ثم خطوة إذن أخرى من النظام (مثل «السماح طوال الوقت»). اضغط «متابعة» للبدء.';

  @override
  String get locationBackgroundPermissionDisclosureTitle => 'الموقع في الخلفية';

  @override
  String get locationBackgroundPermissionDisclosureBody =>
      'لكي تصل شركتك بتحديثات مباشرة موثوقة أثناء القيادة، يحتاج شايل  إلى إذن للوصول إلى الموقع في الخلفية (عندما يكون التطبيق خلف تطبيقات أخرى أو الشاشة مطفأة). يُستخدم ذلك فقط لميزات السائق الموضحة في الشاشة السابقة—وليس للإعلانات.\n\nاضغط «متابعة» لفتح طلب الإذن التالي من النظام. على بعض الأجهزة يجب اختيار «السماح طوال الوقت» أو تفعيل الموقع في الخلفية من الإعدادات.';

  @override
  String get notificationPermissionDisclosureTitle => 'الإشعارات';

  @override
  String get notificationPermissionDisclosureBody =>
      'يرسل تطبيق شايل  إشعارات عن الرحلات والجداول والتحديثات المهمة. لإيصالها يستخدم التطبيق الإشعارات الفورية ورمز جهاز يُخزَّن على خوادمنا. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get cameraPermissionDisclosureTitle => 'الوصول إلى الكاميرا';

  @override
  String get cameraPermissionDisclosureBody =>
      'يستخدم تطبيق شايل الكاميرا فقط عندما تختار التقاط صورة—مثلاً لصورة الملف الشخصي أو المستندات المطلوبة. تُرفع الصور التي تلتقطها إلى خوادمنا لحسابك. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get photosPermissionDisclosureTitle => 'الصور والملفات';

  @override
  String get photosPermissionDisclosureBody =>
      'يصل تطبيق شايل  إلى الصور التي تختارها من معرض الصور أو تخزين الجهاز لرفع صور الملف الشخصي أو المستندات. لا نصل إلا للملفات التي تختارها أنت. اضغط «متابعة» لفتح طلب إذن النظام.';

  @override
  String get appUpdateRequiredTitle => 'التحديث مطلوب';

  @override
  String get appUpdateAvailableTitle => 'تحديث متاح';

  @override
  String get appUpdateRequiredMessage =>
      'يلزم إصدار جديد من تطبيق شايل للمتابعة. يرجى التحديث إلى أحدث إصدار.';

  @override
  String get appUpdateAvailableMessage =>
      'يتوفر إصدار جديد من تطبيق شايل يتضمن تحسينات وإصلاحات.';

  @override
  String appUpdateLatestVersion(String version) {
    return 'أحدث إصدار: $version';
  }

  @override
  String get appUpdateNow => 'حدّث الآن';

  @override
  String get appUpdateLater => 'لاحقاً';

  @override
  String get filter => 'تصنيف';

  @override
  String get tripDateIs => 'الرحلة في يوم';

  @override
  String get collectDocuments => 'أستلم المستندات';

  @override
  String get partiallyCollectedDocuments => 'تم جمع جزء من المستندات';

  @override
  String get noDocumentsAvailable => 'لا توجد مستندات متاحة';

  @override
  String get received => 'تم الاستلام';

  @override
  String get notReceived => 'لم يتم الاستلام';

  @override
  String get stillWithCustomerWaitingForSignature =>
      'لا يزال مع العميل (في انتظار التوقيع)';

  @override
  String get stillWithCustomer => 'لا يزال مع العميل';

  @override
  String get lostByDriver => 'فُقد/ضاع من قبل السائق';

  @override
  String get lostDocument => 'مستند ضائع';

  @override
  String get tripDoesNotExist => 'لا يوجد لهذه الرحلة';

  @override
  String get willBeSentLater => 'سيتم إرساله لاحقًا';

  @override
  String get customerRefusedToSign => 'رفض العميل التوقيع';

  @override
  String get other => 'سبب أخر';

  @override
  String get note => 'ملاحظة';

  @override
  String get confirm => 'تأكيد';

  @override
  String get deliveryPermission => 'اذن التسليم';

  @override
  String get selectTheReasonsForNotReceivingTheDocument =>
      'أختر سبب عدم استلام المستند';

  @override
  String get enterNote => 'فى حالة سبب أخر يرجى ادخال ملاحظة ';

  @override
  String get takeAPhotoOfTheDocument => 'قم بتصوير المستند ( أختياري)';

  @override
  String get openCameraDocument => 'أفتح الكاميرا لتصوير المستند';

  @override
  String get saveAndContinueLater => 'حفظ واستكمل لاحقا';

  @override
  String get saveAndSendDocuments => 'حفظ و ارسال المستندات';

  @override
  String get confirmAndContinueLater => 'تأكيد واستكمل لاحقا';

  @override
  String get confirmAndSendDocuments => 'تأكيد و ارسال المستندات';

  @override
  String get confirmAllDocuments => 'تأكيد استلام كل المستندات';

  @override
  String get pleaseConfirmAllDocuments => 'يرجى تأكيد استلام كل المستندات';

  @override
  String get saveAndSendDocumentsDesc =>
      'اضغط إرسال، سيتم الحفظ و ارسال المستندات إلى شركة LOADLY خلال ثواني';

  @override
  String get saved => 'تم الحفظ';

  @override
  String get savedDesc =>
      'تم حفظ العملية و يرجى الاستكمال لاحقا لجمع المستندات و إرسالها للشركة في اسرع وقت.';

  @override
  String get theDocumentsThatWillBeSent => 'المستندات التي سيتم ارسالها';

  @override
  String get sendDocuments => 'ارسال المستندات';

  @override
  String get pullToRefresh => 'اسحب للتحديث';

  @override
  String get releaseToRefresh => 'ارفع إصبعك للتحديث';

  @override
  String get searchForContractId => 'بحث عن رقم العقد';

  @override
  String get searchForCompanyId => 'بحث عن رقم الشركة';

  @override
  String get searchForVehicleId => 'بحث عن رقم المركبة';

  @override
  String get searchForVehicleTypeId => 'بحث عن رقم نوع المركبة';

  @override
  String get searchForDriverId => 'بحث عن رقم السائق';

  @override
  String get searchByDate => 'بحث بالتاريخ';

  @override
  String get today => 'اليوم';

  @override
  String get yesterday => 'الامس';

  @override
  String get lastWeek => 'اخر 7 ايام';

  @override
  String get chooseDate => 'أختر يوم محدد';

  @override
  String get tripStatus => 'حالة الرحلة';

  @override
  String get noDocumentsCollected => 'لم يتم جمع المستندات';

  @override
  String get partiallyConfirmedDocuments => 'تم تأكيد جزء من المستندات';

  @override
  String get resetFilter => 'مسح التصنيف';

  @override
  String get startDate => 'بداية الرحلة';

  @override
  String get endDate => 'نهاية الرحلة';

  @override
  String get done => 'تم';

  @override
  String get reset => 'مسح';

  @override
  String get companyName => 'اسم الشركة';

  @override
  String get driverName => 'اسم السائق';

  @override
  String get carNumber => 'رقم السيارة';

  @override
  String get documentsPictures => 'صور المستندات';

  @override
  String get receiveDocumentsForSelectedTrips =>
      'أستلم المستندات للرحلات المختارة';

  @override
  String get documentsAreBeingCollected => 'جاري جمع المستندات';

  @override
  String get documentsAreBeingCollectedDesc =>
      'انتظر لحظات حتى يتم جمع كل مستندات الرحلات مرة واحدة.';

  @override
  String get selectedTripsDocuments => 'مستندات الرحلات المختارة';

  @override
  String get receiptConfirmed => 'تم تأكيد الاستلام ';

  @override
  String get confirmed => 'تم تأكيد الاستلام ';

  @override
  String get receiptConfirmedDesc =>
      'اضغط تأكيد، وسيتم تأكيد و ارسال المستندات إلى شركة LOADLY خلال ثواني';

  @override
  String get selectedTrips => 'الرحلات المختارة';

  @override
  String get receiptConfirmation => 'تأكيد الإستلام';

  @override
  String get receiptConfirmationWithMark => 'تأكيد الإستلام؟';

  @override
  String get documentsCollectedByFactoryCollector =>
      'المستندات التي جمعها مندوب المصنع';

  @override
  String get documentsCollectedFromFactoryCollector =>
      'تم جمع المستندات من مندوب المصنع';

  @override
  String get receiptConfirmationForSelectedTrips =>
      'تأكيد استلام للرحلات المختارة';

  @override
  String get confirmWithMark => 'تأكيد؟';

  @override
  String get incorrectPassword => 'كلمة المرور غير صحيحة';

  @override
  String get withCollector => 'تم جمعها من مندوب المصنع';

  @override
  String get externalTrip => 'رحلة خارجية';

  @override
  String get username => 'اسم المستخدم';

  @override
  String get enterUsername => 'أدخل اسم المستخدم';

  @override
  String get enterValidUsername =>
      'أدخل اسم مستخدم صحيح يجب أن يكون اكبر من 3 حروف';

  @override
  String get displayDocuments => 'عرض المستندات';

  @override
  String get noDriver => 'لا يوجد سائق';

  @override
  String get edit => 'تعديل';

  @override
  String get factoryCollector => 'مندوب المصنع';

  @override
  String get search => 'بحث';

  @override
  String get unexpectedError => 'حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى.';

  @override
  String get networkErrorPleaseRetry =>
      'حدث خطأ في الاتصال بالشبكة. يرجى التحقق من الاتصال والمحاولة مرة أخرى.';

  @override
  String get noInternetConnection =>
      'لا يوجد اتصال بالإنترنت. يرجى التحقق من الاتصال والمحاولة مرة أخرى.';

  @override
  String get requestFailedTryAgain =>
      'تعذر إكمال الطلب. يرجى المحاولة مرة أخرى.';

  @override
  String get failedToLoadTrips => 'تعذر تحميل الرحلات. يرجى المحاولة مرة أخرى.';

  @override
  String get failedToLoadDocuments =>
      'تعذر تحميل المستندات. يرجى المحاولة مرة أخرى.';

  @override
  String get failedToCollectDocuments =>
      'تعذر استلام المستندات. يرجى المحاولة مرة أخرى.';

  @override
  String get failedToConfirmDocuments =>
      'تعذر تأكيد المستندات. يرجى المحاولة مرة أخرى.';

  @override
  String get invalidLoginCredentials => 'اسم المستخدم أو كلمة المرور غير صحيحة';

  @override
  String get invalidPhoneNumber => 'رقم الهاتف غير صحيح';

  @override
  String get invalidVerificationCode => 'رمز التحقق غير صحيح أو انتهت صلاحيته.';

  @override
  String get sessionExpired =>
      'انتهت صلاحية الجلسة. يرجى تسجيل الدخول مرة أخرى.';

  @override
  String get invalidRequest => 'الطلب غير صالح. يرجى مراجعة البيانات المدخلة.';

  @override
  String get resourceNotFound => 'العنصر المطلوب غير موجود.';

  @override
  String get actionNotAllowed => 'هذا الإجراء غير مسموح به.';

  @override
  String get invalidSubmittedData => 'البيانات المدخلة غير صحيحة.';

  @override
  String get serverErrorTryLater => 'حدث خطأ في الخادم. يرجى المحاولة لاحقاً.';

  @override
  String get failedToLoadProfile =>
      'تعذر تحميل الملف الشخصي. يرجى المحاولة مرة أخرى.';

  @override
  String get somethingWentWrongTryAgain =>
      'حدث خطأ ما. يرجى المحاولة مرة أخرى.';

  @override
  String get emptyServerResponse =>
      'تعذر قراءة الاستجابة من الخادم. يرجى المحاولة مرة أخرى.';

  @override
  String get missingServerData =>
      'البيانات المطلوبة غير متوفرة. يرجى المحاولة مرة أخرى.';

  @override
  String get passwordResetFailed =>
      'تعذر إعادة تعيين كلمة المرور. يرجى المحاولة مرة أخرى.';

  @override
  String get couldNotSendResetCode =>
      'تعذر إرسال رمز استعادة كلمة المرور. يرجى المحاولة مرة أخرى.';

  @override
  String get tooManyAttempts =>
      'تم تجاوز عدد المحاولات المسموح به. يرجى الانتظار ثم المحاولة مرة أخرى.';

  @override
  String get notificationChannelDescription => 'إشعارات التطبيق العامة';

  @override
  String get notificationFallbackTitle => 'إشعار';
}
