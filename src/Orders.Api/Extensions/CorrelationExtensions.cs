using Shared.Correlation.Context;

namespace Orders.Api.Extensions;

public static class CorrelationExtensions
{
    const string correlationHeader = "Correlation-Id";

    public static IApplicationBuilder UseCorrelationContext(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var correlationContext = context.RequestServices.GetRequiredService<ICorrelationContext>();
            var correlationId = context.Request.Headers[correlationHeader].FirstOrDefault();

            if (string.IsNullOrEmpty(correlationId))
                correlationId = correlationContext.CreateCorrelationId();
            else
                correlationContext.SetCorrelationId(correlationId);

            context.Response.Headers.Append(correlationHeader, correlationId);

            await next(context);
        });

        return app;
    }
}
