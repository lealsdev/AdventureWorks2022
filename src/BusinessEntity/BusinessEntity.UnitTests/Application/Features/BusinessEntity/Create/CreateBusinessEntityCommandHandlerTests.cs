using BusinessEntity.Application.Abstractions.Persistence;
using BusinessEntity.Application.Features.BusinessEntity.Create;
using BusinessEntity.Domain.Entities;
using Moq;

namespace BusinessEntity.UnitTests.Application.Features.BusinessEntity.Create;

public sealed class CreateBusinessEntityCommandHandlerTests
{
    [Fact]
    public async Task Handle_AddsNewEntityThenSavesOnce_AndReturnsIdGeneratedDuringSave()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;

        BusinessEntityEntity? addedEntity = null;

        var repository = new Mock<IBusinessEntityRepository>(MockBehavior.Strict);
        repository.Setup(x => x.AddAsync(It.IsAny<BusinessEntityEntity>(), token))
            .Callback<BusinessEntityEntity, CancellationToken>((entity, _) =>
            {
                Assert.Equal(0, entity.BusinessEntityId);
                Assert.Equal(Guid.Empty, entity.RowGuid);
                Assert.Equal(default(DateTime), entity.ModifiedDate);
                addedEntity = entity;
            })
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.Setup(x => x.SaveChangesAsync(token)).Callback(() =>
        {
            Assert.NotNull(addedEntity);
            // Simulate SQL Server assigning the identity when SaveChanges completes.
            typeof(BusinessEntityEntity).GetProperty(nameof(BusinessEntityEntity.BusinessEntityId))!
                .SetValue(addedEntity, 42);
        }).ReturnsAsync(1);

        var handler = new CreateBusinessEntityCommandHandler(repository.Object, unitOfWork.Object);

        // Act
        var result = await handler.Handle(new CreateBusinessEntityCommand(), token);

        // Assert
        Assert.Equal(42, result.BusinessEntityId);
        repository.Verify(x => x.AddAsync(It.Is<BusinessEntityEntity>(e => ReferenceEquals(e, addedEntity)), token), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
        repository.VerifyNoOtherCalls();
        unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenAddFails_PropagatesExceptionAndDoesNotSave()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;

        var expected = new InvalidOperationException("Cannot add entity.");

        var repository = new Mock<IBusinessEntityRepository>(MockBehavior.Strict);
        repository.Setup(x => x.AddAsync(It.IsAny<BusinessEntityEntity>(), token)).ThrowsAsync(expected);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var handler = new CreateBusinessEntityCommandHandler(repository.Object, unitOfWork.Object);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new CreateBusinessEntityCommand(), token));

        // Assert
        Assert.Same(expected, exception);
        repository.Verify(x => x.AddAsync(It.IsAny<BusinessEntityEntity>(), token), Times.Once);
        repository.VerifyNoOtherCalls();
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenSaveFails_PropagatesExceptionWithoutReturningSuccess()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        var expected = new InvalidOperationException("Cannot persist entity.");
        var repository = new Mock<IBusinessEntityRepository>(MockBehavior.Strict);
        repository.Setup(x => x.AddAsync(It.IsAny<BusinessEntityEntity>(), token)).Returns(Task.CompletedTask);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.Setup(x => x.SaveChangesAsync(token)).ThrowsAsync(expected);
        var handler = new CreateBusinessEntityCommandHandler(repository.Object, unitOfWork.Object);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new CreateBusinessEntityCommand(), token));

        // Assert
        Assert.Same(expected, exception);
        repository.Verify(x => x.AddAsync(It.IsAny<BusinessEntityEntity>(), token), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
        repository.VerifyNoOtherCalls();
        unitOfWork.VerifyNoOtherCalls();
    }
}
