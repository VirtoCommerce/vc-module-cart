using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Cart.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
	public class ShippingRate : Domain.Shipping.Model.ShippingRate
	{
		public ICollection<Discount> Discounts { get; set; }

		public decimal TaxTotal { get; set; }
	}
}
