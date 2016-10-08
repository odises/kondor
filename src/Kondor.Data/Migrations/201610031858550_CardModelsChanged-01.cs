namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardModelsChanged01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cards", "MemId", "dbo.Mems");
            DropForeignKey("dbo.Examples", "MemId", "dbo.Mems");
            DropForeignKey("dbo.Media", "MemId", "dbo.Mems");
            DropForeignKey("dbo.Mems", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Cards", new[] { "MemId" });
            DropIndex("dbo.Mems", new[] { "UserId" });
            DropIndex("dbo.Examples", new[] { "MemId" });
            DropIndex("dbo.Media", new[] { "MemId" });
            CreateTable(
                "dbo.CardStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CardPosition = c.Int(nullable: false),
                        CardId = c.Int(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false),
                        ExaminationDateTime = c.DateTime(nullable: false),
                        ModifiedDateTime = c.DateTime(),
                        UserId = c.String(maxLength: 128),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cards", t => t.CardId, cascadeDelete: true)
                .Index(t => t.CardId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Cards", "CardType", c => c.Int(nullable: false));
            AddColumn("dbo.Cards", "CardStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Cards", "CardData", c => c.String());
            DropColumn("dbo.Cards", "CardPosition");
            DropColumn("dbo.Cards", "MemId");
            DropColumn("dbo.Cards", "CreationDateTime");
            DropColumn("dbo.Cards", "ExaminationDateTime");
            DropColumn("dbo.Cards", "ModifiedDateTime");
            DropColumn("dbo.Cards", "Status");
            DropTable("dbo.Mems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Mems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        MemBody = c.String(),
                        Definition = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Cards", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Cards", "ModifiedDateTime", c => c.DateTime());
            AddColumn("dbo.Cards", "ExaminationDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Cards", "CreationDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Cards", "MemId", c => c.Int(nullable: false));
            AddColumn("dbo.Cards", "CardPosition", c => c.Int(nullable: false));
            DropForeignKey("dbo.CardStates", "CardId", "dbo.Cards");
            DropIndex("dbo.CardStates", new[] { "UserId" });
            DropIndex("dbo.CardStates", new[] { "CardId" });
            DropColumn("dbo.Cards", "CardData");
            DropColumn("dbo.Cards", "CardStatus");
            DropColumn("dbo.Cards", "CardType");
            DropTable("dbo.CardStates");
            CreateIndex("dbo.Media", "MemId");
            CreateIndex("dbo.Examples", "MemId");
            CreateIndex("dbo.Mems", "UserId");
            CreateIndex("dbo.Cards", "MemId");
            AddForeignKey("dbo.Mems", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Media", "MemId", "dbo.Mems", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Examples", "MemId", "dbo.Mems", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Cards", "MemId", "dbo.Mems", "Id", cascadeDelete: true);
        }
    }
}
