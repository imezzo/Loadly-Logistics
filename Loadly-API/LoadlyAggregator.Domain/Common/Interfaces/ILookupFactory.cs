using LoadlyAggregator.Shared.DTOs.Common;
namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface ILookupFactory
{






    Task<LookupResponse> ExecuteLookupAsync(string entity, string lookup, LookupRequest request);
}
