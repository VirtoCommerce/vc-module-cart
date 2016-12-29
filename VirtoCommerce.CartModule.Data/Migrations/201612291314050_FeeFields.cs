namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeeFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "Fee", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Cart", "FeeWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "Fee", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartLineItem", "FeeWithTax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "Fee", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.CartShipment", "FeeWithTax", c => c.Decimal(nullable: false, storeType: "money"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartShipment", "FeeWithTax");
            DropColumn("dbo.CartShipment", "Fee");
            DropColumn("dbo.CartLineItem", "FeeWithTax");
            DropColumn("dbo.CartLineItem", "Fee");
            DropColumn("dbo.Cart", "FeeWithTax");
            DropColumn("dbo.Cart", "Fee");
        }
    }
}
