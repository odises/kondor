namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoiceRemoved : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Voices");
        }
        
        public override void Down()
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
    }
}
