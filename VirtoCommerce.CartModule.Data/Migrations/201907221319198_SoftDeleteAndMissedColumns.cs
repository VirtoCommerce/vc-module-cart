namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SoftDeleteAndMissedColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.CartLineItem", "FulfillmentCenterId", c => c.String(maxLength: 64));
            AddColumn("dbo.CartLineItem", "FulfillmentCenterName", c => c.String(maxLength: 128));
            AddColumn("dbo.CartShipment", "FulfillmentCenterName", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartShipment", "FulfillmentCenterName");
            DropColumn("dbo.CartLineItem", "FulfillmentCenterName");
            DropColumn("dbo.CartLineItem", "FulfillmentCenterId");
            DropColumn("dbo.Cart", "IsDeleted");
        }
    }
}
