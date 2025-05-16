namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOriginalPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "OriginalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "OriginalPrice");
        }
    }
}
