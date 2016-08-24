using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Cart.Model;

namespace VirtoCommerce.CartModule.Data.Builders
{
    public interface ICartValidator
    {
        void Validate(ShoppingCart cart);
    }
}