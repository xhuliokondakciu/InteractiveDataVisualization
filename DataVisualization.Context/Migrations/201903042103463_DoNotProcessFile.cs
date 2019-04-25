namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoNotProcessFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChartsConfiguration", "RequiresProcess", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChartsConfiguration", "RequiresProcess");
        }
    }
}
