using FC.Codeflix.Catalog.UnitTests.Application.Category.CreateCategory;
using FiapCloudGames.Domain.Entities;
using FluentAssertions;
using Moq;
using Payments.Domain.Models;
using System.Linq.Expressions;

namespace PaymentsProcessorService.UnitTests.Payment.Process;

[CollectionDefinition(nameof(ProcessAsyncTestFixture))]
public class ProcessAsyncTestFixtureCollection : ICollectionFixture<ProcessAsyncTestFixture> { }

[Collection(nameof(ProcessAsyncTestFixture))]
public class ProcessAsyncTest
{
    private readonly ProcessAsyncTestFixture _fixture;

    public ProcessAsyncTest(ProcessAsyncTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should process payment successfully")]
    public async Task ProcessAsync_ShouldProcessPaymentSuccessfully()
    {
        // Arrange
        var dto = _fixture.GetValidDto();
        var paymentCreatedEvent = _fixture.GetValidPaymentCreatedEvent();
        var paymentResult = _fixture.GetSuccessPaymentResult();

        _fixture.PaymentCreatedEventRepoMock
            .Setup(r => r.GetFirstOrDefaultByConditionAsync(It.IsAny<Expression<Func<PaymentCreatedEvent, bool>>>()))
            .ReturnsAsync((PaymentCreatedEvent)null);

        _fixture.PaymentCreatedEventRepoMock
            .Setup(r => r.AddAsync(It.IsAny<PaymentCreatedEvent>()))
            .ReturnsAsync(paymentCreatedEvent);

        _fixture.PaymentServiceIntegrationMock
            .Setup(i => i.ProcessPayment(It.IsAny<PaymentCreatedEvent>()))
            .Returns(paymentResult);

        _fixture.SendEmailStatusFunctionMock
            .Setup(s => s.SendEmailStatusAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _fixture.PaymentStatusChangedEventRepoMock
            .Setup(r => r.AddAsync(It.IsAny<PaymentStatusChangedEvent>()))
            .ReturnsAsync(new PaymentStatusChangedEvent());

        // Act
        var response = await _fixture.PaymentService.ProcessAsync(dto);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Processo finalizado");
        response.Data.Should().Be(paymentCreatedEvent.Id.ToString());
    }
}