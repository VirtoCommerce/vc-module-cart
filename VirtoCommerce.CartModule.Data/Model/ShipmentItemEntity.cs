using System;
using System.ComponentModel.DataAnnotations;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class ShipmentItemEntity : AuditableEntity
	{
		[StringLength(128)]
		public string BarCode { get; set; }

		public int Quantity { get; set; }
	
		public virtual LineItemEntity LineItem { get; set; }
		public string LineItemId { get; set; }

		public virtual ShipmentEntity Shipment { get; set; }
		public string ShipmentId { get; set; }

        public virtual ShipmentItem ToModel(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new NullReferenceException("shipmentItem");

            shipmentItem.InjectFrom(this);

            return shipmentItem;
        }

        public virtual ShipmentItemEntity FromModel(ShipmentItem shipmentItem, PrimaryKeyResolvingMap pkMap)
        {
            if (shipmentItem == null)
                throw new NullReferenceException("shipmentItem");

            this.InjectFrom(shipmentItem);

            pkMap.AddPair(shipmentItem, this);
           
            return this;
        }

        /// <summary>
        /// Patch CatalogBase type
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public virtual void Patch(ShipmentItemEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            target.BarCode = this.BarCode;
            target.ShipmentId = this.ShipmentId;
            target.Quantity = this.Quantity;            
        }

    }
}
