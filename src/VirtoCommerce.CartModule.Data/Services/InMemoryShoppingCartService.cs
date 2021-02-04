using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class InMemoryShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartTotalsCalculator _totalsCalculator;
        private readonly IEventPublisher _eventPublisher;
        private readonly InMemoryCartRepository _repository;

        public InMemoryShoppingCartService(IShoppingCartTotalsCalculator totalsCalculator, IEventPublisher eventPublisher, InMemoryCartRepository repository)
        {
            _totalsCalculator = totalsCalculator;
            _eventPublisher = eventPublisher;
            _repository = repository;
        }

        public async Task<ShoppingCart> GetByIdAsync(string cartId, string responseGroup = null)
        {
            var carts = await GetByIdsAsync(new[] { cartId }, responseGroup);
            return carts.FirstOrDefault();
        }

        public async Task<ShoppingCart[]> GetByIdsAsync(string[] cartIds, string responseGroup = null)
        {
            var result = new List<ShoppingCart>();

            var cartEntities = await _repository.GetShoppingCartsByIdsAsync(cartIds, responseGroup);
            foreach (var cartEntity in cartEntities)
            {
                var cart = cartEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance());

                //Calculate totals only for full responseGroup
                if (responseGroup == null)
                {
                    _totalsCalculator.CalculateTotals(cart);
                }

                cart.ReduceDetails(responseGroup);

                result.Add(cart);
            }

            return result.Select(x => x.Clone() as ShoppingCart).ToArray();
        }

        public async Task SaveChangesAsync(ShoppingCart[] carts)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            var changedEntries = new List<GenericChangedEntry<ShoppingCart>>();

            var dataExistCarts = await _repository.GetShoppingCartsByIdsAsync(carts.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());

            foreach (var cart in carts)
            {
                _totalsCalculator.CalculateTotals(cart);

                var originalEntity = dataExistCarts.FirstOrDefault(x => x.Id == cart.Id);
                var modifiedEntity = AbstractTypeFactory<ShoppingCartEntity>.TryCreateInstance().FromModel(cart, pkMap);

                if (originalEntity != null)
                {
                    changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, originalEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()), EntryState.Modified));
                    modifiedEntity.Patch(originalEntity);
                }
                else
                {
                    _repository.Add(modifiedEntity);
                    changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, EntryState.Added));
                }

                //Raise domain events
                await _eventPublisher.Publish(new CartChangeEvent(changedEntries));

                Save(modifiedEntity);

                await _eventPublisher.Publish(new CartChangedEvent(changedEntries));
            }
        }

        public async Task DeleteAsync(string[] cartIds, bool softDelete = false)
        {
            foreach (var cartId in cartIds)
            {
                _repository.Remove(cartId);
            }

            await Task.FromResult((object)null);
        }

        private void Save(ShoppingCartEntity cart)
        {
            SetId(cart);
            SetId(cart.Addresses.OfType<Entity>().ToArray());
            SetId(cart.Discounts.OfType<Entity>().ToArray());
            SetId(cart.Items.OfType<Entity>().ToArray());
            SetId(cart.Payments.OfType<Entity>().ToArray());
            SetId(cart.Shipments.OfType<Entity>().ToArray());
            SetId(cart.TaxDetails.OfType<Entity>().ToArray());
            SetId(cart.Discounts.OfType<Entity>().ToArray());
            SetId(cart.Coupons.OfType<Entity>().ToArray());
            SetId(cart.Addresses.OfType<Entity>().ToArray());
            SetId(cart.Addresses.OfType<Entity>().ToArray());
        }

        private void SetId(Entity entity)
        {
            if (entity.IsTransient())
            {
                entity.Id = Guid.NewGuid().ToString();
            }
        }

        private void SetId(IList<Entity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    SetId(entity);
                }
            }
        }
    }
}
