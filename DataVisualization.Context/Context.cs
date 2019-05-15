using DataVisualization.Models.Chart;
using DataVisualization.Models.ChartConfiguration;
using DataVisualization.Models.Identity;
using DataVisualization.Models.Jobs;
using DataVisualization.Models.Workspace;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DataVisualization.Context
{
    public class VisContext : IdentityDbContext<ApplicationUser>
    {
        public VisContext() : base("DefaultConnection")
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
            modelBuilder.Entity<ApplicationUser>().ToTable("User");
            //modelBuilder.Entity<IdentityRole>().ToTable("Role");
            //modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            //modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

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

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ChartsConfigurations)
                .WithRequired(c => c.User)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Jobs)
                .WithRequired(j => j.UserCreator)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<JobStatus>()
                .HasRequired(j => j.ChartsConfig)
                .WithMany()
                .WillCascadeOnDelete(false);
        }

        public static VisContext Create()
        {
            return new VisContext();
        }
    }
}
