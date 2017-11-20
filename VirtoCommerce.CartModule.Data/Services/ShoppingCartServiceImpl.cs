using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CartModule.Data.Extensions;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartServiceImpl : ServiceBase, IShoppingCartService, IShoppingCartSearchService
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly IEventPublisher<CartChangeEvent> _changingEventPublisher;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IEventPublisher<CartChangedEvent> _changedEventPublisher;

        public ShoppingCartServiceImpl(Func<ICartRepository> repositoryFactory, IEventPublisher<CartChangeEvent> changingEventPublisher, IDynamicPropertyService dynamicPropertyService, IEventPublisher<CartChangedEvent> changedEventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _changingEventPublisher = changingEventPublisher;
            _dynamicPropertyService = dynamicPropertyService;
            _changedEventPublisher = changedEventPublisher;
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
                    retVal.Add(cart);
                }
            }

            _dynamicPropertyService.LoadDynamicPropertyValues(retVal.ToArray<IHasDynamicProperties>());

            return retVal.ToArray();
        }

        public virtual void SaveChanges(ShoppingCart[] carts)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEvents = new List<CartChangedEvent>();

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dataExistCarts = repository.GetShoppingCartsByIds(carts.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var cart in carts)
                {
                    var originalEntity = dataExistCarts.FirstOrDefault(x => x.Id == cart.Id);
                    var originalCart = originalEntity != null ? originalEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()) : cart;

                    var changingEvent = new CartChangeEvent(originalEntity == null ? EntryState.Added : EntryState.Modified, originalCart, cart);
                    _changingEventPublisher.Publish(changingEvent);
                    changedEvents.Add(new CartChangedEvent(changingEvent.ChangeState, changingEvent.OrigCart, changingEvent.ModifiedCart));

                    var modifiedEntity = AbstractTypeFactory<ShoppingCartEntity>.TryCreateInstance().FromModel(cart, pkMap);
                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }

                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }

            //Save dynamic properties
            foreach (var cart in carts)
            {
                _dynamicPropertyService.SaveDynamicPropertyValues(cart);
            }

            //Raise changed events
            foreach (var changedEvent in changedEvents)
            {
                _changedEventPublisher.Publish(changedEvent);
            }
        }

        public virtual void Delete(string[] ids)
        {
            var carts = GetByIds(ids);

            using (var repository = _repositoryFactory())
            {
                foreach (var cart in carts)
                {
                    _changingEventPublisher.Publish(new CartChangeEvent(EntryState.Deleted, cart, cart));
                }

                repository.RemoveCarts(ids);

                foreach (var cart in carts)
                {
                    _dynamicPropertyService.DeleteDynamicPropertyValues(cart);
                }

                repository.UnitOfWork.Commit();

                foreach (var cart in carts)
                {
                    _changedEventPublisher.Publish(new CartChangedEvent(EntryState.Deleted, cart, cart));
                }
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
