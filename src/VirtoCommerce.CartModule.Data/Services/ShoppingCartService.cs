using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Caching;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly IShoppingCartTotalsCalculator _totalsCalculator;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public ShoppingCartService(Func<ICartRepository> repositoryFactory,
                                      IShoppingCartTotalsCalculator totalsCalculator, IEventPublisher eventPublisher,
                                      IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _totalsCalculator = totalsCalculator;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        #region IShoppingCartService Members

        public virtual async Task<ShoppingCart[]> GetByIdsAsync(string[] cartIds, string responseGroup = null)
        {
            var retVal = new List<ShoppingCart>();
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", cartIds), responseGroup);
            var result = await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                using (var repository = _repositoryFactory())
                {
                    //Disable DBContext change tracking for better performance 
                    repository.DisableChangesTracking();
                    cacheEntry.AddExpirationToken(CartCacheRegion.CreateChangeToken(cartIds));

                    var cartEntities = await repository.GetShoppingCartsByIdsAsync(cartIds, responseGroup);
                    foreach (var cartEntity in cartEntities)
                    {
                        var cart = cartEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance());
                        //Calculate totals only for full responseGroup
                        if (responseGroup == null)
                        {
                            _totalsCalculator.CalculateTotals(cart);
                        }
                        cart.ReduceDetails(responseGroup);

                        retVal.Add(cart);
                        
                    }
                }
                return retVal;
            });

            return result.Select(x => x.Clone() as ShoppingCart).ToArray();
        }

        public virtual async Task<ShoppingCart> GetByIdAsync(string id, string responseGroup = null)
        {
            var carts = await GetByIdsAsync(new[] { id }, responseGroup);
            return carts.FirstOrDefault();
        }

        public virtual async Task SaveChangesAsync(ShoppingCart[] carts)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<ShoppingCart>>();

            using (var repository = _repositoryFactory())
            {
                var dataExistCarts = await repository.GetShoppingCartsByIdsAsync(carts.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var cart in carts)
                {
                    //Calculate cart totals before save
                    _totalsCalculator.CalculateTotals(cart);

                    var originalEntity = dataExistCarts.FirstOrDefault(x => x.Id == cart.Id);
                    var modifiedEntity = AbstractTypeFactory<ShoppingCartEntity>.TryCreateInstance()
                                                                                .FromModel(cart, pkMap);

                    if (originalEntity != null)
                    {
                        // This extension is allow to get around breaking changes is introduced in EF Core 3.0 that leads to throw
                        // Database operation expected to affect 1 row(s) but actually affected 0 row(s) exception when trying to add the new children entities with manually set keys
                        // https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.0/breaking-changes#detectchanges-honors-store-generated-key-values
                        repository.TrackModifiedAsAddedForNewChildEntities(originalEntity);

                        changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, originalEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                        if (repository is RedisCartRepository)
                        {
                            repository.Attach(originalEntity);
                        }
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, EntryState.Added));
                    }
                }

                //Raise domain events
                await _eventPublisher.Publish(new CartChangeEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();

                ClearCache(carts);

                await _eventPublisher.Publish(new CartChangedEvent(changedEntries));
            }
        }

        public virtual async Task DeleteAsync(string[] cartIds, bool softDelete = false)
        {
            var carts = (await GetByIdsAsync(cartIds)).ToArray();

            using (var repository = _repositoryFactory())
            {
                //Raise domain events before deletion
                var entityState = softDelete ? EntryState.Modified : EntryState.Deleted;
                var changedEntries = carts.Select(x => new GenericChangedEntry<ShoppingCart>(x, entityState)).ToArray();
                await _eventPublisher.Publish(new CartChangeEvent(changedEntries));

                if (softDelete)
                {
                    await repository.SoftRemoveCartsAsync(cartIds);                   
                }
                else
                {                    
                    await repository.RemoveCartsAsync(cartIds);                    
                }

                await repository.UnitOfWork.CommitAsync();

                ClearCache(carts);

                //Raise domain events after deletion
                await _eventPublisher.Publish(new CartChangedEvent(changedEntries));
            }
        }

        protected virtual void ClearCache(IEnumerable<ShoppingCart> entities)
        {
            CartSearchCacheRegion.ExpireRegion();

            foreach (var entity in entities)
            {
                CartCacheRegion.ExpireCart(entity);
            }
        }

        #endregion
    }
}
