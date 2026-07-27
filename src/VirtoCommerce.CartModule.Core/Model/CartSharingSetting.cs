using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model;

public class CartSharingSetting : AuditableEntity, ICloneable
{
    public string ShoppingCartId { get; set; }

    public string Scope { get; set; }

    public string Access { get; set; }

    // Optional principal this share targets, disambiguated by Scope (a scope defines the id space, e.g. a
    // customer-organization scope stores an organization id here). Null for scopes that don't target a
    // specific principal (Private/Organization/Anyone*).
    public string SharedWithId { get; set; }

    public object Clone() => MemberwiseClone();
}
