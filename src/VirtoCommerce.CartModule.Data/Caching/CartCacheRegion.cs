using System;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.CartModule.Data.Caching
{
    public class CartCacheRegion : CancellableCacheRegion<CartCacheRegion>
    {
        public static IChangeToken CreateChangeToken(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            return CreateChangeTokenForKey(cart.Id);
        }

        public static void ExpireCart(ShoppingCart cart)
        {
            if (cart != null)
            {
                ExpireTokenForKey(cart.Id);
            }
        }
    }
}
