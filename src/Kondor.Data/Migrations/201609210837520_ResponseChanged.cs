namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResponseChanged : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Responses", "TelegramUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Responses", "TelegramUserId", c => c.String());
        }
    }
}
