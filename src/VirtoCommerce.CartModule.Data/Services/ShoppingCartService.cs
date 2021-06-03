using System;
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
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartService : CrudService<ShoppingCart, ShoppingCartEntity, CartChangeEvent, CartChangedEvent>, IShoppingCartService
    {
        private readonly IShoppingCartTotalsCalculator _totalsCalculator;

        public ShoppingCartService(Func<ICartRepository> repositoryFactory,
                                      IShoppingCartTotalsCalculator totalsCalculator, IEventPublisher eventPublisher,
                                      IPlatformMemoryCache platformMemoryCache) : base(eventPublisher, platformMemoryCache, repositoryFactory)
        {
            _totalsCalculator = totalsCalculator;
        }

        protected override ShoppingCartEntity FromModel(ShoppingCartEntity entity, ShoppingCart model, PrimaryKeyResolvingMap pkMap)
        {
            return entity.FromModel(model, pkMap);
        }

        protected override ShoppingCart ToModel(ShoppingCartEntity entity, ShoppingCart model)
        {
            return entity.ToModel(model);
        }

        protected override void Patch(ShoppingCartEntity sourceEntity, ShoppingCartEntity targetEntity)
        {
            sourceEntity.Patch(targetEntity);
        }

        protected override ShoppingCart Clone(ShoppingCart model)
        {
            return (ShoppingCart)model.Clone();
        }

        protected override ShoppingCart PopulateModel(string responseGroup, ShoppingCartEntity entity, ShoppingCart model)
        {
            //Calculate totals only for full responseGroup
            if (responseGroup == null)
            {
                _totalsCalculator.CalculateTotals(model);
            }
            model.ReduceDetails(responseGroup);
            return model;
        }

        protected override Task<ShoppingCartEntity[]> LoadEntities(IRepository repository, string[] ids, string responseGroup)
        {
            return ((ICartRepository)repository).GetShoppingCartsByIdsAsync(ids, responseGroup);
        }

        protected override void ValidateOnSave(ShoppingCart[] entitites)
        {
            new ShoppingCartsValidator().ValidateAndThrow(entitites);

            foreach (var cart in entitites)
            {
                //Calculate cart totals before save
                _totalsCalculator.CalculateTotals(cart);
            }
        }

        protected override Task SoftDelete(IRepository repository, string[] ids)
        {
            return ((ICartRepository)repository).SoftRemoveCartsAsync(ids);
        }

    }
}
