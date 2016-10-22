namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeckEdited : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Decks", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Decks", new[] { "UserId" });
            AlterColumn("dbo.Decks", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Decks", "UserId");
            AddForeignKey("dbo.Decks", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Decks", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Decks", new[] { "UserId" });
            AlterColumn("dbo.Decks", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Decks", "UserId");
            AddForeignKey("dbo.Decks", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
