using Payments.Domain.Enums;

namespace PaymentsProcessorService.Application.DTOs
{
    public class PaymentOutputDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public string? Currency { get; set; }
        public string? Observation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
