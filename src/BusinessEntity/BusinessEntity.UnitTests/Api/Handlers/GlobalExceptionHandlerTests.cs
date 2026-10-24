using System.Text.Json;
using BusinessEntity.Api.Handlers;
using BusinessEntity.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusinessEntity.UnitTests.Api.Handlers;

public sealed class GlobalExceptionHandlerTests
{
    [Theory]
    [InlineData(404, "Resource not found")]
    [InlineData(409, "Conflict")]
    [InlineData(500, "Internal server error")]
    public async Task TryHandleAsync_ReturnsExpectedProblemDetails(
        int statusCode, string title)
    {
        // Arrange: only HTTP objects and serialization services; no server is started.
        using var services = new ServiceCollection().AddLogging().AddOptions().BuildServiceProvider();
        using var body = new MemoryStream();
        var context = new DefaultHttpContext { RequestServices = services, TraceIdentifier = "test-trace" };
        context.Request.Path = "/api/business-entities/42";
        context.Response.Body = body;
        const string internalDetail = "Sensitive exception detail";
        Exception exception = statusCode switch
        {
            404 => new NotFoundException(internalDetail),
            409 => new ConflictException(internalDetail),
            _ => new InvalidOperationException(internalDetail)
        };
        var logger = new Mock<ILogger<GlobalExceptionHandler>>();
        var handler = new GlobalExceptionHandler(logger.Object);

        // Act
        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(handled);
        var log = Assert.Single(logger.Invocations, x => x.Method.Name == nameof(ILogger.Log));
        Assert.Equal(statusCode switch
        {
            404 => LogLevel.Information,
            409 => LogLevel.Warning,
            _ => LogLevel.Error
        }, log.Arguments[0]);
        if (statusCode == 500)
        {
            Assert.Same(exception, log.Arguments[3]);
        }
        else
        {
            Assert.Null(log.Arguments[3]);
        }
        var state = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object?>>>(log.Arguments[2]).ToDictionary();
        Assert.Equal(statusCode, state["StatusCode"]);
        Assert.Equal("test-trace", state["TraceId"]);
        if (statusCode is 404 or 409)
        {
            Assert.Equal(
                exception.GetType().Name,
                state["ExceptionType"]);
        }

        Assert.Equal(statusCode, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        body.Position = 0;
        using var document = await JsonDocument.ParseAsync(body);
        var problem = document.RootElement;
        Assert.Equal(statusCode, problem.GetProperty("status").GetInt32());
        Assert.Equal(title, problem.GetProperty("title").GetString());
        Assert.Equal(context.Request.Path.Value, problem.GetProperty("instance").GetString());
        Assert.Equal("test-trace", problem.GetProperty("traceId").GetString());
        Assert.Equal(statusCode == 500 ? "An unexpected error occurred." : internalDetail,
            problem.GetProperty("detail").GetString());
        if (statusCode == 500)
        {
            Assert.DoesNotContain(internalDetail, problem.GetRawText());
            Assert.DoesNotContain(nameof(InvalidOperationException), problem.GetRawText());
        }
    }
}
