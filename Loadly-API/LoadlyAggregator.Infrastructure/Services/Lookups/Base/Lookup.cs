using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;
namespace LoadlyAggregator.Infrastructure.Services.Lookups.Base;




public abstract class Lookup
{
    protected string Method { get; set; } = "defaultLookups";
    protected readonly ILanguageService LanguageService;
    protected Lookup(ILanguageService languageService)
    {
        LanguageService = languageService;
    }



    public abstract Task<LookupResponse> ExecuteAsync(LookupRequest request);
}
