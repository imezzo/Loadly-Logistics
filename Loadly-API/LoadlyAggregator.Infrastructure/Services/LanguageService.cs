using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Services;

public class LanguageService : ILanguageService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string LanguageContextKey = "CurrentLanguage";
    private const string DefaultLanguage = "en";
    public LanguageService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string GetCurrentLanguage()
    {

        var context = _httpContextAccessor.HttpContext;
        return context?.Items[LanguageContextKey]?.ToString() ?? DefaultLanguage;
    }
    public bool IsArabic()
    {
        return GetCurrentLanguage() == "ar";
    }
    public bool IsEnglish()
    {
        return GetCurrentLanguage() == "en";
    }
}
