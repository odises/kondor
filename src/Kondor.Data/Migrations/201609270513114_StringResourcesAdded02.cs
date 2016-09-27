namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StringResourcesAdded02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.Languages");
            DropIndex("dbo.AspNetUsers", new[] { "LanguageId" });
            AlterColumn("dbo.AspNetUsers", "LanguageId", c => c.Guid(nullable: false));
            CreateIndex("dbo.AspNetUsers", "LanguageId");
            AddForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.Languages", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.Languages");
            DropIndex("dbo.AspNetUsers", new[] { "LanguageId" });
            AlterColumn("dbo.AspNetUsers", "LanguageId", c => c.Guid());
            CreateIndex("dbo.AspNetUsers", "LanguageId");
            AddForeignKey("dbo.AspNetUsers", "LanguageId", "dbo.Languages", "Id");
        }
    }
}
