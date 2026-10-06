using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model;

public class CartSharingSettingTarget : AuditableEntity, ICloneable
{
    public string CartSharingSettingId { get; set; }

    public string SharedWithId { get; set; }

    public object Clone() => MemberwiseClone();
}
