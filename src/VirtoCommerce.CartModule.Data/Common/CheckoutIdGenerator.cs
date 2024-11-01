using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VirtoCommerce.CartModule.Data.Common;

public class CheckoutIdGenerator : Microsoft.EntityFrameworkCore.ValueGeneration.ValueGenerator
{
    public override bool GeneratesTemporaryValues => false;

    protected override object NextValue(EntityEntry entry)
    {
        return entry.Property("Id").CurrentValue;
    }
}
