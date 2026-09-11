using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model;

public class CartSharingSetting : AuditableEntity, ICloneable
{
    public string ShoppingCartId { get; set; }

    public string Scope { get; set; }

    public string Access { get; set; }

    public string Message { get; set; }

    public IList<CartSharingSettingTarget> Targets { get; set; }

    // The deprecated field as JSON still carries it: read only, so no payload can rewrite the set it summarizes.
    // A client that fills Targets and leaves the deprecated field unset would otherwise send them together and
    // wipe the set; one that sends the field before Targets would duplicate the first row.
    [JsonProperty("sharedWithId")]
    public string SharedWithIdValue => Targets?.FirstOrDefault()?.SharedWithId;

    // Binary compatibility for consumers built against the single-target model (X-Cart <= 3.1035, Sales Rep <= 3.1008):
    // reads the first target, writes replace the whole set with one target. Not persisted itself.
    // [JsonIgnore] confines it to the in-process channel those consumers use - they assign the C# property, which
    // serialization never touches - so the NullCollection rule ("absent means untouched") survives every payload.
    [Obsolete("Use Targets.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
    [JsonIgnore]
    public string SharedWithId
    {
        get => SharedWithIdValue;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Targets = [];
                return;
            }

            // Writing back the value the getter returned changes nothing, so a consumer that re-saves a loaded
            // model keeps the whole set; anything else replaces it, which is the single-target contract this
            // property exists to honour.
            if (SharedWithIdValue.EqualsIgnoreCase(value))
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
