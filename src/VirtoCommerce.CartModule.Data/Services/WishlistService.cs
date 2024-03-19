using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public WishlistService(Func<ICartRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }

        public virtual async Task<IList<string>> FindProductsInWishlistsAsync(string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            var cacheKeyPrefix = CacheKey.With(GetType(), nameof(FindProductsInWishlistsAsync), customerId, storeId);

            var models = await _platformMemoryCache.GetOrLoadByIdsAsync(cacheKeyPrefix, productIds,
                missingIds => FindProductsInWishlistsNoCacheAsync(customerId, organizationId, storeId, missingIds),
                ConfigureCache);

            return models.Where(x => x.InWishlist).Select(x => x.Id).ToList();
        }

        public virtual async Task<IDictionary<string, IList<string>>> FindWishlistsByProductsAsync(string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            var cacheKeyPrefix = CacheKey.With(GetType(), nameof(FindWishlistsByProductsAsync), customerId, storeId);

            var models = await _platformMemoryCache.GetOrLoadByIdsAsync(cacheKeyPrefix, productIds,
                 missingIds => FindWishlistsByProductsNoCacheAsync(customerId, organizationId, storeId, missingIds),
                 ConfigureCache);

            return models.ToDictionary(x => x.Id, x => x.WishlistIds);
        }

        protected virtual async Task<IList<InternalEntity>> FindProductsInWishlistsNoCacheAsync(string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            using var repository = _repositoryFactory();

            var query = GetCustomerWishListsQuery(repository, customerId, organizationId, storeId);

            var result = await query
                .SelectMany(cart => cart.Items)
                .Where(lineItem => productIds.Contains(lineItem.ProductId))
                .Select(lineItem => lineItem.ProductId)
                .Distinct()
                .Select(x => new InternalEntity { Id = x, CustomerId = customerId, InWishlist = true })
                .ToListAsync();

            result.AddRange(productIds.Except(result.Select(x => x.Id))
                .Select(x => new InternalEntity { Id = x, CustomerId = customerId }));

            return result;
        }

        protected virtual async Task<IList<InternalEntity>> FindWishlistsByProductsNoCacheAsync(string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            using var repository = _repositoryFactory();

            var query = GetCustomerWishListsQuery(repository, customerId, organizationId, storeId);

            var result = await query
                .SelectMany(cart => cart.Items)
                .Where(lineItem => productIds.Contains(lineItem.ProductId))
                .GroupBy(lineItem => lineItem.ProductId)
                .Select(lineItemGroup => new InternalEntity
                {
                    Id = lineItemGroup.Key,
                    WishlistIds = lineItemGroup.Select(x => x.ShoppingCartId).ToList(),
                    CustomerId = customerId,
                })
                .ToListAsync();

            return result;
        }

        protected virtual IQueryable<ShoppingCartEntity> GetCustomerWishListsQuery(ICartRepository repository, string customerId, string organizationId, string storeId)
        {
            var query = repository.ShoppingCarts.Where(cart =>
                               cart.StoreId == storeId &&
                               cart.Type == ModuleConstants.WishlistCartType &&
                               !cart.IsDeleted);

            var predicate = PredicateBuilder.False<ShoppingCartEntity>();

            if (!string.IsNullOrEmpty(customerId))
            {
                predicate = predicate.Or(x => x.CustomerId == customerId);
            }

            if (!string.IsNullOrEmpty(organizationId))
            {
                predicate = predicate.Or(x => x.OrganizationId == organizationId);
            }

            query = query.Where(predicate);

            return query;
        }

        protected virtual void ConfigureCache(MemoryCacheEntryOptions cacheOptions, string id, InternalEntity model)
        {
            cacheOptions.AddExpirationToken(GenericSearchCachingRegion<ShoppingCart>.CreateChangeTokenForKey(model.CustomerId));
        }

        protected class InternalEntity : Entity
        {
            public string CustomerId { get; set; }
            public bool InWishlist { get; set; }
            public IList<string> WishlistIds { get; set; }
        }
    }
}
