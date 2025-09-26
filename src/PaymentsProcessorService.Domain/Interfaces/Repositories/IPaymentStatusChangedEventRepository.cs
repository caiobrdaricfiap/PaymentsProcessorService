using FiapCloudGames.Domain.Entities;
using Payments.Domain.Models;

namespace FiapCloudGameWebAPI.Domain.Interfaces.Repositories
{
    public interface IPaymentStatusChangedEventRepository : IBaseRepository<PaymentStatusChangedEvent>
    {
    }
}
