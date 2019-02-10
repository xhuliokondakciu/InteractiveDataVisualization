namespace KGTMachineLearningWeb.Context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MGChartsConfiguration : DbMigration
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
                        Title = c.String(),
                        ProcessorId = c.Int(nullable: false),
                        ConfigurationXml = c.String(storeType: "xml"),
                        UserId = c.Int(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessorConfiguration", t => t.ProcessorId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.ProcessorId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.ProcessorConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
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
                "dbo.Thumbnail",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Image = c.Binary(),
                        IsCreated = c.Boolean(nullable: false),
                        CreationError = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChartObject", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.JobStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserFileName = c.String(),
                        SystemFileName = c.String(),
                        ChartDataDirectory = c.String(),
                        Status = c.Int(nullable: false),
                        JobId = c.String(),
                        JobOutput = c.String(),
                        UserNotified = c.Boolean(nullable: false),
                        UserCreatorId = c.String(maxLength: 128),
                        StartTime = c.DateTimeOffset(precision: 7),
                        EndTime = c.DateTimeOffset(precision: 7),
                        JobFinishTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserCreatorId)
                .Index(t => t.UserCreatorId);
            
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
            DropForeignKey("dbo.JobStatus", "UserCreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChartDataSource", "JobStatus_Id", "dbo.JobStatus");
            DropForeignKey("dbo.Category", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Category", "ParentCategoryId", "dbo.Category");
            DropForeignKey("dbo.Thumbnail", "Id", "dbo.ChartObject");
            DropForeignKey("dbo.ChartObject", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChartsConfiguration", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChartsConfiguration", "ProcessorId", "dbo.ProcessorConfiguration");
            DropForeignKey("dbo.ChartDataSource", "Id", "dbo.ChartObject");
            DropForeignKey("dbo.SerieConfiguration", "ChartDataSource_Id", "dbo.ChartDataSource");
            DropForeignKey("dbo.ChartObject", "CategoryId", "dbo.Category");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.JobStatus", new[] { "UserCreatorId" });
            DropIndex("dbo.Thumbnail", new[] { "Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.ChartsConfiguration", new[] { "User_Id" });
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
            DropTable("dbo.JobStatus");
            DropTable("dbo.Thumbnail");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
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
