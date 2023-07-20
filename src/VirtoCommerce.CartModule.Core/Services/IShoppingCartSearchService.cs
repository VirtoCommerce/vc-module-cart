using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.CartModule.Core.Services
{
    public interface IShoppingCartSearchService : ISearchService<ShoppingCartSearchCriteria, ShoppingCartSearchResult, ShoppingCart>
    {
    }
}
