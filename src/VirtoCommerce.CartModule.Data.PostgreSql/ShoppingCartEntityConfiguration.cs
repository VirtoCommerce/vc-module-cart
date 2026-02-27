using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;

namespace VirtoCommerce.CartModule.Data.PostgreSql;

public class ShoppingCartEntityConfiguration : IEntityTypeConfiguration<ShoppingCartEntity>
{
    public void Configure(EntityTypeBuilder<ShoppingCartEntity> builder)
    {
        builder.Property(x => x.Name).UseCaseInsensitiveCollation();
        builder.Property(x => x.OrganizationName).UseCaseInsensitiveCollation();
    }
}
