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

//public class LineItemEntityConfiguration : IEntityTypeConfiguration<LineItemEntity>
//{
//    public void Configure(EntityTypeBuilder<LineItemEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class ShipmentEntityConfiguration : IEntityTypeConfiguration<ShipmentEntity>
//{
//    public void Configure(EntityTypeBuilder<ShipmentEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class ShipmentItemEntityConfiguration : IEntityTypeConfiguration<ShipmentItemEntity>
//{
//    public void Configure(EntityTypeBuilder<ShipmentItemEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class AddressEntityConfiguration : IEntityTypeConfiguration<AddressEntity>
//{
//    public void Configure(EntityTypeBuilder<AddressEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class PaymentEntityConfiguration : IEntityTypeConfiguration<PaymentEntity>
//{
//    public void Configure(EntityTypeBuilder<PaymentEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class TaxDetailEntityConfiguration : IEntityTypeConfiguration<TaxDetailEntity>
//{
//    public void Configure(EntityTypeBuilder<TaxDetailEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class DiscountEntityConfiguration : IEntityTypeConfiguration<DiscountEntity>
//{
//    public void Configure(EntityTypeBuilder<DiscountEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class CouponEntityConfiguration : IEntityTypeConfiguration<CouponEntity>
//{
//    public void Configure(EntityTypeBuilder<CouponEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class CartDynamicPropertyObjectValueEntityConfiguration : IEntityTypeConfiguration<CartDynamicPropertyObjectValueEntity>
//{
//    public void Configure(EntityTypeBuilder<CartDynamicPropertyObjectValueEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class ConfigurationItemEntityConfiguration : IEntityTypeConfiguration<ConfigurationItemEntity>
//{
//    public void Configure(EntityTypeBuilder<ConfigurationItemEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}

//public class ConfigurationItemFileEntityConfiguration : IEntityTypeConfiguration<ConfigurationItemFileEntity>
//{
//    public void Configure(EntityTypeBuilder<ConfigurationItemFileEntity> builder)
//    {
//        builder.ToTable(tb => tb.UseSqlOutputClause(false));
//    }
//}
