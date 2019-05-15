namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Renaming : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SerieConfiguration", newName: "SeriesConfiguration");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.SeriesConfiguration", newName: "SerieConfiguration");
        }
    }
}
