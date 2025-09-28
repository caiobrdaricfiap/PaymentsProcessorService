using FiapCloudGames.Domain.Entities;
using FiapCloudGameWebAPI.Domain.Interfaces.Repositories;
using Payments.Domain.Enums;
using Payments.Domain.Models;
using PaymentsProcessorService.Application.DTOs;
using PaymentsProcessorService.Domain.Mappers;
using PaymentsProcessorService.Infra.Integration.PaymentService;

namespace PaymentsProcessorService.Application.Services;

public class PaymentService
{
    private readonly IPaymentStatusChangedEventRepository _paymentStatusChangedEventRepository;
    private readonly IPaymentCreatedEventRepository _paymentCreatedEventRepository;
    private readonly PaymentServiceIntegration _paymentServiceIntegration;
    private readonly SendEmailStatusFunction _sendEmailStatusFunction;

    public PaymentService(IPaymentStatusChangedEventRepository paymentStatusChangedEventRepository, IPaymentCreatedEventRepository paymentCreatedEventRepository, PaymentServiceIntegration paymentServiceIntegration, SendEmailStatusFunction sendEmailStatusFunction)
    {
        _paymentStatusChangedEventRepository = paymentStatusChangedEventRepository;
        _paymentCreatedEventRepository = paymentCreatedEventRepository;
        _paymentServiceIntegration = paymentServiceIntegration;
        _sendEmailStatusFunction = sendEmailStatusFunction;
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

            paymentCreatedEvent = dto.ToEntity(PaymentStatus.Processing);

            var eventCreated = await _paymentCreatedEventRepository.AddAsync(paymentCreatedEvent);

            await _paymentStatusChangedEventRepository.AddAsync(new PaymentStatusChangedEvent
            {
                PaymentId = eventCreated.Id,
                OldStatus = 0,
                NewStatus = PaymentStatus.Processing,
                ChangedAt = DateTime.UtcNow
            });

            await _sendEmailStatusFunction.SendEmailStatusAsync(dto.UserId, eventCreated.Status.ToString(), "Pagamento em processamento");

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

                await _sendEmailStatusFunction.SendEmailStatusAsync(dto.UserId, PaymentStatus.Processed.ToString(), result.Message);
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

                await _sendEmailStatusFunction.SendEmailStatusAsync(dto.UserId, PaymentStatus.Failed.ToString(), result.Message);
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

            var statusEvents = await _paymentStatusChangedEventRepository
                .GetListByConditionAsync(e => e.PaymentId == guidId);

            var paymentOutputDto = createdEvent.ToOutputDto();

            foreach (var statusEvent in statusEvents.OrderBy(e => e.ChangedAt))
            {
                paymentOutputDto.Status = statusEvent.NewStatus.ToString();
                paymentOutputDto.UpdatedAt = statusEvent.ChangedAt;
                paymentOutputDto.Observation = statusEvent.Observation;
            }

            return ApiResponse.Ok(paymentOutputDto);
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
            var createdEvents = await _paymentCreatedEventRepository.GetListByConditionAsync(e => e.UserId == userId);

            if (createdEvents == null || !createdEvents.Any())
                return ApiResponse.Fail($"Nenhuma transação encontrada para o usuario ID: {userId}");

            var payments = new List<PaymentOutputDto>();

            foreach (var createdEvent in createdEvents)
            {
                var statusEvents = await _paymentStatusChangedEventRepository
                    .GetListByConditionAsync(e => e.PaymentId == createdEvent.Id);

                var paymentOutputDto = createdEvent.ToOutputDto();

                foreach (var statusEvent in statusEvents.OrderBy(e => e.ChangedAt))
                {
                    paymentOutputDto.Status = statusEvent.NewStatus.ToString();
                    paymentOutputDto.UpdatedAt = statusEvent.ChangedAt;
                    paymentOutputDto.Observation = statusEvent.Observation;
                }

                payments.Add(paymentOutputDto);
            }

            return ApiResponse.Ok(payments);
        }
        catch (Exception e)
        {
            return ApiResponse.Fail(e.Message);
        }
    }
}
