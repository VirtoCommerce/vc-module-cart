using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Domain.Cart.Model;

namespace VirtoCommerce.CartModule.Data.Converters
{
	public static class CartShipmentConverter
	{
		public static Shipment ToShipmentModel(this ShipmentUpdateModel updateModel, string currency)
		{
			var shipmentModel = new Shipment
			{
				Currency = currency
			};

			shipmentModel.InjectFrom<NullableAndEnumValueInjecter>(updateModel);

			return shipmentModel;
		}
	}
}
