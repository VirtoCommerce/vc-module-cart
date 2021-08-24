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
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartService : CrudService<ShoppingCart, ShoppingCartEntity, CartChangeEvent, CartChangedEvent>, IShoppingCartService
    {
        private readonly IShoppingCartTotalsCalculator _totalsCalculator;

        public ShoppingCartService(Func<ICartRepository> repositoryFactory,
                                      IShoppingCartTotalsCalculator totalsCalculator, IEventPublisher eventPublisher,
                                      IPlatformMemoryCache platformMemoryCache) : base(repositoryFactory, platformMemoryCache, eventPublisher)
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

        protected override Task BeforeSaveChanges(IEnumerable<ShoppingCart> models)
        {
            new ShoppingCartsValidator().ValidateAndThrow(models);

            foreach (var cart in models)
            {
                //Calculate cart totals before save
                _totalsCalculator.CalculateTotals(cart);
            }

            return Task.CompletedTask;
        }

        protected override Task SoftDelete(IRepository repository, IEnumerable<string> ids)
        {
            return ((ICartRepository)repository).SoftRemoveCartsAsync(ids.ToArray());
        }


        protected async override Task<IEnumerable<ShoppingCartEntity>> LoadEntities(IRepository repository, IEnumerable<string> ids, string responseGroup)
        {
            return await ((ICartRepository)repository).GetShoppingCartsByIdsAsync(ids.ToArray(), responseGroup);
        }

        #region IShoppingCartService compatibility
        public async Task<ShoppingCart[]> GetByIdsAsync(string[] cartIds, string responseGroup = null)
        {
            return (await GetByIdsAsync((IEnumerable<string>)cartIds, responseGroup)).ToArray();
        }

        public Task SaveChangesAsync(ShoppingCart[] carts)
        {
            return SaveChangesAsync((IEnumerable<ShoppingCart>)carts);
        }

        public Task DeleteAsync(string[] cartIds, bool softDelete = false)
        {
            return DeleteAsync((IEnumerable<string>)cartIds, softDelete);
        }
        #endregion
    }
}
