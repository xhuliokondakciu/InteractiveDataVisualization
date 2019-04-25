namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetothumbnail : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Thumbnail", "IsCreated");
            DropColumn("dbo.Thumbnail", "CreationError");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Thumbnail", "CreationError", c => c.String());
            AddColumn("dbo.Thumbnail", "IsCreated", c => c.Boolean(nullable: false));
        }
    }
}
