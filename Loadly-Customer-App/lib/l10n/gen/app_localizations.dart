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

  /// No description provided for @welcome.
  ///
  /// In en, this message translates to:
  /// **'Welcome'**
  String get welcome;

  /// No description provided for @appTitle.
  ///
  /// In en, this message translates to:
  /// **'Loadly Customer'**
  String get appTitle;

  /// No description provided for @getStarted.
  ///
  /// In en, this message translates to:
  /// **'Get started with your account'**
  String get getStarted;

  /// No description provided for @startAccount.
  ///
  /// In en, this message translates to:
  /// **'Start using your account and explore more'**
  String get startAccount;

  /// No description provided for @rememberMe.
  ///
  /// In en, this message translates to:
  /// **'Remember me'**
  String get rememberMe;

  /// No description provided for @continuee.
  ///
  /// In en, this message translates to:
  /// **'Continue'**
  String get continuee;

  /// No description provided for @back.
  ///
  /// In en, this message translates to:
  /// **'Back'**
  String get back;

  /// No description provided for @createPassword.
  ///
  /// In en, this message translates to:
  /// **'Create your password'**
  String get createPassword;

  /// No description provided for @createPasswordDesc.
  ///
  /// In en, this message translates to:
  /// **'Set a strong password to secure your Shiporia account and keep your shipments safe.'**
  String get createPasswordDesc;

  /// No description provided for @authSuccess.
  ///
  /// In en, this message translates to:
  /// **'Your company profile is ready!'**
  String get authSuccess;

  /// No description provided for @authSuccessDesc.
  ///
  /// In en, this message translates to:
  /// **'You can now start adding couriers, creating delivery schedules, and tracking journeys in real-time.'**
  String get authSuccessDesc;

  /// No description provided for @onboardingDesc.
  ///
  /// In en, this message translates to:
  /// **'Loadly is a modern logistics solution that makes it easy for you to organize trips, track, and manage shipments in real-time.'**
  String get onboardingDesc;

  /// No description provided for @onboardingItem1.
  ///
  /// In en, this message translates to:
  /// **'Real-time Tracking'**
  String get onboardingItem1;

  /// No description provided for @onboardingItem1Desc.
  ///
  /// In en, this message translates to:
  /// **'Monitor every shipment live and stay updated on your delivery status anytime, anywhere.'**
  String get onboardingItem1Desc;

  /// No description provided for @onboardingItem2.
  ///
  /// In en, this message translates to:
  /// **'Smart Delivery Management'**
  String get onboardingItem2;

  /// No description provided for @onboardingItem2Desc.
  ///
  /// In en, this message translates to:
  /// **'Easily manage couriers, routes, and schedules to ensure faster and more efficient deliveries.'**
  String get onboardingItem2Desc;

  /// No description provided for @onboardingItem3.
  ///
  /// In en, this message translates to:
  /// **'Reliable & Secure'**
  String get onboardingItem3;

  /// No description provided for @onboardingItem3Desc.
  ///
  /// In en, this message translates to:
  /// **'Your packages are handled with care, supported by a trusted and secure logistics system.'**
  String get onboardingItem3Desc;

  /// No description provided for @createAccount.
  ///
  /// In en, this message translates to:
  /// **'Create your new account now'**
  String get createAccount;

  /// No description provided for @createAccountDesc.
  ///
  /// In en, this message translates to:
  /// **'Join Loadly today and start managing your shipments with ease. Track, organize, and deliver smarter in just a few clicks.'**
  String get createAccountDesc;

  /// No description provided for @privacyPolicy.
  ///
  /// In en, this message translates to:
  /// **'By continuing, you agree to Shipura Terms of Service and Privacy Policy.'**
  String get privacyPolicy;

  /// No description provided for @realTime.
  ///
  /// In en, this message translates to:
  /// **'Real-time'**
  String get realTime;

  /// No description provided for @profileAccount.
  ///
  /// In en, this message translates to:
  /// **'Profile Account'**
  String get profileAccount;

  /// No description provided for @profileAccountDesc.
  ///
  /// In en, this message translates to:
  /// **'Manage your personal information, preferences, and account settings.'**
  String get profileAccountDesc;

  /// No description provided for @pushNotifications.
  ///
  /// In en, this message translates to:
  /// **'Push Notifications'**
  String get pushNotifications;

  /// No description provided for @pushNotificationsDesc.
  ///
  /// In en, this message translates to:
  /// **'Get instant notifications on your device so you don\'t miss any information.'**
  String get pushNotificationsDesc;

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

  /// No description provided for @enterPasswordEnableBiometric.
  ///
  /// In en, this message translates to:
  /// **'Enter your password to enable biometric login.'**
  String get enterPasswordEnableBiometric;

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

  /// No description provided for @changePhoto.
  ///
  /// In en, this message translates to:
  /// **'Change Photo'**
  String get changePhoto;

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

  /// No description provided for @search.
  ///
  /// In en, this message translates to:
  /// **'Search'**
  String get search;

  /// No description provided for @addTrip.
  ///
  /// In en, this message translates to:
  /// **'Add trip'**
  String get addTrip;

  /// No description provided for @contractNumber.
  ///
  /// In en, this message translates to:
  /// **'Contract number'**
  String get contractNumber;

  /// No description provided for @dateOfTrip.
  ///
  /// In en, this message translates to:
  /// **'Date of trip'**
  String get dateOfTrip;

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

  /// No description provided for @filter.
  ///
  /// In en, this message translates to:
  /// **'Filter'**
  String get filter;

  /// No description provided for @dateRange.
  ///
  /// In en, this message translates to:
  /// **'Date Range'**
  String get dateRange;

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

  /// No description provided for @tripStatus.
  ///
  /// In en, this message translates to:
  /// **'Trip Status'**
  String get tripStatus;

  /// No description provided for @vehicleType.
  ///
  /// In en, this message translates to:
  /// **'Vehicle type'**
  String get vehicleType;

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

  /// No description provided for @applyNow.
  ///
  /// In en, this message translates to:
  /// **'Apply now'**
  String get applyNow;

  /// No description provided for @requestNewTrip.
  ///
  /// In en, this message translates to:
  /// **'Request new trip'**
  String get requestNewTrip;

  /// No description provided for @step.
  ///
  /// In en, this message translates to:
  /// **'Step'**
  String get step;

  /// No description provided for @requestNewTripDesc.
  ///
  /// In en, this message translates to:
  /// **'Please follow and complete each step to finish requesting your trips.'**
  String get requestNewTripDesc;

  /// No description provided for @tripInformation.
  ///
  /// In en, this message translates to:
  /// **'Trip Information'**
  String get tripInformation;

  /// No description provided for @contactName.
  ///
  /// In en, this message translates to:
  /// **'Contact name'**
  String get contactName;

  /// No description provided for @enterContactName.
  ///
  /// In en, this message translates to:
  /// **'Please Enter contact Name'**
  String get enterContactName;

  /// No description provided for @contactNumber.
  ///
  /// In en, this message translates to:
  /// **'Contact number'**
  String get contactNumber;

  /// No description provided for @contractRefNumber.
  ///
  /// In en, this message translates to:
  /// **'Contract ref number'**
  String get contractRefNumber;

  /// No description provided for @pickContractRef.
  ///
  /// In en, this message translates to:
  /// **'Please pick a contract ref number'**
  String get pickContractRef;

  /// No description provided for @pickVehicleType.
  ///
  /// In en, this message translates to:
  /// **'Please pick a Vehicle Type'**
  String get pickVehicleType;

  /// No description provided for @contractDestination.
  ///
  /// In en, this message translates to:
  /// **'Contract destination'**
  String get contractDestination;

  /// No description provided for @distance.
  ///
  /// In en, this message translates to:
  /// **'Distance'**
  String get distance;

  /// No description provided for @km.
  ///
  /// In en, this message translates to:
  /// **'KM'**
  String get km;

  /// No description provided for @addManualDestination.
  ///
  /// In en, this message translates to:
  /// **'Add manual destination'**
  String get addManualDestination;

  /// No description provided for @enterDistance.
  ///
  /// In en, this message translates to:
  /// **'Please enter distance or choose a Contract destination'**
  String get enterDistance;

  /// No description provided for @tripDate.
  ///
  /// In en, this message translates to:
  /// **'Trip Date'**
  String get tripDate;

  /// No description provided for @schedule.
  ///
  /// In en, this message translates to:
  /// **'Schedule'**
  String get schedule;

  /// No description provided for @directly.
  ///
  /// In en, this message translates to:
  /// **'Directly'**
  String get directly;

  /// No description provided for @pickStartDate.
  ///
  /// In en, this message translates to:
  /// **'Please pick trip start date'**
  String get pickStartDate;

  /// No description provided for @pickEndDate.
  ///
  /// In en, this message translates to:
  /// **'Please pick trip end date'**
  String get pickEndDate;

  /// No description provided for @cargo.
  ///
  /// In en, this message translates to:
  /// **'Cargo'**
  String get cargo;

  /// No description provided for @enterCargo.
  ///
  /// In en, this message translates to:
  /// **'Please enter actual cargo weight tons or cargo price'**
  String get enterCargo;

  /// No description provided for @actualCargoWeightTons.
  ///
  /// In en, this message translates to:
  /// **'Actual cargo weight tons'**
  String get actualCargoWeightTons;

  /// No description provided for @cargoPrice.
  ///
  /// In en, this message translates to:
  /// **'Cargo price'**
  String get cargoPrice;

  /// No description provided for @cargoDetails.
  ///
  /// In en, this message translates to:
  /// **'Cargo details'**
  String get cargoDetails;

  /// No description provided for @dimensions.
  ///
  /// In en, this message translates to:
  /// **'Dimensions'**
  String get dimensions;

  /// No description provided for @weight.
  ///
  /// In en, this message translates to:
  /// **'Weight'**
  String get weight;

  /// No description provided for @specialHandlingRequirements.
  ///
  /// In en, this message translates to:
  /// **'Special handling requirements'**
  String get specialHandlingRequirements;

  /// No description provided for @helpers.
  ///
  /// In en, this message translates to:
  /// **'Helpers'**
  String get helpers;

  /// No description provided for @includeHelpers.
  ///
  /// In en, this message translates to:
  /// **'Include helpers'**
  String get includeHelpers;

  /// No description provided for @helpersNumber.
  ///
  /// In en, this message translates to:
  /// **'Helpers number'**
  String get helpersNumber;

  /// No description provided for @pickDropPoints.
  ///
  /// In en, this message translates to:
  /// **'Pickup points & drop-off points'**
  String get pickDropPoints;

  /// No description provided for @chooseThePoint.
  ///
  /// In en, this message translates to:
  /// **'Choose the point'**
  String get chooseThePoint;

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

  /// No description provided for @googleMapLink.
  ///
  /// In en, this message translates to:
  /// **'Google map link'**
  String get googleMapLink;

  /// No description provided for @url.
  ///
  /// In en, this message translates to:
  /// **'Url'**
  String get url;

  /// No description provided for @enterGoogleMapsUrl.
  ///
  /// In en, this message translates to:
  /// **'Please enter a valid google maps url'**
  String get enterGoogleMapsUrl;

  /// No description provided for @address.
  ///
  /// In en, this message translates to:
  /// **'Address'**
  String get address;

  /// No description provided for @enterAddress.
  ///
  /// In en, this message translates to:
  /// **'Please enter the point address'**
  String get enterAddress;

  /// No description provided for @addNote.
  ///
  /// In en, this message translates to:
  /// **'Add note'**
  String get addNote;

  /// No description provided for @note.
  ///
  /// In en, this message translates to:
  /// **'Note'**
  String get note;

  /// No description provided for @noNote.
  ///
  /// In en, this message translates to:
  /// **'No Note'**
  String get noNote;

  /// No description provided for @none.
  ///
  /// In en, this message translates to:
  /// **'None'**
  String get none;

  /// No description provided for @addPickDropPoint.
  ///
  /// In en, this message translates to:
  /// **'Please add atleast 1 pickup and 1 drop-off point'**
  String get addPickDropPoint;

  /// No description provided for @addWaypoint.
  ///
  /// In en, this message translates to:
  /// **'Add waypoint'**
  String get addWaypoint;

  /// No description provided for @delete.
  ///
  /// In en, this message translates to:
  /// **'Delete'**
  String get delete;

  /// No description provided for @deletePoint.
  ///
  /// In en, this message translates to:
  /// **'Delete point'**
  String get deletePoint;

  /// No description provided for @wantToDelete.
  ///
  /// In en, this message translates to:
  /// **'Are you sure you want to delete'**
  String get wantToDelete;

  /// No description provided for @requestTrip.
  ///
  /// In en, this message translates to:
  /// **'request Trip'**
  String get requestTrip;

  /// No description provided for @reviewSubmit.
  ///
  /// In en, this message translates to:
  /// **'Review & Submit'**
  String get reviewSubmit;

  /// No description provided for @tripSuccessfullyRequest.
  ///
  /// In en, this message translates to:
  /// **'Trip successfully requested!'**
  String get tripSuccessfullyRequest;

  /// No description provided for @tripSuccessfullyRequestDesc.
  ///
  /// In en, this message translates to:
  /// **'Your Trip is now Request. and send to loadly to manage your shipments with ease.'**
  String get tripSuccessfullyRequestDesc;

  /// No description provided for @trackingDelivery.
  ///
  /// In en, this message translates to:
  /// **'Tracking delivery'**
  String get trackingDelivery;

  /// No description provided for @from.
  ///
  /// In en, this message translates to:
  /// **'From'**
  String get from;

  /// No description provided for @to.
  ///
  /// In en, this message translates to:
  /// **'To'**
  String get to;

  /// No description provided for @estTime.
  ///
  /// In en, this message translates to:
  /// **'Est. time'**
  String get estTime;

  /// No description provided for @unknown.
  ///
  /// In en, this message translates to:
  /// **'Unknown'**
  String get unknown;

  /// No description provided for @clickToDetails.
  ///
  /// In en, this message translates to:
  /// **'Click to details'**
  String get clickToDetails;

  /// No description provided for @yourTripStarted.
  ///
  /// In en, this message translates to:
  /// **'Your Trip started'**
  String get yourTripStarted;

  /// No description provided for @tripActivity.
  ///
  /// In en, this message translates to:
  /// **'Trip activity'**
  String get tripActivity;

  /// No description provided for @vehicle.
  ///
  /// In en, this message translates to:
  /// **'Vehicle'**
  String get vehicle;

  /// No description provided for @tripDetails.
  ///
  /// In en, this message translates to:
  /// **'Trip Details'**
  String get tripDetails;

  /// No description provided for @noAddress.
  ///
  /// In en, this message translates to:
  /// **'No address'**
  String get noAddress;

  /// No description provided for @head.
  ///
  /// In en, this message translates to:
  /// **'Head'**
  String get head;

  /// No description provided for @carId.
  ///
  /// In en, this message translates to:
  /// **'Car ID'**
  String get carId;

  /// No description provided for @verificationInstruction.
  ///
  /// In en, this message translates to:
  /// **'Enter your phone number and we will send you a verification code in less than a minute.'**
  String get verificationInstruction;

  /// No description provided for @phone.
  ///
  /// In en, this message translates to:
  /// **'Phone number'**
  String get phone;

  /// No description provided for @enterYourPhone.
  ///
  /// In en, this message translates to:
  /// **'Enter your phone number'**
  String get enterYourPhone;

  /// No description provided for @enterValidPhone.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid phone number'**
  String get enterValidPhone;

  /// No description provided for @enterValidPhone11Digits.
  ///
  /// In en, this message translates to:
  /// **'Enter 11 digits'**
  String get enterValidPhone11Digits;

  /// No description provided for @enterValidPhone01.
  ///
  /// In en, this message translates to:
  /// **'Phone number must start with 01'**
  String get enterValidPhone01;

  /// No description provided for @signInToAccount.
  ///
  /// In en, this message translates to:
  /// **'Sign in to your account'**
  String get signInToAccount;

  /// No description provided for @email.
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get email;

  /// No description provided for @password.
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get password;

  /// No description provided for @signIn.
  ///
  /// In en, this message translates to:
  /// **'Sign in'**
  String get signIn;

  /// No description provided for @signUp.
  ///
  /// In en, this message translates to:
  /// **'Sign up'**
  String get signUp;

  /// No description provided for @error.
  ///
  /// In en, this message translates to:
  /// **'Error'**
  String get error;

  /// No description provided for @ok.
  ///
  /// In en, this message translates to:
  /// **'OK'**
  String get ok;

  /// No description provided for @enterYourEmail.
  ///
  /// In en, this message translates to:
  /// **'Enter your email'**
  String get enterYourEmail;

  /// No description provided for @enterValidEmail.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid email'**
  String get enterValidEmail;

  /// No description provided for @enterYourPassword.
  ///
  /// In en, this message translates to:
  /// **'Enter your password'**
  String get enterYourPassword;

  /// No description provided for @home.
  ///
  /// In en, this message translates to:
  /// **'Home'**
  String get home;

  /// No description provided for @forgetPassword.
  ///
  /// In en, this message translates to:
  /// **'Forgot your password?'**
  String get forgetPassword;

  /// No description provided for @verificationTitle.
  ///
  /// In en, this message translates to:
  /// **'Verification Code'**
  String get verificationTitle;

  /// No description provided for @verificationSubtitle.
  ///
  /// In en, this message translates to:
  /// **'We will send you a verification code, you can choose to send it to your email or as a text message to your mobile?'**
  String get verificationSubtitle;

  /// No description provided for @verificationSubtitlePhone.
  ///
  /// In en, this message translates to:
  /// **'We will send you a verification code via text message to your phone.'**
  String get verificationSubtitlePhone;

  /// No description provided for @sendViaSms.
  ///
  /// In en, this message translates to:
  /// **'Send code via text message'**
  String get sendViaSms;

  /// No description provided for @sendViaEmail.
  ///
  /// In en, this message translates to:
  /// **'Send code via email'**
  String get sendViaEmail;

  /// No description provided for @continueButton.
  ///
  /// In en, this message translates to:
  /// **'Log In'**
  String get continueButton;

  /// No description provided for @otpInstructionSms.
  ///
  /// In en, this message translates to:
  /// **'We sent you a verification code via text message to your number ({phone}), enter it here to complete your account activation.'**
  String otpInstructionSms(String phone);

  /// No description provided for @otpInstructionEmail.
  ///
  /// In en, this message translates to:
  /// **'We sent you a verification code via email to {email}, enter it here to complete your account activation.'**
  String otpInstructionEmail(String email);

  /// No description provided for @resendPrompt.
  ///
  /// In en, this message translates to:
  /// **'Didn\'t receive a message yet?'**
  String get resendPrompt;

  /// No description provided for @resendLink.
  ///
  /// In en, this message translates to:
  /// **'Send again'**
  String get resendLink;

  /// No description provided for @verifyAccount.
  ///
  /// In en, this message translates to:
  /// **'Verify Account'**
  String get verifyAccount;

  /// No description provided for @verifyEmail.
  ///
  /// In en, this message translates to:
  /// **'Verify Email'**
  String get verifyEmail;

  /// No description provided for @verifyEmailInstruction.
  ///
  /// In en, this message translates to:
  /// **'Enter the 6-digit code we sent to your email to verify your account.'**
  String get verifyEmailInstruction;

  /// No description provided for @enterVerificationCode.
  ///
  /// In en, this message translates to:
  /// **'Enter the verification code'**
  String get enterVerificationCode;

  /// No description provided for @verificationCodeLength.
  ///
  /// In en, this message translates to:
  /// **'Code must be 6 digits'**
  String get verificationCodeLength;

  /// No description provided for @verify.
  ///
  /// In en, this message translates to:
  /// **'Verify'**
  String get verify;

  /// No description provided for @resendCode.
  ///
  /// In en, this message translates to:
  /// **'Resend code'**
  String get resendCode;

  /// No description provided for @codeSentToEmail.
  ///
  /// In en, this message translates to:
  /// **'Verification code sent to your email.'**
  String get codeSentToEmail;

  /// No description provided for @newPassword.
  ///
  /// In en, this message translates to:
  /// **'New password'**
  String get newPassword;

  /// No description provided for @confirmPassword.
  ///
  /// In en, this message translates to:
  /// **'Confirm password'**
  String get confirmPassword;

  /// No description provided for @resetPassword.
  ///
  /// In en, this message translates to:
  /// **'Reset password'**
  String get resetPassword;

  /// No description provided for @passwordMinLength.
  ///
  /// In en, this message translates to:
  /// **'Password must be at least 6 characters'**
  String get passwordMinLength;

  /// No description provided for @personalAccount.
  ///
  /// In en, this message translates to:
  /// **'Personal Account'**
  String get personalAccount;

  /// No description provided for @employeeDriverName.
  ///
  /// In en, this message translates to:
  /// **'Employee/Driver Name'**
  String get employeeDriverName;

  /// No description provided for @emailLabel.
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get emailLabel;

  /// No description provided for @rating.
  ///
  /// In en, this message translates to:
  /// **'Rating'**
  String get rating;

  /// No description provided for @trip.
  ///
  /// In en, this message translates to:
  /// **'Trip'**
  String get trip;

  /// No description provided for @trips.
  ///
  /// In en, this message translates to:
  /// **'Trips'**
  String get trips;

  /// No description provided for @files.
  ///
  /// In en, this message translates to:
  /// **'Files'**
  String get files;

  /// No description provided for @myPersonalData.
  ///
  /// In en, this message translates to:
  /// **'My Personal Data'**
  String get myPersonalData;

  /// No description provided for @fileName.
  ///
  /// In en, this message translates to:
  /// **'File Name'**
  String get fileName;

  /// No description provided for @chooseFromGallery.
  ///
  /// In en, this message translates to:
  /// **'Choose from Gallery'**
  String get chooseFromGallery;

  /// No description provided for @profilePage.
  ///
  /// In en, this message translates to:
  /// **'Profile Page'**
  String get profilePage;

  /// No description provided for @notifications.
  ///
  /// In en, this message translates to:
  /// **'Notifications'**
  String get notifications;

  /// No description provided for @tripsNav.
  ///
  /// In en, this message translates to:
  /// **'Trips'**
  String get tripsNav;

  /// No description provided for @homeNav.
  ///
  /// In en, this message translates to:
  /// **'Home'**
  String get homeNav;

  /// No description provided for @secondsRemaining.
  ///
  /// In en, this message translates to:
  /// **'{seconds} seconds remaining'**
  String secondsRemaining(int seconds);

  /// No description provided for @loginWithPassword.
  ///
  /// In en, this message translates to:
  /// **'Login with password'**
  String get loginWithPassword;

  /// No description provided for @loginWithBiometric.
  ///
  /// In en, this message translates to:
  /// **'Login with Biometric'**
  String get loginWithBiometric;

  /// No description provided for @or.
  ///
  /// In en, this message translates to:
  /// **'OR'**
  String get or;

  /// No description provided for @backToLogin.
  ///
  /// In en, this message translates to:
  /// **'Back to login'**
  String get backToLogin;

  /// No description provided for @online.
  ///
  /// In en, this message translates to:
  /// **'Online'**
  String get online;

  /// No description provided for @offline.
  ///
  /// In en, this message translates to:
  /// **'Offline'**
  String get offline;

  /// No description provided for @logout.
  ///
  /// In en, this message translates to:
  /// **'Logout'**
  String get logout;

  /// No description provided for @changePassword.
  ///
  /// In en, this message translates to:
  /// **'Change Password'**
  String get changePassword;

  /// No description provided for @enableFingerprint.
  ///
  /// In en, this message translates to:
  /// **'Enable Fingerprint'**
  String get enableFingerprint;

  /// No description provided for @useFingerprint.
  ///
  /// In en, this message translates to:
  /// **'Use fingerprint'**
  String get useFingerprint;

  /// No description provided for @enable2FA.
  ///
  /// In en, this message translates to:
  /// **'Enable 2FA'**
  String get enable2FA;

  /// No description provided for @support.
  ///
  /// In en, this message translates to:
  /// **'Support'**
  String get support;

  /// No description provided for @settings.
  ///
  /// In en, this message translates to:
  /// **'Settings'**
  String get settings;

  /// No description provided for @cancel.
  ///
  /// In en, this message translates to:
  /// **'Cancel'**
  String get cancel;

  /// No description provided for @passwordsDoNotMatch.
  ///
  /// In en, this message translates to:
  /// **'Passwords do not match'**
  String get passwordsDoNotMatch;

  /// No description provided for @fullName.
  ///
  /// In en, this message translates to:
  /// **'Full Name'**
  String get fullName;

  /// No description provided for @fullNameEnglish.
  ///
  /// In en, this message translates to:
  /// **'Full Name (English)'**
  String get fullNameEnglish;

  /// No description provided for @fullNameArabic.
  ///
  /// In en, this message translates to:
  /// **'Full Name (Arabic)'**
  String get fullNameArabic;

  /// No description provided for @userName.
  ///
  /// In en, this message translates to:
  /// **'User Name'**
  String get userName;

  /// No description provided for @save.
  ///
  /// In en, this message translates to:
  /// **'Save'**
  String get save;

  /// No description provided for @profileUpdatedSuccessfully.
  ///
  /// In en, this message translates to:
  /// **'Profile updated successfully'**
  String get profileUpdatedSuccessfully;

  /// No description provided for @passwordChangedSuccessfully.
  ///
  /// In en, this message translates to:
  /// **'Password changed successfully'**
  String get passwordChangedSuccessfully;

  /// No description provided for @imagePickerNotImplemented.
  ///
  /// In en, this message translates to:
  /// **'Image picker not implemented yet'**
  String get imagePickerNotImplemented;

  /// No description provided for @retry.
  ///
  /// In en, this message translates to:
  /// **'Retry'**
  String get retry;

  /// No description provided for @user.
  ///
  /// In en, this message translates to:
  /// **'User'**
  String get user;

  /// No description provided for @cameraPermissionRequired.
  ///
  /// In en, this message translates to:
  /// **'Camera permission is required to take photos. Please enable it in Settings.'**
  String get cameraPermissionRequired;

  /// No description provided for @photoLibraryPermissionRequired.
  ///
  /// In en, this message translates to:
  /// **'Photo library permission is required to select photos. Please enable it in Settings.'**
  String get photoLibraryPermissionRequired;

  /// No description provided for @permissionDenied.
  ///
  /// In en, this message translates to:
  /// **'Permission Denied'**
  String get permissionDenied;

  /// No description provided for @openSettings.
  ///
  /// In en, this message translates to:
  /// **'Open Settings'**
  String get openSettings;

  /// No description provided for @takePhoto.
  ///
  /// In en, this message translates to:
  /// **'Take Photo'**
  String get takePhoto;

  /// No description provided for @todayNotifications.
  ///
  /// In en, this message translates to:
  /// **'Today\'s Notifications'**
  String get todayNotifications;

  /// No description provided for @companyNotification.
  ///
  /// In en, this message translates to:
  /// **'Company Notification'**
  String get companyNotification;

  /// No description provided for @newTripSent.
  ///
  /// In en, this message translates to:
  /// **'A new trip has been sent, click to view details'**
  String get newTripSent;

  /// No description provided for @retrievalNotificationSent.
  ///
  /// In en, this message translates to:
  /// **'A retrieval notification has been sent to the company awaiting your reply'**
  String get retrievalNotificationSent;

  /// No description provided for @replacementCarOnWay.
  ///
  /// In en, this message translates to:
  /// **'A replacement car is on its way to you'**
  String get replacementCarOnWay;

  /// No description provided for @vehicleDetails.
  ///
  /// In en, this message translates to:
  /// **'Vehicle Details'**
  String get vehicleDetails;

  /// No description provided for @minsAgo.
  ///
  /// In en, this message translates to:
  /// **'{mins} mins ago'**
  String minsAgo(int mins);

  /// Time ago in hours
  ///
  /// In en, this message translates to:
  /// **'{hours} hours ago'**
  String hoursAgo(int hours);

  /// No description provided for @startTrip.
  ///
  /// In en, this message translates to:
  /// **'Start Trip'**
  String get startTrip;

  /// No description provided for @startShippingAndLoading.
  ///
  /// In en, this message translates to:
  /// **'Start Shipping and Loading'**
  String get startShippingAndLoading;

  /// No description provided for @tripStartsOn.
  ///
  /// In en, this message translates to:
  /// **'Trip starts on: {date} at {time}'**
  String tripStartsOn(String date, String time);

  /// No description provided for @startLoadingShipment.
  ///
  /// In en, this message translates to:
  /// **'Start loading shipment'**
  String get startLoadingShipment;

  /// No description provided for @pickupLocation.
  ///
  /// In en, this message translates to:
  /// **'Pickup Location'**
  String get pickupLocation;

  /// No description provided for @deliveryLocation.
  ///
  /// In en, this message translates to:
  /// **'Delivery Location'**
  String get deliveryLocation;

  /// No description provided for @shipmentType.
  ///
  /// In en, this message translates to:
  /// **'Shipment Type'**
  String get shipmentType;

  /// No description provided for @tripDuration.
  ///
  /// In en, this message translates to:
  /// **'Trip Duration'**
  String get tripDuration;

  /// No description provided for @shipmentDimensions.
  ///
  /// In en, this message translates to:
  /// **'Shipment Dimensions'**
  String get shipmentDimensions;

  /// No description provided for @approximately.
  ///
  /// In en, this message translates to:
  /// **'Approximately'**
  String get approximately;

  /// No description provided for @hours.
  ///
  /// In en, this message translates to:
  /// **'Hours'**
  String get hours;

  /// No description provided for @stopPoints.
  ///
  /// In en, this message translates to:
  /// **'Stop Points'**
  String get stopPoints;

  /// No description provided for @stop.
  ///
  /// In en, this message translates to:
  /// **'Stop'**
  String get stop;

  /// No description provided for @clientDetails.
  ///
  /// In en, this message translates to:
  /// **'Client Details'**
  String get clientDetails;

  /// No description provided for @phoneNumber.
  ///
  /// In en, this message translates to:
  /// **'Phone Number'**
  String get phoneNumber;

  /// No description provided for @responsiblePersonName.
  ///
  /// In en, this message translates to:
  /// **'Responsible Person Name'**
  String get responsiblePersonName;

  /// No description provided for @isThereAProblemCallSupport.
  ///
  /// In en, this message translates to:
  /// **'Is there a problem? Call Support'**
  String get isThereAProblemCallSupport;

  /// No description provided for @clickToConfirm.
  ///
  /// In en, this message translates to:
  /// **'Click to Confirm'**
  String get clickToConfirm;

  /// No description provided for @tripDetailsPlaceholder.
  ///
  /// In en, this message translates to:
  /// **'Details are written here'**
  String get tripDetailsPlaceholder;

  /// No description provided for @noTripsAvailable.
  ///
  /// In en, this message translates to:
  /// **'No trips available'**
  String get noTripsAvailable;

  /// No description provided for @loadingTrips.
  ///
  /// In en, this message translates to:
  /// **'Loading trips...'**
  String get loadingTrips;

  /// No description provided for @errorLoadingTrips.
  ///
  /// In en, this message translates to:
  /// **'Error loading trips'**
  String get errorLoadingTrips;

  /// No description provided for @noNotifications.
  ///
  /// In en, this message translates to:
  /// **'No notifications'**
  String get noNotifications;

  /// No description provided for @loadMore.
  ///
  /// In en, this message translates to:
  /// **'Load More'**
  String get loadMore;

  /// No description provided for @filterByDate.
  ///
  /// In en, this message translates to:
  /// **'Filter by Date'**
  String get filterByDate;

  /// No description provided for @apply.
  ///
  /// In en, this message translates to:
  /// **'Apply'**
  String get apply;

  /// No description provided for @tripStarted.
  ///
  /// In en, this message translates to:
  /// **'Driver started the trip.'**
  String get tripStarted;

  /// No description provided for @waypointArrived.
  ///
  /// In en, this message translates to:
  /// **'Driver arrived at the location.'**
  String get waypointArrived;

  /// No description provided for @waypointLodaingStarted.
  ///
  /// In en, this message translates to:
  /// **'Driver started loading at the location.'**
  String get waypointLodaingStarted;

  /// No description provided for @waypointLodaingCompleted.
  ///
  /// In en, this message translates to:
  /// **'Loading completed at the location.'**
  String get waypointLodaingCompleted;

  /// No description provided for @waypointDeparted.
  ///
  /// In en, this message translates to:
  /// **'Driver departed the location.'**
  String get waypointDeparted;

  /// No description provided for @waypointUnloadingStarted.
  ///
  /// In en, this message translates to:
  /// **'Driver started unloading at the location.'**
  String get waypointUnloadingStarted;

  /// No description provided for @waypointUnloadingCompleted.
  ///
  /// In en, this message translates to:
  /// **'Unloading completed at the location.'**
  String get waypointUnloadingCompleted;

  /// No description provided for @tripCompleted.
  ///
  /// In en, this message translates to:
  /// **'Trip completed successfully at the final location.'**
  String get tripCompleted;

  /// No description provided for @companyName.
  ///
  /// In en, this message translates to:
  /// **'Company name'**
  String get companyName;

  /// No description provided for @noTripActivity.
  ///
  /// In en, this message translates to:
  /// **'No trip activity yet!'**
  String get noTripActivity;

  /// No description provided for @noDriver.
  ///
  /// In en, this message translates to:
  /// **'No driver'**
  String get noDriver;

  /// No description provided for @noVehicle.
  ///
  /// In en, this message translates to:
  /// **'No vehicle yet!'**
  String get noVehicle;

  /// No description provided for @almost.
  ///
  /// In en, this message translates to:
  /// **'Almost'**
  String get almost;

  /// No description provided for @saveAsDraft.
  ///
  /// In en, this message translates to:
  /// **'Save as a draft'**
  String get saveAsDraft;

  /// No description provided for @saveTripAsDraft.
  ///
  /// In en, this message translates to:
  /// **'Save trip as a draft'**
  String get saveTripAsDraft;

  /// No description provided for @saveTripAsDraftDesc.
  ///
  /// In en, this message translates to:
  /// **'able to edit to continue the trip request from trip management list in any time'**
  String get saveTripAsDraftDesc;

  /// No description provided for @discard.
  ///
  /// In en, this message translates to:
  /// **'Discard'**
  String get discard;

  /// No description provided for @prominentDisclosureNotNow.
  ///
  /// In en, this message translates to:
  /// **'Not now'**
  String get prominentDisclosureNotNow;

  /// No description provided for @prominentDisclosureContinue.
  ///
  /// In en, this message translates to:
  /// **'Continue'**
  String get prominentDisclosureContinue;

  /// No description provided for @cameraPermissionDisclosureTitle.
  ///
  /// In en, this message translates to:
  /// **'Camera access'**
  String get cameraPermissionDisclosureTitle;

  /// No description provided for @cameraPermissionDisclosureBody.
  ///
  /// In en, this message translates to:
  /// **'Loadly Customer uses the camera only when you choose to take a photo—for example for your profile picture or required documents. Photos you take are uploaded to our servers for your company account. Tap Continue to open the system permission request.'**
  String get cameraPermissionDisclosureBody;

  /// No description provided for @photosPermissionDisclosureTitle.
  ///
  /// In en, this message translates to:
  /// **'Photos and files'**
  String get photosPermissionDisclosureTitle;

  /// No description provided for @photosPermissionDisclosureBody.
  ///
  /// In en, this message translates to:
  /// **'Loadly Customer accesses images you select from your photo library or device storage to upload profile or document images. We only access files you choose. Tap Continue to open the system permission request.'**
  String get photosPermissionDisclosureBody;

  /// No description provided for @otherTrips.
  ///
  /// In en, this message translates to:
  /// **'Other trips'**
  String get otherTrips;

  /// No description provided for @allTrips.
  ///
  /// In en, this message translates to:
  /// **'All Trips'**
  String get allTrips;

  /// No description provided for @tripsMetrics.
  ///
  /// In en, this message translates to:
  /// **'Trips Metrics'**
  String get tripsMetrics;

  /// No description provided for @kilometres.
  ///
  /// In en, this message translates to:
  /// **'Kilometres'**
  String get kilometres;

  /// No description provided for @year.
  ///
  /// In en, this message translates to:
  /// **'Year'**
  String get year;

  /// No description provided for @month.
  ///
  /// In en, this message translates to:
  /// **'Month'**
  String get month;

  /// No description provided for @lastMonth.
  ///
  /// In en, this message translates to:
  /// **'Last month'**
  String get lastMonth;

  /// No description provided for @lastWeek.
  ///
  /// In en, this message translates to:
  /// **'Last week'**
  String get lastWeek;

  /// No description provided for @reports.
  ///
  /// In en, this message translates to:
  /// **'Reports'**
  String get reports;

  /// No description provided for @reportsDesc.
  ///
  /// In en, this message translates to:
  /// **'All reports for your trips cost and invoices tracking is here.'**
  String get reportsDesc;

  /// No description provided for @previewReport.
  ///
  /// In en, this message translates to:
  /// **'Preview report'**
  String get previewReport;

  /// No description provided for @costTrackingPerTrip.
  ///
  /// In en, this message translates to:
  /// **'Cost tracking per trip'**
  String get costTrackingPerTrip;

  /// No description provided for @costTrackingPerTripDesc.
  ///
  /// In en, this message translates to:
  /// **'Stay on top of your travel expenses! Get detailed reports and track costs for each trip, ensuring transparency and helping you stay within budget.'**
  String get costTrackingPerTripDesc;

  /// No description provided for @invoiceAndBillingInformation.
  ///
  /// In en, this message translates to:
  /// **'Invoice and billing information'**
  String get invoiceAndBillingInformation;

  /// No description provided for @invoiceAndBillingInformationDesc.
  ///
  /// In en, this message translates to:
  /// **'View and manage all your invoices and billing details in one place. Easily track payments, outstanding balances, and payment history for streamlined financial management.'**
  String get invoiceAndBillingInformationDesc;

  /// No description provided for @financialTransactionHistory.
  ///
  /// In en, this message translates to:
  /// **'Financial transaction history'**
  String get financialTransactionHistory;

  /// No description provided for @financialTransactionHistoryDesc.
  ///
  /// In en, this message translates to:
  /// **'Review your complete financial records. Monitor all transactions, deposits, and withdrawals, ensuring accuracy and control over your finances.'**
  String get financialTransactionHistoryDesc;

  /// No description provided for @egp.
  ///
  /// In en, this message translates to:
  /// **'EGP'**
  String get egp;

  /// No description provided for @costLast.
  ///
  /// In en, this message translates to:
  /// **'Cost last'**
  String get costLast;

  /// No description provided for @day.
  ///
  /// In en, this message translates to:
  /// **'day'**
  String get day;

  /// No description provided for @days.
  ///
  /// In en, this message translates to:
  /// **'days'**
  String get days;

  /// No description provided for @months.
  ///
  /// In en, this message translates to:
  /// **'months'**
  String get months;

  /// No description provided for @tripId.
  ///
  /// In en, this message translates to:
  /// **'Trip ID'**
  String get tripId;

  /// No description provided for @vs.
  ///
  /// In en, this message translates to:
  /// **'vs'**
  String get vs;

  /// No description provided for @paidInvoices.
  ///
  /// In en, this message translates to:
  /// **'Paid invoices'**
  String get paidInvoices;

  /// No description provided for @and.
  ///
  /// In en, this message translates to:
  /// **'&'**
  String get and;

  /// No description provided for @unpaidInvoices.
  ///
  /// In en, this message translates to:
  /// **'unpaid invoices'**
  String get unpaidInvoices;

  /// No description provided for @unpaid.
  ///
  /// In en, this message translates to:
  /// **'unpaid'**
  String get unpaid;

  /// No description provided for @partiallyPaid.
  ///
  /// In en, this message translates to:
  /// **'Partially paid'**
  String get partiallyPaid;

  /// No description provided for @paid.
  ///
  /// In en, this message translates to:
  /// **'Paid'**
  String get paid;

  /// No description provided for @paidInvoice.
  ///
  /// In en, this message translates to:
  /// **'Paid'**
  String get paidInvoice;

  /// No description provided for @status.
  ///
  /// In en, this message translates to:
  /// **'status'**
  String get status;

  /// No description provided for @remaining.
  ///
  /// In en, this message translates to:
  /// **'Remaining'**
  String get remaining;

  /// No description provided for @totalAmount.
  ///
  /// In en, this message translates to:
  /// **'Total amount'**
  String get totalAmount;

  /// No description provided for @totalPaid.
  ///
  /// In en, this message translates to:
  /// **'Total paid'**
  String get totalPaid;

  /// No description provided for @remainingBalance.
  ///
  /// In en, this message translates to:
  /// **'Remaining balance'**
  String get remainingBalance;

  /// No description provided for @invoiceId.
  ///
  /// In en, this message translates to:
  /// **'Invoice ID'**
  String get invoiceId;

  /// No description provided for @dueDate.
  ///
  /// In en, this message translates to:
  /// **'Due Date'**
  String get dueDate;

  /// No description provided for @action.
  ///
  /// In en, this message translates to:
  /// **'Action'**
  String get action;

  /// No description provided for @noInvoiceAvailable.
  ///
  /// In en, this message translates to:
  /// **'No invoices available'**
  String get noInvoiceAvailable;

  /// No description provided for @includedContract.
  ///
  /// In en, this message translates to:
  /// **'Included Contract'**
  String get includedContract;

  /// No description provided for @contract.
  ///
  /// In en, this message translates to:
  /// **'Contract'**
  String get contract;

  /// No description provided for @addons.
  ///
  /// In en, this message translates to:
  /// **'Addons'**
  String get addons;

  /// No description provided for @subTotal.
  ///
  /// In en, this message translates to:
  /// **'Sub total'**
  String get subTotal;

  /// No description provided for @total.
  ///
  /// In en, this message translates to:
  /// **'Total'**
  String get total;

  /// No description provided for @discount.
  ///
  /// In en, this message translates to:
  /// **'Discount'**
  String get discount;

  /// No description provided for @tax.
  ///
  /// In en, this message translates to:
  /// **'Tax'**
  String get tax;

  /// No description provided for @totalPrice.
  ///
  /// In en, this message translates to:
  /// **'Total price'**
  String get totalPrice;

  /// No description provided for @requestDate.
  ///
  /// In en, this message translates to:
  /// **'Request date'**
  String get requestDate;

  /// No description provided for @invoicePaymentRequest.
  ///
  /// In en, this message translates to:
  /// **'Invoice payment request'**
  String get invoicePaymentRequest;

  /// No description provided for @invoiceNumber.
  ///
  /// In en, this message translates to:
  /// **'Invoice number'**
  String get invoiceNumber;

  /// No description provided for @amount.
  ///
  /// In en, this message translates to:
  /// **'Amount'**
  String get amount;

  /// No description provided for @attachment.
  ///
  /// In en, this message translates to:
  /// **'Attachment'**
  String get attachment;

  /// No description provided for @download.
  ///
  /// In en, this message translates to:
  /// **'Download'**
  String get download;

  /// No description provided for @received.
  ///
  /// In en, this message translates to:
  /// **'Received'**
  String get received;

  /// No description provided for @rejected.
  ///
  /// In en, this message translates to:
  /// **'Rejected'**
  String get rejected;

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
  /// **'A new version of Loadly Customer is required to continue. Please update to the latest version.'**
  String get appUpdateRequiredMessage;

  /// Default message for optional update prompt
  ///
  /// In en, this message translates to:
  /// **'A new version of Loadly Customer is available with improvements and fixes.'**
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

  /// No description provided for @contractExcludedSolarPrice.
  ///
  /// In en, this message translates to:
  /// **'Contract Excluded Solar Price'**
  String get contractExcludedSolarPrice;

  /// No description provided for @contractIncludedSolarPrice.
  ///
  /// In en, this message translates to:
  /// **'Contract Included Solar Price'**
  String get contractIncludedSolarPrice;

  /// No description provided for @contractOnDemand.
  ///
  /// In en, this message translates to:
  /// **'Contract On Demand'**
  String get contractOnDemand;

  /// No description provided for @seasonalContract.
  ///
  /// In en, this message translates to:
  /// **'Seasonal Contract'**
  String get seasonalContract;

  /// No description provided for @biometricAccountInfoUnavailable.
  ///
  /// In en, this message translates to:
  /// **'Could not get your account info. Please try again.'**
  String get biometricAccountInfoUnavailable;

  /// No description provided for @now.
  ///
  /// In en, this message translates to:
  /// **'Now'**
  String get now;
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
