namespace DataVisualization.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        IsRoot = c.Boolean(nullable: false),
                        IsEveryones = c.Boolean(nullable: false),
                        ParentCategoryId = c.Int(),
                        OwnerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.ParentCategoryId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.ParentCategoryId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.ChartObject",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        CategoryId = c.Int(nullable: false),
                        OwnerId = c.String(maxLength: 128),
                        ChartType = c.Int(nullable: false),
                        Thumbnail = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.CategoryId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.ChartDataSource",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        TimeSerieFilePath = c.String(),
                        TimeSerieColumn = c.String(),
                        JobStatus_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChartObject", t => t.Id, cascadeDelete: true)
                .ForeignKey("dbo.JobStatus", t => t.JobStatus_Id)
                .Index(t => t.Id)
                .Index(t => t.JobStatus_Id);
            
            CreateTable(
                "dbo.SerieConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        FilePath = c.String(),
                        ColumnName = c.String(),
                        ChartDataSource_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChartDataSource", t => t.ChartDataSource_Id, cascadeDelete: true)
                .Index(t => t.ChartDataSource_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.ChartsConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        ProcessorId = c.Int(),
                        ConfigurationXml = c.String(nullable: false, storeType: "xml"),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IsSystem = c.Boolean(nullable: false),
                        RequiresProcess = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessorConfiguration", t => t.ProcessorId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProcessorId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ProcessorConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Path = c.String(nullable: false),
                        ExtraParameters = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.JobStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserFileName = c.String(),
                        SystemFileName = c.String(),
                        ChartDataDirectory = c.String(),
                        Status = c.Int(nullable: false),
                        HangfireJobId = c.String(),
                        JobOutput = c.String(),
                        UserNotified = c.Boolean(nullable: false),
                        ChartsConfigId = c.Int(nullable: false),
                        UserCreatorId = c.String(nullable: false, maxLength: 128),
                        StartTime = c.DateTimeOffset(precision: 7),
                        EndTime = c.DateTimeOffset(precision: 7),
                        JobFinishTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChartsConfiguration", t => t.ChartsConfigId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserCreatorId, cascadeDelete: true)
                .Index(t => t.ChartsConfigId)
                .Index(t => t.UserCreatorId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Category", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Category", "ParentCategoryId", "dbo.Category");
            DropForeignKey("dbo.ChartObject", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.JobStatus", "UserCreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.JobStatus", "ChartsConfigId", "dbo.ChartsConfiguration");
            DropForeignKey("dbo.ChartDataSource", "JobStatus_Id", "dbo.JobStatus");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChartsConfiguration", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChartsConfiguration", "ProcessorId", "dbo.ProcessorConfiguration");
            DropForeignKey("dbo.ChartDataSource", "Id", "dbo.ChartObject");
            DropForeignKey("dbo.SerieConfiguration", "ChartDataSource_Id", "dbo.ChartDataSource");
            DropForeignKey("dbo.ChartObject", "CategoryId", "dbo.Category");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.JobStatus", new[] { "UserCreatorId" });
            DropIndex("dbo.JobStatus", new[] { "ChartsConfigId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.ChartsConfiguration", new[] { "UserId" });
            DropIndex("dbo.ChartsConfiguration", new[] { "ProcessorId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.SerieConfiguration", new[] { "ChartDataSource_Id" });
            DropIndex("dbo.ChartDataSource", new[] { "JobStatus_Id" });
            DropIndex("dbo.ChartDataSource", new[] { "Id" });
            DropIndex("dbo.ChartObject", new[] { "OwnerId" });
            DropIndex("dbo.ChartObject", new[] { "CategoryId" });
            DropIndex("dbo.Category", new[] { "OwnerId" });
            DropIndex("dbo.Category", new[] { "ParentCategoryId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.JobStatus");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.ProcessorConfiguration");
            DropTable("dbo.ChartsConfiguration");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SerieConfiguration");
            DropTable("dbo.ChartDataSource");
            DropTable("dbo.ChartObject");
            DropTable("dbo.Category");
        }
    }
}
