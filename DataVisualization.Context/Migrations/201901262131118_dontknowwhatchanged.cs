namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dontknowwhatchanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ChartsConfiguration", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.ChartsConfiguration", "ConfigurationXml", c => c.String(nullable: false, storeType: "xml"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChartsConfiguration", "ConfigurationXml", c => c.String(storeType: "xml"));
            AlterColumn("dbo.ChartsConfiguration", "Title", c => c.String());
        }
    }
}
