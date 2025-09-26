using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Models;

namespace FiapCloudGames.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PaymentStatusChangedEvent> PaymentStatusChangedEvent { get; set; }
        public DbSet<PaymentCreatedEvent> PaymentCreatedEvent { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }

}