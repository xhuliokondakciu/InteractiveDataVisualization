namespace KGTMachineLearningWeb.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notrequiredprocessorid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChartsConfiguration", "ProcessorId", "dbo.ProcessorConfiguration");
            DropIndex("dbo.ChartsConfiguration", new[] { "ProcessorId" });
            AlterColumn("dbo.ChartsConfiguration", "ProcessorId", c => c.Int());
            CreateIndex("dbo.ChartsConfiguration", "ProcessorId");
            AddForeignKey("dbo.ChartsConfiguration", "ProcessorId", "dbo.ProcessorConfiguration", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChartsConfiguration", "ProcessorId", "dbo.ProcessorConfiguration");
            DropIndex("dbo.ChartsConfiguration", new[] { "ProcessorId" });
            AlterColumn("dbo.ChartsConfiguration", "ProcessorId", c => c.Int(nullable: false));
            CreateIndex("dbo.ChartsConfiguration", "ProcessorId");
            AddForeignKey("dbo.ChartsConfiguration", "ProcessorId", "dbo.ProcessorConfiguration", "Id", cascadeDelete: true);
        }
    }
}
