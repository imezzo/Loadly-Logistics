namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IAuthenticatable
{
    string Email { get; set; }
    string UserName { get; set; }
    string Phone { get; set; }
    string Password { get; set; }
}
