using People.Api.Controllers.Person.V1;
using People.Application.Features.Person.GetById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace People.UnitTests.Api.Controllers.Person.V1;

public sealed class GetPersonByIdControllerTests
{
    [Fact]
    public async Task Get_SendsQueryWithIdAndToken_AndReturnsOkWithResult()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;

        var expected = TestPerson.Result();

        var sender = new Mock<ISender>(MockBehavior.Strict);
        sender.Setup(x => x.Send(It.Is<GetPersonByIdQuery>(q => q.BusinessEntityId == 42), token))
            .ReturnsAsync(expected);

        var controller = new GetPersonByIdController(sender.Object);

        // Act
        var result = await controller.Get(42, token);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Same(expected, ok.Value);
        sender.Verify(x => x.Send(It.Is<GetPersonByIdQuery>(q => q.BusinessEntityId == 42), token), Times.Once);
        sender.VerifyNoOtherCalls();
    }

    [Fact]
    public void Result_SerializesContactAndDemographicsAsObjects()
    {
        using var json = System.Text.Json.JsonDocument.Parse(
            System.Text.Json.JsonSerializer.Serialize(
                TestPerson.Result(), new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)));

        Assert.Equal(System.Text.Json.JsonValueKind.Object, json.RootElement.GetProperty("additionalContactInfo").ValueKind);
        Assert.Equal(System.Text.Json.JsonValueKind.Object, json.RootElement.GetProperty("demographics").ValueKind);
        Assert.Equal(2, json.RootElement.GetProperty("demographics").GetProperty("totalChildren").GetInt32());
    }
}
