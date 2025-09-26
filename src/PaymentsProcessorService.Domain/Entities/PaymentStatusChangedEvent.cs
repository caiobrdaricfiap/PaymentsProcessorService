using Payments.Domain.Enums;
using PaymentsProcessorService.Domain.Models;

namespace FiapCloudGames.Domain.Entities
{
    public class PaymentStatusChangedEvent : BaseEntity
    {
        public Guid PaymentId { get; set; }
        public PaymentStatus OldStatus { get; set; }
        public PaymentStatus NewStatus { get; set; }
        public string? Observation { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}