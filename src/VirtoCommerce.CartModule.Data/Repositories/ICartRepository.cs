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

        Task<IList<ShoppingCartEntity>> GetShoppingCartsByIdsAsync(IList<string> ids, string responseGroup = null);
        Task RemoveCartsAsync(IList<string> ids);
        Task SoftRemoveCartsAsync(IList<string> ids);
    }
}
