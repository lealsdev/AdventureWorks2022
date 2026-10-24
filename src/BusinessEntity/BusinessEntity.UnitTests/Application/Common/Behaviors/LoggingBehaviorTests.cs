
using BusinessEntity.Application.Common.Behaviors;
using BusinessEntity.Application.Common.Exceptions;
using BusinessEntity.Application.Configuration;
using BusinessEntity.Application.Features.BusinessEntity.GetById;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusinessEntity.UnitTests.Application.Common.Behaviors;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_InvokesNextOnceWithToken_ReturnsResultAndLogsNameAndDuration()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();

        var logger = new Mock<ILogger<LoggingBehavior<GetBusinessEntityByIdQuery, GetBusinessEntityByIdResult>>>();

        var behavior = new LoggingBehavior<GetBusinessEntityByIdQuery, GetBusinessEntityByIdResult>(
            logger.Object);

        var expected = new GetBusinessEntityByIdResult(
            42,
            Guid.Empty,
            new DateTime(2026, 1, 2));

        var next = new Mock<RequestHandlerDelegate<GetBusinessEntityByIdResult>>(
            MockBehavior.Strict);

        next.Setup(x => x(cancellation.Token))
            .ReturnsAsync(expected);

        // Act
        var result = await behavior.Handle(
            new GetBusinessEntityByIdQuery(42),
            next.Object,
            cancellation.Token);

        // Assert
        Assert.Same(expected, result);

        next.Verify(x => x(cancellation.Token), Times.Once);
        next.VerifyNoOtherCalls();

        var logs = logger.Invocations
            .Where(x => x.Method.Name == nameof(ILogger.Log))
            .ToArray();

        Assert.Equal(2, logs.Length);

        Assert.All(logs, log =>
        {
            Assert.Equal(LogLevel.Information, log.Arguments[0]);
            Assert.Null(log.Arguments[3]);
        });

        var start = Assert
            .IsAssignableFrom<IEnumerable<KeyValuePair<string, object?>>>(
                logs[0].Arguments[2])
            .ToDictionary();

        Assert.Equal("Executing {RequestName}", start["{OriginalFormat}"]);

        Assert.Equal(
            nameof(GetBusinessEntityByIdQuery),
            start["RequestName"]);

        var completed = Assert
            .IsAssignableFrom<IEnumerable<KeyValuePair<string, object?>>>(
                logs[1].Arguments[2])
            .ToDictionary();

        Assert.Equal(
            "Completed {RequestName} in {ElapsedMilliseconds} ms",
            completed["{OriginalFormat}"]);

        Assert.Equal(
            nameof(GetBusinessEntityByIdQuery),
            completed["RequestName"]);

        Assert.True(
            Assert.IsType<double>(completed["ElapsedMilliseconds"]) >= 0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Handle_WhenNextThrows_PropagatesOriginalExceptionWithoutErrorOrSuccessLog(
        bool notFound)
    {
        // Arrange
        var logger = new Mock<ILogger<LoggingBehavior<GetBusinessEntityByIdQuery, GetBusinessEntityByIdResult>>>();

        var behavior = new LoggingBehavior<GetBusinessEntityByIdQuery, GetBusinessEntityByIdResult>(
            logger.Object);

        Exception expected = notFound
            ? new NotFoundException("Missing")
            : new InvalidOperationException("Internal detail");

        var next = new Mock<RequestHandlerDelegate<GetBusinessEntityByIdResult>>(
            MockBehavior.Strict);

        next.Setup(x => x(CancellationToken.None))
            .ThrowsAsync(expected);

        // Act
        var exception = await Record.ExceptionAsync(() => behavior.Handle(
            new GetBusinessEntityByIdQuery(42),
            next.Object,
            CancellationToken.None));

        // Assert
        Assert.Same(expected, exception);

        next.Verify(x => x(CancellationToken.None), Times.Once);

        AssertOnlyStartLogged(logger);
    }

    [Fact]
    public async Task Handle_WhenNextIsCancelled_PropagatesCancellationWithoutSuccessLog()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var logger = new Mock<ILogger<LoggingBehavior<GetBusinessEntityByIdQuery, GetBusinessEntityByIdResult>>>();

        var behavior = new LoggingBehavior<GetBusinessEntityByIdQuery, GetBusinessEntityByIdResult>(
            logger.Object);

        var next = new Mock<RequestHandlerDelegate<GetBusinessEntityByIdResult>>(
            MockBehavior.Strict);

        next.Setup(x => x(cancellation.Token))
            .Returns(Task.FromCanceled<GetBusinessEntityByIdResult>(
                cancellation.Token));

        // Act
        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => behavior.Handle(
                new GetBusinessEntityByIdQuery(42),
                next.Object,
                cancellation.Token));

        // Assert
        Assert.Equal(cancellation.Token, exception.CancellationToken);

        next.Verify(x => x(cancellation.Token), Times.Once);

        AssertOnlyStartLogged(logger);
    }

    [Fact]
    public void AddApplicationConfiguration_RegistersLoggingBehaviorOnce()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationConfiguration();

        // Assert
        var registration = Assert.Single(
            services,
            x => x.ServiceType == typeof(IPipelineBehavior<,>)
                && x.ImplementationType == typeof(LoggingBehavior<,>));

        Assert.Equal(ServiceLifetime.Transient, registration.Lifetime);
    }

    private static void AssertOnlyStartLogged(
        Mock<ILogger<LoggingBehavior<GetBusinessEntityByIdQuery, GetBusinessEntityByIdResult>>> logger)
    {
        var log = Assert.Single(
            logger.Invocations,
            x => x.Method.Name == nameof(ILogger.Log));

        Assert.Equal(LogLevel.Information, log.Arguments[0]);
        Assert.Null(log.Arguments[3]);

        var state = Assert
            .IsAssignableFrom<IEnumerable<KeyValuePair<string, object?>>>(
                log.Arguments[2])
            .ToDictionary();

        Assert.Equal(
            "Executing {RequestName}",
            state["{OriginalFormat}"]);

        Assert.Equal(
            nameof(GetBusinessEntityByIdQuery),
            state["RequestName"]);
    }
}
