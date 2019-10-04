namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentSelfPricesFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "PaymentTotal", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "PaymentTotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "Status", c => c.String(maxLength: 64));
            AddColumn("dbo.CartPayment", "TaxType", c => c.String(maxLength: 64));
            AddColumn("dbo.CartPayment", "Price", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartPayment", "PriceWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartPayment", "DiscountAmount", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartPayment", "DiscountAmountWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartPayment", "Total", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartPayment", "TotalWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartPayment", "TaxTotal", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartPayment", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CartDiscount", "PaymentId", c => c.String(maxLength: 128));
            AddColumn("dbo.CartTaxDetail", "PaymentId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CartDiscount", "PaymentId");
            CreateIndex("dbo.CartTaxDetail", "PaymentId");
            AddForeignKey("dbo.CartTaxDetail", "PaymentId", "dbo.CartPayment", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CartDiscount", "PaymentId", "dbo.CartPayment", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartDiscount", "PaymentId", "dbo.CartPayment");
            DropForeignKey("dbo.CartTaxDetail", "PaymentId", "dbo.CartPayment");
            DropIndex("dbo.CartTaxDetail", new[] { "PaymentId" });
            DropIndex("dbo.CartDiscount", new[] { "PaymentId" });
            DropColumn("dbo.CartTaxDetail", "PaymentId");
            DropColumn("dbo.CartDiscount", "PaymentId");
            DropColumn("dbo.CartPayment", "TaxPercentRate");
            DropColumn("dbo.CartPayment", "TaxTotal");
            DropColumn("dbo.CartPayment", "TotalWithTax");
            DropColumn("dbo.CartPayment", "Total");
            DropColumn("dbo.CartPayment", "DiscountAmountWithTax");
            DropColumn("dbo.CartPayment", "DiscountAmount");
            DropColumn("dbo.CartPayment", "PriceWithTax");
            DropColumn("dbo.CartPayment", "Price");
            DropColumn("dbo.CartPayment", "TaxType");
            DropColumn("dbo.Cart", "Status");
            DropColumn("dbo.Cart", "PaymentTotalWithTax");
            DropColumn("dbo.Cart", "PaymentTotal");
        }
    }
}
