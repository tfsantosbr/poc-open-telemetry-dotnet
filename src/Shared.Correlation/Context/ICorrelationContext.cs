namespace Shared.Correlation.Context;

public interface ICorrelationContext
{
    string CreateCorrelationId();
    string? GetCorrelationId();
    void SetCorrelationId(string correlationId);
}
