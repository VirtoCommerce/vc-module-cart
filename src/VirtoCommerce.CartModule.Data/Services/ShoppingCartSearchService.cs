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
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartSearchService : SearchService<ShoppingCartSearchCriteria, ShoppingCartSearchResult, ShoppingCart, ShoppingCartEntity>, IShoppingCartSearchService
    {
        private readonly ISearchPhraseParser _searchPhraseParser;

        public ShoppingCartSearchService(
            Func<ICartRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IShoppingCartService crudService,
            IOptions<CrudOptions> crudOptions,
            IOptionalDependency<ISearchPhraseParser> searchPhraseParser)
            : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
        {
            if (searchPhraseParser.HasValue)
            {
                _searchPhraseParser = searchPhraseParser.Value;
            }
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
                    predicate = predicate.Or(x => x.CustomerId == criteria.CustomerId && x.OrganizationId == null);
                }

                if (!criteria.CustomerIds.IsNullOrEmpty())
                {
                    predicate = predicate.Or(x => criteria.CustomerIds.Contains(x.CustomerId) && x.OrganizationId == null);
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

            if (criteria.IsAnonymous != null)
            {
                query = query.Where(x => x.IsAnonymous == criteria.IsAnonymous.Value);
            }

            if (criteria.HasLineItems != null)
            {
                query = criteria.HasLineItems.Value
                    ? query.Where(x => x.LineItemsCount > 0)
                    : query.Where(x => x.LineItemsCount <= 0);
            }

            if (!criteria.Types.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.Types.Contains(x.Type));
            }

            if (!criteria.NotTypes.IsNullOrEmpty())
            {
                query = query.Where(x => !criteria.NotTypes.Contains(x.Type));
            }

            if (criteria.HasAbandonmentNotification != null)
            {
                query = criteria.HasAbandonmentNotification.Value
                    ? query.Where(x => x.AbandonmentNotificationDate.HasValue)
                    : query.Where(x => !x.AbandonmentNotificationDate.HasValue);
            }

            if (criteria.AbandonmentNotificationStartDate != null)
            {
                query = query.Where(x => x.AbandonmentNotificationDate >= criteria.AbandonmentNotificationStartDate.Value);
            }

            if (criteria.AbandonmentNotificationEndDate != null)
            {
                query = query.Where(x => x.AbandonmentNotificationDate <= criteria.AbandonmentNotificationEndDate.Value);
            }

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                var keyword = criteria.Keyword;

                if (_searchPhraseParser != null)
                {
                    var searchPhrase = _searchPhraseParser.Parse(keyword);
                    query = query.ApplyFilters(searchPhrase.Filters);
                    keyword = searchPhrase.Keyword;
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(x =>
                        x.Name.Contains(keyword) ||
                        x.OrganizationName.Contains(keyword));
                }
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

            if (string.IsNullOrEmpty(criteria.OrganizationId))
            {
                return customerToken;
            }

            var organizationToken = GenericSearchCachingRegion<ShoppingCart>.CreateChangeTokenForKey(criteria.OrganizationId);

            return new CompositeChangeToken(new[] { customerToken, organizationToken });
        }
    }
}
