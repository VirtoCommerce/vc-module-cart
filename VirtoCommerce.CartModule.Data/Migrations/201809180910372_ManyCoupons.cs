namespace VirtoCommerce.CartModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ManyCoupons : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartCoupon",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Code = c.String(maxLength: 64),
                    ShoppingCartId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cart", t => t.ShoppingCartId, cascadeDelete: true)
                .Index(t => t.ShoppingCartId);


            Sql("INSERT INTO dbo.CartCoupon (Id, Code, ShoppingCartId) SELECT cast(LOWER(REPLACE( NEWID(), '-', '')) as nvarchar(128)), Coupon, Id FROM dbo.Cart WHERE Coupon IS NOT NULL");

            DropColumn("dbo.Cart", "Coupon");
        }

        public override void Down()
        {
            AddColumn("dbo.Cart", "Coupon", c => c.String(maxLength: 64));
            DropForeignKey("dbo.CartCoupon", "ShoppingCartId", "dbo.Cart");
            DropIndex("dbo.CartCoupon", new[] { "ShoppingCartId" });
            DropTable("dbo.CartCoupon");
        }
    }
}
