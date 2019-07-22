namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewFulfilmentFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartLineItem", "FulfilmentCenterId", c => c.String(maxLength: 64));
            AddColumn("dbo.CartLineItem", "FulfillmentCenterName", c => c.String(maxLength: 128));
            AddColumn("dbo.CartShipment", "FulfillmentCenterName", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartShipment", "FulfillmentCenterName");
            DropColumn("dbo.CartLineItem", "FulfillmentCenterName");
            DropColumn("dbo.CartLineItem", "FulfilmentCenterId");
        }
    }
}
