using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class LineItemEntity : AuditableEntity
    {
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

        public bool IsReadOnly { get; set; }

        [StringLength(64)]
        public string FulfilmentCenterId { get; set; }

        [StringLength(128)]
        public string FulfillmentCenterName { get; set; }

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
        public decimal Fee { get; set; }

        [Column(TypeName = "Money")]
        public decimal FeeWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        [NotMapped]
        public LineItem ModelLineItem { get; set; }

        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; } = new NullCollection<TaxDetailEntity>();
        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; } = new NullCollection<DiscountEntity>();
        public virtual ObservableCollection<ShipmentItemEntity> ShipmentItems { get; set; } = new NullCollection<ShipmentItemEntity>();

        public virtual LineItem ToModel(LineItem lineItem)
        {
            if (lineItem == null)
                throw new ArgumentNullException(nameof(lineItem));

            lineItem.InjectFrom(this);
            lineItem.Note = Comment;

            if (!Discounts.IsNullOrEmpty())
            {
                lineItem.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            }

            if (!TaxDetails.IsNullOrEmpty())
            {
                lineItem.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            }

            return lineItem;
        }

        public virtual LineItemEntity FromModel(LineItem lineItem, PrimaryKeyResolvingMap pkMap)
        {
            if (lineItem == null)
                throw new ArgumentNullException(nameof(lineItem));

            pkMap.AddPair(lineItem, this);

            this.InjectFrom(lineItem);
            //Preserve link of the  original model LineItem for future references binding LineItems with  ShipmentLineItems 
            ModelLineItem = lineItem;

            Comment = lineItem.Note;

            if (lineItem.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(lineItem.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            if (lineItem.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(lineItem.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            return this;
        }

        public virtual void Patch(LineItemEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.ListPrice = ListPrice;
            target.ListPriceWithTax = ListPriceWithTax;
            target.SalePrice = SalePrice;
            target.SalePriceWithTax = SalePriceWithTax;
            target.Fee = Fee;
            target.FeeWithTax = FeeWithTax;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
            target.Quantity = Quantity;
            target.TaxTotal = TaxTotal;
            target.TaxPercentRate = TaxPercentRate;
            target.Weight = Weight;
            target.Height = Height;
            target.Width = Width;
            target.MeasureUnit = MeasureUnit;
            target.WeightUnit = WeightUnit;
            target.Length = Length;
            target.TaxType = TaxType;
            target.Comment = Comment;
            target.IsReadOnly = IsReadOnly;
            target.ValidationType = ValidationType;
            target.PriceId = PriceId;
            target.LanguageCode = LanguageCode;
            target.IsReccuring = IsReccuring;
            target.IsGift = IsGift;
            target.ImageUrl = ImageUrl;
            target.ProductId = ProductId;
            target.ProductType = ProductType;
            target.ShipmentMethodCode = ShipmentMethodCode;
            target.RequiredShipping = RequiredShipping;
            target.ProductType = ProductType;
            target.FulfilmentLocationCode = FulfilmentLocationCode;
            target.FulfilmentCenterId = FulfilmentCenterId;
            target.FulfillmentCenterName = FulfillmentCenterName;

            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AbstractTypeFactory<DiscountEntityComparer>.TryCreateInstance();
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }

            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AbstractTypeFactory<TaxDetailEntityComparer>.TryCreateInstance();
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }
        }
    }
}
