using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Validation;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using CartType = VirtoCommerce.CartModule.Core.ModuleConstants.CartType;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartService : CrudService<ShoppingCart, ShoppingCartEntity, CartChangeEvent, CartChangedEvent>, IShoppingCartService
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly IShoppingCartTotalsCalculator _totalsCalculator;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly IShippingMethodsSearchService _shippingMethodSearchService;

        public ShoppingCartService(
            Func<ICartRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IEventPublisher eventPublisher,
            IShoppingCartTotalsCalculator totalsCalculator,
            IBlobUrlResolver blobUrlResolver,
            IPaymentMethodsSearchService paymentMethodsSearchService,
            IShippingMethodsSearchService shippingMethodSearchService)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _totalsCalculator = totalsCalculator;
            _blobUrlResolver = blobUrlResolver;
            _paymentMethodsSearchService = paymentMethodsSearchService;
            _shippingMethodSearchService = shippingMethodSearchService;
        }

        protected override ShoppingCart ProcessModel(string responseGroup, ShoppingCartEntity entity, ShoppingCart model)
        {
            var cartResponseGroup = EnumUtility.SafeParse(responseGroup, CartResponseGroup.Full);
            if (cartResponseGroup.HasFlag(CartResponseGroup.RecalculateTotals))
            {
                _totalsCalculator.CalculateTotals(model);
            }
            model.ReduceDetails(responseGroup);
            ResolveFileUrls(model);
            ResolvePayments(model);
            ResolveShipments(model);

            return model;
        }

        protected override async Task BeforeSaveChanges(IList<ShoppingCart> models)
        {
            await new ShoppingCartsValidator().ValidateAndThrowAsync(models);

            using var repository = _repositoryFactory();

            foreach (var cart in models)
            {
                //Calculate cart totals before save
                _totalsCalculator.CalculateTotals(cart);
                if (cart.Type == CartType.Wishlist || cart.Type == CartType.SavedForLater)
                {
                    await ValidateName(cart, repository);
                }
            }
        }

        protected virtual async Task ValidateName(ShoppingCart cart, ICartRepository repository)
        {
            var resultName = cart.Name;
            var query = repository.ShoppingCarts.Where(x =>
                !x.IsDeleted && x.Id != cart.Id
                && ((cart.OrganizationId != null && x.OrganizationId == cart.OrganizationId)
                    || x.CustomerId == cart.CustomerId)
            );
            var index = 1;
            while (await query.AnyAsync(x => x.Name == resultName))
            {
                resultName = $"{cart.Name} ({index++})";
            }
            cart.Name = resultName;
        }

        protected override Task SoftDelete(IRepository repository, IList<string> ids)
        {
            return ((ICartRepository)repository).SoftRemoveCartsAsync(ids);
        }

        protected override Task<IList<ShoppingCartEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
        {
            return ((ICartRepository)repository).GetShoppingCartsByIdsAsync(ids, responseGroup);
        }

        // Saving a cart for one user (CustomerId) should not clear cache for other users
        protected override void ClearSearchCache(IList<ShoppingCart> models)
        {
            GenericSearchCachingRegion<ShoppingCart>.ExpireTokenForKey(string.Empty);

            var ids = models
                .SelectMany(x => new[] { x.CustomerId, x.OrganizationId })
                .Where(x => x != null)
                .Distinct();

            foreach (var id in ids)
            {
                GenericSearchCachingRegion<ShoppingCart>.ExpireTokenForKey(id);
            }
        }

        protected virtual void ResolveFileUrls(ShoppingCart cart)
        {
            if (cart.Items is null)
            {
                return;
            }

            var files = cart.Items
                .Where(i => i.ConfigurationItems != null)
                .SelectMany(i => i.ConfigurationItems
                    .Where(c => c.Files != null)
                    .SelectMany(c => c.Files
                        .Where(f => !string.IsNullOrEmpty(f.Url))));

            foreach (var file in files)
            {
                file.Url = file.Url.StartsWith("/api") ? file.Url : _blobUrlResolver.GetAbsoluteUrl(file.Url);
            }
        }

        protected virtual void ResolvePayments(ShoppingCart cart)
        {
            if (cart.Payments == null)
            {
                return;
            }

            var paymentMethodCodes = GetPaymentMethodCodesAsync(cart.StoreId);
            cart.Payments = cart.Payments.Where(x => x.PaymentGatewayCode == null || paymentMethodCodes.Contains(x.PaymentGatewayCode)).ToList();
        }

        protected virtual void ResolveShipments(ShoppingCart cart)
        {
            if (cart.Shipments == null)
            {
                return;
            }
            var shippingMethodCodes = GetShippingMethodCodesAsync(cart.StoreId);
            cart.Shipments = cart.Shipments.Where(x => x.ShipmentMethodCode == null || shippingMethodCodes.Contains(x.ShipmentMethodCode)).ToList();
        }

        protected string[] GetPaymentMethodCodesAsync(string storeId)
        {
            var criteria = AbstractTypeFactory<PaymentMethodsSearchCriteria>.TryCreateInstance();
            criteria.StoreId = storeId;
            criteria.IsActive = true;

            var paymentMethods = _paymentMethodsSearchService.SearchAllNoCloneAsync(criteria).GetAwaiter().GetResult();

            return paymentMethods.Select(x => x.Code).ToArray();
        }

        protected string[] GetShippingMethodCodesAsync(string storeId)
        {
            var criteria = AbstractTypeFactory<ShippingMethodsSearchCriteria>.TryCreateInstance();
            criteria.StoreId = storeId;
            criteria.IsActive = true;

            var shippingMethods = _shippingMethodSearchService.SearchAllNoCloneAsync(criteria).GetAwaiter().GetResult();

            return shippingMethods.Select(x => x.Code).ToArray();
        }
    }
}
