using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using Shared.Correlation.Context;
using Shared.Correlation.OpenTelemetry.Processors;

namespace Shared.Correlation.OpenTelemetry.Extensions;

public static class OpenTelemetryExtensions
{
    public static OpenTelemetryLoggerOptions AddCorrelationLogProcessor(this OpenTelemetryLoggerOptions options)
    {
        return options.AddProcessor(serviceProvider =>
            new CorrelationLogProcessor(serviceProvider.GetRequiredService<ICorrelationContext>()));
    }
}
