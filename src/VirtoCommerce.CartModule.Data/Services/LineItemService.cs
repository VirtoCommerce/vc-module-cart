using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.CartModule.Data.Services;

public class LineItemService : CrudService<LineItem, LineItemEntity, LineItemChangeEvent, LineItemChangedEvent>, ILineItemService
{
    public LineItemService(Func<IRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher)
        : base(repositoryFactory, platformMemoryCache, eventPublisher)
    {
    }

    protected override Task<IList<LineItemEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return ((ICartRepository)repository).GetLineItemsByIdsAsync(ids, responseGroup);
    }
}
