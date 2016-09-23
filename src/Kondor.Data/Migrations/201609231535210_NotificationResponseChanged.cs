namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationResponseChanged : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "UserId" });
            AddColumn("dbo.Notifications", "TelegramUserId", c => c.Int(nullable: false));
            AddColumn("dbo.Responses", "TelegramUserId", c => c.Int(nullable: false));
            DropColumn("dbo.Notifications", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "UserId", c => c.String(maxLength: 128));
            DropColumn("dbo.Responses", "TelegramUserId");
            DropColumn("dbo.Notifications", "TelegramUserId");
            CreateIndex("dbo.Notifications", "UserId");
            AddForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
