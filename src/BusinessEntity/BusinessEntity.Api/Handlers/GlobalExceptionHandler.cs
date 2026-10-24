using BusinessEntity.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace BusinessEntity.Api.Handlers
{
    public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title, detail) = exception switch
            {
                NotFoundException =>
                    (
                        StatusCodes.Status404NotFound,
                        "Resource not found",
                        exception.Message
                    ),

                ConflictException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Conflict",
                        exception.Message
                    ),

                _ =>
                    (
                        StatusCodes.Status500InternalServerError,
                        "Internal server error",
                        "An unexpected error occurred."
                    )
            };

            if (statusCode == StatusCodes.Status404NotFound)
            {
                logger.LogInformation(
                    "Resource not found. StatusCode: {StatusCode}, ExceptionType: {ExceptionType}, TraceId: {TraceId}",
                    statusCode, exception.GetType().Name, httpContext.TraceIdentifier);
            }
            else if (statusCode == StatusCodes.Status409Conflict)
            {
                logger.LogWarning(
                    "Resource conflict. StatusCode: {StatusCode}, ExceptionType: {ExceptionType}, TraceId: {TraceId}",
                    statusCode, exception.GetType().Name, httpContext.TraceIdentifier);
            }
            else
            {
                logger.LogError(
                    exception,
                    "An unhandled exception occurred. StatusCode: {StatusCode}, TraceId: {TraceId}",
                    statusCode, httpContext.TraceIdentifier);
            }

            await Results.Problem(
                statusCode: statusCode,
                title: title,
                detail: detail,
                instance: httpContext.Request.Path.ToString(),
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = httpContext.TraceIdentifier
                }
            ).ExecuteAsync(httpContext);

            return true;
        }
    }
}
