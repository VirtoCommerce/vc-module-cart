namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartAddress", "Name", c => c.String(maxLength: 2048));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartAddress", "Name");
        }
    }
}
