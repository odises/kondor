namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardModelsChanged02 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Examples", "MemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Examples", "MemId", c => c.Int(nullable: false));
        }
    }
}
