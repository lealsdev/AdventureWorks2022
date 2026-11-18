using Moq;
using People.Application.Abstractions;
using People.Application.Common.Exceptions;
using People.Application.Features.Person.GetById;
using People.Domain.Entities;

namespace People.UnitTests.Application.Features.Person.GetById;

public sealed class GetPersonByIdQueryHandlerTests
{

    [Fact]
    public async Task Handle_MapsEveryProperty_AndOnlyReadsOnce()
    {
        using var cancellation = new CancellationTokenSource();

        var expected = TestPerson.Result();
        
        var repository = new Mock<IPersonRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetByIdAsync(42, cancellation.Token)).ReturnsAsync(TestPerson.Entity(expected));
        
        var actual = await new GetPersonByIdQueryHandler(repository.Object).Handle(new(42), cancellation.Token);

        Assert.Equal(expected, actual);
        Assert.Same(expected.AdditionalContactInfo, actual.AdditionalContactInfo);
        Assert.Same(expected.Demographics, actual.Demographics);

        repository.Verify(r => r.GetByIdAsync(42, cancellation.Token), Times.Once);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_MissingPerson_ThrowsNotFound_AndOnlyReadsOnce()
    {
        using var cancellation = new CancellationTokenSource();
        
        var repository = new Mock<IPersonRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetByIdAsync(42, cancellation.Token)).ReturnsAsync((PersonEntity?)null);
        
        var error = await Assert.ThrowsAsync<NotFoundException>(() => new GetPersonByIdQueryHandler(repository.Object).Handle(new(42), cancellation.Token));
        
        Assert.Equal(nameof(PersonEntity), error.Message);

        repository.Verify(r => r.GetByIdAsync(42, cancellation.Token), Times.Once);
        repository.VerifyNoOtherCalls();
    }
}
