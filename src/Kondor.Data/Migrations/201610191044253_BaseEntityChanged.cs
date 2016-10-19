namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseEntityChanged : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Cards", "RowStatus");
            DropColumn("dbo.CardStates", "RowStatus");
            DropColumn("dbo.StringResources", "RowStatus");
            DropColumn("dbo.ExampleViews", "RowStatus");
            DropColumn("dbo.Examples", "RowStatus");
            DropColumn("dbo.Media", "RowStatus");
            DropColumn("dbo.Notifications", "RowStatus");
            DropColumn("dbo.Responses", "RowStatus");
            DropColumn("dbo.Settings", "RowStatus");
            DropColumn("dbo.Updates", "RowStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Updates", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Settings", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Responses", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Notifications", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Media", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Examples", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.ExampleViews", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.StringResources", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.CardStates", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Cards", "RowStatus", c => c.Int(nullable: false));
        }
    }
}
