using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface IResetPasswordUrlService
{






    string GenerateResetPasswordUrl(string module, string token);






    string GetBaseUrl(string module);
}

