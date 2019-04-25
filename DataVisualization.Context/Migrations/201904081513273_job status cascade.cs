namespace DataVisualization.Context.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobstatuscascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.JobStatus", "UserCreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.JobStatus", "ChartsConfigId", "dbo.ChartsConfiguration");
            AddForeignKey("dbo.JobStatus", "UserCreatorId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.JobStatus", "ChartsConfigId", "dbo.ChartsConfiguration", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobStatus", "ChartsConfigId", "dbo.ChartsConfiguration");
            DropForeignKey("dbo.JobStatus", "UserCreatorId", "dbo.AspNetUsers");
            AddForeignKey("dbo.JobStatus", "ChartsConfigId", "dbo.ChartsConfiguration", "Id", cascadeDelete: true);
            AddForeignKey("dbo.JobStatus", "UserCreatorId", "dbo.AspNetUsers", "Id");
        }
    }
}
