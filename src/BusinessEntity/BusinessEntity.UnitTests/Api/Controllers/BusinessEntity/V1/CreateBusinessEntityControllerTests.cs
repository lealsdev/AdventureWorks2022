using BusinessEntity.Api.Configuration;
using BusinessEntity.Api.Controllers.BusinessEntity.V1;
using BusinessEntity.Application.Features.BusinessEntity.Create;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BusinessEntity.UnitTests.Api.Controllers.BusinessEntity.V1;

public sealed class CreateBusinessEntityControllerTests
{
    [Fact]
    public async Task Create_SendsCommandWithToken_AndReturnsCreatedAtNamedRouteWithIdAndBody()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;

        var expected = new CreateBusinessEntityCommandResult(42);

        var sender = new Mock<ISender>(MockBehavior.Strict);

        sender.Setup(x => x.Send(It.IsAny<CreateBusinessEntityCommand>(), token)).ReturnsAsync(expected);

        var controller = new CreateBusinessEntityController(sender.Object);

        // Act
        var result = await controller.Create(token);

        // Assert
        var created = Assert.IsType<CreatedAtRouteResult>(result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal(ApiEndpoints.BusinessEntity.V1.GetByIdName, created.RouteName);
        Assert.NotNull(created.RouteValues);
        Assert.Equal(42, Assert.IsType<int>(created.RouteValues["id"]));
        Assert.Same(expected, created.Value);
        sender.Verify(x => x.Send(It.IsAny<CreateBusinessEntityCommand>(), token), Times.Once);
        sender.VerifyNoOtherCalls();
    }
}
