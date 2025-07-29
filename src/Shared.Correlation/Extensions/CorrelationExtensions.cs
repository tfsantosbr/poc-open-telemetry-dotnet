using Microsoft.Extensions.DependencyInjection;
using Shared.Correlation.Context;

namespace Shared.Correlation.Extensions;

public static class CorrelationExtensions
{
    public static IServiceCollection AddCorrelationContext(this IServiceCollection services)
    {
        services.AddSingleton<ICorrelationContext, CorrelationContext>();

        return services;
    }
}
