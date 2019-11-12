using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Core.Services
{
    public interface IAbandonedCartResolver
    {
        Task<AbandonedCart> ResolveAsync(ShoppingCart cart);
    }
}
