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
  /// **'Loadly Contractor'**
  String get appTitle;

  /// Welcome greeting on login screen
  ///
  /// In en, this message translates to:
  /// **'Welcome'**
  String get welcome;

  /// No description provided for @welcomeToLoadly.
  ///
  /// In en, this message translates to:
  /// **'Welcome to Loadly'**
  String get welcomeToLoadly;

  /// No description provided for @welcomeDesc.
  ///
  /// In en, this message translates to:
  /// **'Sign in to your account or create a new one'**
  String get welcomeDesc;

  /// No description provided for @switchLanguage.
  ///
  /// In en, this message translates to:
  /// **'Switch language'**
  String get switchLanguage;

  /// No description provided for @createAccount.
  ///
  /// In en, this message translates to:
  /// **'Create account'**
  String get createAccount;

  /// No description provided for @createNewAccount.
  ///
  /// In en, this message translates to:
  /// **'Create new account'**
  String get createNewAccount;

  /// Instruction for phone verification
  ///
  /// In en, this message translates to:
  /// **'Enter your phone number and we will send you a verification code'**
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

  /// full name placeholder
  ///
  /// In en, this message translates to:
  /// **'Enter your full name'**
  String get enterYourFullName;

  /// address details placeholder
  ///
  /// In en, this message translates to:
  /// **'Enter your address details'**
  String get enterYourAddress;

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

  /// National ID Number
  ///
  /// In en, this message translates to:
  /// **'National ID Number'**
  String get enterYourId;

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

  /// Time ago in hours
  ///
  /// In en, this message translates to:
  /// **'{hours} hours ago'**
  String hoursAgo(int hours);

  /// Trip details screen title
  ///
  /// In en, this message translates to:
  /// **'Trip Details'**
  String get tripDetails;

  /// Trip ID label
  ///
  /// In en, this message translates to:
  /// **'Trip ID'**
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

  /// Tollgates Total Amount section label
  ///
  /// In en, this message translates to:
  /// **'Tollgates Total Amount'**
  String get tollgatesTotalAmount;

  /// Tollgates weight total amount section label
  ///
  /// In en, this message translates to:
  /// **'Tollgates Weight Total Amount'**
  String get tollgatesWeightTotalAmount;

  /// Tollgate weight images section label
  ///
  /// In en, this message translates to:
  /// **'Tollgate Weight Images'**
  String get tollgateWeightImages;

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

  /// Apply button
  ///
  /// In en, this message translates to:
  /// **'Apply'**
  String get apply;

  /// Clear button
  ///
  /// In en, this message translates to:
  /// **'Clear'**
  String get clear;

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
  /// **'How Loadly Contractor uses your location'**
  String get locationPermissionDisclosureTitle;

  /// Prominent disclosure for location including background use (Google Play User Data policy)
  ///
  /// In en, this message translates to:
  /// **'Loadly Contractor collects your device location—including in the background when the app is not on screen—while you are online or working on trips. We use it to show your position on the map, switch online/offline accurately, plan routes for pickups and deliveries, and share live location with your company. Location is sent only to Loadly servers as needed for these contractor features.\n\nYou will first be asked to allow location while using the app. If full live tracking is needed, a second screen will explain background access, followed by another system permission step (for example “Allow all the time”). Tap Continue to start.'**
  String get locationPermissionDisclosureBody;

  /// Title for second disclosure before background / always location permission
  ///
  /// In en, this message translates to:
  /// **'Location in the background'**
  String get locationBackgroundPermissionDisclosureTitle;

  /// Prominent disclosure before ACCESS_BACKGROUND_LOCATION / always permission step
  ///
  /// In en, this message translates to:
  /// **'So your company can receive reliable live updates while you drive, Loadly Contractor needs permission to access location in the background (when the app is behind other apps or the screen is off). This is only used for the contractor features described on the previous screen—not for advertising.\n\nTap Continue to open the next system permission. On some devices you must choose “Allow all the time” or enable background location in Settings.'**
  String get locationBackgroundPermissionDisclosureBody;

  /// Android foreground service notification title while sharing live location
  ///
  /// In en, this message translates to:
  /// **'Loadly Contractor is online'**
  String get locationTrackingNotificationTitle;

  /// Android foreground service notification body while sharing live location
  ///
  /// In en, this message translates to:
  /// **'Sharing your location with Loadly while you are online.'**
  String get locationTrackingNotificationText;

  /// Title for in-app notification disclosure before system permission
  ///
  /// In en, this message translates to:
  /// **'Notifications'**
  String get notificationPermissionDisclosureTitle;

  /// Prominent disclosure for notifications (Google Play User Data policy)
  ///
  /// In en, this message translates to:
  /// **'Loadly Contractor sends notifications about trips, schedules, and important updates. To deliver them, the app uses push notifications and a device token stored on our servers. Tap Continue to open the system permission request.'**
  String get notificationPermissionDisclosureBody;

  /// Title for in-app camera disclosure before system permission
  ///
  /// In en, this message translates to:
  /// **'Camera access'**
  String get cameraPermissionDisclosureTitle;

  /// Prominent disclosure for camera (Google Play User Data policy)
  ///
  /// In en, this message translates to:
  /// **'Loadly Contractor uses the camera only when you choose to take a photo—for example for your profile picture or required documents. Photos you take are uploaded to our servers for your contractor account. Tap Continue to open the system permission request.'**
  String get cameraPermissionDisclosureBody;

  /// Title for in-app photos/library disclosure before system permission
  ///
  /// In en, this message translates to:
  /// **'Photos and files'**
  String get photosPermissionDisclosureTitle;

  /// Prominent disclosure for photo library (Google Play User Data policy)
  ///
  /// In en, this message translates to:
  /// **'Loadly Contractor accesses images you select from your photo library or device storage to upload profile or document images. We only access files you choose. Tap Continue to open the system permission request.'**
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
  /// **'A new version of Loadly Contractor is required to continue. Please update to the latest version.'**
  String get appUpdateRequiredMessage;

  /// Default message for optional update prompt
  ///
  /// In en, this message translates to:
  /// **'A new version of Loadly Contractor is available with improvements and fixes.'**
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

  /// Enter customs
  ///
  /// In en, this message translates to:
  /// **'Enter customs'**
  String get enterCustoms;

  /// Exit customs
  ///
  /// In en, this message translates to:
  /// **'Exit customs'**
  String get exitCustoms;

  /// Customs total amount
  ///
  /// In en, this message translates to:
  /// **'Customs total amount'**
  String get customsTotalAmount;

  /// Document image
  ///
  /// In en, this message translates to:
  /// **'Document image'**
  String get documentImage;

  /// One or more photos could not be found. Please retake them.
  ///
  /// In en, this message translates to:
  /// **'One or more photos could not be found. Please retake them.'**
  String get tripErrorImageMissingRetake;

  /// question mark
  ///
  /// In en, this message translates to:
  /// **'?'**
  String get questionMark;

  /// No description provided for @invalidCredentials.
  ///
  /// In en, this message translates to:
  /// **'Invalid credentials.'**
  String get invalidCredentials;

  /// No description provided for @invalidPhoneNumber.
  ///
  /// In en, this message translates to:
  /// **'Invalid phone number.'**
  String get invalidPhoneNumber;

  /// No description provided for @networkError.
  ///
  /// In en, this message translates to:
  /// **'No internet connection.'**
  String get networkError;

  /// No description provided for @unknownError.
  ///
  /// In en, this message translates to:
  /// **'Something went wrong.'**
  String get unknownError;

  /// No description provided for @requestFailed.
  ///
  /// In en, this message translates to:
  /// **'Request failed.'**
  String get requestFailed;

  /// No description provided for @failedToLoadDrivers.
  ///
  /// In en, this message translates to:
  /// **'Failed to load drivers.'**
  String get failedToLoadDrivers;

  /// No description provided for @failedToLoadDocumentTypes.
  ///
  /// In en, this message translates to:
  /// **'Failed to load document types.'**
  String get failedToLoadDocumentTypes;

  /// No description provided for @failedToCreateDriver.
  ///
  /// In en, this message translates to:
  /// **'Failed to create driver.'**
  String get failedToCreateDriver;

  /// No description provided for @failedToUpdateTripDriver.
  ///
  /// In en, this message translates to:
  /// **'Failed to update trip driver.'**
  String get failedToUpdateTripDriver;

  /// No description provided for @failedToLoadTrip.
  ///
  /// In en, this message translates to:
  /// **'Failed to load trip.'**
  String get failedToLoadTrip;

  /// No description provided for @failedToLoadTrips.
  ///
  /// In en, this message translates to:
  /// **'Failed to load trips.'**
  String get failedToLoadTrips;

  /// No description provided for @failedToLoadBookingRequests.
  ///
  /// In en, this message translates to:
  /// **'Failed to load booking requests.'**
  String get failedToLoadBookingRequests;

  /// No description provided for @failedToBookTrip.
  ///
  /// In en, this message translates to:
  /// **'Failed to book trip.'**
  String get failedToBookTrip;

  /// No description provided for @failedToWithdrawBooking.
  ///
  /// In en, this message translates to:
  /// **'Failed to withdraw booking.'**
  String get failedToWithdrawBooking;

  /// No description provided for @failedToLoadVehicleTypes.
  ///
  /// In en, this message translates to:
  /// **'Failed to load vehicle types.'**
  String get failedToLoadVehicleTypes;

  /// No description provided for @failedToLoadNotifications.
  ///
  /// In en, this message translates to:
  /// **'Failed to load notifications.'**
  String get failedToLoadNotifications;

  /// No description provided for @failedToLoadMore.
  ///
  /// In en, this message translates to:
  /// **'Failed to load more.'**
  String get failedToLoadMore;

  /// No description provided for @failedToMarkAsRead.
  ///
  /// In en, this message translates to:
  /// **'Failed to mark as read.'**
  String get failedToMarkAsRead;

  /// No description provided for @registrationFailed.
  ///
  /// In en, this message translates to:
  /// **'Registration failed.'**
  String get registrationFailed;

  /// No description provided for @invalidResponse.
  ///
  /// In en, this message translates to:
  /// **'Invalid response. Please try again.'**
  String get invalidResponse;

  /// No description provided for @error400.
  ///
  /// In en, this message translates to:
  /// **'Invalid request. Please check your input.'**
  String get error400;

  /// No description provided for @error401.
  ///
  /// In en, this message translates to:
  /// **'Invalid or expired session. Please sign in again.'**
  String get error401;

  /// No description provided for @error404.
  ///
  /// In en, this message translates to:
  /// **'Resource not found.'**
  String get error404;

  /// No description provided for @error405.
  ///
  /// In en, this message translates to:
  /// **'This action is not allowed.'**
  String get error405;

  /// No description provided for @error422.
  ///
  /// In en, this message translates to:
  /// **'The given data was invalid.'**
  String get error422;

  /// No description provided for @error500.
  ///
  /// In en, this message translates to:
  /// **'Server error. Please try again later.'**
  String get error500;

  /// No description provided for @errorDefault.
  ///
  /// In en, this message translates to:
  /// **'Something went wrong. Please try again.'**
  String get errorDefault;

  /// No description provided for @continuee.
  ///
  /// In en, this message translates to:
  /// **'Continue'**
  String get continuee;

  /// No description provided for @trackYourPackageEasily.
  ///
  /// In en, this message translates to:
  /// **'Track your package easily'**
  String get trackYourPackageEasily;

  /// No description provided for @trackYourPackageEasilyDesc.
  ///
  /// In en, this message translates to:
  /// **'Enter your package ID and get real-time updates on your shipment’s journey.'**
  String get trackYourPackageEasilyDesc;

  /// No description provided for @realtimeNotifications.
  ///
  /// In en, this message translates to:
  /// **'Realtime notifications'**
  String get realtimeNotifications;

  /// No description provided for @realtimeNotificationsDesc.
  ///
  /// In en, this message translates to:
  /// **'Get a realtime notification about your package status. You can customize the notification easily.'**
  String get realtimeNotificationsDesc;

  /// No description provided for @uploadYourDocuments.
  ///
  /// In en, this message translates to:
  /// **'Upload your documents'**
  String get uploadYourDocuments;

  /// No description provided for @contractorInformation.
  ///
  /// In en, this message translates to:
  /// **'Contractor information'**
  String get contractorInformation;

  /// No description provided for @fullName.
  ///
  /// In en, this message translates to:
  /// **'Full Name'**
  String get fullName;

  /// No description provided for @addressDetails.
  ///
  /// In en, this message translates to:
  /// **'Address details'**
  String get addressDetails;

  /// No description provided for @uploadImageOrBrowse.
  ///
  /// In en, this message translates to:
  /// **'Upload Image or browse'**
  String get uploadImageOrBrowse;

  /// No description provided for @pngJpgUpTo.
  ///
  /// In en, this message translates to:
  /// **'PNG, JPG up to 10MB'**
  String get pngJpgUpTo;

  /// No description provided for @registerConfirm.
  ///
  /// In en, this message translates to:
  /// **'I confirm that all personal information provided is correct and I agree to the terms and conditions.'**
  String get registerConfirm;

  /// No description provided for @yourRequestSuccessfullyRegistered.
  ///
  /// In en, this message translates to:
  /// **'Your request successfully registered'**
  String get yourRequestSuccessfullyRegistered;

  /// No description provided for @yourRequestSuccessfullyRegisteredDesc.
  ///
  /// In en, this message translates to:
  /// **'We’ll check your documents. and open your account to receive trips from loadly very soon, maybe checking your paper takes 2 days'**
  String get yourRequestSuccessfullyRegisteredDesc;

  /// No description provided for @goToHomepage.
  ///
  /// In en, this message translates to:
  /// **'Go to homepage'**
  String get goToHomepage;

  /// No description provided for @market.
  ///
  /// In en, this message translates to:
  /// **'Market'**
  String get market;

  /// No description provided for @marketScreenTitle.
  ///
  /// In en, this message translates to:
  /// **'The market'**
  String get marketScreenTitle;

  /// No description provided for @marketScreenHeaderDesc.
  ///
  /// In en, this message translates to:
  /// **'Shop spare parts, tires, and equipment for your fleet.'**
  String get marketScreenHeaderDesc;

  /// No description provided for @marketComingSoonTitle.
  ///
  /// In en, this message translates to:
  /// **'Coming soon'**
  String get marketComingSoonTitle;

  /// No description provided for @marketComingSoonBody.
  ///
  /// In en, this message translates to:
  /// **'A marketplace will soon be launched.'**
  String get marketComingSoonBody;

  /// No description provided for @marketHomePageButton.
  ///
  /// In en, this message translates to:
  /// **'Home page'**
  String get marketHomePageButton;

  /// No description provided for @account.
  ///
  /// In en, this message translates to:
  /// **'Account'**
  String get account;

  /// No description provided for @enterPasswordEnableBiometric.
  ///
  /// In en, this message translates to:
  /// **'Enter your password to enable biometric login.'**
  String get enterPasswordEnableBiometric;

  /// No description provided for @biometric.
  ///
  /// In en, this message translates to:
  /// **'Biometric'**
  String get biometric;

  /// No description provided for @enableBiometric.
  ///
  /// In en, this message translates to:
  /// **'Would you like to enable biometric login for faster access?'**
  String get enableBiometric;

  /// No description provided for @biometricDesc.
  ///
  /// In en, this message translates to:
  /// **'Enable to finger print and Pin to open the app.'**
  String get biometricDesc;

  /// No description provided for @languages.
  ///
  /// In en, this message translates to:
  /// **'Languages'**
  String get languages;

  /// No description provided for @helpAndCenter.
  ///
  /// In en, this message translates to:
  /// **'Help and center'**
  String get helpAndCenter;

  /// No description provided for @helpAndCenterDesc.
  ///
  /// In en, this message translates to:
  /// **'Chat or call with our support team for assistance.'**
  String get helpAndCenterDesc;

  /// No description provided for @logoutDesc.
  ///
  /// In en, this message translates to:
  /// **'Please enable the biometric or your password to make it easy for login again.'**
  String get logoutDesc;

  /// No description provided for @edit.
  ///
  /// In en, this message translates to:
  /// **'Edit'**
  String get edit;

  /// No description provided for @saveChanges.
  ///
  /// In en, this message translates to:
  /// **'Save Changes'**
  String get saveChanges;

  /// No description provided for @back.
  ///
  /// In en, this message translates to:
  /// **'Back'**
  String get back;

  /// No description provided for @totalTripsCount.
  ///
  /// In en, this message translates to:
  /// **'Total trips count'**
  String get totalTripsCount;

  /// No description provided for @unpaidAmount.
  ///
  /// In en, this message translates to:
  /// **'Unpaid amount'**
  String get unpaidAmount;

  /// No description provided for @personalInformation.
  ///
  /// In en, this message translates to:
  /// **'Personal information'**
  String get personalInformation;

  /// No description provided for @shipping.
  ///
  /// In en, this message translates to:
  /// **'Shipping'**
  String get shipping;

  /// No description provided for @completedTripsDue.
  ///
  /// In en, this message translates to:
  /// **'Completed trips due'**
  String get completedTripsDue;

  /// No description provided for @completedTripsDueUntilNow.
  ///
  /// In en, this message translates to:
  /// **'Completed trips due until now'**
  String get completedTripsDueUntilNow;

  /// No description provided for @tripsAndFinancialDue.
  ///
  /// In en, this message translates to:
  /// **'Trips and financial due'**
  String get tripsAndFinancialDue;

  /// No description provided for @contactSupport.
  ///
  /// In en, this message translates to:
  /// **'Contact support'**
  String get contactSupport;

  /// No description provided for @trackTheTrip.
  ///
  /// In en, this message translates to:
  /// **'Track the trip'**
  String get trackTheTrip;

  /// No description provided for @trackTheTripDesc.
  ///
  /// In en, this message translates to:
  /// **'Enter your trip ID and get the realtime tracking.'**
  String get trackTheTripDesc;

  /// No description provided for @enterTripID.
  ///
  /// In en, this message translates to:
  /// **'Enter trip ID'**
  String get enterTripID;

  /// No description provided for @tripRequest.
  ///
  /// In en, this message translates to:
  /// **'Trip request'**
  String get tripRequest;

  /// No description provided for @tripsRequest.
  ///
  /// In en, this message translates to:
  /// **'Trips request'**
  String get tripsRequest;

  /// No description provided for @tripsRequestDesc.
  ///
  /// In en, this message translates to:
  /// **'All trips request you have.'**
  String get tripsRequestDesc;

  /// No description provided for @tripsReview.
  ///
  /// In en, this message translates to:
  /// **'Trips review'**
  String get tripsReview;

  /// No description provided for @myTripsReview.
  ///
  /// In en, this message translates to:
  /// **'Trips review'**
  String get myTripsReview;

  /// No description provided for @completedTrips.
  ///
  /// In en, this message translates to:
  /// **'Completed trips'**
  String get completedTrips;

  /// No description provided for @showAll.
  ///
  /// In en, this message translates to:
  /// **'Show All'**
  String get showAll;

  /// No description provided for @loadlyAdminSendThisTripFromAgo.
  ///
  /// In en, this message translates to:
  /// **'A Shail admin sent this trip {time} ago.'**
  String loadlyAdminSendThisTripFromAgo(String time);

  /// No description provided for @tripDate.
  ///
  /// In en, this message translates to:
  /// **'Trip date'**
  String get tripDate;

  /// No description provided for @tripFrom.
  ///
  /// In en, this message translates to:
  /// **'Trip from'**
  String get tripFrom;

  /// No description provided for @tripTo.
  ///
  /// In en, this message translates to:
  /// **'Trip to'**
  String get tripTo;

  /// No description provided for @vehicleType.
  ///
  /// In en, this message translates to:
  /// **'Vehicle type'**
  String get vehicleType;

  /// No description provided for @tripCost.
  ///
  /// In en, this message translates to:
  /// **'Trip cost'**
  String get tripCost;

  /// No description provided for @tripValue.
  ///
  /// In en, this message translates to:
  /// **'Value'**
  String get tripValue;

  /// No description provided for @waitingForSupervisorApproval.
  ///
  /// In en, this message translates to:
  /// **'Waiting for supervisor approval'**
  String get waitingForSupervisorApproval;

  /// No description provided for @editRequest.
  ///
  /// In en, this message translates to:
  /// **'Edit request'**
  String get editRequest;

  /// No description provided for @cannotEditThisTrip.
  ///
  /// In en, this message translates to:
  /// **'This trip cannot be edited'**
  String get cannotEditThisTrip;

  /// No description provided for @confirmCancelTrip.
  ///
  /// In en, this message translates to:
  /// **'Are you sure you want to cancel this trip?'**
  String get confirmCancelTrip;

  /// No description provided for @editRequestUpdatedTitle.
  ///
  /// In en, this message translates to:
  /// **'Request updated'**
  String get editRequestUpdatedTitle;

  /// No description provided for @editRequestUpdatedMessage.
  ///
  /// In en, this message translates to:
  /// **'Your booking request has been updated successfully'**
  String get editRequestUpdatedMessage;

  /// No description provided for @cancelTrip.
  ///
  /// In en, this message translates to:
  /// **'Cancel trip'**
  String get cancelTrip;

  /// No description provided for @bookTrip.
  ///
  /// In en, this message translates to:
  /// **'Book trip'**
  String get bookTrip;

  /// No description provided for @requestTripBooking.
  ///
  /// In en, this message translates to:
  /// **'Request trip booking'**
  String get requestTripBooking;

  /// No description provided for @selectDriver.
  ///
  /// In en, this message translates to:
  /// **'Select driver'**
  String get selectDriver;

  /// No description provided for @searchForDriver.
  ///
  /// In en, this message translates to:
  /// **'Search for a driver'**
  String get searchForDriver;

  /// No description provided for @noDrivers.
  ///
  /// In en, this message translates to:
  /// **'No drivers'**
  String get noDrivers;

  /// No description provided for @chooseThisDriver.
  ///
  /// In en, this message translates to:
  /// **'Choose this driver'**
  String get chooseThisDriver;

  /// No description provided for @chooseDriverPlaceholder.
  ///
  /// In en, this message translates to:
  /// **'Choose driver'**
  String get chooseDriverPlaceholder;

  /// No description provided for @driversFromYourList.
  ///
  /// In en, this message translates to:
  /// **'Drivers from your list'**
  String get driversFromYourList;

  /// No description provided for @tapToAddNewDriver.
  ///
  /// In en, this message translates to:
  /// **'Tap here to add a new driver'**
  String get tapToAddNewDriver;

  /// No description provided for @orAddNewDriver.
  ///
  /// In en, this message translates to:
  /// **'Or add a new driver'**
  String get orAddNewDriver;

  /// No description provided for @addNewDriver.
  ///
  /// In en, this message translates to:
  /// **'Add new driver'**
  String get addNewDriver;

  /// No description provided for @addDriver.
  ///
  /// In en, this message translates to:
  /// **'Add driver'**
  String get addDriver;

  /// No description provided for @driverInformation.
  ///
  /// In en, this message translates to:
  /// **'Driver information'**
  String get driverInformation;

  /// No description provided for @driverDocumentsHint.
  ///
  /// In en, this message translates to:
  /// **'ID, driving license, and vehicle license'**
  String get driverDocumentsHint;

  /// No description provided for @pleaseUploadRequiredDocuments.
  ///
  /// In en, this message translates to:
  /// **'Please upload the required documents'**
  String get pleaseUploadRequiredDocuments;

  /// No description provided for @invalidNationalId.
  ///
  /// In en, this message translates to:
  /// **'Invalid national ID'**
  String get invalidNationalId;

  /// No description provided for @pickupPoint.
  ///
  /// In en, this message translates to:
  /// **'Pickup point'**
  String get pickupPoint;

  /// No description provided for @dropoffPoint.
  ///
  /// In en, this message translates to:
  /// **'Drop-off point'**
  String get dropoffPoint;

  /// No description provided for @tripBookedTitle.
  ///
  /// In en, this message translates to:
  /// **'This trip has been booked for you.'**
  String get tripBookedTitle;

  /// No description provided for @tripBookedUpdateTitle.
  ///
  /// In en, this message translates to:
  /// **'Your booking request has been updated'**
  String get tripBookedUpdateTitle;

  /// No description provided for @tripBookedMessage.
  ///
  /// In en, this message translates to:
  /// **'Wait for the representative to approve this trip for you.'**
  String get tripBookedMessage;

  /// No description provided for @pleaseSelectDriver.
  ///
  /// In en, this message translates to:
  /// **'Please select a driver first'**
  String get pleaseSelectDriver;

  /// No description provided for @favoriteList.
  ///
  /// In en, this message translates to:
  /// **'Favorite list'**
  String get favoriteList;

  /// No description provided for @preview.
  ///
  /// In en, this message translates to:
  /// **'Preview'**
  String get preview;

  /// No description provided for @newTripRequestsCount.
  ///
  /// In en, this message translates to:
  /// **'New trip requests ({count})'**
  String newTripRequestsCount(String count);

  /// No description provided for @allYourTripRequests.
  ///
  /// In en, this message translates to:
  /// **'All your trip requests'**
  String get allYourTripRequests;

  /// No description provided for @allYourBookedTripRequests.
  ///
  /// In en, this message translates to:
  /// **'All transfer requests you applied for are waiting for supervisor approval'**
  String get allYourBookedTripRequests;

  /// No description provided for @notStarted.
  ///
  /// In en, this message translates to:
  /// **'Not started'**
  String get notStarted;

  /// No description provided for @finishedTransfers.
  ///
  /// In en, this message translates to:
  /// **'Finished trips'**
  String get finishedTransfers;

  /// No description provided for @cancellations.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get cancellations;

  /// No description provided for @requestedTransfers.
  ///
  /// In en, this message translates to:
  /// **'Requested transfers'**
  String get requestedTransfers;

  /// No description provided for @homeNewTripBadge.
  ///
  /// In en, this message translates to:
  /// **'NEW'**
  String get homeNewTripBadge;

  /// No description provided for @bookedTransfers.
  ///
  /// In en, this message translates to:
  /// **'Booked transfers'**
  String get bookedTransfers;

  /// No description provided for @requestedTransfersCount.
  ///
  /// In en, this message translates to:
  /// **'Requested transfers ({count})'**
  String requestedTransfersCount(String count);

  /// No description provided for @bookedTransfersCount.
  ///
  /// In en, this message translates to:
  /// **'Booked transfers ({count})'**
  String bookedTransfersCount(String count);

  /// No description provided for @priceInEgp.
  ///
  /// In en, this message translates to:
  /// **'{amount} EGP'**
  String priceInEgp(String amount);

  /// No description provided for @all.
  ///
  /// In en, this message translates to:
  /// **'All'**
  String get all;

  /// No description provided for @requested.
  ///
  /// In en, this message translates to:
  /// **'Requested'**
  String get requested;

  /// No description provided for @pending.
  ///
  /// In en, this message translates to:
  /// **'Pending'**
  String get pending;

  /// No description provided for @scheduled.
  ///
  /// In en, this message translates to:
  /// **'Scheduled'**
  String get scheduled;

  /// No description provided for @inProgress.
  ///
  /// In en, this message translates to:
  /// **'In progress'**
  String get inProgress;

  /// No description provided for @completed.
  ///
  /// In en, this message translates to:
  /// **'Completed'**
  String get completed;

  /// No description provided for @tripCompleted.
  ///
  /// In en, this message translates to:
  /// **'Completed'**
  String get tripCompleted;

  /// No description provided for @tripWasCancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get tripWasCancelled;

  /// No description provided for @delivered.
  ///
  /// In en, this message translates to:
  /// **'Delivered'**
  String get delivered;

  /// No description provided for @loadingCargo.
  ///
  /// In en, this message translates to:
  /// **'Loading'**
  String get loadingCargo;

  /// No description provided for @reviewed.
  ///
  /// In en, this message translates to:
  /// **'Reviewed'**
  String get reviewed;

  /// No description provided for @cancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get cancelled;

  /// No description provided for @draft.
  ///
  /// In en, this message translates to:
  /// **'Draft'**
  String get draft;

  /// No description provided for @tripInProgress.
  ///
  /// In en, this message translates to:
  /// **'Trip in progress'**
  String get tripInProgress;

  /// No description provided for @newRequests.
  ///
  /// In en, this message translates to:
  /// **'New requests'**
  String get newRequests;

  /// No description provided for @alreadyBooked.
  ///
  /// In en, this message translates to:
  /// **'Already booked'**
  String get alreadyBooked;

  /// No description provided for @searchForTrip.
  ///
  /// In en, this message translates to:
  /// **'Search for trip'**
  String get searchForTrip;

  /// No description provided for @filter.
  ///
  /// In en, this message translates to:
  /// **'Filter'**
  String get filter;

  /// No description provided for @applyFilter.
  ///
  /// In en, this message translates to:
  /// **'Apply filter'**
  String get applyFilter;

  /// No description provided for @clearFilter.
  ///
  /// In en, this message translates to:
  /// **'Clear filter'**
  String get clearFilter;

  /// No description provided for @searchForPlace.
  ///
  /// In en, this message translates to:
  /// **'Search for place'**
  String get searchForPlace;

  /// No description provided for @searchForPlaceDesc.
  ///
  /// In en, this message translates to:
  /// **'Search for a city or region with available trips that you can access.'**
  String get searchForPlaceDesc;

  /// No description provided for @movingFrom.
  ///
  /// In en, this message translates to:
  /// **'Moving from'**
  String get movingFrom;

  /// No description provided for @confirm.
  ///
  /// In en, this message translates to:
  /// **'Confirm'**
  String get confirm;

  /// No description provided for @skip.
  ///
  /// In en, this message translates to:
  /// **'Skip'**
  String get skip;

  /// No description provided for @pleaseConfirmYourPassword.
  ///
  /// In en, this message translates to:
  /// **'Please confirm your password'**
  String get pleaseConfirmYourPassword;

  /// No description provided for @passwordChangedSuccessfully.
  ///
  /// In en, this message translates to:
  /// **'Password changed successfully'**
  String get passwordChangedSuccessfully;

  /// No description provided for @tripDetailsNumber.
  ///
  /// In en, this message translates to:
  /// **'Trip Details #{id}'**
  String tripDetailsNumber(String id);

  /// No description provided for @notDue.
  ///
  /// In en, this message translates to:
  /// **'Not due'**
  String get notDue;

  /// No description provided for @financialDetails.
  ///
  /// In en, this message translates to:
  /// **'Financial details'**
  String get financialDetails;

  /// No description provided for @driverDetails.
  ///
  /// In en, this message translates to:
  /// **'Driver details'**
  String get driverDetails;

  /// No description provided for @tripDispatchTime.
  ///
  /// In en, this message translates to:
  /// **'Trip dispatch time'**
  String get tripDispatchTime;

  /// No description provided for @tripAcceptanceTime.
  ///
  /// In en, this message translates to:
  /// **'Trip acceptance time'**
  String get tripAcceptanceTime;

  /// No description provided for @elapsedTripDuration.
  ///
  /// In en, this message translates to:
  /// **'Elapsed trip duration'**
  String get elapsedTripDuration;

  /// No description provided for @distanceInKm.
  ///
  /// In en, this message translates to:
  /// **'Distance (km)'**
  String get distanceInKm;

  /// No description provided for @kmCount.
  ///
  /// In en, this message translates to:
  /// **'{count} km'**
  String kmCount(String count);

  /// No description provided for @notes.
  ///
  /// In en, this message translates to:
  /// **'Notes'**
  String get notes;

  /// No description provided for @noNotes.
  ///
  /// In en, this message translates to:
  /// **'None'**
  String get noNotes;

  /// No description provided for @otherDiscounts.
  ///
  /// In en, this message translates to:
  /// **'Other discounts'**
  String get otherDiscounts;

  /// No description provided for @hoursCount.
  ///
  /// In en, this message translates to:
  /// **'{count} {unit}'**
  String hoursCount(String count, String unit);

  /// No description provided for @changeDriver.
  ///
  /// In en, this message translates to:
  /// **'Change driver'**
  String get changeDriver;

  /// No description provided for @chooseNewDriver.
  ///
  /// In en, this message translates to:
  /// **'Choose new driver'**
  String get chooseNewDriver;

  /// No description provided for @cancelBooking.
  ///
  /// In en, this message translates to:
  /// **'Cancel booking'**
  String get cancelBooking;

  /// No description provided for @confirmChanges.
  ///
  /// In en, this message translates to:
  /// **'Confirm changes'**
  String get confirmChanges;

  /// No description provided for @assignTheDriver.
  ///
  /// In en, this message translates to:
  /// **'Assign the driver'**
  String get assignTheDriver;

  /// No description provided for @chooseDriver.
  ///
  /// In en, this message translates to:
  /// **'Choose driver'**
  String get chooseDriver;

  /// No description provided for @yourSupervisorHasApprovedTheTripPleaseAddDriver.
  ///
  /// In en, this message translates to:
  /// **'Your supervisor has approved the trip, please add a driver.'**
  String get yourSupervisorHasApprovedTheTripPleaseAddDriver;

  /// No description provided for @ifDriverIsNotAppointedWithinTheTripWillBeSentToAnotherContractor.
  ///
  /// In en, this message translates to:
  /// **'If a driver is not appointed within ??, the trip will be sent to another contractor.'**
  String get ifDriverIsNotAppointedWithinTheTripWillBeSentToAnotherContractor;

  /// No description provided for @readyForTrip.
  ///
  /// In en, this message translates to:
  /// **'Ready for trip'**
  String get readyForTrip;

  /// No description provided for @youHaveSuccessfullyAddedTheDriver.
  ///
  /// In en, this message translates to:
  /// **'You have successfully added the driver.'**
  String get youHaveSuccessfullyAddedTheDriver;

  /// No description provided for @youHaveSuccessfullyAddedTheDriverDesc.
  ///
  /// In en, this message translates to:
  /// **'The trip will begin in {date}, so be prepared. If you have any questions, please don\'t hesitate to call the person in charge.'**
  String youHaveSuccessfullyAddedTheDriverDesc(String date);

  /// No description provided for @waitingForManagerApproval.
  ///
  /// In en, this message translates to:
  /// **'Waiting for manager approval'**
  String get waitingForManagerApproval;

  /// Calendar label for the current day
  ///
  /// In en, this message translates to:
  /// **'Today'**
  String get today;

  /// Placeholder when no vehicle type is selected in a filter
  ///
  /// In en, this message translates to:
  /// **'Choose vehicle type'**
  String get chooseVehicleType;

  /// Filter label for trip start date
  ///
  /// In en, this message translates to:
  /// **'Trip start date'**
  String get tripStartDate;

  /// Filter label for trip end date
  ///
  /// In en, this message translates to:
  /// **'Trip end date'**
  String get tripEndDate;

  /// Filter label for trip status
  ///
  /// In en, this message translates to:
  /// **'Trip status'**
  String get tripStatus;

  /// Relative time for notifications within the last minute
  ///
  /// In en, this message translates to:
  /// **'Just now'**
  String get justNow;

  /// Morning meridiem label in English trip date display
  ///
  /// In en, this message translates to:
  /// **'AM'**
  String get timeAm;

  /// Evening meridiem label in English trip date display
  ///
  /// In en, this message translates to:
  /// **'PM'**
  String get timePm;

  /// Morning period label for trip card time in Arabic locale
  ///
  /// In en, this message translates to:
  /// **'AM'**
  String get meridiemAm;

  /// Evening period label for trip card time in Arabic locale
  ///
  /// In en, this message translates to:
  /// **'PM'**
  String get meridiemPm;

  /// Elapsed time when under one minute
  ///
  /// In en, this message translates to:
  /// **'less than a minute'**
  String get lessThanAMinute;

  /// No description provided for @elapsedMinuteUnit.
  ///
  /// In en, this message translates to:
  /// **'minute'**
  String get elapsedMinuteUnit;

  /// No description provided for @elapsedMinutesUnit.
  ///
  /// In en, this message translates to:
  /// **'minutes'**
  String get elapsedMinutesUnit;

  /// No description provided for @elapsedHourUnit.
  ///
  /// In en, this message translates to:
  /// **'hour'**
  String get elapsedHourUnit;

  /// No description provided for @elapsedHoursUnit.
  ///
  /// In en, this message translates to:
  /// **'hours'**
  String get elapsedHoursUnit;

  /// No description provided for @elapsedDayUnit.
  ///
  /// In en, this message translates to:
  /// **'day'**
  String get elapsedDayUnit;

  /// No description provided for @elapsedDaysUnit.
  ///
  /// In en, this message translates to:
  /// **'days'**
  String get elapsedDaysUnit;

  /// No description provided for @elapsedTwoMinutes.
  ///
  /// In en, this message translates to:
  /// **'2 minutes'**
  String get elapsedTwoMinutes;

  /// No description provided for @elapsedTwoHours.
  ///
  /// In en, this message translates to:
  /// **'2 hours'**
  String get elapsedTwoHours;

  /// No description provided for @elapsedTwoDays.
  ///
  /// In en, this message translates to:
  /// **'2 days'**
  String get elapsedTwoDays;

  /// No description provided for @elapsedMinuteSingular.
  ///
  /// In en, this message translates to:
  /// **'minute'**
  String get elapsedMinuteSingular;

  /// No description provided for @elapsedMinutesFew.
  ///
  /// In en, this message translates to:
  /// **'{count} minutes'**
  String elapsedMinutesFew(int count);

  /// No description provided for @elapsedMinutesOther.
  ///
  /// In en, this message translates to:
  /// **'{count} minutes'**
  String elapsedMinutesOther(int count);

  /// No description provided for @elapsedHourSingular.
  ///
  /// In en, this message translates to:
  /// **'hour'**
  String get elapsedHourSingular;

  /// No description provided for @elapsedHoursFew.
  ///
  /// In en, this message translates to:
  /// **'{count} hours'**
  String elapsedHoursFew(int count);

  /// No description provided for @elapsedHoursOther.
  ///
  /// In en, this message translates to:
  /// **'{count} hours'**
  String elapsedHoursOther(int count);

  /// No description provided for @elapsedDaySingular.
  ///
  /// In en, this message translates to:
  /// **'day'**
  String get elapsedDaySingular;

  /// No description provided for @elapsedDaysFew.
  ///
  /// In en, this message translates to:
  /// **'{count} days'**
  String elapsedDaysFew(int count);

  /// No description provided for @elapsedDaysOther.
  ///
  /// In en, this message translates to:
  /// **'{count} days'**
  String elapsedDaysOther(int count);

  /// No description provided for @elapsedMinutesShort.
  ///
  /// In en, this message translates to:
  /// **'{count} min'**
  String elapsedMinutesShort(int count);

  /// No description provided for @priceInArabicEgpFull.
  ///
  /// In en, this message translates to:
  /// **'{amount} EGP'**
  String priceInArabicEgpFull(String amount);

  /// Title when navigation routing fails
  ///
  /// In en, this message translates to:
  /// **'Route Error'**
  String get routeError;

  /// No description provided for @fileSizeKb.
  ///
  /// In en, this message translates to:
  /// **'{size} KB'**
  String fileSizeKb(String size);

  /// No description provided for @fileSizeMb.
  ///
  /// In en, this message translates to:
  /// **'{size} MB'**
  String fileSizeMb(String size);

  /// No description provided for @financialDueAllTripsCount.
  ///
  /// In en, this message translates to:
  /// **'Total trips : {count}'**
  String financialDueAllTripsCount(String count);

  /// No description provided for @dueTripsCountLabel.
  ///
  /// In en, this message translates to:
  /// **'Due trips count'**
  String get dueTripsCountLabel;

  /// No description provided for @amountDueUntilNow.
  ///
  /// In en, this message translates to:
  /// **'Amount due until now'**
  String get amountDueUntilNow;

  /// No description provided for @financialDueAmountDescription.
  ///
  /// In en, this message translates to:
  /// **'The due amount is what you receive from Loadly after submitting the trip documents.'**
  String get financialDueAmountDescription;

  /// No description provided for @allTrips.
  ///
  /// In en, this message translates to:
  /// **'All trips'**
  String get allTrips;

  /// No description provided for @dueTransferTripBadge.
  ///
  /// In en, this message translates to:
  /// **'Due trip'**
  String get dueTransferTripBadge;

  /// No description provided for @entitlementFulfilledBadge.
  ///
  /// In en, this message translates to:
  /// **'Entitlement fulfilled'**
  String get entitlementFulfilledBadge;

  /// No description provided for @transferTripPricePrefix.
  ///
  /// In en, this message translates to:
  /// **'Trip price :'**
  String get transferTripPricePrefix;
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
