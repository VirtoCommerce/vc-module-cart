using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class InMemoryShoppingCartSearchService : IShoppingCartSearchService
    {
        private readonly IShoppingCartService _cartService;
        private readonly InMemoryCartRepository _repository;

        public InMemoryShoppingCartSearchService(IShoppingCartService cartService, InMemoryCartRepository repository)
        {
            _cartService = cartService;
            _repository = repository;
        }

        public async Task<ShoppingCartSearchResult> SearchCartAsync(ShoppingCartSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<ShoppingCartSearchResult>.TryCreateInstance();

            var sortInfos = BuildSortExpression(criteria);
            var query = BuildQuery(_repository, criteria);

            result.TotalCount = query.Count();

            if (criteria.Take > 0)
            {
                var ids =  query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                 .Select(x => x.Id)
                                 .Skip(criteria.Skip).Take(criteria.Take)
                                 .ToArray();

                result.Results = (await _cartService.GetByIdsAsync(ids, criteria.ResponseGroup)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
            }

            return result;
        }

        protected virtual IQueryable<ShoppingCartEntity> BuildQuery(InMemoryCartRepository repository, ShoppingCartSearchCriteria criteria)
        {
            var query = repository.ShoppingCarts.Where(x => x.IsDeleted == false);

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

            if (criteria.CreatedStartDate != null)
            {
                query = query.Where(x => x.CreatedDate >= criteria.CreatedStartDate.Value);
            }

            if (criteria.CreatedEndDate != null)
            {
                query = query.Where(x => x.CreatedDate <= criteria.CreatedEndDate.Value);
            }

            if (criteria.ModifiedStartDate != null)
            {
                query = query.Where(x => x.ModifiedDate >= criteria.ModifiedStartDate.Value);
            }

            if (criteria.ModifiedEndDate != null)
            {
                query = query.Where(x => x.ModifiedDate <= criteria.ModifiedEndDate.Value);
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(ShoppingCartSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = ReflectionUtility.GetPropertyName<ShoppingCartEntity>(x => x.CreatedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }
    }
}
