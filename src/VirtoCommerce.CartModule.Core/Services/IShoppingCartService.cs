using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.CartModule.Core.Services
{
    [Obsolete(@"This interface left for compatibility purposes. Use ICrudService<ShoppingCart> instead.")]
    public interface IShoppingCartService
    {
        Task<ShoppingCart[]> GetByIdsAsync(string[] cartIds, string responseGroup = null);
        Task<ShoppingCart> GetByIdAsync(string cartId, string responseGroup = null);
        Task SaveChangesAsync(ShoppingCart[] carts);
        Task DeleteAsync(string[] cartIds, bool softDelete = false);
    }
}
