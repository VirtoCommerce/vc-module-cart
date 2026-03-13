using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtoCommerce.CartModule.Data.Model;

namespace VirtoCommerce.CartModule.Data.MySql
{
    public class ConfigurationItemEntityConfiguration : IEntityTypeConfiguration<ConfigurationItemEntity>
    {
        public void Configure(EntityTypeBuilder<ConfigurationItemEntity> builder)
        {
            builder.Property(x => x.ListPrice).HasColumnType("decimal").HasPrecision(18, 4);
            builder.Property(x => x.SalePrice).HasColumnType("decimal").HasPrecision(18, 4);
        }
    }
}
