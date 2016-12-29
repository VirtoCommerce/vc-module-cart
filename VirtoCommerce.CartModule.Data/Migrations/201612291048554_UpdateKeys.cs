namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKeys : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CartPayment", new[] { "ShoppingCartId" });
            DropIndex("dbo.CartLineItem", new[] { "ShoppingCartId" });
            AlterColumn("dbo.CartPayment", "ShoppingCartId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.CartLineItem", "ShoppingCartId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.CartPayment", "ShoppingCartId");
            CreateIndex("dbo.CartLineItem", "ShoppingCartId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CartLineItem", new[] { "ShoppingCartId" });
            DropIndex("dbo.CartPayment", new[] { "ShoppingCartId" });
            AlterColumn("dbo.CartLineItem", "ShoppingCartId", c => c.String(maxLength: 128));
            AlterColumn("dbo.CartPayment", "ShoppingCartId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CartLineItem", "ShoppingCartId");
            CreateIndex("dbo.CartPayment", "ShoppingCartId");
        }
    }
}
