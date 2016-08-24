using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;

namespace VirtoCommerce.CartModule.Data.Converters
{
	public static class ShippingRateConveter
	{
		public static Model.ShippingRate ToDataModel(this Domain.Shipping.Model.ShippingRate shippingRate)
		{
			var result = new Model.ShippingRate();

			result.InjectFrom(shippingRate);

			return result;
		}
	}
}
