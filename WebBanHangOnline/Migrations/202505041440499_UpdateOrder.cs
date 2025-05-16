namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Order", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.tb_Order", "IsPaid", c => c.Boolean(nullable: false));
            AddColumn("dbo.tb_Order", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_Order", "UserId");
            DropColumn("dbo.tb_Order", "IsPaid");
            DropColumn("dbo.tb_Order", "Status");
        }
    }
}
