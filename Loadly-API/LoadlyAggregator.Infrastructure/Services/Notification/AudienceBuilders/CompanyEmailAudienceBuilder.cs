using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;
using Lang = LoadlyAggregator.Shared.Constants.NotificationLanguage;

namespace LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;

/// <summary>
/// Builds a company <see cref="NotificationRecipient"/> for trip-related events.
/// Encapsulates: trip fetch → company fetch → template resolution → HTML email build → channel logic.
/// Always includes <see cref="NotificationChannel.IN_APP"/>; adds Email if the company has one.
/// Callers may supply additional channels and push/title overrides for events that differ
/// from the default email+in-app pattern (e.g. TripScheduled adds Push with a custom body).
/// </summary>
public class CompanyEmailAudienceBuilder : ICompanyEmailAudienceBuilder
{
    private readonly ITripRepository _tripRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly INotificationTemplateService _templateService;
    private readonly IEmailTemplateBuilder _emailTemplateBuilder;

    public CompanyEmailAudienceBuilder(
        ITripRepository tripRepository,
        ICompanyRepository companyRepository,
        INotificationTemplateService templateService,
        IEmailTemplateBuilder emailTemplateBuilder)
    {
        _tripRepository = tripRepository;
        _companyRepository = companyRepository;
        _templateService = templateService;
        _emailTemplateBuilder = emailTemplateBuilder;
    }

    public async Task<NotificationRecipient?> BuildAsync(
        long companyId,
        long tripId,
        NotificationEventType notificationEventType,
        NotificationChannel[]? additionalChannels = null,
        string? titleOverride = null,
        string? bodyOverride = null,
        Dictionary<string, string>? pushData = null,
        string? pushClickAction = null,
        NotificationResourceType? resourceType = null,
        long? resourceId = null,
        string? clickAction = null)
    {
        var trip = await _tripRepository.GetByIdAsync(tripId);
        var company = await _companyRepository.GetByIdAsync(companyId);
        if (trip is null || company is null)
            return null;

        string eventKey = notificationEventType.ToString();
        var placeholders = NotificationPlaceholderBuilder.ForTrip(tripId, trip.StartDate, trip.EndDate);

        string heading = NotificationDefinitionRegistry.GetCompanyHeadingOrDisplayName(notificationEventType);

        string title = await _templateService.ResolveAsync($"{eventKey}_company_subject", Lang.Default, placeholders);
        string body = await _templateService.ResolveAsync($"{eventKey}_company_body", Lang.Default, placeholders);

        string emailSubject = await _templateService.ResolveAsync($"{eventKey}_company_subject", Lang.Email, placeholders);
        string emailBody = await _templateService.ResolveAsync($"{eventKey}_company_body", Lang.Email, placeholders);
        string htmlBody = _emailTemplateBuilder.BuildCompanyTripEmail(company.FullName, emailBody, heading);

        var channels = new HashSet<NotificationChannel> { NotificationChannel.IN_APP };
        if (!string.IsNullOrWhiteSpace(company.Email))
            channels.Add(NotificationChannel.EMAIL);
        if (additionalChannels is not null)
            foreach (NotificationChannel ch in additionalChannels)
                channels.Add(ch);

        return new NotificationRecipient
        {
            ModelId = company.Id,
            ModelType = "Company",
            NotificationEventType = notificationEventType,
            Domain = NotificationDomain.Company,
            Channels = channels,
            Title = titleOverride ?? title,
            Body = bodyOverride ?? body,
            ResourceType = resourceType,
            ResourceId = resourceId,
            ClickAction = clickAction,
            EmailAddress = company.Email,
            EmailSubject = emailSubject,
            EmailHtmlBody = htmlBody,
            PushData = pushData,
            PushClickAction = pushClickAction
        };
    }
}
