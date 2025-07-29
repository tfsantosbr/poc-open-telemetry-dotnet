
using System.Diagnostics;

namespace Shared.Correlation.Context;

public class CorrelationContext : ICorrelationContext
{
    private readonly AsyncLocal<string> _correlationId = new();

    public string CreateCorrelationId()
    {
        var correlationId = Guid.NewGuid().ToString("N");

        SetCorrelationId(correlationId);

        return correlationId;
    }

    public string? GetCorrelationId()
    {
        return _correlationId.Value;
    }

    public void SetCorrelationId(string correlationId)
    {
        _correlationId.Value = correlationId;

        Activity.Current?.SetTag("correlation_id", correlationId);
    }
}
