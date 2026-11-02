namespace LoadlyAggregator.Domain.Admin.Services;

public interface IContractorNotificationService
{
    Task SendContractorWelcomeEmailIfEmailChannelAsync(Common.Entities.Contractor contractor, string resetPasswordUrl);

    Task SendContractorApprovedSmsAsync(Common.Entities.Contractor contractor);
}
