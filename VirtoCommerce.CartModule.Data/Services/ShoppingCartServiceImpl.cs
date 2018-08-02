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
        public ShoppingCartServiceImpl(Func<ICartRepository> repositoryFactory, IDynamicPropertyService dynamicPropertyService,
                                      IShopingCartTotalsCalculator totalsCalculator, IEventPublisher eventPublisher)
        {
            RepositoryFactory = repositoryFactory;
            EventPublisher = eventPublisher;
            DynamicPropertyService = dynamicPropertyService;
            TotalsCalculator = totalsCalculator;
        }

        protected Func<ICartRepository> RepositoryFactory { get; }
        protected IDynamicPropertyService DynamicPropertyService { get; }
        protected IShopingCartTotalsCalculator TotalsCalculator { get; }
        protected IEventPublisher EventPublisher { get; }

        #region IShoppingCartService Members

        public virtual ShoppingCart[] GetByIds(string[] cartIds, string responseGroup = null)
        {
            var retVal = new List<ShoppingCart>();

            using (var repository = RepositoryFactory())
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
                        TotalsCalculator.CalculateTotals(cart);
                    }
                    retVal.Add(cart);
                }
            }

            DynamicPropertyService.LoadDynamicPropertyValues(retVal.ToArray<IHasDynamicProperties>());

            return retVal.ToArray();
        }

        public virtual void SaveChanges(ShoppingCart[] carts)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<ShoppingCart>>();

            using (var repository = RepositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dataExistCarts = repository.GetShoppingCartsByIds(carts.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var cart in carts)
                {
                    //Calculate cart totals before save
                    TotalsCalculator.CalculateTotals(cart);

                    var originalEntity = dataExistCarts.FirstOrDefault(x => x.Id == cart.Id);
                    var modifiedEntity = AbstractTypeFactory<ShoppingCartEntity>.TryCreateInstance()
                                                                                .FromModel(cart, pkMap);
                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
                        var oldEntry = originalEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance());
                        DynamicPropertyService.LoadDynamicPropertyValues(oldEntry);
                        changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, oldEntry, EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, EntryState.Added));
                    }
                }

                //Raise domain events
                EventPublisher.Publish(new CartChangeEvent(changedEntries));
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
                EventPublisher.Publish(new CartChangedEvent(changedEntries));
            }

            //Save dynamic properties
            foreach (var cart in carts)
            {
                DynamicPropertyService.SaveDynamicPropertyValues(cart);
            }
        }

        public virtual void Delete(string[] cartIds)
        {
            var carts = GetByIds(cartIds);

            using (var repository = RepositoryFactory())
            {
                //Raise domain events before deletion
                var changedEntries = carts.Select(x => new GenericChangedEntry<ShoppingCart>(x, EntryState.Deleted));
                EventPublisher.Publish(new CartChangeEvent(changedEntries));

                repository.RemoveCarts(cartIds);

                foreach (var cart in carts)
                {
                    DynamicPropertyService.DeleteDynamicPropertyValues(cart);
                }
                repository.UnitOfWork.Commit();
                //Raise domain events after deletion
                EventPublisher.Publish(new CartChangedEvent(changedEntries));
            }
        }

        #endregion

        #region IShoppingCartSearchService Members

        public GenericSearchResult<ShoppingCart> Search(ShoppingCartSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<ShoppingCart>();
            using (var repository = RepositoryFactory())
            {
                var query = GetQuery(repository, criteria);

                query = SortQuery(query, criteria);

                retVal.TotalCount = query.Count();

                var cartIds = query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArray();
                var carts = GetByIds(cartIds);
                retVal.Results = carts.AsQueryable().OrderBySortInfos(GetSortInfos(criteria)).ToList();

                return retVal;
            }
        }

        protected virtual IQueryable<ShoppingCartEntity> SortQuery(IQueryable<ShoppingCartEntity> query, ShoppingCartSearchCriteria criteria)
        {
            return query.OrderBySortInfos(GetSortInfos(criteria));
        }

        protected virtual IQueryable<ShoppingCartEntity> GetQuery(ICartRepository repository, ShoppingCartSearchCriteria criteria)
        {
            var query = GetQueryable(repository);

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

            if (!string.IsNullOrEmpty(criteria.OrganizationId))
            {
                query = query.Where(x => x.OrganizationId == criteria.OrganizationId);
            }

            if (!criteria.CustomerIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.CustomerIds.Contains(x.CustomerId));
            }

            return query;
        }

        protected virtual IQueryable<ShoppingCartEntity> GetQueryable(ICartRepository repository)
        {
            return repository.ShoppingCarts;
        }

        protected virtual SortInfo[] GetSortInfos(ShoppingCartSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<ShoppingCartEntity>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
            }
            return sortInfos;
        }

        #endregion
    }
}
