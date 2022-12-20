using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class WishlistService : IWishlistService
    {
        private const string WishlistCartType = "Wishlist";

        private readonly Func<ICartRepository> _repositoryFunc;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public WishlistService(Func<ICartRepository> repositoryFunc, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFunc = repositoryFunc;
            _platformMemoryCache = platformMemoryCache;
        }

        public virtual Task<List<string>> FindProductsInWishlistsAsync(string customerId, string storeId, IList<string> productIds)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindProductsInWishlistsAsync), BuildCacheKey(customerId, storeId, productIds));
            return _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(GenericSearchCachingRegion<ShoppingCart>.CreateChangeToken());

                using var repository = _repositoryFunc();
                var productIdsInWishLists = await repository
                    .ShoppingCarts
                    .Where(cart => cart.CustomerId == customerId &&
                                   cart.StoreId == storeId &&
                                   cart.Type == WishlistCartType)
                    .SelectMany(cart => cart.Items)
                    .Where(lineItem => productIds.Contains(lineItem.ProductId))
                    .Select(lineItem => lineItem.ProductId)
                    .Distinct()
                    .ToListAsync();

                return productIdsInWishLists;
            });
        }

        public virtual string BuildCacheKey(string customerId, string storeId, IList<string> productIds)
        {
            var keysValues = new List<string>() { customerId, storeId };
            keysValues.AddRange(productIds);

            return string.Join("|", keysValues);
        }
    }
}
