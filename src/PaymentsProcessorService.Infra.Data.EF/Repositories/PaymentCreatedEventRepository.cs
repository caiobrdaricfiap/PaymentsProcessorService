using FiapCloudGames.Infrastructure.Data;
using FiapCloudGameWebAPI.Domain.Interfaces.Repositories;
using FiapCloudGameWebAPI.Infrastructure.Repositories;
using Payments.Domain.Models;

namespace FiapCloudGames.Infrastructure.Repositories;

public class PaymentCreatedEventRepository : BaseRepository<PaymentCreatedEvent>, IPaymentCreatedEventRepository
{
    public PaymentCreatedEventRepository(ApplicationDbContext context) : base(context)
    {
    }
}