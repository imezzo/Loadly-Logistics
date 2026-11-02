namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IEmailTemplateBuilder
{
    string BuildCompanyTripEmail(string recipientName, string bodyContent, string heading);
}
