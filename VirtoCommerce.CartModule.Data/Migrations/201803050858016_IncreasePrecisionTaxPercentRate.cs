namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreasePrecisionTaxPercentRate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cart", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("dbo.CartPayment", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("dbo.CartLineItem", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("dbo.CartShipment", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CartShipment", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CartLineItem", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CartPayment", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Cart", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
