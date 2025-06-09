using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtoCommerce.CartModule.Data.Model;

namespace VirtoCommerce.CartModule.Data.SqlServer;

public class ShoppingCartEntityConfiguration : IEntityTypeConfiguration<ShoppingCartEntity>
{
    public void Configure(EntityTypeBuilder<ShoppingCartEntity> builder)
    {
        builder.ToTable(tb => tb.UseSqlOutputClause(false));
    }
}

public class LineItemEntityConfiguration : IEntityTypeConfiguration<LineItemEntity>
{
    public void Configure(EntityTypeBuilder<LineItemEntity> builder)
    {
        builder.ToTable(tb => tb.UseSqlOutputClause(false));
    }
}
