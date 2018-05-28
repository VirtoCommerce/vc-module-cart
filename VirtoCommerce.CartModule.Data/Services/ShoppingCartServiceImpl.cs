using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Common.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartServiceImpl : ServiceBase, IShoppingCartService, IShoppingCartSearchService
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IShopingCartTotalsCalculator _totalsCalculator;
        private readonly IEventPublisher _eventPublisher;
        public ShoppingCartServiceImpl(Func<ICartRepository> repositoryFactory, IDynamicPropertyService dynamicPropertyService,
                                      IShopingCartTotalsCalculator totalsCalculator, IEventPublisher eventPublisher)
		{
			_repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _dynamicPropertyService = dynamicPropertyService;
            _totalsCalculator = totalsCalculator;
        }

        #region IShoppingCartService Members

        public virtual ShoppingCart[] GetByIds(string[] cartIds, string responseGroup = null)
        {
            var retVal = new List<ShoppingCart>();

            using (var repository = _repositoryFactory())
            {
                //Disable DBContext change tracking for better performance 
                repository.DisableChangesTracking();

                var cartEntities = repository.GetShoppingCartsByIds(cartIds);
                foreach (var cartEntity in cartEntities)
                {
                    var cart = cartEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance());
                    //Calculate totals only for full responseGroup
                    if (responseGroup == null)
                    {
                        _totalsCalculator.CalculateTotals(cart);
                    }
                    retVal.Add(cart);
                }
            }
          
            _dynamicPropertyService.LoadDynamicPropertyValues(retVal.ToArray<IHasDynamicProperties>());

            return retVal.ToArray();
        }

        public virtual void SaveChanges(ShoppingCart[] carts)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<ShoppingCart>>();

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dataExistCarts = repository.GetShoppingCartsByIds(carts.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var cart in carts)
                {
                    //Calculate cart totals before save
                    _totalsCalculator.CalculateTotals(cart);

                    var originalEntity = dataExistCarts.FirstOrDefault(x => x.Id == cart.Id);
                    var modifiedEntity = AbstractTypeFactory<ShoppingCartEntity>.TryCreateInstance()
                                                                                .FromModel(cart, pkMap);
                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
                        changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, originalEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, EntryState.Added));
                    }
                }

                //Raise domain events
                _eventPublisher.Publish(new CartChangeEvent(changedEntries));
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
                _eventPublisher.Publish(new CartChangedEvent(changedEntries));
            }

            //Save dynamic properties
            foreach (var cart in carts)
            {
                _dynamicPropertyService.SaveDynamicPropertyValues(cart);
            }
        }

        public virtual void Delete(string[] cartIds)
        {
            var carts = GetByIds(cartIds);

            using (var repository = _repositoryFactory())
            {
                //Raise domain events before deletion
                var changedEntries = carts.Select(x => new GenericChangedEntry<ShoppingCart>(x, EntryState.Deleted));
                _eventPublisher.Publish(new CartChangeEvent(changedEntries));

                repository.RemoveCarts(cartIds);

                foreach (var cart in carts)
                {
                    _dynamicPropertyService.DeleteDynamicPropertyValues(cart);
                }
                repository.UnitOfWork.Commit();
                //Raise domain events after deletion
                _eventPublisher.Publish(new CartChangedEvent(changedEntries));
            }
        }

        #endregion

        #region IShoppingCartSearchService Members

        public GenericSearchResult<ShoppingCart> Search(ShoppingCartSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<ShoppingCart>();
            using (var repository = _repositoryFactory())
            {
                var query = repository.ShoppingCarts;

                if (!string.IsNullOrEmpty(criteria.Status))
                {
                    query = query.Where(x => x.Status == criteria.Status);
                }

                if (!string.IsNullOrEmpty(criteria.Name))
                {
                    query = query.Where(x => x.Name == criteria.Name);
                }

                if (!string.IsNullOrEmpty(criteria.CustomerId))
                {
                    query = query.Where(x => x.CustomerId == criteria.CustomerId);
                }

                if (!string.IsNullOrEmpty(criteria.StoreId))
                {
                    query = query.Where(x => criteria.StoreId == x.StoreId);
                }

                if (!string.IsNullOrEmpty(criteria.Currency))
                {
                    query = query.Where(x => x.Currency == criteria.Currency);
                }

                if (!string.IsNullOrEmpty(criteria.Type))
                {
                    query = query.Where(x => x.Type == criteria.Type);
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<ShoppingCartEntity>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
                }

                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();

                var cartIds = query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArray();
                var carts = GetByIds(cartIds);
                retVal.Results = carts.AsQueryable().OrderBySortInfos(sortInfos).ToList();

                return retVal;
            }
        }

        #endregion
    }
}
