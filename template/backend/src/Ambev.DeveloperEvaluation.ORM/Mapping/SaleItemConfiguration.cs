using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.Product)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Quantity)
            .IsRequired()
            .HasColumnType("int");

        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Discount)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(i => i.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.DiscountPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0);

        builder.HasOne(i => i.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}