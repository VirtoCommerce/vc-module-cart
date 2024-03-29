using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class ShipmentItemEntity : AuditableEntity
    {
        [StringLength(128)]
        public string BarCode { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public LineItem ModelLineItem { get; set; }

        public string LineItemId { get; set; }
        public virtual LineItemEntity LineItem { get; set; }

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public virtual ShipmentItem ToModel(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            shipmentItem.Id = Id;
            shipmentItem.CreatedBy = CreatedBy;
            shipmentItem.CreatedDate = CreatedDate;
            shipmentItem.ModifiedBy = ModifiedBy;
            shipmentItem.ModifiedDate = ModifiedDate;

            shipmentItem.BarCode = BarCode;
            shipmentItem.Quantity = Quantity;

            shipmentItem.LineItemId = LineItemId;

            if (ModelLineItem != null)
            {
                shipmentItem.LineItem = ModelLineItem;
            }
            return shipmentItem;
        }

        public virtual ShipmentItemEntity FromModel(ShipmentItem shipmentItem, PrimaryKeyResolvingMap pkMap)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            Id = shipmentItem.Id;
            CreatedBy = shipmentItem.CreatedBy;
            CreatedDate = shipmentItem.CreatedDate;
            ModifiedBy = shipmentItem.ModifiedBy;
            ModifiedDate = shipmentItem.ModifiedDate;

            BarCode = shipmentItem.BarCode;
            Quantity = shipmentItem.Quantity;
            LineItemId = shipmentItem.LineItemId;

            pkMap.AddPair(shipmentItem, this);
            
            if (shipmentItem.LineItem != null)
            {
                LineItemId = shipmentItem.LineItem.Id;
                if (shipmentItem.LineItem.IsTransient())
                {
                    ModelLineItem = shipmentItem.LineItem;
                }
            }

            return this;
        }

        /// <summary>
        /// Patch CatalogBase type
        /// </summary>
        /// <param name="target"></param>
        public virtual void Patch(ShipmentItemEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.BarCode = BarCode;
            target.Quantity = Quantity;
        }
    }
}
