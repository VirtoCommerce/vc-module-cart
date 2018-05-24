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
    public class DeletePropertyCartChangedEventHandler : IEventHandler<CartChangedEvent>
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public DeletePropertyCartChangedEventHandler(IDynamicPropertyService dynamicPropertyService)
        {
            _dynamicPropertyService = dynamicPropertyService;
        }

        public Task Handle(CartChangedEvent message)
        {
                foreach (var changedEntry in message.ChangedEntries)
                {
                   if(changedEntry.NewEntry.Items.Count() < changedEntry.OldEntry.Items.Count())
                       TryDeleteCartProperty(changedEntry);
                }
            return Task.CompletedTask;
        }

        protected virtual void TryDeleteCartProperty(GenericChangedEntry<ShoppingCart> changedEntry)
        {
            var shoppingCart = changedEntry.OldEntry;

            var removedLineItemProperties = new List<DynamicObjectProperty>();

            var changedLineItems = changedEntry.NewEntry.Items.ToArray();
            var origLineItems = changedEntry.OldEntry.Items.ToArray();

            var intersect = origLineItems.Intersect(changedLineItems).ToArray();
            var removedLineItem = origLineItems.Except(intersect).ToArray();

            foreach (LineItem line in removedLineItem)
            {
                if(line.DynamicProperties != null)
                    removedLineItemProperties = line.DynamicProperties.ToList();
            }

            var cartItemProperties = new HashSet<DynamicObjectProperty>();

            //Delete cart line item dynamic properties
            var propertyIds = new List<string>();
            if (cartItemProperties != null && removedLineItemProperties.Count > 0)
            {
                foreach (DynamicObjectProperty prop in removedLineItemProperties)
                    propertyIds.Add(prop.Id);
            }
            if (propertyIds.Count > 0)
            {
                _dynamicPropertyService.DeleteDynamicPropertyValues(shoppingCart);
            }
        }
    }
}
