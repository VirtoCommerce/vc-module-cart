using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartSearchService : SearchService<ShoppingCartSearchCriteria, ShoppingCartSearchResult, ShoppingCart, ShoppingCartEntity>, IShoppingCartSearchService
    {
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly IShippingMethodsSearchService _shippingMethodSearchService;

        public ShoppingCartSearchService(
            Func<ICartRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IShoppingCartService crudService,
            IOptions<CrudOptions> crudOptions,
            IPaymentMethodsSearchService paymentMethodsSearchService,
            IShippingMethodsSearchService shippingMethodSearchService)
            : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
        {
            _paymentMethodsSearchService = paymentMethodsSearchService;
            _shippingMethodSearchService = shippingMethodSearchService;
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

            if (!string.IsNullOrEmpty(criteria.NotType))
            {
                query = query.Where(x => x.Type != criteria.NotType);
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

        protected override async Task<ShoppingCartSearchResult> ProcessSearchResultAsync(ShoppingCartSearchResult result, ShoppingCartSearchCriteria criteria)
        {
            var paymentMethodCodes = await GetPaymentMethodCodesAsync(criteria.StoreId);
            var shippingMethodCodes = await GetShippingMethodCodesAsync(criteria.StoreId);

            foreach (var cart in result.Results)
            {
                cart.Payments = cart.Payments.Where(x => paymentMethodCodes.Contains(x.PaymentGatewayCode)).ToList();
                cart.Shipments = cart.Shipments.Where(x => shippingMethodCodes.Contains(x.ShipmentMethodCode)).ToList();
            }

            return await base.ProcessSearchResultAsync(result, criteria);
        }

        private async Task<string[]> GetPaymentMethodCodesAsync(string storeId)
        {
            var criteria = AbstractTypeFactory<PaymentMethodsSearchCriteria>.TryCreateInstance();
            criteria.StoreId = storeId;
            criteria.IsActive = true;

            var paymentMethods = await _paymentMethodsSearchService.SearchAsync(criteria);

            return paymentMethods.Results.Select(x => x.Code).ToArray();
        }

        private async Task<string[]> GetShippingMethodCodesAsync(string storeId)
        {
            var criteria = AbstractTypeFactory<ShippingMethodsSearchCriteria>.TryCreateInstance();
            criteria.StoreId = storeId;
            criteria.IsActive = true;

            var shippingMethods = await _shippingMethodSearchService.SearchAsync(criteria);

            return shippingMethods.Results.Select(x => x.Code).ToArray();
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
