using Omu.ValueInjecter;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class PaymentEntity : Entity
    {
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

        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; } = new NullCollection<DiscountEntity>();
        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; } = new NullCollection<TaxDetailEntity>();
        public virtual ObservableCollection<AddressEntity> Addresses { get; set; } = new NullCollection<AddressEntity>();


        public virtual Payment ToModel(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            payment.InjectFrom(this);

            if (!TaxDetails.IsNullOrEmpty())
            {
                payment.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            }

            if (!Discounts.IsNullOrEmpty())
            {
                payment.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            }

            if (!Addresses.IsNullOrEmpty())
            {
                payment.BillingAddress = Addresses.First().ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
            }

            return payment;
        }

        public virtual PaymentEntity FromModel(Payment payment, PrimaryKeyResolvingMap pkMap)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            pkMap.AddPair(payment, this);
            this.InjectFrom(payment);

            if (payment.BillingAddress != null)
            {
                Addresses = new ObservableCollection<AddressEntity>(new[] { AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(payment.BillingAddress) });
            }

            if (payment.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(payment.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            if (payment.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(payment.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            return this;
        }

        public virtual void Patch(PaymentEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Amount = Amount;
            target.PaymentGatewayCode = PaymentGatewayCode;
            target.Price = Price;
            target.PriceWithTax = PriceWithTax;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
            target.TaxType = TaxType;
            target.TaxPercentRate = TaxPercentRate;
            target.TaxTotal = TaxTotal;
            target.Total = Total;
            target.TotalWithTax = TotalWithTax;
            target.Purpose = Purpose;
            target.OuterId = OuterId;

            if (!Addresses.IsNullCollection())
            {
                Addresses.Patch(target.Addresses, (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AbstractTypeFactory<TaxDetailEntityComparer>.TryCreateInstance();
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }

            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AbstractTypeFactory<DiscountEntityComparer>.TryCreateInstance();
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }
        }
    }
}
