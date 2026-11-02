namespace LoadlyAggregator.Domain.Common.Interfaces;





public interface IMfaCodeService
{




    string GenerateCode(long userId, string userType);




    bool VerifyCode(long userId, string userType, string code);
}

