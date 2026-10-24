using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinessEntity.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Executing {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        stopwatch.Stop();

        logger.LogInformation(
            "Completed {RequestName} in {ElapsedMilliseconds} ms",
            requestName, Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2));

        return response;
    }
}
