using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartSearchService : SearchService<ShoppingCartSearchCriteria, ShoppingCartSearchResult, ShoppingCart, ShoppingCartEntity>, IShoppingCartSearchService
    {
        public ShoppingCartSearchService(
            Func<ICartRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IShoppingCartService crudService,
            IOptions<CrudOptions> crudOptions)
            : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
        {
        }


        protected override IQueryable<ShoppingCartEntity> BuildQuery(IRepository repository, ShoppingCartSearchCriteria criteria)
        {
            var query = ((ICartRepository)repository).ShoppingCarts.Where(x => !x.IsDeleted);

            if (criteria.CustomerOrOrganization)
            {
                // build "CustomerId or OrganizationId" predicate
                var predicate = PredicateBuilder.False<ShoppingCartEntity>();

                if (!string.IsNullOrEmpty(criteria.OrganizationId))
                {
                    predicate = predicate.Or(x => x.OrganizationId == criteria.OrganizationId);
                }

                if (!string.IsNullOrEmpty(criteria.CustomerId))
                {
                    predicate = predicate.Or(x => x.CustomerId == criteria.CustomerId);
                }

                if (!criteria.CustomerIds.IsNullOrEmpty())
                {
                    predicate = predicate.Or(x => criteria.CustomerIds.Contains(x.CustomerId));
                }

                query = query.Where(predicate);
            }
            else
            {
                if (!string.IsNullOrEmpty(criteria.OrganizationId))
                {
                    query = query.Where(x => x.OrganizationId == criteria.OrganizationId);
                }

                if (!string.IsNullOrEmpty(criteria.CustomerId))
                {
                    query = query.Where(x => x.CustomerId == criteria.CustomerId);
                }

                if (!criteria.CustomerIds.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.CustomerIds.Contains(x.CustomerId));
                }
            }

            if (!string.IsNullOrEmpty(criteria.Status))
            {
                query = query.Where(x => x.Status == criteria.Status);
            }

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                query = query.Where(x => x.Name == criteria.Name);
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

        protected override IList<SortInfo> BuildSortExpression(ShoppingCartSearchCriteria criteria)
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

        protected override IChangeToken CreateCacheToken(ShoppingCartSearchCriteria criteria)
        {
            var customerKey = criteria.CustomerId ?? string.Empty;
            var customerToken = GenericSearchCachingRegion<ShoppingCart>.CreateChangeTokenForKey(customerKey);

            var result = customerToken;

            if (criteria.OrganizationId != null)
            {
                var organizationToken = GenericSearchCachingRegion<ShoppingCart>.CreateChangeTokenForKey(criteria.OrganizationId);

                var changeTokens = new List<IChangeToken>() { customerToken, organizationToken };
                result = new CompositeChangeToken(changeTokens);
            }

            return result;
        }
    }
}
