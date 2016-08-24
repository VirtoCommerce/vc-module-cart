using System.Collections.Generic;
using VirtoCommerce.Domain.Commerce.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class ShipmentUpdateModel
    {
        public ShipmentUpdateModel()
        {
            Items = new List<ShipmentItemUpdateModel>();
        }

		public string Id { get; set; }

        public string ShipmentMethodCode { get; set; }

        public string ShipmentMethodOption { get; set; }

        public string FulfilmentCenterId { get; set; }

        public Address DeliveryAddress { get; set; }

        public decimal ShippingPrice { get; set; }

        public ICollection<ShipmentItemUpdateModel> Items { get; set; }
    }
}