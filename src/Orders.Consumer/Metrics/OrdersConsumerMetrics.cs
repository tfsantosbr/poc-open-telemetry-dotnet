using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Orders.Consumer.Metrics;

public class OrdersConsumerMetrics : IDisposable
{
    // Fields

    internal const string ActivitySourceName = "Orders.Consumer.Metrics";
    internal const string MeterName = "Orders.Consumer.Metrics";
    private readonly Meter Meter;
    public ActivitySource ActivitySource { get; }

    // Constructor

    public OrdersConsumerMetrics()
    {
        string? version = typeof(OrdersConsumerMetrics).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        Meter = new Meter(MeterName, version);

        OrderProcessingDuration = Meter.CreateHistogram<long>(
            name: "orders.processing.duration",
            unit: "milliseconds",
            description: "Duration of orders processing in milliseconds");
    }

    // Properties

    private Histogram<long> OrderProcessingDuration { get; }

    // Public Methods

    public void RecordOrderProcessingDuration(long duration) => OrderProcessingDuration.Record(duration);

    public void Dispose()
    {
        Meter.Dispose();
        ActivitySource.Dispose();

        GC.SuppressFinalize(this);
    }
}
