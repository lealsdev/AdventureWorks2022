using BusinessEntity.Api.Features.BusinessEntity.V1.GetById;
using BusinessEntity.Application.Features.BusinessEntity.GetById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BusinessEntity.UnitTests.Api.Controllers.BusinessEntity.V1;

public sealed class GetBusinessEntityByIdControllerTests
{
    [Fact]
    public async Task Get_SendsQueryWithIdAndToken_AndReturnsOkWithResult()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;

        var expected = new GetBusinessEntityByIdResult(42, Guid.Parse("8e1e7010-aeb7-4329-9b43-a727241d149a"), new DateTime(2026, 1, 2));

        var sender = new Mock<ISender>(MockBehavior.Strict);
        sender.Setup(x => x.Send(It.Is<GetBusinessEntityByIdQuery>(q => q.Id == 42), token))
            .ReturnsAsync(expected);

        var controller = new GetBusinessEntityByIdController(sender.Object);

        // Act
        var result = await controller.Get(42, token);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Same(expected, ok.Value);
        sender.Verify(x => x.Send(It.Is<GetBusinessEntityByIdQuery>(q => q.Id == 42), token), Times.Once);
        sender.VerifyNoOtherCalls();
    }
}
