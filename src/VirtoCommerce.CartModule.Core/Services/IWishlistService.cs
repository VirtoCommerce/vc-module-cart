using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.CartModule.Core.Services
{
    public interface IWishlistService
    {
        Task<IList<string>> FindProductsInWishlistsAsync(string customerId, string organizationId, string storeId, IList<string> productIds);
    }
}
