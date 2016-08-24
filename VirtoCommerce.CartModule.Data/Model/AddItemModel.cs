using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.CartModule.Data.Model
{
	public class AddItemModel
	{
		public string ProductId { get; set; }

		public string ImageUrl { get; set; }

		public int Quantity { get; set; }

		public string CatalogId { get; set; }

		public string Sku { get; set; }

		public string Name { get; set; }

		public decimal ListPrice { get; set; }

		public decimal SalePrice { get; set; }

		public decimal PlacedPrice { get; set; }

		public decimal ExtendedPrice { get; set; }

		public decimal DiscountTotal { get; set; }

		public decimal TaxTotal { get; set; }
	}
}
