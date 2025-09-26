using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGameWebAPI.Domain.Interfaces.Repositories;
using FiapCloudGameWebAPI.Infrastructure.Repositories;
using Payments.Domain.Models;

namespace FiapCloudGames.Infrastructure.Repositories;

public class PaymentStatusChangedEventRepository : BaseRepository<PaymentStatusChangedEvent>, IPaymentStatusChangedEventRepository
{
    public PaymentStatusChangedEventRepository(ApplicationDbContext context) : base(context)
    {
    }
}