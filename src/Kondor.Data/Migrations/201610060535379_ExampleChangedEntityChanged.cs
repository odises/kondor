namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExampleChangedEntityChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.CardStates", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.StringResources", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.ExampleViews", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Examples", "ExampleUniqueId", c => c.Guid(nullable: false));
            AddColumn("dbo.Examples", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Media", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Notifications", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Responses", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Settings", "RowStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Updates", "RowStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Updates", "RowStatus");
            DropColumn("dbo.Settings", "RowStatus");
            DropColumn("dbo.Responses", "RowStatus");
            DropColumn("dbo.Notifications", "RowStatus");
            DropColumn("dbo.Media", "RowStatus");
            DropColumn("dbo.Examples", "RowStatus");
            DropColumn("dbo.Examples", "ExampleUniqueId");
            DropColumn("dbo.ExampleViews", "RowStatus");
            DropColumn("dbo.StringResources", "RowStatus");
            DropColumn("dbo.CardStates", "RowStatus");
            DropColumn("dbo.Cards", "RowStatus");
        }
    }
}
