namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobstatuscascadenon : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.JobStatus", new[] { "UserCreatorId" });
            AlterColumn("dbo.JobStatus", "UserCreatorId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.JobStatus", "UserCreatorId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.JobStatus", new[] { "UserCreatorId" });
            AlterColumn("dbo.JobStatus", "UserCreatorId", c => c.String(maxLength: 128));
            CreateIndex("dbo.JobStatus", "UserCreatorId");
        }
    }
}
