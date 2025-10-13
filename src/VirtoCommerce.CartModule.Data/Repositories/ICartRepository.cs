using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public interface ICartRepository : IRepository
    {
        IQueryable<ShoppingCartEntity> ShoppingCarts { get; }
        IQueryable<LineItemEntity> LineItems { get; }

        Task<IList<ShoppingCartEntity>> GetShoppingCartsByIdsAsync(IList<string> ids, string responseGroup = null);
        Task RemoveCartsAsync(IList<string> ids);
        Task SoftRemoveCartsAsync(IList<string> ids);
        Task<IList<ProductWishlistEntity>> FindWishlistsByProductsAsync(string customerId, string organizationId, string storeId, IList<string> productIds);

        Task<IList<LineItemEntity>> GetLineItemsByIdsAsync(IList<string> ids, string responseGroup = null);
    }
}
