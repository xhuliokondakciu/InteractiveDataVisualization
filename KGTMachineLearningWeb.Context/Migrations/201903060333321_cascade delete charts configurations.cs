namespace KGTMachineLearningWeb.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadedeletechartsconfigurations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChartsConfiguration", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ChartsConfiguration", new[] { "UserId" });
            AlterColumn("dbo.ChartsConfiguration", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ChartsConfiguration", "UserId");
            AddForeignKey("dbo.ChartsConfiguration", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChartsConfiguration", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ChartsConfiguration", new[] { "UserId" });
            AlterColumn("dbo.ChartsConfiguration", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ChartsConfiguration", "UserId");
            AddForeignKey("dbo.ChartsConfiguration", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
