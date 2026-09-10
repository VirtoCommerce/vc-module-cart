using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model;

public class CartSharingSetting : AuditableEntity, ICloneable
{
    public string ShoppingCartId { get; set; }

    public string Scope { get; set; }

    public string Access { get; set; }

    public string Message { get; set; }

    public IList<CartSharingSettingTarget> Targets { get; set; }

    public object Clone()
    {
        var result = (CartSharingSetting)MemberwiseClone();

        result.Targets = Targets?.Select(x => x.CloneTyped()).ToList();

        return result;
    }
}
