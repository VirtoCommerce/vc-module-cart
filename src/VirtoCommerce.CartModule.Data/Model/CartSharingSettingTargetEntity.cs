using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using static VirtoCommerce.Platform.Data.Infrastructure.DbContextBase;

namespace VirtoCommerce.CartModule.Data.Model;

public class CartSharingSettingTargetEntity : AuditableEntity, IDataEntity<CartSharingSettingTargetEntity, CartSharingSettingTarget>
{
    [Required]
    [StringLength(IdLength)]
    public string CartSharingSettingId { get; set; }

    [Required]
    [StringLength(IdLength)]
    public string SharedWithId { get; set; }

    public virtual CartSharingSettingEntity CartSharingSetting { get; set; }

    public virtual CartSharingSettingTarget ToModel(CartSharingSettingTarget model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;
        model.CreatedDate = CreatedDate;
        model.ModifiedDate = ModifiedDate;
        model.CreatedBy = CreatedBy;
        model.ModifiedBy = ModifiedBy;

        model.CartSharingSettingId = CartSharingSettingId;
        model.SharedWithId = SharedWithId;

        return model;
    }

    public virtual CartSharingSettingTargetEntity FromModel(CartSharingSettingTarget model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedDate = model.CreatedDate;
        ModifiedDate = model.ModifiedDate;
        CreatedBy = model.CreatedBy;
        ModifiedBy = model.ModifiedBy;

        CartSharingSettingId = model.CartSharingSettingId;
        SharedWithId = model.SharedWithId;

        return this;
    }

    public virtual void Patch(CartSharingSettingTargetEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.SharedWithId = SharedWithId;
    }
}
