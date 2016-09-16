namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExampleViewAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExampleViews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExampleId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Views = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Examples", t => t.ExampleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ExampleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExampleViews", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExampleViews", "ExampleId", "dbo.Examples");
            DropIndex("dbo.ExampleViews", new[] { "UserId" });
            DropIndex("dbo.ExampleViews", new[] { "ExampleId" });
            DropTable("dbo.ExampleViews");
        }
    }
}
