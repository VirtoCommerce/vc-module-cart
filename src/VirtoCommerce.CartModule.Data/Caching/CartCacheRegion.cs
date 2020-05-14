using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.CartModule.Data.Caching
{
    public class CartCacheRegion : CancellableCacheRegion<CartCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _entityRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(ShoppingCart[] carts)
        {
            if (carts == null)
            {
                throw new ArgumentNullException(nameof(carts));
            }
            return CreateChangeToken(carts.Select(x => x.Id).ToArray());
        }

        public static IChangeToken CreateChangeToken(string[] cartIds)
        {
            if (cartIds == null)
            {
                throw new ArgumentNullException(nameof(cartIds));
            }
            var changeTokens = new List<IChangeToken>() { CreateChangeToken() };
            foreach (var cartId in cartIds)
            {
                changeTokens.Add(new CancellationChangeToken(_entityRegionTokenLookup.GetOrAdd(cartId, new CancellationTokenSource()).Token));
            }
            return new CompositeChangeToken(changeTokens);
        }

        public static void ExpireInventory(ShoppingCart cart)
        {
            if (_entityRegionTokenLookup.TryRemove(cart.Id, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
