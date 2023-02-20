using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtoCommerce.CartModule.Data.Model;

namespace VirtoCommerce.CoreModule.Data.MySql
{
    public class TaxDetailEntityConfiguration : IEntityTypeConfiguration<TaxDetailEntity>
    {
        public void Configure(EntityTypeBuilder<TaxDetailEntity> builder)
        {
            builder.Property(x => x.Amount).HasColumnType("decimal").HasPrecision(18, 4);
        }
    }
}
