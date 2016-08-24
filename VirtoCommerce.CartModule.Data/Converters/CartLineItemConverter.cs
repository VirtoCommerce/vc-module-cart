using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Cart.Model;

namespace VirtoCommerce.CartModule.Data.Converters
{
	public static class CartLineItemConverter
	{
		public static ShipmentItem ToShipmentItem(this LineItem lineItem)
		{
			var shipmentItem = new ShipmentItem
			{
				LineItem = lineItem,
				Quantity = lineItem.Quantity
			};
			return shipmentItem;
		}
	}
}
