namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hangfirevariablechange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobStatus", "HangfireJobId", c => c.String());
            DropColumn("dbo.JobStatus", "JobId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobStatus", "JobId", c => c.String());
            DropColumn("dbo.JobStatus", "HangfireJobId");
        }
    }
}
