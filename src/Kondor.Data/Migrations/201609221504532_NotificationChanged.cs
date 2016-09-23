namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Notifications", "UserId");
            AddForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Notifications", "ChatId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "ChatId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropColumn("dbo.Notifications", "UserId");
        }
    }
}
