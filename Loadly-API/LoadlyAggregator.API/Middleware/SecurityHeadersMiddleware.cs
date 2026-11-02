namespace LoadlyAggregator.API.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();


            headers["X-Content-Type-Options"] = "nosniff";


            headers["X-Frame-Options"] = "DENY";


            headers["X-XSS-Protection"] = "1; mode=block";


            if (env.IsProduction())
            {

                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            }



            var connectSrc = env.IsProduction() ? "'self'" : "*";
            var imgSrc = env.IsProduction() ? "'self' data: https:" : "'self' data: https: http:";

            headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                $"img-src {imgSrc}; " +
                "font-src 'self' data:; " +
                $"connect-src {connectSrc}; " +
                "frame-ancestors 'none'; " +
                "base-uri 'self'; " +
                "form-action 'self'";

            return Task.CompletedTask;
        });

        await _next(context);
    }
}

