using KGTMachineLearningWeb.Models.Chart;
using KGTMachineLearningWeb.Models.ChartConfiguration;
using KGTMachineLearningWeb.Models.Identity;
using KGTMachineLearningWeb.Models.Jobs;
using KGTMachineLearningWeb.Models.Workspace;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace KGTMachineLearningWeb.Context
{
    public class KGTContext : IdentityDbContext<ApplicationUser>
    {
        public KGTContext() : base("DefaultConnection")
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ChartObject> ChartObjects { get; set; }
        public DbSet<ChartDataSource> DataToProcess { get; set; }
        public DbSet<JobStatus> JobStatus { get; set; }
        public DbSet<ChartsConfiguration> ChartsConfigurations { get; set; }
        public DbSet<ProcessorConfiguration> ProcessorConfiguration { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Entity<IdentityUser>().ToTable("User");
            //modelBuilder.Entity<ApplicationUser>().ToTable("User");
            //modelBuilder.Entity<IdentityRole>().ToTable("Role");
            //modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            //modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasOptional(c => c.Owner)
                .WithMany(u => u.OwnedCategories)
                .HasForeignKey(c => c.OwnerId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChartObject>()
                .HasRequired(co => co.Thumbnail)
                .WithRequiredPrincipal(t => t.ChartObject)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChartObject>()
                .HasOptional(co => co.ChartDataSource)
                .WithRequired(ds => ds.ChartObject)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChartDataSource>()
                .HasMany(ds => ds.Series)
                .WithRequired(s => s.ChartDataSource)
                .WillCascadeOnDelete(true);
        }

        public static KGTContext Create()
        {
            return new KGTContext();
        }
    }
}
