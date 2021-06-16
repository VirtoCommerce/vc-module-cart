using System;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Core.Services
{
    /// <summary>
    /// This interface should implement <see cref="ICrudService<ShoppingCart>"/> without methods.
    /// Methods left for compatibility and should be removed after upgrade to inheritance
    /// </summary>
    public interface IShoppingCartService
    {
        [Obsolete(@"Need to remove after inherit IShoppingCartService from ICrudService<ShoppingCart>.")]
        Task<ShoppingCart[]> GetByIdsAsync(string[] cartIds, string responseGroup = null);
        [Obsolete(@"Need to remove after inherit IShoppingCartService from ICrudService<ShoppingCart>.")]
        Task<ShoppingCart> GetByIdAsync(string cartId, string responseGroup = null);
        [Obsolete(@"Need to remove after inherit IShoppingCartService from ICrudService<ShoppingCart>.")]
        Task SaveChangesAsync(ShoppingCart[] carts);
        [Obsolete(@"Need to remove after inherit IShoppingCartService from ICrudService<ShoppingCart>.")]
        Task DeleteAsync(string[] cartIds, bool softDelete = false);
    }
}
