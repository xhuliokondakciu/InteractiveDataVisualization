namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changesuseridtostring : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ChartsConfiguration", new[] { "User_Id" });
            DropColumn("dbo.ChartsConfiguration", "UserId");
            RenameColumn(table: "dbo.ChartsConfiguration", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.ChartsConfiguration", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ChartsConfiguration", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ChartsConfiguration", new[] { "UserId" });
            AlterColumn("dbo.ChartsConfiguration", "UserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.ChartsConfiguration", name: "UserId", newName: "User_Id");
            AddColumn("dbo.ChartsConfiguration", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.ChartsConfiguration", "User_Id");
        }
    }
}
