// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appTitle => 'Loadly Workplace';

  @override
  String get welcome => 'Welcome';

  @override
  String get verificationInstruction =>
      'Enter your phone number and we will send you a verification code in less than a minute.';

  @override
  String get phone => 'Phone number';

  @override
  String get enterYourPhone => 'Enter your phone number';

  @override
  String get enterValidPhone => 'Enter a valid phone number';

  @override
  String get enterValidPhone11Digits => 'Enter 11 digits';

  @override
  String get enterValidPhone01 => 'Phone number must start with 01';

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
  String get tripDetails => 'Trip Details';

  @override
  String get tripId => 'trip ID';

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
  String get completed => 'Completed';

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
  String get applyFilter => 'Apply';

  @override
  String get clearFilter => 'Clear';

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
      'How loadly Workplace  uses your location';

  @override
  String get locationPermissionDisclosureBody =>
      'Loadly Workplace collects your device location—including in the background when the app is not on screen—while you are online or working on trips. We use it to show your position on the map, switch online/offline accurately, plan routes for pickups and deliveries, and share live location with your company. Location is sent only to Loadly servers as needed for these driver features.\n\nYou will first be asked to allow location while using the app. If full live tracking is needed, a second screen will explain background access, followed by another system permission step (for example “Allow all the time”). Tap Continue to start.';

  @override
  String get locationBackgroundPermissionDisclosureTitle =>
      'Location in the background';

  @override
  String get locationBackgroundPermissionDisclosureBody =>
      'So your company can receive reliable live updates while you drive, Loadly Workplace needs permission to access location in the background (when the app is behind other apps or the screen is off). This is only used for the driver features described on the previous screen—not for advertising.\n\nTap Continue to open the next system permission. On some devices you must choose “Allow all the time” or enable background location in Settings.';

  @override
  String get notificationPermissionDisclosureTitle => 'Notifications';

  @override
  String get notificationPermissionDisclosureBody =>
      'Loadly Workplace sends notifications about trips, schedules, and important updates. To deliver them, the app uses push notifications and a device token stored on our servers. Tap Continue to open the system permission request.';

  @override
  String get cameraPermissionDisclosureTitle => 'Camera access';

  @override
  String get cameraPermissionDisclosureBody =>
      'Loadly Workplace uses the camera only when you choose to take a photo—for example for your profile picture or required documents. Photos you take are uploaded to our servers for your account. Tap Continue to open the system permission request.';

  @override
  String get photosPermissionDisclosureTitle => 'Photos and files';

  @override
  String get photosPermissionDisclosureBody =>
      'Loadly Workplace accesses images you select from your photo library or device storage to upload profile or document images. We only access files you choose. Tap Continue to open the system permission request.';

  @override
  String get appUpdateRequiredTitle => 'Update required';

  @override
  String get appUpdateAvailableTitle => 'Update available';

  @override
  String get appUpdateRequiredMessage =>
      'A new version of Loadly Workplace is required to continue. Please update to the latest version.';

  @override
  String get appUpdateAvailableMessage =>
      'A new version of Loadly Workplace is available with improvements and fixes.';

  @override
  String appUpdateLatestVersion(String version) {
    return 'Latest version: $version';
  }

  @override
  String get appUpdateNow => 'Update now';

  @override
  String get appUpdateLater => 'Later';

  @override
  String get filter => 'Filter';

  @override
  String get tripDateIs => 'Trip date is';

  @override
  String get collectDocuments => 'Collect documents';

  @override
  String get partiallyCollectedDocuments => 'Partially collected documents';

  @override
  String get noDocumentsAvailable => 'No documents available';

  @override
  String get received => 'Received';

  @override
  String get notReceived => 'Not received';

  @override
  String get stillWithCustomerWaitingForSignature =>
      'Still With Customer Waiting For Signature';

  @override
  String get stillWithCustomer => 'Still With Customer';

  @override
  String get lostByDriver => 'Lost By Driver';

  @override
  String get lostDocument => 'Lost document';

  @override
  String get tripDoesNotExist => 'Trip Does Not Exist';

  @override
  String get willBeSentLater => 'Will Be Sent Later';

  @override
  String get customerRefusedToSign => 'Customer Refused To Sign';

  @override
  String get other => 'Other';

  @override
  String get note => 'Note';

  @override
  String get confirm => 'Confirm';

  @override
  String get deliveryPermission => 'Delivery permission';

  @override
  String get selectTheReasonsForNotReceivingTheDocument =>
      'Select the reasons for not receiving the document';

  @override
  String get enterNote => 'In case of anthor reason please enter a note';

  @override
  String get takeAPhotoOfTheDocument =>
      'Take a photo of the document (optional)';

  @override
  String get openCameraDocument => 'photograph document';

  @override
  String get saveAndContinueLater => 'Save and continue later';

  @override
  String get saveAndSendDocuments => 'Save & Send documents';

  @override
  String get confirmAndContinueLater => 'Confirm and continue later';

  @override
  String get confirmAndSendDocuments => 'Confirm & Send documents';

  @override
  String get confirmAllDocuments => 'Confirm all documents';

  @override
  String get pleaseConfirmAllDocuments => 'Please confirm all documents';

  @override
  String get saveAndSendDocumentsDesc =>
      'Click send, and the documents will be saved and sent to LOADLY within seconds.';

  @override
  String get saved => 'Saved';

  @override
  String get savedDesc =>
      'The process has been saved. Please complete it later to collect the documents and send them to the company as soon as possible.';

  @override
  String get theDocumentsThatWillBeSent => 'The documents that will be sent';

  @override
  String get sendDocuments => 'Send documents';

  @override
  String get pullToRefresh => 'Pull to Refresh';

  @override
  String get releaseToRefresh => 'Release to Refresh';

  @override
  String get searchForContractId => 'Search for contract ID';

  @override
  String get searchForCompanyId => 'Search for company ID';

  @override
  String get searchForVehicleId => 'Search for vehicle ID';

  @override
  String get searchForVehicleTypeId => 'Search for vehicle Type ID';

  @override
  String get searchForDriverId => 'Search for driver ID';

  @override
  String get searchByDate => 'Search by date';

  @override
  String get today => 'Today';

  @override
  String get yesterday => 'Yesterday';

  @override
  String get lastWeek => 'Last 7 days';

  @override
  String get chooseDate => 'Choose date';

  @override
  String get tripStatus => 'Trip status';

  @override
  String get noDocumentsCollected => 'No documents collected';

  @override
  String get partiallyConfirmedDocuments => 'Partially confirmed documents';

  @override
  String get resetFilter => 'Reset filter';

  @override
  String get startDate => 'Start date';

  @override
  String get endDate => 'End date';

  @override
  String get done => 'Done';

  @override
  String get reset => 'Reset';

  @override
  String get companyName => 'Company name';

  @override
  String get driverName => 'driver name';

  @override
  String get carNumber => 'car number';

  @override
  String get documentsPictures => 'Documents pictures';

  @override
  String get receiveDocumentsForSelectedTrips =>
      'Receive documents for selected trips';

  @override
  String get documentsAreBeingCollected => 'Documents are being collected';

  @override
  String get documentsAreBeingCollectedDesc =>
      'Please wait a few moments until all trips documents are collected at once.';

  @override
  String get selectedTripsDocuments => 'Selected trips documents';

  @override
  String get receiptConfirmed => 'Receipt confirmed';

  @override
  String get confirmed => 'Confirmed';

  @override
  String get receiptConfirmedDesc =>
      'Click Confirm, and the documents will be confirmed and sent to LOADLY within seconds.';

  @override
  String get selectedTrips => 'Selected trips';

  @override
  String get receiptConfirmation => 'Receipt confirmation';

  @override
  String get receiptConfirmationWithMark => 'Receipt confirmation?';

  @override
  String get documentsCollectedByFactoryCollector =>
      'Documents collected by factory collector';

  @override
  String get documentsCollectedFromFactoryCollector =>
      'Documents collected from factory collector';

  @override
  String get receiptConfirmationForSelectedTrips =>
      'Receipt confirmation for selected trips';

  @override
  String get confirmWithMark => 'Confirm?';

  @override
  String get incorrectPassword => 'Incorrect password, Please try again';

  @override
  String get withCollector => 'Collected by factory collector';

  @override
  String get externalTrip => 'External trip';

  @override
  String get username => 'Username';

  @override
  String get enterUsername => 'Enter your username';

  @override
  String get enterValidUsername =>
      'Enter a valid userbame must be more than 3 characters';

  @override
  String get displayDocuments => 'Display documents';

  @override
  String get noDriver => 'No driver';

  @override
  String get edit => 'Edit';

  @override
  String get factoryCollector => 'Factory collector';

  @override
  String get search => 'Search';

  @override
  String get unexpectedError =>
      'An unexpected error occurred. Please try again.';

  @override
  String get networkErrorPleaseRetry =>
      'A network error occurred. Please check your connection and try again.';

  @override
  String get noInternetConnection =>
      'No internet connection. Please check your connection and try again.';

  @override
  String get requestFailedTryAgain =>
      'The request could not be completed. Please try again.';

  @override
  String get failedToLoadTrips => 'Could not load trips. Please try again.';

  @override
  String get failedToLoadDocuments =>
      'Could not load documents. Please try again.';

  @override
  String get failedToCollectDocuments =>
      'Could not receive the documents. Please try again.';

  @override
  String get failedToConfirmDocuments =>
      'Could not confirm the documents. Please try again.';

  @override
  String get invalidLoginCredentials =>
      'The username or password is incorrect.';

  @override
  String get invalidPhoneNumber => 'The phone number is incorrect.';

  @override
  String get invalidVerificationCode =>
      'The verification code is incorrect or has expired.';

  @override
  String get sessionExpired =>
      'Your session has expired. Please sign in again.';

  @override
  String get invalidRequest =>
      'The request is invalid. Please review the information you entered.';

  @override
  String get resourceNotFound => 'The requested item was not found.';

  @override
  String get actionNotAllowed => 'This action is not allowed.';

  @override
  String get invalidSubmittedData => 'The information you entered is invalid.';

  @override
  String get serverErrorTryLater =>
      'A server error occurred. Please try again later.';

  @override
  String get failedToLoadProfile =>
      'Could not load your profile. Please try again.';

  @override
  String get somethingWentWrongTryAgain =>
      'Something went wrong. Please try again.';

  @override
  String get emptyServerResponse =>
      'Could not read the server response. Please try again.';

  @override
  String get missingServerData =>
      'The required information is unavailable. Please try again.';

  @override
  String get passwordResetFailed =>
      'Could not reset the password. Please try again.';

  @override
  String get couldNotSendResetCode =>
      'Could not send the password reset code. Please try again.';

  @override
  String get tooManyAttempts => 'Too many attempts. Please wait and try again.';

  @override
  String get notificationChannelDescription => 'General app notifications';

  @override
  String get notificationFallbackTitle => 'Notification';
}
