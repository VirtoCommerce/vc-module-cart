using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.CartModule.Core.Services
{
    public interface IWishlistService
    {
        Task<List<string>> FindProductsInWishlistsAsync(string customerId, string storeId, IList<string> productIds);
    }
}
