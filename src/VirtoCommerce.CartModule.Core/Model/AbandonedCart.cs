using System;

namespace VirtoCommerce.CartModule.Core.Model
{
    public class AbandonedCart
    {
        public bool IsAbandoned { get; set; } = true;
        public AbandonedCartStatus Status { get; set; }
        public DateTime AbandonedDate { get; set; }
    }
}
