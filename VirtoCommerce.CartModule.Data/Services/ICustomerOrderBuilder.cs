using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Order.Model;

namespace VirtoCommerce.CartModule.Data.Services
{
    public interface ICustomerOrderBuilder
    {
        CustomerOrder PlaceCustomerOrder(IShoppingCartBuilder cartBuilder);
    }
}
