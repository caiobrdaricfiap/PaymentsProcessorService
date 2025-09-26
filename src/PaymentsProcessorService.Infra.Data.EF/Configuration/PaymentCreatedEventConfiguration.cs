using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Domain.Models;

public class PaymentCreatedEventConfiguration : IEntityTypeConfiguration<PaymentCreatedEvent>
{
    public void Configure(EntityTypeBuilder<PaymentCreatedEvent> builder)
    {
        builder.ToTable("PaymentCreatedEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnType("UNIQUEIDENTIFIER")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UserId).HasColumnType("INT").IsRequired();
        builder.Property(e => e.GameId).HasColumnType("INT").IsRequired();
        builder.Property(e => e.Amount).HasColumnType("DECIMAL(18,2)").IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasColumnType("NVARCHAR(50)").IsRequired();
        builder.Property(e => e.Currency).HasConversion<string>().HasColumnType("NVARCHAR(50)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnType("DATETIME2").IsRequired();
    }
}