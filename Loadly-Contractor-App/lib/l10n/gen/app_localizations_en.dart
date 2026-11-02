// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appTitle => 'Loadly Contractor';

  @override
  String get welcome => 'Welcome';

  @override
  String get welcomeToLoadly => 'Welcome to Loadly';

  @override
  String get welcomeDesc => 'Sign in to your account or create a new one';

  @override
  String get switchLanguage => 'Switch language';

  @override
  String get createAccount => 'Create account';

  @override
  String get createNewAccount => 'Create new account';

  @override
  String get verificationInstruction =>
      'Enter your phone number and we will send you a verification code';

  @override
  String get phone => 'Phone number';

  @override
  String get enterYourPhone => 'Enter your phone number';

  @override
  String get enterYourFullName => 'Enter your full name';

  @override
  String get enterYourAddress => 'Enter your address details';

  @override
  String get enterValidPhone => 'Enter a valid phone number';

  @override
  String get enterValidPhone11Digits => 'Enter 11 digits';

  @override
  String get enterValidPhone01 => 'Phone number must start with 01';

  @override
  String get enterYourId => 'National ID Number';

  @override
  String get signInToAccount => 'Sign in to your account';

  @override
  String get email => 'Email';

  @override
  String get password => 'Password';

  @override
  String get signIn => 'Sign in';

  @override
  String get error => 'Error';

  @override
  String get ok => 'OK';

  @override
  String get enterYourEmail => 'Enter your email';

  @override
  String get enterValidEmail => 'Enter a valid email';

  @override
  String get enterYourPassword => 'Enter your password';

  @override
  String get home => 'Home';

  @override
  String get forgetPassword => 'Forgot your password?';

  @override
  String get verificationTitle => 'Verification Code';

  @override
  String get verificationSubtitle =>
      'We will send you a verification code, you can choose to send it to your email or as a text message to your mobile?';

  @override
  String get verificationSubtitlePhone =>
      'We will send you a verification code via text message to your phone.';

  @override
  String get sendViaSms => 'Send code via text message';

  @override
  String get sendViaEmail => 'Send code via email';

  @override
  String get continueButton => 'Log In';

  @override
  String otpInstructionSms(String phone) {
    return 'We sent you a verification code via text message to your number ($phone), enter it here to complete your account activation.';
  }

  @override
  String otpInstructionEmail(String email) {
    return 'We sent you a verification code via email to $email, enter it here to complete your account activation.';
  }

  @override
  String get resendPrompt => 'Didn\'t receive a message yet?';

  @override
  String get resendLink => 'Send again';

  @override
  String get verifyAccount => 'Verify Account';

  @override
  String get verifyEmail => 'Verify Email';

  @override
  String get verifyEmailInstruction =>
      'Enter the 6-digit code we sent to your email to verify your account.';

  @override
  String get enterVerificationCode => 'Enter the verification code';

  @override
  String get verificationCodeLength => 'Code must be 6 digits';

  @override
  String get verify => 'Verify';

  @override
  String get resendCode => 'Resend code';

  @override
  String get codeSentToEmail => 'Verification code sent to your email.';

  @override
  String get newPassword => 'New password';

  @override
  String get confirmPassword => 'Confirm password';

  @override
  String get resetPassword => 'Reset password';

  @override
  String get passwordMinLength => 'Password must be at least 6 characters';

  @override
  String get personalAccount => 'Personal Account';

  @override
  String get employeeDriverName => 'Employee/Driver Name';

  @override
  String get emailLabel => 'Email';

  @override
  String get rating => 'Rating';

  @override
  String get trip => 'Trip';

  @override
  String get trips => 'Trips';

  @override
  String get files => 'Files';

  @override
  String get myPersonalData => 'My Personal Data';

  @override
  String get fileName => 'File Name';

  @override
  String get chooseFromGallery => 'Choose from Gallery';

  @override
  String get profilePage => 'Profile Page';

  @override
  String get notifications => 'Notifications';

  @override
  String get tripsNav => 'Trips';

  @override
  String get homeNav => 'Home';

  @override
  String secondsRemaining(int seconds) {
    return '$seconds seconds remaining';
  }

  @override
  String get loginWithPassword => 'Login with password';

  @override
  String get loginWithBiometric => 'Login with Biometric';

  @override
  String get or => 'OR';

  @override
  String get backToLogin => 'Back to login';

  @override
  String get rememberMe => 'Remember me';

  @override
  String get online => 'Online';

  @override
  String get offline => 'Offline';

  @override
  String get active => 'Active';

  @override
  String get inactive => 'Inactive';

  @override
  String get logout => 'Logout';

  @override
  String get changePassword => 'Change Password';

  @override
  String get enableFingerprint => 'Enable Fingerprint';

  @override
  String get biometricErrorFailedOrCancelled =>
      'Biometric authentication failed or cancelled.';

  @override
  String get biometricErrorEnableFailed => 'Failed to enable biometric login.';

  @override
  String get biometricErrorDisableFailed =>
      'Failed to disable biometric login.';

  @override
  String get biometricLocalizedReasonSignIn => 'Authenticate to sign in';

  @override
  String get biometricLocalizedReasonEnable =>
      'Authenticate to enable fingerprint login';

  @override
  String get biometricEnablePasswordDialogMessage =>
      'Enter your password to enable fingerprint login.';

  @override
  String get biometricOfferAfterLoginMessage =>
      'Would you like to enable fingerprint login for faster access?';

  @override
  String get biometricAccountInfoUnavailable =>
      'Could not get your account info. Please try again.';

  @override
  String get enable2FA => 'Enable 2FA';

  @override
  String get support => 'Support';

  @override
  String get settings => 'Settings';

  @override
  String get cancel => 'Cancel';

  @override
  String get passwordsDoNotMatch => 'Passwords do not match';

  @override
  String get fullNameEnglish => 'Full Name (English)';

  @override
  String get fullNameArabic => 'Full Name (Arabic)';

  @override
  String get userName => 'User Name';

  @override
  String get save => 'Save';

  @override
  String get editProfile => 'Edit Profile';

  @override
  String get profileUpdatedSuccessfully => 'Profile updated successfully';

  @override
  String get imagePickerNotImplemented => 'Image picker not implemented yet';

  @override
  String get retry => 'Retry';

  @override
  String get user => 'User';

  @override
  String get cameraPermissionRequired =>
      'Camera permission is required to take photos. Please enable it in Settings.';

  @override
  String get photoLibraryPermissionRequired =>
      'Photo library permission is required to select photos. Please enable it in Settings.';

  @override
  String get permissionDenied => 'Permission Denied';

  @override
  String get openSettings => 'Open Settings';

  @override
  String get takePhoto => 'Take Photo';

  @override
  String get todayNotifications => 'Today\'s Notifications';

  @override
  String get company => 'Company';

  @override
  String get companyNotification => 'Company Notification';

  @override
  String get newTripSent => 'A new trip has been sent, click to view details';

  @override
  String get retrievalNotificationSent =>
      'A retrieval notification has been sent to the company awaiting your reply';

  @override
  String get replacementCarOnWay => 'A replacement car is on its way to you';

  @override
  String get vehicleDetails => 'Vehicle Details';

  @override
  String minsAgo(int mins) {
    return '$mins mins ago';
  }

  @override
  String hoursAgo(int hours) {
    return '$hours hours ago';
  }

  @override
  String get tripDetails => 'Trip Details';

  @override
  String get tripId => 'Trip ID';

  @override
  String get status => 'Status';

  @override
  String get statusRequested => 'Requested';

  @override
  String get statusPending => 'Pending';

  @override
  String get statusScheduled => 'Scheduled';

  @override
  String get statusDriverAccepted => 'Driver Accepted';

  @override
  String get statusFirstPickupLoading => 'Loading';

  @override
  String get statusInProgress => 'In Progress';

  @override
  String get statusCompleted => 'Completed';

  @override
  String get statusReviewed => 'Reviewed';

  @override
  String get statusCancelled => 'Cancelled';

  @override
  String get statusUnknown => '—';

  @override
  String get startTrip => 'Start Trip';

  @override
  String get startShippingAndLoading => 'Start Shipping and Loading';

  @override
  String tripStartsOn(String date, String time) {
    return 'Trip starts on: $date at $time';
  }

  @override
  String get startLoadingShipment => 'Start loading shipment';

  @override
  String get pickupLocation => 'Pickup';

  @override
  String get startTripLocation => 'Start Trip';

  @override
  String get deliveryLocation => 'Drop Off';

  @override
  String get weight => 'Weight';

  @override
  String get shipmentType => 'Shipment Type';

  @override
  String get tripDuration => 'Trip Duration';

  @override
  String get shipmentDimensions => 'Shipment Dimensions';

  @override
  String get approximately => 'Approximately';

  @override
  String get hours => 'hours';

  @override
  String get stopPoints => 'Stop Points';

  @override
  String get tripPoints => 'Trip Points';

  @override
  String get stop => 'Stop';

  @override
  String get clientDetails => 'Client Details';

  @override
  String get phoneNumber => 'Phone Number';

  @override
  String get responsiblePersonName => 'Contact Name';

  @override
  String get isThereAProblemCallSupport => 'Is there a problem? Call Support';

  @override
  String get clickToConfirm => 'Click to Confirm';

  @override
  String get tripDetailsPlaceholder => 'Details are written here';

  @override
  String get noTripsAvailable => 'No trips available';

  @override
  String get noNotifications => 'No notifications';

  @override
  String get noFilesAvailable => 'No files available';

  @override
  String get loadingTrips => 'Loading trips...';

  @override
  String get errorLoadingTrips => 'Error loading trips';

  @override
  String get currentTripTab => 'Current Trip';

  @override
  String get previousTripsTab => 'Previous Trips';

  @override
  String get yourCurrentTrip => 'Your Current Trip';

  @override
  String get yourPreviousTrips => 'Your Previous Trips';

  @override
  String get shipmentDetails => 'Shipment Details';

  @override
  String tripOnDayAtTime(String date, String time) {
    return 'Trip on: $date at $time';
  }

  @override
  String get onTheWayToLoading => 'On the way to loading';

  @override
  String get tripScheduleUpdated => 'Trip Schedule Updated';

  @override
  String tripScheduleUpdatedMessage(String oldSchedule, String newSchedule) {
    return 'Your trip schedule has been updated: $oldSchedule to $newSchedule';
  }

  @override
  String get newSchedule => 'New schedule';

  @override
  String get acceptTrip => 'Accept Trip';

  @override
  String get arriveAtWaypoint => 'I\'ve Arrived';

  @override
  String get departFromWaypoint => 'Depart';

  @override
  String get completeLoading => 'Complete Loading';

  @override
  String get startUnloading => 'Start Unloading';

  @override
  String get completeUnloading => 'Complete Unloading';

  @override
  String get optionalNotePlaceholder => 'Optional note';

  @override
  String get completeTrip => 'Complete Trip';

  @override
  String get openCameraForPhotos => 'Open camera for taking photos';

  @override
  String get openCamera => 'Open camera';

  @override
  String get processing => 'Processing...';

  @override
  String get shipmentPhotos => 'Shipment Photos';

  @override
  String get receiptNote => 'Receipt Note';

  @override
  String get deliveryNote => 'Delivery Note';

  @override
  String get unloadingPhotos => 'Unloading Photos';

  @override
  String get tollgatePhotos => 'Tollgate Photos';

  @override
  String get tollgatesTotalAmount => 'Tollgates Total Amount';

  @override
  String get tollgatesWeightTotalAmount => 'Tollgates Weight Total Amount';

  @override
  String get tollgateWeightImages => 'Tollgate Weight Images';

  @override
  String get notePlaceholder => 'Note';

  @override
  String get noteRequired => 'Note is required';

  @override
  String get tripPointNoNote => 'No note';

  @override
  String get currentLocationLoading => 'Fetching location...';

  @override
  String get viewDocument => 'View Document';

  @override
  String get odometer => 'Odometer';

  @override
  String get odometerImage => 'Odometer Image';

  @override
  String get receiptImage => 'Receipt Image';

  @override
  String get shipmentImages => 'Shipment Images';

  @override
  String get deliveryNoteImage => 'Delivery Note Image';

  @override
  String get tollgateImages => 'Tollgate Images';

  @override
  String get tripErrorGeneric => 'An error occurred. Please try again.';

  @override
  String get tripErrorMustBeScheduledToAccept =>
      'Trip must be scheduled to accept.';

  @override
  String get tripErrorDeliveryNoteRequired =>
      'Delivery note image is required at dropoff.';

  @override
  String get tripErrorWaypointMustBePending => 'Waypoint must be pending.';

  @override
  String get tripErrorWaypointMustBeArrived =>
      'You must arrive at the waypoint first.';

  @override
  String get tripErrorWaypointMustBeLoadingStarted =>
      'Loading must be started first.';

  @override
  String get tripErrorWaypointMustBeLoadingCompleted =>
      'Loading must be completed first.';

  @override
  String get tripErrorAllWaypointsMustBeCompleted =>
      'All waypoints must be completed first.';

  @override
  String get tripErrorFirstWaypointMustBeLoadingCompleted =>
      'First pickup loading must be completed first.';

  @override
  String get tripErrorTripMustBeInProgress => 'Trip must be in progress.';

  @override
  String get tripErrorTripMustBeDriverAccepted =>
      'Trip must be accepted first.';

  @override
  String get tripErrorTripMustBeFirstPickupLoading =>
      'First pickup loading must be completed first.';

  @override
  String get tripErrorInvalidImageFormat => 'Image must be JPEG, PNG, or WebP.';

  @override
  String get tripErrorImageTooLarge => 'Image must be 10MB or less.';

  @override
  String get tripErrorInvalidOdometerImage =>
      'Please capture a clear photo of the car odometer.';

  @override
  String get tripErrorUnreadableOdometerImage =>
      'Could not read the odometer. Please retake the photo with better focus.';

  @override
  String get loadMore => 'Load More';

  @override
  String get filterByDate => 'Filter by Date';

  @override
  String get fromDate => 'From';

  @override
  String get toDate => 'To';

  @override
  String get apply => 'Apply';

  @override
  String get clear => 'Clear';

  @override
  String get documents => 'Documents';

  @override
  String get documentOpenFailed => 'Could not open document. Copy URL?';

  @override
  String get copyUrl => 'Copy URL';

  @override
  String get urlCopied => 'URL copied to clipboard';

  @override
  String get documentTypeMissing => 'Could not detect document type.';

  @override
  String get language => 'Language';

  @override
  String get languageArabic => 'Arabic';

  @override
  String get languageEnglish => 'English';

  @override
  String get navigateToWaypoint => 'Navigate to waypoint';

  @override
  String get navigateToPickup => 'Navigate to pickup';

  @override
  String get navigateToDropoff => 'Navigate to dropoff';

  @override
  String get showRoadRoute => 'Show road route';

  @override
  String get loadingRoadRoute => 'Loading route…';

  @override
  String get tripCompletedSuccessfully => 'Trip Completed Successfully';

  @override
  String get tripCompletedSuccessMessage =>
      'Thank you for completing the trip. Great job!';

  @override
  String get backToTrips => 'Back to Trips';

  @override
  String get homeLocationServicesDisabled =>
      'Location services are turned off. Turn them on to use the map and go online.';

  @override
  String get homeLocationPermissionDenied =>
      'Location permission was denied. Allow location access to use the map and go online.';

  @override
  String get homeLocationPermissionDeniedForever =>
      'Location permission is permanently denied. Enable it in the app settings.';

  @override
  String get homeLocationUnavailable =>
      'Location is unavailable. Try restarting the app.';

  @override
  String get homeLocationGetPositionFailed =>
      'Could not get your location. Please try again.';

  @override
  String get homeLocationGetInitialFailed =>
      'Could not start location updates. Please try again.';

  @override
  String get homeLocationStreamError =>
      'Location updates stopped. Please try again.';

  @override
  String appVersion(String version) {
    return 'Version $version';
  }

  @override
  String get prominentDisclosureContinue => 'Continue';

  @override
  String get prominentDisclosureNotNow => 'Not now';

  @override
  String get locationPermissionDisclosureTitle =>
      'How Loadly Contractor uses your location';

  @override
  String get locationPermissionDisclosureBody =>
      'Loadly Contractor collects your device location—including in the background when the app is not on screen—while you are online or working on trips. We use it to show your position on the map, switch online/offline accurately, plan routes for pickups and deliveries, and share live location with your company. Location is sent only to Loadly servers as needed for these contractor features.\n\nYou will first be asked to allow location while using the app. If full live tracking is needed, a second screen will explain background access, followed by another system permission step (for example “Allow all the time”). Tap Continue to start.';

  @override
  String get locationBackgroundPermissionDisclosureTitle =>
      'Location in the background';

  @override
  String get locationBackgroundPermissionDisclosureBody =>
      'So your company can receive reliable live updates while you drive, Loadly Contractor needs permission to access location in the background (when the app is behind other apps or the screen is off). This is only used for the contractor features described on the previous screen—not for advertising.\n\nTap Continue to open the next system permission. On some devices you must choose “Allow all the time” or enable background location in Settings.';

  @override
  String get locationTrackingNotificationTitle => 'Loadly Contractor is online';

  @override
  String get locationTrackingNotificationText =>
      'Sharing your location with Loadly while you are online.';

  @override
  String get notificationPermissionDisclosureTitle => 'Notifications';

  @override
  String get notificationPermissionDisclosureBody =>
      'Loadly Contractor sends notifications about trips, schedules, and important updates. To deliver them, the app uses push notifications and a device token stored on our servers. Tap Continue to open the system permission request.';

  @override
  String get cameraPermissionDisclosureTitle => 'Camera access';

  @override
  String get cameraPermissionDisclosureBody =>
      'Loadly Contractor uses the camera only when you choose to take a photo—for example for your profile picture or required documents. Photos you take are uploaded to our servers for your contractor account. Tap Continue to open the system permission request.';

  @override
  String get photosPermissionDisclosureTitle => 'Photos and files';

  @override
  String get photosPermissionDisclosureBody =>
      'Loadly Contractor accesses images you select from your photo library or device storage to upload profile or document images. We only access files you choose. Tap Continue to open the system permission request.';

  @override
  String get appUpdateRequiredTitle => 'Update required';

  @override
  String get appUpdateAvailableTitle => 'Update available';

  @override
  String get appUpdateRequiredMessage =>
      'A new version of Loadly Contractor is required to continue. Please update to the latest version.';

  @override
  String get appUpdateAvailableMessage =>
      'A new version of Loadly Contractor is available with improvements and fixes.';

  @override
  String appUpdateLatestVersion(String version) {
    return 'Latest version: $version';
  }

  @override
  String get appUpdateNow => 'Update now';

  @override
  String get appUpdateLater => 'Later';

  @override
  String get enterCustoms => 'Enter customs';

  @override
  String get exitCustoms => 'Exit customs';

  @override
  String get customsTotalAmount => 'Customs total amount';

  @override
  String get documentImage => 'Document image';

  @override
  String get tripErrorImageMissingRetake =>
      'One or more photos could not be found. Please retake them.';

  @override
  String get questionMark => '?';

  @override
  String get invalidCredentials => 'Invalid credentials.';

  @override
  String get invalidPhoneNumber => 'Invalid phone number.';

  @override
  String get networkError => 'No internet connection.';

  @override
  String get unknownError => 'Something went wrong.';

  @override
  String get requestFailed => 'Request failed.';

  @override
  String get failedToLoadDrivers => 'Failed to load drivers.';

  @override
  String get failedToLoadDocumentTypes => 'Failed to load document types.';

  @override
  String get failedToCreateDriver => 'Failed to create driver.';

  @override
  String get failedToUpdateTripDriver => 'Failed to update trip driver.';

  @override
  String get failedToLoadTrip => 'Failed to load trip.';

  @override
  String get failedToLoadTrips => 'Failed to load trips.';

  @override
  String get failedToLoadBookingRequests => 'Failed to load booking requests.';

  @override
  String get failedToBookTrip => 'Failed to book trip.';

  @override
  String get failedToWithdrawBooking => 'Failed to withdraw booking.';

  @override
  String get failedToLoadVehicleTypes => 'Failed to load vehicle types.';

  @override
  String get failedToLoadNotifications => 'Failed to load notifications.';

  @override
  String get failedToLoadMore => 'Failed to load more.';

  @override
  String get failedToMarkAsRead => 'Failed to mark as read.';

  @override
  String get registrationFailed => 'Registration failed.';

  @override
  String get invalidResponse => 'Invalid response. Please try again.';

  @override
  String get error400 => 'Invalid request. Please check your input.';

  @override
  String get error401 => 'Invalid or expired session. Please sign in again.';

  @override
  String get error404 => 'Resource not found.';

  @override
  String get error405 => 'This action is not allowed.';

  @override
  String get error422 => 'The given data was invalid.';

  @override
  String get error500 => 'Server error. Please try again later.';

  @override
  String get errorDefault => 'Something went wrong. Please try again.';

  @override
  String get continuee => 'Continue';

  @override
  String get trackYourPackageEasily => 'Track your package easily';

  @override
  String get trackYourPackageEasilyDesc =>
      'Enter your package ID and get real-time updates on your shipment’s journey.';

  @override
  String get realtimeNotifications => 'Realtime notifications';

  @override
  String get realtimeNotificationsDesc =>
      'Get a realtime notification about your package status. You can customize the notification easily.';

  @override
  String get uploadYourDocuments => 'Upload your documents';

  @override
  String get contractorInformation => 'Contractor information';

  @override
  String get fullName => 'Full Name';

  @override
  String get addressDetails => 'Address details';

  @override
  String get uploadImageOrBrowse => 'Upload Image or browse';

  @override
  String get pngJpgUpTo => 'PNG, JPG up to 10MB';

  @override
  String get registerConfirm =>
      'I confirm that all personal information provided is correct and I agree to the terms and conditions.';

  @override
  String get yourRequestSuccessfullyRegistered =>
      'Your request successfully registered';

  @override
  String get yourRequestSuccessfullyRegisteredDesc =>
      'We’ll check your documents. and open your account to receive trips from loadly very soon, maybe checking your paper takes 2 days';

  @override
  String get goToHomepage => 'Go to homepage';

  @override
  String get market => 'Market';

  @override
  String get marketScreenTitle => 'The market';

  @override
  String get marketScreenHeaderDesc =>
      'Shop spare parts, tires, and equipment for your fleet.';

  @override
  String get marketComingSoonTitle => 'Coming soon';

  @override
  String get marketComingSoonBody => 'A marketplace will soon be launched.';

  @override
  String get marketHomePageButton => 'Home page';

  @override
  String get account => 'Account';

  @override
  String get enterPasswordEnableBiometric =>
      'Enter your password to enable biometric login.';

  @override
  String get biometric => 'Biometric';

  @override
  String get enableBiometric =>
      'Would you like to enable biometric login for faster access?';

  @override
  String get biometricDesc => 'Enable to finger print and Pin to open the app.';

  @override
  String get languages => 'Languages';

  @override
  String get helpAndCenter => 'Help and center';

  @override
  String get helpAndCenterDesc =>
      'Chat or call with our support team for assistance.';

  @override
  String get logoutDesc =>
      'Please enable the biometric or your password to make it easy for login again.';

  @override
  String get edit => 'Edit';

  @override
  String get saveChanges => 'Save Changes';

  @override
  String get back => 'Back';

  @override
  String get totalTripsCount => 'Total trips count';

  @override
  String get unpaidAmount => 'Unpaid amount';

  @override
  String get personalInformation => 'Personal information';

  @override
  String get shipping => 'Shipping';

  @override
  String get completedTripsDue => 'Completed trips due';

  @override
  String get completedTripsDueUntilNow => 'Completed trips due until now';

  @override
  String get tripsAndFinancialDue => 'Trips and financial due';

  @override
  String get contactSupport => 'Contact support';

  @override
  String get trackTheTrip => 'Track the trip';

  @override
  String get trackTheTripDesc =>
      'Enter your trip ID and get the realtime tracking.';

  @override
  String get enterTripID => 'Enter trip ID';

  @override
  String get tripRequest => 'Trip request';

  @override
  String get tripsRequest => 'Trips request';

  @override
  String get tripsRequestDesc => 'All trips request you have.';

  @override
  String get tripsReview => 'Trips review';

  @override
  String get myTripsReview => 'Trips review';

  @override
  String get completedTrips => 'Completed trips';

  @override
  String get showAll => 'Show All';

  @override
  String loadlyAdminSendThisTripFromAgo(String time) {
    return 'A Shail admin sent this trip $time ago.';
  }

  @override
  String get tripDate => 'Trip date';

  @override
  String get tripFrom => 'Trip from';

  @override
  String get tripTo => 'Trip to';

  @override
  String get vehicleType => 'Vehicle type';

  @override
  String get tripCost => 'Trip cost';

  @override
  String get tripValue => 'Value';

  @override
  String get waitingForSupervisorApproval => 'Waiting for supervisor approval';

  @override
  String get editRequest => 'Edit request';

  @override
  String get cannotEditThisTrip => 'This trip cannot be edited';

  @override
  String get confirmCancelTrip => 'Are you sure you want to cancel this trip?';

  @override
  String get editRequestUpdatedTitle => 'Request updated';

  @override
  String get editRequestUpdatedMessage =>
      'Your booking request has been updated successfully';

  @override
  String get cancelTrip => 'Cancel trip';

  @override
  String get bookTrip => 'Book trip';

  @override
  String get requestTripBooking => 'Request trip booking';

  @override
  String get selectDriver => 'Select driver';

  @override
  String get searchForDriver => 'Search for a driver';

  @override
  String get noDrivers => 'No drivers';

  @override
  String get chooseThisDriver => 'Choose this driver';

  @override
  String get chooseDriverPlaceholder => 'Choose driver';

  @override
  String get driversFromYourList => 'Drivers from your list';

  @override
  String get tapToAddNewDriver => 'Tap here to add a new driver';

  @override
  String get orAddNewDriver => 'Or add a new driver';

  @override
  String get addNewDriver => 'Add new driver';

  @override
  String get addDriver => 'Add driver';

  @override
  String get driverInformation => 'Driver information';

  @override
  String get driverDocumentsHint => 'ID, driving license, and vehicle license';

  @override
  String get pleaseUploadRequiredDocuments =>
      'Please upload the required documents';

  @override
  String get invalidNationalId => 'Invalid national ID';

  @override
  String get pickupPoint => 'Pickup point';

  @override
  String get dropoffPoint => 'Drop-off point';

  @override
  String get tripBookedTitle => 'This trip has been booked for you.';

  @override
  String get tripBookedUpdateTitle => 'Your booking request has been updated';

  @override
  String get tripBookedMessage =>
      'Wait for the representative to approve this trip for you.';

  @override
  String get pleaseSelectDriver => 'Please select a driver first';

  @override
  String get favoriteList => 'Favorite list';

  @override
  String get preview => 'Preview';

  @override
  String newTripRequestsCount(String count) {
    return 'New trip requests ($count)';
  }

  @override
  String get allYourTripRequests => 'All your trip requests';

  @override
  String get allYourBookedTripRequests =>
      'All transfer requests you applied for are waiting for supervisor approval';

  @override
  String get notStarted => 'Not started';

  @override
  String get finishedTransfers => 'Finished trips';

  @override
  String get cancellations => 'Cancelled';

  @override
  String get requestedTransfers => 'Requested transfers';

  @override
  String get homeNewTripBadge => 'NEW';

  @override
  String get bookedTransfers => 'Booked transfers';

  @override
  String requestedTransfersCount(String count) {
    return 'Requested transfers ($count)';
  }

  @override
  String bookedTransfersCount(String count) {
    return 'Booked transfers ($count)';
  }

  @override
  String priceInEgp(String amount) {
    return '$amount EGP';
  }

  @override
  String get all => 'All';

  @override
  String get requested => 'Requested';

  @override
  String get pending => 'Pending';

  @override
  String get scheduled => 'Scheduled';

  @override
  String get inProgress => 'In progress';

  @override
  String get completed => 'Completed';

  @override
  String get tripCompleted => 'Completed';

  @override
  String get tripWasCancelled => 'Cancelled';

  @override
  String get delivered => 'Delivered';

  @override
  String get loadingCargo => 'Loading';

  @override
  String get reviewed => 'Reviewed';

  @override
  String get cancelled => 'Cancelled';

  @override
  String get draft => 'Draft';

  @override
  String get tripInProgress => 'Trip in progress';

  @override
  String get newRequests => 'New requests';

  @override
  String get alreadyBooked => 'Already booked';

  @override
  String get searchForTrip => 'Search for trip';

  @override
  String get filter => 'Filter';

  @override
  String get applyFilter => 'Apply filter';

  @override
  String get clearFilter => 'Clear filter';

  @override
  String get searchForPlace => 'Search for place';

  @override
  String get searchForPlaceDesc =>
      'Search for a city or region with available trips that you can access.';

  @override
  String get movingFrom => 'Moving from';

  @override
  String get confirm => 'Confirm';

  @override
  String get skip => 'Skip';

  @override
  String get pleaseConfirmYourPassword => 'Please confirm your password';

  @override
  String get passwordChangedSuccessfully => 'Password changed successfully';

  @override
  String tripDetailsNumber(String id) {
    return 'Trip Details #$id';
  }

  @override
  String get notDue => 'Not due';

  @override
  String get financialDetails => 'Financial details';

  @override
  String get driverDetails => 'Driver details';

  @override
  String get tripDispatchTime => 'Trip dispatch time';

  @override
  String get tripAcceptanceTime => 'Trip acceptance time';

  @override
  String get elapsedTripDuration => 'Elapsed trip duration';

  @override
  String get distanceInKm => 'Distance (km)';

  @override
  String kmCount(String count) {
    return '$count km';
  }

  @override
  String get notes => 'Notes';

  @override
  String get noNotes => 'None';

  @override
  String get otherDiscounts => 'Other discounts';

  @override
  String hoursCount(String count, String unit) {
    return '$count $unit';
  }

  @override
  String get changeDriver => 'Change driver';

  @override
  String get chooseNewDriver => 'Choose new driver';

  @override
  String get cancelBooking => 'Cancel booking';

  @override
  String get confirmChanges => 'Confirm changes';

  @override
  String get assignTheDriver => 'Assign the driver';

  @override
  String get chooseDriver => 'Choose driver';

  @override
  String get yourSupervisorHasApprovedTheTripPleaseAddDriver =>
      'Your supervisor has approved the trip, please add a driver.';

  @override
  String get ifDriverIsNotAppointedWithinTheTripWillBeSentToAnotherContractor =>
      'If a driver is not appointed within ??, the trip will be sent to another contractor.';

  @override
  String get readyForTrip => 'Ready for trip';

  @override
  String get youHaveSuccessfullyAddedTheDriver =>
      'You have successfully added the driver.';

  @override
  String youHaveSuccessfullyAddedTheDriverDesc(String date) {
    return 'The trip will begin in $date, so be prepared. If you have any questions, please don\'t hesitate to call the person in charge.';
  }

  @override
  String get waitingForManagerApproval => 'Waiting for manager approval';

  @override
  String get today => 'Today';

  @override
  String get chooseVehicleType => 'Choose vehicle type';

  @override
  String get tripStartDate => 'Trip start date';

  @override
  String get tripEndDate => 'Trip end date';

  @override
  String get tripStatus => 'Trip status';

  @override
  String get justNow => 'Just now';

  @override
  String get timeAm => 'AM';

  @override
  String get timePm => 'PM';

  @override
  String get meridiemAm => 'AM';

  @override
  String get meridiemPm => 'PM';

  @override
  String get lessThanAMinute => 'less than a minute';

  @override
  String get elapsedMinuteUnit => 'minute';

  @override
  String get elapsedMinutesUnit => 'minutes';

  @override
  String get elapsedHourUnit => 'hour';

  @override
  String get elapsedHoursUnit => 'hours';

  @override
  String get elapsedDayUnit => 'day';

  @override
  String get elapsedDaysUnit => 'days';

  @override
  String get elapsedTwoMinutes => '2 minutes';

  @override
  String get elapsedTwoHours => '2 hours';

  @override
  String get elapsedTwoDays => '2 days';

  @override
  String get elapsedMinuteSingular => 'minute';

  @override
  String elapsedMinutesFew(int count) {
    return '$count minutes';
  }

  @override
  String elapsedMinutesOther(int count) {
    return '$count minutes';
  }

  @override
  String get elapsedHourSingular => 'hour';

  @override
  String elapsedHoursFew(int count) {
    return '$count hours';
  }

  @override
  String elapsedHoursOther(int count) {
    return '$count hours';
  }

  @override
  String get elapsedDaySingular => 'day';

  @override
  String elapsedDaysFew(int count) {
    return '$count days';
  }

  @override
  String elapsedDaysOther(int count) {
    return '$count days';
  }

  @override
  String elapsedMinutesShort(int count) {
    return '$count min';
  }

  @override
  String priceInArabicEgpFull(String amount) {
    return '$amount EGP';
  }

  @override
  String get routeError => 'Route Error';

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
    return 'Total trips : $count';
  }

  @override
  String get dueTripsCountLabel => 'Due trips count';

  @override
  String get amountDueUntilNow => 'Amount due until now';

  @override
  String get financialDueAmountDescription =>
      'The due amount is what you receive from Loadly after submitting the trip documents.';

  @override
  String get allTrips => 'All trips';

  @override
  String get dueTransferTripBadge => 'Due trip';

  @override
  String get entitlementFulfilledBadge => 'Entitlement fulfilled';

  @override
  String get transferTripPricePrefix => 'Trip price :';
}
