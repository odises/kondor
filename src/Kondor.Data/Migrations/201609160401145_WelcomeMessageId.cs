namespace Kondor.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class WelcomeMessageId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "WelcomeMessageId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "WelcomeMessageId");
        }
    }
}
