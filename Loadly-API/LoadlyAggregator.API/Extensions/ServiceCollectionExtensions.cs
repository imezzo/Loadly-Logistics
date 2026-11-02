using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using LoadlyAggregator.API.Filters;

namespace LoadlyAggregator.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Do not use AddFluentValidationAutoValidation() — the pipeline is synchronous and cannot run async rules.
        // AsyncFluentValidationFilter runs validators with ValidateAsync so validators can use IsUnique, MustAsync, etc.
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}

