using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Orders.Api.Metrics;

public class OrderMetrics : IDisposable
{
    // Fields

    internal const string ActivitySourceName = "Orders.Api.Metrics.OrderMetrics";
    internal const string MeterName = "Orders.Api.Metrics.OrderMetrics";
    private readonly Meter Meter;
    public ActivitySource ActivitySource { get; }

    // Constructor

    public OrderMetrics()
    {
        string? version = typeof(OrderMetrics).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        Meter = new Meter(MeterName, version);

        TotalOrderRequest = Meter.CreateCounter<int>(name: "orders.requests.total", description: "Total number of order requests");
    }

    // Properties

    private Counter<int> TotalOrderRequest { get; }

    // Public Methods

    public void SumOrderRequest() => TotalOrderRequest.Add(1);

    public void Dispose()
    {
        ActivitySource.Dispose();
        Meter.Dispose();

        GC.SuppressFinalize(this);
    }
}
