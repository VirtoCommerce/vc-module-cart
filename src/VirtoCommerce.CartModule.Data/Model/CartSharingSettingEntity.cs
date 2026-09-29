using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using static VirtoCommerce.Platform.Data.Infrastructure.DbContextBase;

namespace VirtoCommerce.CartModule.Data.Model;

public class CartSharingSettingEntity : AuditableEntity, IDataEntity<CartSharingSettingEntity, CartSharingSetting>
{
    [StringLength(IdLength)]
    public string ShoppingCartId { get; set; }

    [Required]
    [StringLength(Length32)]
    public string Scope { get; set; }

    [Required]
    [StringLength(Length32)]
    public string Access { get; set; }

    [StringLength(ModuleConstants.Sharing.MessageMaxLength)]
    public string Message { get; set; }

    public virtual ShoppingCartEntity ShoppingCart { get; set; }

    public virtual ObservableCollection<CartSharingSettingTargetEntity> Targets { get; set; } = new NullCollection<CartSharingSettingTargetEntity>();

    public virtual CartSharingSetting ToModel(CartSharingSetting model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;
        model.CreatedDate = CreatedDate;
        model.ModifiedDate = ModifiedDate;
        model.CreatedBy = CreatedBy;
        model.ModifiedBy = ModifiedBy;

        model.ShoppingCartId = ShoppingCartId;
        model.Scope = Scope;
        model.Access = Access;
        model.Message = Message;

        model.Targets = Targets.Select(x => x.ToModel(AbstractTypeFactory<CartSharingSettingTarget>.TryCreateInstance())).ToList();

        return model;
    }

    public virtual CartSharingSettingEntity FromModel(CartSharingSetting model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedDate = model.CreatedDate;
        ModifiedDate = model.ModifiedDate;
        CreatedBy = model.CreatedBy;
        ModifiedBy = model.ModifiedBy;

        ShoppingCartId = model.ShoppingCartId;
        Scope = model.Scope;
        Access = model.Access;
        Message = model.Message;

        if (model.Targets != null)
        {
            Targets = new ObservableCollection<CartSharingSettingTargetEntity>(model.Targets.Select(x => AbstractTypeFactory<CartSharingSettingTargetEntity>.TryCreateInstance().FromModel(x, pkMap)));
        }

        return this;
    }

    public virtual void Patch(CartSharingSettingEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.ShoppingCartId = ShoppingCartId;
        target.Scope = Scope;
        target.Access = Access;
        target.Message = Message;

        if (!Targets.IsNullCollection())
        {
            Targets.Patch(target.Targets, (sourceTarget, targetTarget) => sourceTarget.Patch(targetTarget));
        }
    }
}
