using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Domain.Models;

namespace FiapCloudGames.Infrastructure.Configuration
{
    public class PaymentStatusChangedEventConfiguration : IEntityTypeConfiguration<PaymentStatusChangedEvent>
    {
        public void Configure(EntityTypeBuilder<PaymentStatusChangedEvent> builder)
        {
            builder.ToTable("PaymentStatusChangedEvents");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnType("UNIQUEIDENTIFIER")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.PaymentId)
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired();

            builder.Property(e => e.OldStatus)
                .HasConversion<string>()
                .HasColumnType("NVARCHAR(50)")
                .IsRequired();

            builder.Property(e => e.NewStatus)
                .HasConversion<string>()
                .HasColumnType("NVARCHAR(50)")
                .IsRequired();

            builder.Property(e => e.Observation)
                .HasColumnType("NVARCHAR(255)")
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(e => e.ChangedAt)
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.HasOne<PaymentCreatedEvent>()
                .WithMany()
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}