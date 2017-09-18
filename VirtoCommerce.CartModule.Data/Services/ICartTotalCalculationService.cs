using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Cart.Model;

namespace VirtoCommerce.CartModule.Data.Services
{
    public interface ICartTotalCalculationService
    {
        void CalculateCartTotals(ShoppingCart cart);
    }
}
