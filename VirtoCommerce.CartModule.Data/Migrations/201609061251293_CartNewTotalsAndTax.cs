namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartNewTotalsAndTax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "SubTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "ShippingTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "HandlingTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "DiscountTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "DiscountAmount", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "DiscountAmountWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "ShippingPriceWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "DiscountTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "TotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartDiscount", "DiscountAmountWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "ListPriceWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "SalePriceWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "DiscountAmount", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "DiscountAmountWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            DropColumn("dbo.CartLineItem", "PlacedPrice");
            DropColumn("dbo.CartLineItem", "ExtendedPrice");
            DropColumn("dbo.CartLineItem", "DiscountTotal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CartLineItem", "DiscountTotal", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "ExtendedPrice", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "PlacedPrice", c => c.Decimal(nullable: false, storeType: "money"));
            DropColumn("dbo.CartLineItem", "DiscountAmountWithTax");
            DropColumn("dbo.CartLineItem", "DiscountAmount");
            DropColumn("dbo.CartLineItem", "SalePriceWithTax");
            DropColumn("dbo.CartLineItem", "ListPriceWithTax");
            DropColumn("dbo.CartDiscount", "DiscountAmountWithTax");
            DropColumn("dbo.CartShipment", "TotalWithTax");
            DropColumn("dbo.CartShipment", "DiscountTotalWithTax");
            DropColumn("dbo.CartShipment", "ShippingPriceWithTax");
            DropColumn("dbo.Cart", "DiscountAmountWithTax");
            DropColumn("dbo.Cart", "DiscountAmount");
            DropColumn("dbo.Cart", "DiscountTotalWithTax");
            DropColumn("dbo.Cart", "HandlingTotalWithTax");
            DropColumn("dbo.Cart", "ShippingTotalWithTax");
            DropColumn("dbo.Cart", "SubTotalWithTax");
        }
    }
}
