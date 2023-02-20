using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtoCommerce.CartModule.Data.Model;

namespace VirtoCommerce.CoreModule.Data.MySql
{
    public class PaymentEntityConfiguration : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            builder.Property(x => x.Amount).HasColumnType("decimal").HasPrecision(18, 4);
            builder.Property(x => x.Price).HasColumnType("decimal").HasPrecision(18, 4);
            builder.Property(x => x.PriceWithTax).HasColumnType("decimal").HasPrecision(18, 4);
            builder.Property(x => x.DiscountAmount).HasColumnType("decimal").HasPrecision(18, 4);
            builder.Property(x => x.DiscountAmountWithTax).HasColumnType("decimal").HasPrecision(18, 4);
            builder.Property(x => x.Total).HasColumnType("decimal").HasPrecision(18, 4);
            builder.Property(x => x.TotalWithTax).HasColumnType("decimal").HasPrecision(18, 4);
            builder.Property(x => x.TaxTotal).HasColumnType("decimal").HasPrecision(18, 4);
        }
    }
}
