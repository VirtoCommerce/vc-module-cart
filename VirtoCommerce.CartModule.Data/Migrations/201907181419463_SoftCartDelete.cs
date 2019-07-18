namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SoftCartDelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cart", "IsDeleted");
        }
    }
}
