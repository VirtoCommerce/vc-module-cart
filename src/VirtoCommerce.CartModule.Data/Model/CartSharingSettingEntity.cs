using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CartModule.Data.Model;

public class CartSharingSettingEntity : AuditableEntity, IDataEntity<CartSharingSettingEntity, CartSharingSetting>
{
    public string ShoppingCartId { get; set; }

    [Required]
    public string Mode { get; set; }

    [Required]
    public string Access { get; set; }

    public bool IsActive { get; set; }

    public virtual ShoppingCartEntity ShoppingCart { get; set; }

    public CartSharingSetting ToModel(CartSharingSetting model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;
        model.CreatedDate = CreatedDate;
        model.ModifiedDate = ModifiedDate;
        model.CreatedBy = CreatedBy;
        model.ModifiedBy = ModifiedBy;

        model.ShoppingCartId = ShoppingCartId;
        model.Mode = Mode;
        model.Access = Access;
        model.IsActive = IsActive;

        return model;
    }

    public CartSharingSettingEntity FromModel(CartSharingSetting model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedDate = model.CreatedDate;
        ModifiedDate = model.ModifiedDate;
        CreatedBy = model.CreatedBy;
        ModifiedBy = model.ModifiedBy;

        ShoppingCartId = model.ShoppingCartId;
        Mode = model.Mode;
        Access = model.Access;
        IsActive = model.IsActive;

        return this;
    }

    public void Patch(CartSharingSettingEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.ShoppingCartId = ShoppingCartId;
        target.Mode = Mode;
        target.Access = Access;
        target.IsActive = IsActive;
    }
}
