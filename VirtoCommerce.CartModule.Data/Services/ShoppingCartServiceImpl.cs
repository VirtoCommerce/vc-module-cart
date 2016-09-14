using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;


namespace VirtoCommerce.CartModule.Data.Services
{
	public class ShoppingCartServiceImpl : ServiceBase, IShoppingCartService, IShoppingCartSearchService
	{
		private const string _workflowName = "CartRecalculate";
		private Func<ICartRepository> _repositoryFactory;
		private readonly IEventPublisher<CartChangeEvent> _eventPublisher;
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public ShoppingCartServiceImpl(Func<ICartRepository> repositoryFactory, IEventPublisher<CartChangeEvent> eventPublisher, IDynamicPropertyService dynamicPropertyService)
		{
			_repositoryFactory = repositoryFactory;
			_eventPublisher = eventPublisher;
            _dynamicPropertyService = dynamicPropertyService;
        }

        #region IShoppingCartService Members
        public virtual ShoppingCart[] GetByIds(string[] cartIds, string responseGroup = null)
		{
			List<ShoppingCart> retVal = new List<ShoppingCart>();
			using (var repository = _repositoryFactory())
			{
				var cartEntities = repository.GetShoppingCartsByIds(cartIds);
				foreach(var cartEntity in cartEntities)
				{
					var cart = cartEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance());
                    retVal.Add(cart);
				}
			}
            _dynamicPropertyService.LoadDynamicPropertyValues(retVal.ToArray());
            return retVal.ToArray();
		}

        public virtual void SaveChanges(ShoppingCart[] carts)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dataExistCarts = repository.GetShoppingCartsByIds(carts.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var cart in carts)
                {
                    var sourceCartEntity = AbstractTypeFactory<ShoppingCartEntity>.TryCreateInstance();
                    if (sourceCartEntity != null)
                    {
                        sourceCartEntity = sourceCartEntity.FromModel(cart, pkMap);
                        var targetCartEntity = dataExistCarts.FirstOrDefault(x => x.Id == cart.Id);
                        if (targetCartEntity != null)
                        {
                            var origCart = targetCartEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance());
                            _eventPublisher.Publish(new CartChangeEvent(EntryState.Modified, origCart, cart));

                            changeTracker.Attach(targetCartEntity);
                            sourceCartEntity.Patch(targetCartEntity);
                        }
                        else
                        {
                            repository.Add(sourceCartEntity);
                            _eventPublisher.Publish(new CartChangeEvent(EntryState.Added, cart, cart));
                        }
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

        }

        public virtual void Delete(string[] ids)
        {
            var carts = GetByIds(ids);
            using (var repository = _repositoryFactory())
            {
                var cartEntities = repository.GetShoppingCartsByIds(ids);
                foreach (var cart in carts)
                {
                    _eventPublisher.Publish(new CartChangeEvent(Platform.Core.Common.EntryState.Deleted, cart, cart));
                }
                repository.RemoveCarts(ids);
                foreach (var cart in carts)
                {
                    _dynamicPropertyService.DeleteDynamicPropertyValues(cart);
                }
                repository.UnitOfWork.Commit();
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
                if(!string.IsNullOrEmpty(criteria.Currency))
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
                retVal.Results = carts.AsQueryable<ShoppingCart>().OrderBySortInfos(sortInfos).ToList();
                return retVal;
            }
        } 
        #endregion

    }
}
