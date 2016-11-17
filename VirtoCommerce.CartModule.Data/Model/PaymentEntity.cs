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
	public class PaymentEntity : Entity
	{
        public PaymentEntity()
        {
            Addresses = new NullCollection<AddressEntity>();
            TaxDetails = new NullCollection<TaxDetailEntity>();
            Discounts = new NullCollection<DiscountEntity>();
        }

        [StringLength(128)]
		public string OuterId { get; set; }
		[Required]
		[StringLength(64)]
		public string Currency { get; set; }
		[StringLength(64)]
		public string PaymentGatewayCode { get; set; }
		[Column(TypeName = "Money")]
		public decimal Amount { get; set; }
		[StringLength(1024)]
		public string Purpose { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        [Column(TypeName = "Money")]
        public decimal Price { get; set; }
        [Column(TypeName = "Money")]
        public decimal PriceWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountAmountWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal Total { get; set; }
        [Column(TypeName = "Money")]
        public decimal TotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal TaxTotal { get; set; }
        public decimal TaxPercentRate { get; set; }

        public virtual ShoppingCartEntity ShoppingCart { get; set; }
		public string ShoppingCartId { get; set; }

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; }
        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; }

        public virtual ObservableCollection<AddressEntity> Addresses { get; set; }

        public virtual Payment ToModel(Payment payment)
        {
            if (payment == null)
                throw new NullReferenceException("payment");

            payment.InjectFrom(this);

            if (!this.TaxDetails.IsNullOrEmpty())
            {
                payment.TaxDetails = this.TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            }
            if (!this.Discounts.IsNullOrEmpty())
            {
                payment.Discounts = this.Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            }
            if (!this.Addresses.IsNullOrEmpty())
            {
                payment.BillingAddress = this.Addresses.First().ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
            }
            return payment;
        }

        public virtual PaymentEntity FromModel(Payment payment, PrimaryKeyResolvingMap pkMap)
        {
            if (payment == null)
                throw new NullReferenceException("payment");

            pkMap.AddPair(payment, this);
            this.InjectFrom(payment);
            if (payment.BillingAddress != null)
            {
                this.Addresses = new ObservableCollection<AddressEntity>(new AddressEntity[] { AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(payment.BillingAddress) });
            }
            if (payment.TaxDetails != null)
            {
                this.TaxDetails = new ObservableCollection<TaxDetailEntity>(payment.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }
            if (payment.Discounts != null)
            {
                this.Discounts = new ObservableCollection<DiscountEntity>(payment.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }
            return this;
        }

        public virtual void Patch(PaymentEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            target.Amount = this.Amount;
            target.PaymentGatewayCode = this.PaymentGatewayCode;


            target.Price = this.Price;
            target.PriceWithTax = this.PriceWithTax;
            target.DiscountAmount = this.DiscountAmount;
            target.DiscountAmountWithTax = this.DiscountAmountWithTax;
            target.TaxType = this.TaxType;
            target.TaxPercentRate = this.TaxPercentRate;
            target.TaxTotal = this.TaxTotal;
            target.Total = this.Total;
            target.TotalWithTax = this.TotalWithTax;
            target.Purpose = this.Purpose;
            target.OuterId = this.OuterId;

            if (!this.Addresses.IsNullCollection())
            {
                this.Addresses.Patch(target.Addresses, new AddressComparer(), (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
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
