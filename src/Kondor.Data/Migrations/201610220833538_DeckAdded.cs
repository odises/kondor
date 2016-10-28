namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeckAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Decks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        CreationDateTime = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SubDecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        CreationDateTime = c.DateTime(nullable: false),
                        DeckId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Decks", t => t.DeckId, cascadeDelete: true)
                .Index(t => t.DeckId);
            
            AddColumn("dbo.Cards", "DeckId", c => c.Int());
            AddColumn("dbo.Cards", "SubDeckId", c => c.Int());
            CreateIndex("dbo.Cards", "DeckId");
            CreateIndex("dbo.Cards", "SubDeckId");
            AddForeignKey("dbo.Cards", "DeckId", "dbo.Decks", "Id");
            AddForeignKey("dbo.Cards", "SubDeckId", "dbo.SubDecks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cards", "SubDeckId", "dbo.SubDecks");
            DropForeignKey("dbo.Decks", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SubDecks", "DeckId", "dbo.Decks");
            DropForeignKey("dbo.Cards", "DeckId", "dbo.Decks");
            DropIndex("dbo.SubDecks", new[] { "DeckId" });
            DropIndex("dbo.Decks", new[] { "UserId" });
            DropIndex("dbo.Cards", new[] { "SubDeckId" });
            DropIndex("dbo.Cards", new[] { "DeckId" });
            DropColumn("dbo.Cards", "SubDeckId");
            DropColumn("dbo.Cards", "DeckId");
            DropTable("dbo.SubDecks");
            DropTable("dbo.Decks");
        }
    }
}
