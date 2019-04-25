namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class configurablecharttyhpe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChartObject", "ChartType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChartObject", "ChartType");
        }
    }
}
