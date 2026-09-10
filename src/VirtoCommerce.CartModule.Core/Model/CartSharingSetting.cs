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

    // Binary compatibility for consumers built against the single-target model (X-Cart <= 3.1035, Sales Rep <= 3.1008):
    // reads the first target, writes replace the whole set with one target. Not persisted itself.
    [Obsolete("Use Targets.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
    public string SharedWithId
    {
        get => Targets?.FirstOrDefault()?.SharedWithId;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Targets = [];
                return;
            }

            if (Targets?.Count == 1 && Targets[0].SharedWithId.EqualsIgnoreCase(value))
            {
                return;
            }

            var target = AbstractTypeFactory<CartSharingSettingTarget>.TryCreateInstance();

            target.CartSharingSettingId = Id;
            target.SharedWithId = value;

            Targets = [target];
        }
    }

    public object Clone()
    {
        var result = (CartSharingSetting)MemberwiseClone();

        result.Targets = Targets?.Select(x => x.CloneTyped()).ToList();

        return result;
    }
}
