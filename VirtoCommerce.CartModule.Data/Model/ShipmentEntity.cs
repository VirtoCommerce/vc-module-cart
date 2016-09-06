using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
	public class ShipmentEntity : AuditableEntity
	{
		public ShipmentEntity()
		{
            Items = new NullCollection<ShipmentItemEntity>();
            Discounts = new NullCollection<DiscountEntity>();
            Items = new NullCollection<ShipmentItemEntity>();
			Addresses = new NullCollection<AddressEntity>();
			TaxDetails = new NullCollection<TaxDetailEntity>();
		}

		[StringLength(64)]
		public string ShipmentMethodCode { get; set; }
        [StringLength(64)]
        public string ShipmentMethodOption { get; set; }

        [StringLength(64)]
		public string FulfilmentCenterId { get; set; }
		[Required]
		[StringLength(3)]
		public string Currency { get; set; }

		[StringLength(16)]
		public string WeightUnit { get; set; }
		public decimal? WeightValue { get; set; }
		public decimal? VolumetricWeight { get; set; }

		[StringLength(16)]
		public string DimensionUnit { get; set; }
		public decimal? DimensionHeight { get; set; }
		public decimal? DimensionLength { get; set; }
		public decimal? DimensionWidth { get; set; }

		public bool TaxIncluded { get; set; }
		[Column(TypeName = "Money")]
		public decimal ShippingPrice { get; set; }
        [Column(TypeName = "Money")]
        public decimal ShippingPriceWithTax { get; set; }

        [Column(TypeName = "Money")]
		public decimal DiscountTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountTotalWithTax { get; set; }

        [Column(TypeName = "Money")]
		public decimal TaxTotal { get; set; }

        [Column(TypeName = "Money")]
        public decimal Total { get; set; }
        [Column(TypeName = "Money")]
        public decimal TotalWithTax { get; set; }

        [StringLength(64)]
		public string TaxType { get; set; }

        public virtual ObservableCollection<ShipmentItemEntity> Items { get; set; }
        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; }
        public virtual ObservableCollection<AddressEntity> Addresses { get; set; }
		public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; }
		public virtual ShoppingCartEntity ShoppingCart { get; set; }
		public string ShoppingCartId { get; set; }

        public virtual Shipment ToModel(Shipment shipment)
        {          
            if (shipment == null)
                throw new NullReferenceException("shipment");

            shipment.InjectFrom(this);

            if (!this.Addresses.IsNullOrEmpty())
            {
                shipment.DeliveryAddress = this.Addresses.First().ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
            }
            if (!this.Discounts.IsNullOrEmpty())
            {
                shipment.Discounts = this.Discounts.Select(x=> x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            }
            if (!this.Items.IsNullOrEmpty())
            {
                shipment.Items = this.Items.Select(x => x.ToModel(AbstractTypeFactory<ShipmentItem>.TryCreateInstance())).ToList();
            }
            if (!this.TaxDetails.IsNullOrEmpty())
            {
                shipment.TaxDetails = this.TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            }

            return shipment;
        }

        public virtual ShipmentEntity FromModel(Shipment shipment, PrimaryKeyResolvingMap pkMap)
        {         
            if (shipment == null)
                throw new NullReferenceException("shipment");

            this.InjectFrom(shipment);

            pkMap.AddPair(shipment, this);
            //Allow to empty address
            this.Addresses = new ObservableCollection<AddressEntity>();
            if (shipment.DeliveryAddress != null)
            {
                this.Addresses = new ObservableCollection<AddressEntity>(new AddressEntity[] { AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(shipment.DeliveryAddress) });
            }
            if (shipment.Items != null)
            {
                this.Items = new ObservableCollection<ShipmentItemEntity>(shipment.Items.Select(x => AbstractTypeFactory<ShipmentItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }          
            if (shipment.TaxDetails != null)
            {
                this.TaxDetails = new ObservableCollection<TaxDetailEntity>(shipment.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }
            if (shipment.Discounts != null)
            {
                this.Discounts = new ObservableCollection<DiscountEntity>(shipment.Discounts.Select(x=> AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }


            return this;
        }

        public virtual void Patch(ShipmentEntity target)
        {         
            if (target == null)
                throw new NullReferenceException("target");

            target.ShipmentMethodCode = this.ShipmentMethodCode;
            target.Total = this.Total;
            target.TotalWithTax = this.TotalWithTax;
            target.TaxTotal = this.TaxTotal;
            target.ShippingPrice = this.ShippingPrice;
            target.ShippingPriceWithTax = this.ShippingPriceWithTax;
            target.DiscountTotal = this.DiscountTotal;
            target.DiscountTotal = this.DiscountTotalWithTax;
            target.TaxIncluded = this.TaxIncluded;
            target.Currency = this.Currency;
            target.WeightUnit = this.WeightUnit;
            target.WeightValue = this.WeightValue;
            target.DimensionHeight = this.DimensionHeight;
            target.DimensionLength = this.DimensionLength;
            target.DimensionUnit = this.DimensionUnit;
            target.DimensionWidth = this.DimensionWidth;
            target.TaxType = this.TaxType;
            target.ShipmentMethodOption = this.ShipmentMethodOption;

            if (!this.Addresses.IsNullCollection())
            {
                this.Addresses.Patch(target.Addresses, new AddressComparer(), (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }
            if (this.Items != null)
            {
                this.Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }
            if (!this.TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AnonymousComparer.Create((TaxDetailEntity x) => x.Name);
                this.TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }
            if (!this.Discounts.IsNullCollection())
            {
                var discountComparer = AnonymousComparer.Create((DiscountEntity x) => x.PromotionId);
                this.Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }
        }
    }
}
