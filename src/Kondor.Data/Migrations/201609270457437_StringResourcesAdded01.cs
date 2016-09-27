namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StringResourcesAdded01 : DbMigration
    {
        public override void Up()
        {
            var defaultLanguageId = Guid.NewGuid().ToString();

            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            // by hand
            Sql($"INSERT INTO [dbo].[Languages] ([Id], [Name]) VALUES ('{defaultLanguageId}','English')");

            CreateTable(
                "dbo.StringResources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupCode = c.String(),
                        Text = c.String(),
                        LanguageId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.LanguageId, cascadeDelete: true)
                .Index(t => t.LanguageId);
            
            AddColumn("dbo.AspNetUsers", "LanguageId", c => c.Guid());

            // by hand
            Sql($"UPDATE [dbo].[AspNetUsers] SET LanguageId = '{defaultLanguageId}' WHERE LanguageId IS NULL");

            CreateIndex("dbo.AspNetUsers", "LanguageId");
            AddForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.Languages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.StringResources", "LanguageId", "dbo.Languages");
            DropIndex("dbo.StringResources", new[] { "LanguageId" });
            DropIndex("dbo.AspNetUsers", new[] { "LanguageId" });
            DropColumn("dbo.AspNetUsers", "LanguageId");
            DropTable("dbo.StringResources");
            DropTable("dbo.Languages");
        }
    }
}
