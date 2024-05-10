using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Data.Model;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public interface ICartRawDatabaseCommand
    {
        Task SoftRemove(CartDbContext dbContext, IList<string> ids);

        Task<IList<ProductWishlistEntity>> FindWishlistsByProductsAsync(CartDbContext dbContext, string customerId, string organizationId, string storeId, IList<string> productIds);
    }
}
