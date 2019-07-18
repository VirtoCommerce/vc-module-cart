using System.Linq;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public interface ICartRepository : IRepository
    {
        IQueryable<ShoppingCartEntity> ShoppingCarts { get; }
        ShoppingCartEntity[] GetShoppingCartsByIds(string[] ids, string responseGroup = null);
        void RemoveCarts(string[] ids);

        void SoftRemoveCarts(string[] ids);
    }
}
