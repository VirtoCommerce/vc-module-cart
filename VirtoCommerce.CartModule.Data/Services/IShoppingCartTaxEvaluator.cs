using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Shipping.Model;

namespace VirtoCommerce.CartModule.Data.Services
{
    public interface IShoppingCartTaxEvaluator
    {
        void EvaluateTaxes(ShoppingCart cart);
        void EvaluateTaxes(ShoppingCart cart, IEnumerable<ShippingRate> shippingRates);
    }
}
