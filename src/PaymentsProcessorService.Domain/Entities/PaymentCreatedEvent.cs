using FiapCloudGames.Domain.Entities;
using Payments.Domain.Enums;
using PaymentsProcessorService.Domain.Models;

namespace Payments.Domain.Models
{
    public class PaymentCreatedEvent : BaseEntity
    {
        public int UserId { get; set; }
        public int GameId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public Currency Currency { get; set; }
        public string? Observation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
