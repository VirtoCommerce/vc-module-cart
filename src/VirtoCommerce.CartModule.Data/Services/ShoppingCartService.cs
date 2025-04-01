using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Validation;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartService : CrudService<ShoppingCart, ShoppingCartEntity, CartChangeEvent, CartChangedEvent>, IShoppingCartService
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly IShoppingCartTotalsCalculator _totalsCalculator;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public ShoppingCartService(
            Func<ICartRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IEventPublisher eventPublisher,
            IShoppingCartTotalsCalculator totalsCalculator,
            IBlobUrlResolver blobUrlResolver)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _totalsCalculator = totalsCalculator;
            _blobUrlResolver = blobUrlResolver;
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
                if (cart.Type == ModuleConstants.WishlistCartType)
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

        private void ResolveFileUrls(ShoppingCart cart)
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
    }
}
