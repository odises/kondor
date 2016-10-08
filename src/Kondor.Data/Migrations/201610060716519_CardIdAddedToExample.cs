namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardIdAddedToExample : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Examples", "CardId", c => c.Int(nullable: false));
            CreateIndex("dbo.Examples", "CardId");
            AddForeignKey("dbo.Examples", "CardId", "dbo.Cards", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Examples", "CardId", "dbo.Cards");
            DropIndex("dbo.Examples", new[] { "CardId" });
            DropColumn("dbo.Examples", "CardId");
        }
    }
}
