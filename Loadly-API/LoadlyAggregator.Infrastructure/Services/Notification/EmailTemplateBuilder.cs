namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class EmailTemplateBuilder : Domain.Common.Interfaces.IEmailTemplateBuilder
{
    private const string Signature = "Best regards,<br>Loadly Aggregator Team";

    public string BuildCompanyTripEmail(string recipientName, string bodyContent, string heading)
    {
        return $@"
            <h2>{heading}</h2>
            <p>Dear {recipientName},</p>
            <p>{bodyContent}</p>
            <br>
            <p>{Signature}</p>
        ";
    }
}
