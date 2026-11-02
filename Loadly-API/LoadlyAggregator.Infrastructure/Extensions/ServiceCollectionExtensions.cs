using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using LoadlyAggregator.Domain.Common.Constants;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;
using LoadlyAggregator.Infrastructure.Authorization;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Infrastructure.Data.Seeders;
using LoadlyAggregator.Infrastructure.HealthChecks;
using LoadlyAggregator.Infrastructure.Repositories.Implementations;
using LoadlyAggregator.Infrastructure.Services.Auth;
using LoadlyAggregator.Infrastructure.Services.Media;
using LoadlyAggregator.Infrastructure.Services.Media.StorageProviders;
using LoadlyAggregator.Infrastructure.Services.Queue;
using LoadlyAggregator.Infrastructure.Services.Sms.SmsProviders;

namespace LoadlyAggregator.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(
                serviceProvider.GetRequiredService<Infrastructure.Data.Interceptors.AuditableTimestampInterceptor>(),
                serviceProvider.GetRequiredService<Infrastructure.Data.Interceptors.AuditLogInterceptor>());
        });


        var logsConnectionString = configuration.GetConnectionString("LogsConnection")
            ?? throw new InvalidOperationException("Connection string 'LogsConnection' not found.");

        services.AddDbContext<LogsDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(logsConnectionString);
            options.AddInterceptors(
                serviceProvider.GetRequiredService<Infrastructure.Data.Interceptors.AuditableTimestampInterceptor>());
        });


        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyDetailRepository, CompanyDetailRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IContractorRepository, ContractorRepository>();
        services.AddScoped<IContractorDocumentRepository, ContractorDocumentRepository>();
        services.AddScoped<ICollectorRepository, CollectorRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IGovernerateRepository, GovernerateRepository>();
        services.AddScoped<ISectorRepository, SectorRepository>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<ICompanyDocumentRepository, CompanyDocumentRepository>();
        services.AddScoped<IContractDocumentRepository, ContractDocumentRepository>();
        services.AddScoped<IIndustryTypeRepository, IndustryTypeRepository>();
        services.AddScoped<ICommunicationMethodRepository, CommunicationMethodRepository>();
        services.AddScoped<IServiceRequirementRepository, ServiceRequirementRepository>();
        services.AddScoped<IVehicleTypeRepository, VehicleTypeRepository>();
        services.AddScoped<IDriverLicenseTypeRepository, DriverLicenseTypeRepository>();
        services.AddScoped<IDriverDetailRepository, DriverDetailRepository>();
        services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
        services.AddScoped<IDriverDocumentRepository, DriverDocumentRepository>();
        services.AddScoped<ITaxRepository, TaxRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<IColorRepository, ColorRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IFuelTypeRepository, FuelTypeRepository>();
        services.AddScoped<IContractExtraFeeRepository, ContractExtraFeeRepository>();
        services.AddScoped<IWaypointRepository, WaypointRepository>();
        services.AddScoped<IContractDestinationRepository, ContractDestinationRepository>();
        services.AddScoped<IContractVehicleTypeRepository, ContractVehicleTypeRepository>();
        services.AddScoped<IContractSlotRepository, ContractSlotRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IContractBillingSnapshotRepository, ContractBillingSnapshotRepository>();
        services.AddScoped<IContractApprovalRepository, ContractApprovalRepository>();
        services.AddScoped<IContractStatusHistoryRepository, ContractStatusHistoryRepository>();
        services.AddScoped<IVehicleAssignmentHistoryRepository, VehicleAssignmentHistoryRepository>();
        services.AddScoped<Domain.Common.Services.VehicleAssignmentHistoryRecorder>();
        services.AddScoped<ITripVehicleAssignmentSnapshotRepository, TripVehicleAssignmentSnapshotRepository>();
        services.AddScoped<IVehiclePlateAliasRepository, VehiclePlateAliasRepository>();
        services.AddScoped<Domain.Common.Services.TripVehicleAssignmentSnapshotRecorder>();
        services.AddScoped<Domain.Common.Services.VehiclePlateAliasRecorder>();
        services.AddScoped<IDocumentStatusRepository, DocumentStatusRepository>();
        services.AddScoped<IVehicleCapacityUnitRepository, VehicleCapacityUnitRepository>();
        services.AddScoped<IPortRepository, PortRepository>();
        services.AddScoped<IContainerRepository, ContainerRepository>();
        services.AddScoped<IShippingLineRepository, ShippingLineRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IVehicleInsuranceRepository, VehicleInsuranceRepository>();
        services.AddScoped<IVehicleFacilityRepository, VehicleFacilityRepository>();
        services.AddScoped<IVehicleFacilityVisitRepository, VehicleFacilityVisitRepository>();
        services.AddScoped<IVehicleBrandRepository, VehicleBrandRepository>();
        services.AddScoped<IVehicleBrandModelRepository, VehicleBrandModelRepository>();
        services.AddScoped<IVehicleDocumentRepository, VehicleDocumentRepository>();
        services.AddScoped<IVehicleDetailRepository, VehicleDetailRepository>();
        services.AddScoped<IOperatingHoursRepository, OperatingHoursRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IInvoiceDetailRepository, InvoiceDetailRepository>();
        services.AddScoped<IContractorPayoutRepository, ContractorPayoutRepository>();
        services.AddScoped<IInvoicePaymentRepository, InvoicePaymentRepository>();
        services.AddScoped<IBankTransferRepository, BankTransferRepository>();
        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<ITripBookingInvitationRepository, TripBookingInvitationRepository>();
        services.AddScoped<ITripBookingRequestRepository, TripBookingRequestRepository>();
        services.AddScoped<ITripBookingInvitationRoundRepository, TripBookingInvitationRoundRepository>();
        services.AddScoped<ITripDocumentRepository, TripDocumentRepository>();
        services.AddScoped<ITripChargeRepository, TripChargeRepository>();
        services.AddScoped<ITargetRepository, TargetRepository>();
        services.AddScoped<ITripWaypointRepository, TripWaypointRepository>();
        services.AddScoped<ITripContractDestinationRepository, TripContractDestinationRepository>();
        services.AddScoped<ITripRouteSyncRepository, TripRouteSyncRepository>();
        services.AddScoped<IRecordingRepository, RecordingRepository>();
        services.AddScoped<INotificationsLogRepository, NotificationsLogRepository>();
        services.AddScoped<Domain.Common.Interfaces.INotificationRepository, Infrastructure.Repositories.Implementations.NotificationRepository>();
        services.AddScoped<Domain.Common.Interfaces.INotificationPreferenceRepository, Infrastructure.Repositories.Implementations.NotificationPreferenceRepository>();
        services.AddScoped<Domain.Common.Interfaces.IBroadcastNotificationRepository, Infrastructure.Repositories.Implementations.BroadcastNotificationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IOdometerScanLogRepository, OdometerScanLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleHasPermissionRepository, RoleHasPermissionRepository>();
        services.AddScoped<IModelHasRoleRepository, ModelHasRoleRepository>();
        services.AddScoped<IModelHasPermissionRepository, ModelHasPermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
        services.AddScoped<IGeneralSettingRepository, GeneralSettingRepository>();


        services.AddScoped<Domain.Common.Interfaces.ICurrentUserService, Infrastructure.Services.CurrentUser.CurrentUserService>();
        services.AddScoped<Domain.Common.Interfaces.IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<Infrastructure.Services.Auth.IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<Domain.Common.Interfaces.IJwtTokenService, JwtTokenService>();
        services.AddScoped<Domain.Common.Interfaces.IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<Domain.Common.Interfaces.IPasswordResetTokenService, PasswordResetTokenService>();
        services.AddScoped<Domain.Common.Interfaces.IResetPasswordUrlService, ResetPasswordUrlService>();
        services.AddScoped<Domain.Common.Interfaces.IMfaCodeService, MfaCodeService>();
        services.AddScoped<Domain.Common.Interfaces.ILanguageService, Infrastructure.Services.LanguageService>();
        services.Configure<Infrastructure.Options.SmtpOptions>(
            configuration.GetSection(Infrastructure.Options.SmtpOptions.SectionName));
        services.Configure<Infrastructure.Options.OpenAiOptions>(
            configuration.GetSection(Infrastructure.Options.OpenAiOptions.SectionName));
        services.Configure<Infrastructure.Options.GoogleMapsOptions>(
            configuration.GetSection(Infrastructure.Options.GoogleMapsOptions.SectionName));
        services.AddHttpClient("googlemaps", static client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        services.AddScoped<Domain.Common.Interfaces.IGoogleMapsPlatformService, Infrastructure.Services.Maps.GoogleMapsPlatformService>();
        services.AddHttpClient("openai", static client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/");
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        RegisterOdometerReadingExtractor(services, configuration);
        services.AddScoped<Domain.Common.Interfaces.IQueueNameResolver, Infrastructure.Services.Queue.QueueNameResolver>();
        services.AddScoped<Domain.Common.Interfaces.INotificationPreferenceService, Infrastructure.Services.Notification.NotificationPreferenceService>();
        services.Configure<Infrastructure.Options.NotificationDeduplicationOptions>(
            configuration.GetSection(Infrastructure.Options.NotificationDeduplicationOptions.SectionName));
        services.AddScoped<Domain.Common.Interfaces.INotificationDeduplicationService, Infrastructure.Services.Notification.NotificationDeduplicationService>();
        services.AddScoped<Domain.Common.Interfaces.IEmailSender, Infrastructure.Services.Notification.EmailSender>();
        services.AddScoped<Domain.Common.Interfaces.ISmsSender, Infrastructure.Services.Notification.SmsSender>();
        services.AddScoped<Domain.Common.Interfaces.IPushSender, Infrastructure.Services.Notification.PushSender>();
        services.AddScoped<Domain.Common.Interfaces.INotificationTransport, Infrastructure.Services.Notification.NotificationTransportService>();
        services.AddSingleton<Infrastructure.Services.Localization.ResourceCatalogProvider>();
        services.AddScoped<Domain.Common.Interfaces.INotificationTemplateService, Infrastructure.Services.Notification.NotificationTemplateService>();
        services.AddScoped<Domain.Common.Interfaces.IEmailTemplateBuilder, Infrastructure.Services.Notification.EmailTemplateBuilder>();
        services.AddScoped<Domain.Common.Interfaces.INotificationStore, Infrastructure.Services.Notification.NotificationStore>();
        services.AddScoped<Domain.Common.Interfaces.ITranslationService, Infrastructure.Services.Translation.TranslationService>();


        RegisterStorageProvider(services, configuration);


        services.AddScoped<Domain.Common.Interfaces.IMediaService, MediaService>();
        services.AddScoped<Domain.Common.Interfaces.IGoogleMapsPlaceExtractor, Infrastructure.Services.Maps.GoogleMapsPlaceExtractor>();
        services.AddScoped<Domain.Common.Interfaces.IExcelParseService, Infrastructure.Services.ExcelImport.ExcelParseService>();

        RegisterSmsProvider(services, configuration);

        services.AddScoped<Domain.Common.Interfaces.IEnumService, Infrastructure.Services.EnumService>();


        services.AddScoped<Domain.Common.Services.Mfa.IMfaMethodStrategy, Domain.Common.Services.Mfa.EmailMfaStrategy>();
        services.AddScoped<Domain.Common.Services.Mfa.IMfaMethodStrategy, Domain.Common.Services.Mfa.SmsMfaStrategy>();
        services.AddScoped<Domain.Common.Services.Mfa.MfaMethodStrategyFactory>();


        services.AddScoped<Domain.Common.Services.Auth.AuthService>();


        services.AddScoped<Domain.Admin.Services.IAdminNotificationService, Domain.Admin.Services.AdminNotificationService>();
        services.AddScoped<Domain.Admin.Services.IContractorNotificationService, Domain.Admin.Services.ContractorNotificationService>();
        services.AddScoped<Domain.Admin.Services.ICollectorNotificationService, Domain.Admin.Services.CollectorNotificationService>();
        services.AddScoped<Domain.Company.Services.ICompanyUserNotificationService, Domain.Company.Services.CompanyUserNotificationService>();
        services.AddScoped<Domain.Common.Notifications.INotificationPipeline, Infrastructure.Services.Notification.NotificationPipeline>();
        // Audience builders — reusable recipient-construction logic shared across handlers.
        services.AddScoped<Infrastructure.Services.Notification.AudienceBuilders.ICompanyEmailAudienceBuilder, Infrastructure.Services.Notification.AudienceBuilders.CompanyEmailAudienceBuilder>();
        services.AddScoped<Infrastructure.Services.Notification.AudienceBuilders.IAdminPermissionAudienceBuilder, Infrastructure.Services.Notification.AudienceBuilders.AdminPermissionAudienceBuilder>();
        services.AddScoped<Infrastructure.Services.Notification.AudienceBuilders.ICompanyUsersAudienceBuilder, Infrastructure.Services.Notification.AudienceBuilders.CompanyUsersAudienceBuilder>();
        services.AddScoped<Infrastructure.Services.Notification.AudienceBuilders.ICreatorAdminAudienceBuilder, Infrastructure.Services.Notification.AudienceBuilders.CreatorAdminAudienceBuilder>();

        // Typed handlers — one per NotificationData subclass.
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripRequestedByCompanyNotificationData>, Infrastructure.Services.Notification.Handlers.TripRequestedHandler>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripScheduledNotificationData>, Infrastructure.Services.Notification.Handlers.TripScheduledHandler>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripCancelledNotificationData>, Infrastructure.Services.Notification.Handlers.TripCancelledHandler>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripCompletedByDriverNotificationData>, Infrastructure.Services.Notification.Handlers.TripCompletedHandler>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripProgressNotificationData>, Infrastructure.Services.Notification.Handlers.TripProgressHandler>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripPushedToContractorsNotificationData>, Infrastructure.Services.Notification.Handlers.TripPushedToContractorsHandler>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripBookingRequestSubmittedNotificationData>, Infrastructure.Services.Notification.Handlers.TripBookingRequestSubmittedHandler>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripBookingRequestApprovedNotificationData>, Infrastructure.Services.Notification.Handlers.TripBookingRequestApprovedHandler>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandler<Domain.Common.Notifications.EventData.TripContractorUnassignedNotificationData>, Infrastructure.Services.Notification.Handlers.TripContractorUnassignedHandler>();

        // Non-generic adapters — admin resolves these without reflection.
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripRequestedByCompanyNotificationData>>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripScheduledNotificationData>>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripCancelledNotificationData>>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripCompletedByDriverNotificationData>>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripProgressNotificationData>>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripPushedToContractorsNotificationData>>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripBookingRequestSubmittedNotificationData>>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripBookingRequestApprovedNotificationData>>();
        services.AddScoped<Domain.Common.Notifications.INotificationHandlerAdapter,
            Infrastructure.Services.Notification.NotificationHandlerAdapter<Domain.Common.Notifications.EventData.TripContractorUnassignedNotificationData>>();

        services.AddScoped<Domain.Common.Notifications.INotificationEventDispatcher, Infrastructure.Services.Notification.NotificationDispatcher>();
        services.AddScoped<Domain.Admin.Services.IContractApprovalPipeline, Domain.Admin.Services.DefaultContractApprovalPipeline>();
        services.AddScoped<Domain.Admin.Services.IContractStateMachine, Domain.Admin.Services.ContractStateMachine>();


        // Single source of "now" for the entire app. Use TimeProvider.System in production;
        // tests replace with FakeTimeProvider to control time deterministically.
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<Infrastructure.Data.Interceptors.AuditLogInterceptor>();
        services.AddSingleton<Infrastructure.Data.Interceptors.AuditableTimestampInterceptor>();


        services.AddMemoryCache();
        services.AddScoped<Domain.Common.Interfaces.IPermissionService, Infrastructure.Services.Permissions.PermissionService>();


        services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, PermissionAuthorizationHandler>();


        var redisConnection = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string not found.");
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "Loadly_";
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();
            logger.LogInformation("Connecting application Redis at: {ConnectionString}", redisConnection);

            return ConnectionMultiplexer.Connect(RedisConnectionOptionsFactory.Create(redisConnection));
        });


        services.AddScoped<IPasswordResetCacheService, PasswordResetCacheService>();


        services.AddScoped<Domain.Common.Interfaces.IMfaCacheService, MfaCacheService>();


        RegisterRedisQueue(services, configuration);


        services.AddScoped<DataSeeder>();
        services.AddScoped<PermissionLoader>();


        services.AddScoped<Domain.Admin.Actions.Auth.AdminLoginAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminForgetPasswordAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminResetPasswordAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminResetPasswordWithCodeAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminChangePasswordAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminEnableMfaAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminDisableMfaAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminSendMfaCodeAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminVerifyMfaAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminRefreshTokenAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminResendVerificationAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminVerifyEmailAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminVerifyPhoneAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminGetProfileAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminUpdateProfileAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminListSessionsAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminRevokeSessionAction>();
        services.AddScoped<Domain.Admin.Actions.Auth.AdminValidatePasswordAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyLoginAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyLoginWithOtpAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanySendLoginOtpAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyForgetPasswordAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyResetPasswordAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyResetPasswordWithCodeAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyChangePasswordAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyEnableMfaAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyDisableMfaAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanySendMfaCodeAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyVerifyMfaAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyRefreshTokenAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyResendVerificationAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyVerifyEmailAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyVerifyPhoneAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyGetProfileAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyUpdateProfileAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyListSessionsAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyRevokeSessionAction>();
        services.AddScoped<Domain.Company.Actions.Auth.CompanyValidatePasswordAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyGetTripAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyListTripsAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyCreateTripAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyUpdateTripAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyDeleteTripAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyCancelTripAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyDownloadTripDocumentAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyTrackTripsAction>();
        services.AddScoped<Domain.Company.Actions.Trips.CompanyBulkCreateTripsAction>();
        services.AddScoped<Domain.Company.Actions.Dashboard.CompanyGetDashboardMonthlyTripsAction>();
        services.AddScoped<Domain.Company.Actions.Dashboard.CompanyGetTripsStatisticsAction>();
        services.AddScoped<Domain.Company.Actions.Dashboard.CompanyGetTripCostPeriodTotalsAction>();
        services.AddScoped<Domain.Company.Actions.Dashboard.CompanyGetInvoiceStatisticsAction>();
        services.AddScoped<Domain.Company.Actions.Dashboard.CompanyGetHomeDashboardAction>();
        services.AddScoped<Domain.Company.Actions.Users.CreateCompanyUserAction>();
        services.AddScoped<Domain.Company.Actions.Users.GetCompanyUserAction>();
        services.AddScoped<Domain.Company.Actions.Users.ListCompanyUsersAction>();
        services.AddScoped<Domain.Company.Actions.Users.UpdateCompanyUserAction>();
        services.AddScoped<Domain.Company.Actions.Users.DeleteCompanyUserAction>();
        services.AddScoped<Domain.Company.Actions.Roles.CreateCompanyRoleAction>();
        services.AddScoped<Domain.Company.Actions.Roles.GetCompanyRoleAction>();
        services.AddScoped<Domain.Company.Actions.Roles.ListCompanyRolesAction>();
        services.AddScoped<Domain.Company.Actions.Roles.UpdateCompanyRoleAction>();
        services.AddScoped<Domain.Company.Actions.Roles.DeleteCompanyRoleAction>();
        services.AddScoped<Domain.Company.Actions.RoleHasPermissions.GetCompanyRolePermissionsGroupedAction>();
        services.AddScoped<Domain.Company.Actions.RoleHasPermissions.SyncCompanyRoleHasPermissionAction>();
        services.AddScoped<Domain.Company.Actions.Contracts.CompanyGetContractAction>();
        services.AddScoped<Domain.Company.Actions.Contracts.CompanyGetContractConsumptionTypeAction>();
        services.AddScoped<Domain.Company.Actions.Contracts.CompanyGetContractDestinationAction>();
        services.AddScoped<Domain.Company.Actions.Contracts.CompanyListContractsAction>();
        services.AddScoped<Domain.Company.Actions.Maps.CompanyExtractPlaceFromGoogleMapsAction>();
        services.AddScoped<Domain.Company.Actions.Maps.CompanyExtractPlacesFromGoogleMapsAction>();

        services.AddScoped<Domain.Driver.Actions.Auth.DriverLoginAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverLoginWithOtpAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverSendLoginOtpAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverForgetPasswordAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverResetPasswordAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverResetPasswordWithCodeAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverChangePasswordAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverEnableMfaAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverDisableMfaAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverSendMfaCodeAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverVerifyMfaAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverRefreshTokenAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverResendVerificationAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverVerifyEmailAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverVerifyPhoneAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverGetProfileAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverUpdateProfileAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverListSessionsAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverRevokeSessionAction>();
        services.AddScoped<Domain.Driver.Actions.Auth.DriverValidatePasswordAction>();

        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorLoginAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorLoginWithOtpAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorSendLoginOtpAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorForgetPasswordAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorResetPasswordAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorResetPasswordWithCodeAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorChangePasswordAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorEnableMfaAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorDisableMfaAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorSendMfaCodeAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorVerifyMfaAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorRefreshTokenAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorResendVerificationAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorVerifyEmailAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorVerifyPhoneAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorGetProfileAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorListSessionsAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorRevokeSessionAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.ContractorValidatePasswordAction>();
        services.AddScoped<Domain.Contractor.Actions.Auth.RegisterContractorAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorGetTripAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorListTripsAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorGetTripInvitationStatisticsAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorGetTripStatusStatisticsAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorGetTripPayoutStatisticsAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorDownloadTripDocumentAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorCreateBookingRequestAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorWithdrawTripBookingAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorListBookingRequestsAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorGetBookingRequestAction>();
        services.AddScoped<Domain.Contractor.Actions.Trips.ContractorChangeTripDriverAction>();
        services.AddScoped<Domain.Contractor.Actions.Drivers.ContractorListDriversAction>();
        services.AddScoped<Domain.Contractor.Actions.Drivers.ContractorGetDriverAction>();
        services.AddScoped<Domain.Contractor.Actions.Drivers.ContractorCreateDriverAction>();
        services.AddScoped<Domain.Contractor.Actions.Vehicles.ContractorListVehiclesAction>();
        services.AddScoped<Domain.Contractor.Actions.Vehicles.ContractorGetVehicleAction>();
        services.AddScoped<Domain.Contractor.Actions.Documents.ContractorListDocumentsAction>();

        services.AddScoped<Domain.Collector.Actions.Auth.CollectorLoginAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorLoginWithOtpAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorSendLoginOtpAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorForgetPasswordAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorResetPasswordAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorResetPasswordWithCodeAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorChangePasswordAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorEnableMfaAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorDisableMfaAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorSendMfaCodeAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorVerifyMfaAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorRefreshTokenAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorResendVerificationAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorVerifyEmailAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorVerifyPhoneAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorGetProfileAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorUpdateProfileAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorListSessionsAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorRevokeSessionAction>();
        services.AddScoped<Domain.Collector.Actions.Auth.CollectorValidatePasswordAction>();

        services.AddScoped<Domain.Driver.Actions.Location.DriverGoOnlineAction>();
        services.AddScoped<Domain.Driver.Actions.Location.DriverGoOfflineAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverStartLoadingCargoAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverCompleteLoadingCargoAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverStartUnloadingCargoAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverCompleteUnloadingCargoAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverStartTripAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverGetTripAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverDownloadTripDocumentAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverListTripsAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverArriveAtWaypointAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverDepartFromWaypointAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverEntersCustomsAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverExitsCustomsAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverCompleteTripAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.SuggestOdometerReadingAction>();
        services.AddScoped<Domain.Driver.Actions.Trips.DriverSyncRouteAction>();
        services.AddScoped<Domain.Driver.Actions.Documents.DriverUploadDocumentAction>();
        services.AddScoped<Domain.Driver.Actions.Documents.DriverUpdateDocumentAction>();


        services.AddScoped<Domain.Common.Actions.Maintenance.TriggerRefreshTokenCleanupAction>();



        services.AddScoped<Domain.Admin.Actions.Logs.GetNotificationsLogsAction>();
        services.AddScoped<Domain.Admin.Actions.Logs.GetActivityLogsAction>();
        services.AddScoped<Domain.Admin.Actions.Logs.GetActivityLogAction>();
        services.AddScoped<Domain.Admin.Actions.Logs.GetOdometerScanLogsAction>();
        services.AddScoped<Domain.Admin.Actions.Logs.GetOdometerScanLogAction>();
        services.AddScoped<Domain.Admin.Actions.Notifications.ListNotificationsAction>();
        services.AddScoped<Domain.Admin.Actions.Notifications.GetUnreadCountAction>();
        services.AddScoped<Domain.Admin.Actions.Notifications.MarkNotificationReadAction>();
        services.AddScoped<Domain.Admin.Actions.Notifications.BatchMarkNotificationsReadAction>();
        services.AddScoped<Domain.Admin.Actions.Notifications.MarkAllNotificationsReadAction>();
        services.AddScoped<Domain.Company.Actions.Notifications.CompanyListNotificationsAction>();
        services.AddScoped<Domain.Company.Actions.Notifications.CompanyGetUnreadCountAction>();
        services.AddScoped<Domain.Company.Actions.Notifications.CompanyMarkNotificationReadAction>();
        services.AddScoped<Domain.Company.Actions.Notifications.CompanyBatchMarkNotificationsReadAction>();
        services.AddScoped<Domain.Company.Actions.Notifications.CompanyMarkAllNotificationsReadAction>();
        services.AddScoped<Domain.Driver.Actions.Notifications.DriverListNotificationsAction>();
        services.AddScoped<Domain.Driver.Actions.Notifications.DriverGetUnreadCountAction>();
        services.AddScoped<Domain.Driver.Actions.Notifications.DriverMarkNotificationReadAction>();
        services.AddScoped<Domain.Driver.Actions.Notifications.DriverBatchMarkNotificationsReadAction>();
        services.AddScoped<Domain.Driver.Actions.Notifications.DriverMarkAllNotificationsReadAction>();
        services.AddScoped<Domain.Contractor.Actions.Notifications.ContractorListNotificationsAction>();
        services.AddScoped<Domain.Contractor.Actions.Notifications.ContractorGetUnreadCountAction>();
        services.AddScoped<Domain.Contractor.Actions.Notifications.ContractorMarkNotificationReadAction>();
        services.AddScoped<Domain.Contractor.Actions.Notifications.ContractorBatchMarkNotificationsReadAction>();
        services.AddScoped<Domain.Contractor.Actions.Notifications.ContractorMarkAllNotificationsReadAction>();
        services.AddScoped<Domain.Admin.Actions.Notifications.ListNotificationPreferencesAction>();
        services.AddScoped<Domain.Admin.Actions.Notifications.UpdateNotificationPreferenceAction>();
        services.AddScoped<Domain.Company.Actions.Notifications.CompanyListNotificationPreferencesAction>();
        services.AddScoped<Domain.Company.Actions.Notifications.CompanyUpdateNotificationPreferenceAction>();
        services.AddScoped<Domain.Driver.Actions.Notifications.DriverListNotificationPreferencesAction>();
        services.AddScoped<Domain.Driver.Actions.Notifications.DriverUpdateNotificationPreferenceAction>();
        services.AddScoped<Domain.Contractor.Actions.Notifications.ContractorListNotificationPreferencesAction>();
        services.AddScoped<Domain.Contractor.Actions.Notifications.ContractorUpdateNotificationPreferenceAction>();

        services.AddScoped<Domain.Admin.Actions.Admins.CreateAdminAction>();
        services.AddScoped<Domain.Admin.Actions.Admins.GetAdminAction>();
        services.AddScoped<Domain.Admin.Actions.Admins.ListAdminsAction>();
        services.AddScoped<Domain.Admin.Actions.Admins.UpdateAdminAction>();
        services.AddScoped<Domain.Admin.Actions.Admins.DeleteAdminAction>();
        services.AddScoped<Domain.Admin.Actions.Admins.BulkUpdateAdminStatusAction>();

        services.AddScoped<Domain.Admin.Actions.Contractors.CreateContractorAction>();
        services.AddScoped<Domain.Admin.Actions.Contractors.GetContractorAction>();
        services.AddScoped<Domain.Admin.Actions.Contractors.ListContractorsAction>();
        services.AddScoped<Domain.Admin.Actions.Contractors.UpdateContractorAction>();
        services.AddScoped<Domain.Admin.Actions.Contractors.DeleteContractorAction>();
        services.AddScoped<Domain.Admin.Actions.Contractors.GetContractorDashboardStatisticsAction>();

        services.AddScoped<Domain.Admin.Actions.Collectors.CreateCollectorAction>();
        services.AddScoped<Domain.Admin.Actions.Collectors.GetCollectorAction>();
        services.AddScoped<Domain.Admin.Actions.Collectors.ListCollectorsAction>();
        services.AddScoped<Domain.Admin.Actions.Collectors.UpdateCollectorAction>();
        services.AddScoped<Domain.Admin.Actions.Collectors.DeleteCollectorAction>();
        services.AddScoped<Domain.Admin.Actions.Collectors.BulkUpdateCollectorStatusAction>();


        services.AddScoped<Domain.Admin.Actions.Roles.CreateRoleAction>();
        services.AddScoped<Domain.Admin.Actions.Roles.GetRoleAction>();
        services.AddScoped<Domain.Admin.Actions.RoleHasPermissions.GetRolePermissionsGroupedAction>();
        services.AddScoped<Domain.Admin.Actions.Roles.ListRolesAction>();
        services.AddScoped<Domain.Admin.Actions.Roles.UpdateRoleAction>();
        services.AddScoped<Domain.Admin.Actions.Roles.DeleteRoleAction>();


        services.AddScoped<Domain.Admin.Actions.RoleHasPermissions.SyncRoleHasPermissionAction>();


        services.AddScoped<Domain.Admin.Actions.Drivers.CreateDriverAction>();
        services.AddScoped<Domain.Admin.Actions.Drivers.GetDriverAction>();
        services.AddScoped<Domain.Admin.Actions.Drivers.ListDriversAction>();
        services.AddScoped<Domain.Admin.Actions.Drivers.UpdateDriverAction>();
        services.AddScoped<Domain.Admin.Actions.Drivers.DeleteDriverAction>();
        services.AddScoped<Domain.Admin.Actions.Drivers.BulkImportDriversAction>();
        services.AddScoped<Domain.Admin.Actions.Drivers.GetDriverWorkingStatisticsAction>();
        services.AddScoped<Domain.Admin.Actions.Drivers.ExportDriversExcelAction>();


        services.AddScoped<Domain.Admin.Actions.Companies.StoreCompanyAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.BulkImportCompaniesAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.GetCompanyAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.ListCompaniesAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.UpdateCompanyAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.DeleteCompanyAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.CreateCompanyUserAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.GetCompanyUserAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.ListCompanyUsersAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.UpdateCompanyUserAction>();
        services.AddScoped<Domain.Admin.Actions.Companies.DeleteCompanyUserAction>();


        services.AddScoped<Domain.Admin.Helpers.PushTripToContractorHelper>();
        services.AddScoped<Domain.Admin.Actions.Trips.CreateTripAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.CreateHistoricalTripAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.BulkCreateTripsAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.BulkImportTripsAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.GetTripAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.GetTripRouteAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.PreviewTripContractorPriceAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.ListTripsAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.ExportTripsExcelAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.UpdateTripAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.UpdateTripChargesAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.AuditTripAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.BulkUpdateTripChargesInvoiceTypeAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.SummarizeTripChargesByInvoiceTypeAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.SetTripOverNightCostBearerAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.SetTripDispatchingModeAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.UpdateTripDispatcherNotesAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.CancelTripAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.CompleteTripAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.PushTripToContractorsAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.ApproveTripBookingRequestAction>();
        services.AddScoped<Domain.Collector.Actions.Trips.CollectTripDocumentsAction>();
        services.AddScoped<Domain.Collector.Actions.Trips.ConfirmTripDocumentsAction>();
        services.AddScoped<Domain.Collector.Actions.Trips.BulkCollectTripsDocumentsAction>();
        services.AddScoped<Domain.Collector.Actions.Trips.BulkConfirmTripsDocumentsAction>();
        services.AddScoped<Domain.Collector.Actions.Trips.CollectorListTripsAction>();
        services.AddScoped<Domain.Collector.Actions.Trips.CollectorGetTripAction>();
        services.AddScoped<Domain.Collector.Actions.Documents.CollectorListDocumentsAction>();
        services.AddScoped<Domain.Collector.Actions.Documents.CollectorGetDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.DeleteTripAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.DownloadTripDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.Trips.TrackTripsAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.StartLoadingCargoAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.CompleteLoadingCargoAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.StartUnloadingCargoAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.CompleteUnloadingCargoAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.StartTripAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.ArriveAtWaypointAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.DepartFromWaypointAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.ArriveAndStartLoadingAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.ArriveAndStartUnloadingAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.CompleteLoadingAndDepartAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.CompleteUnloadingAndDepartAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.EntersCustomsAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.ExitsCustomsAction>();
        services.AddScoped<Domain.Admin.Actions.TripsManagement.CompleteTripAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetTripsStatisticsAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetMonthlyTripsCostStatisticsAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.CreateTargetAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.UpdateTargetAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.DeleteTargetAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetTargetAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.ListTargetsAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetMonthlyTargetsAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetMonthlyTripsTargetsAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetMonthlyCostTargetsAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetMonthlyTripsAvailableDriversAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetWeeklyVehicleTripsComparisonAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetWeeklyDriverTripsComparisonAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetTripPerformanceAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetDriverFleetOverviewAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetContractorActivityAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetHomeDashboardAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetSectorAggregationDashboardAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.GetVehicleStatusStatisticsAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.ListDriverCompletedTripsByLifecycleActorAction>();
        services.AddScoped<Domain.Admin.Actions.Dashboard.ExportDriverLifecycleTripsAction>();
        services.AddScoped<Domain.Admin.Actions.Maps.ExtractPlaceFromGoogleMapsAction>();
        services.AddScoped<Domain.Admin.Actions.Maps.ExtractPlacesFromGoogleMapsAction>();

        services.AddScoped<ITripContractExtraFeePricing, TripContractExtraFeePricingService>();
        services.AddScoped<Domain.Common.Actions.CalculateAndPersistTripChargeAction>();


        services.AddScoped<Domain.Common.Services.InvoicePaymentAllocator>();
        services.AddScoped<Domain.Admin.Actions.Invoices.CreateInvoiceAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.LinkInvoicePaymentsAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.CancelInvoiceAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.SetInvoicePaidAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.GetInvoiceAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.ListInvoicesAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.ExportInvoicesExcelAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.GetInvoiceStatisticsAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.DownloadTaxedInvoiceAttachmentAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.CreateBankTransferAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.GetBankTransferAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.ListBankTransfersAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.UpdateBankTransferAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.DeleteBankTransferAction>();
        services.AddScoped<Domain.Admin.Actions.Invoices.DownloadBankTransferAttachmentAction>();

        services.AddScoped<Domain.Admin.Actions.ContractorPayouts.CreateContractorPayoutAction>();
        services.AddScoped<Domain.Admin.Actions.ContractorPayouts.SetContractorPayoutPaidAction>();
        services.AddScoped<Domain.Admin.Actions.ContractorPayouts.BulkSetContractorPayoutsPaidAction>();
        services.AddScoped<Domain.Admin.Actions.ContractorPayouts.SetContractorPayoutCancelledAction>();
        services.AddScoped<Domain.Admin.Actions.ContractorPayouts.GetContractorPayoutAction>();
        services.AddScoped<Domain.Admin.Actions.ContractorPayouts.ListContractorPayoutsAction>();

        services.AddScoped<Domain.Admin.Actions.Cities.CreateCityAction>();
        services.AddScoped<Domain.Admin.Actions.Cities.GetCityAction>();
        services.AddScoped<Domain.Admin.Actions.Cities.ListCitiesAction>();
        services.AddScoped<Domain.Admin.Actions.Cities.UpdateCityAction>();
        services.AddScoped<Domain.Admin.Actions.Cities.DeleteCityAction>();




        services.AddScoped<Domain.Admin.Actions.Governerates.CreateGovernerateAction>();
        services.AddScoped<Domain.Admin.Actions.Governerates.GetGovernerateAction>();
        services.AddScoped<Domain.Admin.Actions.Governerates.ListGoverneratesAction>();
        services.AddScoped<Domain.Admin.Actions.Governerates.UpdateGovernerateAction>();
        services.AddScoped<Domain.Admin.Actions.Governerates.DeleteGovernerateAction>();

        services.AddScoped<Domain.Admin.Actions.Sectors.CreateSectorAction>();
        services.AddScoped<Domain.Admin.Actions.Sectors.GetSectorAction>();
        services.AddScoped<Domain.Admin.Actions.Sectors.ListSectorsAction>();
        services.AddScoped<Domain.Admin.Actions.Sectors.UpdateSectorAction>();
        services.AddScoped<Domain.Admin.Actions.Sectors.DeleteSectorAction>();

        services.AddScoped<Domain.Admin.Actions.Districts.CreateDistrictAction>();
        services.AddScoped<Domain.Admin.Actions.Districts.GetDistrictAction>();
        services.AddScoped<Domain.Admin.Actions.Districts.ListDistrictsAction>();
        services.AddScoped<Domain.Admin.Actions.Districts.UpdateDistrictAction>();
        services.AddScoped<Domain.Admin.Actions.Districts.DeleteDistrictAction>();


        services.AddScoped<Domain.Admin.Actions.IndustryTypes.CreateIndustryTypeAction>();
        services.AddScoped<Domain.Admin.Actions.IndustryTypes.GetIndustryTypeAction>();
        services.AddScoped<Domain.Admin.Actions.IndustryTypes.ListIndustryTypesAction>();
        services.AddScoped<Domain.Admin.Actions.IndustryTypes.UpdateIndustryTypeAction>();
        services.AddScoped<Domain.Admin.Actions.IndustryTypes.DeleteIndustryTypeAction>();


        services.AddScoped<Domain.Admin.Actions.CommunicationMethods.CreateCommunicationMethodAction>();
        services.AddScoped<Domain.Admin.Actions.CommunicationMethods.GetCommunicationMethodAction>();
        services.AddScoped<Domain.Admin.Actions.CommunicationMethods.ListCommunicationMethodsAction>();
        services.AddScoped<Domain.Admin.Actions.CommunicationMethods.UpdateCommunicationMethodAction>();
        services.AddScoped<Domain.Admin.Actions.CommunicationMethods.DeleteCommunicationMethodAction>();


        services.AddScoped<Domain.Admin.Actions.OperatingHours.CreateOperatingHoursAction>();
        services.AddScoped<Domain.Admin.Actions.OperatingHours.GetOperatingHoursAction>();
        services.AddScoped<Domain.Admin.Actions.OperatingHours.ListOperatingHoursAction>();
        services.AddScoped<Domain.Admin.Actions.OperatingHours.UpdateOperatingHoursAction>();
        services.AddScoped<Domain.Admin.Actions.OperatingHours.DeleteOperatingHoursAction>();


        services.AddScoped<Domain.Admin.Actions.ServiceRequirements.CreateServiceRequirementAction>();
        services.AddScoped<Domain.Admin.Actions.ServiceRequirements.GetServiceRequirementAction>();
        services.AddScoped<Domain.Admin.Actions.ServiceRequirements.ListServiceRequirementsAction>();
        services.AddScoped<Domain.Admin.Actions.ServiceRequirements.UpdateServiceRequirementAction>();
        services.AddScoped<Domain.Admin.Actions.ServiceRequirements.DeleteServiceRequirementAction>();


        services.AddScoped<Domain.Admin.Actions.VehicleTypes.CreateVehicleTypeAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleTypes.GetVehicleTypeAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleTypes.ListVehicleTypesAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleTypes.UpdateVehicleTypeAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleTypes.DeleteVehicleTypeAction>();


        services.AddScoped<Domain.Admin.Actions.DriverLicenseTypes.CreateDriverLicenseTypeAction>();
        services.AddScoped<Domain.Admin.Actions.DriverLicenseTypes.GetDriverLicenseTypeAction>();
        services.AddScoped<Domain.Admin.Actions.DriverLicenseTypes.ListDriverLicenseTypesAction>();
        services.AddScoped<Domain.Admin.Actions.DriverLicenseTypes.UpdateDriverLicenseTypeAction>();
        services.AddScoped<Domain.Admin.Actions.DriverLicenseTypes.DeleteDriverLicenseTypeAction>();


        services.AddScoped<Domain.Admin.Actions.DriverDocuments.CreateDriverDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.DriverDocuments.GetDriverDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.DriverDocuments.ListDriverDocumentsAction>();
        services.AddScoped<Domain.Admin.Actions.DriverDocuments.UpdateDriverDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.DriverDocuments.DeleteDriverDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.DriverDocuments.DownloadDriverDocumentAction>();


        services.AddScoped<Domain.Admin.Actions.Taxes.CreateTaxAction>();
        services.AddScoped<Domain.Admin.Actions.Taxes.GetTaxAction>();
        services.AddScoped<Domain.Admin.Actions.Taxes.ListTaxesAction>();
        services.AddScoped<Domain.Admin.Actions.Taxes.UpdateTaxAction>();
        services.AddScoped<Domain.Admin.Actions.Taxes.DeleteTaxAction>();

        services.AddScoped<Domain.Admin.Actions.Leads.CreateLeadAction>();
        services.AddScoped<Domain.Admin.Actions.Leads.GetLeadAction>();
        services.AddScoped<Domain.Admin.Actions.Leads.GetLeadByPhoneAction>();
        services.AddScoped<Domain.Admin.Actions.Leads.ListLeadsAction>();
        services.AddScoped<Domain.Admin.Actions.Leads.UpdateLeadAction>();
        services.AddScoped<Domain.Admin.Actions.Leads.DeleteLeadAction>();


        services.AddScoped<Domain.Admin.Actions.Discounts.CreateDiscountAction>();
        services.AddScoped<Domain.Admin.Actions.Discounts.GetDiscountAction>();
        services.AddScoped<Domain.Admin.Actions.Discounts.ListDiscountsAction>();
        services.AddScoped<Domain.Admin.Actions.Discounts.UpdateDiscountAction>();
        services.AddScoped<Domain.Admin.Actions.Discounts.DeleteDiscountAction>();


        services.AddScoped<Domain.Admin.Actions.Colors.CreateColorAction>();
        services.AddScoped<Domain.Admin.Actions.Colors.GetColorAction>();
        services.AddScoped<Domain.Admin.Actions.Colors.ListColorsAction>();
        services.AddScoped<Domain.Admin.Actions.Colors.UpdateColorAction>();
        services.AddScoped<Domain.Admin.Actions.Colors.DeleteColorAction>();


        services.AddScoped<Domain.Admin.Actions.Tags.CreateTagAction>();
        services.AddScoped<Domain.Admin.Actions.Tags.GetTagAction>();
        services.AddScoped<Domain.Admin.Actions.Tags.ListTagsAction>();
        services.AddScoped<Domain.Admin.Actions.Tags.UpdateTagAction>();
        services.AddScoped<Domain.Admin.Actions.Tags.DeleteTagAction>();


        services.AddScoped<Domain.Admin.Actions.FuelTypes.CreateFuelTypeAction>();
        services.AddScoped<Domain.Admin.Actions.FuelTypes.GetFuelTypeAction>();
        services.AddScoped<Domain.Admin.Actions.FuelTypes.ListFuelTypesAction>();
        services.AddScoped<Domain.Admin.Actions.FuelTypes.UpdateFuelTypeAction>();
        services.AddScoped<Domain.Admin.Actions.FuelTypes.DeleteFuelTypeAction>();



        services.AddScoped<Domain.Admin.Actions.Contracts.CreateContractAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.GetContractAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.GetContractConsumptionTypeAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.GetContractVehicleTypeIdAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.GetContractVehicleTypeAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.GetContractDestinationAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.ExportContractDestinationsExcelAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.ListContractsAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.CalculateContractDestinationCostsAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.CalculateContractSlotChargesAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.CreateDraftContractBillingSnapshotAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.UpdateDraftContractBillingSnapshotAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.PatchContractBillingSnapshotCalendarDayAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.PatchContractBillingSnapshotCalendarDayTripCountAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.PatchContractBillingSnapshotCommercialTermsAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.DeleteContractBillingSnapshotCommercialTermsAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.GetContractBillingSnapshotAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.ListContractBillingSnapshotsAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.DeleteContractBillingSnapshotAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.ExportContractSlotChargesExcelAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.ExportContractBillingSnapshotExcelAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.UpdateContractAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.DeleteContractAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.PullBackToDraftContractAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.DeactivateContractAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.ReactivateContractAction>();
        services.AddScoped<Domain.Admin.Actions.Contracts.TerminateContractAction>();


        services.AddScoped<Domain.Admin.Actions.ContractApprovals.ApproveContractApprovalAction>();
        services.AddScoped<Domain.Admin.Actions.ContractApprovals.RejectContractApprovalAction>();
        services.AddScoped<Domain.Admin.Actions.ContractApprovals.SubmitContractApprovalDecisionAction>();
        services.AddScoped<Domain.Admin.Actions.ContractApprovals.GetApprovalStatusAction>();
        services.AddScoped<Domain.Admin.Actions.ContractApprovals.ListContractApprovalsAction>();


        services.AddScoped<Domain.Admin.Actions.DocumentStatuses.CreateDocumentStatusAction>();
        services.AddScoped<Domain.Admin.Actions.DocumentStatuses.GetDocumentStatusAction>();
        services.AddScoped<Domain.Admin.Actions.DocumentStatuses.ListDocumentStatusesAction>();
        services.AddScoped<Domain.Admin.Actions.DocumentStatuses.UpdateDocumentStatusAction>();
        services.AddScoped<Domain.Admin.Actions.DocumentStatuses.DeleteDocumentStatusAction>();


        services.AddScoped<Domain.Admin.Actions.VehicleCapacityUnits.CreateVehicleCapacityUnitAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleCapacityUnits.GetVehicleCapacityUnitAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleCapacityUnits.ListVehicleCapacityUnitsAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleCapacityUnits.UpdateVehicleCapacityUnitAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleCapacityUnits.DeleteVehicleCapacityUnitAction>();

        services.AddScoped<Domain.Admin.Actions.Ports.CreatePortAction>();
        services.AddScoped<Domain.Admin.Actions.Ports.GetPortAction>();
        services.AddScoped<Domain.Admin.Actions.Ports.ListPortsAction>();
        services.AddScoped<Domain.Admin.Actions.Ports.UpdatePortAction>();
        services.AddScoped<Domain.Admin.Actions.Ports.DeletePortAction>();

        services.AddScoped<Domain.Admin.Actions.Containers.CreateContainerAction>();
        services.AddScoped<Domain.Admin.Actions.Containers.GetContainerAction>();
        services.AddScoped<Domain.Admin.Actions.Containers.ListContainersAction>();
        services.AddScoped<Domain.Admin.Actions.Containers.UpdateContainerAction>();
        services.AddScoped<Domain.Admin.Actions.Containers.DeleteContainerAction>();

        services.AddScoped<Domain.Admin.Actions.ShippingLines.CreateShippingLineAction>();
        services.AddScoped<Domain.Admin.Actions.ShippingLines.GetShippingLineAction>();
        services.AddScoped<Domain.Admin.Actions.ShippingLines.ListShippingLinesAction>();
        services.AddScoped<Domain.Admin.Actions.ShippingLines.UpdateShippingLineAction>();
        services.AddScoped<Domain.Admin.Actions.ShippingLines.DeleteShippingLineAction>();

        services.AddScoped<Domain.Admin.Actions.Vehicles.CreateVehicleAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.GetVehicleAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.ListVehiclesAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.UpdateVehicleAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.DeleteVehicleAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.BulkImportVehiclesAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.ExportVehiclesExcelAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.SetVehicleAssignedParkingFacilityAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.AssignVehicleDriverAction>();
        services.AddScoped<Domain.Admin.Actions.Vehicles.ListVehicleAssignmentHistoriesAction>();


        services.AddScoped<Domain.Admin.Actions.VehicleInsurances.CreateVehicleInsuranceAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleInsurances.GetVehicleInsuranceAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleInsurances.ListVehicleInsurancesAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleInsurances.UpdateVehicleInsuranceAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleInsurances.DeleteVehicleInsuranceAction>();

        services.AddScoped<Domain.Admin.Actions.VehicleFacilities.CreateVehicleFacilityAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleFacilities.GetVehicleFacilityAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleFacilities.ListVehicleFacilitiesAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleFacilities.UpdateVehicleFacilityAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleFacilities.DeleteVehicleFacilityAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleFacilityVisits.EnterVehicleFacilityVisitAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleFacilityVisits.ExitVehicleFacilityVisitAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleFacilityVisits.EnterVehicleParkingFacilityAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleFacilityVisits.ExitVehicleParkingFacilityAction>();


        services.AddScoped<Domain.Admin.Actions.VehicleBrands.CreateVehicleBrandAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleBrands.GetVehicleBrandAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleBrands.ListVehicleBrandsAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleBrands.UpdateVehicleBrandAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleBrands.DeleteVehicleBrandAction>();


        services.AddScoped<Domain.Admin.Actions.VehicleBrandModels.CreateVehicleBrandModelAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleBrandModels.GetVehicleBrandModelAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleBrandModels.ListVehicleBrandModelsAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleBrandModels.UpdateVehicleBrandModelAction>();
        services.AddScoped<Domain.Admin.Actions.VehicleBrandModels.DeleteVehicleBrandModelAction>();


        services.AddScoped<Domain.Company.Actions.Invoices.GetInvoiceAction>();
        services.AddScoped<Domain.Company.Actions.Invoices.ListInvoicesAction>();
        services.AddScoped<Domain.Company.Actions.Invoices.DownloadTaxedInvoiceAttachmentAction>();

        services.AddScoped<Domain.Admin.Actions.Documents.CreateDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.Documents.GetDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.Documents.ListDocumentsAction>();
        services.AddScoped<Domain.Admin.Actions.Documents.UpdateDocumentAction>();
        services.AddScoped<Domain.Admin.Actions.Documents.DeleteDocumentAction>();


        services.AddScoped<Domain.Common.Interfaces.IBroadcastNotificationTargetResolver, Infrastructure.Services.Notification.BroadcastNotificationTargetResolver>();
        services.AddSingleton<Domain.Common.Interfaces.IBroadcastNotificationEnqueueOptions, Infrastructure.Services.Queue.BroadcastNotificationEnqueueOptions>();
        services.Configure<Infrastructure.Options.BroadcastShareOptions>(
            configuration.GetSection(Infrastructure.Options.BroadcastShareOptions.SectionName));
        services.AddSingleton<Domain.Common.Interfaces.IBroadcastShareUrlBuilder, Infrastructure.Services.Notification.BroadcastShareUrlBuilder>();
        services.AddScoped<Domain.Admin.Actions.Settings.PushNotifications.SendPushNotificationAction>();
        services.AddScoped<Domain.Admin.Actions.Settings.PushNotifications.SendBroadcastNotificationAction>();
        services.AddScoped<Domain.Admin.Actions.Settings.PushNotifications.ListBroadcastNotificationsAction>();
        services.AddScoped<Domain.Admin.Actions.Settings.PushNotifications.GetBroadcastNotificationAction>();
        services.AddScoped<Domain.Common.Actions.BroadcastNotifications.GetPublicBroadcastNotificationAction>();
        services.AddScoped<Domain.Admin.Actions.Settings.GetGeneralSettingAction>();
        services.AddScoped<Domain.Admin.Actions.Settings.SetGeneralSettingAction>();
        services.AddScoped<Domain.Admin.Actions.Settings.ListGeneralSettingsAction>();

        services.AddScoped<Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction>();
        services.AddScoped<Domain.Common.Actions.DeviceToken.DeactivateDeviceTokensAction>();

        services.AddScoped<Domain.Common.Actions.AppVersion.CheckAppVersionAction>();

        RegisterLookupServices(services);
        RegisterHealthChecks(services);

        return services;
    }




    private static void RegisterOdometerReadingExtractor(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["OdometerOcr:Provider"]?.ToLower() ?? "openai";

        switch (provider)
        {
            case "openai":
                services.AddScoped<Domain.Common.Interfaces.IOdometerReadingExtractor, Infrastructure.Services.Ocr.OpenAiOdometerReadingExtractor>();
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown odometer OCR provider: {provider}. Valid options: openai");
        }
    }

    private static void RegisterStorageProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Media:StorageProvider"]?.ToLower() ?? "local";

        switch (provider)
        {
            case "local":
                services.AddScoped<Domain.Common.Interfaces.IStorageProvider, LocalStorageProvider>();
                break;

            case "s3":
            case "aws":
            case "awss3":
                services.AddScoped<Domain.Common.Interfaces.IStorageProvider, S3StorageProvider>();
                break;

            case "azure":
            case "azureblob":
                services.AddScoped<Domain.Common.Interfaces.IStorageProvider, AzureBlobStorageProvider>();
                break;

            case "gcs":
            case "google":
            case "googlecloud":
                services.AddScoped<Domain.Common.Interfaces.IStorageProvider, GoogleCloudStorageProvider>();
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown storage provider: {provider}. " +
                    "Valid options: local, s3, azure, gcs");
        }
    }




    private static void RegisterSmsProvider(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddScoped<Domain.Common.Interfaces.ISmsProvider>(sp =>
        {
            var repo = sp.GetRequiredService<IGeneralSettingRepository>();
            var baseConfig = sp.GetRequiredService<IConfiguration>();
            var config = BuildMergedSmsConfiguration(repo, baseConfig);
            var provider = config["Sms:Provider"]?.ToLower() ?? "victory_link";

            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return provider switch
            {
                "akedly" or "akedly.io" => CreateAkedlyProvider(sp, httpClientFactory, config),
                "victory_link" or "victorylink" => CreateVictoryLinkProvider(sp, httpClientFactory, config),
                "ultramsg" or "ultra_msg" => CreateUltraMsgProvider(sp, httpClientFactory, config),
                _ => throw new InvalidOperationException(
                    $"Unknown SMS provider: {provider}. Valid options: akedly, victory_link, ultramsg. " +
                    "Set Sms:Provider in general_settings or appsettings.")
            };
        });
    }

    private static IConfiguration BuildMergedSmsConfiguration(IGeneralSettingRepository repo, IConfiguration baseConfig)
    {
        var fromDb = repo.GetByPrefixAsync(GeneralSettingKeys.SmsPrefix).GetAwaiter().GetResult();
        var builder = new ConfigurationBuilder();
        builder.AddConfiguration(baseConfig);
        if (fromDb.Count > 0)
        {
            builder.AddInMemoryCollection(fromDb!);
        }
        return builder.Build();
    }

    private static AkedlySmsProvider CreateAkedlyProvider(IServiceProvider sp, IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        var httpClient = httpClientFactory.CreateClient("AkedlySms");
        var logger = sp.GetRequiredService<ILogger<AkedlySmsProvider>>();
        return new AkedlySmsProvider(httpClient, config, logger);
    }

    private static VictoryLinkSmsProvider CreateVictoryLinkProvider(IServiceProvider sp, IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        var httpClient = httpClientFactory.CreateClient("VictoryLinkSms");
        var logger = sp.GetRequiredService<ILogger<VictoryLinkSmsProvider>>();
        return new VictoryLinkSmsProvider(httpClient, config, logger);
    }

    private static UltraMsgSmsProvider CreateUltraMsgProvider(IServiceProvider sp, IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        var httpClient = httpClientFactory.CreateClient("UltraMsgWhatsApp");
        var logger = sp.GetRequiredService<ILogger<UltraMsgSmsProvider>>();
        return new UltraMsgSmsProvider(httpClient, config, logger);
    }




    private static void RegisterRedisQueue(IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? configuration["Redis:ConnectionString"]
            ?? "localhost:6379";


        services.AddSingleton<QueueRedisConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<QueueRedisConnection>>();
            logger.LogInformation("Connecting queue Redis at: {ConnectionString}", redisConnectionString);

            var multiplexer = ConnectionMultiplexer.Connect(RedisConnectionOptionsFactory.Create(redisConnectionString));
            return new QueueRedisConnection(multiplexer);
        });


        services.AddScoped<Domain.Common.Interfaces.IQueueService, Infrastructure.Services.Queue.RedisQueueService>();


        services.AddScoped<Infrastructure.Services.Queue.EmailQueueProcessor>();
        services.AddScoped<Infrastructure.Services.Queue.SmsQueueProcessor>();
        services.AddScoped<Infrastructure.Services.Queue.PushNotificationQueueProcessor>();
        services.AddScoped<Infrastructure.Services.Queue.BroadcastNotificationQueueProcessor>();
        services.AddScoped<Infrastructure.Services.Queue.IQueueProcessorResolver, Infrastructure.Services.Queue.QueueProcessorResolver>();
        services.AddScoped<Infrastructure.Services.Queue.CompositeQueueProcessor>();
        services.AddScoped<Infrastructure.Services.Queue.IQueueProcessor>(sp =>
            new Infrastructure.Services.Queue.RetryableQueueProcessor(
                sp.GetRequiredService<Infrastructure.Services.Queue.CompositeQueueProcessor>(),
                sp.GetRequiredService<Domain.Common.Interfaces.IQueueService>(),
                sp.GetRequiredService<ILogger<Infrastructure.Services.Queue.RetryableQueueProcessor>>(),
                configuration));


        var queueOptions = new QueueOptions();
        configuration.GetSection("Queue").Bind(queueOptions);

        if (queueOptions.Queues.Count == 0)
        {
            queueOptions.Queues = new List<string> { "default" };
        }
        if (queueOptions.DomainToQueueMapping == null || queueOptions.DomainToQueueMapping.Count == 0)
        {
            queueOptions.DomainToQueueMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [NotificationDomain.Admin.ToStringValue()] = "admin",
                [NotificationDomain.Company.ToStringValue()] = "company",
                [NotificationDomain.Driver.ToStringValue()] = "driver",
                [NotificationDomain.Contractor.ToStringValue()] = "contractor",
                [NotificationDomain.Default.ToStringValue()] = "admin"
            };
        }
        if (string.IsNullOrEmpty(queueOptions.DefaultQueue))
        {
            queueOptions.DefaultQueue = "admin";
        }
        if (queueOptions.PollingIntervalMs <= 0)
        {
            queueOptions.PollingIntervalMs = 1000;
        }
        if (queueOptions.ErrorRetryDelayMs <= 0)
        {
            queueOptions.ErrorRetryDelayMs = 5000;
        }
        if (queueOptions.BroadcastNotificationBatchSize <= 0)
        {
            queueOptions.BroadcastNotificationBatchSize = 200;
        }

        services.Configure<QueueOptions>(options =>
        {
            options.Queues = queueOptions.Queues;
            options.DomainToQueueMapping = queueOptions.DomainToQueueMapping;
            options.DefaultQueue = queueOptions.DefaultQueue;
            options.PollingIntervalMs = queueOptions.PollingIntervalMs;
            options.ErrorRetryDelayMs = queueOptions.ErrorRetryDelayMs;
            options.BroadcastNotificationBatchSize = queueOptions.BroadcastNotificationBatchSize;
        });

        services.Configure<QueueProcessorRegistryOptions>(options =>
        {
            options.ProcessorMappings = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                [LoadlyAggregator.Shared.Constants.QueueMessageTypes.Email] = typeof(EmailQueueProcessor),
                [LoadlyAggregator.Shared.Constants.QueueMessageTypes.Sms] = typeof(SmsQueueProcessor),
                [LoadlyAggregator.Shared.Constants.QueueMessageTypes.PushNotification] = typeof(PushNotificationQueueProcessor),
                [LoadlyAggregator.Shared.Constants.QueueMessageTypes.BroadcastNotification] = typeof(BroadcastNotificationQueueProcessor)
            };
        });


        services.AddHostedService<RedisQueueBackgroundService>();


        services.Configure<RefreshTokenCleanupOptions>(options =>
        {
            var cleanupIntervalHours = configuration["AuthSettings:RefreshTokenCleanupIntervalHours"];
            if (int.TryParse(cleanupIntervalHours, out var intervalHours) && intervalHours > 0)
            {
                options.CleanupIntervalHours = intervalHours;
            }

            var expiredRetentionDays = configuration["AuthSettings:ExpiredTokenRetentionDays"];
            if (int.TryParse(expiredRetentionDays, out var expiredDays) && expiredDays > 0)
            {
                options.ExpiredTokenRetentionDays = expiredDays;
            }

            var revokedRetentionDays = configuration["AuthSettings:RevokedTokenRetentionDays"];
            if (int.TryParse(revokedRetentionDays, out var revokedDays) && revokedDays > 0)
            {
                options.RevokedTokenRetentionDays = revokedDays;
            }

            var batchSize = configuration["AuthSettings:CleanupBatchSize"];
            if (int.TryParse(batchSize, out var batch) && batch > 0)
            {
                options.BatchSize = batch;
            }

            var preferredStartHour = configuration["AuthSettings:PreferredCleanupStartHour"];
            if (int.TryParse(preferredStartHour, out var startHour) && startHour >= 0 && startHour <= 23)
            {
                options.PreferredCleanupStartHour = startHour;
            }

            var preferredEndHour = configuration["AuthSettings:PreferredCleanupEndHour"];
            if (int.TryParse(preferredEndHour, out var endHour) && endHour >= 0 && endHour <= 23)
            {
                options.PreferredCleanupEndHour = endHour;
            }
        });




        services.AddSingleton<RefreshTokenCleanupService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RefreshTokenCleanupService>>();
            var options = sp.GetRequiredService<IOptions<RefreshTokenCleanupOptions>>();
            var timeProvider = sp.GetRequiredService<TimeProvider>();
            return new RefreshTokenCleanupService(sp, logger, options, timeProvider);
        });


        services.AddSingleton<Domain.Common.Interfaces.IRefreshTokenCleanupService>(sp =>
            sp.GetRequiredService<RefreshTokenCleanupService>());




        services.AddSingleton<IHostedService>(sp =>
            sp.GetRequiredService<RefreshTokenCleanupService>());
    }




    private static void RegisterLookupServices(IServiceCollection services)
    {

        services.AddScoped<Domain.Common.Interfaces.ILookupFactory, Infrastructure.Services.Lookups.Factories.LookupFactory>();


        services.AddScoped<Infrastructure.Services.Lookups.Entities.CitiesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.DistrictsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.GoverneratesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.SectorsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.IndustryTypesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ServiceRequirementsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.CommunicationMethodsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.DocumentStatusesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.AdminsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.CompaniesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.DriversDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ContractorsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.CollectorsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.OperatingHoursDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ContractVehicleTypesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.VehicleTypesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.VehicleBrandsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.VehicleBrandModelsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.VehicleCapacityUnitsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.PortsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ContainersDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ShippingLinesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.VehicleInsurancesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.RolesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ExtraFeeTypesLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ContractReferenceNumberDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.TaxesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.LeadsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.DiscountsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.FuelTypesDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.TagsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.WaypointsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ContractDestinationsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.ContractBillingSnapshotsDefaultLookup>();
        services.AddScoped<Infrastructure.Services.Lookups.Entities.VehicleFacilitiesDefaultLookup>();
    }

    private static void RegisterHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database")
            .AddDbContextCheck<LogsDbContext>("logs-database")
            .AddCheck<RedisHealthCheck>("redis");
    }
}

