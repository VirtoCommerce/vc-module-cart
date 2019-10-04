namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaxPercentRateConversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));

            AlterColumn("dbo.CartLineItem ", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("dbo.CartShipment", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("dbo.CartPayment", "TaxPercentRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cart", "TaxPercentRate");
        }
    }
}
