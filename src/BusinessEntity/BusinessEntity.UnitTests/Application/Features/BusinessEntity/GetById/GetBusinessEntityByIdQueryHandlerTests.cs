using BusinessEntity.Application.Abstractions.Persistence;
using BusinessEntity.Application.Common.Exceptions;
using BusinessEntity.Application.Features.BusinessEntity.GetById;
using BusinessEntity.Domain.Entities;
using Moq;

namespace BusinessEntity.UnitTests.Application.Features.BusinessEntity.GetById;

public sealed class GetBusinessEntityByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenEntityExists_ReturnsAllPersistedValuesWithoutWriting()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();

        var token = cancellation.Token;

        var entity = BusinessEntityEntity.Create();

        var rowGuid = Guid.Parse("8e1e7010-aeb7-4329-9b43-a727241d149a");

        var modifiedDate = new DateTime(2026, 1, 2, 3, 4, 5);

        // Simulate materialization of SQL-generated values without changing the domain API.
        typeof(BusinessEntityEntity).GetProperty(nameof(entity.BusinessEntityId))!.SetValue(entity, 42);
        typeof(BusinessEntityEntity).GetProperty(nameof(entity.RowGuid))!.SetValue(entity, rowGuid);
        typeof(BusinessEntityEntity).GetProperty(nameof(entity.ModifiedDate))!.SetValue(entity, modifiedDate);

        var repository = new Mock<IBusinessEntityRepository>(MockBehavior.Strict);
        repository.Setup(x => x.GetByIdAsync(42, token)).ReturnsAsync(entity);

        var handler = new GetBusinessEntityByIdQueryHandler(repository.Object);

        // Act
        var result = await handler.Handle(new GetBusinessEntityByIdQuery(42), token);

        // Assert
        Assert.Equal(new GetBusinessEntityByIdResult(42, rowGuid, modifiedDate), result);
        repository.Verify(x => x.GetByIdAsync(42, token), Times.Once);
        repository.Verify(x => x.AddAsync(It.IsAny<BusinessEntityEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenEntityDoesNotExist_ThrowsNotFoundWithoutWriting()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();

        var token = cancellation.Token;

        var repository = new Mock<IBusinessEntityRepository>(MockBehavior.Strict);
        repository.Setup(x => x.GetByIdAsync(42, token)).ReturnsAsync((BusinessEntityEntity?)null);

        var handler = new GetBusinessEntityByIdQueryHandler(repository.Object);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetBusinessEntityByIdQuery(42), token));

        // Assert
        Assert.Equal(nameof(BusinessEntityEntity), exception.Message);
        repository.Verify(x => x.GetByIdAsync(42, token), Times.Once);
        repository.Verify(x => x.AddAsync(It.IsAny<BusinessEntityEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.VerifyNoOtherCalls();
    }
}
