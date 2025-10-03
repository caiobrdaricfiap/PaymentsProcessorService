using FC.Codeflix.Catalog.UnitTests.Application.Category.Common;
using FiapCloudGameWebAPI.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Payments.Domain.Enums;
using Payments.Domain.Models;
using PaymentsProcessorService.Application.DTOs;
using PaymentsProcessorService.Application.Services;
using PaymentsProcessorService.Infra.Integration.PaymentService;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.CreateCategory
{
    [CollectionDefinition(nameof(ProcessAsyncTestFixture))]
    public class ProcessAsyncTestFixtureCollection : ICollectionFixture<ProcessAsyncTestFixture>
    { }
    public class ProcessAsyncTestFixture : PaymentsBaseFixture
    {
        public int UserId { get; } = 1;
        public int GameId { get; } = 2;
        public decimal Amount { get; } = 100m;
        public Currency Currency { get; } = Currency.BRL;

        public PaymentProcessInputDto GetValidDto() => new()
        {
            UserId = UserId,
            GameId = GameId,
            Amount = Amount,
            Currency = Currency
        };

        public PaymentCreatedEvent GetValidPaymentCreatedEvent() => new()
        {
            Id = Guid.NewGuid(),
            UserId = UserId,
            GameId = GameId,
            Amount = Amount,
            Status = PaymentStatus.Processing,
            Currency = Currency,
            CreatedAt = DateTime.UtcNow
        };

        public PaymentResult GetSuccessPaymentResult() => new()
        {
            Success = true,
            Message = "Pagamento aprovado"
        };

        public Mock<IPaymentCreatedEventRepository> PaymentCreatedEventRepoMock { get; }
        public Mock<IPaymentStatusChangedEventRepository> PaymentStatusChangedEventRepoMock { get; }
        public Mock<PaymentServiceIntegration> PaymentServiceIntegrationMock { get; }
        public Mock<SendEmailStatusFunction> SendEmailStatusFunctionMock { get; }

        public PaymentService PaymentService { get; }

        public ProcessAsyncTestFixture()
        {
            var configurationMock = new Mock<IConfiguration>();

            configurationMock.Setup(c => c["SendEmailFunctionBaseUrl"]).Returns("http://fake-url");
            configurationMock.Setup(c => c["FunctionCode"]).Returns("fake-code");
            var httpClient = new HttpClient();
            var loggerMock = new Mock<ILogger<SendEmailStatusFunction>>();

            


            PaymentCreatedEventRepoMock = new Mock<IPaymentCreatedEventRepository>();
            PaymentStatusChangedEventRepoMock = new Mock<IPaymentStatusChangedEventRepository>();
            PaymentServiceIntegrationMock = new Mock<PaymentServiceIntegration>();
            SendEmailStatusFunctionMock = new Mock<SendEmailStatusFunction>(
            configurationMock.Object,
            httpClient,
            loggerMock.Object
            );

            PaymentService = new PaymentService(
                PaymentStatusChangedEventRepoMock.Object,
                PaymentCreatedEventRepoMock.Object,
                PaymentServiceIntegrationMock.Object,
                SendEmailStatusFunctionMock.Object
            );
        }

        

    }
}
