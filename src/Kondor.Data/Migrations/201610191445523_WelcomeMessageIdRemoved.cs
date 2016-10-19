namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WelcomeMessageIdRemoved : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "WelcomeMessageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "WelcomeMessageId", c => c.Int(nullable: false));
        }
    }
}
