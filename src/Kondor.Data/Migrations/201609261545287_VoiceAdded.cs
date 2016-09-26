namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoiceAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Voices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        VoiceData = c.Binary(),
                        ContentType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Voices");
        }
    }
}
