using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SeleniumDotNet
{
    public class TestDbContext : DbContext
    {
        public TestDbContext()
        {
            
        }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<ReviewUrlDto> ReviewUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TestDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Remove this method.
            optionsBuilder.UseSqlServer(
              "Server=(localdb)\\MSSQLLocalDB;Database=wine-reviews;Trusted_Connection=True;");

            // optionsBuilder.UseSqlServer(
            //   "Server=tcp:mc-d-euw-id-sql.database.windows.net,1433;Initial Catalog=mc-d-euw-id-sdb2;Persist Security Info=False;User ID=MarelSuperUser;Password=Pui4un2mxpmyfux!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
    }
}
