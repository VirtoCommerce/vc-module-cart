using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
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
        private readonly IShoppingCartTotalsCalculator _totalsCalculator;

        public ShoppingCartService(
            Func<ICartRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IEventPublisher eventPublisher,
            IShoppingCartTotalsCalculator totalsCalculator)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
            _totalsCalculator = totalsCalculator;
        }

        protected override ShoppingCart ProcessModel(string responseGroup, ShoppingCartEntity entity, ShoppingCart model)
        {
            //Calculate totals only for full responseGroup
            if (responseGroup == null)
            {
                _totalsCalculator.CalculateTotals(model);
            }
            model.ReduceDetails(responseGroup);
            return model;
        }

        protected override Task BeforeSaveChanges(IList<ShoppingCart> models)
        {
            new ShoppingCartsValidator().ValidateAndThrow(models);

            foreach (var cart in models)
            {
                //Calculate cart totals before save
                _totalsCalculator.CalculateTotals(cart);
            }

            return Task.CompletedTask;
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
    }
}
