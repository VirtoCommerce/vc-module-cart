using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Swagger;

namespace VirtoCommerce.CartModule.Core.Model
{
    [SwaggerSchemaId("CartLineItem")]
    public class LineItem : AuditableEntity, IHasTaxDetalization, IHasDynamicProperties, ITaxable, IHasDiscounts, ICloneable
    {
        public string ProductId { get; set; }
        public string CatalogId { get; set; }
        public string CategoryId { get; set; }
        public string Sku { get; set; }
        public string ProductType { get; set; }

        public string Name { get; set; }
        public string ProductOuterId { get; set; }
        public int Quantity { get; set; }

        public string FulfillmentCenterId { get; set; }
        public string FulfillmentCenterName { get; set; }

        public string FulfillmentLocationCode { get; set; }
        public string ShipmentMethodCode { get; set; }
        public bool RequiredShipping { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string ImageUrl { get; set; }

        public bool IsGift { get; set; }

        /// <summary>
        /// Flag indicating that this lineItem was not accepted. Eg., gift was rejected by customer.
        /// </summary>
        public bool IsRejected { get; set; }
        public string Currency { get; set; }

        public string LanguageCode { get; set; }

        public string Note { get; set; }

        public bool IsReccuring { get; set; }

        public bool TaxIncluded { get; set; }

        public bool IsConfigured { get; set; }

        public decimal? VolumetricWeight { get; set; }


        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }

        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }

        /// <summary>
        /// Represent any line item validation type (noPriceValidate, noQuantityValidate etc) this value can be used in storefront 
        /// to select appropriate validation strategy
        /// </summary>
        public string ValidationType { get; set; }
        /// <summary>
        /// Flag represent that current line item its is read only and cannot be changed or removed from cart
        /// </summary>
        public bool IsReadOnly { get; set; }

        public string PriceId { get; set; }

        public virtual decimal ListPrice { get; set; }

        public decimal ListPriceWithTax { get; set; }

        public decimal ListTotal { get; set; }
        public decimal ListTotalWithTax { get; set; }

        private decimal? _salePrice;
        public virtual decimal SalePrice
        {
            get
            {
                return _salePrice ?? ListPrice;
            }
            set
            {
                _salePrice = value;
            }
        }

        public decimal SalePriceWithTax { get; set; }

        /// <summary>
        /// Resulting price with discount for one unit
        /// </summary>
        public virtual decimal PlacedPrice { get; set; }
        public virtual decimal PlacedPriceWithTax { get; set; }

        public virtual decimal ExtendedPrice { get; set; }

        public virtual decimal ExtendedPriceWithTax { get; set; }

        /// <summary>
        /// Gets the value of the single qty line item discount amount
        /// </summary>
        public virtual decimal DiscountAmount { get; set; }

        /// <summary>
        /// Indicates whether the discount amount per item was rounded according to the currency settings.
        /// If false, DiscountAmount and PlacedPrice should not be visible to the customer, as these values may be incorrect;
        /// in this case, DiscountTotal and ExtendedPrice should be used.
        /// </summary>
        public bool IsDiscountAmountRounded { get; set; }

        public virtual decimal DiscountAmountWithTax { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountTotalWithTax { get; set; }

        //Any extra Fee 
        public virtual decimal Fee { get; set; }

        public virtual decimal FeeWithTax { get; set; }

        public string VendorId { get; set; }

        public bool SelectedForCheckout { get; set; } = true;

        #region IHasDiscounts
        public ICollection<Discount> Discounts { get; set; }
        #endregion

        #region ITaxable Members

        /// <summary>
        /// Tax category or type
        /// </summary>
        public string TaxType { get; set; }


        public decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        #endregion

        #region IHaveTaxDetalization Members
        public ICollection<TaxDetail> TaxDetails { get; set; }
        #endregion

        #region IHasDynamicProperties Members
        public virtual string ObjectType => typeof(LineItem).FullName;
        public virtual ICollection<DynamicObjectProperty> DynamicProperties { get; set; }

        #endregion

        public ICollection<ConfigurationItem> ConfigurationItems { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as LineItem;

            result.TaxDetails = TaxDetails?.Select(x => x.Clone()).OfType<TaxDetail>().ToList();
            result.Discounts = Discounts?.Select(x => x.Clone()).OfType<Discount>().ToList();
            result.DynamicProperties = DynamicProperties?.Select(x => x.Clone()).OfType<DynamicObjectProperty>().ToList();

            return result;
        }

        #endregion
    }
}
