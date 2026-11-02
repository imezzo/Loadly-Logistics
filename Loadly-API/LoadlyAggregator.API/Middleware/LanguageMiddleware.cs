using System.Globalization;

namespace LoadlyAggregator.API.Middleware;

public class LanguageMiddleware
{
    private readonly RequestDelegate _next;
    private const string LanguageHeader = "lang";
    private const string DefaultLanguage = "en";
    public const string LanguageContextKey = "CurrentLanguage";

    public LanguageMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        var languageHeader = context.Request.Headers[LanguageHeader].FirstOrDefault()
            ?? context.Request.Headers["Lang"].FirstOrDefault()
            ?? context.Request.Headers["LANG"].FirstOrDefault();

        var language = languageHeader?.Trim().ToLower();


        if (string.IsNullOrEmpty(language) || (language != "en" && language != "ar"))
        {
            language = DefaultLanguage;
        }


        context.Items[LanguageContextKey] = language;

        var culture = new CultureInfo(language);
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;


        context.Response.Headers[LanguageHeader] = language;

        await _next(context);
    }
}

