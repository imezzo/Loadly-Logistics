import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_ar.dart';
import 'app_localizations_en.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'gen/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
    : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations? of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations);
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
        delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
      ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('ar'),
    Locale('en'),
  ];

  /// Application title shown in app bar
  ///
  /// In en, this message translates to:
  /// **'Loadly Workplace'**
  String get appTitle;

  /// Welcome greeting on login screen
  ///
  /// In en, this message translates to:
  /// **'Welcome'**
  String get welcome;

  /// Instruction for phone verification
  ///
  /// In en, this message translates to:
  /// **'Enter your phone number and we will send you a verification code in less than a minute.'**
  String get verificationInstruction;

  /// Phone number field label
  ///
  /// In en, this message translates to:
  /// **'Phone number'**
  String get phone;

  /// Phone input placeholder
  ///
  /// In en, this message translates to:
  /// **'Enter your phone number'**
  String get enterYourPhone;

  /// Phone validation error
  ///
  /// In en, this message translates to:
  /// **'Enter a valid phone number'**
  String get enterValidPhone;

  /// Phone length validation error
  ///
  /// In en, this message translates to:
  /// **'Enter 11 digits'**
  String get enterValidPhone11Digits;

  /// Phone format validation error
  ///
  /// In en, this message translates to:
  /// **'Phone number must start with 01'**
  String get enterValidPhone01;

  /// Sign in screen title
  ///
  /// In en, this message translates to:
  /// **'Sign in to your account'**
  String get signInToAccount;

  /// Email field label
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get email;

  /// Password field label
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get password;

  /// Sign in button label
  ///
  /// In en, this message translates to:
  /// **'Sign in'**
  String get signIn;

  /// Error dialog title
  ///
  /// In en, this message translates to:
  /// **'Error'**
  String get error;

  /// OK button label
  ///
  /// In en, this message translates to:
  /// **'OK'**
  String get ok;

  /// Email input placeholder
  ///
  /// In en, this message translates to:
  /// **'Enter your email'**
  String get enterYourEmail;

  /// Email validation error
  ///
  /// In en, this message translates to:
  /// **'Enter a valid email'**
  String get enterValidEmail;

  /// Password input placeholder
  ///
  /// In en, this message translates to:
  /// **'Enter your password'**
  String get enterYourPassword;

  /// Home tab label
  ///
  /// In en, this message translates to:
  /// **'Home'**
  String get home;

  /// Forgot password link
  ///
  /// In en, this message translates to:
  /// **'Forgot your password?'**
  String get forgetPassword;

  /// Verification screen title
  ///
  /// In en, this message translates to:
  /// **'Verification Code'**
  String get verificationTitle;

  /// Verification method selection prompt
  ///
  /// In en, this message translates to:
  /// **'We will send you a verification code, you can choose to send it to your email or as a text message to your mobile?'**
  String get verificationSubtitle;

  /// SMS verification description
  ///
  /// In en, this message translates to:
  /// **'We will send you a verification code via text message to your phone.'**
  String get verificationSubtitlePhone;

  /// SMS verification option
  ///
  /// In en, this message translates to:
  /// **'Send code via text message'**
  String get sendViaSms;

  /// Email verification option
  ///
  /// In en, this message translates to:
  /// **'Send code via email'**
  String get sendViaEmail;

  /// Continue/Log in button
  ///
  /// In en, this message translates to:
  /// **'Log In'**
  String get continueButton;

  /// OTP instruction when code was sent via SMS
  ///
  /// In en, this message translates to:
  /// **'We sent you a verification code via text message to your number ({phone}), enter it here to complete your account activation.'**
  String otpInstructionSms(String phone);

  /// OTP instruction when code was sent via email
  ///
  /// In en, this message translates to:
  /// **'We sent you a verification code via email to {email}, enter it here to complete your account activation.'**
  String otpInstructionEmail(String email);

  /// Resend code prompt
  ///
  /// In en, this message translates to:
  /// **'Didn\'t receive a message yet?'**
  String get resendPrompt;

  /// Resend code link
  ///
  /// In en, this message translates to:
  /// **'Send again'**
  String get resendLink;

  /// Verify account button
  ///
  /// In en, this message translates to:
  /// **'Verify Account'**
  String get verifyAccount;

  /// Verify email screen title
  ///
  /// In en, this message translates to:
  /// **'Verify Email'**
  String get verifyEmail;

  /// Email verification instruction
  ///
  /// In en, this message translates to:
  /// **'Enter the 6-digit code we sent to your email to verify your account.'**
  String get verifyEmailInstruction;

  /// Verification code input placeholder
  ///
  /// In en, this message translates to:
  /// **'Enter the verification code'**
  String get enterVerificationCode;

  /// Verification code length validation
  ///
  /// In en, this message translates to:
  /// **'Code must be 6 digits'**
  String get verificationCodeLength;

  /// Verify button
  ///
  /// In en, this message translates to:
  /// **'Verify'**
  String get verify;

  /// Resend code button
  ///
  /// In en, this message translates to:
  /// **'Resend code'**
  String get resendCode;

  /// Code sent confirmation message
  ///
  /// In en, this message translates to:
  /// **'Verification code sent to your email.'**
  String get codeSentToEmail;

  /// New password field label
  ///
  /// In en, this message translates to:
  /// **'New password'**
  String get newPassword;

  /// Confirm password field label
  ///
  /// In en, this message translates to:
  /// **'Confirm password'**
  String get confirmPassword;

  /// Reset password button
  ///
  /// In en, this message translates to:
  /// **'Reset password'**
  String get resetPassword;

  /// Password length validation
  ///
  /// In en, this message translates to:
  /// **'Password must be at least 6 characters'**
  String get passwordMinLength;

  /// Personal account section title
  ///
  /// In en, this message translates to:
  /// **'Personal Account'**
  String get personalAccount;

  /// Employee or driver name label
  ///
  /// In en, this message translates to:
  /// **'Employee/Driver Name'**
  String get employeeDriverName;

  /// Email label in profile
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get emailLabel;

  /// Driver rating label
  ///
  /// In en, this message translates to:
  /// **'Rating'**
  String get rating;

  /// Single trip label
  ///
  /// In en, this message translates to:
  /// **'Trip'**
  String get trip;

  /// Trips list label
  ///
  /// In en, this message translates to:
  /// **'Trips'**
  String get trips;

  /// Files tab label
  ///
  /// In en, this message translates to:
  /// **'Files'**
  String get files;

  /// Personal data section
  ///
  /// In en, this message translates to:
  /// **'My Personal Data'**
  String get myPersonalData;

  /// File name field label
  ///
  /// In en, this message translates to:
  /// **'File Name'**
  String get fileName;

  /// Gallery picker option
  ///
  /// In en, this message translates to:
  /// **'Choose from Gallery'**
  String get chooseFromGallery;

  /// Profile page title
  ///
  /// In en, this message translates to:
  /// **'Profile Page'**
  String get profilePage;

  /// Notifications tab label
  ///
  /// In en, this message translates to:
  /// **'Notifications'**
  String get notifications;

  /// Trips navigation label
  ///
  /// In en, this message translates to:
  /// **'Trips'**
  String get tripsNav;

  /// Home navigation label
  ///
  /// In en, this message translates to:
  /// **'Home'**
  String get homeNav;

  /// Countdown seconds remaining
  ///
  /// In en, this message translates to:
  /// **'{seconds} seconds remaining'**
  String secondsRemaining(int seconds);

  /// Password login option
  ///
  /// In en, this message translates to:
  /// **'Login with password'**
  String get loginWithPassword;

  /// Biometric login option
  ///
  /// In en, this message translates to:
  /// **'Login with Biometric'**
  String get loginWithBiometric;

  /// Divider between options
  ///
  /// In en, this message translates to:
  /// **'OR'**
  String get or;

  /// Back to login link
  ///
  /// In en, this message translates to:
  /// **'Back to login'**
  String get backToLogin;

  /// Remember me checkbox
  ///
  /// In en, this message translates to:
  /// **'Remember me'**
  String get rememberMe;

  /// Online status
  ///
  /// In en, this message translates to:
  /// **'Online'**
  String get online;

  /// Offline status
  ///
  /// In en, this message translates to:
  /// **'Offline'**
  String get offline;

  /// Generic active status
  ///
  /// In en, this message translates to:
  /// **'Active'**
  String get active;

  /// Generic inactive status
  ///
  /// In en, this message translates to:
  /// **'Inactive'**
  String get inactive;

  /// Logout button
  ///
  /// In en, this message translates to:
  /// **'Logout'**
  String get logout;

  /// Change password option
  ///
  /// In en, this message translates to:
  /// **'Change Password'**
  String get changePassword;

  /// Enable fingerprint option
  ///
  /// In en, this message translates to:
  /// **'Enable Fingerprint'**
  String get enableFingerprint;

  /// Error when fingerprint login fails or user cancels
  ///
  /// In en, this message translates to:
  /// **'Biometric authentication failed or cancelled.'**
  String get biometricErrorFailedOrCancelled;

  /// Error when enabling fingerprint login fails
  ///
  /// In en, this message translates to:
  /// **'Failed to enable biometric login.'**
  String get biometricErrorEnableFailed;

  /// Error when disabling fingerprint login fails
  ///
  /// In en, this message translates to:
  /// **'Failed to disable biometric login.'**
  String get biometricErrorDisableFailed;

  /// System biometric prompt when logging in with fingerprint/Face ID
  ///
  /// In en, this message translates to:
  /// **'Authenticate to sign in'**
  String get biometricLocalizedReasonSignIn;

  /// System biometric prompt when enabling fingerprint login
  ///
  /// In en, this message translates to:
  /// **'Authenticate to enable fingerprint login'**
  String get biometricLocalizedReasonEnable;

  /// Body text in password dialog before enabling fingerprint
  ///
  /// In en, this message translates to:
  /// **'Enter your password to enable fingerprint login.'**
  String get biometricEnablePasswordDialogMessage;

  /// Prompt after password login to offer enabling fingerprint
  ///
  /// In en, this message translates to:
  /// **'Would you like to enable fingerprint login for faster access?'**
  String get biometricOfferAfterLoginMessage;

  /// Error when profile login is missing for fingerprint enable
  ///
  /// In en, this message translates to:
  /// **'Could not get your account info. Please try again.'**
  String get biometricAccountInfoUnavailable;

  /// Enable two-factor authentication
  ///
  /// In en, this message translates to:
  /// **'Enable 2FA'**
  String get enable2FA;

  /// Support section
  ///
  /// In en, this message translates to:
  /// **'Support'**
  String get support;

  /// Settings title
  ///
  /// In en, this message translates to:
  /// **'Settings'**
  String get settings;

  /// Cancel button
  ///
  /// In en, this message translates to:
  /// **'Cancel'**
  String get cancel;

  /// Password mismatch validation
  ///
  /// In en, this message translates to:
  /// **'Passwords do not match'**
  String get passwordsDoNotMatch;

  /// Full name in English field
  ///
  /// In en, this message translates to:
  /// **'Full Name (English)'**
  String get fullNameEnglish;

  /// Full name in Arabic field
  ///
  /// In en, this message translates to:
  /// **'Full Name (Arabic)'**
  String get fullNameArabic;

  /// Username field label
  ///
  /// In en, this message translates to:
  /// **'User Name'**
  String get userName;

  /// Save button
  ///
  /// In en, this message translates to:
  /// **'Save'**
  String get save;

  /// Edit profile button to unlock personal data for editing
  ///
  /// In en, this message translates to:
  /// **'Edit Profile'**
  String get editProfile;

  /// Profile update success message
  ///
  /// In en, this message translates to:
  /// **'Profile updated successfully'**
  String get profileUpdatedSuccessfully;

  /// Image picker not available message
  ///
  /// In en, this message translates to:
  /// **'Image picker not implemented yet'**
  String get imagePickerNotImplemented;

  /// Retry button
  ///
  /// In en, this message translates to:
  /// **'Retry'**
  String get retry;

  /// Generic user label
  ///
  /// In en, this message translates to:
  /// **'User'**
  String get user;

  /// Camera permission request message
  ///
  /// In en, this message translates to:
  /// **'Camera permission is required to take photos. Please enable it in Settings.'**
  String get cameraPermissionRequired;

  /// Photo library permission request message
  ///
  /// In en, this message translates to:
  /// **'Photo library permission is required to select photos. Please enable it in Settings.'**
  String get photoLibraryPermissionRequired;

  /// Permission denied dialog title
  ///
  /// In en, this message translates to:
  /// **'Permission Denied'**
  String get permissionDenied;

  /// Open settings button
  ///
  /// In en, this message translates to:
  /// **'Open Settings'**
  String get openSettings;

  /// Take photo option
  ///
  /// In en, this message translates to:
  /// **'Take Photo'**
  String get takePhoto;

  /// Today notifications section
  ///
  /// In en, this message translates to:
  /// **'Today\'s Notifications'**
  String get todayNotifications;

  /// Company name label
  ///
  /// In en, this message translates to:
  /// **'Company'**
  String get company;

  /// Company notification label
  ///
  /// In en, this message translates to:
  /// **'Company Notification'**
  String get companyNotification;

  /// New trip notification message
  ///
  /// In en, this message translates to:
  /// **'A new trip has been sent, click to view details'**
  String get newTripSent;

  /// Retrieval notification message
  ///
  /// In en, this message translates to:
  /// **'A retrieval notification has been sent to the company awaiting your reply'**
  String get retrievalNotificationSent;

  /// Replacement car notification
  ///
  /// In en, this message translates to:
  /// **'A replacement car is on its way to you'**
  String get replacementCarOnWay;

  /// Vehicle details section
  ///
  /// In en, this message translates to:
  /// **'Vehicle Details'**
  String get vehicleDetails;

  /// Time ago in minutes
  ///
  /// In en, this message translates to:
  /// **'{mins} mins ago'**
  String minsAgo(int mins);

  /// Trip details screen title
  ///
  /// In en, this message translates to:
  /// **'Trip Details'**
  String get tripDetails;

  /// Trip ID label
  ///
  /// In en, this message translates to:
  /// **'trip ID'**
  String get tripId;

  /// Status label
  ///
  /// In en, this message translates to:
  /// **'Status'**
  String get status;

  /// Trip status: requested
  ///
  /// In en, this message translates to:
  /// **'Requested'**
  String get statusRequested;

  /// Trip status: pending
  ///
  /// In en, this message translates to:
  /// **'Pending'**
  String get statusPending;

  /// Trip status: scheduled
  ///
  /// In en, this message translates to:
  /// **'Scheduled'**
  String get statusScheduled;

  /// Trip status: driver accepted
  ///
  /// In en, this message translates to:
  /// **'Driver Accepted'**
  String get statusDriverAccepted;

  /// Trip status: first pickup loading
  ///
  /// In en, this message translates to:
  /// **'Loading'**
  String get statusFirstPickupLoading;

  /// Trip status: in progress
  ///
  /// In en, this message translates to:
  /// **'In Progress'**
  String get statusInProgress;

  /// Trip status: completed
  ///
  /// In en, this message translates to:
  /// **'Completed'**
  String get statusCompleted;

  /// Trip status: reviewed
  ///
  /// In en, this message translates to:
  /// **'Reviewed'**
  String get statusReviewed;

  /// Trip status: cancelled
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get statusCancelled;

  /// Trip status: unknown
  ///
  /// In en, this message translates to:
  /// **'—'**
  String get statusUnknown;

  /// Start trip button
  ///
  /// In en, this message translates to:
  /// **'Start Trip'**
  String get startTrip;

  /// Start shipping button
  ///
  /// In en, this message translates to:
  /// **'Start Shipping and Loading'**
  String get startShippingAndLoading;

  /// Trip start date and time
  ///
  /// In en, this message translates to:
  /// **'Trip starts on: {date} at {time}'**
  String tripStartsOn(String date, String time);

  /// Start loading shipment label
  ///
  /// In en, this message translates to:
  /// **'Start loading shipment'**
  String get startLoadingShipment;

  /// Pickup label
  ///
  /// In en, this message translates to:
  /// **'Pickup'**
  String get pickupLocation;

  /// Label for the first pickup when driver has departed from it
  ///
  /// In en, this message translates to:
  /// **'Start Trip'**
  String get startTripLocation;

  /// Drop off label
  ///
  /// In en, this message translates to:
  /// **'Drop Off'**
  String get deliveryLocation;

  /// Weight label
  ///
  /// In en, this message translates to:
  /// **'Weight'**
  String get weight;

  /// Shipment type label
  ///
  /// In en, this message translates to:
  /// **'Shipment Type'**
  String get shipmentType;

  /// Trip duration label
  ///
  /// In en, this message translates to:
  /// **'Trip Duration'**
  String get tripDuration;

  /// Shipment dimensions label
  ///
  /// In en, this message translates to:
  /// **'Shipment Dimensions'**
  String get shipmentDimensions;

  /// Approximate indicator
  ///
  /// In en, this message translates to:
  /// **'Approximately'**
  String get approximately;

  /// Hours unit
  ///
  /// In en, this message translates to:
  /// **'hours'**
  String get hours;

  /// Stop points section
  ///
  /// In en, this message translates to:
  /// **'Stop Points'**
  String get stopPoints;

  /// Trip points section
  ///
  /// In en, this message translates to:
  /// **'Trip Points'**
  String get tripPoints;

  /// Stop label
  ///
  /// In en, this message translates to:
  /// **'Stop'**
  String get stop;

  /// Client details section
  ///
  /// In en, this message translates to:
  /// **'Client Details'**
  String get clientDetails;

  /// Phone number label
  ///
  /// In en, this message translates to:
  /// **'Phone Number'**
  String get phoneNumber;

  /// Responsible person label
  ///
  /// In en, this message translates to:
  /// **'Contact Name'**
  String get responsiblePersonName;

  /// Support contact prompt
  ///
  /// In en, this message translates to:
  /// **'Is there a problem? Call Support'**
  String get isThereAProblemCallSupport;

  /// Confirm action prompt
  ///
  /// In en, this message translates to:
  /// **'Click to Confirm'**
  String get clickToConfirm;

  /// Trip details placeholder text
  ///
  /// In en, this message translates to:
  /// **'Details are written here'**
  String get tripDetailsPlaceholder;

  /// Empty trips list message
  ///
  /// In en, this message translates to:
  /// **'No trips available'**
  String get noTripsAvailable;

  /// Empty notifications list message
  ///
  /// In en, this message translates to:
  /// **'No notifications'**
  String get noNotifications;

  /// Empty files list message in profile
  ///
  /// In en, this message translates to:
  /// **'No files available'**
  String get noFilesAvailable;

  /// Trips loading indicator
  ///
  /// In en, this message translates to:
  /// **'Loading trips...'**
  String get loadingTrips;

  /// Trips loading error message
  ///
  /// In en, this message translates to:
  /// **'Error loading trips'**
  String get errorLoadingTrips;

  /// Current trip tab label
  ///
  /// In en, this message translates to:
  /// **'Current Trip'**
  String get currentTripTab;

  /// Previous trips tab label
  ///
  /// In en, this message translates to:
  /// **'Previous Trips'**
  String get previousTripsTab;

  /// Your current trip section
  ///
  /// In en, this message translates to:
  /// **'Your Current Trip'**
  String get yourCurrentTrip;

  /// Your previous trips section
  ///
  /// In en, this message translates to:
  /// **'Your Previous Trips'**
  String get yourPreviousTrips;

  /// Shipment details section
  ///
  /// In en, this message translates to:
  /// **'Shipment Details'**
  String get shipmentDetails;

  /// Trip date and time display
  ///
  /// In en, this message translates to:
  /// **'Trip on: {date} at {time}'**
  String tripOnDayAtTime(String date, String time);

  /// On the way to loading status
  ///
  /// In en, this message translates to:
  /// **'On the way to loading'**
  String get onTheWayToLoading;

  /// Completed status
  ///
  /// In en, this message translates to:
  /// **'Completed'**
  String get completed;

  /// Trip schedule update title
  ///
  /// In en, this message translates to:
  /// **'Trip Schedule Updated'**
  String get tripScheduleUpdated;

  /// Trip schedule update notification message
  ///
  /// In en, this message translates to:
  /// **'Your trip schedule has been updated: {oldSchedule} to {newSchedule}'**
  String tripScheduleUpdatedMessage(String oldSchedule, String newSchedule);

  /// New schedule label
  ///
  /// In en, this message translates to:
  /// **'New schedule'**
  String get newSchedule;

  /// Accept trip button
  ///
  /// In en, this message translates to:
  /// **'Accept Trip'**
  String get acceptTrip;

  /// Arrive at waypoint button
  ///
  /// In en, this message translates to:
  /// **'I\'ve Arrived'**
  String get arriveAtWaypoint;

  /// Depart from waypoint button
  ///
  /// In en, this message translates to:
  /// **'Depart'**
  String get departFromWaypoint;

  /// Complete loading button
  ///
  /// In en, this message translates to:
  /// **'Complete Loading'**
  String get completeLoading;

  /// Start unloading button
  ///
  /// In en, this message translates to:
  /// **'Start Unloading'**
  String get startUnloading;

  /// Complete unloading button
  ///
  /// In en, this message translates to:
  /// **'Complete Unloading'**
  String get completeUnloading;

  /// Placeholder for optional note when completing loading
  ///
  /// In en, this message translates to:
  /// **'Optional note'**
  String get optionalNotePlaceholder;

  /// Complete trip button
  ///
  /// In en, this message translates to:
  /// **'Complete Trip'**
  String get completeTrip;

  /// Prompt to open camera for taking photos
  ///
  /// In en, this message translates to:
  /// **'Open camera for taking photos'**
  String get openCameraForPhotos;

  /// Open camera button
  ///
  /// In en, this message translates to:
  /// **'Open camera'**
  String get openCamera;

  /// Label shown while an action is being processed
  ///
  /// In en, this message translates to:
  /// **'Processing...'**
  String get processing;

  /// Shipment photos section label
  ///
  /// In en, this message translates to:
  /// **'Shipment Photos'**
  String get shipmentPhotos;

  /// Receipt note label
  ///
  /// In en, this message translates to:
  /// **'Receipt Note'**
  String get receiptNote;

  /// Delivery note label
  ///
  /// In en, this message translates to:
  /// **'Delivery Note'**
  String get deliveryNote;

  /// Unloading photos section label
  ///
  /// In en, this message translates to:
  /// **'Unloading Photos'**
  String get unloadingPhotos;

  /// Tollgate photos section label
  ///
  /// In en, this message translates to:
  /// **'Tollgate Photos'**
  String get tollgatePhotos;

  /// Note input placeholder
  ///
  /// In en, this message translates to:
  /// **'Note'**
  String get notePlaceholder;

  /// Note required validation error
  ///
  /// In en, this message translates to:
  /// **'Note is required'**
  String get noteRequired;

  /// No note placeholder for trip point
  ///
  /// In en, this message translates to:
  /// **'No note'**
  String get tripPointNoNote;

  /// Location loading indicator
  ///
  /// In en, this message translates to:
  /// **'Fetching location...'**
  String get currentLocationLoading;

  /// View document button
  ///
  /// In en, this message translates to:
  /// **'View Document'**
  String get viewDocument;

  /// Odometer label
  ///
  /// In en, this message translates to:
  /// **'Odometer'**
  String get odometer;

  /// Odometer image label
  ///
  /// In en, this message translates to:
  /// **'Odometer Image'**
  String get odometerImage;

  /// Receipt image label
  ///
  /// In en, this message translates to:
  /// **'Receipt Image'**
  String get receiptImage;

  /// Shipment images label
  ///
  /// In en, this message translates to:
  /// **'Shipment Images'**
  String get shipmentImages;

  /// Delivery note image label
  ///
  /// In en, this message translates to:
  /// **'Delivery Note Image'**
  String get deliveryNoteImage;

  /// Tollgate images label
  ///
  /// In en, this message translates to:
  /// **'Tollgate Images'**
  String get tollgateImages;

  /// Generic trip error message
  ///
  /// In en, this message translates to:
  /// **'An error occurred. Please try again.'**
  String get tripErrorGeneric;

  /// Error when accepting non-scheduled trip
  ///
  /// In en, this message translates to:
  /// **'Trip must be scheduled to accept.'**
  String get tripErrorMustBeScheduledToAccept;

  /// Error when delivery note is missing at dropoff
  ///
  /// In en, this message translates to:
  /// **'Delivery note image is required at dropoff.'**
  String get tripErrorDeliveryNoteRequired;

  /// Error when waypoint is not pending
  ///
  /// In en, this message translates to:
  /// **'Waypoint must be pending.'**
  String get tripErrorWaypointMustBePending;

  /// Error when action requires arrival first
  ///
  /// In en, this message translates to:
  /// **'You must arrive at the waypoint first.'**
  String get tripErrorWaypointMustBeArrived;

  /// Error when loading not started
  ///
  /// In en, this message translates to:
  /// **'Loading must be started first.'**
  String get tripErrorWaypointMustBeLoadingStarted;

  /// Error when loading not completed
  ///
  /// In en, this message translates to:
  /// **'Loading must be completed first.'**
  String get tripErrorWaypointMustBeLoadingCompleted;

  /// Error when not all waypoints are completed
  ///
  /// In en, this message translates to:
  /// **'All waypoints must be completed first.'**
  String get tripErrorAllWaypointsMustBeCompleted;

  /// Error when first pickup loading not completed
  ///
  /// In en, this message translates to:
  /// **'First pickup loading must be completed first.'**
  String get tripErrorFirstWaypointMustBeLoadingCompleted;

  /// Error when trip is not in progress
  ///
  /// In en, this message translates to:
  /// **'Trip must be in progress.'**
  String get tripErrorTripMustBeInProgress;

  /// Error when trip not accepted
  ///
  /// In en, this message translates to:
  /// **'Trip must be accepted first.'**
  String get tripErrorTripMustBeDriverAccepted;

  /// Error when first pickup loading not done
  ///
  /// In en, this message translates to:
  /// **'First pickup loading must be completed first.'**
  String get tripErrorTripMustBeFirstPickupLoading;

  /// Error for invalid image format
  ///
  /// In en, this message translates to:
  /// **'Image must be JPEG, PNG, or WebP.'**
  String get tripErrorInvalidImageFormat;

  /// Error for image size limit
  ///
  /// In en, this message translates to:
  /// **'Image must be 10MB or less.'**
  String get tripErrorImageTooLarge;

  /// Error when captured image is not an odometer
  ///
  /// In en, this message translates to:
  /// **'Please capture a clear photo of the car odometer.'**
  String get tripErrorInvalidOdometerImage;

  /// Error when odometer text cannot be read by OCR
  ///
  /// In en, this message translates to:
  /// **'Could not read the odometer. Please retake the photo with better focus.'**
  String get tripErrorUnreadableOdometerImage;

  /// Load more button
  ///
  /// In en, this message translates to:
  /// **'Load More'**
  String get loadMore;

  /// Filter by date label
  ///
  /// In en, this message translates to:
  /// **'Filter by Date'**
  String get filterByDate;

  /// From date filter label
  ///
  /// In en, this message translates to:
  /// **'From'**
  String get fromDate;

  /// To date filter label
  ///
  /// In en, this message translates to:
  /// **'To'**
  String get toDate;

  /// Apply filter button
  ///
  /// In en, this message translates to:
  /// **'Apply'**
  String get applyFilter;

  /// Clear filter button
  ///
  /// In en, this message translates to:
  /// **'Clear'**
  String get clearFilter;

  /// Documents section title
  ///
  /// In en, this message translates to:
  /// **'Documents'**
  String get documents;

  /// Document open failure message with copy option
  ///
  /// In en, this message translates to:
  /// **'Could not open document. Copy URL?'**
  String get documentOpenFailed;

  /// Copy URL button
  ///
  /// In en, this message translates to:
  /// **'Copy URL'**
  String get copyUrl;

  /// URL copied confirmation message
  ///
  /// In en, this message translates to:
  /// **'URL copied to clipboard'**
  String get urlCopied;

  /// Shown when document type id is missing before upload or update
  ///
  /// In en, this message translates to:
  /// **'Could not detect document type.'**
  String get documentTypeMissing;

  /// Language setting label
  ///
  /// In en, this message translates to:
  /// **'Language'**
  String get language;

  /// Arabic language label
  ///
  /// In en, this message translates to:
  /// **'Arabic'**
  String get languageArabic;

  /// English language label
  ///
  /// In en, this message translates to:
  /// **'English'**
  String get languageEnglish;

  /// Button to open external maps for turn-by-turn navigation to the waypoint
  ///
  /// In en, this message translates to:
  /// **'Navigate to waypoint'**
  String get navigateToWaypoint;

  /// Button to navigate to pickup waypoint
  ///
  /// In en, this message translates to:
  /// **'Navigate to pickup'**
  String get navigateToPickup;

  /// Button to navigate to dropoff waypoint
  ///
  /// In en, this message translates to:
  /// **'Navigate to dropoff'**
  String get navigateToDropoff;

  /// Loads Google Directions road-aligned polyline (billable); default map uses straight segments
  ///
  /// In en, this message translates to:
  /// **'Show road route'**
  String get showRoadRoute;

  /// Shown while fetching road route from Directions API
  ///
  /// In en, this message translates to:
  /// **'Loading route…'**
  String get loadingRoadRoute;

  /// Title on trip completion success screen
  ///
  /// In en, this message translates to:
  /// **'Trip Completed Successfully'**
  String get tripCompletedSuccessfully;

  /// Message on trip completion success screen
  ///
  /// In en, this message translates to:
  /// **'Thank you for completing the trip. Great job!'**
  String get tripCompletedSuccessMessage;

  /// Button to return to trips list from success screen
  ///
  /// In en, this message translates to:
  /// **'Back to Trips'**
  String get backToTrips;

  /// Home map when system GPS/location is off
  ///
  /// In en, this message translates to:
  /// **'Location services are turned off. Turn them on to use the map and go online.'**
  String get homeLocationServicesDisabled;

  /// Home map when user denied location permission
  ///
  /// In en, this message translates to:
  /// **'Location permission was denied. Allow location access to use the map and go online.'**
  String get homeLocationPermissionDenied;

  /// Home map when location is blocked in system settings
  ///
  /// In en, this message translates to:
  /// **'Location permission is permanently denied. Enable it in the app settings.'**
  String get homeLocationPermissionDeniedForever;

  /// Home map when Geolocator/plugin fails
  ///
  /// In en, this message translates to:
  /// **'Location is unavailable. Try restarting the app.'**
  String get homeLocationUnavailable;

  /// Home map when getCurrentPosition fails
  ///
  /// In en, this message translates to:
  /// **'Could not get your location. Please try again.'**
  String get homeLocationGetPositionFailed;

  /// Home map when initial position for tracking fails
  ///
  /// In en, this message translates to:
  /// **'Could not start location updates. Please try again.'**
  String get homeLocationGetInitialFailed;

  /// Home map when position stream errors
  ///
  /// In en, this message translates to:
  /// **'Location updates stopped. Please try again.'**
  String get homeLocationStreamError;

  /// App version display
  ///
  /// In en, this message translates to:
  /// **'Version {version}'**
  String appVersion(String version);

  /// Button to proceed to the system permission dialog after reading disclosure
  ///
  /// In en, this message translates to:
  /// **'Continue'**
  String get prominentDisclosureContinue;

  /// Decline the in-app permission disclosure
  ///
  /// In en, this message translates to:
  /// **'Not now'**
  String get prominentDisclosureNotNow;

  /// Title for in-app location disclosure before system permission
  ///
  /// In en, this message translates to:
  /// **'How loadly Workplace  uses your location'**
  String get locationPermissionDisclosureTitle;

  /// Prominent disclosure for location including background use (Google Play User Data policy)
  ///
  /// In en, this message translates to:
  /// **'Loadly Workplace collects your device location—including in the background when the app is not on screen—while you are online or working on trips. We use it to show your position on the map, switch online/offline accurately, plan routes for pickups and deliveries, and share live location with your company. Location is sent only to Loadly servers as needed for these driver features.\n\nYou will first be asked to allow location while using the app. If full live tracking is needed, a second screen will explain background access, followed by another system permission step (for example “Allow all the time”). Tap Continue to start.'**
  String get locationPermissionDisclosureBody;

  /// Title for second disclosure before background / always location permission
  ///
  /// In en, this message translates to:
  /// **'Location in the background'**
  String get locationBackgroundPermissionDisclosureTitle;

  /// Prominent disclosure before ACCESS_BACKGROUND_LOCATION / always permission step
  ///
  /// In en, this message translates to:
  /// **'So your company can receive reliable live updates while you drive, Loadly Workplace needs permission to access location in the background (when the app is behind other apps or the screen is off). This is only used for the driver features described on the previous screen—not for advertising.\n\nTap Continue to open the next system permission. On some devices you must choose “Allow all the time” or enable background location in Settings.'**
  String get locationBackgroundPermissionDisclosureBody;

  /// Title for in-app notification disclosure before system permission
  ///
  /// In en, this message translates to:
  /// **'Notifications'**
  String get notificationPermissionDisclosureTitle;

  /// Prominent disclosure for notifications (Google Play User Data policy)
  ///
  /// In en, this message translates to:
  /// **'Loadly Workplace sends notifications about trips, schedules, and important updates. To deliver them, the app uses push notifications and a device token stored on our servers. Tap Continue to open the system permission request.'**
  String get notificationPermissionDisclosureBody;

  /// Title for in-app camera disclosure before system permission
  ///
  /// In en, this message translates to:
  /// **'Camera access'**
  String get cameraPermissionDisclosureTitle;

  /// Prominent disclosure for camera (Google Play User Data policy)
  ///
  /// In en, this message translates to:
  /// **'Loadly Workplace uses the camera only when you choose to take a photo—for example for your profile picture or required documents. Photos you take are uploaded to our servers for your account. Tap Continue to open the system permission request.'**
  String get cameraPermissionDisclosureBody;

  /// Title for in-app photos/library disclosure before system permission
  ///
  /// In en, this message translates to:
  /// **'Photos and files'**
  String get photosPermissionDisclosureTitle;

  /// Prominent disclosure for photo library (Google Play User Data policy)
  ///
  /// In en, this message translates to:
  /// **'Loadly Workplace accesses images you select from your photo library or device storage to upload profile or document images. We only access files you choose. Tap Continue to open the system permission request.'**
  String get photosPermissionDisclosureBody;

  /// Title for blocking force update screen
  ///
  /// In en, this message translates to:
  /// **'Update required'**
  String get appUpdateRequiredTitle;

  /// Title for optional update prompt
  ///
  /// In en, this message translates to:
  /// **'Update available'**
  String get appUpdateAvailableTitle;

  /// Default message for force update screen
  ///
  /// In en, this message translates to:
  /// **'A new version of Loadly Workplace is required to continue. Please update to the latest version.'**
  String get appUpdateRequiredMessage;

  /// Default message for optional update prompt
  ///
  /// In en, this message translates to:
  /// **'A new version of Loadly Workplace is available with improvements and fixes.'**
  String get appUpdateAvailableMessage;

  /// Shows the latest app version on the update prompt
  ///
  /// In en, this message translates to:
  /// **'Latest version: {version}'**
  String appUpdateLatestVersion(String version);

  /// Primary action to open the app store for an update
  ///
  /// In en, this message translates to:
  /// **'Update now'**
  String get appUpdateNow;

  /// Dismiss optional update prompt
  ///
  /// In en, this message translates to:
  /// **'Later'**
  String get appUpdateLater;

  /// No description provided for @filter.
  ///
  /// In en, this message translates to:
  /// **'Filter'**
  String get filter;

  /// No description provided for @tripDateIs.
  ///
  /// In en, this message translates to:
  /// **'Trip date is'**
  String get tripDateIs;

  /// No description provided for @collectDocuments.
  ///
  /// In en, this message translates to:
  /// **'Collect documents'**
  String get collectDocuments;

  /// No description provided for @partiallyCollectedDocuments.
  ///
  /// In en, this message translates to:
  /// **'Partially collected documents'**
  String get partiallyCollectedDocuments;

  /// No description provided for @noDocumentsAvailable.
  ///
  /// In en, this message translates to:
  /// **'No documents available'**
  String get noDocumentsAvailable;

  /// No description provided for @received.
  ///
  /// In en, this message translates to:
  /// **'Received'**
  String get received;

  /// No description provided for @notReceived.
  ///
  /// In en, this message translates to:
  /// **'Not received'**
  String get notReceived;

  /// No description provided for @stillWithCustomerWaitingForSignature.
  ///
  /// In en, this message translates to:
  /// **'Still With Customer Waiting For Signature'**
  String get stillWithCustomerWaitingForSignature;

  /// No description provided for @stillWithCustomer.
  ///
  /// In en, this message translates to:
  /// **'Still With Customer'**
  String get stillWithCustomer;

  /// No description provided for @lostByDriver.
  ///
  /// In en, this message translates to:
  /// **'Lost By Driver'**
  String get lostByDriver;

  /// No description provided for @lostDocument.
  ///
  /// In en, this message translates to:
  /// **'Lost document'**
  String get lostDocument;

  /// No description provided for @tripDoesNotExist.
  ///
  /// In en, this message translates to:
  /// **'Trip Does Not Exist'**
  String get tripDoesNotExist;

  /// No description provided for @willBeSentLater.
  ///
  /// In en, this message translates to:
  /// **'Will Be Sent Later'**
  String get willBeSentLater;

  /// No description provided for @customerRefusedToSign.
  ///
  /// In en, this message translates to:
  /// **'Customer Refused To Sign'**
  String get customerRefusedToSign;

  /// No description provided for @other.
  ///
  /// In en, this message translates to:
  /// **'Other'**
  String get other;

  /// No description provided for @note.
  ///
  /// In en, this message translates to:
  /// **'Note'**
  String get note;

  /// No description provided for @confirm.
  ///
  /// In en, this message translates to:
  /// **'Confirm'**
  String get confirm;

  /// No description provided for @deliveryPermission.
  ///
  /// In en, this message translates to:
  /// **'Delivery permission'**
  String get deliveryPermission;

  /// No description provided for @selectTheReasonsForNotReceivingTheDocument.
  ///
  /// In en, this message translates to:
  /// **'Select the reasons for not receiving the document'**
  String get selectTheReasonsForNotReceivingTheDocument;

  /// No description provided for @enterNote.
  ///
  /// In en, this message translates to:
  /// **'In case of anthor reason please enter a note'**
  String get enterNote;

  /// No description provided for @takeAPhotoOfTheDocument.
  ///
  /// In en, this message translates to:
  /// **'Take a photo of the document (optional)'**
  String get takeAPhotoOfTheDocument;

  /// No description provided for @openCameraDocument.
  ///
  /// In en, this message translates to:
  /// **'photograph document'**
  String get openCameraDocument;

  /// No description provided for @saveAndContinueLater.
  ///
  /// In en, this message translates to:
  /// **'Save and continue later'**
  String get saveAndContinueLater;

  /// No description provided for @saveAndSendDocuments.
  ///
  /// In en, this message translates to:
  /// **'Save & Send documents'**
  String get saveAndSendDocuments;

  /// No description provided for @confirmAndContinueLater.
  ///
  /// In en, this message translates to:
  /// **'Confirm and continue later'**
  String get confirmAndContinueLater;

  /// No description provided for @confirmAndSendDocuments.
  ///
  /// In en, this message translates to:
  /// **'Confirm & Send documents'**
  String get confirmAndSendDocuments;

  /// No description provided for @confirmAllDocuments.
  ///
  /// In en, this message translates to:
  /// **'Confirm all documents'**
  String get confirmAllDocuments;

  /// No description provided for @pleaseConfirmAllDocuments.
  ///
  /// In en, this message translates to:
  /// **'Please confirm all documents'**
  String get pleaseConfirmAllDocuments;

  /// No description provided for @saveAndSendDocumentsDesc.
  ///
  /// In en, this message translates to:
  /// **'Click send, and the documents will be saved and sent to LOADLY within seconds.'**
  String get saveAndSendDocumentsDesc;

  /// No description provided for @saved.
  ///
  /// In en, this message translates to:
  /// **'Saved'**
  String get saved;

  /// No description provided for @savedDesc.
  ///
  /// In en, this message translates to:
  /// **'The process has been saved. Please complete it later to collect the documents and send them to the company as soon as possible.'**
  String get savedDesc;

  /// No description provided for @theDocumentsThatWillBeSent.
  ///
  /// In en, this message translates to:
  /// **'The documents that will be sent'**
  String get theDocumentsThatWillBeSent;

  /// No description provided for @sendDocuments.
  ///
  /// In en, this message translates to:
  /// **'Send documents'**
  String get sendDocuments;

  /// No description provided for @pullToRefresh.
  ///
  /// In en, this message translates to:
  /// **'Pull to Refresh'**
  String get pullToRefresh;

  /// No description provided for @releaseToRefresh.
  ///
  /// In en, this message translates to:
  /// **'Release to Refresh'**
  String get releaseToRefresh;

  /// No description provided for @searchForContractId.
  ///
  /// In en, this message translates to:
  /// **'Search for contract ID'**
  String get searchForContractId;

  /// No description provided for @searchForCompanyId.
  ///
  /// In en, this message translates to:
  /// **'Search for company ID'**
  String get searchForCompanyId;

  /// No description provided for @searchForVehicleId.
  ///
  /// In en, this message translates to:
  /// **'Search for vehicle ID'**
  String get searchForVehicleId;

  /// No description provided for @searchForVehicleTypeId.
  ///
  /// In en, this message translates to:
  /// **'Search for vehicle Type ID'**
  String get searchForVehicleTypeId;

  /// No description provided for @searchForDriverId.
  ///
  /// In en, this message translates to:
  /// **'Search for driver ID'**
  String get searchForDriverId;

  /// No description provided for @searchByDate.
  ///
  /// In en, this message translates to:
  /// **'Search by date'**
  String get searchByDate;

  /// No description provided for @today.
  ///
  /// In en, this message translates to:
  /// **'Today'**
  String get today;

  /// No description provided for @yesterday.
  ///
  /// In en, this message translates to:
  /// **'Yesterday'**
  String get yesterday;

  /// No description provided for @lastWeek.
  ///
  /// In en, this message translates to:
  /// **'Last 7 days'**
  String get lastWeek;

  /// No description provided for @chooseDate.
  ///
  /// In en, this message translates to:
  /// **'Choose date'**
  String get chooseDate;

  /// No description provided for @tripStatus.
  ///
  /// In en, this message translates to:
  /// **'Trip status'**
  String get tripStatus;

  /// No description provided for @noDocumentsCollected.
  ///
  /// In en, this message translates to:
  /// **'No documents collected'**
  String get noDocumentsCollected;

  /// No description provided for @partiallyConfirmedDocuments.
  ///
  /// In en, this message translates to:
  /// **'Partially confirmed documents'**
  String get partiallyConfirmedDocuments;

  /// No description provided for @resetFilter.
  ///
  /// In en, this message translates to:
  /// **'Reset filter'**
  String get resetFilter;

  /// No description provided for @startDate.
  ///
  /// In en, this message translates to:
  /// **'Start date'**
  String get startDate;

  /// No description provided for @endDate.
  ///
  /// In en, this message translates to:
  /// **'End date'**
  String get endDate;

  /// No description provided for @done.
  ///
  /// In en, this message translates to:
  /// **'Done'**
  String get done;

  /// No description provided for @reset.
  ///
  /// In en, this message translates to:
  /// **'Reset'**
  String get reset;

  /// No description provided for @companyName.
  ///
  /// In en, this message translates to:
  /// **'Company name'**
  String get companyName;

  /// No description provided for @driverName.
  ///
  /// In en, this message translates to:
  /// **'driver name'**
  String get driverName;

  /// No description provided for @carNumber.
  ///
  /// In en, this message translates to:
  /// **'car number'**
  String get carNumber;

  /// No description provided for @documentsPictures.
  ///
  /// In en, this message translates to:
  /// **'Documents pictures'**
  String get documentsPictures;

  /// No description provided for @receiveDocumentsForSelectedTrips.
  ///
  /// In en, this message translates to:
  /// **'Receive documents for selected trips'**
  String get receiveDocumentsForSelectedTrips;

  /// No description provided for @documentsAreBeingCollected.
  ///
  /// In en, this message translates to:
  /// **'Documents are being collected'**
  String get documentsAreBeingCollected;

  /// No description provided for @documentsAreBeingCollectedDesc.
  ///
  /// In en, this message translates to:
  /// **'Please wait a few moments until all trips documents are collected at once.'**
  String get documentsAreBeingCollectedDesc;

  /// No description provided for @selectedTripsDocuments.
  ///
  /// In en, this message translates to:
  /// **'Selected trips documents'**
  String get selectedTripsDocuments;

  /// No description provided for @receiptConfirmed.
  ///
  /// In en, this message translates to:
  /// **'Receipt confirmed'**
  String get receiptConfirmed;

  /// No description provided for @confirmed.
  ///
  /// In en, this message translates to:
  /// **'Confirmed'**
  String get confirmed;

  /// No description provided for @receiptConfirmedDesc.
  ///
  /// In en, this message translates to:
  /// **'Click Confirm, and the documents will be confirmed and sent to LOADLY within seconds.'**
  String get receiptConfirmedDesc;

  /// No description provided for @selectedTrips.
  ///
  /// In en, this message translates to:
  /// **'Selected trips'**
  String get selectedTrips;

  /// No description provided for @receiptConfirmation.
  ///
  /// In en, this message translates to:
  /// **'Receipt confirmation'**
  String get receiptConfirmation;

  /// No description provided for @receiptConfirmationWithMark.
  ///
  /// In en, this message translates to:
  /// **'Receipt confirmation?'**
  String get receiptConfirmationWithMark;

  /// No description provided for @documentsCollectedByFactoryCollector.
  ///
  /// In en, this message translates to:
  /// **'Documents collected by factory collector'**
  String get documentsCollectedByFactoryCollector;

  /// No description provided for @documentsCollectedFromFactoryCollector.
  ///
  /// In en, this message translates to:
  /// **'Documents collected from factory collector'**
  String get documentsCollectedFromFactoryCollector;

  /// No description provided for @receiptConfirmationForSelectedTrips.
  ///
  /// In en, this message translates to:
  /// **'Receipt confirmation for selected trips'**
  String get receiptConfirmationForSelectedTrips;

  /// No description provided for @confirmWithMark.
  ///
  /// In en, this message translates to:
  /// **'Confirm?'**
  String get confirmWithMark;

  /// No description provided for @incorrectPassword.
  ///
  /// In en, this message translates to:
  /// **'Incorrect password, Please try again'**
  String get incorrectPassword;

  /// No description provided for @withCollector.
  ///
  /// In en, this message translates to:
  /// **'Collected by factory collector'**
  String get withCollector;

  /// No description provided for @externalTrip.
  ///
  /// In en, this message translates to:
  /// **'External trip'**
  String get externalTrip;

  /// No description provided for @username.
  ///
  /// In en, this message translates to:
  /// **'Username'**
  String get username;

  /// No description provided for @enterUsername.
  ///
  /// In en, this message translates to:
  /// **'Enter your username'**
  String get enterUsername;

  /// No description provided for @enterValidUsername.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid userbame must be more than 3 characters'**
  String get enterValidUsername;

  /// No description provided for @displayDocuments.
  ///
  /// In en, this message translates to:
  /// **'Display documents'**
  String get displayDocuments;

  /// No description provided for @noDriver.
  ///
  /// In en, this message translates to:
  /// **'No driver'**
  String get noDriver;

  /// No description provided for @edit.
  ///
  /// In en, this message translates to:
  /// **'Edit'**
  String get edit;

  /// No description provided for @factoryCollector.
  ///
  /// In en, this message translates to:
  /// **'Factory collector'**
  String get factoryCollector;

  /// No description provided for @search.
  ///
  /// In en, this message translates to:
  /// **'Search'**
  String get search;

  /// No description provided for @unexpectedError.
  ///
  /// In en, this message translates to:
  /// **'An unexpected error occurred. Please try again.'**
  String get unexpectedError;

  /// No description provided for @networkErrorPleaseRetry.
  ///
  /// In en, this message translates to:
  /// **'A network error occurred. Please check your connection and try again.'**
  String get networkErrorPleaseRetry;

  /// No description provided for @noInternetConnection.
  ///
  /// In en, this message translates to:
  /// **'No internet connection. Please check your connection and try again.'**
  String get noInternetConnection;

  /// No description provided for @requestFailedTryAgain.
  ///
  /// In en, this message translates to:
  /// **'The request could not be completed. Please try again.'**
  String get requestFailedTryAgain;

  /// No description provided for @failedToLoadTrips.
  ///
  /// In en, this message translates to:
  /// **'Could not load trips. Please try again.'**
  String get failedToLoadTrips;

  /// No description provided for @failedToLoadDocuments.
  ///
  /// In en, this message translates to:
  /// **'Could not load documents. Please try again.'**
  String get failedToLoadDocuments;

  /// No description provided for @failedToCollectDocuments.
  ///
  /// In en, this message translates to:
  /// **'Could not receive the documents. Please try again.'**
  String get failedToCollectDocuments;

  /// No description provided for @failedToConfirmDocuments.
  ///
  /// In en, this message translates to:
  /// **'Could not confirm the documents. Please try again.'**
  String get failedToConfirmDocuments;

  /// No description provided for @invalidLoginCredentials.
  ///
  /// In en, this message translates to:
  /// **'The username or password is incorrect.'**
  String get invalidLoginCredentials;

  /// No description provided for @invalidPhoneNumber.
  ///
  /// In en, this message translates to:
  /// **'The phone number is incorrect.'**
  String get invalidPhoneNumber;

  /// No description provided for @invalidVerificationCode.
  ///
  /// In en, this message translates to:
  /// **'The verification code is incorrect or has expired.'**
  String get invalidVerificationCode;

  /// No description provided for @sessionExpired.
  ///
  /// In en, this message translates to:
  /// **'Your session has expired. Please sign in again.'**
  String get sessionExpired;

  /// No description provided for @invalidRequest.
  ///
  /// In en, this message translates to:
  /// **'The request is invalid. Please review the information you entered.'**
  String get invalidRequest;

  /// No description provided for @resourceNotFound.
  ///
  /// In en, this message translates to:
  /// **'The requested item was not found.'**
  String get resourceNotFound;

  /// No description provided for @actionNotAllowed.
  ///
  /// In en, this message translates to:
  /// **'This action is not allowed.'**
  String get actionNotAllowed;

  /// No description provided for @invalidSubmittedData.
  ///
  /// In en, this message translates to:
  /// **'The information you entered is invalid.'**
  String get invalidSubmittedData;

  /// No description provided for @serverErrorTryLater.
  ///
  /// In en, this message translates to:
  /// **'A server error occurred. Please try again later.'**
  String get serverErrorTryLater;

  /// No description provided for @failedToLoadProfile.
  ///
  /// In en, this message translates to:
  /// **'Could not load your profile. Please try again.'**
  String get failedToLoadProfile;

  /// No description provided for @somethingWentWrongTryAgain.
  ///
  /// In en, this message translates to:
  /// **'Something went wrong. Please try again.'**
  String get somethingWentWrongTryAgain;

  /// No description provided for @emptyServerResponse.
  ///
  /// In en, this message translates to:
  /// **'Could not read the server response. Please try again.'**
  String get emptyServerResponse;

  /// No description provided for @missingServerData.
  ///
  /// In en, this message translates to:
  /// **'The required information is unavailable. Please try again.'**
  String get missingServerData;

  /// No description provided for @passwordResetFailed.
  ///
  /// In en, this message translates to:
  /// **'Could not reset the password. Please try again.'**
  String get passwordResetFailed;

  /// No description provided for @couldNotSendResetCode.
  ///
  /// In en, this message translates to:
  /// **'Could not send the password reset code. Please try again.'**
  String get couldNotSendResetCode;

  /// No description provided for @tooManyAttempts.
  ///
  /// In en, this message translates to:
  /// **'Too many attempts. Please wait and try again.'**
  String get tooManyAttempts;

  /// No description provided for @notificationChannelDescription.
  ///
  /// In en, this message translates to:
  /// **'General app notifications'**
  String get notificationChannelDescription;

  /// No description provided for @notificationFallbackTitle.
  ///
  /// In en, this message translates to:
  /// **'Notification'**
  String get notificationFallbackTitle;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['ar', 'en'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'ar':
      return AppLocalizationsAr();
    case 'en':
      return AppLocalizationsEn();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.',
  );
}
