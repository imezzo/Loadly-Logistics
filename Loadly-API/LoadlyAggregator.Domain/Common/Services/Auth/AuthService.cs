using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;

namespace LoadlyAggregator.Domain.Common.Services.Auth;

public class AuthService
{
    private readonly IPasswordHashingService _passwordHashingService;

    public AuthService(IPasswordHashingService passwordHashingService)
    {
        _passwordHashingService = passwordHashingService;
    }

    public void ValidateCredentials<T>(T? user, string providedPassword)
        where T : BaseUserEntity
    {
        UserValidator.ValidateUserExists(user, typeof(T).Name);
        UserValidator.ValidatePassword(_passwordHashingService, providedPassword, user!.Password);
        UserValidator.ValidateStatus(user.Status);

    }
}

