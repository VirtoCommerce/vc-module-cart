using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Cart.Events;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Common.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CartModule.Data.Handlers
{
    public class DeleteDynamicPropertiesForRemovedLineItemsCartChangedEventHandler : IEventHandler<CartChangedEvent>
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public DeleteDynamicPropertiesForRemovedLineItemsCartChangedEventHandler(IDynamicPropertyService dynamicPropertyService)
        {
            _dynamicPropertyService = dynamicPropertyService;
        }

        public virtual Task Handle(CartChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries.Where(x => x.EntryState == Platform.Core.Common.EntryState.Modified))
            {
                TryDeleteDynamicPropertiesForRemovedLineItems(changedEntry);
            }
            return Task.CompletedTask;
        }

        protected virtual void TryDeleteDynamicPropertiesForRemovedLineItems(GenericChangedEntry<ShoppingCart> changedEntry)
        {
            var originalDynPropOwners = changedEntry.OldEntry.GetFlatObjectsListWithInterface<IHasDynamicProperties>()
                                          .Distinct()
                                          .ToList();
            var modifiedDynPropOwners = changedEntry.NewEntry.GetFlatObjectsListWithInterface<IHasDynamicProperties>()
                                         .Distinct()
                                         .ToList();
            var removingDynPropOwners = new List<IHasDynamicProperties>();
            var hasDynPropComparer = AnonymousComparer.Create((IHasDynamicProperties x) => x.Id);
            modifiedDynPropOwners.CompareTo(originalDynPropOwners, hasDynPropComparer, (state, changed, orig) =>
            {
                if (state == EntryState.Deleted)
                {
                    _dynamicPropertyService.DeleteDynamicPropertyValues(orig);
                }

            });

        }
    }
}
