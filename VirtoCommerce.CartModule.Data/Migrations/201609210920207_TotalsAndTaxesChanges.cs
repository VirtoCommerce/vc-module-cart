namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TotalsAndTaxesChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "SubTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "ShippingTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "HandlingTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "DiscountTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "DiscountAmount", c => c.Decimal(nullable: false, storeType: "money"));


            AddColumn("dbo.CartShipment", "Price", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "PriceWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "DiscountAmount", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "DiscountAmountWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 3, scale: 3));
            AddColumn("dbo.CartShipment", "TotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));

            AddColumn("dbo.CartDiscount", "DiscountAmountWithTax", c => c.Decimal(nullable: false, storeType: "money"));

            AddColumn("dbo.CartLineItem", "ListPriceWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "SalePriceWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "DiscountAmount", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "DiscountAmountWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 3, scale: 3));
            AddColumn("dbo.CartLineItem", "IsReadOnly", c => c.Boolean(nullable: false));

            Sql("UPDATE dbo.CartShipment SET Price = ShippingPrice, DiscountAmount = DiscountTotal");

            DropColumn("dbo.CartShipment", "ShippingPrice");
            DropColumn("dbo.CartShipment", "DiscountTotal");
            DropColumn("dbo.CartLineItem", "PlacedPrice");
            DropColumn("dbo.CartLineItem", "ExtendedPrice");
            DropColumn("dbo.CartLineItem", "DiscountTotal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CartLineItem", "DiscountTotal", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "ExtendedPrice", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "PlacedPrice", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "DiscountTotal", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "ShippingPrice", c => c.Decimal(nullable: false, storeType: "money"));
            DropColumn("dbo.CartLineItem", "TaxPercentRate");
            DropColumn("dbo.CartLineItem", "DiscountAmountWithTax");
            DropColumn("dbo.CartLineItem", "DiscountAmount");
            DropColumn("dbo.CartLineItem", "SalePriceWithTax");
            DropColumn("dbo.CartLineItem", "ListPriceWithTax");
            DropColumn("dbo.CartLineItem", "IsReadOnly");
            DropColumn("dbo.CartDiscount", "DiscountAmountWithTax");
            DropColumn("dbo.CartShipment", "TotalWithTax");
            DropColumn("dbo.CartShipment", "TaxPercentRate");
            DropColumn("dbo.CartShipment", "DiscountAmountWithTax");
            DropColumn("dbo.CartShipment", "DiscountAmount");
            DropColumn("dbo.CartShipment", "PriceWithTax");
            DropColumn("dbo.CartShipment", "Price");
            DropColumn("dbo.Cart", "DiscountAmount");
            DropColumn("dbo.Cart", "DiscountTotalWithTax");
            DropColumn("dbo.Cart", "HandlingTotalWithTax");
            DropColumn("dbo.Cart", "ShippingTotalWithTax");
            DropColumn("dbo.Cart", "SubTotalWithTax");
        }
    }
}
