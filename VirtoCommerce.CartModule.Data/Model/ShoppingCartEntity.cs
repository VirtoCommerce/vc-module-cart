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
	public class ShoppingCartEntity : AuditableEntity
	{
		public ShoppingCartEntity()
		{
            Discounts = new NullCollection<DiscountEntity>();
            Items = new NullCollection<LineItemEntity>();
			Payments = new NullCollection<PaymentEntity>();
			Addresses = new NullCollection<AddressEntity>();
			Shipments = new NullCollection<ShipmentEntity>();
			TaxDetails = new NullCollection<TaxDetailEntity>();
		}
	
		[StringLength(64)]
		public string Name { get; set; }
		[Required]
		[StringLength(64)]
		public string StoreId { get; set; }
		[StringLength(64)]
		public string ChannelId { get; set; }

		public bool IsAnonymous { get; set; }
		[Required]
		[StringLength(64)]
		public string CustomerId { get; set; }
		[StringLength(128)]
		public string CustomerName { get; set; }
		[StringLength(64)]
		public string OrganizationId { get; set; }
		[Required]
		[StringLength(3)]
		public string Currency { get; set; }
		[StringLength(64)]
		public string Coupon { get; set; }
		[StringLength(16)]
		public string LanguageCode { get; set; }
		public bool TaxIncluded { get; set; }
		public bool IsRecuring { get; set; }
		[StringLength(2048)]
		public string Comment { get; set; }
		[Column(TypeName = "Money")]
		public decimal Total { get; set; }
		[Column(TypeName = "Money")]
		public decimal SubTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal SubTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
		public decimal ShippingTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal ShippingTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
        public decimal PaymentTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal PaymentTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
		public decimal HandlingTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal HandlingTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
		public decimal DiscountTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "Money")]
		public decimal TaxTotal { get; set; }
        [StringLength(64)]
        public string ValidationType { get; set; }
        [StringLength(64)]
        public string Status { get; set; }
        [Column(TypeName = "Money")]
        public decimal Fee { get; set; }
        [Column(TypeName = "Money")]
        public decimal FeeWithTax { get; set; }

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; }
        public virtual ObservableCollection<AddressEntity> Addresses { get; set; }
		public virtual ObservableCollection<LineItemEntity> Items { get; set; }
		public virtual ObservableCollection<PaymentEntity> Payments { get; set; }
		public virtual ObservableCollection<ShipmentEntity> Shipments { get; set; }
		public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; }


        public virtual ShoppingCart ToModel(ShoppingCart cart)
        {         
            if (cart == null)
                throw new NullReferenceException("cart");

            cart.InjectFrom(this);

            cart.Discounts = this.Discounts.Select(x=> x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            cart.Items = this.Items.Select(x => x.ToModel(AbstractTypeFactory<LineItem>.TryCreateInstance())).ToList();
            cart.Addresses = this.Addresses.Select(x => x.ToModel(AbstractTypeFactory<Address>.TryCreateInstance())).ToList();
            cart.Shipments = this.Shipments.Select(x => x.ToModel(AbstractTypeFactory<Shipment>.TryCreateInstance())).ToList();
            cart.Payments = this.Payments.Select(x => x.ToModel(AbstractTypeFactory<Payment>.TryCreateInstance())).ToList();
            cart.TaxDetails = this.TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();

            if (!string.IsNullOrEmpty(this.Coupon))
            {
                cart.Coupon = new Coupon
                {
                    Code = this.Coupon
                };
            }
            return cart;
        }

        public virtual ShoppingCartEntity FromModel(ShoppingCart cart, PrimaryKeyResolvingMap pkMap)
        {

           if (cart == null)
                throw new NullReferenceException("cart");

            pkMap.AddPair(cart, this);

            this.InjectFrom(cart);

            if (cart.Addresses != null)
            {
                this.Addresses = new ObservableCollection<AddressEntity>(cart.Addresses.Select(x => AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(x)));
            }
            if (cart.Items != null)
            {
                this.Items = new ObservableCollection<LineItemEntity>(cart.Items.Select(x => AbstractTypeFactory<LineItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            if (cart.Shipments != null)
            {
                this.Shipments = new ObservableCollection<ShipmentEntity>(cart.Shipments.Select(x => AbstractTypeFactory<ShipmentEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            if (cart.Payments != null)
            {
                this.Payments = new ObservableCollection<PaymentEntity>(cart.Payments.Select(x => AbstractTypeFactory<PaymentEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            if (cart.Discounts != null)
            {
                this.Discounts = new ObservableCollection<DiscountEntity>(cart.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }
            if (cart.TaxDetails != null)
            {
                this.TaxDetails = new ObservableCollection<TaxDetailEntity>(cart.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }
            if (cart.Coupon != null)
            {
                this.Coupon = cart.Coupon.Code;
            }
            return this;
        }

        public virtual void Patch(ShoppingCartEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            target.Fee = this.Fee;
            target.FeeWithTax = this.FeeWithTax;
            target.Status = this.Status;
            target.Currency = this.Currency;
            target.ValidationType = this.ValidationType;
            target.CustomerId = this.CustomerId;
            target.CustomerName = this.CustomerName;
            target.IsAnonymous = this.IsAnonymous;
            target.IsRecuring = this.IsRecuring;
            target.LanguageCode = this.LanguageCode;
            target.Comment = this.Comment;
            target.OrganizationId = this.OrganizationId;
            target.Total = this.Total;
            target.SubTotal = this.SubTotal;
            target.SubTotalWithTax = this.SubTotalWithTax;
            target.ShippingTotal = this.ShippingTotal;
            target.ShippingTotalWithTax = this.ShippingTotalWithTax;
            target.PaymentTotal = this.PaymentTotal;
            target.PaymentTotalWithTax = this.PaymentTotalWithTax;
            target.HandlingTotal = this.HandlingTotal;
            target.HandlingTotalWithTax = this.HandlingTotalWithTax;
            target.DiscountTotal = this.DiscountTotal;
            target.DiscountTotalWithTax = this.DiscountTotalWithTax;
            target.DiscountAmount = this.DiscountAmount;
            target.TaxTotal = this.TaxTotal;
            target.Coupon = this.Coupon;         

            if (!this.Items.IsNullCollection())
            {
                this.Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }

            if (!this.Payments.IsNullCollection())
            {
                this.Payments.Patch(target.Payments, (sourcePayment, targetPayment) => sourcePayment.Patch(targetPayment));
            }

            if (!this.Addresses.IsNullCollection())
            {
                this.Addresses.Patch(target.Addresses, new AddressComparer(), (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!this.Shipments.IsNullCollection())
            {
                this.Shipments.Patch(target.Shipments, (sourceShipment, targetShipment) => sourceShipment.Patch(targetShipment));
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
