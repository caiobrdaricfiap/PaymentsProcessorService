using FiapCloudGames.Domain.Entities;
using FiapCloudGameWebAPI.Domain.Interfaces.Repositories;
using Payments.Domain.Enums;
using Payments.Domain.Models;
using PaymentsProcessorService.Application.DTOs;
using PaymentsProcessorService.Infra.Integration.PaymentService;

namespace Payments.Application.Services
{
    public class PaymentService
    {
        private readonly IPaymentStatusChangedEventRepository _paymentStatusChangedEventRepository;
        private readonly IPaymentCreatedEventRepository _paymentCreatedEventRepository;
        private readonly PaymentServiceIntegration _paymentServiceIntegration;

        public PaymentService(IPaymentStatusChangedEventRepository paymentStatusChangedEventRepository, IPaymentCreatedEventRepository paymentCreatedEventRepository, PaymentServiceIntegration paymentServiceIntegration)
        {
            _paymentStatusChangedEventRepository = paymentStatusChangedEventRepository;
            _paymentCreatedEventRepository = paymentCreatedEventRepository;
            _paymentServiceIntegration = paymentServiceIntegration;
        }

        public async Task<ApiResponse> ProcessAsync(PaymentProcessInputDto dto)
        {
            try
            {
                PaymentCreatedEvent? paymentCreatedEvent;

                paymentCreatedEvent = await _paymentCreatedEventRepository
                    .GetFirstOrDefaultByConditionAsync(e =>
                        e.UserId == dto.UserId &&
                        e.GameId == dto.GameId);

                if (paymentCreatedEvent is not null)
                {
                    var updatedEvent = await _paymentStatusChangedEventRepository
                        .GetFirstOrDefaultByConditionAsync(p =>
                        p.PaymentId == paymentCreatedEvent.Id &&
                        p.NewStatus != PaymentStatus.Failed);

                    if (updatedEvent != null)
                        return ApiResponse.Fail("Já existe um pagamento em processamente ou o jogador já possui o jogo.");
                }

                paymentCreatedEvent = new()
                {
                    UserId = dto.UserId,
                    GameId = dto.GameId,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    Status = PaymentStatus.Processing,
                    CreatedAt = DateTime.UtcNow
                };

                var eventCreated = await _paymentCreatedEventRepository.AddAsync(paymentCreatedEvent);

                await _paymentStatusChangedEventRepository.AddAsync(new PaymentStatusChangedEvent
                {
                    PaymentId = eventCreated.Id,
                    OldStatus = 0,
                    NewStatus = PaymentStatus.Processing,
                    ChangedAt = DateTime.UtcNow
                });

                // Simular integração com gateway de pagamento
                var result = _paymentServiceIntegration.ProcessPayment(eventCreated);

                if (result.Success)
                {
                    await _paymentStatusChangedEventRepository.AddAsync(new()
                    {
                        PaymentId = eventCreated.Id,
                        OldStatus = eventCreated.Status,
                        NewStatus = PaymentStatus.Processed,
                        Observation = result.Message,
                        ChangedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    await _paymentStatusChangedEventRepository.AddAsync(new()
                    {
                        PaymentId = eventCreated.Id,
                        OldStatus = eventCreated.Status,
                        NewStatus = PaymentStatus.Failed,
                        Observation = result.Message,
                        ChangedAt = DateTime.UtcNow
                    });
                }

                return ApiResponse.Ok(eventCreated.Id.ToString(), "Processo finalizado");
            }
            catch (Exception e)
            {
                return ApiResponse.Fail(e.Message);
            }
        }

        public async Task<ApiResponse> GetByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                    return ApiResponse.Fail($"Id da transação ID: {id} é invalido");

                var createdEvent = await _paymentCreatedEventRepository.GetAsync(guidId);

                if (createdEvent == null)
                    return ApiResponse.Fail($"Nenhuma transação encontrada com o ID: {id}");

                // 2. Recupera todos os eventos de alteração de status para esse pagamento
                var statusEvents = await _paymentStatusChangedEventRepository
                    .GetListByConditionAsync(e => e.PaymentId == guidId);

                // 3. Reconstrói o estado do pagamento
                var payment = new PaymentOutputDto
                {
                    Id = createdEvent.Id,
                    UserId = createdEvent.UserId,
                    GameId = createdEvent.GameId,
                    Amount = createdEvent.Amount,
                    Currency = createdEvent.Currency.ToString(),
                    Observation = createdEvent.Observation,
                    CreatedAt = createdEvent.CreatedAt,
                    UpdatedAt = createdEvent.CreatedAt
                };

                // Aplica os eventos de status em ordem cronológica
                foreach (var statusEvent in statusEvents.OrderBy(e => e.ChangedAt))
                {
                    payment.Status = statusEvent.NewStatus.ToString();
                    payment.UpdatedAt = statusEvent.ChangedAt;
                }

                return ApiResponse.Ok(payment);
            }
            catch (Exception e)
            {
                return ApiResponse.Fail(e.Message);
            }
        }

        public async Task<ApiResponse> GetAllByUserAsync(int userId)
        {
            try
            {
                // 1. Recupera todos os eventos de criação do usuário
                var createdEvents = await _paymentCreatedEventRepository.GetListByConditionAsync(e => e.UserId == userId);

                if (createdEvents == null || !createdEvents.Any())
                    return ApiResponse.Fail($"Nenhuma transação encontrada para o usuario ID: {userId}");

                var payments = new List<PaymentOutputDto>();

                foreach (var createdEvent in createdEvents)
                {
                    // 2. Recupera todos os eventos de alteração de status para esse pagamento
                    var statusEvents = await _paymentStatusChangedEventRepository
                        .GetListByConditionAsync(e => e.PaymentId == createdEvent.Id);

                    // 3. Reconstrói o estado do pagamento
                    var payment = new PaymentOutputDto
                    {
                        UserId = createdEvent.UserId,
                        GameId = createdEvent.GameId,
                        Amount = createdEvent.Amount,
                        Currency = createdEvent.Currency.ToString(),
                        Observation = createdEvent.Observation,
                        Id = createdEvent.Id,
                        CreatedAt = createdEvent.CreatedAt,
                        UpdatedAt = createdEvent.CreatedAt
                    };

                    foreach (var statusEvent in statusEvents.OrderBy(e => e.ChangedAt))
                    {
                        payment.Status = statusEvent.NewStatus.ToString();
                        payment.UpdatedAt = statusEvent.ChangedAt;
                    }

                    payments.Add(payment);
                }

                return ApiResponse.Ok(payments);

            }
            catch (Exception e)
            {
                return ApiResponse.Fail(e.Message);
            }
        }
    }
}
