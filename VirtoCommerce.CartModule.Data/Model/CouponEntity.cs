using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class CouponEntity : Entity
    {
        [StringLength(64)]
        public string Code { get; set; }


        // navigation properties
        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }


        public virtual string ToModel()
        {
            return Code;
        }

        public virtual CouponEntity FromModel(string model)
        {
            Code = model;

            return this;
        }

        public virtual void Patch(CouponEntity target)
        {
            target.Code = Code;
        }
    }
}
