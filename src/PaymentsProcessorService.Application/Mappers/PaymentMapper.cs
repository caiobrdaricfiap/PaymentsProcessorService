using Payments.Domain.Enums;
using Payments.Domain.Models;
using PaymentsProcessorService.Application.DTOs;

namespace PaymentsProcessorService.Domain.Mappers
{
    public static class PaymentMapper
    {
        public static PaymentCreatedEvent ToEntity(this PaymentProcessInputDto inputDto, PaymentStatus status)
        {
            return new()
            {
                UserId = inputDto.UserId,
                GameId = inputDto.GameId,
                Amount = inputDto.Amount,
                Status = status,
                Currency = inputDto.Currency,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static PaymentOutputDto ToOutputDto(this PaymentCreatedEvent entity)
        {
            return new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                GameId = entity.GameId,
                Amount = entity.Amount,
                Currency = entity.Currency.ToString(),
                Observation = entity.Observation,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
