using System.Collections.Generic;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CartModule.Core.Events
{
    public class LineItemChangeEvent : GenericChangedEntryEvent<LineItem>
    {
        public LineItemChangeEvent(IEnumerable<GenericChangedEntry<LineItem>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
