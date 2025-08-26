using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model;

public class CartSharingSetting : AuditableEntity, ICloneable
{
    public string ShoppingCartId { get; set; }

    public string Scope { get; set; }

    public string Access { get; set; }

    public bool IsActive { get; set; }

    public object Clone() => MemberwiseClone();
}
