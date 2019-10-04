namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCartType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "Type", c => c.String(maxLength: 64));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cart", "Type");
        }
    }
}
