namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface ICurrentUserService
{




    long? GetCurrentUserId();




    string? GetCurrentUserType();




    bool IsAuthenticated();
}
