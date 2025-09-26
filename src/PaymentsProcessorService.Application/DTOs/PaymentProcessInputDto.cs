using Payments.Domain.Enums;

namespace PaymentsProcessorService.Application.DTOs
{
    public class PaymentProcessInputDto
    {
        public int UserId { get; set; }
        public int GameId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
