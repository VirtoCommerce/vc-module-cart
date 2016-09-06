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
	public class LineItemEntity : AuditableEntity
	{
		public LineItemEntity()
		{
			TaxDetails = new NullCollection<TaxDetailEntity>();
            Discounts = new NullCollection<DiscountEntity>();
        }

		[Required]
		[StringLength(3)]
		public string Currency { get; set; }
		[Required]
		[StringLength(64)]
		public string ProductId { get; set; }
        [Required]
        [StringLength(64)]
        public string Sku { get; set; }
        [Required]
		[StringLength(64)]
		public string CatalogId { get; set; }

		[StringLength(64)]
		public string CategoryId { get; set; }

        [StringLength(64)]
        public string ProductType { get; set; }
        [Required]
		[StringLength(256)]
		public string Name { get; set; }

		public int Quantity { get; set; }

		[StringLength(64)]
		public string FulfilmentLocationCode { get; set; }

		[StringLength(64)]
		public string ShipmentMethodCode { get; set; }
		public bool RequiredShipping { get; set; }
		[StringLength(1028)]
		public string ImageUrl { get; set; }

		public bool IsGift { get; set; }

		[StringLength(16)]
		public string LanguageCode { get; set; }

		[StringLength(2048)]
		public string Comment { get; set; }

        [StringLength(64)]
        public string ValidationType { get; set; }

        public bool IsReccuring { get; set; }

		public bool TaxIncluded { get; set; }

		public decimal? VolumetricWeight { get; set; }

		[StringLength(32)]
		public string WeightUnit { get; set; }
		public decimal? Weight { get; set; }
		[StringLength(32)]
		public string MeasureUnit { get; set; }
		public decimal? Height { get; set; }
		public decimal? Length { get; set; }
		public decimal? Width { get; set; }

        [StringLength(128)]
        public string PriceId { get; set; }

        [Column(TypeName = "Money")]
		public decimal ListPrice { get; set; }
        [Column(TypeName = "Money")]
        public decimal ListPriceWithTax { get; set; }
        [Column(TypeName = "Money")]
		public decimal SalePrice { get; set; }
        [Column(TypeName = "Money")]
        public decimal SalePriceWithTax { get; set; }     
		[Column(TypeName = "Money")]
		public decimal DiscountAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountAmountWithTax { get; set; }
        [Column(TypeName = "Money")]
		public decimal TaxTotal { get; set; }
		[StringLength(64)]
		public string TaxType { get; set; }

		public virtual ShoppingCartEntity ShoppingCart { get; set; }
		public string ShoppingCartId { get; set; }

		public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; }

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; }

        public virtual LineItem ToModel(LineItem lineItem)
        {
            if (lineItem == null)
                throw new ArgumentNullException("lineItem");

            lineItem.InjectFrom(this);
            if (!this.Discounts.IsNullOrEmpty())
            {
                lineItem.Discounts = this.Discounts.Select(x=> x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            }
            lineItem.TaxDetails = this.TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            return lineItem;
        }

        public virtual LineItemEntity FromModel(LineItem lineItem, PrimaryKeyResolvingMap pkMap)
        {
            if (lineItem == null)
                throw new ArgumentNullException("lineItem");

            pkMap.AddPair(lineItem, this);

            this.InjectFrom(lineItem);

            if (!lineItem.Discounts.IsNullOrEmpty())
            {
                this.Discounts = new ObservableCollection<DiscountEntity>(lineItem.Discounts.Select(x=> AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }
            if (lineItem.TaxDetails != null)
            {
                this.TaxDetails = new ObservableCollection<TaxDetailEntity>();
                this.TaxDetails.AddRange(lineItem.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }
            return this;
        }

        public virtual void Patch(LineItemEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            target.ListPrice = this.ListPrice;
            target.ListPriceWithTax = this.ListPriceWithTax;
            target.SalePrice = this.SalePrice;
            target.SalePriceWithTax = this.SalePriceWithTax;
            target.DiscountAmount = this.DiscountAmount;
            target.DiscountAmountWithTax = this.DiscountAmountWithTax;
            target.Quantity = this.Quantity;
            target.TaxTotal = this.TaxTotal;
            target.Weight = this.Weight;
            target.Height = this.Height;
            target.Width = this.Width;
            target.MeasureUnit = this.MeasureUnit;
            target.WeightUnit = this.WeightUnit;
            target.Length = this.Length;
            target.TaxType = this.TaxType;
      
            if (!this.Discounts.IsNullCollection())
            {
                var discountComparer = AnonymousComparer.Create((DiscountEntity x) => x.PromotionId);
                this.Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }

            if (!this.TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AnonymousComparer.Create((TaxDetailEntity x) => x.Name);
                this.TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }
        }
    }
}
