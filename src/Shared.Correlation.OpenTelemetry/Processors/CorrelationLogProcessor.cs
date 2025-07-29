using OpenTelemetry;
using OpenTelemetry.Logs;
using Shared.Correlation.Context;

namespace Shared.Correlation.OpenTelemetry.Processors;

public class CorrelationLogProcessor(ICorrelationContext correlationContext) : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord record)
    {
        var correlationId = correlationContext.GetCorrelationId();

        if (string.IsNullOrEmpty(correlationId))
            return;

        var correlationAttribute = new KeyValuePair<string, object?>("correlation_id", correlationId);

        record.Attributes ??= [];
        record.Attributes = [.. record.Attributes, correlationAttribute];

        base.OnEnd(record);
    }
}
