namespace KGTMachineLearningWeb.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dontknowwhy : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProcessorConfiguration", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.ProcessorConfiguration", "Path", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProcessorConfiguration", "Path", c => c.String());
            AlterColumn("dbo.ProcessorConfiguration", "Name", c => c.String());
        }
    }
}
