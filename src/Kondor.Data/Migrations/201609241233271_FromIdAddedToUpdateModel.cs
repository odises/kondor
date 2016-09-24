namespace Kondor.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FromIdAddedToUpdateModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Updates", "FromId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Updates", "FromId");
        }
    }
}
