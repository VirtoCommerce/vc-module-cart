namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LongerEmailAddress : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CartAddress", "Email", c => c.String(maxLength: 254));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CartAddress", "Email", c => c.String(maxLength: 64));
        }
    }
}
