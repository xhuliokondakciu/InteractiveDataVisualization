namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chartsconfiginjobstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobStatus", "ChartsConfigId", c => c.Int(nullable: false));
            CreateIndex("dbo.JobStatus", "ChartsConfigId");
            AddForeignKey("dbo.JobStatus", "ChartsConfigId", "dbo.ChartsConfiguration", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobStatus", "ChartsConfigId", "dbo.ChartsConfiguration");
            DropIndex("dbo.JobStatus", new[] { "ChartsConfigId" });
            DropColumn("dbo.JobStatus", "ChartsConfigId");
        }
    }
}
