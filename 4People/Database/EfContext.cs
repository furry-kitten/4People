using System.Threading.Tasks;
using _4People.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace _4People.Database
{
    public class EfContext : DbContext
    {
        public EfContext()
        {

            Database.Migrate();
            Database.EnsureCreated();
        }

        public EfContext(DbContextOptions<EfContext> options) : base(options)
        {
            Database.Migrate();
            Database.EnsureCreated();
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public static async Task<EfContext> CreateInstanceAsync()
        {
            EfContextFactory factory = new();
            var context = factory.CreateDbContext();
            return context;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Constants.CreatingString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                        .HasMany(company => company.Subdivisions)
                        .WithOne(subdivision => subdivision.Company)
                        .HasForeignKey(subdivision => subdivision.CompanyId);

            modelBuilder.Entity<Subdivision>()
                        .HasOne(subdivision => subdivision.Leader)
                        .WithOne(employee => employee.SubordinateUnit)
                        .HasForeignKey<Subdivision>(subdivision => subdivision.LeaderId);

            modelBuilder.Entity<Subdivision>()
                        .HasMany(subdivision => subdivision.Employees)
                        .WithOne(employee => employee.Subdivision)
                        .HasForeignKey(employee => employee.SubdivisionId);

            modelBuilder.Entity<Employee>()
                        .Property(employee => employee.Salary)
                        .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}