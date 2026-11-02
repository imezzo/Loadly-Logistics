namespace LoadlyAggregator.Domain.Company.Services;

/// <summary>
/// Transactional emails for company employees (portal + admin-created users), via <see cref="Common.Interfaces.INotificationTransport"/>.
/// </summary>
public interface ICompanyUserNotificationService
{
    Task SendWelcomeSetPasswordEmailAsync(Common.Entities.Company user, string resetPasswordUrl);
}
