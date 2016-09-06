using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Shipping.Model;

namespace VirtoCommerce.CartModule.Data.Services
{
    public interface IShoppingCartPromotionEvaluator
    {
        void EvaluatePromotions(ShoppingCart cart);
        void EvaluatePromotions(ShoppingCart cart, IEnumerable<ShippingRate> shippingRates);
    }
}
