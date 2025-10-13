using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.CartModule.Data.Services;

public class LineItemSearchService : SearchService<LineItemSearchCriteria, LineItemSearchResult, LineItem, LineItemEntity>, ILineItemSearchService
{
    public LineItemSearchService(
        Func<ICartRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        ILineItemService crudService,
        IOptions<CrudOptions> crudOptions)
        : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
    {
    }

    protected override IQueryable<LineItemEntity> BuildQuery(IRepository repository, LineItemSearchCriteria criteria)
    {
        var query = ((ICartRepository)repository).LineItems;

        if (!string.IsNullOrEmpty(criteria.ShoppingCartId))
        {
            query = query.Where(x => x.ShoppingCartId == criteria.ShoppingCartId);
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(LineItemSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos = new[]
            {
                    new SortInfo
                    {
                        SortColumn = ReflectionUtility.GetPropertyName<LineItemEntity>(x => x.CreatedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
        }

        return sortInfos;
    }
}
