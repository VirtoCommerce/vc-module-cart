using System;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model
{
    public class AbandonedCartContext : IEvaluationContext
    {
        public string ShoppingCartId { get; set; }
        public DateTime? ShoppingCartModifiedDate { get; set; }
    }
}
