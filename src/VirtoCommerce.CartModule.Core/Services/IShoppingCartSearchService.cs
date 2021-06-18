using System;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model.Search;

namespace VirtoCommerce.CartModule.Core.Services
{
    /// <summary>
    /// This interface should implement <see cref="ISearchService"/> without explicitly defined methods.
    /// Methods left for compatibility and should be removed after upgrade to inheritance
    /// </summary>
    public interface IShoppingCartSearchService
    {
        [Obsolete(@"Need to remove after inherit IShoppingCartService from ICrudService<ShoppingCart>.")]
        Task<ShoppingCartSearchResult> SearchCartAsync(ShoppingCartSearchCriteria criteria);
    }
}
